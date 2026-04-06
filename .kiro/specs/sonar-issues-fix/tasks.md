# Implementation Plan

- [x] 1. Write bug condition exploration test
  - **Property 1: Bug Condition** - 30 Sonar Issues Present in Unfixed Code
  - **CRITICAL**: This test MUST FAIL on unfixed code - failure confirms the bugs exist
  - **DO NOT attempt to fix the test or the code when it fails**
  - **GOAL**: Surface counterexamples that demonstrate the issues exist
  - **Scoped PBT Approach**: For deterministic issues, scope to concrete failing cases
  - Run `dotnet build /warnaserror` — expect failures on CS8625 (null para não-nullable) e CS0649 (campo não utilizado)
  - Attempt covariant assignment `IResult<string> r = new Result<object>()` — expect compile error (no `out T`)
  - Run `dotnet test` on `DynamoDbConfiguratorTests` — expect `InvalidOperationException` on `[Theory]` without parameters
  - Document counterexamples: CS8625 in EntityTests/AddressTests/PageResultTests, compile error on covariance, xUnit exception on Theory
  - Mark task complete when build failures and test failures are documented
  - _Requirements: 1.1, 1.2, 1.3, 1.6, 1.7, 1.9, 1.17, 1.18, 1.29, 1.30_

- [x] 2. Write preservation property tests (BEFORE implementing fix)
  - **Property 2: Preservation** - Existing Test Suite Passes on Unfixed Code
  - **IMPORTANT**: Follow observation-first methodology
  - Run `dotnet test` on the full solution and record all passing tests as baseline
  - Observe: all existing unit and integration tests pass on unfixed code
  - Write property-based tests (or confirm existing ones) for: `Cnpj`/`Cpf`/`Phone`/`Email` validation produces same results regardless of Regex implementation; `Entity<T>` equality by Id is symmetric and transitive; `Address` validation is consistent with any length config
  - Verify all preservation tests PASS on unfixed code
  - Mark task complete when baseline is recorded and preservation tests pass
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9, 3.10, 3.11, 3.12, 3.13, 3.14, 3.15, 3.16_

---

## Sagi.Sdk.Results

- [x] 3. Fix Sagi.Sdk.Results issues

  - [x] 3.1 Add covariance `out T` to `IResult<T>`
    - File: `Sagi.Sdk.Results/src/Sagi.Sdk.Results/Contracts/IResult.cs`
    - Change `IResult<T>` to `IResult<out T>`
    - `T` only appears in output position (`T? Value { get; }`) — safe for covariance
    - _Bug_Condition: isBugCondition(IResult<T>) where T NOT MARKED out (1.1)_
    - _Expected_Behavior: IResult<Derived> assignable to IResult<Base>_
    - _Preservation: Value, IsSuccess, IsFailure, Errors semantics unchanged_
    - _Requirements: 2.1, 3.1_

  - [x] 3.2 Remove explicit caller information from `Error` constructor
    - File: `Sagi.Sdk.Results/src/Sagi.Sdk.Results/Error.cs`
    - Change `ArgumentException.ThrowIfNullOrEmpty(code, nameof(code))` → `ArgumentException.ThrowIfNullOrEmpty(code)`
    - Change `ArgumentException.ThrowIfNullOrEmpty(message, nameof(message))` → `ArgumentException.ThrowIfNullOrEmpty(message)`
    - _Bug_Condition: isBugCondition(ArgumentException.ThrowIfNullOrEmpty(x, nameof(x))) (1.2)_
    - _Requirements: 2.2, 3.2_

  - [x] 3.3 Remove boolean literal from `Result<T>.IsSuccess`
    - File: `Sagi.Sdk.Results/src/Sagi.Sdk.Results/Result.cs`
    - Change `Errors.Any() == false` → `!Errors.Any()`
    - _Bug_Condition: isBugCondition("Errors.Any() == false") (1.3)_
    - _Requirements: 2.3, 3.3_

---

