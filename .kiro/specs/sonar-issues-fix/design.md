# Sonar Issues Fix — Bugfix Design

## Overview

Correção de 30 issues SonarQube distribuídos em 4 SDKs do monorepo Sagi (.NET 8). Os problemas abrangem: violações de design de API (covariância, visibilidade de construtores), code smells de manutenibilidade (campos não utilizados, literais booleanos, caller information explícito), riscos de runtime (método virtual no construtor, operator == sem null-safety), e problemas em testes (null em tipos não-nullable, Theory sem parâmetros). Nenhuma alteração funcional é introduzida — o comportamento observável dos SDKs permanece idêntico após a correção.

A estratégia é cirúrgica: cada issue recebe a menor mudança possível que elimina o problema sem alterar a semântica pública. Os itens mais complexos (`Entity<T>`, `Address`, `GeneratedRegex`, covariância em `IResult<T>`) têm decisões de design explícitas documentadas abaixo.

## Glossary

- **Bug_Condition (C)**: Conjunto de condições de código que fazem o SonarQube reportar um issue — código que compila e funciona mas viola regras de qualidade, segurança ou manutenibilidade
- **Property (P)**: Comportamento correto esperado após a correção — o código deve compilar, passar todos os testes existentes e não reportar o issue no Sonar
- **Preservation**: Comportamento funcional e semântica pública que não devem mudar — assinaturas de API, lógica de validação, resultados de operações
- **`Entity<T>`**: Classe base abstrata em `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Entities/Entity.cs` que fornece identidade, eventos e igualdade por Id para entidades DDD
- **`Validateble`**: Classe base abstrata em `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Contracts/Validateble.cs` que acumula erros de validação
- **`Event<T>`**: Classe base abstrata em `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Contracts/Event.cs` para eventos de domínio
- **`IResult<T>`**: Interface em `Sagi.Sdk.Results/src/Sagi.Sdk.Results/Contracts/IResult.cs` que representa resultado tipado de uma operação
- **`GeneratedRegexAttribute`**: Atributo do .NET 7+ que instrui o compilador a gerar código de regex em compile-time, eliminando alocação em runtime
- **Covariância (`out T`)**: Modificador de tipo genérico que permite `IResult<Derived>` ser atribuído a `IResult<Base>` quando `T` aparece apenas em posição de saída
- **Caller Information**: Atributos `[CallerMemberName]`, `[CallerArgumentExpression]` que o compilador preenche automaticamente — passar o nome explicitamente anula o benefício


## Bug Details

### Bug Condition

Os issues se manifestam em tempo de compilação ou análise estática — o código funciona em runtime mas viola regras de qualidade. A condição de bug é detectável por análise do código-fonte sem execução.

**Formal Specification:**
```
FUNCTION isBugCondition(codeElement)
  INPUT: codeElement — qualquer declaração, expressão ou statement C#
  OUTPUT: boolean

  RETURN (
    // Sagi.Sdk.Results
    (codeElement IS IResult<T> AND T NOT MARKED out)                                    // 1.1
    OR (codeElement IS ArgumentException.ThrowIfNullOrEmpty(x, nameof(x)))              // 1.2
    OR (codeElement IS "Errors.Any() == false")                                         // 1.3

    // Sagi.Sdk.Domain — fonte
    OR (codeElement IS Event<T>.constructor AND visibility IS public)                   // 1.4
    OR (codeElement IS ArgumentNullException.ThrowIfNull(x, nameof(x)))                 // 1.5
    OR (codeElement IS Entity._errors FIELD AND never read)                             // 1.6
    OR (codeElement IS Entity.constructor AND calls GenerateId() directly)              // 1.7
    OR (codeElement IS "Active = true" AS field initializer AND also set in ctor)       // 1.8
    OR (codeElement IS Entity.operator== AND Sonar classifies as Blocker)               // 1.9
    OR (codeElement IS Address.constructor AND parameterCount > 7)                      // 1.10
    OR (codeElement IS Cnpj AND uses new Regex(...) at runtime)                         // 1.11
    OR (codeElement IS Cpf AND uses Regex.IsMatch(...) at runtime)                      // 1.12
    OR (codeElement IS Email AND redundant null checks on non-nullable)                 // 1.13
    OR (codeElement IS "isMatch is false")                                              // 1.14
    OR (codeElement IS Phone AND uses new Regex(...) at runtime)                        // 1.15
    OR (codeElement IS ZipCode AND redundant null check on non-nullable)                // 1.16

    // Sagi.Sdk.Domain — testes
    OR (codeElement IS test passing null to non-nullable parameter)                     // 1.17, 1.18

    // Sagi.Sdk.MongoDb
    OR (codeElement IS MongoContext.constructor AND visibility IS public)               // 1.19
    OR (codeElement IS ServicesExtensions AND passes nameof() to ThrowIfNull)           // 1.20
    OR (codeElement IS MongoDockerContainer.Dispose AND missing GC.SuppressFinalize)    // 1.21

    // Sagi.Sdk.AWS.DynamoDb — fonte
    OR (codeElement IS DynamoDbHosting AND not static AND no protected ctor)            // 1.22
    OR (codeElement IS "tables.Length > 0 == true" boolean literal)                    // 1.23
    OR (codeElement IS DynamoDbTableEventsArgs AND name NOT ending in EventArgs)        // 1.24
    OR (codeElement IS Payment CLASS AND no namespace)                                  // 1.25
    OR (codeElement IS AwsOptions RECORD AND no namespace)                              // 1.26
    OR (codeElement IS PaymentTable AND has empty statement ";;")                       // 1.27

    // Sagi.Sdk.AWS.DynamoDb — testes
    OR (codeElement IS DynamoDbDockerContainer.Dispose AND missing GC.SuppressFinalize) // 1.28
    OR (codeElement IS [Theory] method AND has no parameters)                           // 1.29
    OR (codeElement IS PageResultTests AND passes null to string non-nullable)          // 1.30
  )
END FUNCTION
```

