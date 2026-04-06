# Bugfix Requirements Document

## Introduction

O monorepo Sagi SDKs (.NET) apresenta um conjunto de problemas reportados pelo SonarQube distribuídos em 4 projetos: `Sagi.Sdk.Results`, `Sagi.Sdk.Domain`, `Sagi.Sdk.MongoDb` e `Sagi.Sdk.AWS.DynamoDb`. Os problemas variam de severidade Blocker a Info e incluem: violações de design de API (covariância ausente, construtores com visibilidade incorreta), code smells de manutenibilidade (campos não utilizados, literais booleanos desnecessários, argumentos que ocultam caller information), riscos de runtime (chamada a método overridable no construtor, operator == sem suporte a null em testes), e problemas de qualidade em testes (null literal em tipos não-nullable, Theory sem parâmetros). A correção elimina todos os issues abertos no Sonar sem alterar o comportamento funcional dos SDKs.

## Bug Analysis

### Current Behavior (Defect)

**Sagi.Sdk.Results**

1.1 WHEN `IResult<T>` é utilizado em contextos de covariância THEN o sistema não permite atribuição covariante porque o parâmetro `T` não possui o keyword `out`

1.2 WHEN `Error` é construído com `code` ou `message` THEN o sistema passa `nameof(code)` e `nameof(message)` explicitamente para `ArgumentException.ThrowIfNullOrEmpty`, ocultando o caller information automático

1.3 WHEN `Result<T>.IsSuccess` é avaliado THEN o sistema compara `Errors.Any() == false` com Boolean literal desnecessário

**Sagi.Sdk.Domain — Código fonte**

1.4 WHEN `Event<T>` é instanciado diretamente (sem herança) THEN o sistema permite isso porque o construtor é `public` em vez de `protected`

1.5 WHEN `Validateble.AddError` ou `Validateble.AddErrors` é chamado THEN o sistema passa o nome do parâmetro explicitamente para `ArgumentNullException.ThrowIfNull`, ocultando o caller information automático

1.6 WHEN `Entity<T>` é compilado THEN o sistema mantém o campo privado `_errors` declarado mas nunca utilizado

1.7 WHEN uma instância de `Entity<T>` é construída THEN o sistema chama `GenerateId()` — método virtual/abstrato — diretamente no construtor, o que pode causar comportamento indefinido em subclasses que sobrescrevem o método

1.8 WHEN `Entity<T>` é compilado THEN o sistema mantém inicializador de membro redundante em `Active = true` (L18) porque todos os construtores já atribuem o valor

1.9 WHEN `Entity<T>` é compilado THEN o sistema define sobrecarga de `operator ==` (L62) que o SonarQube classifica como Blocker por conflito com a semântica de igualdade por referência esperada para entidades

1.10 WHEN `Address` é construído THEN o sistema exige 9 parâmetros no construtor, excedendo o máximo autorizado de 7

1.11 WHEN `Cnpj.Validate()` executa validação por regex THEN o sistema instancia `Regex` em runtime em vez de usar `GeneratedRegexAttribute` para compilação em compile-time

1.12 WHEN `Cpf.Validate()` executa validação por regex THEN o sistema instancia `Regex` em runtime em vez de usar `GeneratedRegexAttribute` para compilação em compile-time

1.13 WHEN `Email.Validate()` executa verificações de null THEN o sistema realiza verificações de null desnecessárias (L32, L43, L54, L65) em contextos onde o compilador já garante não-nulidade

1.14 WHEN `Email.IsValid` é avaliado THEN o sistema usa Boolean literal desnecessário (L104)

1.15 WHEN `Phone.Validate()` executa validação por regex THEN o sistema instancia `Regex` em runtime (L46, L63, L93) em vez de usar `GeneratedRegexAttribute`

1.16 WHEN `ZipCode.Validate()` executa verificação de null THEN o sistema realiza verificação de null desnecessária (L26)

**Sagi.Sdk.Domain — Testes**

1.17 WHEN `EntityTests` testa `operator ==` e `operator !=` com null THEN o sistema produz warnings de compilação (L100, L109, L135, L144) porque `null` é passado para parâmetros não-nullable

1.18 WHEN testes de value objects (`AddressTests`, `CityTests`, `EmailTests`, `NameTests`, `NeighborhoodTests`, `PhoneTests`, `StateTests`, `ZipCodeTests`) passam `null` para construtores THEN o sistema produz warnings de compilação porque os parâmetros são não-nullable

**Sagi.Sdk.MongoDb**

1.19 WHEN `MongoContext` é instanciado diretamente (sem herança) THEN o sistema permite isso porque o construtor é `public` em vez de `protected`

1.20 WHEN `ServicesExtensions.AddMongo` registra serviços THEN o sistema passa argumentos explícitos de caller information (L57, L58) desnecessariamente

1.21 WHEN `MongoDockerContainer.Dispose()` é chamado THEN o sistema não chama `GC.SuppressFinalize(this)`, o que pode causar chamada dupla ao finalizador em subclasses

**Sagi.Sdk.AWS.DynamoDb — Código fonte**

