namespace FluentAas.Core;

/// <summary>
/// Represents the outcome of a validation process, containing a collection of
/// individual <see cref="ValidationResult"/> entries and high-level convenience
/// properties such as <see cref="HasErrors"/>.
/// </summary>
public sealed class ValidationReport
{
    /// <summary>
    /// Gets the list of validation results included in this report.
    /// </summary>
    private IReadOnlyList<ValidationResult> Results { get; }

    /// <summary>
    /// Indicates whether the validation report contains at least one error-level result.
    /// </summary>
    public bool HasErrors => Results.Any(r => r.Level == ValidationLevel.Error);

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationReport"/> class.
    /// </summary>
    /// <param name="results">The collection of validation results.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="results"/> is <c>null</c>.
    /// </exception>
    public ValidationReport(IEnumerable<ValidationResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        Results = results.ToList().AsReadOnly();
    }
}