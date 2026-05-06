# Sagi SDKs — Contexto do Projeto

## Visão Geral

Monorepo .NET 10 com 5 SDKs independentes para integração com serviços e padrões utilizados nos projetos Sagi. Cada SDK segue a mesma estrutura: `src/` com a biblioteca e `tests/` com os testes.

**Solução:** `Sagi.Sdk.sln`  
**Runtime:** .NET 10.0 (`global.json` com `rollForward: latestMajor`)  
**Testes:** xUnit + NSubstitute + AutoFixture + Bogus + FsCheck (para property-based testing)  
**Cobertura:** coverlet.collector

---

## Projetos

### 1. Sagi.Sdk.Results
**Caminho:** `Sagi.Sdk.Results/src/Sagi.Sdk.Results/`  
**Propósito:** Padronização de retornos e tratamento de erros.

**Arquivos principais:**
- `Error.cs` — Implementa `IError` com `Code` e `Message`. Usa `ArgumentException.ThrowIfNullOrEmpty` no construtor.
- `Result.cs` — `Result<T>` implementa `IResult<T>`. Aceita valor ou coleção de erros. Lança `ArgumentNullException` para nulos.
- `Contracts/IError.cs` — Interface com `Code` e `Message`.
- `Contracts/IResult.cs` — Interfaces `IResult` e `IResult<T>`.

**Dependências:** Nenhuma (projeto base).

---

### 2. Sagi.Sdk.Domain
**Caminho:** `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/`  
**Propósito:** Abstrações DDD — entidades, value objects, eventos.  
**Depende de:** `Sagi.Sdk.Results`

**Contratos (`Contracts/`):**
- `Validateble.cs` — Classe base abstrata com lista de `IError`, métodos `AddError`, `AddErrors`, `ClearErrors`, `Validate()` abstrato. Método `Validate(Validateble, errorCode, nullMessage)` para validação encadeada.
- `Event<T>.cs` — Classe base para eventos de domínio com `AggregateId`, `AggregateVersion`, `Timestamp`, `Name` abstrato.

**Entidades (`Entities/`):**
- `Entity<T>.cs` — Classe base abstrata. Herda `Validateble`. Possui `Id`, `Active`, `CreateAt`, `UpdateAt`, `Version`, `Events`. Métodos `AddEvent`, `LoadEvent`, `ClearEvents`. Implementa `==`, `!=`, `Equals`, `GetHashCode`.

**Value Objects (`ValueObjects/`):**
- `ValueObject<TChild>.cs` — Abstrato, herda `Validateble`, implementa `IEquatable<TChild>`.
- `Name.cs` — Nome completo com `FirstName`, `LastName`, `FullName`, `MinLength`, `MaxLength`. `TryParse` e implicit operator de `string`.
- `Email.cs` — Endereço de e-mail com `Address`, `User`, `Host`, `Domain`, `TopLevelDomain`. Validação por regex.
- `Phone.cs` — Telefone com `DDI`, `DDD`, `Number`. `TryParse` extrai partes via regex. Formato `+55 (11) 9 1234-5678`.
- `Cpf.cs` — CPF com validação de dígitos verificadores. `Formatted` como `000.000.000-00`.
- `Cnpj.cs` — CNPJ com validação de dígitos verificadores. `Formatted` como `00.000.000/0000-00`.
- `ZipCode.cs` — CEP com 8 dígitos. `Formatted` como `00.000-000`.
- `Country.cs` — País com `Name`, `Abbreviation` (2 chars), `IsoCode` (3 chars).
- `State.cs` — Estado com `Name`, `Abbreviation` (2 chars), `Country`.
- `City.cs` — Cidade com `Name`, `State`.
- `Neighborhood.cs` — Bairro com `Name`, `City`.
- `Address.cs` — Endereço completo com `Street`, `Number`, `Complement`, `Neighborhood`, `ZipCode`. Comprimentos configuráveis.

**Padrão dos Value Objects:**
- Todos herdam `ValueObject<TChild>`
- Todos implementam `Validate()`, `Equals(TChild?)`, `Equals(object?)`, `GetHashCode()`, `ToString()`
- Todos têm `TryParse(...)` estático
- Maioria tem `implicit operator` para conversão

---

