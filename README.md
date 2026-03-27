# FluentAAS – Fluent C# Library for the Asset Administration Shell

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/FluentAAS.Builder.svg)](https://www.nuget.org/packages/FluentAAS.Builder/)
[![Downloads](https://img.shields.io/nuget/dt/FluentAAS.Builder.svg)](https://www.nuget.org/packages/FluentAAS.Builder/)
[![.NET](https://img.shields.io/badge/.NET-9.0%2B-512BD4)](https://dotnet.microsoft.com/)

Building AAS models in C# shouldn't require reading 500 pages of specs first.

FluentAAS gives you a fluent API that feels like writing normal C# code – while handling the complexity of AAS 3.0 compliance under the hood.

```csharp
var environment = AasBuilder.Create()
    .AddShell("urn:aas:my-machine", "CNC-Machine-2000")
    .WithGlobalAssetId("urn:asset:serial-001")
    .AddDigitalNameplate("urn:submodel:nameplate")
        .WithManufacturerName("en", "Acme Manufacturing Ltd.")
        .WithSerialNumber("SN-2024-00142")
        .BuildDigitalNameplate()
    .CompleteShellConfiguration()
    .Build();
```

That's it. A valid, IDTA-compliant AAS model. Ready to export.


## 📦 Published NuGet Package

[![NuGet Version](https://img.shields.io/nuget/v/FluentAAS.Builder?label=NuGet%20Version)](https://www.nuget.org/packages/FluentAAS.Builder/)

**FluentAAS.Builder is published on NuGet and ready for production use.**

- **Package page:** https://www.nuget.org/packages/FluentAAS.Builder/
- **Package README (NuGet):** https://www.nuget.org/packages/FluentAAS.Builder/#readme-body-tab
- **Documentation:** https://github.com/Freezor/FluentAAS#readme
- **GitHub releases:** https://github.com/Freezor/FluentAAS/releases

### Install

```bash
dotnet add package FluentAAS.Builder
```

### Quick Reference

- Build AAS environments with a fluent C# API.
- Use official IDTA-oriented templates like Digital Nameplate.
- Validate and export models as JSON or AASX.

---

---

## The Problem

If you've ever tried to create AAS models programmatically, you know the pain:

- The IDTA specs are hundreds of pages long
- Most tooling is Java-based (Eclipse BaSyx, AASX Package Explorer)
- Building JSON/XML structures by hand is error-prone
- Semantic IDs are easy to get wrong – and you won't know until import fails

I built FluentAAS because I needed something that just works for .NET developers. No ceremony, no boilerplate, no guessing if your model is valid.

---

## What FluentAAS Does

**Fluent Builder API**  
Create shells, submodels, and elements using a clean, chainable syntax. IntelliSense guides you through the API.

**Supported Submodel Templates**  
Pre-built builders for official IDTA templates (Digital Nameplate, more coming). They enforce required fields and set semantic IDs automatically.

**Immutable Model**  
All AAS types are C# records. Great for testing, snapshots, and debugging.

**Validation**  
Catch problems before export. The validation service gives you detailed error reports.

**JSON & AASX Export**  
Serialize to JSON or package as AASX for import into other tools.

---

## Getting Started

```bash
dotnet add package FluentAAS.Builder
```

### Basic Example

```csharp
using FluentAAS;

// Create an AAS environment
var environment = AasBuilder.Create()
    .AddShell("urn:aas:example:cnc-machine", "CNC-Mill-2000")
    .WithGlobalAssetId("urn:asset:example:cnc-001")
    
    // Add a Digital Nameplate (IDTA 02006-2-0)
    .AddDigitalNameplate("urn:submodel:example:nameplate")
        .WithManufacturerName("en", "Acme Manufacturing Ltd.")
        .WithManufacturerName("de", "Acme Maschinenbau GmbH")
        .WithManufacturerProductDesignation("en", "Universal CNC Milling Machine")
        .WithSerialNumber("SN-2024-00142")
        .WithYearOfConstruction("2024")
        .BuildDigitalNameplate()
    .CompleteShellConfiguration()
    .Build();

// Export as JSON
string json = AasJsonSerializer.ToJson(environment);

// Or as AASX package
environment.ToAasx("./cnc-machine.aasx");
```

---

## Two Ways to Build Submodels

### 1. Supported Templates (Recommended)

Use these when your submodel matches an official IDTA template. The builder enforces required fields and sets the correct semantic IDs.

```csharp
.AddDigitalNameplate("urn:submodel:nameplate")
    .WithManufacturerName("en", "Acme Ltd.")    // required
    .WithManufacturerProductDesignation("en", "Controller") // required
    .WithSerialNumber("SN-001")                  // required
    .WithContactInformation(contact => contact
        .WithManufacturerContact("Acme Service Desk", "+49 123 456789") // required role
        .WithServiceHotline("+49 800 111222")                            // required role
        .WithEmail("info@acme.example")
        .WithWebsiteUrl("https://acme.example/support")
        .WithAddress("Industrial Ave 3", "Munich", "80331", "DE"))
    .BuildDigitalNameplate()
```

**Currently supported:**
- Digital Nameplate V2.0 (IDTA 02006-2-0)

**Planned:**
- Handover Documentation
- Technical Data
- Digital Product Passport

### 2. Generic Builder

For custom submodels or templates not yet supported:

```csharp
.AddSubmodel("urn:submodel:custom", "ProductionData")
    .WithSemanticId(new Reference(
        ReferenceTypes.ExternalReference,
        [new Key(KeyTypes.Submodel, "urn:my-company:production-data:1.0")]))
    .AddProperty("Temperature", "85.5")
    .AddProperty("SpindleSpeed", "1200")
    .AddMultiLanguageProperty("Status", ls => ls
        .Add("en", "Running")
        .Add("de", "Läuft"))
    .CompleteSubmodelConfiguration()
```

You get full control when you need it.

---

## Staged Submodel Composition (`.AddSubmodel(...)`)

For modular systems (DDD layers, plugins, stepwise enrichment), you can now register and enrich submodels at any point before `Build()`.

### Register fully built submodels

```csharp
var builder = AasBuilder.Create()
    .AddShell("urn:aas:example:machine-42", "Machine42")
    .CompleteShellConfiguration()
    .AddSubmodel(new Submodel("urn:submodel:ops") { IdShort = "Operations" })
    .AddSubmodel(new Submodel("urn:submodel:maintenance") { IdShort = "Maintenance" });
```

### Add staged fragments by submodel ID

```csharp
builder
    .AddSubmodelFragment("urn:submodel:ops", fragment => fragment
        .AddProperty("Status", "Running")
        .AddProperty("Shift", "Night"))
    .AddSubmodelFragment("urn:submodel:maintenance", fragment => fragment
        .AddMultiLanguageProperty("LastService", ls => ls
            .Add("en", "Completed")
            .Add("de", "Abgeschlossen")));

var environment = builder.Build();
```

### Why use staged composition?

- Build submodels in separate modules/services and attach them later.
- Support plugin-style late binding with runtime-loaded components.
- Preserve fluent chaining and existing inline builder methods.
- Keep strong validation guarantees:
  - `Submodel.Id` and `Submodel.IdShort` are validated on add.
  - Duplicate submodel IDs are rejected at build-time.
  - Fragment targets must exist.
  - Shell submodel references must resolve to known submodels.

> Backward compatibility note: `AddExistingSubmodel(...)` still works, but it delegates to `AddSubmodelInternal(...)` (not `AddSubmodel(...)`) and therefore skips the additional public `IdShort` validation performed by `AddSubmodel(...)`.

---

## Project Structure

| Package                  | What it does                                |
|--------------------------|---------------------------------------------|
| **FluentAAS.Core**       | Immutable AAS meta-model types (C# records) |
| **FluentAAS.Builder**    | Fluent builders + submodel templates        |
| **FluentAAS.Validation** | Rule-based validation against AAS 3.0       |
| **FluentAAS.IO**         | JSON serialization, AASX packaging          |

---

## Validation

```csharp
var validator = new AasValidationService();
var report = validator.Validate(environment);

if (!report.IsValid)
{
    foreach (var error in report.Errors)
    {
        Console.WriteLine($"{error.Path}: {error.Message}");
    }
}
```

Find issues before your customer does.

---

## Why I Built This

I worked on AAS tooling at Fraunhofer IOSB-INA. The official specs are thorough – but not exactly developer-friendly. Most examples are in Java. The XML/JSON structures are verbose. And when something doesn't work, the error messages rarely help.

FluentAAS is my attempt to make AAS practical for .NET teams. Not a complete replacement for the official tools – but a library that handles the common cases well.

---

## What This Is (and Isn't)

**FluentAAS is:**
- A library for creating AAS models in C#
- Focused on developer experience
- Good for generating Digital Nameplates and similar submodels
- MIT licensed, use it however you want

**FluentAAS is not:**
- A full AAS server implementation
- A replacement for AASX Package Explorer
- Feature-complete (yet)

I'm building this in the open. If a template you need is missing, let me know – or submit a PR.

---

## Roadmap

| Feature                  | Status     |
|--------------------------|------------|
| Digital Nameplate V2.0   | ✅ Done     |
| JSON/AASX Export         | ✅ Done     |
| Validation Service       | ✅ Done     |
| Handover Documentation   | ✅ Done     |
| Technical Data           | 📋 Planned |
| Digital Product Passport | 📋 Planned |
| AAS Registry Integration | 📋 Planned |

---

## Contributing

PRs welcome for:

- New IDTA submodel templates
- Better validation rules
- Documentation and examples
- Bug fixes

If you're unsure whether something fits, open an issue first.

---

## Resources

- [IDTA Submodel Templates](https://industrialdigitaltwin.org/content-hub/submodels) – Official template specs
- [AASX Package Explorer](https://github.com/admin-shell-io/aasx-package-explorer) – GUI tool for viewing/editing AAS
- [AAS Specs](https://industrialdigitaltwin.org/content-hub/aasspecifications) – The full specification

---

## License

MIT – free for commercial and open-source use.

---

## Questions?

If you're working on AAS integration and have questions, feel free to reach out on [LinkedIn](https://www.linkedin.com/in/oliver-fries-industrie40-dotnet/).

I'm also available for consulting on .NET modernization and Industry 4.0 projects – but no pressure. The library stands on its own.
