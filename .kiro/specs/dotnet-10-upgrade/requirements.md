# Requirements Document

## Introduction

Upgrade the Sagi SDKs monorepo from .NET 8 to .NET 10. The upgrade covers the SDK runtime version declared in `global.json`, the `TargetFramework` in all `.csproj` files across the 4 SDKs and their test projects, and the NuGet package versions for all dependencies. The goal is to ensure the entire solution builds and all tests pass on .NET 10 without regressions.

All NuGet packages selected during the upgrade MUST support `net8.0`, `net9.0`, and `net10.0` as target frameworks (multi-targeting compatibility). No package version that exclusively targets `net10.0` shall be chosen if it drops support for `net8.0` or `net9.0`.

## Glossary

- **Monorepo**: The single repository containing all four Sagi SDKs under `Sagi.Sdk.sln`.
- **SDK_Runtime**: The .NET SDK version declared in `global.json` used to build and run the solution.
- **TargetFramework**: The MSBuild property in each `.csproj` that specifies the target .NET version (e.g., `net8.0`, `net10.0`).
- **global.json**: The file at the repository root that pins the .NET SDK version with `rollForward` policy.
- **Source_Project**: Any `.csproj` under a `src/` folder (library project).
- **Test_Project**: Any `.csproj` under a `tests/` folder (test project).
- **NuGet_Package**: A third-party or Microsoft library declared as a `<PackageReference>` in a `.csproj`.
- **Multi_Target_Compatible**: A NuGet package version whose `.nuspec` declares target framework support for `net8.0`, `net9.0`, and `net10.0` simultaneously.
- **Microsoft_Extensions**: The family of packages under `Microsoft.Extensions.*` (Hosting, Configuration, DependencyInjection, Options).
- **Build**: The process of compiling the solution via `dotnet build`.
- **Test_Suite**: The full set of xUnit tests executed via `dotnet test`.
- **Upgrade**: The act of changing version identifiers from .NET 8 to .NET 10 and updating compatible NuGet packages.

---

## Requirements

### Requirement 1: Update SDK Runtime Declaration

**User Story:** As a developer, I want the repository's `global.json` to target .NET 10, so that all builds and tooling use the correct SDK version.

#### Acceptance Criteria

1. THE `global.json` SHALL declare `"version": "10.0.0"` under the `sdk` key.
2. THE `global.json` SHALL retain `"rollForward": "latestMajor"` to allow use of any installed .NET 10 SDK patch.
3. THE `global.json` SHALL retain `"allowPrerelease": true` to permit preview SDK versions during the .NET 10 adoption period.

---

### Requirement 2: Update TargetFramework in All Source Projects

**User Story:** As a developer, I want all library `.csproj` files to target `net10.0`, so that the published NuGet packages are built against .NET 10.

#### Acceptance Criteria

1. THE `Sagi.Sdk.Results` Source_Project SHALL declare `<TargetFramework>net10.0</TargetFramework>`.
2. THE `Sagi.Sdk.Domain` Source_Project SHALL declare `<TargetFramework>net10.0</TargetFramework>`.
3. THE `Sagi.Sdk.MongoDb` Source_Project SHALL declare `<TargetFramework>net10.0</TargetFramework>`.
4. THE `Sagi.Sdk.AWS.DynamoDb` Source_Project SHALL declare `<TargetFramework>net10.0</TargetFramework>`.
5. THE `Samples` Source_Project under `Sagi.Sdk.AWS.DynamoDb/src/Samples/` SHALL declare `<TargetFramework>net10.0</TargetFramework>`.

---

### Requirement 3: Update TargetFramework in All Test Projects

**User Story:** As a developer, I want all test `.csproj` files to target `net10.0`, so that tests run on the same runtime as the libraries.

#### Acceptance Criteria

1. THE `Sagi.Sdk.Results.Tests` Test_Project SHALL declare `<TargetFramework>net10.0</TargetFramework>`.
2. THE `Sagi.Sdk.Domain.Tests` Test_Project SHALL declare `<TargetFramework>net10.0</TargetFramework>`.
3. THE `Sagi.Sdk.MongoDb.Tests` Test_Project SHALL declare `<TargetFramework>net10.0</TargetFramework>`.
4. THE `Sagi.Sdk.AWS.DynamoDb.Tests` Test_Project SHALL declare `<TargetFramework>net10.0</TargetFramework>`.

---

### Requirement 4: Update Microsoft.Extensions NuGet Packages

**User Story:** As a developer, I want all `Microsoft.Extensions.*` packages to use versions compatible with .NET 10 while remaining compatible with .NET 8 and .NET 9, so that there are no dependency resolution conflicts and the packages can be consumed by projects targeting any of those frameworks.

#### Acceptance Criteria

1. WHEN a `.csproj` references `Microsoft.Extensions.Hosting`, THE NuGet_Package version SHALL be updated to a Multi_Target_Compatible release that supports `net8.0`, `net9.0`, and `net10.0`.
2. WHEN a `.csproj` references `Microsoft.Extensions.Configuration`, THE NuGet_Package version SHALL be updated to a Multi_Target_Compatible release that supports `net8.0`, `net9.0`, and `net10.0`.
3. WHEN a `.csproj` references `Microsoft.Extensions.DependencyInjection` or `Microsoft.Extensions.DependencyInjection.Abstractions`, THE NuGet_Package version SHALL be updated to a Multi_Target_Compatible release that supports `net8.0`, `net9.0`, and `net10.0`.
4. WHEN a `.csproj` references `Microsoft.Extensions.Options.ConfigurationExtensions`, THE NuGet_Package version SHALL be updated to a Multi_Target_Compatible release that supports `net8.0`, `net9.0`, and `net10.0`.
5. IF a `Microsoft.Extensions.*` package does not yet have a stable Multi_Target_Compatible release, THEN THE Source_Project or Test_Project SHALL reference the latest available preview version that supports `net8.0`, `net9.0`, and `net10.0`.

