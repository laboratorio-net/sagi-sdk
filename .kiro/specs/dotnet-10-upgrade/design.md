# Design Document — .NET 10 Upgrade

## Overview

This document describes the technical approach for upgrading the Sagi SDKs monorepo from .NET 8 to .NET 10. The upgrade is a mechanical version-bump across configuration files, project files, and NuGet package references. There is no new business logic; the goal is a clean build and a green test suite on .NET 10 while preserving backward compatibility for consumers on net8.0 and net9.0.

The upgrade touches five categories of files:

1. `global.json` — SDK runtime pin
2. Source `.csproj` files (4 SDKs + 1 Samples executable)
3. Test `.csproj` files (4 test projects)
4. NuGet package versions across all projects
5. Source code — only if .NET 10 introduces breaking changes that affect the existing code

---

## Architecture

The monorepo has a flat dependency graph:

```
Sagi.Sdk.Results          (no external NuGet deps)
    ↑
Sagi.Sdk.Domain           (depends on Results; System.Net.Http + System.Text.RegularExpressions to be removed)
    ↑
Sagi.Sdk.AWS.DynamoDb     (depends on Results; AWSSDK.* + Microsoft.Extensions.Hosting)
    ↑
Sagi.Sdk.AWS.DynamoDb/src/Samples  (executable; depends on DynamoDb SDK)

Sagi.Sdk.MongoDb          (independent; MongoDB.Driver + Microsoft.Extensions.*)
```

All projects currently target `net8.0`. After the upgrade all will target `net10.0`. Because the SDKs are published as NuGet packages consumed by downstream projects that may still run on net8.0 or net9.0, every selected package version must carry TFM support for all three frameworks.

---

## Components and Interfaces

### global.json

Single change: bump `version` from `"8.0.0"` to `"10.0.0"`. The `rollForward: latestMajor` and `allowPrerelease: true` keys are retained unchanged.

```json
{
  "sdk": {
    "version": "10.0.0",
    "rollForward": "latestMajor",
    "allowPrerelease": true
  }
}
```

### Source Projects — TargetFramework

Each source `.csproj` changes `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`.

Affected files:
- `Sagi.Sdk.Results/src/Sagi.Sdk.Results/Sagi.Sdk.Results.csproj`
- `Sagi.Sdk.Domain/src/Sagi.Sdk.Domain/Sagi.Sdk.Domain.csproj`
- `Sagi.Sdk.MongoDb/src/Sagi.Sdk.MongoDb/Sagi.Sdk.MongoDb.csproj`
- `Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/Sagi.Sdk.AWS.DynamoDb.csproj`
- `Sagi.Sdk.AWS.DynamoDb/src/Samples/Samples.csproj`

### Test Projects — TargetFramework

Same change in each test `.csproj`.

Affected files:
- `Sagi.Sdk.Results/tests/Sagi.Sdk.Results.Tests/Sagi.Sdk.Results.Tests.csproj`
- `Sagi.Sdk.Domain/tests/Sagi.Sdk.Domain.Tests/Sagi.Sdk.Domain.Tests.csproj`
- `Sagi.Sdk.MongoDb/tests/Sagi.Sdk.MongoDb.Tests/Sagi.Sdk.MongoDb.Tests.csproj`
- `Sagi.Sdk.AWS.DynamoDb/tests/Sagi.Sdk.AWS.DynamoDb.Tests/Sagi.Sdk.AWS.DynamoDb.Tests.csproj`

---

## Data Models

### NuGet Package Version Matrix

All versions below have been selected to satisfy the Multi_Target_Compatible constraint (net8.0 + net9.0 + net10.0). The selection rule is: use the latest stable release whose `.nuspec` lists `net8.0`, `net9.0`, and `net10.0` (or `netstandard2.0`/`netstandard2.1` which are compatible with all three).

#### Microsoft.Extensions.* packages

