using FluentAAS.Validation.Rules;

namespace FluentAas.Validation;

/// <summary>
/// Represents a service capable of validating an AAS environment by applying
/// one or more <see cref="IValidationRule"/> implementations.
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validates the specified AAS environment and returns a <see cref="ValidationReport"/>
    /// containing the combined results of all validation rules.
    /// </summary>
    /// <param name="environment">
    /// The AAS environment adapter providing access to the AAS <see cref="Environment"/> to validate.
    /// </param>
    /// <returns>
    /// A <see cref="ValidationReport"/> summarizing all validation outcomes.
    /// </returns>
    ValidationReport Validate(IEnvironment environment);
}