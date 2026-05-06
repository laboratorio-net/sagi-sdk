# Requirements Document

## Introduction

Este documento especifica os requisitos para tornar todos os 5 SDKs do monorepo Sagi retrocompatíveis com .NET 8 e .NET 9, além do .NET 10 já existente. A mudança envolve configurar multi-targeting nos arquivos `.csproj` dos projetos: `Sagi.Sdk.Results`, `Sagi.Sdk.Domain`, `Sagi.Sdk.MongoDb`, `Sagi.Sdk.AWS.DynamoDb` e `Sagi.Sdk.DotEnv`.

O objetivo é permitir que consumidores desses SDKs utilizem as bibliotecas em projetos que ainda não migraram para .NET 10, mantendo compatibilidade com versões anteriores do runtime sem duplicar código ou criar branches separados.

## Glossary

- **SDK_Project**: Qualquer um dos 5 projetos do monorepo (Results, Domain, MongoDb, AWS.DynamoDb, DotEnv)
- **Multi_Targeting**: Configuração do MSBuild que compila um projeto para múltiplos frameworks simultaneamente
- **Target_Framework**: Versão específica do .NET para a qual o código é compilado (net8.0, net9.0, net10.0)
- **Build_System**: MSBuild e dotnet CLI responsáveis pela compilação dos projetos
- **Package_Consumer**: Aplicação ou biblioteca que referencia os SDKs via NuGet ou ProjectReference
- **Dependency_Version**: Versão de um pacote NuGet referenciado pelo SDK
- **Compatibility_Matrix**: Conjunto de combinações válidas entre Target_Framework e versões de dependências

## Requirements

### Requirement 1: Configurar Multi-Targeting nos Projetos

**User Story:** Como desenvolvedor de SDK, quero configurar multi-targeting nos 5 projetos, para que cada SDK compile para .NET 8, .NET 9 e .NET 10 simultaneamente.

#### Acceptance Criteria

1. THE Build_System SHALL compilar cada SDK_Project para net8.0, net9.0 e net10.0 em uma única execução de build
2. WHEN `dotnet build` é executado na raiz do monorepo, THE Build_System SHALL gerar assemblies separados para cada Target_Framework
3. THE SDK_Project SHALL usar `<TargetFrameworks>` (plural) no lugar de `<TargetFramework>` (singular) no arquivo .csproj
4. THE SDK_Project SHALL especificar os frameworks na ordem `net8.0;net9.0;net10.0` separados por ponto e vírgula
5. WHEN o build é concluído, THE Build_System SHALL produzir 3 assemblies por projeto em `bin/Debug/` ou `bin/Release/`

### Requirement 2: Ajustar Dependências de Pacotes NuGet

**User Story:** Como desenvolvedor de SDK, quero ajustar as versões de dependências NuGet para garantir compatibilidade com .NET 8 e .NET 9, para que os SDKs funcionem corretamente em todos os runtimes suportados.

#### Acceptance Criteria

1. WHERE um SDK_Project referencia pacotes Microsoft.Extensions.*, THE SDK_Project SHALL usar versões compatíveis com net8.0 como versão mínima
2. WHERE um SDK_Project referencia AWSSDK.*, THE SDK_Project SHALL manter as versões atuais que já suportam net8.0
3. WHERE um SDK_Project referencia MongoDB.Driver, THE SDK_Project SHALL verificar compatibilidade com net8.0 e ajustar se necessário
4. THE SDK_Project SHALL evitar usar APIs ou recursos exclusivos de .NET 10 sem conditional compilation
5. WHEN uma dependência não suporta net8.0, THE Build_System SHALL falhar com mensagem de erro clara indicando o pacote incompatível

### Requirement 3: Manter Compatibilidade de Referências entre Projetos

**User Story:** Como desenvolvedor de SDK, quero que as referências entre projetos (ProjectReference) funcionem corretamente com multi-targeting, para que Sagi.Sdk.Domain e Sagi.Sdk.AWS.DynamoDb continuem referenciando Sagi.Sdk.Results sem conflitos.

#### Acceptance Criteria

1. WHEN Sagi.Sdk.Domain referencia Sagi.Sdk.Results, THE Build_System SHALL resolver a referência para o Target_Framework correspondente
2. WHEN Sagi.Sdk.AWS.DynamoDb referencia Sagi.Sdk.Results, THE Build_System SHALL resolver a referência para o Target_Framework correspondente
3. THE Build_System SHALL compilar os projetos na ordem correta respeitando o grafo de dependências
4. WHEN um SDK_Project é compilado para net8.0, THE Build_System SHALL usar o assembly net8.0 das suas ProjectReferences
5. THE Build_System SHALL falhar se houver incompatibilidade de Target_Framework entre projetos dependentes

### Requirement 4: Preservar Funcionalidade Existente

**User Story:** Como desenvolvedor de SDK, quero garantir que toda a funcionalidade existente continue operando corretamente após a mudança para multi-targeting, para que não haja regressões de comportamento.

#### Acceptance Criteria

1. WHEN os testes são executados com `dotnet test`, THE Build_System SHALL executar todos os testes para cada Target_Framework
2. THE SDK_Project SHALL manter 100% dos testes existentes passando para net8.0, net9.0 e net10.0
3. THE SDK_Project SHALL preservar todas as APIs públicas sem breaking changes
4. THE SDK_Project SHALL manter o comportamento de runtime idêntico entre os três Target_Frameworks
5. WHEN um teste falha em um Target_Framework específico, THE Build_System SHALL reportar claramente qual framework apresentou a falha