1.22 WHEN `DynamoDbHosting` é utilizado THEN o sistema não possui construtor `protected` nem keyword `static`, violando a regra de design para classes utilitárias

1.23 WHEN `TablesInitializer` avalia condição booleana (L35) THEN o sistema usa Boolean literal desnecessário

1.24 WHEN `TablesInitializer` dispara evento (L58) THEN o sistema usa uma classe de evento que não termina com `EventArgs`, violando a convenção de nomenclatura

1.25 WHEN `Payment` (sample) é compilado THEN o sistema declara a classe fora de um namespace nomeado

1.26 WHEN `AwsOptions` (sample) é compilado THEN o sistema declara a classe fora de um namespace nomeado

1.27 WHEN `PaymentTable` é compilado THEN o sistema contém um empty statement (L23)

**Sagi.Sdk.AWS.DynamoDb — Testes**

1.28 WHEN `DynamoDbDockerContainer.Dispose()` é chamado THEN o sistema não chama `GC.SuppressFinalize(this)`

1.29 WHEN `DynamoDbConfiguratorTests` executa o método de Theory (L25) THEN o sistema define um método `[Theory]` sem parâmetros, o que é inválido

1.30 WHEN `PageResultTests` cria instância com token nulo (L20) THEN o sistema passa `null` para o parâmetro `token` do tipo `string` não-nullable

---

### Expected Behavior (Correct)

**Sagi.Sdk.Results**

2.1 WHEN `IResult<T>` é utilizado em contextos de covariância THEN o sistema SHALL permitir atribuição covariante com o parâmetro `T` declarado como `out T`

2.2 WHEN `Error` é construído com `code` ou `message` THEN o sistema SHALL chamar `ArgumentException.ThrowIfNullOrEmpty(code)` e `ArgumentException.ThrowIfNullOrEmpty(message)` sem o argumento de nome explícito, deixando o caller information ser inferido automaticamente

2.3 WHEN `Result<T>.IsSuccess` é avaliado THEN o sistema SHALL usar `!Errors.Any()` ou expressão equivalente sem Boolean literal

**Sagi.Sdk.Domain — Código fonte**

2.4 WHEN `Event<T>` é instanciado THEN o sistema SHALL exigir herança, pois o construtor SHALL ser `protected`

2.5 WHEN `Validateble.AddError` ou `Validateble.AddErrors` é chamado THEN o sistema SHALL chamar `ArgumentNullException.ThrowIfNull(error)` e `ArgumentNullException.ThrowIfNull(errors)` sem o argumento de nome explícito

2.6 WHEN `Entity<T>` é compilado THEN o sistema SHALL não conter o campo privado `_errors` (removido)

2.7 WHEN uma instância de `Entity<T>` é construída THEN o sistema SHALL inicializar `Id` sem chamar `GenerateId()` diretamente no construtor; a geração do Id SHALL ser movida para um mecanismo que não invoque método virtual no construtor (ex: propriedade com inicialização lazy ou atribuição via parâmetro)

2.8 WHEN `Entity<T>` é compilado THEN o sistema SHALL não conter o inicializador redundante removido, mantendo apenas a atribuição no construtor

2.9 WHEN `Entity<T>` implementa igualdade THEN o sistema SHALL remover a sobrecarga de `operator ==` ou refatorá-la de forma que o SonarQube não a classifique como Blocker, mantendo a semântica de igualdade por Id

2.10 WHEN `Address` é construído THEN o sistema SHALL aceitar no máximo 7 parâmetros diretos, com os demais agrupados em um objeto de configuração, record ou builder

2.11 WHEN `Cnpj` realiza validação por regex THEN o sistema SHALL usar `[GeneratedRegex]` para compilar a expressão regular em compile-time

2.12 WHEN `Cpf` realiza validação por regex THEN o sistema SHALL usar `[GeneratedRegex]` para compilar a expressão regular em compile-time

2.13 WHEN `Email.Validate()` executa verificações THEN o sistema SHALL remover as verificações de null desnecessárias (L32, L43, L54, L65)

2.14 WHEN `Email.IsValid` é avaliado THEN o sistema SHALL usar expressão booleana direta sem literal desnecessário

2.15 WHEN `Phone` realiza validação por regex THEN o sistema SHALL usar `[GeneratedRegex]` para compilar as expressões regulares em compile-time

2.16 WHEN `ZipCode.Validate()` executa verificação THEN o sistema SHALL remover a verificação de null desnecessária

**Sagi.Sdk.Domain — Testes**

2.17 WHEN `EntityTests` testa `operator ==` e `operator !=` com null THEN o sistema SHALL usar `null!` (null-forgiving operator) ou refatorar os testes para eliminar os warnings de compilação

2.18 WHEN testes de value objects passam null para construtores THEN o sistema SHALL usar `null!` ou `default!` nos locais identificados para eliminar os warnings de compilação

**Sagi.Sdk.MongoDb**

2.19 WHEN `MongoContext` é instanciado THEN o sistema SHALL exigir herança, pois o construtor SHALL ser `protected`

2.20 WHEN `ServicesExtensions.AddMongo` registra serviços THEN o sistema SHALL chamar os métodos sem os argumentos de caller information explícitos (L57, L58)

