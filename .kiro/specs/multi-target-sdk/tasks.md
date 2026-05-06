# Implementation Plan: multi-target-sdk

## Overview

Substituir `<TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>` nos 10 arquivos `.csproj` do monorepo (5 de produção + 5 de teste). Nenhuma dependência NuGet precisa ser alterada.

## Tasks

- [x] 1. Atualizar projetos de produção sem dependências externas
  - [x] 1.1 Atualizar `Sagi.Sdk.Results.csproj`
    - Substituir `<TargetFramework>net10.0</TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
    - _Requirements: 1.3, 1.4_

  - [x] 1.2 Atualizar `Sagi.Sdk.Domain.csproj`
    - Substituir `<TargetFramework>net10.0</TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
    - _Requirements: 1.3, 1.4, 3.1_

  - [x] 1.3 Atualizar `Sagi.Sdk.DotEnv.csproj`
    - Substituir `<TargetFramework>net10.0</TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
    - _Requirements: 1.3, 1.4, 2.1_

- [x] 2. Atualizar projetos de produção com dependências externas
  - [x] 2.1 Atualizar `Sagi.Sdk.MongoDb.csproj`
    - Substituir `<TargetFramework>net10.0</TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
    - Nenhuma versão de pacote precisa ser alterada (MongoDB.Driver 3.3.0 e Microsoft.Extensions.* 9.0.5 já suportam net8.0)
    - _Requirements: 1.3, 1.4, 2.1, 2.3_

  - [x] 2.2 Atualizar `Sagi.Sdk.AWS.DynamoDb.csproj`
    - Substituir `<TargetFramework>net10.0</TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
    - Nenhuma versão de pacote precisa ser alterada (AWSSDK.* 3.7.x e Microsoft.Extensions.Hosting 9.0.5 já suportam net8.0)
    - _Requirements: 1.3, 1.4, 2.2, 3.2_

- [x] 3. Checkpoint — verificar projetos de produção
  - Garantir que todos os 5 `.csproj` de produção contenham `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>` e nenhum contenha `<TargetFramework>` (singular). Perguntar ao usuário se houver dúvidas.

- [x] 4. Atualizar projetos de teste
  - [x] 4.1 Atualizar `Sagi.Sdk.Results.Tests.csproj`
    - Substituir `<TargetFramework>net10.0</TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
    - _Requirements: 6.1, 6.2_

  - [x] 4.2 Atualizar `Sagi.Sdk.Domain.Tests.csproj`
    - Substituir `<TargetFramework>net10.0</TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
    - _Requirements: 6.1, 6.2_

  - [x] 4.3 Atualizar `Sagi.Sdk.MongoDb.Tests.csproj`
    - Substituir `<TargetFramework>net10.0</TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
    - _Requirements: 6.1, 6.2_

  - [x] 4.4 Atualizar `Sagi.Sdk.AWS.DynamoDb.Tests.csproj`
    - Substituir `<TargetFramework>net10.0</TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
    - _Requirements: 6.1, 6.2_

  - [x] 4.5 Atualizar `Sagi.Sdk.DotEnv.Tests.csproj`
    - Substituir `<TargetFramework>net10.0</TargetFramework>` por `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
    - _Requirements: 6.1, 6.2_

- [x] 5. Checkpoint final — validar build e testes
  - Garantir que todos os 10 `.csproj` estejam atualizados. Executar `dotnet build Sagi.Sdk.sln` e `dotnet test Sagi.Sdk.sln` para confirmar que todos os projetos compilam e todos os testes passam nos três frameworks. Perguntar ao usuário se houver dúvidas.

## Notes

- Nenhuma dependência NuGet precisa ser alterada — todas as versões atuais já são compatíveis com net8.0 (ver matriz de compatibilidade no design)
- O MSBuild resolve automaticamente as `<ProjectReference>` para o TFM correspondente (Domain → Results, DynamoDb → Results)
- O `global.json` com `rollForward: latestMajor` permite que o SDK .NET 10 compile para net8.0 e net9.0 sem instalar SDKs adicionais
