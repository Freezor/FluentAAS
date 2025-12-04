# FluentAAS – Fluent C# Library for the Asset Administration Shell (AAS)

**License:** MIT  
**NuGet:** _coming soon_  
**Status:** Active development  

FluentAAS is a modular **Fluent API** for creating, validating, serializing, and packaging **Asset Administration Shell (AAS 3.0)** models in C#.  
It serves as a developer-friendly abstraction over the official AAS Core meta-model, enabling both flexible generic modeling *and* highly structured, specification-compliant **supported submodel templates**.

---

# Key Features

### Fluent Builder API  
Create Asset Administration Shells, Submodels, and Submodel Elements using a clean, expressive DSL.

### Immutable AAS Model  
All AAS graph elements are immutable C# records, ideal for TDD and snapshot tests.

### Full AAS Validation  
Includes a standalone `IValidationService` with detailed validation reports.

### JSON / AASX Serialization  
Serialize and deserialize environments using `FluentAAS.IO`.

### Supported Submodel Templates  
Includes prebuilt builders for frequently used AAS Submodels such as:

- **Digital Nameplate V2_0**  
- …more templates coming soon

These templates ensure developers produce specification-compliant submodels with minimal effort.

---

# 📦 Project Structure

| Package                  | Responsibility                                        |
|--------------------------|-------------------------------------------------------|
| **FluentAAS.Core**       | Immutable AAS meta-model types                        |
| **FluentAAS.Builder**    | Fluent builders for AAS entities + submodel templates |
| **FluentAAS.Validation** | Rule-based meta-model validation                      |
| **FluentAAS.IO**         | JSON + AASX serialization/deserialization helpers     |

---

# Getting Started

## Installation

```bash
dotnet add package FluentAAS
````

(If not yet available, clone the repository and reference the projects directly.)

---

# How to Build AAS Models with FluentAAS

FluentAAS offers **two different ways** to build submodels:

---

# 1. Using Supported Submodel Templates

*(Recommended when your submodel is covered by an official spec)*

These builders:

* enforce required fields
* automatically apply semantic IDs
* ensure correct AAS element hierarchy
* dramatically reduce boilerplate

### Example: Building a Digital Nameplate Submodel (V2.0)

Based on your integration test:

```csharp
var environment = AasBuilder.Create()
    .AddShell("urn:aas:example:my-shell", "MyShell")
    .WithGlobalAssetId("urn:asset:example:my-asset")
    .AddDigitalNameplate("urn:submodel:example:digital-nameplate:V2_0")
        .WithManufacturerName("de", "Muster AG")
        .WithManufacturerName("en", "Sample Corp")
        .WithManufacturerProductDesignation("de", "Super-Antriebseinheit XS")
        .WithManufacturerProductDesignation("en", "Super Drive Unit XS")
        .WithSerialNumber("SN-000123")
        .Build()    // finish the Digital Nameplate
    .Done()        // return to shell builder
    .Build();      // finish AAS environment
```

### Serialize / Deserialize

```csharp
string json = AasJsonSerializer.ToJson(environment);
var env2 = AasJsonSerializer.FromJson(json);
```

### Export to AASX

```csharp
environment.ToAasx("./example.aasx", "/aas/env.json");
```

This is the **easiest and safest** way to build complex spec-conformant submodels.

---

# 2. Using the Generic Submodel Builder

*(Recommended when no official FluentAAS template exists yet)*

Use this when creating:

* your own custom submodels
* unofficial AAS extensions
* submodels not yet added to `FluentAAS.Templates`

### Example: Creating a Custom Submodel

```csharp
var environment = AasBuilder.Create()
    .AddShell("urn:aas:example:my-shell", "MyShell")
    .WithGlobalAssetId("urn:asset:example:my-asset")
    .AddSubmodel("urn:aas:example:generic-submodel:1", "GenericSubmodel")
        .WithSemanticId(new Reference(
            ReferenceTypes.ExternalReference,
            [
                new Key(KeyTypes.Submodel, "urn:aas:example:generic-submodel:semantic-id")
            ]))
        .AddMultiLanguageProperty(
            "GenericMultiLanguageProperty",
            ls => ls.Add("en", "Example value")
                    .Add("de", "Beispielwert"))
        .AddElement("GenericProperty", "example value")
    .Done()
    .Build();
```

This approach gives you **full control**, mirroring the raw AAS 3.0 capabilities.

---

# Supported Submodels vs Generic Submodels

| Type                                | When to Use                                                         | Advantages                                                 |
|-------------------------------------|---------------------------------------------------------------------|------------------------------------------------------------|
| **Supported Submodels (Templates)** | You need official AAS-compliant submodels (e.g., Digital Nameplate) | Automatic semantic IDs, required field checks, cleaner API |
| **Generic Submodels**               | Custom extensions, early drafts, unsupported specs                  | Total flexibility, full AAS feature surface                |

### Philosophy

FluentAAS lets you:

* **Be productive quickly** when using known AAS submodels
* **Be fully expressive** when building anything custom

This duality is one of the core design goals of the library.

---

# 🤝 Contributing

Pull requests are welcome for:

* Additional official submodel templates
* Improved validation rules
* Useful builder extensions
* Performance improvements