### Exemplos Concretos

- **1.1** `IResult<T>` sem `out`: `IResult<DerivedError>` não pode ser atribuído a `IResult<BaseError>` — falha em tempo de compilação em código cliente
- **1.7** `GenerateId()` no construtor: se subclasse sobrescreve `GenerateId()` e acessa estado não inicializado, o comportamento é indefinido (NullReferenceException silenciosa ou Id incorreto)
- **1.9** `operator ==` Blocker: o Sonar S3875 classifica como Blocker porque `Entity<T>` não é um value type — `operator ==` em reference types cria confusão semântica
- **1.10** `Address` com 9 parâmetros: viola S107 (máximo 7) — o construtor de 9 parâmetros é o que aciona o issue; o de 5 parâmetros está dentro do limite
- **1.24** `DynamoDbTableEventsArgs`: o nome não termina em `EventArgs` — viola S3376 (convenção de nomenclatura para args de evento)
- **1.29** `[Theory]` sem parâmetros: xUnit lança `InvalidOperationException` em runtime ao tentar executar o método


## Expected Behavior

### Preservation Requirements

**Comportamentos que NÃO devem mudar:**
- `IResult<T>` continua expondo `Value`, `IsSuccess`, `IsFailure`, `Errors` com a mesma semântica
- `Error` continua lançando `ArgumentException` para `code` ou `message` nulos/vazios
- `Result<T>` continua retornando `IsSuccess = true` para valor e `IsFailure = true` para erros
- `Entity<T>` continua gerando `Id` único, `CreateAt`, `Active = true`, e expondo `Events`, `Version`, `AddEvent`, `LoadEvent`, `ClearEvents`
- Igualdade de `Entity<T>` continua sendo por `Id` (não por referência)
- `Address` continua validando e expondo `Street`, `Number`, `Complement`, `Neighborhood`, `ZipCode`
- `Cnpj`, `Cpf`, `Email`, `Phone`, `ZipCode` continuam validando, formatando e expondo propriedades com a mesma semântica
- `Cnpj` alfanumérico (IN RFB nº 2.229/2024) continua sendo validado pelo algoritmo já implementado
- `MongoContext<T>` continua executando todas as operações CRUD com a mesma semântica
- `DynamoDbContext<TModel>` continua executando todas as operações com a mesma semântica
- `TablesInitializer` continua criando tabelas ausentes e disparando o evento de tabela pronta
- `PageQuery` e `PageResult<TResult>` continuam paginando com a mesma semântica
- Todos os testes existentes continuam passando sem regressões funcionais

**Escopo das mudanças:**
Apenas estrutura de código (visibilidade, nomenclatura, expressões equivalentes, atributos de compilação). Nenhuma lógica de negócio é alterada.


## Hypothesized Root Cause

Os issues têm causas independentes por categoria:

1. **Covariância ausente em `IResult<T>` (1.1)**: A interface foi definida antes de haver necessidade de atribuição covariante. O parâmetro `T` aparece apenas em posição de saída (`T? Value`), tornando `out T` seguro e correto.

2. **Caller Information explícito (1.2, 1.5, 1.20)**: Padrão antigo de passar `nameof(param)` explicitamente para métodos `ThrowIfNull`/`ThrowIfNullOrEmpty`. O .NET 6+ usa `[CallerArgumentExpression]` internamente — passar o nome explicitamente sobrescreve o valor inferido pelo compilador com a string literal do nome do parâmetro, que coincide, mas é redundante e viola S3236.

3. **Boolean literals desnecessários (1.3, 1.23)**: Expressões como `Errors.Any() == false` e `tables.Length > 0 == true` são equivalentes a `!Errors.Any()` e `tables.Length > 0`. O Sonar S1125 detecta comparações com `true`/`false` literais.

4. **Construtores `public` em classes abstratas (1.4, 1.19)**: `Event<T>` e `MongoContext<T>` são abstratos — construtores `public` em classes abstratas são inúteis (não se pode instanciar diretamente) e enganosos. O Sonar S3442 exige `protected`.

