# Requirements Document

## Introduction

O `Sagi.Sdk.DotEnv` é um novo SDK do monorepo Sagi que integra arquivos `.env` ao sistema de configuração do .NET (`IConfiguration` / `IConfigurationBuilder`). Ele implementa um `IConfigurationProvider` e um `IConfigurationSource` nativos, permitindo que variáveis definidas em arquivos `.env` sejam consumidas da mesma forma que `appsettings.json` ou variáveis de ambiente — sem dependências externas além do `Microsoft.Extensions.Configuration`.

O SDK segue os mesmos padrões estruturais, de código e de testes dos demais SDKs do monorepo (`Sagi.Sdk.Results`, `Sagi.Sdk.Domain`, `Sagi.Sdk.MongoDb`, `Sagi.Sdk.AWS.DynamoDb`).

---

## Glossary

- **DotEnvProvider**: O `IConfigurationProvider` responsável por ler e expor as entradas do arquivo `.env` ao `IConfiguration`.
- **DotEnvSource**: O `IConfigurationSource` que instancia o `DotEnvProvider` e carrega as opções de configuração.
- **DotEnvOptions**: Classe de opções que encapsula o diretório de busca e o nome do arquivo `.env`.
- **ConfigurationBuilder**: O `IConfigurationBuilder` do .NET ao qual o `DotEnvSource` é adicionado via método de extensão.
- **Entry**: Uma linha do arquivo `.env` no formato `CHAVE=VALOR`.
- **Parser**: Componente interno responsável por transformar as linhas do arquivo `.env` em pares chave-valor.
- **Sample**: Aplicação Console de demonstração que ilustra o uso do SDK.

---

## Requirements

### Requirement 1: Carregamento padrão do arquivo `.env`

**User Story:** Como desenvolvedor, quero adicionar o arquivo `.env` ao `IConfiguration` com uma única chamada de extensão, para que as variáveis de ambiente do projeto sejam carregadas automaticamente sem configuração adicional.

#### Acceptance Criteria

1. THE `ConfigurationBuilder` SHALL expor um método de extensão `AddDotEnv()` que adiciona um `DotEnvSource` ao pipeline de configuração.
2. WHEN `AddDotEnv()` é chamado sem argumentos, THE `DotEnvProvider` SHALL buscar o arquivo `.env` no diretório de trabalho atual (`Directory.GetCurrentDirectory()`).
3. WHEN o arquivo `.env` é encontrado no diretório padrão, THE `DotEnvProvider` SHALL carregar todas as entradas válidas como pares chave-valor no `IConfiguration`.
4. IF o arquivo `.env` não existir no diretório informado, THEN THE `DotEnvProvider` SHALL concluir o carregamento sem lançar exceção, resultando em zero entradas adicionadas.

---

### Requirement 2: Configuração de diretório e nome de arquivo personalizados

**User Story:** Como desenvolvedor, quero informar um diretório e/ou nome de arquivo diferentes do padrão, para que o SDK localize o arquivo `.env` correto em projetos com estrutura de diretórios não convencional.

#### Acceptance Criteria

1. THE `ConfigurationBuilder` SHALL expor um método de extensão `AddDotEnv(Action<DotEnvOptions>)` que aceita um delegate para configurar as opções.
2. WHEN `AddDotEnv(Action<DotEnvOptions>)` é chamado com um diretório específico, THE `DotEnvProvider` SHALL buscar o arquivo `.env` no diretório informado em vez do diretório de trabalho atual.
3. WHEN `AddDotEnv(Action<DotEnvOptions>)` é chamado com um nome de arquivo específico, THE `DotEnvProvider` SHALL buscar o arquivo com o nome informado em vez de `.env`.
4. WHEN `AddDotEnv(Action<DotEnvOptions>)` é chamado com diretório e nome de arquivo específicos, THE `DotEnvProvider` SHALL combinar ambos para construir o caminho completo do arquivo.
5. IF o diretório informado em `DotEnvOptions` for nulo ou vazio, THEN THE `DotEnvSource` SHALL lançar `ArgumentException` ao ser construído.
6. IF o nome de arquivo informado em `DotEnvOptions` for nulo ou vazio, THEN THE `DotEnvSource` SHALL lançar `ArgumentException` ao ser construído.

---

### Requirement 3: Parsing do arquivo `.env`

**User Story:** Como desenvolvedor, quero que o SDK interprete corretamente o formato padrão de arquivos `.env`, para que apenas entradas válidas sejam carregadas e entradas malformadas sejam ignoradas sem interromper a aplicação.

#### Acceptance Criteria