2.21 WHEN `MongoDockerContainer.Dispose()` é chamado THEN o sistema SHALL chamar `GC.SuppressFinalize(this)` ao final do método

**Sagi.Sdk.AWS.DynamoDb — Código fonte**

2.22 WHEN `DynamoDbHosting` é utilizado THEN o sistema SHALL ter construtor `protected` ou ser declarado como `static class`

2.23 WHEN `TablesInitializer` avalia condição booleana (L35) THEN o sistema SHALL usar expressão booleana direta sem literal desnecessário

2.24 WHEN `TablesInitializer` dispara evento THEN o sistema SHALL usar uma classe de evento cujo nome termine com `EventArgs`

2.25 WHEN `Payment` (sample) é compilado THEN o sistema SHALL declarar a classe dentro de um namespace nomeado

2.26 WHEN `AwsOptions` (sample) é compilado THEN o sistema SHALL declarar a classe dentro de um namespace nomeado

2.27 WHEN `PaymentTable` é compilado THEN o sistema SHALL não conter empty statements

**Sagi.Sdk.AWS.DynamoDb — Testes**

2.28 WHEN `DynamoDbDockerContainer.Dispose()` é chamado THEN o sistema SHALL chamar `GC.SuppressFinalize(this)` ao final do método

2.29 WHEN `DynamoDbConfiguratorTests` define um método de Theory THEN o sistema SHALL garantir que o método `[Theory]` possua ao menos um parâmetro com `[InlineData]` ou equivalente

2.30 WHEN `PageResultTests` cria instância com token THEN o sistema SHALL usar `string.Empty` ou um valor não-nulo válido em vez de `null` para o parâmetro `token`

---

### Unchanged Behavior (Regression Prevention)

3.1 WHEN `IResult<T>` é usado para retornar valores tipados THEN o sistema SHALL CONTINUE TO expor `Value`, `IsSuccess`, `IsFailure` e `Errors` com a mesma semântica

3.2 WHEN `Error` é construído com `code` e `message` válidos THEN o sistema SHALL CONTINUE TO lançar `ArgumentException` para valores nulos ou vazios

3.3 WHEN `Result<T>` é construído com valor ou com erros THEN o sistema SHALL CONTINUE TO retornar `IsSuccess = true` para valor e `IsFailure = true` para erros

3.4 WHEN `Event<T>` é herdado e instanciado por subclasse THEN o sistema SHALL CONTINUE TO inicializar `Timestamp`, `AggregateId`, `AggregateVersion` e `Name` corretamente

3.5 WHEN `Validateble` acumula erros via `AddError` ou `AddErrors` THEN o sistema SHALL CONTINUE TO expor `Errors`, `IsValid` e `IsInvalid` com a mesma semântica

3.6 WHEN `Entity<T>` é herdado e instanciado THEN o sistema SHALL CONTINUE TO gerar `Id`, `CreateAt`, `Active = true` e expor `Events`, `Version`, `AddEvent`, `LoadEvent`, `ClearEvents`

3.7 WHEN dois `Entity<T>` são comparados por igualdade THEN o sistema SHALL CONTINUE TO comparar por `Id` (não por referência)

3.8 WHEN `Address` é construído com todos os campos válidos THEN o sistema SHALL CONTINUE TO validar e expor `Street`, `Number`, `Complement`, `Neighborhood`, `ZipCode` corretamente

3.9 WHEN `Cnpj`, `Cpf`, `Email`, `Phone`, `ZipCode` recebem entradas válidas THEN o sistema SHALL CONTINUE TO validar, formatar e expor as propriedades com a mesma semântica

3.10 WHEN `Cnpj` recebe CNPJ alfanumérico (IN RFB nº 2.229/2024) THEN o sistema SHALL CONTINUE TO validar corretamente usando o algoritmo alfanumérico já implementado

3.11 WHEN `MongoContext<T>` é herdado e utilizado THEN o sistema SHALL CONTINUE TO executar `GetAll`, `GetByIdAsync`, `ExistsAsync`, `InsertAsync`, `InsertMany`, `UpdateAsync`, `DeleteAsync` com a mesma semântica

3.12 WHEN `ServicesExtensions.AddMongo` é chamado THEN o sistema SHALL CONTINUE TO registrar o `IMongoDatabase` e configurar convenções camelCase e enum como string

3.13 WHEN `DynamoDbContext<TModel>` executa operações THEN o sistema SHALL CONTINUE TO realizar `GetSingleAsync`, `GetAll`, `SaveAsync`, `DeleteAsync` com a mesma semântica

3.14 WHEN `TablesInitializer` inicializa tabelas THEN o sistema SHALL CONTINUE TO criar tabelas ausentes e disparar o evento de tabela pronta (com o nome corrigido terminando em `EventArgs`)

3.15 WHEN `PageQuery` e `PageResult<TResult>` são utilizados THEN o sistema SHALL CONTINUE TO paginar resultados com `PageSize`, `PageToken` e `HasNextPage` com a mesma semântica

3.16 WHEN todos os testes existentes são executados THEN o sistema SHALL CONTINUE TO passar sem regressões funcionais