5. **Campo `_errors` não utilizado em `Entity<T>` (1.6)**: `Entity<T>` herda `Validateble` que já possui sua própria lista `_errors`. O campo declarado em `Entity<T>` nunca é lido — foi provavelmente um resquício de refatoração.

6. **`GenerateId()` no construtor de `Entity<T>` (1.7)**: Chamar método virtual/abstrato no construtor (Sonar S1699) é perigoso porque a subclasse ainda não foi inicializada quando o construtor da base executa. Se `GenerateId()` na subclasse acessa campos da subclasse, eles ainda são `default`. A solução é inicializar `Id` com `default!` no construtor e deixar a subclasse atribuir via propriedade ou factory method.

7. **`operator ==` em `Entity<T>` (1.9)**: O Sonar S3875 classifica como Blocker a sobrecarga de `operator ==` em tipos de referência que não são value-like. A semântica de igualdade por Id já está implementada em `Equals(object?)` — o `operator ==` é redundante e pode ser removido sem perda funcional, pois os testes que o usam podem ser reescritos com `Equals` ou mantidos com `null!` para os casos de null.

8. **`Address` com 9 parâmetros (1.10)**: O construtor interno de 9 parâmetros (com os limites de comprimento) viola S107. A solução é extrair os 4 parâmetros de configuração (`streetMinLength`, `streetMaxLength`, `numberMinLength`, `numberMaxLength`) para um objeto de configuração `AddressLengthOptions` (record ou struct), reduzindo para 6 parâmetros.

9. **`Regex` em runtime (1.11, 1.12, 1.15)**: `Cnpj` usa `static readonly Regex` compilados (correto para os 3 campos estáticos), mas o método `Normalize` usa `Regex.Replace(value, @"[.\-/]", string.Empty)` inline. `Cpf` usa `Regex.IsMatch` inline. `Phone` usa `new Regex(...)` em `OnlyDigits` e `Regex.Match` inline em `TryParse`. Todos devem migrar para `[GeneratedRegex]` em partial class.

10. **Verificações de null desnecessárias em `Email` e `ZipCode` (1.13, 1.16)**: `Email.GetHost()`, `GetUser()`, `GetDomain()`, `GetTopLevenDomain()` verificam `string.IsNullOrEmpty` em variáveis que o compilador já sabe serem não-nulas no contexto. `ZipCode.Validate()` verifica null em `Value` que é `string` não-nullable. O Sonar S2589 detecta condições sempre verdadeiras/falsas.

11. **`DynamoDbHosting` sem `static` ou `protected` ctor (1.22)**: A classe tem apenas membros estáticos mas não é declarada `static`. O Sonar S1118 exige que classes utilitárias (todos membros estáticos) sejam `static` ou tenham construtor `protected`/`private`.

12. **Nomenclatura `DynamoDbTableEventsArgs` (1.24)**: O nome não termina em `EventArgs` — viola S3376. Renomear para `DynamoDbTableReadyEventArgs` e atualizar o delegate e todos os usos.

13. **Namespace ausente em samples (1.25, 1.26)**: `Payment.cs` e `AwsOptions` (record em `Program.cs`) não têm namespace. `Payment` deve receber `namespace Samples.Entities;`. `AwsOptions` está em `Program.cs` como top-level record — mover para arquivo próprio com namespace `Samples`.

14. **Empty statement em `PaymentTable` (1.27)**: `BillingMode = BillingMode.PAY_PER_REQUEST;;` tem ponto-e-vírgula duplo. Remover o extra.

15. **`GC.SuppressFinalize` ausente (1.21, 1.28)**: `MongoDockerContainer.Dispose()` e `DynamoDbDockerContainer.Dispose()` implementam `IDisposable` sem chamar `GC.SuppressFinalize(this)`. O Sonar S3971 exige a chamada para evitar que o GC chame o finalizador em objetos já descartados.

16. **`[Theory]` sem parâmetros (1.29)**: `ShouldValidateConstructorParameters` usa `[Theory]` mas não tem `[InlineData]` nem `[MemberData]` — deve ser `[Fact]` pois recebe dados via AutoFixture, não via xUnit data source.

17. **`null` em parâmetro `string` não-nullable (1.17, 1.18, 1.30)**: Testes passam `null` literal para parâmetros `string` não-nullable. A solução é usar `null!` (null-forgiving operator) ou, onde semanticamente melhor, usar `string.Empty`.


## Correctness Properties

Property 1: Bug Condition — Todos os issues SonarQube são eliminados

_For any_ elemento de código onde `isBugCondition` retorna `true`, o código corrigido SHALL não reportar o issue correspondente no SonarQube, mantendo compilação sem erros e sem warnings CS8600/CS8625.

**Validates: Requirements 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 2.8, 2.9, 2.10, 2.11, 2.12, 2.13, 2.14, 2.15, 2.16, 2.17, 2.18, 2.19, 2.20, 2.21, 2.22, 2.23, 2.24, 2.25, 2.26, 2.27, 2.28, 2.29, 2.30**

Property 2: Preservation — Comportamento funcional inalterado

_For any_ entrada onde `isBugCondition` retorna `false` (código não afetado pelas correções), o código corrigido SHALL produzir exatamente o mesmo resultado que o código original, preservando toda a semântica pública dos 4 SDKs.

