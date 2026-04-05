# Implementation Plan: .NET 10 Upgrade

## Overview

Mechanical version-bump across `global.json`, all `.csproj` files, and NuGet package references. No new business logic. Validation is done by running the existing build and test pipeline on .NET 10.

## Tasks

- [x] 1. Update global.json SDK version
  - Change `"version": "8.0.0"` to `"10.0.0"` in `global.json`
  - Retain `"rollForward": "latestMajor"` and `"allowPrerelease": true` unchanged
  - _Requirements: 1.1, 1.2, 1.3_

- [x] 2. Update TargetFramework in all source projects
  - [x] 2.1 Update `Sagi.Sdk.Results/src/Sagi.Sdk.Results/Sagi.Sdk.Results.csproj`
    - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
    - _Requirements: 2.1_
  - [x] 2.2 Update `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Sagi.Sdk.Domain.csproj`
    - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
    - _Requirements: 2.2_
  - [x] 2.3 Update `Sagi.Sdk.MongoDb/src/Sagi.Sdk.MongoDb/Sagi.Sdk.MongoDb.csproj`
    - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
    - _Requirements: 2.3_
  - [x] 2.4 Update `Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/Sagi.Sdk.AWS.DynamoDb.csproj`
    - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
    - _Requirements: 2.4_
  - [x] 2.5 Update `Sagi.Sdk.AWS.DynamoDb/src/Samples/Samples.csproj`
    - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
    - _Requirements: 2.5_

- [x] 3. Update TargetFramework in all test projects
  - [x] 3.1 Update `Sagi.Sdk.Results/tests/Sagi.Sdk.Results.Tests/Sagi.Sdk.Results.Tests.csproj`
    - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
    - _Requirements: 3.1_
  - [x] 3.2 Update `Sagi.Sdk.Domain/tests/Sagi.Sdk.Domain.Tests/Sagi.Sdk.Domain.Tests.csproj`
    - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
    - _Requirements: 3.2_
  - [x] 3.3 Update `Sagi.Sdk.MongoDb/tests/Sagi.Sdk.MongoDb.Tests/Sagi.Sdk.MongoDb.Tests.csproj`
    - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
    - _Requirements: 3.3_
  - [x] 3.4 Update `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/Sagi.Sdk.AWS.DynamoDb.Tests.csproj`
    - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
    - _Requirements: 3.4_

- [x] 4. Remove in-box package references (System.Net.Http and System.Text.RegularExpressions)
  - [x] 4.1 Remove from `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Sagi.Sdk.Domain.csproj`
    - Remove `<PackageReference Include="System.Net.Http" Version="4.3.4" />`
    - Remove `<PackageReference Include="System.Text.RegularExpressions" Version="4.3.1" />`
    - _Requirements: 7.1, 7.2_
  - [x] 4.2 Remove from `Sagi.Sdk.Results/tests/Sagi.Sdk.Results.Tests/Sagi.Sdk.Results.Tests.csproj`
    - Remove `<PackageReference Include="System.Net.Http" Version="4.3.4" />`
    - Remove `<PackageReference Include="System.Text.RegularExpressions" Version="4.3.1" />`
    - _Requirements: 7.1, 7.2_
  - [x] 4.3 Remove from `Sagi.Sdk.MongoDb/tests/Sagi.Sdk.MongoDb.Tests/Sagi.Sdk.MongoDb.Tests.csproj`
    - Remove `<PackageReference Include="System.Net.Http" Version="4.3.4" />`
    - Remove `<PackageReference Include="System.Text.RegularExpressions" Version="4.3.1" />`
    - _Requirements: 7.1, 7.2_
  - [x] 4.4 Remove from `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/Sagi.Sdk.AWS.DynamoDb.Tests.csproj`
    - Remove `<PackageReference Include="System.Net.Http" Version="4.3.4" />`
    - _Requirements: 7.1_