---

### Requirement 5: Review and Update Third-Party NuGet Packages

**User Story:** As a developer, I want all third-party NuGet packages to be verified for compatibility with .NET 8, .NET 9, and .NET 10 and updated if necessary, so that the solution builds without warnings or errors and the packages remain usable by consumers on any of those frameworks.

#### Acceptance Criteria

1. WHEN `MongoDB.Driver` is referenced, THE NuGet_Package version SHALL be a Multi_Target_Compatible release that supports `net8.0`, `net9.0`, and `net10.0`; IF the current version does not satisfy this, THEN THE version SHALL be updated to the latest release that does.
2. WHEN `AWSSDK.DynamoDBv2`, `AWSSDK.Core`, `AWSSDK.SecurityToken`, or `AWSSDK.Extensions.NETCore.Setup` are referenced, THE NuGet_Package versions SHALL be Multi_Target_Compatible releases that support `net8.0`, `net9.0`, and `net10.0`; IF the current versions do not satisfy this, THEN THE versions SHALL be updated to the latest releases that do.
3. WHEN `Kralizek.Extensions.Configuration.AWSSecretsManager` is referenced, THE NuGet_Package version SHALL be a Multi_Target_Compatible release that supports `net8.0`, `net9.0`, and `net10.0`; IF the current version does not satisfy this, THEN THE version SHALL be updated to the latest release that does.
4. WHEN `xunit`, `xunit.runner.visualstudio`, or `Microsoft.NET.Test.Sdk` are referenced in a Test_Project, THE NuGet_Package versions SHALL be updated to the latest stable Multi_Target_Compatible releases that support `net8.0`, `net9.0`, and `net10.0`.
5. WHEN `coverlet.collector` is referenced in a Test_Project, THE NuGet_Package version SHALL be updated to the latest stable Multi_Target_Compatible release that supports `net8.0`, `net9.0`, and `net10.0`.
6. WHEN `NSubstitute`, `AutoFixture.*`, or `Bogus` are referenced in a Test_Project, THE NuGet_Package versions SHALL be Multi_Target_Compatible releases that support `net8.0`, `net9.0`, and `net10.0`; IF the current versions do not satisfy this, THEN THE versions SHALL be updated to the latest releases that do.
7. WHEN `Ductus.FluentDocker` is referenced in a Test_Project, THE NuGet_Package version SHALL be a Multi_Target_Compatible release that supports `net8.0`, `net9.0`, and `net10.0`; IF the current version does not satisfy this, THEN THE version SHALL be updated to the latest release that does.

---

### Requirement 6: Enforce Multi-Targeting Compatibility for All NuGet Packages

**User Story:** As a developer, I want every NuGet package version chosen during the upgrade to support `net8.0`, `net9.0`, and `net10.0`, so that consumers of the Sagi SDKs on older supported runtimes are not broken.

#### Acceptance Criteria

1. THE Upgrade SHALL select only NuGet_Package versions that are Multi_Target_Compatible, declaring support for `net8.0`, `net9.0`, and `net10.0` in their target framework metadata.
2. WHEN a NuGet_Package version supports `net10.0` but drops support for `net8.0` or `net9.0`, THE Upgrade SHALL NOT select that version and SHALL use the latest version that retains support for all three frameworks.
3. IF no released version of a NuGet_Package supports all three frameworks simultaneously, THEN THE affected package SHALL be flagged for manual review before the Upgrade is considered complete.

---

### Requirement 7: Remove Redundant In-Box Packages

**User Story:** As a developer, I want packages that are included in-box with .NET 10 to be removed from `.csproj` files, so that there are no unnecessary or conflicting references.

#### Acceptance Criteria

1. WHEN `System.Net.Http` is referenced with a version pinned to `4.3.4`, THE reference SHALL be removed from all `.csproj` files because the package is included in the .NET 10 runtime.
2. WHEN `System.Text.RegularExpressions` is referenced with a version pinned to `4.3.1`, THE reference SHALL be removed from all `.csproj` files because the package is included in the .NET 10 runtime.

---

### Requirement 9: Solution Builds Successfully on .NET 10

**User Story:** As a developer, I want the entire solution to compile without errors on .NET 10, so that I can confirm the upgrade is complete and correct.

#### Acceptance Criteria

1. WHEN `dotnet build Sagi.Sdk.sln` is executed after the Upgrade, THE Build SHALL complete with zero errors.
2. WHEN `dotnet build Sagi.Sdk.sln` is executed after the Upgrade, THE Build SHALL produce zero warnings related to deprecated APIs or target framework incompatibilities.
3. IF any source-level breaking change is introduced by .NET 10, THEN THE affected Source_Project or Test_Project SHALL be updated to resolve the breaking change before the Build is considered complete.

---

### Requirement 10: All Tests Pass on .NET 10

**User Story:** As a developer, I want the full Test_Suite to pass after the Upgrade, so that I have confidence no regressions were introduced.

#### Acceptance Criteria

1. WHEN `dotnet test Sagi.Sdk.sln` is executed after the Upgrade, THE Test_Suite SHALL report zero failed tests.
2. WHEN `dotnet test Sagi.Sdk.sln` is executed after the Upgrade, THE Test_Suite SHALL report the same number of passing tests as before the Upgrade.
3. IF a test failure is caused by a .NET 10 behavioral change rather than a product bug, THEN THE Test_Project SHALL be updated to reflect the correct expected behavior on .NET 10.
