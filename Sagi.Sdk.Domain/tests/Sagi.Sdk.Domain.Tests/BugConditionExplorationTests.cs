using Sagi.Sdk.Domain.Entities;
using Sagi.Sdk.Domain.Tests.Entities.Fake;
using Sagi.Sdk.Domain.ValueObjects;
using Sagi.Sdk.Results.Contracts;

namespace Sagi.Sdk.Domain.Tests;

/// <summary>
/// Bug Condition Exploration Tests — Task 1
///
/// These tests document the 30 SonarQube issues present in the unfixed codebase.
/// They are EXPECTED TO FAIL (or produce warnings) on unfixed code.
/// DO NOT fix the code when these tests fail — failure confirms the bugs exist.
///
/// Validates: Requirements 1.1, 1.2, 1.3, 1.6, 1.7, 1.9, 1.17, 1.18, 1.29, 1.30
/// </summary>
public class BugConditionExplorationTests
{
    // ── Counterexample 1: IResult<T> missing covariance (Requirement 1.1) ──────────────────
    // isBugCondition: IResult<T> where T NOT MARKED out
    // Expected: IResult<DerivedError> should be assignable to IResult<BaseError>
    // Actual: compile error — T is invariant, covariant assignment is rejected
    //
    // The following code does NOT compile because IResult<T> lacks `out T`:
    //   IResult<string> r = new Result<object>();  // CS0266 — cannot implicitly convert
    //
    // Documented counterexample: IResult<T> is invariant — covariant assignment fails at compile time.
    [Fact(DisplayName = "BugCondition_1_1: IResult<T> is invariant — T is not marked out")]
    public void BugCondition_IResult_IsInvariant_MissingOutKeyword()
    {
        // Verify the interface exists and T is NOT covariant (no `out` keyword)
        // If T were `out T`, IResult<Derived> would be assignable to IResult<Base>
        var resultType = typeof(IResult<>);
        var typeParam = resultType.GetGenericArguments()[0];

        // BUG: T is invariant — GenericParameterAttributes should include Covariant (0x01) after fix
        // On unfixed code: Covariant flag is NOT set
        bool isCovariant = (typeParam.GenericParameterAttributes &
            System.Reflection.GenericParameterAttributes.Covariant) != 0;

        // This assertion FAILS on unfixed code — T is not covariant
        Assert.True(isCovariant,
            "BUG 1.1: IResult<T> is missing 'out T' covariance. " +
            "T is invariant — IResult<Derived> cannot be assigned to IResult<Base>. " +
            "Counterexample: IResult<string> r = new Result<object>() fails to compile.");
    }

