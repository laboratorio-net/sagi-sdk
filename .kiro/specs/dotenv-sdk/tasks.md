# Plano de Implementação: Sagi.Sdk.DotEnv

## Visão Geral

Implementação incremental do SDK `Sagi.Sdk.DotEnv` seguindo o padrão do monorepo. Cada task constrói sobre a anterior, terminando com a integração completa na solução.

## Tasks

- [x] 1. Scaffold da estrutura de projetos
  - Criar `Sagi.Sdk.DotEnv/src/Sagi.Sdk.DotEnv/Sagi.Sdk.DotEnv.csproj` targeting `net8.0`, com referência a `Microsoft.Extensions.Configuration` e sem outras dependências externas
  - Criar `Sagi.Sdk.DotEnv/tests/Sagi.Sdk.DotEnv.Tests/Sagi.Sdk.DotEnv.Tests.csproj` targeting `net8.0`, com referências a xUnit, NSubstitute, AutoFixture, Bogus e FsCheck.Xunit
  - Criar `Sagi.Sdk.DotEnv/samples/Sagi.Sdk.DotEnv.Sample/Sagi.Sdk.DotEnv.Sample.csproj` targeting `net8.0` como `Exe`
  - Criar `Sagi.Sdk.DotEnv/Sagi.Sdk.DotEnv.sln` e adicionar os três projetos
  - Adicionar os três projetos à solução raiz `Sagi.Sdk.sln`
  - Criar as pastas `Extensions/`, `Options/`, `Provider/`, `Parser/` dentro do projeto principal
  - _Requirements: 1.1, 5.5, 6.1_

- [x] 2. Implementar `DotEnvOptions`
  - [x] 2.1 Criar `Options/DotEnvOptions.cs` com propriedades `Directory` (default `Directory.GetCurrentDirectory()`) e `FileName` (default `".env"`)
    - Sem validação na própria classe — validação é responsabilidade do `DotEnvSource`
    - _Requirements: 2.1, 2.2, 2.3_

  - [x] 2.2 Escrever testes unitários para `DotEnvOptions`
    - Verificar que os valores padrão de `Directory` e `FileName` estão corretos
    - _Requirements: 2.1, 5.4_

- [x] 3. Implementar `DotEnvParser`
  - [x] 3.1 Criar `Parser/DotEnvParser.cs` como classe estática com método `Parse(IEnumerable<string> lines)`
    - Ignorar linhas nulas, vazias ou só whitespace
    - Ignorar linhas que começam com `#` (após trim)
    - Ignorar linhas sem `=`
    - Ignorar linhas com chave vazia após trim
    - Split no primeiro `=` via `line.Split('=', 2)` para preservar `=` no valor
    - Aplicar trim em chave e valor
    - Retornar `IReadOnlyDictionary<string, string>`
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7_

  - [x] 3.2 Escrever testes unitários para `DotEnvParser`
    - Linha válida `KEY=VALUE` → par correto
    - Linha vazia → ignorada
    - Linha só whitespace → ignorada
    - Linha com `#` → ignorada
    - Linha sem `=` → ignorada
    - Valor com múltiplos `=` → valor preservado integralmente
    - Trim em chave e valor
    - Chave vazia após trim → ignorada
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 5.1_

  - [x] 3.3 Escrever property test — Property 1: Round-trip de parsing
    - `// Feature: dotenv-sdk, Property 1: Round-trip de parsing`
    - Gerador: conjuntos aleatórios de pares `(string key, string value)` onde a chave não contém `=` e não é whitespace-only
    - Serializar como `KEY=VALUE`, parsear, verificar que todos os pares estão presentes com valores idênticos após trim
    - `[Property(MaxTest = 100)]`
    - **Property 1: Round-trip de parsing**
    - **Validates: Requirements 1.3, 3.1, 3.7, 4.3, 5.6**

  - [x] 3.4 Escrever property test — Property 2: Valores com `=` preservados
    - `// Feature: dotenv-sdk, Property 2: Valores com = são preservados integralmente`
    - Gerador: chaves válidas aleatórias + valores aleatórios com pelo menos um `=` inserido
    - Verificar que o valor completo (tudo após o primeiro `=` na linha) é preservado
    - `[Property(MaxTest = 100)]`
    - **Property 2: Valores com `=` são preservados integralmente**
    - **Validates: Requirements 3.5**

  - [x] 3.5 Escrever property test — Property 3: Linhas inválidas não afetam o resultado
    - `// Feature: dotenv-sdk, Property 3: Linhas inválidas não afetam o resultado`
    - Gerador: lista de linhas válidas intercaladas com linhas inválidas (whitespace, `#`-prefixadas, sem `=`)
    - Verificar que o resultado contém exatamente as entradas das linhas válidas
    - `[Property(MaxTest = 100)]`
    - **Property 3: Linhas inválidas não afetam o resultado**
    - **Validates: Requirements 3.2, 3.3, 3.4**

  - [x] 3.6 Escrever property test — Property 4: Idempotência do parsing
    - `// Feature: dotenv-sdk, Property 4: Idempotência do parsing`
    - Gerador: listas aleatórias de linhas (mix de válidas e inválidas)
    - Verificar que `Parse(lines)` chamado duas vezes retorna dicionários com os mesmos pares
    - `[Property(MaxTest = 100)]`
    - **Property 4: Idempotência do parsing**
    - **Validates: Requirements 3.1, 3.7**