### 3. Sagi.Sdk.MongoDb
**Caminho:** `Sagi.Sdk.MongoDb/src/Sagi.Sdk.MongoDb/`  
**Propósito:** Integração simplificada com MongoDB.  
**Dependências:** `MongoDB.Driver 3.3.0`, `Microsoft.Extensions.*`

**Arquivos principais:**
- `Context/Document.cs` — Classe base para documentos. `Id` gerado via `ObjectId.GenerateNewId()`.
- `Context/IMongoContext<T>.cs` — Interface com `GetAll`, `GetByIdAsync`, `ExistsAsync`, `InsertAsync`, `InserMany`, `UpdateAsync`, `DeleteAsync`.
- `Context/MongoContext<T>.cs` — Implementação abstrata de `IMongoContext<T>`. Requer `CollectionName` abstrato. Recebe `IMongoDatabase` no construtor.
- `Extensions/ServicesExtensions.cs` — `AddMongo(IConfiguration)` e `AddMongo(Action<MongoOptions>)`. Configura convenções camelCase e enum como string.
- `Options/MongoOptions.cs` — `ConnectionString` e `DatabaseName`. Section: `"Mongo"`.

**Registro DI:**
```csharp
services.AddMongo(configuration); // via IConfiguration
services.AddMongo(opt => {        // via Action
    opt.ConnectionString = "...";
    opt.DatabaseName = "...";
});
```

---

### 4. Sagi.Sdk.AWS.DynamoDb
**Caminho:** `Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/`  
**Propósito:** Integração com Amazon DynamoDB.  
**Dependências:** `AWSSDK.DynamoDBv2 3.7.x`, `AWSSDK.Extensions.NETCore.Setup`, `Microsoft.Extensions.Hosting 9.x`  
**Depende de:** `Sagi.Sdk.Results`

**Arquivos principais:**
- `Config/DynamoDbConfigurator.cs` — Herda `AWSOptions`. Configura `BillingMode`, credenciais, `ServiceURL`, `InitializeDb`, lista de `CreateTableRequest`. `UseAWSService` é `true` quando não há credenciais explícitas.
- `Context/IDynamoDbContext<TModel>.cs` — Interface com `GetSingleAsync`, `GetAll`, `SaveAsync`, `DeleteAsync`.
- `Context/DynamoDbContext<TModel>.cs` — Implementação usando `IDynamoDBContext` (alto nível) e `IAmazonDynamoDB` (baixo nível para scan/delete).
- `Extensions/DynamoConfigExtensions.cs` — `AddDynamoDb(Action<DynamoDbConfigurator>)`. Registra cliente, contexto, `IDynamoDbContext<>` genérico e opcionalmente o inicializador de tabelas.
- `Extensions/DynamoDbAttributeValueExtensions.cs` — Serialização/deserialização de `PageToken` em Base64+JSON.
- `Factories/AmazonDynamoDBClientFactory.cs` — Cria `AmazonDynamoDBClient` com ou sem `SessionToken`.
- `Hosting/DynamoDbHosting.cs` — Host standalone para rodar DynamoDB fora de uma aplicação web.
- `Initializers/IDynamoDbTableInitializer.cs` — Interface para inicialização de tabelas.
- `Initializers/TablesInitializer.cs` — Cria tabelas se não existirem. Dispara evento `TableIsReady`.
- `Initializers/DatabaseContextInitializer.cs` — `BackgroundService` que chama `ConfigureAsync()` na inicialização.
- `Pages/PageQuery.cs` — Paginação com `PageSize` (1–100) e `PageToken`. Usa `Error` do `Sagi.Sdk.Results`.
- `Pages/PageResult<TResult>.cs` — Resultado paginado com `Items`, `PageToken`, `HasNextPage`.

**Registro DI:**
```csharp
services.AddDynamoDb(cfg => {
    cfg.ServiceURL = "http://localhost:8000"; // local
    cfg.Accesskey = "root";
    cfg.SecretKey = "secret";
    cfg.InitializeDb = true;
    cfg.ConfigureTable(new CreateTableRequest { ... });
});
```

**Docker local:**
```yaml
# Sagi.Sdk.AWS.DynamoDb/docker-compose.yaml
# DynamoDB local na porta 8000
# DynamoDB Admin na porta 8001
```

---