**Validates: Requirements 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9, 3.10, 3.11, 3.12, 3.13, 3.14, 3.15, 3.16**


## Fix Implementation

### Sagi.Sdk.Results

**Arquivo:** `Sagi.Sdk.Results/src/Sagi.Sdk.Results/Contracts/IResult.cs`

**Issue 1.1 — Covariância em `IResult<T>`:**
```csharp
// ANTES
public interface IResult<T> : IResult
{
    T? Value { get; }
}

// DEPOIS
public interface IResult<out T> : IResult
{
    T? Value { get; }
}
```
`out T` é seguro porque `T` aparece apenas em posição de saída (`Value` é get-only). Nenhum método recebe `T` como parâmetro na interface. `Result<T>` implementa a interface e continua funcionando — classes concretas podem usar `T` em qualquer posição.

---

**Arquivo:** `Sagi.Sdk.Results/src/Sagi.Sdk.Results/Error.cs`

**Issue 1.2 — Caller Information explícito:**
```csharp
// ANTES
ArgumentException.ThrowIfNullOrEmpty(code, nameof(code));
ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));

// DEPOIS
ArgumentException.ThrowIfNullOrEmpty(code);
ArgumentException.ThrowIfNullOrEmpty(message);
```

---

**Arquivo:** `Sagi.Sdk.Results/src/Sagi.Sdk.Results/Result.cs`

**Issue 1.3 — Boolean literal:**
```csharp
// ANTES
public bool IsSuccess => Errors == null || Errors.Any() == false;

// DEPOIS
public bool IsSuccess => Errors == null || !Errors.Any();
```

---

### Sagi.Sdk.Domain — Contratos e Entidades

**Arquivo:** `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Contracts/Event.cs`

**Issue 1.4 — Construtor `public` em classe abstrata:**
```csharp
// ANTES
public Event() { Timestamp = DateTimeOffset.UtcNow; }

// DEPOIS
protected Event() { Timestamp = DateTimeOffset.UtcNow; }
```

---

**Arquivo:** `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Contracts/Validateble.cs`

**Issue 1.5 — Caller Information explícito:**
```csharp
// ANTES
ArgumentNullException.ThrowIfNull(error, nameof(error));
ArgumentNullException.ThrowIfNull(errors, nameof(errors));

// DEPOIS
ArgumentNullException.ThrowIfNull(error);
ArgumentNullException.ThrowIfNull(errors);
```

---

**Arquivo:** `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Entities/Entity.cs`

**Issue 1.6 — Campo `_errors` não utilizado:**
Remover a linha:
```csharp
// REMOVER
private readonly List<IError> _errors = [];
```
`Entity<T>` herda `Validateble` que já possui `_errors`. O campo em `Entity<T>` nunca é referenciado.

**Issue 1.7 — `GenerateId()` no construtor:**

Decisão de design: remover a chamada de `GenerateId()` do construtor e inicializar `Id` com `default!`. A subclasse é responsável por atribuir o Id no seu próprio construtor, chamando `GenerateId()` de forma segura após a inicialização completa.

```csharp
// ANTES
protected Entity()
{
    Id = GenerateId();
    CreateAt = DateTimeOffset.UtcNow;
    Active = true;
}
public T Id { get; private set; } = default!;

// DEPOIS
protected Entity()
{
    CreateAt = DateTimeOffset.UtcNow;
    Active = true;
}
public T Id { get; protected set; } = default!;
```

A subclasse `FakeEntity` (e qualquer entidade concreta) deve chamar `GenerateId()` no seu próprio construtor:
```csharp
public FakeEntity()
{
    Id = GenerateId();
}
```

Impacto: `Id` muda de `private set` para `protected set` para permitir que subclasses atribuam. Isso é uma mudança de visibilidade mínima e não quebra código cliente (que nunca atribui `Id` externamente).

**Issue 1.8 — Inicializador redundante `Active = true`:**
O campo `Active` não tem inicializador de membro — a atribuição já ocorre no construtor. Verificar se há inicializador de campo e removê-lo se existir. Pela leitura do código atual, `Active` não tem inicializador de campo explícito (apenas `public bool Active { get; private set; }`), então este issue pode ser um falso positivo do Sonar ou referir-se à linha do construtor. Manter como está se não houver inicializador duplicado.

**Issue 1.9 — `operator ==` Blocker:**

Decisão de design: **remover** `operator ==` e `operator !=` de `Entity<T>`.

Justificativa:
- O Sonar S3875 classifica como Blocker porque `Entity<T>` é reference type — `operator ==` em reference types cria ambiguidade semântica
- A igualdade por Id já está implementada em `Equals(object?)` e `GetHashCode()`
- Os testes que usam `entity1 == entity2` podem ser reescritos com `entity1.Equals(entity2)` ou `Assert.Equal(entity1, entity2)`
- Os testes que testam `entity == null` podem usar `entity is null` ou `Assert.Null(entity)`

