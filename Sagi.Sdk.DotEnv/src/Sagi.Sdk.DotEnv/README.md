# Sagi.Sdk.DotEnv

SDK para integração de arquivos `.env` ao sistema de configuração nativo do .NET (`IConfiguration` / `IConfigurationBuilder`).

## Descrição

O `Sagi.Sdk.DotEnv` implementa um `IConfigurationProvider` e um `IConfigurationSource` que carregam variáveis de ambiente de arquivos `.env` diretamente no pipeline de configuração do .NET. As variáveis carregadas ficam acessíveis via `IConfiguration` da mesma forma que `appsettings.json` ou variáveis de ambiente do sistema.

**Características:**
- Sem dependências externas além de `Microsoft.Extensions.Configuration`
- Suporte a valores com múltiplos `=` (preservados integralmente)
- Comentários com `#` são ignorados
- Arquivo ausente não gera exceção (comportamento opcional por padrão)
- Comparação de chaves case-insensitive

## Instalação

```sh
dotnet add package Sagi.Sdk.DotEnv
```

## Uso

### Exemplo 1: Carregamento padrão

Carrega o arquivo `.env` do diretório de trabalho atual:

```csharp
using Microsoft.Extensions.Configuration;
using Sagi.Sdk.DotEnv.Extensions;

IConfiguration config = new ConfigurationBuilder()
    .AddDotEnv()
    .Build();

string dbUrl = config["DATABASE_URL"];
```

### Exemplo 2: Diretório e nome de arquivo personalizados

```csharp
IConfiguration config = new ConfigurationBuilder()
    .AddDotEnv(opt =>
    {
        opt.Directory = "/app/config";
        opt.FileName = ".env.production";
    })
    .Build();

string appName = config["APP_NAME"];
```

## Formato do arquivo `.env`

```
# Comentário — linha ignorada
DATABASE_URL=postgres://localhost:5432/mydb
APP_SECRET=abc=def=ghi        # valor com = é preservado
  PADDED_KEY  =  padded value  # trim aplicado em chave e valor
LINHA_SEM_IGUAL                # ignorada
                               # linha vazia — ignorada
```

**Regras de parsing:**
- Linhas vazias ou só com espaços em branco são ignoradas
- Linhas que começam com `#` (após trim) são comentários e são ignoradas
- Linhas sem `=` são ignoradas
- Chaves e valores têm trim aplicado
- Valores com múltiplos `=` são preservados integralmente (split apenas no primeiro `=`)
- Chaves vazias após trim são ignoradas

## Ordem de precedência

O `DotEnvProvider` segue a ordem de precedência padrão do .NET: a última fonte adicionada ao `IConfigurationBuilder` tem prioridade. Para que variáveis de ambiente do sistema sobrescrevam o `.env`, adicione `AddEnvironmentVariables()` após `AddDotEnv()`:

```csharp
IConfiguration config = new ConfigurationBuilder()
    .AddDotEnv()
    .AddEnvironmentVariables()
    .Build();
```

## Licença

Este projeto está licenciado sob os termos da licença MIT.
