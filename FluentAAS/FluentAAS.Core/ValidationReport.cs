namespace FluentAas.Core;

public sealed class ValidationReport
{
    private readonly IReadOnlyList<ValidationResult> _results;

    public IReadOnlyList<ValidationResult> Results => _results;

    public bool HasErrors => _results.Any(r => r.Level == ValidationLevel.Error);

    public ValidationReport(IEnumerable<ValidationResult> results)
    {
        _results = results?.ToList() ?? throw new ArgumentNullException(nameof(results));
    }
}