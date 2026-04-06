using Sagi.Sdk.Domain.Tests.Entities.Fake;
using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests;

/// <summary>
/// Preservation Property Tests — Task 2
///
/// These tests document the behavior that MUST be preserved after all Sonar fixes are applied.
/// They MUST PASS on unfixed code (they test existing correct behavior).
/// They MUST ALSO PASS on fixed code (they verify no regressions were introduced).
///
/// Validates: Requirements 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9, 3.10, 3.11, 3.12,
///            3.13, 3.14, 3.15, 3.16
/// </summary>
public class PreservationPropertyTests
{
    // ── Property: Cnpj validation produces consistent results (Requirements 3.9, 3.10) ────────
    //
    // For any CNPJ string, calling Validate() twice on the same instance produces the same result.
    // The regex implementation (runtime vs GeneratedRegex) must not change the outcome.

    [Theory(DisplayName = "Preservation_3_9: Cnpj validation is idempotent for valid numeric CNPJs")]
    [InlineData("45723174000110")]
    [InlineData("78.580.638/0001-36")]
    [InlineData("50.880.859/0001-00")]
    [InlineData("92.372.614/0001-12")]
    [InlineData("32.916.798/0001-02")]
    [InlineData("55.938.308/0001-74")]
    public void Preservation_Cnpj_ValidNumeric_IsIdempotent(string input)
    {
        var cnpj = new Cnpj(input);

        cnpj.Validate();
        bool firstResult = cnpj.IsValid;

        cnpj.Validate();
        bool secondResult = cnpj.IsValid;

        Assert.True(firstResult, $"Cnpj '{input}' should be valid.");
        Assert.Equal(firstResult, secondResult);
    }

    [Theory(DisplayName = "Preservation_3_9: Cnpj validation is idempotent for invalid CNPJs")]
    [InlineData("12345678901234")]
    [InlineData("00000000000000")]
    [InlineData("11111111111111")]
    [InlineData("")]
    [InlineData("1234")]
    public void Preservation_Cnpj_InvalidInput_IsIdempotent(string input)
    {
        var cnpj = new Cnpj(input);

        cnpj.Validate();
        bool firstResult = cnpj.IsInvalid;

        cnpj.Validate();
        bool secondResult = cnpj.IsInvalid;

        Assert.True(firstResult, $"Cnpj '{input}' should be invalid.");
        Assert.Equal(firstResult, secondResult);
    }

    [Theory(DisplayName = "Preservation_3_10: Cnpj alphanumeric validation is idempotent (IN RFB 2.229/2024)")]
    [InlineData("7Y.SLA.A4X/0001-57")]
    [InlineData("N1.H5P.GAX/0001-98")]
    [InlineData("3B.46L.6G8/0001-51")]
    [InlineData("ZL.YEK.4PN/0001-93")]
    [InlineData("6P.710.14N/0001-89")]
    public void Preservation_Cnpj_ValidAlphanumeric_IsIdempotent(string input)
    {
        var cnpj = new Cnpj(input);

        cnpj.Validate();
        bool firstResult = cnpj.IsValid;

        cnpj.Validate();
        bool secondResult = cnpj.IsValid;

        Assert.True(firstResult, $"Alphanumeric Cnpj '{input}' should be valid.");
        Assert.Equal(firstResult, secondResult);
        Assert.True(cnpj.IsAlphanumeric);
    }

    [Theory(DisplayName = "Preservation_3_9: Cnpj formatted output is stable across calls")]
    [InlineData("45723174000110", "45.723.174/0001-10")]
    [InlineData("78580638000136", "78.580.638/0001-36")]
    public void Preservation_Cnpj_FormattedOutput_IsStable(string raw, string expectedFormatted)
    {
        var cnpj = new Cnpj(raw);

        Assert.Equal(expectedFormatted, cnpj.Formatted);
        Assert.Equal(expectedFormatted, cnpj.Formatted); // second call — same result
    }

    // ── Property: Cpf validation produces consistent results (Requirement 3.9) ───────────────
    //
    // For any CPF string, calling Validate() twice on the same instance produces the same result.