- [x] 4. Checkpoint — Verificar testes do parser
  - Garantir que todos os testes passam antes de prosseguir. Perguntar ao usuário se houver dúvidas.

- [x] 5. Implementar `DotEnvSource` e `DotEnvProvider`
  - [x] 5.1 Criar `Provider/DotEnvSource.cs` implementando `IConfigurationSource`
    - Construtor recebe `DotEnvOptions` e valida `Directory` e `FileName` com `ArgumentException.ThrowIfNullOrEmpty`
    - `Build()` instancia e retorna `DotEnvProvider`
    - _Requirements: 2.5, 2.6, 4.2_

  - [x] 5.2 Escrever testes unitários para `DotEnvSource`
    - `Directory` nulo → `ArgumentException`
    - `Directory` vazio → `ArgumentException`
    - `FileName` nulo → `ArgumentException`
    - `FileName` vazio → `ArgumentException`
    - Opções válidas → `Build()` retorna instância de `DotEnvProvider`
    - _Requirements: 2.5, 2.6, 5.4_

  - [x] 5.3 Criar `Provider/DotEnvProvider.cs` herdando `ConfigurationProvider`
    - Construtor recebe `DotEnvOptions`
    - `Load()` combina `Directory` e `FileName` com `Path.Combine`
    - Se o arquivo não existir, retornar silenciosamente (sem exceção)
    - Delegar parsing ao `DotEnvParser.Parse`
    - Popular `Data` com `StringComparer.OrdinalIgnoreCase`
    - _Requirements: 1.2, 1.3, 1.4, 2.4, 4.1, 4.3_

  - [x] 5.4 Escrever testes unitários para `DotEnvProvider`
    - Arquivo existente com entradas válidas → `Data` populado corretamente
    - Arquivo inexistente → sem exceção, `Data` vazio
    - Arquivo vazio → sem exceção, `Data` vazio
    - _Requirements: 1.3, 1.4, 5.2_

- [x] 6. Implementar `ConfigurationBuilderExtensions`
  - [x] 6.1 Criar `Extensions/ConfigurationBuilderExtensions.cs` com dois overloads de `AddDotEnv`
    - `AddDotEnv()` sem argumentos delega para `AddDotEnv(_ => { })`
    - `AddDotEnv(Action<DotEnvOptions>)` cria `DotEnvOptions`, aplica o delegate e adiciona `DotEnvSource` ao builder
    - _Requirements: 1.1, 2.1_

  - [x] 6.2 Escrever testes unitários para `ConfigurationBuilderExtensions`
    - `AddDotEnv()` → `Sources` contém `DotEnvSource` com valores padrão
    - `AddDotEnv(opt => ...)` → `Sources` contém `DotEnvSource` com opções customizadas
    - Diretório e nome customizados → caminho combinado corretamente no provider
    - _Requirements: 1.1, 1.2, 2.1, 2.2, 2.3, 2.4, 5.3_

- [x] 7. Checkpoint — Garantir que todos os testes passam
  - Executar `dotnet test` no projeto de testes. Perguntar ao usuário se houver dúvidas.

- [x] 8. Implementar o Sample de demonstração
  - [x] 8.1 Criar `samples/Sagi.Sdk.DotEnv.Sample/.env` com entradas de exemplo (DATABASE_URL, APP_NAME, DEBUG, chave com `=` no valor)
    - _Requirements: 6.2_

  - [x] 8.2 Implementar `samples/Sagi.Sdk.DotEnv.Sample/Program.cs`
    - Demonstrar `AddDotEnv()` sem argumentos e imprimir as chaves lidas
    - Demonstrar `AddDotEnv(opt => { opt.FileName = ".env"; })` com opções explícitas
    - _Requirements: 6.2, 6.3_

- [x] 9. Escrever o README do SDK
  - Criar `src/Sagi.Sdk.DotEnv/README.md` em português
  - Incluir: descrição, instalação via `dotnet add package`, exemplos de `AddDotEnv()` e `AddDotEnv(Action<DotEnvOptions>)`, e descrição do formato `.env` suportado
  - _Requirements: 7.1, 7.2, 7.3_

- [x] 10. Checkpoint final — Verificar integração completa
  - Garantir que `dotnet build` e `dotnet test` passam em toda a solução. Perguntar ao usuário se houver dúvidas.

## Notas

- Tasks marcadas com `*` são opcionais e podem ser puladas para um MVP mais rápido
- Cada task referencia os requisitos correspondentes para rastreabilidade
- Os testes de propriedade usam `FsCheck.Xunit` com `[Property(MaxTest = 100)]`
- Convenções do monorepo: 4 espaços, CRLF, sem `var`, `_camelCase` para campos privados