## Sagi.Sdk.Domain — Contratos e Entidades

- [x] 4. Fix `Event<T>` constructor visibility
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Contracts/Event.cs`
  - Change constructor visibility from `public` to `protected`
  - Abstract class constructors must be `protected` — cannot be instantiated directly
  - _Bug_Condition: isBugCondition(Event<T>.constructor AND visibility IS public) (1.4)_
  - _Expected_Behavior: only subclasses can call the constructor_
  - _Preservation: subclasses continue initializing Timestamp, AggregateId, AggregateVersion, Name correctly_
  - _Requirements: 2.4, 3.4_

- [x] 5. Remove explicit caller information from `Validateble`
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Contracts/Validateble.cs`
  - Change `ArgumentNullException.ThrowIfNull(error, nameof(error))` → `ArgumentNullException.ThrowIfNull(error)`
  - Change `ArgumentNullException.ThrowIfNull(errors, nameof(errors))` → `ArgumentNullException.ThrowIfNull(errors)`
  - _Bug_Condition: isBugCondition(ArgumentNullException.ThrowIfNull(x, nameof(x))) (1.5)_
  - _Requirements: 2.5, 3.5_

- [x] 6. Fix `Entity<T>` — remove unused `_errors` field
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Entities/Entity.cs`
  - Remove `private readonly List<IError> _errors = [];` — `Entity<T>` inherits `_errors` from `Validateble`
  - _Bug_Condition: isBugCondition(Entity._errors FIELD AND never read) (1.6)_
  - _Requirements: 2.6, 3.6_

- [x] 7. Fix `Entity<T>` — move `GenerateId()` out of base constructor
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Entities/Entity.cs`
  - Remove `Id = GenerateId()` from the base constructor
  - Change `public T Id { get; private set; }` to `public T Id { get; protected set; }`
  - File: `Sagi.Sdk.Domain/tests/Sagi.Sdk.Domain.Tests/Entities/Fake/FakeEntity.cs`
  - Add `Id = GenerateId();` to `FakeEntity` constructor (and any other concrete entity in tests)
  - _Bug_Condition: isBugCondition(Entity.constructor AND calls GenerateId() directly) (1.7)_
  - _Expected_Behavior: subclass assigns Id in its own constructor after full initialization_
  - _Preservation: Entity<T> continues generating unique Id, CreateAt, Active=true, Events, Version_
  - _Requirements: 2.7, 3.6_

- [x] 8. Verify and fix redundant `Active = true` initializer in `Entity<T>`
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Entities/Entity.cs`
  - Check if `Active` has a field initializer (`= true`) in addition to the constructor assignment
  - If a duplicate field initializer exists, remove it — keep only the constructor assignment
  - _Bug_Condition: isBugCondition("Active = true" AS field initializer AND also set in ctor) (1.8)_
  - _Requirements: 2.8_

- [x] 9. Remove `operator ==` and `operator !=` from `Entity<T>`
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Entities/Entity.cs`
  - Remove `public static bool operator ==(Entity<T> a, Entity<T> b)` and `operator !=`
  - Equality by Id is already implemented via `Equals(object?)` and `GetHashCode()`
  - File: `Sagi.Sdk.Domain/tests/Sagi.Sdk.Domain.Tests/Entities/EntityTests.cs`
  - Refactor tests that use `entity1 == entity2` → `entity1.Equals(entity2)` or `Assert.Equal(entity1, entity2)`
  - Refactor tests that use `entity == null` → `entity is null` or `Assert.Null(entity)`
  - Refactor tests that use `null == null` via operator → `Assert.True(entity1 is null && entity2 is null)`
  - _Bug_Condition: isBugCondition(Entity.operator== AND Sonar classifies as Blocker) (1.9)_
  - _Preservation: equality by Id semantics preserved via Equals/GetHashCode_
  - _Requirements: 2.9, 3.7_

---

## Sagi.Sdk.Domain — Value Objects

