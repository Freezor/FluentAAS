namespace FluentAAS.Validation.Rules;

/// <summary>
/// Defines a validation rule that can be executed against an AAS environment.
/// Implementations analyze an <see cref="IEnvironment"/> and return
/// one or more <see cref="ValidationResult"/> instances describing any issues found.
/// </summary>
public interface IValidationRule
{
    /// <summary>
    /// Evaluates the AAS environment and returns all validation results produced by this rule.
    /// </summary>
    /// <param name="env">
    /// The AAS environment adapter providing access to the AAS <see cref="Environment"/>
    /// to be validated.
    /// </param>
    /// <returns>
    /// A sequence of <see cref="ValidationResult"/> entries representing warnings,
    /// informational messages, or errors discovered during evaluation.
    /// </returns>
    IEnumerable<ValidationResult> Evaluate(IEnvironment env);
}