### 5. Sagi.Sdk.DotEnv
**Caminho:** `Sagi.Sdk.DotEnv/src/Sagi.Sdk.DotEnv/`  
**Propósito:** Integração de arquivos `.env` ao pipeline de configuração nativo do .NET (`IConfiguration` / `IConfigurationBuilder`).  
**Dependências:** `Microsoft.Extensions.Configuration 8.0.0`

**Arquivos principais:**
- `Options/DotEnvOptions.cs` — `Directory` (default `Directory.GetCurrentDirectory()`) e `FileName` (default `".env"`). Sem validação própria.
- `Parser/DotEnvParser.cs` — Classe estática. `Parse(IEnumerable<string>)` retorna `IReadOnlyDictionary<string, string>`. Ignora linhas vazias, comentários (`#`), linhas sem `=` e chaves vazias. Split no primeiro `=` para preservar `=` no valor.
- `Provider/DotEnvSource.cs` — Implementa `IConfigurationSource`. Valida `Directory` e `FileName` com `ArgumentException.ThrowIfNullOrEmpty` no construtor.
- `Provider/DotEnvProvider.cs` — Herda `ConfigurationProvider`. `Load()` lê o arquivo via `Path.Combine`, delega ao `DotEnvParser` e popula `Data` com `StringComparer.OrdinalIgnoreCase`. Arquivo ausente não lança exceção.
- `Extensions/ConfigurationBuilderExtensions.cs` — `AddDotEnv()` e `AddDotEnv(Action<DotEnvOptions>)`.

**Registro:**
```csharp
builder.AddDotEnv();                          // defaults: diretório atual + ".env"
builder.AddDotEnv(opt => {
    opt.Directory = "/app/config";
    opt.FileName  = ".env.production";
});
```

**Testes:** xUnit (unitários) + FsCheck.Xunit (property-based, 4 propriedades com `[Property(MaxTest = 100)]`).

---

## Convenções de Código

