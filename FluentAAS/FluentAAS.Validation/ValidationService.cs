namespace FluentAas.Validation;

public sealed class ValidationService : IValidationService
{
    private readonly IReadOnlyList<IValidationRule> _rules;

    public ValidationService(IEnumerable<IValidationRule> rules)
    {
        _rules = rules?.ToList() ?? throw new ArgumentNullException(nameof(rules));
    }

    public ValidationReport Validate(IAasEnvironmentAdapter environment)
    {
        if (environment is null)
            throw new ArgumentNullException(nameof(environment));

        var results = new List<ValidationResult>();

        foreach (var rule in _rules)
        {
            results.AddRange(rule.Evaluate(environment));
        }

        return new ValidationReport(results);
    }
}