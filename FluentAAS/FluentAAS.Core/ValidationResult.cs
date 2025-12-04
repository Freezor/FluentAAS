namespace FluentAas.Core;

/// <summary>
/// Represents a single validation outcome, including severity, code, message,
/// and an optional path indicating where the issue occurred within the AAS model.
/// </summary>
/// <param name="Level">The severity level of the validation result.</param>
/// <param name="Code">A machine-readable code that identifies the type of validation issue.</param>
/// <param name="Message">A human-readable description of the validation issue.</param>
/// <param name="Path">
/// An optional JSON-like or structural path pointing to the location of the issue within the model.
/// </param>
public sealed record ValidationResult(
    ValidationLevel Level,
    string          Code,
    string          Message,
    string?         Path = null
);