- [x] 10. Refactor `Address` constructor to max 7 parameters
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Address.cs`
  - Create `AddressLengthOptions` record (same file or `AddressLengthOptions.cs`):
    ```csharp
    public record AddressLengthOptions(
        short StreetMinLength = 2,
        short StreetMaxLength = 80,
        short NumberMinLength = 1,
        short NumberMaxLength = 10);
    ```
  - Replace the 9-parameter constructor with a 6-parameter constructor accepting `AddressLengthOptions options`
  - Update the 5-parameter constructor to delegate to the 6-parameter one with `new AddressLengthOptions()`
  - File: `Sagi.Sdk.Domain/tests/Sagi.Sdk.Domain.Tests/ValueObjects/AddressTests.cs`
  - Update any test that uses the 9-parameter constructor to use `AddressLengthOptions` instead
  - _Bug_Condition: isBugCondition(Address.constructor AND parameterCount > 7) (1.10)_
  - _Preservation: Address continues validating Street, Number, Complement, Neighborhood, ZipCode_
  - _Requirements: 2.10, 3.8_

- [x] 11. Migrate `Cnpj` regex to `[GeneratedRegex]`
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Cnpj.cs`
  - Declare class as `partial`
  - Replace the 3 `static readonly Regex` fields with `[GeneratedRegex]` partial methods:
    - `NumericRawRegex()` for `@"^\d{14}$"`
    - `AlphanumericRawRegex()` for `@"^[A-Z0-9]{14}$"`
    - `FormattedMaskRegex()` for the formatted mask pattern
    - `FormattedSeparatorsRegex()` for `@"[.\-/]"` (used in `Normalize`)
  - Update all usages of `s_numericRaw`, `s_alphanumericRaw`, `s_formattedMask` to call the generated methods
  - Replace `Regex.Replace(value, @"[.\-/]", string.Empty)` in `Normalize` with `FormattedSeparatorsRegex().Replace(value, string.Empty)`
  - _Bug_Condition: isBugCondition(Cnpj AND uses new Regex(...) at runtime) (1.11)_
  - _Preservation: Cnpj validation, formatting, TryParse, alphanumeric (IN RFB 2.229/2024) all produce same results_
  - _Requirements: 2.11, 3.9, 3.10_

- [x] 12. Migrate `Cpf` regex to `[GeneratedRegex]`
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Cpf.cs`
  - Declare class as `partial`
  - Add `[GeneratedRegex(@"^\d{11}$")] private static partial Regex ElevenDigitsRegex();`
  - Replace `Regex.IsMatch(Number, @"^\d{11}$")` with `ElevenDigitsRegex().IsMatch(Number)`
  - _Bug_Condition: isBugCondition(Cpf AND uses Regex.IsMatch(...) at runtime) (1.12)_
  - _Preservation: Cpf validation, formatting, TryParse produce same results_
  - _Requirements: 2.12, 3.9_

- [x] 13. Fix `Email` — remove redundant null checks and boolean literal
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Email.cs`
  - Declare class as `partial`
  - In `GetHost()`, `GetUser()`, `GetDomain()`, `GetTopLevenDomain()`: remove `?.` on `data` (string.Split never returns null)
  - Remove any null checks on `Address` or `Host` that the compiler already knows are non-null
  - Change `if (isMatch is false)` → `if (!isMatch)` in `ValidateAddressPattern` (issue 1.14)
  - Migrate the inline regex in `ValidateAddressPattern` to `[GeneratedRegex]`:
    ```csharp
    [GeneratedRegex(@"^[a-zA-Z0-9_.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailPatternRegex();
    ```
  - Replace `Regex.IsMatch(Address, pattern, RegexOptions.IgnoreCase)` with `EmailPatternRegex().IsMatch(Address)`
  - _Bug_Condition: isBugCondition(Email AND redundant null checks) (1.13) AND isBugCondition("isMatch is false") (1.14)_
  - _Preservation: Email validation, Address/User/Host/Domain/TopLevelDomain properties unchanged_
  - _Requirements: 2.13, 2.14, 3.9_