1. THE `Parser` SHALL interpretar cada linha no formato `CHAVE=VALOR` como uma entrada válida, onde `CHAVE` é a parte antes do primeiro `=` e `VALOR` é a parte restante.
2. WHEN uma linha está vazia ou contém apenas espaços em branco, THE `Parser` SHALL ignorar essa linha.
3. WHEN uma linha começa com `#`, THE `Parser` SHALL ignorar essa linha (comentário).
4. WHEN uma linha não contém o caractere `=`, THE `Parser` SHALL ignorar essa linha.
5. WHEN o `VALOR` contém o caractere `=`, THE `Parser` SHALL preservar o `=` como parte do valor (split apenas no primeiro `=`).
6. WHEN a `CHAVE` ou o `VALOR` contém espaços em branco nas extremidades, THE `Parser` SHALL remover esses espaços (trim).
7. FOR ALL arquivos `.env` válidos, o resultado do parsing SHALL conter exatamente as entradas com chave não vazia presentes no arquivo.

---

### Requirement 4: Integração com `IConfiguration`

**User Story:** Como desenvolvedor, quero que as variáveis carregadas do `.env` sejam acessíveis via `IConfiguration` da mesma forma que qualquer outra fonte de configuração, para que o código da aplicação não precise saber a origem dos valores.

#### Acceptance Criteria

1. THE `DotEnvProvider` SHALL implementar `IConfigurationProvider` do namespace `Microsoft.Extensions.Configuration`.
2. THE `DotEnvSource` SHALL implementar `IConfigurationSource` do namespace `Microsoft.Extensions.Configuration`.
3. WHEN o `DotEnvProvider` é carregado, THE `IConfiguration` SHALL retornar o valor correto para cada chave presente no arquivo `.env` via `configuration["CHAVE"]`.
4. WHEN a mesma chave existe em múltiplas fontes de configuração, THE `IConfiguration` SHALL respeitar a ordem de precedência padrão do .NET (última fonte adicionada tem prioridade).

---

### Requirement 5: Cobertura de testes

**User Story:** Como mantenedor do SDK, quero cobertura de testes automatizados seguindo os padrões do monorepo, para que regressões sejam detectadas e o comportamento do SDK seja documentado por testes.

#### Acceptance Criteria

1. THE `Sagi.Sdk.DotEnv.Tests` SHALL cobrir o comportamento do `Parser` para: entradas válidas, linhas vazias, comentários, linhas sem `=`, valores com `=`, e trim de espaços.
2. THE `Sagi.Sdk.DotEnv.Tests` SHALL cobrir o comportamento do `DotEnvProvider` para: arquivo existente com entradas, arquivo inexistente, e arquivo vazio.
3. THE `Sagi.Sdk.DotEnv.Tests` SHALL cobrir o método de extensão `AddDotEnv()` verificando que o provider é adicionado ao `IConfigurationBuilder`.
4. THE `Sagi.Sdk.DotEnv.Tests` SHALL cobrir a validação de `DotEnvOptions` para diretório e nome de arquivo nulos ou vazios.
5. THE `Sagi.Sdk.DotEnv.Tests` SHALL usar xUnit como framework de testes, NSubstitute para mocks, AutoFixture para geração de dados e Bogus para dados fake, seguindo o padrão dos demais projetos do monorepo.
6. FOR ALL métodos públicos do `Parser`, os testes SHALL verificar que parsing seguido de serialização das chaves e valores produz os mesmos dados originais (round-trip de chave e valor).

---

### Requirement 6: Sample de demonstração

**User Story:** Como desenvolvedor avaliando o SDK, quero uma aplicação Console de exemplo funcional, para que eu possa entender rapidamente como integrar o `Sagi.Sdk.DotEnv` em meu projeto.

#### Acceptance Criteria

1. THE `Sample` SHALL ser uma aplicação Console .NET 8 localizada em `Sagi.Sdk.DotEnv/samples/Sagi.Sdk.DotEnv.Sample/`.
2. WHEN o `Sample` é executado, THE `Sample` SHALL carregar um arquivo `.env` de exemplo via `AddDotEnv()` e imprimir os valores lidos no console.
3. THE `Sample` SHALL demonstrar tanto o uso sem argumentos (`AddDotEnv()`) quanto o uso com opções (`AddDotEnv(opt => { ... })`).

---

### Requirement 7: README do SDK

**User Story:** Como desenvolvedor, quero um README claro e objetivo no pacote, para que eu saiba como instalar e usar o SDK sem precisar ler o código-fonte.

#### Acceptance Criteria

1. THE `README.md` SHALL estar localizado em `Sagi.Sdk.DotEnv/src/Sagi.Sdk.DotEnv/README.md` e incluído no pacote NuGet.
2. THE `README.md` SHALL conter: descrição do SDK, instrução de instalação via `dotnet add package`, exemplos de uso com `AddDotEnv()` e `AddDotEnv(Action<DotEnvOptions>)`, e descrição do formato suportado do arquivo `.env`.
3. THE `README.md` SHALL estar escrito em português, seguindo o padrão dos demais READMEs do monorepo.