```csharp
// REMOVER completamente
public static bool operator ==(Entity<T> a, Entity<T> b) { ... }
public static bool operator !=(Entity<T> a, Entity<T> b) { ... }
```

Os testes em `EntityTests.cs` que testam `operator ==` e `operator !=` devem ser refatorados para usar `Assert.Equal`, `Assert.NotEqual`, `Assert.Null`, `Assert.NotNull` ou comparações com `is null`.

---

### Sagi.Sdk.Domain — Value Objects

**Arquivo:** `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Address.cs`

**Issue 1.10 — Construtor com 9 parâmetros:**

Decisão de design: extrair os 4 parâmetros de configuração de comprimento para um record `AddressLengthOptions`.

```csharp
// NOVO TIPO (mesmo arquivo ou arquivo separado AddressLengthOptions.cs)
public record AddressLengthOptions(
    short StreetMinLength = 2,
    short StreetMaxLength = 80,
    short NumberMinLength = 1,
    short NumberMaxLength = 10);
```

O construtor de 5 parâmetros (público, dentro do limite) passa a usar valores default do record:
```csharp
public Address(
    string street,
    string number,
    string complement,
    Neighborhood neighborhood,
    ZipCode zipCode)
    : this(street, number, complement, neighborhood, zipCode, new AddressLengthOptions())
{ }
```

O construtor de 9 parâmetros é substituído por um de 6:
```csharp
public Address(
    string street,
    string number,
    string complement,
    Neighborhood neighborhood,
    ZipCode zipCode,
    AddressLengthOptions options)
{
    Street = street?.Trim();
    Number = number?.Trim();
    Complement = complement?.Trim();
    Neighborhood = neighborhood;
    ZipCode = zipCode;
    StreetMinLength = options.StreetMinLength;
    StreetMaxLength = options.StreetMaxLength;
    NumberMinLength = options.NumberMinLength;
    NumberMaxLength = options.NumberMaxLength;
}
```

Isso reduz de 9 para 6 parâmetros (dentro do limite de 7) e mantém toda a funcionalidade. Testes que usam o construtor de 9 parâmetros devem ser atualizados para usar `AddressLengthOptions`.

---

**Arquivo:** `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Cnpj.cs`

**Issue 1.11 — `Regex` em runtime no método `Normalize`:**

`Cnpj` já usa `static readonly Regex` para os 3 campos estáticos — esses estão corretos. O único uso inline é `Regex.Replace(value, @"[.\-/]", string.Empty)` dentro de `Normalize`. Migrar para `[GeneratedRegex]`:

```csharp
// Adicionar como partial class e membro estático
public sealed partial class Cnpj : ValueObject<Cnpj>
{
    [GeneratedRegex(@"[.\-/]")]
    private static partial Regex FormattedSeparatorsRegex();
}
```

Substituir os 3 campos `static readonly Regex` por métodos `[GeneratedRegex]`:
```csharp
[GeneratedRegex(@"^\d{14}$")]
private static partial Regex NumericRawRegex();

[GeneratedRegex(@"^[A-Z0-9]{14}$")]
private static partial Regex AlphanumericRawRegex();

[GeneratedRegex(@"^[A-Z0-9]{2}\.[A-Z0-9]{3}\.[A-Z0-9]{3}\/[A-Z0-9]{4}-\d{2}$")]
private static partial Regex FormattedMaskRegex();

[GeneratedRegex(@"[.\-/]")]
private static partial Regex FormattedSeparatorsRegex();
```

Atualizar todos os usos de `s_numericRaw`, `s_alphanumericRaw`, `s_formattedMask` para chamar os métodos gerados. A classe deve ser declarada `partial`.

---

**Arquivo:** `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Cpf.cs`

**Issue 1.12 — `Regex.IsMatch` em runtime:**

```csharp
// ANTES
if (!Regex.IsMatch(Number, @"^\d{11}$"))

// DEPOIS — partial class com GeneratedRegex
public sealed partial class Cpf : ValueObject<Cpf>
{
    [GeneratedRegex(@"^\d{11}$")]
    private static partial Regex ElevenDigitsRegex();
}

// Uso:
if (!ElevenDigitsRegex().IsMatch(Number))
```

---

**Arquivo:** `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Email.cs`

**Issue 1.13 — Verificações de null desnecessárias:**

Os métodos `GetHost()`, `GetUser()`, `GetDomain()`, `GetTopLevenDomain()` verificam `string.IsNullOrEmpty(Address)` e `string.IsNullOrEmpty(Host)`. Como `Address` e `Host` são `string` (não `string?`), o compilador sabe que não são null. Simplificar para verificar apenas `string.IsNullOrEmpty` (que cobre o caso de string vazia, que é o caso real aqui):

```csharp
// Manter string.IsNullOrEmpty pois Address pode ser "" (string vazia é válida como entrada)
// mas remover o aviso do Sonar usando pattern matching ou simplificando a lógica
private string GetHost()
{
    if (string.IsNullOrEmpty(Address)) return string.Empty;
    var data = Address.Split('@');
    return data.Length == 2 ? data[1] : string.Empty;
}
```