These ship as part of the .NET platform release train. Version `10.0.0` targets net10.0 only. Version `9.0.x` targets net8.0 and net9.0. The correct multi-targeting choice is **`9.0.x`** — specifically the latest `9.0.*` patch — because those packages declare `net8.0` and `net9.0` TFM support and are fully compatible at runtime on net10.0 via the `netstandard2.0`/`netstandard2.1` fallback or the `net9.0` TFM.

> Design decision: Microsoft.Extensions `10.0.x` packages drop net8.0 support. To satisfy the Multi_Target_Compatible requirement (net8.0 + net9.0 + net10.0), the latest `9.0.*` patch is the correct choice. These packages run without issue on net10.0 because .NET 10 is backward compatible with net9.0 binaries.

| Package | Current | Target |
|---|---|---|
| `Microsoft.Extensions.Hosting` | `9.0.4` | `9.0.5` |
| `Microsoft.Extensions.Configuration` | `9.0.4` | `9.0.5` |
| `Microsoft.Extensions.DependencyInjection` | `9.0.4` | `9.0.5` |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | `9.0.4` | `9.0.5` |
| `Microsoft.Extensions.Options.ConfigurationExtensions` | `9.0.4` | `9.0.5` |

#### Third-party packages

| Package | Current | Target | Notes |
|---|---|---|---|
| `MongoDB.Driver` | `3.3.0` | `3.3.0` | Targets `netstandard2.0` + `netstandard2.1` — compatible with net8/9/10. No change needed. |
| `AWSSDK.Core` | `3.7.402.35` | `3.7.402.35` | Targets `netstandard2.0` — compatible. No change needed. |
| `AWSSDK.DynamoDBv2` | `3.7.406.17` | `3.7.406.17` | Targets `netstandard2.0` — compatible. No change needed. |
| `AWSSDK.SecurityToken` | `3.7.401.78` | `3.7.401.78` | Targets `netstandard2.0` — compatible. No change needed. |
| `AWSSDK.Extensions.NETCore.Setup` | `3.7.400` | `3.7.400` | Targets `netstandard2.0` — compatible. No change needed. |
| `Kralizek.Extensions.Configuration.AWSSecretsManager` | `1.7.0` | `1.7.0` | Targets `netstandard2.0` — compatible. No change needed. |
| `xunit` | `2.5.3` / `2.9.2` | `2.9.3` | Latest stable; supports net8/9/10. |
| `xunit.runner.visualstudio` | `2.5.3` / `2.8.2` | `2.8.3` | Latest stable; supports net8/9/10. |
| `Microsoft.NET.Test.Sdk` | `17.8.0` / `17.12.0` | `17.14.1` | Latest stable; supports net8/9/10. |
| `coverlet.collector` | `6.0.0` / `6.0.2` | `6.0.4` | Latest stable; supports net8/9/10. |
| `NSubstitute` | `5.3.0` | `5.3.0` | Targets `netstandard2.0` — compatible. No change needed. |
| `AutoFixture.AutoNSubstitute` | `4.18.1` | `4.18.1` | Targets `netstandard2.0` — compatible. No change needed. |
| `AutoFixture.Idioms` | `4.18.1` | `4.18.1` | Targets `netstandard2.0` — compatible. No change needed. |
| `AutoFixture.Xunit2` | `4.18.1` | `4.18.1` | Targets `netstandard2.0` — compatible. No change needed. |
| `Bogus` | `35.6.2` | `35.6.2` | Targets `netstandard2.0` — compatible. No change needed. |
| `Ductus.FluentDocker` | `2.10.59` | `2.10.59` | Targets `netstandard2.0` — compatible. No change needed. |

#### In-box packages to remove

These packages were pinned to old versions that predate .NET Core. They are included in the .NET runtime itself and the explicit `<PackageReference>` entries must be removed to avoid version conflicts.

| Package | Version | Remove from |
|---|---|---|
| `System.Net.Http` | `4.3.4` | `Sagi.Sdk.Domain`, `Sagi.Sdk.Results.Tests`, `Sagi.Sdk.MongoDb.Tests`, `Sagi.Sdk.AWS.DynamoDb.Tests` |
| `System.Text.RegularExpressions` | `4.3.1` | `Sagi.Sdk.Domain`, `Sagi.Sdk.Results.Tests`, `Sagi.Sdk.MongoDb.Tests` |

