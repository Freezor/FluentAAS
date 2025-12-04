using FluentAAS.Validation.Rules;

namespace FluentAas.Validation;

/// <summary>
/// Default implementation of <see cref="IValidationService"/> that executes a
/// collection of <see cref="IValidationRule"/> instances against an AAS environment.
/// </summary>
public sealed class ValidationService : IValidationService
{
    private readonly IReadOnlyList<IValidationRule> _rules;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationService"/> class.
    /// </summary>
    /// <param name="rules">The set of validation rules to apply.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="rules"/> is <c>null</c>.
    /// </exception>
    public ValidationService(IEnumerable<IValidationRule> rules)
    {
        _rules = rules?.ToList() ?? throw new ArgumentNullException(nameof(rules));
    }

    /// <summary>
    /// Validates the specified AAS environment using all configured validation rules.
    /// </summary>
    /// <param name="environment">The AAS environment to validate.</param>
    /// <returns>
    /// A <see cref="ValidationReport"/> containing all validation results produced
    /// by the applied rules.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="environment"/> is <c>null</c>.
    /// </exception>
    public ValidationReport Validate(IEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        var results = new List<ValidationResult>();

        foreach (var rule in _rules)
        {
            results.AddRange(rule.Evaluate(environment));
        }

        return new ValidationReport(results);
    }
}