- [x] 5. Update Microsoft.Extensions.* package versions to 9.0.5
  - [x] 5.1 Update `Sagi.Sdk.MongoDb/src/Sagi.Sdk.MongoDb/Sagi.Sdk.MongoDb.csproj`
    - Bump `Microsoft.Extensions.Configuration` from `9.0.4` to `9.0.5`
    - Bump `Microsoft.Extensions.DependencyInjection.Abstractions` from `9.0.4` to `9.0.5`
    - Bump `Microsoft.Extensions.Options.ConfigurationExtensions` from `9.0.4` to `9.0.5`
    - _Requirements: 4.2, 4.3, 4.4_
  - [x] 5.2 Update `Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/Sagi.Sdk.AWS.DynamoDb.csproj`
    - Bump `Microsoft.Extensions.Hosting` from `9.0.4` to `9.0.5`
    - _Requirements: 4.1_
  - [x] 5.3 Update `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/Sagi.Sdk.AWS.DynamoDb.Tests.csproj`
    - Bump `Microsoft.Extensions.Hosting` from `9.0.4` to `9.0.5`
    - _Requirements: 4.1_
  - [x] 5.4 Update `Sagi.Sdk.MongoDb/tests/Sagi.Sdk.MongoDb.Tests/Sagi.Sdk.MongoDb.Tests.csproj`
    - Bump `Microsoft.Extensions.Configuration` from `9.0.4` to `9.0.5`
    - Bump `Microsoft.Extensions.DependencyInjection` from `9.0.4` to `9.0.5`
    - _Requirements: 4.2, 4.3_

- [x] 6. Update test tooling package versions
  - [x] 6.1 Update `Sagi.Sdk.Results/tests/Sagi.Sdk.Results.Tests/Sagi.Sdk.Results.Tests.csproj`
    - Bump `xunit` from `2.5.3` to `2.9.3`
    - Bump `xunit.runner.visualstudio` from `2.5.3` to `2.8.3`
    - Bump `Microsoft.NET.Test.Sdk` from `17.8.0` to `17.14.1`
    - Bump `coverlet.collector` from `6.0.0` to `6.0.4`
    - _Requirements: 5.4, 5.5_
  - [x] 6.2 Update `Sagi.Sdk.Domain/tests/Sagi.Sdk.Domain.Tests/Sagi.Sdk.Domain.Tests.csproj`
    - Bump `xunit` from `2.5.3` to `2.9.3`
    - Bump `xunit.runner.visualstudio` from `2.5.3` to `2.8.3`
    - Bump `Microsoft.NET.Test.Sdk` from `17.8.0` to `17.14.1`
    - Bump `coverlet.collector` from `6.0.0` to `6.0.4`
    - _Requirements: 5.4, 5.5_
  - [x] 6.3 Update `Sagi.Sdk.MongoDb/tests/Sagi.Sdk.MongoDb.Tests/Sagi.Sdk.MongoDb.Tests.csproj`
    - Bump `xunit` from `2.5.3` to `2.9.3`
    - Bump `xunit.runner.visualstudio` from `2.5.3` to `2.8.3`
    - Bump `Microsoft.NET.Test.Sdk` from `17.8.0` to `17.14.1`
    - Bump `coverlet.collector` from `6.0.0` to `6.0.4`
    - _Requirements: 5.4, 5.5_
  - [x] 6.4 Update `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/Sagi.Sdk.AWS.DynamoDb.Tests.csproj`
    - Bump `xunit` from `2.9.2` to `2.9.3`
    - Bump `xunit.runner.visualstudio` from `2.8.2` to `2.8.3`
    - Bump `Microsoft.NET.Test.Sdk` from `17.12.0` to `17.14.1`
    - Bump `coverlet.collector` from `6.0.2` to `6.0.4`
    - _Requirements: 5.4, 5.5_

- [x] 7. Checkpoint — Verify restore and build succeed
  - Run `dotnet restore Sagi.Sdk.sln` and confirm zero errors
  - Run `dotnet build Sagi.Sdk.sln --no-restore -c Release` and confirm zero errors and zero warnings
  - If any CS-prefixed build errors appear due to .NET 10 breaking changes, fix them at the call site in the affected source file
  - Ensure all tests pass, ask the user if questions arise.
  - _Requirements: 9.1, 9.2, 9.3_

- [x] 8. Validate full test suite and pack
  - Run `dotnet test Sagi.Sdk.sln --no-build -c Release` and confirm zero failures
  - Run `dotnet pack Sagi.Sdk.sln -c Release --no-build` and confirm `.nupkg` files are generated for all four SDK projects
  - Inspect the generated `.nuspec` inside each package to confirm `<targetFramework>net10.0</targetFramework>` is present
  - _Requirements: 10.1, 10.2, 10.3_

## Notes

- The CI workflow (`.github/workflows/ci.yaml`) delegates to reusable workflows in `laboratorio-net/actions` and contains no inline `dotnet-version` entries — no changes needed there.
- `Microsoft.Extensions.*` packages are intentionally kept at `9.0.x` (not `10.0.x`) to preserve multi-target compatibility with net8.0 and net9.0 consumers.
- AWS SDK, MongoDB.Driver, NSubstitute, AutoFixture, Bogus, and FluentDocker all target `netstandard2.0` and require no version changes.
- No property-based tests are added — this upgrade has no new business logic to test.
