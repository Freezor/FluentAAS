# Contributing to FluentAAS

Thank you for your interest in contributing!  
FluentAAS is an open-source C# library designed to provide a fluent, modular API for building, validating, and serializing Asset Administration Shell (AAS) models. Contributions of all kinds are welcome — code, bug reports, documentation, tests, and ideas.

## Before You Start

Please take a moment to understand the project structure and responsibilities of each module:

- `FluentAas.Core` — Immutable AAS meta-model types  
- `FluentAas.Builder` — Fluent DSL for creating models  
- `FluentAas.Validation` — Validation rules & reporting  
- `FluentAas.IO` — JSON/XML serialization utilities  

(See architecture overview for details.)

This helps ensure contributions follow the intended modular design. :contentReference[oaicite:1]{index=1}

---

# How to Contribute

## 1. Fork & Clone

```bash
git clone https://github.com/<yourname>/FluentAAS.git
cd FluentAAS
````

## 2. Create a Feature Branch

```bash
git checkout -b feature/my-improvement
```

Use clear branch names such as:

* `feature/...`
* `fix/...`
* `docs/...`
* `test/...`

## 3. Keep Pull Requests Small

Prefer many small, focused PRs over one large one.
A good PR should do **one thing well**.

Examples:

* Add a builder method
* Add a validation rule
* Improve documentation
* Add integration tests

---

# Testing Guidelines

Contributions must include tests when reasonable:

* **Unit tests** for isolated logic
* **Integration tests** when interacting with multiple modules
* **Shouldly** is the standard assertion library
* Use **xUnit** as the test framework

When adding features to Builder, Validation, or IO modules, always include:

* Input → expected output tests
* Round-trip serialization tests if applicable

Tests should run via:

```bash
dotnet test
```

---

# Coding Guidelines

Follow clean, modern C# conventions:

* Use **meaningful names**
* Keep classes and methods small
* Follow **SOLID** and **DRY** where it makes sense (don’t over-engineer)
* Avoid introducing new external dependencies without discussion
* All public types must include **XML documentation comments**
* Keep builders fluent and chainable
* Core models should remain **immutable**
* Validation must be deterministic and rule-based

---

# What Makes a Good Pull Request?

A PR is likely to be accepted if it:

* Follows the architecture boundaries (Core, Builder, Validation, IO)
* Includes meaningful tests
* Does not break public APIs without discussion
* Is clear, focused, and easy to review
* Includes documentation updates if adding features

---

# 🐛 Reporting Issues

Please include:

1. A clear description of the problem
2. Steps to reproduce
3. Relevant code snippets
4. Expected vs. actual behavior
5. Version information

Bug reports with failing test cases are especially appreciated.

---

# Documentation Contributions

Improving docs is a valuable contribution:

* API documentation
* Usage examples
* Tutorials
* Architecture illustrations
* Clarifying builder workflows

If in doubt, open a PR — unclear documentation is a common pain point, and improvements are always welcome.

---

# Discussion / Proposals

Before working on a larger feature, please open a **GitHub Discussion** or **Issue** to align on:

* API design
* Architecture fit
* Naming conventions
* Expected behavior

This prevents spending time on something that may not fit long-term goals.

---

# Code of Conduct

Be respectful, constructive, and supportive.
We aim for a welcoming environment for developers of all backgrounds and experience levels.

---

# Ready to Submit?

1. Run all tests
2. Ensure code is formatted (`dotnet format` recommended)
3. Update documentation where needed
4. Push your branch
5. Open a Pull Request with a clear description

Thank you for helping make FluentAAS a better library!
