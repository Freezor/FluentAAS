# FluentAAS – Fluent C# Library for the Asset Administration Shell (AAS)

**License:** MIT  
**Goal:** A clean, fluent, modular C# library for creating, validating, and serializing AAS models.

---

## Overview

**FluentAAS** provides an intuitive **Fluent API** and modular architecture for working with Asset Administration Shell (AAS) models.  
It focuses on readability, immutability, strong validation, and CI/CD-friendly outputs.

---

## Features

- **Fluent Builder API** for concise AAS model construction
- **Immutable C# records** representing the AAS meta-model
- **Dedicated validation service (`IValidationService`)**
- **Machine-readable validation reports** for pipelines
- **Modular architecture** separated into:
    - `FluentAas.Core` – domain models
    - `FluentAas.Builder` – fluent creation
    - `FluentAas.Validation` – model checks
    - `FluentAas.IO` – JSON/XML serialization

---

## Example

```csharp
var aas = AasBuilder.Create("urn:aas:id:123")
    .WithAsset("urn:asset:id:xyz", AssetKind.Instance)
    .AddSubmodel(SubmodelBuilder.Create("urn:submodel:id:data")
        .AddProperty("Manufacturer", "ACME Corp")
        .Build())
    .Build();