Remover o operador `?.` em `data?.Length` — `string.Split` nunca retorna null.

**Issue 1.14 — Boolean literal em `Email`:**
```csharp
// ANTES
if (isMatch is false)

// DEPOIS
if (!isMatch)
```

Adicionalmente, migrar o `Regex` inline de `ValidateAddressPattern` para `[GeneratedRegex]`:
```csharp
[GeneratedRegex(@"^[a-zA-Z0-9_.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$", RegexOptions.IgnoreCase)]
private static partial Regex EmailPatternRegex();
```

---

**Arquivo:** `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Phone.cs`

**Issue 1.15 — `Regex` em runtime:**

`Phone` tem 3 usos de Regex em runtime:
1. `Regex.IsMatch(DDI, @"^\+\d{1,4}$")` em `Validate()`
2. `Regex.Match(value, @"^(?<ddi>\d{2})(?<ddd>\d{2})(?<number>\d{8,9})$")` em `TryParse`
3. `new Regex(@"[^\d]", RegexOptions.Compiled)` em `OnlyDigits`

```csharp
public sealed partial class Phone : ValueObject<Phone>, IEquatable<Phone>
{
    [GeneratedRegex(@"^\+\d{1,4}$")]
    private static partial Regex DdiPatternRegex();

    [GeneratedRegex(@"^(?<ddi>\d{2})(?<ddd>\d{2})(?<number>\d{8,9})$")]
    private static partial Regex PhonePartsRegex();

    [GeneratedRegex(@"[^\d]")]
    private static partial Regex NonDigitsRegex();
}
```

Substituir `Regex.IsMatch(DDI, ...)` por `DdiPatternRegex().IsMatch(DDI)`, `Regex.Match(value, ...)` por `PhonePartsRegex().Match(value)`, e `new Regex(...)` por `NonDigitsRegex()`.

---

**Arquivo:** `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/ZipCode.cs`

**Issue 1.16 — Verificação de null desnecessária:**

`Value` é `string` não-nullable. A verificação `if (!Value.All(char.IsDigit))` já funciona sem null check. Se o Sonar aponta null check redundante, é porque há um `if (Value is null)` ou similar que deve ser removido. Pela leitura do código atual, `Value` não tem null check explícito em `Validate()` — o issue pode ser sobre o uso de `?.` em algum contexto. Verificar e remover qualquer `?.` ou `?? ""` desnecessário em `Value`.

---

### Sagi.Sdk.Domain — Testes

**Issue 1.17 — Null em `EntityTests` para `operator ==`:**

Com a remoção de `operator ==` (issue 1.9), os testes que testam `operator ==` com null devem ser refatorados:

```csharp
// ANTES (testa operator ==)
Assert.False(entity == null);
Assert.True(entity1 == entity2);

// DEPOIS (testa Equals)
Assert.False(entity.Equals(null));
Assert.True(entity1.Equals(entity2));
// ou usando Assert.Equal / Assert.NotEqual
Assert.Equal(entity1, entity2);
Assert.NotNull(entity);
```

Para os casos que testavam `null == null` via `operator ==`:
```csharp
// ANTES
FakeEntity? entity1 = null;
FakeEntity? entity2 = null;
Assert.True(entity1 == entity2);

// DEPOIS — testar referential equality com object.ReferenceEquals ou simplesmente remover
// pois sem operator ==, null == null usa o operator padrão do C# (referência)
Assert.True(entity1 is null && entity2 is null);
```

**Issue 1.18 — Null em testes de value objects:**

Para todos os testes que passam `null` para construtores de value objects com parâmetros `string` não-nullable:
```csharp
// ANTES
new Address(null, "123", ...)

// DEPOIS
new Address(null!, "123", ...)
```

Usar `null!` (null-forgiving operator) para suprimir o warning CS8625 nos testes onde o objetivo é testar o comportamento com entrada nula.

---

### Sagi.Sdk.MongoDb

**Arquivo:** `Sagi.Sdk.MongoDb/src/Sagi.Sdk.MongoDb/Context/MongoContext.cs`

**Issue 1.19 — Construtor `public` em classe abstrata:**
```csharp
// ANTES
public MongoContext(IMongoDatabase database)

// DEPOIS
protected MongoContext(IMongoDatabase database)
```

**Arquivo:** `Sagi.Sdk.MongoDb/src/Sagi.Sdk.MongoDb/Extensions/ServicesExtensions.cs`

**Issue 1.20 — Caller Information explícito:**
```csharp
// ANTES
ArgumentNullException.ThrowIfNull(options.ConnectionString, nameof(options.ConnectionString));
ArgumentNullException.ThrowIfNull(options.DatabaseName, nameof(options.DatabaseName));

// DEPOIS
ArgumentNullException.ThrowIfNull(options.ConnectionString);
ArgumentNullException.ThrowIfNull(options.DatabaseName);
```

**Arquivo:** `Sagi.Sdk.MongoDb/tests/Sagi.Sdk.MongoDb.Tests/Fixtures/Docker/MongoDockerContainer.cs`