- [x] 14. Migrate `Phone` regex to `[GeneratedRegex]`
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Phone.cs`
  - Declare class as `partial`
  - Add three `[GeneratedRegex]` partial methods:
    - `DdiPatternRegex()` for `@"^\+\d{1,4}$"`
    - `PhonePartsRegex()` for `@"^(?<ddi>\d{2})(?<ddd>\d{2})(?<number>\d{8,9})$"`
    - `NonDigitsRegex()` for `@"[^\d]"`
  - Replace `Regex.IsMatch(DDI, ...)` with `DdiPatternRegex().IsMatch(DDI)`
  - Replace `Regex.Match(value, ...)` with `PhonePartsRegex().Match(value)`
  - Replace `new Regex(@"[^\d]", RegexOptions.Compiled)` in `OnlyDigits` with `NonDigitsRegex()`
  - _Bug_Condition: isBugCondition(Phone AND uses new Regex(...) at runtime) (1.15)_
  - _Preservation: Phone validation, formatting, TryParse, DDI/DDD/Number extraction unchanged_
  - _Requirements: 2.15, 3.9_

- [x] 15. Fix `ZipCode` — remove redundant null check
  - File: `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/ZipCode.cs`
  - Inspect `Validate()` for any `?.` or null check on `Value` (non-nullable `string`)
  - Remove any redundant null guard that the compiler already guarantees
  - _Bug_Condition: isBugCondition(ZipCode AND redundant null check on non-nullable) (1.16)_
  - _Preservation: ZipCode validation and Formatted property unchanged_
  - _Requirements: 2.16, 3.9_

---

## Sagi.Sdk.Domain — Testes

- [x] 16. Fix null warnings in `EntityTests` (operator == removal follow-up)
  - File: `Sagi.Sdk.Domain/tests/Sagi.Sdk.Domain.Tests/Entities/EntityTests.cs`
  - Replace all `entity == null` / `null == entity` patterns with `entity is null` or `Assert.Null`
  - Replace `entity1 == entity2` with `Assert.Equal(entity1, entity2)` or `entity1.Equals(entity2)`
  - For tests that passed `null!` to operator parameters (L100, L109, L135, L144): refactor to use `is null` pattern
  - Ensure all entity equality tests still verify Id-based equality semantics
  - _Bug_Condition: isBugCondition(test passing null to non-nullable parameter) (1.17)_
  - _Requirements: 2.17, 3.7_

- [x] 17. Fix null warnings in value object tests
  - Files: `AddressTests.cs`, `CityTests.cs`, `EmailTests.cs`, `NameTests.cs`, `NeighborhoodTests.cs`, `PhoneTests.cs`, `StateTests.cs`, `ZipCodeTests.cs`
  - All in `Sagi.Sdk.Domain/tests/Sagi.Sdk.Domain.Tests/ValueObjects/`
  - Replace `null` with `null!` (null-forgiving operator) wherever passed to non-nullable `string` constructor parameters
  - Use `string.Empty` where semantically more appropriate (e.g., testing empty string behavior)
  - Also update `AddressTests.cs` to use `AddressLengthOptions` if it uses the 9-parameter constructor (see task 10)
  - _Bug_Condition: isBugCondition(test passing null to non-nullable parameter) (1.18)_
  - _Requirements: 2.18_

---

## Sagi.Sdk.MongoDb

- [x] 18. Fix `MongoContext` constructor visibility
  - File: `Sagi.Sdk.MongoDb/src/Sagi.Sdk.MongoDb/Context/MongoContext.cs`
  - Change constructor visibility from `public` to `protected`
  - _Bug_Condition: isBugCondition(MongoContext.constructor AND visibility IS public) (1.19)_
  - _Preservation: MongoContext<T> continues working for all subclasses; all CRUD operations unchanged_
  - _Requirements: 2.19, 3.11_

- [x] 19. Remove explicit caller information from `ServicesExtensions`
  - File: `Sagi.Sdk.MongoDb/src/Sagi.Sdk.MongoDb/Extensions/ServicesExtensions.cs`
  - Change `ArgumentNullException.ThrowIfNull(options.ConnectionString, nameof(options.ConnectionString))` → `ArgumentNullException.ThrowIfNull(options.ConnectionString)`
  - Change `ArgumentNullException.ThrowIfNull(options.DatabaseName, nameof(options.DatabaseName))` → `ArgumentNullException.ThrowIfNull(options.DatabaseName)`
  - _Bug_Condition: isBugCondition(ServicesExtensions AND passes nameof() to ThrowIfNull) (1.20)_
  - _Requirements: 2.20, 3.12_

- [x] 20. Add `GC.SuppressFinalize` to `MongoDockerContainer.Dispose`
  - File: `Sagi.Sdk.MongoDb/tests/Sagi.Sdk.MongoDb.Tests/Fixtures/Docker/MongoDockerContainer.cs`
  - Expand the expression-bodied `Dispose()` to a block body
  - Add `GC.SuppressFinalize(this)` as the last statement
  - _Bug_Condition: isBugCondition(MongoDockerContainer.Dispose AND missing GC.SuppressFinalize) (1.21)_
  - _Requirements: 2.21_

---

## Sagi.Sdk.AWS.DynamoDb — Código Fonte

- [x] 21. Declare `DynamoDbHosting` as `static class`
  - File: `Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/Hosting/DynamoDbHosting.cs`
  - Change `public class DynamoDbHosting` → `public static class DynamoDbHosting`
  - All members are already static — no other changes needed
  - Remove `[ExcludeFromCodeCoverage]` only if it causes issues with static class; otherwise keep it
  - _Bug_Condition: isBugCondition(DynamoDbHosting AND not static AND no protected ctor) (1.22)_
  - _Preservation: DynamoDbHosting.RunAsync, StopAsync, Provider continue working_
  - _Requirements: 2.22_

- [x] 22. Remove boolean literal from `TablesInitializer`
  - File: `Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/Initializers/TablesInitializer.cs`
  - Locate the boolean literal comparison (e.g., `== true` or `== false`) in the file
  - In `CreateIfNotExistAsync`: change `(await TableExistAsync(request.TableName)) == false` → `!await TableExistAsync(request.TableName)`
  - In `AddTable`: `tables.Length > 0` is already correct — verify no `== true` suffix exists
  - _Bug_Condition: isBugCondition("tables.Length > 0 == true" boolean literal) (1.23)_
  - _Requirements: 2.23_

- [x] 23. Rename `DynamoDbTableEventsArgs` to `DynamoDbTableReadyEventArgs`
  - File: `Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/Initializers/TablesInitializer.cs`
  - Rename class `DynamoDbTableEventsArgs` → `DynamoDbTableReadyEventArgs`
  - Update the delegate: `DynamoDbTableEventHandler(object sender, DynamoDbTableReadyEventArgs e)`
  - Update all usages in `TablesInitializer` (`CreateIfNotExistAsync`, `OnReadyTable`)
  - Search for any other files referencing `DynamoDbTableEventsArgs` and update them
  - _Bug_Condition: isBugCondition(DynamoDbTableEventsArgs AND name NOT ending in EventArgs) (1.24)_
  - _Preservation: TablesInitializer continues creating tables and firing the ready event_
  - _Requirements: 2.24, 3.14_

- [x] 24. Add namespace to `Payment` sample class
  - File: `Sagi.Sdk.AWS.DynamoDb/src/Samples/Entities/Payment.cs`
  - Add `namespace Samples.Entities;` at the top of the file
  - _Bug_Condition: isBugCondition(Payment CLASS AND no namespace) (1.25)_
  - _Requirements: 2.25_

- [x] 25. Move `AwsOptions` record to its own file with namespace
  - File: `Sagi.Sdk.AWS.DynamoDb/src/Samples/Program.cs`
  - Remove the inline `record AwsOptions(...)` declaration from `Program.cs`
  - Create new file: `Sagi.Sdk.AWS.DynamoDb/src/Samples/AwsOptions.cs`
  - Content: `namespace Samples; public record AwsOptions(string Accesskey, string SecretKey, string ServiceUrl);`
  - _Bug_Condition: isBugCondition(AwsOptions RECORD AND no namespace) (1.26)_
  - _Requirements: 2.26_

- [x] 26. Remove empty statement from `PaymentTable`
  - File: `Sagi.Sdk.AWS.DynamoDb/src/Samples/Tables/PaymentTable.cs`
  - Find `BillingMode = BillingMode.PAY_PER_REQUEST;;` and remove the extra semicolon
  - _Bug_Condition: isBugCondition(PaymentTable AND has empty statement ";;") (1.27)_
  - _Requirements: 2.27_

---

## Sagi.Sdk.AWS.DynamoDb — Testes

- [x] 27. Add `GC.SuppressFinalize` to `DynamoDbDockerContainer.Dispose`
  - File: `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/Fixtures/Docker/DynamoDbDockerContainer.cs`
  - Expand the expression-bodied `Dispose()` to a block body
  - Add `GC.SuppressFinalize(this)` as the last statement
  - _Bug_Condition: isBugCondition(DynamoDbDockerContainer.Dispose AND missing GC.SuppressFinalize) (1.28)_
  - _Requirements: 2.28_

- [x] 28. Fix `[Theory]` without parameters in `DynamoDbConfiguratorTests`
  - File: `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/UnitTests/Config/DynamoDbConfiguratorTests.cs`
  - Identify the `[Theory]` method that has no parameters (not `ShouldValidateConstructorParameters` which has `GuardClauseAssertion assertion`)
  - Change `[Theory]` → `[Fact]` on the parameterless method
  - _Bug_Condition: isBugCondition([Theory] method AND has no parameters) (1.29)_
  - _Requirements: 2.29_

- [x] 29. Fix null for non-nullable `string` in `PageResultTests`
  - File: `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/UnitTests/Pages/PageResultTests.cs`
  - Find `[InlineData(null)]` where `token` parameter is `string` (non-nullable)
  - Change the parameter type to `string?` to accept null without CS8625 warning
  - _Bug_Condition: isBugCondition(PageResultTests AND passes null to string non-nullable) (1.30)_
  - _Requirements: 2.30_

---

## Validação Final

- [x] 30. Fix verification — run exploration test (now expected to pass)
  - **Property 1: Expected Behavior** - All 30 Sonar Issues Eliminated
  - **IMPORTANT**: Re-run the SAME checks from task 1 - do NOT write new tests
  - Run `dotnet build /warnaserror` — **EXPECTED OUTCOME**: build succeeds with no CS warnings
  - Verify covariant assignment `IResult<string> r = new Result<string>("x")` compiles and `r.Value == "x"`
  - Run `DynamoDbConfiguratorTests` — **EXPECTED OUTCOME**: no `InvalidOperationException` on Theory
  - Run Sonar analysis — **EXPECTED OUTCOME**: 0 open issues for the 30 identified rules
  - _Requirements: 2.1–2.30_

- [x] 31. Preservation verification — run preservation tests
  - **Property 2: Preservation** - No Regressions in Existing Test Suite
  - **IMPORTANT**: Re-run the SAME tests from task 2 - do NOT write new tests
  - Run `dotnet test` on the full solution
  - **EXPECTED OUTCOME**: all tests pass, same count as baseline recorded in task 2
  - Confirm integration tests for MongoDb and DynamoDb pass (requires Docker)
  - _Requirements: 3.1–3.16_

- [x] 32. Checkpoint — all tests pass
  - Ensure `dotnet build` succeeds with no warnings
  - Ensure `dotnet test` passes completely
  - Confirm no new Sonar issues were introduced by the fixes
  - Ask the user if any questions arise