    [Theory(DisplayName = "Preservation_3_9: Cpf validation is idempotent for valid CPFs")]
    [InlineData("11144477735")]
    [InlineData("111.444.777-35")]
    public void Preservation_Cpf_ValidInput_IsIdempotent(string input)
    {
        var cpf = new Cpf(input);

        cpf.Validate();
        bool firstResult = cpf.IsValid;

        cpf.Validate();
        bool secondResult = cpf.IsValid;

        Assert.True(firstResult, $"Cpf '{input}' should be valid.");
        Assert.Equal(firstResult, secondResult);
    }

    [Theory(DisplayName = "Preservation_3_9: Cpf validation is idempotent for invalid CPFs")]
    [InlineData("12345678900")]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("")]
    [InlineData("1234")]
    public void Preservation_Cpf_InvalidInput_IsIdempotent(string input)
    {
        var cpf = new Cpf(input);

        cpf.Validate();
        bool firstResult = cpf.IsInvalid;

        cpf.Validate();
        bool secondResult = cpf.IsInvalid;

        Assert.True(firstResult, $"Cpf '{input}' should be invalid.");
        Assert.Equal(firstResult, secondResult);
    }

    [Theory(DisplayName = "Preservation_3_9: Cpf formatted output is stable across calls")]
    [InlineData("11144477735", "111.444.777-35")]
    public void Preservation_Cpf_FormattedOutput_IsStable(string raw, string expectedFormatted)
    {
        var cpf = new Cpf(raw);

        Assert.Equal(expectedFormatted, cpf.Formatted);
        Assert.Equal(expectedFormatted, cpf.Formatted); // second call — same result
    }

    // ── Property: Phone validation produces consistent results (Requirement 3.9) ─────────────
    //
    // For any Phone, calling Validate() twice produces the same result.

    [Theory(DisplayName = "Preservation_3_9: Phone validation is idempotent for valid phones")]
    [InlineData("+55", "11", "987654321")]
    [InlineData("+55", "84", "12345678")]
    [InlineData("+1", "21", "987654321")]
    public void Preservation_Phone_ValidInput_IsIdempotent(string ddi, string ddd, string number)
    {
        var phone = new Phone(ddi, ddd, number);

        phone.Validate();
        bool firstResult = phone.IsValid;

        phone.Validate();
        bool secondResult = phone.IsValid;

        Assert.True(firstResult, $"Phone '{ddi} {ddd} {number}' should be valid.");
        Assert.Equal(firstResult, secondResult);
    }

    [Theory(DisplayName = "Preservation_3_9: Phone validation is idempotent for invalid phones")]
    [InlineData("55", "84", "987654321")]   // DDI missing '+'
    [InlineData("+55", "8", "987654321")]   // DDD too short
    [InlineData("+55", "84", "98A76")]      // number has letters
    public void Preservation_Phone_InvalidInput_IsIdempotent(string ddi, string ddd, string number)
    {
        var phone = new Phone(ddi, ddd, number);

        phone.Validate();
        bool firstResult = phone.IsInvalid;

        phone.Validate();
        bool secondResult = phone.IsInvalid;

        Assert.True(firstResult, $"Phone '{ddi} {ddd} {number}' should be invalid.");
        Assert.Equal(firstResult, secondResult);
    }

    [Theory(DisplayName = "Preservation_3_9: Phone TryParse produces consistent results")]
    [InlineData("5511987654321", true)]
    [InlineData("551112345678", true)]
    [InlineData("", false)]
    [InlineData("abc", false)]
    public void Preservation_Phone_TryParse_IsConsistent(string input, bool expectedSuccess)
    {
        bool result1 = Phone.TryParse(input, out var phone1);
        bool result2 = Phone.TryParse(input, out var phone2);

        Assert.Equal(expectedSuccess, result1);
        Assert.Equal(result1, result2);

        if (expectedSuccess)
        {
            Assert.NotNull(phone1);
            Assert.NotNull(phone2);
            Assert.Equal(phone1!.DDI, phone2!.DDI);
            Assert.Equal(phone1.DDD, phone2.DDD);
            Assert.Equal(phone1.Number, phone2.Number);
        }
    }

    // ── Property: Email validation produces consistent results (Requirement 3.9) ─────────────
    //
    // For any Email, calling Validate() twice produces the same result.

    [Theory(DisplayName = "Preservation_3_9: Email validation is idempotent for valid emails")]
    [InlineData("contact@email.com")]
    [InlineData("user@domain.org.br")]
    [InlineData("test.user@sub.domain.com")]
    public void Preservation_Email_ValidInput_IsIdempotent(string address)
    {
        var email = new Email(address);

        email.Validate();
        bool firstResult = email.IsValid;

        email.Validate();
        bool secondResult = email.IsValid;

        Assert.True(firstResult, $"Email '{address}' should be valid.");
        Assert.Equal(firstResult, secondResult);
    }

    [Theory(DisplayName = "Preservation_3_9: Email validation is idempotent for invalid emails")]
    [InlineData("")]
    [InlineData("notanemail")]
    [InlineData("missing@tld")]
    [InlineData("@nodomain.com")]
    [InlineData("contact@ email.com")]
    public void Preservation_Email_InvalidInput_IsIdempotent(string address)
    {
        var email = new Email(address);

        email.Validate();
        bool firstResult = email.IsInvalid;

        email.Validate();
        bool secondResult = email.IsInvalid;

        Assert.True(firstResult, $"Email '{address}' should be invalid.");
        Assert.Equal(firstResult, secondResult);
    }

    [Theory(DisplayName = "Preservation_3_9: Email properties are stable after construction")]
    [InlineData("contact@fakerdomain.org.br", "contact", "fakerdomain.org.br", "fakerdomain", "org.br")]
    [InlineData("user@example.com", "user", "example.com", "example", "com")]
    public void Preservation_Email_Properties_AreStable(
        string address, string expectedUser, string expectedHost,
        string expectedDomain, string expectedTld)
    {
        var email = new Email(address);

        Assert.Equal(expectedUser, email.User);
        Assert.Equal(expectedHost, email.Host);
        Assert.Equal(expectedDomain, email.Domain);
        Assert.Equal(expectedTld, email.TopLevelDomain);

        // Properties must be stable — same values on repeated access
        Assert.Equal(email.User, email.User);
        Assert.Equal(email.Host, email.Host);
        Assert.Equal(email.Domain, email.Domain);
        Assert.Equal(email.TopLevelDomain, email.TopLevelDomain);
    }

    // ── Property: Entity<T> equality by Id is symmetric and transitive (Requirement 3.7) ─────
    //
    // For any two Entity<T> instances:
    //   Symmetry:     a.Equals(b) == b.Equals(a)
    //   Transitivity: if a.Equals(b) && b.Equals(c) then a.Equals(c)
    //   Reflexivity:  a.Equals(a) == true

    [Fact(DisplayName = "Preservation_3_7: Entity equality by Id is reflexive")]
    public void Preservation_Entity_Equality_IsReflexive()
    {
        var entity = new FakeEntity();

        Assert.True(entity.Equals(entity));
        Assert.Equal(entity.GetHashCode(), entity.GetHashCode());
    }

    [Fact(DisplayName = "Preservation_3_7: Entity equality by Id is symmetric")]
    public void Preservation_Entity_Equality_IsSymmetric()
    {
        var entity1 = new FakeEntity();
        var entity2 = entity1.Clone(); // same Id

        bool ab = entity1.Equals(entity2);
        bool ba = entity2.Equals(entity1);

        Assert.Equal(ab, ba);
        Assert.True(ab, "Entities with same Id should be equal.");
    }

    [Fact(DisplayName = "Preservation_3_7: Entity equality by Id is transitive")]
    public void Preservation_Entity_Equality_IsTransitive()
    {
        var entity1 = new FakeEntity();
        var entity2 = entity1.Clone(); // same Id as entity1
        var entity3 = entity1.Clone(); // same Id as entity1

        bool ab = entity1.Equals(entity2);
        bool bc = entity2.Equals(entity3);
        bool ac = entity1.Equals(entity3);

        Assert.True(ab);
        Assert.True(bc);
        Assert.True(ac, "Transitivity: if a==b and b==c then a==c.");
    }

    [Fact(DisplayName = "Preservation_3_7: Entities with different Ids are not equal")]
    public void Preservation_Entity_DifferentIds_AreNotEqual()
    {
        var entity1 = new FakeEntity();
        var entity2 = new FakeEntity(); // different Id

        Assert.False(entity1.Equals(entity2));
        Assert.False(entity2.Equals(entity1)); // symmetric
    }

    [Fact(DisplayName = "Preservation_3_7: Entity.Equals(null) returns false")]
    public void Preservation_Entity_EqualsNull_ReturnsFalse()
    {
        var entity = new FakeEntity();

        Assert.False(entity.Equals(null));
        Assert.False(entity.Equals((object?)null));
    }

    [Fact(DisplayName = "Preservation_3_7: Entity GetHashCode is consistent with Equals")]
    public void Preservation_Entity_GetHashCode_IsConsistentWithEquals()
    {
        var entity1 = new FakeEntity();
        var entity2 = entity1.Clone();

        Assert.True(entity1.Equals(entity2));
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    // ── Property: Entity<T> initialization is correct (Requirement 3.6) ─────────────────────
    //
    // Entity<T> must generate a non-default Id, set Active=true, set CreateAt to today,
    // and expose empty Events list on construction.

    [Fact(DisplayName = "Preservation_3_6: Entity initializes Id, Active, CreateAt, Events correctly")]
    public void Preservation_Entity_Initialization_IsCorrect()
    {
        var entity = new FakeEntity();

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.True(entity.Active);
        Assert.Equal(DateTimeOffset.UtcNow.Date, entity.CreateAt.Date);
        Assert.Empty(entity.Events);
        Assert.Equal(0, entity.Version);
    }

    [Fact(DisplayName = "Preservation_3_6: Each Entity gets a unique Id")]
    public void Preservation_Entity_EachInstance_GetsUniqueId()
    {
        var ids = Enumerable.Range(0, 10)
            .Select(_ => new FakeEntity().Id)
            .ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    // ── Property: Address validation is consistent with any length config (Requirement 3.8) ──
    //
    // Address validation must produce consistent results regardless of how it is constructed.

    [Fact(DisplayName = "Preservation_3_8: Address with valid fields is valid")]
    public void Preservation_Address_ValidFields_IsValid()
    {
        var country = new Country("Brazil", "BR", "BRA");
        var state = new State("Rio Grande do Norte", "RN", country);
        var city = new City("Natal", state);
        var neighborhood = new Neighborhood("Tirol", city);
        var zipCode = new ZipCode("59020010");

        var address = new Address("Av. Afonso Pena", "123", "Apto 101", neighborhood, zipCode);
        address.Validate();

        Assert.True(address.IsValid);
        Assert.Equal("Av. Afonso Pena", address.Street);
        Assert.Equal("123", address.Number);
    }

    [Theory(DisplayName = "Preservation_3_8: Address validation is idempotent")]
    [InlineData("Av. Afonso Pena", "123", true)]
    [InlineData("A", "123", false)]          // street too short
    [InlineData("Rua das Flores", "", false)] // number empty
    public void Preservation_Address_Validation_IsIdempotent(string street, string number, bool expectedValid)
    {
        var country = new Country("Brazil", "BR", "BRA");
        var state = new State("RN", "RN", country);
        var city = new City("Natal", state);
        var neighborhood = new Neighborhood("Tirol", city);
        var zipCode = new ZipCode("59020010");

        var address = new Address(street, number, null!, neighborhood, zipCode);

        address.Validate();
        bool firstResult = address.IsValid;

        address.Validate();
        bool secondResult = address.IsValid;

        Assert.Equal(expectedValid, firstResult);
        Assert.Equal(firstResult, secondResult);
    }

    [Fact(DisplayName = "Preservation_3_8: Address with 9-param constructor (length config) validates consistently")]
    public void Preservation_Address_NineParamConstructor_ValidatesConsistently()
    {
        var country = new Country("Brazil", "BR", "BRA");
        var state = new State("RN", "RN", country);
        var city = new City("Natal", state);
        var neighborhood = new Neighborhood("Tirol", city);
        var zipCode = new ZipCode("59020010");

        // Using the 6-parameter constructor with custom length config via AddressLengthOptions
        var address = new Address(
            "Rua das Flores", "123", null!,
            neighborhood, zipCode,
            new AddressLengthOptions(
                StreetMinLength: 2,
                StreetMaxLength: 80,
                NumberMinLength: 1,
                NumberMaxLength: 10));

        address.Validate();
        Assert.True(address.IsValid);

        // Validate again — must produce same result
        address.Validate();
        Assert.True(address.IsValid);
    }

    [Fact(DisplayName = "Preservation_3_8: Address 5-param and 9-param constructors produce same validation result")]
    public void Preservation_Address_FiveAndNineParamConstructors_ProduceSameResult()
    {
        var country = new Country("Brazil", "BR", "BRA");
        var state = new State("RN", "RN", country);
        var city = new City("Natal", state);
        var neighborhood = new Neighborhood("Tirol", city);
        var zipCode = new ZipCode("59020010");

        var address5 = new Address("Rua das Flores", "123", null!, neighborhood, zipCode);
        var address9 = new Address(
            "Rua das Flores", "123", null!,
            neighborhood, zipCode,
            new AddressLengthOptions(
                StreetMinLength: 2,
                StreetMaxLength: 80,
                NumberMinLength: 1,
                NumberMaxLength: 10));

        address5.Validate();
        address9.Validate();

        Assert.Equal(address5.IsValid, address9.IsValid);
        Assert.Equal(address5.Street, address9.Street);
        Assert.Equal(address5.Number, address9.Number);
        Assert.Equal(address5.StreetMinLength, address9.StreetMinLength);
        Assert.Equal(address5.StreetMaxLength, address9.StreetMaxLength);
        Assert.Equal(address5.NumberMinLength, address9.NumberMinLength);
        Assert.Equal(address5.NumberMaxLength, address9.NumberMaxLength);
    }

    // ── Property: IResult<T> semantics are preserved (Requirements 3.1, 3.2, 3.3) ────────────
    //
    // Result<T> must continue exposing Value, IsSuccess, IsFailure, Errors with same semantics.

    [Fact(DisplayName = "Preservation_3_3: Result<T> with value has IsSuccess=true")]
    public void Preservation_Result_WithValue_IsSuccess()
    {
        var result = new Sagi.Sdk.Results.Result<string>("hello");

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal("hello", result.Value);
    }

    [Fact(DisplayName = "Preservation_3_3: Result<T> with errors has IsFailure=true")]
    public void Preservation_Result_WithErrors_IsFailure()
    {
        var errors = new Sagi.Sdk.Results.Error[] { new("ERR", "Something went wrong") };
        var result = new Sagi.Sdk.Results.Result<string>(errors);

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Equal(errors, result.Errors);
    }

    [Fact(DisplayName = "Preservation_3_2: Error throws ArgumentException for null/empty code")]
    public void Preservation_Error_ThrowsForNullCode()
    {
        // ThrowsAny accepts ArgumentNullException (subclass of ArgumentException) for null input
        Assert.ThrowsAny<ArgumentException>(() => new Sagi.Sdk.Results.Error(null!, "message"));
        Assert.ThrowsAny<ArgumentException>(() => new Sagi.Sdk.Results.Error("", "message"));
    }

    [Fact(DisplayName = "Preservation_3_2: Error throws ArgumentException for null/empty message")]
    public void Preservation_Error_ThrowsForNullMessage()
    {
        // ThrowsAny accepts ArgumentNullException (subclass of ArgumentException) for null input
        Assert.ThrowsAny<ArgumentException>(() => new Sagi.Sdk.Results.Error("code", null!));
        Assert.ThrowsAny<ArgumentException>(() => new Sagi.Sdk.Results.Error("code", ""));
    }
}