### Requirement 5: Validar Geração de Pacotes NuGet

**User Story:** Como desenvolvedor de SDK, quero que os pacotes NuGet gerados contenham os assemblies para os três frameworks, para que consumidores possam usar o SDK independente da versão do .NET que estejam usando.

#### Acceptance Criteria

1. WHEN `dotnet pack` é executado, THE Build_System SHALL incluir assemblies para net8.0, net9.0 e net10.0 no pacote .nupkg
2. THE Build_System SHALL criar a estrutura de pastas `lib/net8.0/`, `lib/net9.0/` e `lib/net10.0/` dentro do .nupkg
3. WHEN um Package_Consumer referencia o SDK em um projeto net8.0, THE Build_System SHALL resolver automaticamente o assembly net8.0
4. WHEN um Package_Consumer referencia o SDK em um projeto net9.0, THE Build_System SHALL resolver automaticamente o assembly net9.0
5. WHEN um Package_Consumer referencia o SDK em um projeto net10.0, THE Build_System SHALL resolver automaticamente o assembly net10.0
6. THE Build_System SHALL incluir os metadados corretos no .nuspec indicando os frameworks suportados

### Requirement 6: Atualizar Projetos de Teste

**User Story:** Como desenvolvedor de SDK, quero que os projetos de teste também suportem multi-targeting, para que possam validar o comportamento dos SDKs em todos os runtimes suportados.

#### Acceptance Criteria

1. THE Build_System SHALL configurar os projetos em `tests/` com `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
2. WHEN `dotnet test` é executado, THE Build_System SHALL executar cada test suite 3 vezes (uma por Target_Framework)
3. THE Build_System SHALL reportar resultados de teste separados por Target_Framework
4. WHEN um teste usa APIs específicas de framework, THE Build_System SHALL compilar condicionalmente usando diretivas `#if`
5. THE Build_System SHALL falhar se qualquer teste falhar em qualquer dos três Target_Frameworks

### Requirement 7: Documentar Mudanças e Compatibilidade

**User Story:** Como consumidor do SDK, quero documentação clara sobre quais versões do .NET são suportadas, para que eu possa tomar decisões informadas sobre atualização e compatibilidade.

#### Acceptance Criteria

1. THE SDK_Project SHALL incluir informação de frameworks suportados no README.md de cada projeto
2. THE SDK_Project SHALL documentar quaisquer diferenças de comportamento entre Target_Frameworks se existirem
3. THE SDK_Project SHALL especificar versões mínimas de dependências no README.md
4. WHEN há breaking changes relacionados a versões de framework, THE SDK_Project SHALL documentá-los claramente
5. THE SDK_Project SHALL incluir exemplos de uso para cada Target_Framework se houver diferenças

### Requirement 8: Validar Compilação Condicional

**User Story:** Como desenvolvedor de SDK, quero usar compilação condicional quando necessário, para que possa aproveitar recursos específicos de cada versão do .NET sem quebrar compatibilidade.

#### Acceptance Criteria

1. WHERE o código usa APIs disponíveis apenas em .NET 9 ou superior, THE SDK_Project SHALL usar diretivas `#if NET9_0_OR_GREATER`
2. WHERE o código usa APIs disponíveis apenas em .NET 10 ou superior, THE SDK_Project SHALL usar diretivas `#if NET10_0_OR_GREATER`
3. THE SDK_Project SHALL fornecer implementações alternativas ou fallbacks para frameworks mais antigos quando necessário
4. THE Build_System SHALL compilar cada bloco condicional apenas para os Target_Frameworks apropriados
5. WHEN uma API não está disponível em um Target_Framework, THE SDK_Project SHALL usar uma implementação compatível ou lançar NotSupportedException com mensagem clara

### Requirement 9: Manter Configuração do EditorConfig

**User Story:** Como desenvolvedor de SDK, quero que as convenções de código definidas no .editorconfig continuem sendo aplicadas, para que o estilo de código permaneça consistente após as mudanças.

#### Acceptance Criteria

1. THE Build_System SHALL aplicar as regras do .editorconfig durante a compilação para todos os Target_Frameworks
2. THE SDK_Project SHALL manter indentação de 2 espaços em arquivos .csproj conforme definido no .editorconfig
3. THE Build_System SHALL reportar warnings de estilo de código consistentemente entre Target_Frameworks
4. WHEN há violações de estilo, THE Build_System SHALL reportá-las independente do Target_Framework sendo compilado
5. THE SDK_Project SHALL manter todas as convenções de nomenclatura (PascalCase, camelCase, _camelCase) inalteradas

### Requirement 10: Validar Integração Contínua

**User Story:** Como desenvolvedor de SDK, quero que o pipeline de CI compile e teste todos os Target_Frameworks, para que problemas de compatibilidade sejam detectados automaticamente.

#### Acceptance Criteria

1. WHEN o CI executa, THE Build_System SHALL compilar todos os SDK_Projects para net8.0, net9.0 e net10.0
2. WHEN o CI executa testes, THE Build_System SHALL executar todos os testes para cada Target_Framework
3. THE Build_System SHALL falhar o build se qualquer Target_Framework não compilar
4. THE Build_System SHALL falhar o build se qualquer teste falhar em qualquer Target_Framework
5. THE Build_System SHALL reportar métricas de cobertura de código separadas por Target_Framework se aplicável
