# FluentAAS.Builder - Published NuGet Package

[![NuGet Version](https://img.shields.io/nuget/v/FluentAAS.Builder?label=NuGet&logo=nuget)](https://www.nuget.org/packages/FluentAAS.Builder)
[![NuGet Downloads](https://img.shields.io/nuget/dt/FluentAAS.Builder?label=Downloads)](https://www.nuget.org/packages/FluentAAS.Builder)
[![GitHub Release](https://img.shields.io/github/v/release/Freezor/FluentAAS?label=GitHub%20Release)](https://github.com/Freezor/FluentAAS/releases)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

> **FluentAAS.Builder is available on NuGet:**  
> https://www.nuget.org/packages/FluentAAS.Builder

FluentAAS.Builder provides a fluent .NET API for creating **Asset Administration Shell (AAS)** models with a developer-friendly workflow.

---

## Install

```bash
dotnet add package FluentAAS.Builder
```

NuGet package page: https://www.nuget.org/packages/FluentAAS.Builder

---

## Quick Start

```csharp
using FluentAAS;

var environment = AasBuilder.Create()
    .AddShell("urn:aas:example:machine", "DemoMachine")
    .WithGlobalAssetId("urn:asset:example:001")
    .AddDigitalNameplate("urn:submodel:example:nameplate")
        .WithManufacturerName("en", "Acme Manufacturing")
        .WithSerialNumber("SN-001")
        .BuildDigitalNameplate()
    .CompleteShellConfiguration()
    .Build();
```

---

## Package Metadata & Links

- **NuGet package:** https://www.nuget.org/packages/FluentAAS.Builder
- **Repository:** https://github.com/Freezor/FluentAAS
- **Releases:** https://github.com/Freezor/FluentAAS/releases
- **Documentation:** https://github.com/Freezor/FluentAAS#readme

---

## Features at a Glance

- Fluent builder API for AAS model creation
- Support for AAS-related templates and structured model building
- JSON/AASX related integrations via companion packages
- Designed for discoverability and traceability through NuGet + GitHub metadata

---

## Badge Syntax (copy/paste)

Use this exact shields.io badge for the latest NuGet version:

```md
[![NuGet Version](https://img.shields.io/nuget/v/FluentAAS.Builder?label=NuGet&logo=nuget)](https://www.nuget.org/packages/FluentAAS.Builder)
```

Template for another package:

```md
[![NuGet Version](https://img.shields.io/nuget/v/<PACKAGE_ID>?label=NuGet&logo=nuget)](https://www.nuget.org/packages/<PACKAGE_ID>)
```

---

## CI/CD Publishing Note

This repository's GitHub Actions pipeline packs with `/p:PackageVersion=...` and pushes to nuget.org.
As long as package metadata in `.csproj` remains correct, the package and source links stay visible and discoverable from both GitHub and NuGet.