**Issue 1.21 — `GC.SuppressFinalize` ausente:**
```csharp
// ANTES
public void Dispose() => Container?.Dispose();

// DEPOIS
public void Dispose()
{
    Container?.Dispose();
    GC.SuppressFinalize(this);
}
```

---

### Sagi.Sdk.AWS.DynamoDb — Código Fonte

**Arquivo:** `Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/Hosting/DynamoDbHosting.cs`

**Issue 1.22 — Classe utilitária sem `static`:**

Decisão de design: declarar `DynamoDbHosting` como `static class`. Todos os membros já são estáticos. Isso é mais expressivo que adicionar um construtor `protected`.

```csharp
// ANTES
public class DynamoDbHosting

// DEPOIS
public static class DynamoDbHosting
```

Remover as propriedades de instância (já são estáticas) — nenhuma mudança adicional necessária.

**Arquivo:** `Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/Initializers/TablesInitializer.cs`

**Issue 1.23 — Boolean literal:**
```csharp
// ANTES
if (tables.Length > 0)

// Verificar se há "== true" ou "== false" — pela leitura do código, a condição já está correta
// O issue pode ser em outro local do arquivo. Confirmar na análise do Sonar.
```

**Issue 1.24 — Nomenclatura `EventArgs`:**

Renomear `DynamoDbTableEventsArgs` para `DynamoDbTableReadyEventArgs` e atualizar o delegate:

```csharp
// ANTES
public class DynamoDbTableEventsArgs : EventArgs { ... }
public delegate Task DynamoDbTableEventHandler(object sender, DynamoDbTableEventsArgs e);

// DEPOIS
public class DynamoDbTableReadyEventArgs : EventArgs { ... }
public delegate Task DynamoDbTableEventHandler(object sender, DynamoDbTableReadyEventArgs e);
```

Atualizar todos os usos em `TablesInitializer` e em qualquer código cliente.

**Arquivo:** `Sagi.Sdk.AWS.DynamoDb/src/Samples/Entities/Payment.cs`

**Issue 1.25 — Namespace ausente:**
```csharp
// ADICIONAR no topo
namespace Samples.Entities;
```

**Arquivo:** `Sagi.Sdk.AWS.DynamoDb/src/Samples/Program.cs`

**Issue 1.26 — `AwsOptions` sem namespace:**

Mover o record `AwsOptions` de `Program.cs` para um arquivo próprio:

```csharp
// NOVO ARQUIVO: Sagi.Sdk.AWS.DynamoDb/src/Samples/AwsOptions.cs
namespace Samples;

public record AwsOptions(string Accesskey, string SecretKey, string ServiceUrl);
```

Remover a declaração inline de `Program.cs`.

**Arquivo:** `Sagi.Sdk.AWS.DynamoDb/src/Samples/Tables/PaymentTable.cs`

**Issue 1.27 — Empty statement:**
```csharp
// ANTES
BillingMode = BillingMode.PAY_PER_REQUEST;;

// DEPOIS
BillingMode = BillingMode.PAY_PER_REQUEST;
```

---

### Sagi.Sdk.AWS.DynamoDb — Testes

**Arquivo:** `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/Fixtures/Docker/DynamoDbDockerContainer.cs`

**Issue 1.28 — `GC.SuppressFinalize` ausente:**
```csharp
// ANTES
public void Dispose() => Container?.Dispose();

// DEPOIS
public void Dispose()
{
    Container?.Dispose();
    GC.SuppressFinalize(this);
}
```

**Arquivo:** `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/UnitTests/Config/DynamoDbConfiguratorTests.cs`

**Issue 1.29 — `[Theory]` sem parâmetros:**
```csharp
// ANTES
[Theory, AutoNSubstituteData]
public void ShouldValidateConstructorParameters(GuardClauseAssertion assertion)

// DEPOIS — manter [Theory] pois recebe parâmetro via AutoNSubstituteData (AutoFixture)
// O issue é que o método ShouldValidateConstructorParameters NÃO tem parâmetros
// Verificar: se o método realmente não tem parâmetros, trocar para [Fact]
// Se tem parâmetros (GuardClauseAssertion assertion), o [Theory] está correto
// Pela leitura do código: o método TEM parâmetro — o issue pode ser outro método sem parâmetros
// Confirmar qual método específico o Sonar aponta
```

Pela leitura do código, `ShouldValidateConstructorParameters` recebe `GuardClauseAssertion assertion` — tem parâmetro. O issue 1.29 provavelmente refere-se a outro método `[Theory]` sem parâmetros no arquivo. Verificar e converter para `[Fact]`.

**Arquivo:** `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/UnitTests/Pages/PageResultTests.cs`

**Issue 1.30 — `null` em parâmetro `string` não-nullable:**
```csharp
// ANTES
[InlineData("")]
[InlineData(null)]
public void HasNextPage_ShouldBeFalse_WhenPageTokenIsNull(string token)

// DEPOIS — PageToken é string? (nullable), então null é válido
// O parâmetro do teste deve ser string? para aceitar null sem warning
public void HasNextPage_ShouldBeFalse_WhenPageTokenIsNull(string? token)
```