---

## Correctness Properties

PBT does not apply to this feature. The upgrade consists entirely of configuration file edits and package version bumps. There are no pure functions with varying inputs to test — every acceptance criterion is either a static file content check (SMOKE) or a build/test pipeline execution check (INTEGRATION). Property-based testing would add no value here.

---

## Error Handling

### Potential .NET 10 Source-Level Breaking Changes

.NET 10 is a Standard Term Support (STS) release. The BCL breaking changes between .NET 8 and .NET 10 that are most likely to affect this codebase:

1. **`ArgumentException.ThrowIfNullOrEmpty`** — present since .NET 7, unchanged in .NET 10. No action needed.
2. **`ObjectId.GenerateNewId()`** (MongoDB.Driver) — not a BCL type; governed by the driver version, which is unchanged at 3.3.0.
3. **Regex compiled static fields** — `Regex` behavior is unchanged. The compiled static regexes in `Cnpj.cs`, `Email.cs`, `Phone.cs`, etc. are unaffected.
4. **`IEquatable<T>` / `GetHashCode`** — no behavioral changes in .NET 10.
5. **`BackgroundService`** — `Microsoft.Extensions.Hosting` `BackgroundService` is unchanged in the 9.0.x version being used.

If `dotnet build` surfaces any CS-prefixed errors after the TFM bump, they must be resolved before the upgrade is considered complete (Requirement 9.3). The most common sources are:

- Obsolete API usage promoted to errors via `<TreatWarningsAsErrors>`
- Nullable reference type warnings that become errors
- Removed overloads in BCL types

Resolution approach: fix at the call site in the affected source file; do not suppress warnings globally.

### Package Resolution Failures

If `dotnet restore` fails after the version bumps, the likely cause is a transitive dependency conflict. Resolution steps:

1. Run `dotnet restore --verbosity detailed` to identify the conflicting package.
2. Add an explicit `<PackageReference>` with the required version to the affected `.csproj`.
3. If the conflict is between a Microsoft.Extensions `9.0.x` package and a transitive reference pulling `10.0.x`, pin the `9.0.x` version explicitly.

---

## Testing Strategy

This upgrade has no new business logic, so there are no new unit tests or property tests to write. The validation strategy is entirely integration-based: run the existing test suite on .NET 10 and confirm it passes.

### Step-by-step validation

**Step 1 — Restore**
```sh
dotnet restore Sagi.Sdk.sln
```
Expected: zero errors. Any `NU` warning about package compatibility should be investigated before proceeding.

**Step 2 — Build**
```sh
dotnet build Sagi.Sdk.sln --no-restore -c Release
```
Expected: `Build succeeded. 0 Error(s). 0 Warning(s).`

If warnings appear related to deprecated APIs or TFM incompatibilities, they must be resolved (Requirement 9.2).

**Step 3 — Test**
```sh
dotnet test Sagi.Sdk.sln --no-build -c Release
```
Expected: all tests pass, zero failures, same pass count as the net8.0 baseline.

**Step 4 — Pack (smoke check)**
```sh
dotnet pack Sagi.Sdk.sln -c Release --no-build
```
Expected: `.nupkg` files generated for the four SDK projects. Inspect the generated `.nuspec` inside each package to confirm `<targetFramework>net10.0</targetFramework>` is present.

### Baseline

Before making any changes, record the current test pass count:
```sh
dotnet test Sagi.Sdk.sln --logger "console;verbosity=normal" 2>&1 | grep -E "passed|failed|skipped"
```
This count is the baseline for Requirement 10.2.

### CI considerations

If the repository has a GitHub Actions workflow (`.github/`), the `dotnet-version` matrix or `uses: actions/setup-dotnet` step must be updated to include `10.0.x` alongside any existing `8.0.x` / `9.0.x` entries.