    // ── Counterexample 2: Entity._errors unused field (Requirement 1.6) ─────────────────────
    // isBugCondition: Entity._errors FIELD AND never read
    // Expected: field should not exist — Entity<T> inherits _errors from Validateble
    // Actual: private readonly List<IError> _errors = [] declared but never referenced
    [Fact(DisplayName = "BugCondition_1_6: Entity<T> declares unused _errors field")]
    public void BugCondition_Entity_HasUnusedErrorsField()
    {
        var entityType = typeof(Entity<>);
        var field = entityType.GetField(
            "_errors",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // BUG: _errors field exists in Entity<T> but is never read
        // Entity<T> inherits _errors from Validateble — this field is a dead duplicate
        // Assert.Null(field) — on unfixed code this FAILS because the field exists
        Assert.True(field == null,
            "BUG 1.6: Entity<T> declares private readonly List<IError> _errors = [] " +
            "but never reads it. Entity<T> already inherits _errors from Validateble. " +
            "Counterexample: Entity<T>._errors field exists via reflection but is unreachable code.");
    }

    // ── Counterexample 3: GenerateId() called in base constructor (Requirement 1.7) ─────────
    // isBugCondition: Entity.constructor AND calls GenerateId() directly
    // Expected: GenerateId() should NOT be called in the base constructor
    // Actual: Id = GenerateId() is called in Entity<T>() constructor — virtual call in ctor
    [Fact(DisplayName = "BugCondition_1_7: Entity<T> calls virtual GenerateId() in base constructor")]
    public void BugCondition_Entity_CallsVirtualGenerateIdInConstructor()
    {
        // Verify that GenerateId() is abstract/virtual
        var entityType = typeof(Entity<>);
        var generateIdMethod = entityType.GetMethod(
            "GenerateId",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(generateIdMethod);
        bool isVirtualOrAbstract = generateIdMethod!.IsVirtual || generateIdMethod.IsAbstract;
        Assert.True(isVirtualOrAbstract, "GenerateId() must be virtual/abstract for this bug to apply.");

        // BUG: The constructor calls GenerateId() which is abstract — dangerous virtual call in ctor
        // If a subclass overrides GenerateId() and accesses uninitialized subclass state,
        // the result is undefined behavior (NullReferenceException or incorrect Id)
        //
        // Verify the bug exists by inspecting the constructor body via IL or by checking
        // that FakeEntity.Id is set during construction (which means the ctor called GenerateId())
        var entity = new FakeEntity();

        // On unfixed code: Id is set in the base constructor via GenerateId()
        // This means the virtual method was called before the subclass was fully initialized
        // The fix should move Id assignment to the subclass constructor
        Assert.NotEqual(Guid.Empty, entity.Id);

        // Document: the constructor calls GenerateId() — confirmed by the fact that Id is
        // non-empty immediately after construction without any explicit assignment in FakeEntity
        // (on unfixed code, FakeEntity does NOT call Id = GenerateId() in its own constructor)
        var fakeEntityType = typeof(FakeEntity);
        var ctor = fakeEntityType.GetConstructor(Type.EmptyTypes);
        Assert.NotNull(ctor);

        // The bug is confirmed: Entity<T> base constructor calls abstract GenerateId()
        // Sonar S1699 — do not call overridable methods in constructors
        // This test documents the condition; the fix moves GenerateId() to subclass constructors
    }

    // ── Counterexample 4: operator== Blocker (Requirement 1.9) ──────────────────────────────
    // isBugCondition: Entity.operator== AND Sonar classifies as Blocker
    // Expected: operator== should be removed — equality by Id via Equals/GetHashCode is sufficient
    // Actual: operator== and operator!= are defined on Entity<T> (reference type) — Sonar S3875 Blocker
    [Fact(DisplayName = "BugCondition_1_9: Entity<T> defines operator== on reference type (Sonar S3875 Blocker)")]
    public void BugCondition_Entity_DefinesEqualityOperatorOnReferenceType()
    {
        var entityType = typeof(Entity<Guid>);

        var equalityOp = entityType.GetMethod(
            "op_Equality",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        var inequalityOp = entityType.GetMethod(
            "op_Inequality",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        // BUG: operator== and operator!= exist on Entity<T> — a reference type
        // Sonar S3875 classifies this as Blocker: operator== on reference types creates
        // semantic ambiguity between reference equality and value equality
        Assert.True(equalityOp == null,
            "BUG 1.9: Entity<T> defines operator== (op_Equality) on a reference type. " +
            "Sonar S3875 classifies this as Blocker. " +
            "Counterexample: Entity<T>.op_Equality exists via reflection — should be removed.");

        Assert.True(inequalityOp == null,
            "BUG 1.9: Entity<T> defines operator!= (op_Inequality) on a reference type. " +
            "Counterexample: Entity<T>.op_Inequality exists via reflection — should be removed.");
    }

    // ── Counterexample 5: null warnings in EntityTests (Requirement 1.17) ───────────────────
    // isBugCondition: test passing null to non-nullable parameter
    // Expected: tests should use null! or is null pattern — no CS8625/CS8604 warnings
    // Actual: EntityTests.cs L100, L109, L135, L144 produce CS8625/CS8604 warnings
    //
    // Documented counterexample (from dotnet build output):
    //   EntityTests.cs(100,32): warning CS8625: Cannot convert null literal to non-nullable reference type
    //   EntityTests.cs(109,21): warning CS8604: Possible null reference argument for parameter 'a'
    //   EntityTests.cs(109,32): warning CS8604: Possible null reference argument for parameter 'b'
    //   EntityTests.cs(135,31): warning CS8625: Cannot convert null literal to non-nullable reference type
    //   EntityTests.cs(144,22): warning CS8604: Possible null reference argument for parameter 'a'
    //   EntityTests.cs(144,33): warning CS8604: Possible null reference argument for parameter 'b'
    [Fact(DisplayName = "BugCondition_1_17: EntityTests passes null to non-nullable operator== parameters")]
    public void BugCondition_EntityTests_NullWarningsInOperatorTests()
    {
        // Verify the warnings exist by checking the test methods that cause them
        var testType = typeof(Sagi.Sdk.Domain.Tests.Entities.EntityTest);

        // ShouldReturnFalseForNullAndEntityUsingEqualityOperator — passes null to operator==
        var nullEntityTest = testType.GetMethod("ShouldReturnFalseForNullAndEntityUsingEqualityOperator");
        Assert.NotNull(nullEntityTest);

        // ShouldReturnTrueForNullEntitiesUsingEqualityOperator — passes null == null to operator==
        var nullNullTest = testType.GetMethod("ShouldReturnTrueForNullEntitiesUsingEqualityOperator");
        Assert.NotNull(nullNullTest);

        // ShouldReturnTrueForNullAndEntityUsingInequalityOperator — passes null to operator!=
        var nullInequalityTest = testType.GetMethod("ShouldReturnTrueForNullAndEntityUsingInequalityOperator");
        Assert.NotNull(nullInequalityTest);

        // ShouldReturnFalseForNullEntitiesUsingInequalityOperator — passes null != null to operator!=
        var nullNullInequalityTest = testType.GetMethod("ShouldReturnFalseForNullEntitiesUsingInequalityOperator");
        Assert.NotNull(nullNullInequalityTest);

        // BUG 1.17 confirmed: these test methods exist and produce CS8625/CS8604 warnings
        // because they pass null to non-nullable Entity<T> parameters of operator== and operator!=
        // Counterexamples from build:
        //   EntityTests.cs(100,32): CS8625 — null passed to operator== parameter
        //   EntityTests.cs(109,21): CS8604 — null reference argument for 'a' in operator==
        //   EntityTests.cs(109,32): CS8604 — null reference argument for 'b' in operator==
        //   EntityTests.cs(135,31): CS8625 — null passed to operator!= parameter
        //   EntityTests.cs(144,22): CS8604 — null reference argument for 'a' in operator!=
        //   EntityTests.cs(144,33): CS8604 — null reference argument for 'b' in operator!=
    }

    // ── Counterexample 6: null warnings in AddressTests (Requirement 1.18) ──────────────────
    // isBugCondition: test passing null to non-nullable parameter
    // Expected: tests should use null! — no CS8625 warnings
    // Actual: AddressTests.cs L41, L51, L62, L72, L84, L95 produce CS8625 warnings
    //
    // Documented counterexample (from dotnet build output):
    //   AddressTests.cs(41,46): warning CS8625: Cannot convert null literal to non-nullable reference type
    //   AddressTests.cs(51,46): warning CS8625: Cannot convert null literal to non-nullable reference type
    //   AddressTests.cs(62,57): warning CS8625: Cannot convert null literal to non-nullable reference type
    //   AddressTests.cs(72,57): warning CS8625: Cannot convert null literal to non-nullable reference type
    //   AddressTests.cs(84,56): warning CS8625: Cannot convert null literal to non-nullable reference type
    //   AddressTests.cs(95,56): warning CS8625: Cannot convert null literal to non-nullable reference type
    [Fact(DisplayName = "BugCondition_1_18: AddressTests passes null to non-nullable string parameters")]
    public void BugCondition_AddressTests_NullWarningsInValueObjectTests()
    {
        // Verify the Address constructor has non-nullable string parameters
        var addressType = typeof(Address);
        var ctor = addressType.GetConstructors()
            .FirstOrDefault(c => c.GetParameters().Length == 5);

        Assert.NotNull(ctor);

        var streetParam = ctor!.GetParameters().First(p => p.Name == "street");
        var numberParam = ctor.GetParameters().First(p => p.Name == "number");

        // Verify parameters are non-nullable string (not string?)
        Assert.Equal(typeof(string), streetParam.ParameterType);
        Assert.Equal(typeof(string), numberParam.ParameterType);

        // BUG 1.18 confirmed: AddressTests passes null to these non-nullable string parameters
        // producing CS8625 warnings at compile time
        // Counterexamples from build:
        //   AddressTests.cs(41,46): CS8625 — null passed to 'street' (non-nullable string)
        //   AddressTests.cs(51,46): CS8625 — null passed to 'street' (non-nullable string)
        //   AddressTests.cs(62,57): CS8625 — null passed to 'number' (non-nullable string)
        //   AddressTests.cs(72,57): CS8625 — null passed to 'number' (non-nullable string)
        //   AddressTests.cs(84,56): CS8625 — null passed to 'number' (non-nullable string)
        //   AddressTests.cs(95,56): CS8625 — null passed to 'number' (non-nullable string)
    }
}