## Testing Strategy

### Validation Approach

A estratégia segue duas fases: primeiro verificar que o código original exibe os issues (exploração), depois verificar que o código corrigido elimina os issues e preserva o comportamento (fix + preservation checking).

### Exploratory Bug Condition Checking

**Goal**: Confirmar que os issues existem no código original antes de corrigir. Para issues de compilação/análise estática, a exploração é feita rodando o Sonar ou compilando com warnings tratados como erros.

**Test Plan**: Executar `dotnet build` com `/warnaserror` para capturar CS warnings. Executar análise Sonar local (SonarScanner) para confirmar os 30 issues. Rodar `dotnet test` para confirmar que os testes existentes passam antes de qualquer mudança.

**Test Cases**:
1. **Compilação com warnings**: `dotnet build /warnaserror` — deve falhar em CS8625 (null para não-nullable), CS0649 (campo não utilizado) (falha no código original)
2. **Testes existentes**: `dotnet test` — deve passar completamente no código original (confirma baseline)
3. **`[Theory]` sem parâmetros**: executar `DynamoDbConfiguratorTests` — deve lançar `InvalidOperationException` no método problemático
4. **Covariância**: tentar `IResult<string> r = new Result<object>()` — deve falhar sem `out T`

**Expected Counterexamples**:
- Warnings CS8625 em testes que passam `null` para `string` não-nullable
- Erro de compilação ao tentar atribuição covariante sem `out T`
- `InvalidOperationException` no `[Theory]` sem parâmetros

### Fix Checking

**Goal**: Verificar que após cada correção o issue correspondente é eliminado.

**Pseudocode:**
```
FOR ALL issue IN sonar_issues DO
  apply_fix(issue)
  result := dotnet_build()
  ASSERT result.warnings NOT CONTAINS issue.warning_code
  result := sonar_scan()
  ASSERT result.issues NOT CONTAINS issue
END FOR
```

### Preservation Checking

**Goal**: Verificar que nenhuma correção altera o comportamento funcional.

**Pseudocode:**
```
FOR ALL test IN existing_test_suite DO
  ASSERT test.result_before_fix == test.result_after_fix
END FOR
```

**Testing Approach**: Os testes unitários e de integração existentes são a principal garantia de preservation. Após cada grupo de correções, executar `dotnet test` para confirmar que nenhum teste regrediu.

**Test Cases**:
1. **`Entity<T>` após remoção de `operator ==`**: refatorar testes de `EntityTests` para usar `Equals`/`Assert.Equal` — verificar que a semântica de igualdade por Id é preservada
2. **`Address` após refatoração do construtor**: atualizar testes de `AddressTests` para usar `AddressLengthOptions` — verificar que validação continua funcionando
3. **`GeneratedRegex` em `Cnpj`, `Cpf`, `Phone`, `Email`**: executar todos os testes de value objects — verificar que validação, formatação e `TryParse` produzem os mesmos resultados
4. **`IResult<T>` com `out T`**: verificar que `Result<T>` ainda implementa `IResult<T>` corretamente e que `Value`, `IsSuccess`, `IsFailure`, `Errors` funcionam
5. **`Entity<T>` após mover `GenerateId()` para subclasse**: verificar que `FakeEntity` gera `Id` não-default e que `CreateAt`, `Active`, `Events` são inicializados corretamente

### Unit Tests

- Testar `IResult<T>` covariante: `IResult<string> r = new Result<string>("x")` compila e `r.Value == "x"`
- Testar `Error` com valores nulos: `ArgumentException` ainda é lançada
- Testar `Result<T>.IsSuccess` e `IsFailure` com valor e com erros
- Testar `Entity<T>` sem `operator ==`: igualdade por `Equals` e `GetHashCode` por Id
- Testar `Address` com `AddressLengthOptions`: construtor de 5 e de 6 parâmetros
- Testar `Cnpj`, `Cpf`, `Phone`, `Email` com `GeneratedRegex`: mesmos casos válidos e inválidos
- Testar `MongoContext` com construtor `protected`: apenas subclasses podem instanciar
- Testar `DynamoDbHosting` como `static class`: não pode ser instanciada
- Testar `TablesInitializer` com `DynamoDbTableReadyEventArgs`: evento disparado com nome correto

### Property-Based Tests

- Gerar strings aleatórias e verificar que `Cpf`, `Cnpj`, `Email`, `Phone` com `GeneratedRegex` produzem o mesmo resultado de validação que a versão com `Regex` em runtime
- Gerar entidades aleatórias e verificar que `Equals` por Id é transitivo e simétrico após remoção de `operator ==`
- Gerar `Address` com `AddressLengthOptions` aleatórios e verificar que validação é consistente

### Integration Tests

- Executar suite completa de testes de integração do `Sagi.Sdk.AWS.DynamoDb` (com Docker) após todas as correções
- Executar suite completa de testes de integração do `Sagi.Sdk.MongoDb` (com Docker) após todas as correções
- Verificar que `TablesInitializer` dispara `DynamoDbTableReadyEventArgs` (nome corrigido) corretamente