Definidas no `.editorconfig`:
- Indentação: 4 espaços (C#), 2 espaços (XML/csproj)
- `end_of_line = crlf`
- Sem `var` explícito (preferir tipos explícitos)
- PascalCase para tipos, métodos, propriedades, campos públicos
- camelCase para variáveis locais e parâmetros
- `_camelCase` para campos privados
- `s_camelCase` para campos privados estáticos
- Interfaces com prefixo `I`
- Sem `this.` qualificador
- Campos `readonly` preferidos (`dotnet_style_readonly_field = true:warning`)

---

## Convenções de Commit

Todos os commits devem seguir o padrão **Conventional Commits** (Semantic Commit) para manter o histórico legível e permitir geração automática de changelogs.

### Formato

```
<tipo>(<escopo>): <assunto>

<corpo>

<rodapé>
```

### Regras

1. **Assunto (primeira linha):**
   - Máximo de 50 caracteres (limite padrão do Git)
   - Minúscula após o tipo
   - Sem ponto final
   - Imperativo ("adiciona", não "adicionado" ou "adicionando")

2. **Corpo (opcional, mas recomendado):**
   - Separado do assunto por uma linha em branco
   - Máximo de 72 caracteres por linha
   - Explica **o que** e **por que**, não **como**
   - Estilo release notes: lista de mudanças, impactos, breaking changes

3. **Rodapé (opcional):**
   - `BREAKING CHANGE:` para mudanças incompatíveis
   - `Refs:` para referências a issues/PRs
   - `Co-authored-by:` para co-autores

### Tipos

| Tipo | Descrição | Exemplo |
|---|---|---|
| `feat` | Nova funcionalidade | `feat(dotenv): adiciona suporte a arquivos .env` |
| `fix` | Correção de bug | `fix(parser): corrige parsing de valores com =` |
| `docs` | Documentação | `docs(readme): atualiza exemplos de uso` |
| `style` | Formatação, espaços, ponto e vírgula | `style(domain): aplica editorconfig` |
| `refactor` | Refatoração sem mudança de comportamento | `refactor(results): extrai validação para método` |
| `perf` | Melhoria de performance | `perf(mongo): otimiza query de busca` |
| `test` | Adição ou correção de testes | `test(dotenv): adiciona property tests` |
| `build` | Build, dependências, CI/CD | `build: atualiza para .NET 10` |
| `ci` | Configuração de CI/CD | `ci: adiciona workflow de testes` |
| `chore` | Tarefas de manutenção | `chore: atualiza .gitignore` |
| `revert` | Reverte commit anterior | `revert: reverte feat(dotenv)` |

### Escopos

Use o nome do SDK ou componente afetado:
- `results`, `domain`, `mongo`, `dynamodb`, `dotenv`
- `tests`, `docs`, `build`, `ci`
- Omita o escopo se a mudança afeta múltiplos SDKs

### Exemplos

**Exemplo 1: Nova feature com corpo detalhado**
```
feat(dotenv): adiciona integração com IConfiguration

Implementa DotEnvProvider e DotEnvSource para carregar arquivos .env
no pipeline de configuração nativo do .NET.

Mudanças:
- DotEnvParser: parsing de linhas com suporte a comentários e valores com =
- DotEnvProvider: herda ConfigurationProvider, arquivo ausente não lança exceção
- ConfigurationBuilderExtensions: AddDotEnv() e AddDotEnv(Action<DotEnvOptions>)
- 25 testes (8 unitários + 4 property-based com FsCheck)

Refs: #42
```

**Exemplo 2: Bugfix**
```
fix(domain): corrige validação de CNPJ alfanumérico

Ajusta regex para aceitar caracteres A-Z nas 12 primeiras posições
conforme IN RFB nº 2.229/2024.

BREAKING CHANGE: CNPJs com letras minúsculas agora são normalizados
para maiúsculas antes da validação.
```

**Exemplo 3: Documentação**
```
docs(readme): adiciona Sagi.Sdk.DotEnv à lista de projetos
```

**Exemplo 4: Build/infra**
```
build: atualiza todos os projetos para .NET 10

Migra TargetFramework de net8.0 para net10.0 em todos os csproj.
Atualiza global.json e steering com nova versão do runtime.
```

**Exemplo 5: Refatoração**
```
refactor(dotenv): extrai validadores para classes internas

Move lógica de validação numérica e alfanumérica para
NumericValidator e AlphanumericValidator como private static class.

Sem mudança de comportamento externo.
```

### Ferramentas

Para validar commits localmente antes do push:
```sh
# Instalar commitlint (opcional)
npm install -g @commitlint/cli @commitlint/config-conventional
```

---

## Estrutura de Testes

Cada projeto tem testes em `tests/`:
- `Sagi.Sdk.Domain.Tests` — Testa `Entity`, `Validateble`, value objects
- `Sagi.Sdk.Results.Tests` — Testa `Error` e `Result<T>`
- `Sagi.Sdk.MongoDb.Tests` — Testa `Document`, `MongoContext`, `ServicesExtensions`
- `Sagi.Sdk.AWS.DynamoDb.Tests` — Testa contexto DynamoDB com FluentDocker
- `Sagi.Sdk.DotEnv.Tests` — Testa `DotEnvParser`, `DotEnvProvider`, `DotEnvSource`, `ConfigurationBuilderExtensions` com xUnit e FsCheck

**Fakes usados nos testes:**
- `FakeEntity : Entity<Guid>` — expõe métodos protegidos para teste
- `FakeEvent : Event<Guid>` — evento de teste com `Subject` e `Message`
- `FakeDocument : Document` — documento MongoDB de teste
- `FakeContext : MongoContext<FakeDocument>` — contexto MongoDB de teste
- `FakeModel` — modelo DynamoDB de teste com `TABLE_NAME = "fakeTable"`
- `FakeSearch : AsyncSearch<FakeModel>` — mock de busca assíncrona DynamoDB

**Property-Based Testing (DotEnv):**
- `DotEnvParserPropertyTests.cs` — 4 propriedades com FsCheck.Xunit:
  - Property 1: Round-trip de parsing (Requirements 1.3, 3.1, 3.7, 4.3, 5.6)
  - Property 2: Valores com `=` preservados (Requirement 3.5)
  - Property 3: Linhas inválidas não afetam o resultado (Requirements 3.2, 3.3, 3.4)
  - Property 4: Idempotência do parsing (Requirements 3.1, 3.7)

**Executar testes:**
```sh
dotnet test
```

---

## Dependências entre Projetos

```
Sagi.Sdk.Results          (base, sem dependências)
    ↑
Sagi.Sdk.Domain           (depende de Results)
    ↑
Sagi.Sdk.AWS.DynamoDb     (depende de Results)
Sagi.Sdk.MongoDb          (sem dependência dos outros SDKs)
Sagi.Sdk.DotEnv           (sem dependência dos outros SDKs)
```

---

## Histórico de Alterações

### [2026-05] Multi-Targeting — Suporte a .NET 8 e .NET 9

#### Contexto

Necessidade de retrocompatibilidade com .NET 8 e .NET 9 para os 5 SDKs do monorepo, mantendo suporte a .NET 10.

#### Mudanças

Todos os 5 projetos de produção foram atualizados para multi-targeting:
- `Sagi.Sdk.Results/src/Sagi.Sdk.Results/Sagi.Sdk.Results.csproj`
- `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Sagi.Sdk.Domain.csproj`
- `Sagi.Sdk.DotEnv/src/Sagi.Sdk.DotEnv/Sagi.Sdk.DotEnv.csproj`
- `Sagi.Sdk.MongoDb/src/Sagi.Sdk.MongoDb/Sagi.Sdk.MongoDb.csproj`
- `Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/Sagi.Sdk.AWS.DynamoDb.csproj`

Cada `.csproj` agora contém `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>` em lugar de `<TargetFramework>net10.0</TargetFramework>`.

Projetos de teste mantêm `<TargetFramework>net10.0</TargetFramework>` (sem multi-targeting).

#### Decisões de Design

- **Sem mudanças de código:** Nenhuma API exclusiva de .NET 10 foi usada nos SDKs, portanto nenhuma compilação condicional foi necessária.
- **Sem ajustes de dependências:** Todas as versões atuais de pacotes NuGet já suportam net8.0 (MongoDB.Driver 3.3.0, AWSSDK.* 3.7.x, Microsoft.Extensions.* 9.0.5).
- **MSBuild automático:** O MSBuild resolve automaticamente `<ProjectReference>` para o TFM correspondente (Domain → Results, DynamoDb → Results).
- **Testes em net10.0 apenas:** Projetos de teste compilam apenas para .NET 10 para simplificar CI/CD e reduzir tempo de build.

#### Padrão a Seguir

Quando adicionar novos SDKs ou estender existentes:
1. Configure multi-targeting nos projetos de produção: `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
2. Mantenha testes em `<TargetFramework>net10.0</TargetFramework>`
3. Use `#if NET9_0_OR_GREATER` ou `#if NET10_0_OR_GREATER` apenas se APIs exclusivas forem necessárias
4. Valide compatibilidade de dependências NuGet com net8.0 antes de adicionar

---

### [2026-05] Sagi.Sdk.DotEnv — Novo SDK


Adicionado o SDK `Sagi.Sdk.DotEnv` ao monorepo. Integra arquivos `.env` ao pipeline de configuração nativo do .NET sem dependências externas além de `Microsoft.Extensions.Configuration`. Targeting `net10.0`. Inclui testes unitários (xUnit) e property-based tests (FsCheck.Xunit) cobrindo 4 propriedades de corretude do parser.

---

### [2026-04] CNPJ Alfanumérico — IN RFB nº 2.229/2024

#### Contexto

A Receita Federal publicou a Instrução Normativa RFB nº 2.229, de 15 de outubro de 2024, instituindo o CNPJ alfanumérico. A partir de julho de 2026, novos CNPJs poderão conter letras (`A–Z`) nas 12 primeiras posições. Os CNPJs numéricos existentes continuam válidos sem alteração.

Fonte oficial consultada: [gov.br — Receita Federal, 16/10/2024](https://www.gov.br/receitafederal/pt-br/assuntos/noticias/2024/outubro/cnpj-tera-letras-e-numeros-a-partir-de-julho-de-2026)

#### Algoritmo (conforme gov.br)

O novo algoritmo unifica numérico e alfanumérico sob a mesma fórmula módulo 11:

- Posições 1–12: alfanumérico `[A-Z0-9]`
- Posições 13–14: dígitos verificadores numéricos
- Valor de cada caractere = `ASCII - 48` (ex: `'0'`=0, `'A'`=17, `'Z'`=42)
- Pesos de 2 a 9, aplicados da direita para esquerda, reiniciando após 9 → 2
- Regra do DV: resto 0 ou 1 → DV = 0, senão DV = 11 - resto
- Segundo DV calculado sobre os 12 chars base + primeiro DV (13 chars)

O algoritmo numérico legado usa pesos fixos diferentes (`[5,4,3,2,9,8,7,6,5,4,3,2]` e `[6,5,4,3,2,9,8,7,6,5,4,3,2]`) e foi mantido isolado para compatibilidade retroativa.

#### Arquivos alterados

**`Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/ValueObjects/Cnpj.cs`**

Arquivo reescrito. Mudanças em relação à versão anterior:

1. Três `Regex` estáticos compilados substituem a regex inline de validação:
   - `s_numericRaw` — detecta 14 dígitos puros
   - `s_alphanumericRaw` — detecta 14 chars `[A-Z0-9]`
   - `s_formattedMask` — detecta entrada formatada `XX.XXX.XXX/XXXX-DD`

2. Nova propriedade `IsAlphanumeric` — `true` quando `Number` não é puramente numérico.

3. `Formatted` atualizado — numérico usa `long.ToString(mask)` (retrocompatível); alfanumérico usa interpolação de string com slices.

4. `Validate()` — despacha para `NumericValidator` ou `AlphanumericValidator` com base em `IsAlphanumeric`. Mensagem de erro de tamanho alterada de "14 digits" para "14 characters".

5. `Normalize()` reescrito — aceita entrada formatada (remove `.`, `/`, `-`), raw numérico, raw alfanumérico, ou tenta extrair apenas dígitos como fallback legado.

6. `Equals` e `GetHashCode` — passaram a usar `OrdinalIgnoreCase` para suportar comparação case-insensitive entre entradas normalizadas.

7. Dois validadores internos como `private static class` (sem acesso externo):

   **`NumericValidator`** — lógica original preservada integralmente:
   - Pesos fixos `s_weights1` e `s_weights2`
   - `IsAllSameDigit` rejeita sequências repetidas
   - `CalculateDigit` com índice direto nos pesos

   **`AlphanumericValidator`** — novo algoritmo IN RFB 2.229/2024:
   - `HasValidFormat` valida chars `[A-Z0-9]` nas posições 0–11 e dígitos nas posições 12–13
   - `IsAllSameChar` rejeita sequências repetidas
   - `CalculateCheckDigit` itera da direita para esquerda com peso ciclando 2→9

**`Sagi.Sdk.Domain/tests/Sagi.Sdk.Domain.Tests/ValueObjects/CnpjTests.cs`**

Arquivo reescrito. Testes organizados em três seções por comentário inline:

- `// ── Numeric (legacy)` — mantém todos os testes anteriores + 5 válidos e 5 inválidos dos exemplos fornecidos
- `// ── Alphanumeric (IN RFB nº 2.229/2024)` — 5 válidos, 5 inválidos (DV errado), 2 inválidos (chars inválidos), `Formatted`, normalização
- `// ── Shared behaviour` — `TryParse`, implicit operator, `Equals`, `GetHashCode`, `Formatted`, `ToString` para ambos os formatos

Total: 21 testes, todos passando.

#### Decisões de design

- Os validadores foram implementados como `private static class` dentro de `Cnpj` para manter o isolamento das regras sem criar arquivos adicionais, conforme solicitado.
- O algoritmo numérico legado foi preservado intacto em `NumericValidator` para garantir que CNPJs existentes continuem sendo validados pela mesma lógica de sempre.
- O novo `AlphanumericValidator` só é acionado quando `IsAlphanumeric` é `true`, evitando qualquer impacto em código que já usa CNPJs numéricos.
- `Normalize` faz uppercase antes de qualquer comparação, tornando a entrada case-insensitive na entrada mas armazenando sempre em maiúsculas.
- Não foram adicionados exemplos estáticos de CNPJs no código — os casos de teste usam os exemplos fornecidos apenas como `InlineData` nos testes.

#### Padrão a seguir em futuras extensões de Value Objects

Quando um value object precisar suportar múltiplos formatos/algoritmos de validação:

1. Detectar o formato no construtor via `Normalize` e armazenar normalizado.
2. Expor uma propriedade booleana que identifica o formato (`IsAlphanumeric`, `IsInternational`, etc.).
3. Criar um `private static class` por algoritmo dentro do value object.
4. Em `Validate()`, despachar para o validador correto com base na propriedade de detecção.
5. Nos testes, separar os casos por seção com comentário inline e cobrir: válidos, inválidos por DV errado, inválidos por formato, e comportamentos compartilhados (`TryParse`, `Equals`, `Formatted`).
