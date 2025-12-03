namespace FluentAas.Core;

public enum ValidationLevel
{
    Info,
    Warning,
    Error
}

public sealed record ValidationResult(
    ValidationLevel Level,
    string          Code,
    string          Message,
    string?         Path = null
);