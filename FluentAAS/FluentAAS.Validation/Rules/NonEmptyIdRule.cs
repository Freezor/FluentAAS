using FluentAAS.Validation.Rules;

namespace FluentAas.Validation.Rules;

/// <summary>
/// Validation rule ensuring that all Asset Administration Shells and Submodels
/// within the environment have non-empty identifiers.
/// </summary>
public sealed class NonEmptyIdRule : IValidationRule
{
    /// <summary>
    /// Evaluates all shells and submodels in the AAS environment, producing errors
    /// when an element contains an empty or whitespace identifier.
    /// </summary>
    /// <param name="env">The environment adapter providing access to the AAS environment.</param>
    /// <returns>A sequence of <see cref="ValidationResult"/> objects.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="env"/> is null.</exception>
    public IEnumerable<ValidationResult> Evaluate(IAasEnvironmentAdapter env)
    {
        ArgumentNullException.ThrowIfNull(env);

        var environment = env.Environment;

        // Validate Asset Administration Shells
        if (environment.AssetAdministrationShells is not null)
        {
            for (int i = 0; i < environment.AssetAdministrationShells.Count; i++)
            {
                var shell = environment.AssetAdministrationShells[i];

                if (string.IsNullOrWhiteSpace(shell.Id))
                {
                    yield return new ValidationResult(
                                                      ValidationLevel.Error,
                                                      "AAS.ID.EMPTY",
                                                      "AssetAdministrationShell.Id must not be empty.",
                                                      Path: $"AAS[{i}]");
                }
            }
        }

        // Validate Submodels
        if (environment.Submodels is null) yield break;
        {
            for (var i = 0; i < environment.Submodels.Count; i++)
            {
                var submodel = environment.Submodels[i];

                if (string.IsNullOrWhiteSpace(submodel.Id))
                {
                    yield return new ValidationResult(
                                                      ValidationLevel.Error,
                                                      "SUBMODEL.ID.EMPTY",
                                                      "Submodel.Id must not be empty.",
                                                      Path: $"Submodel[{i}]");
                }
            }
        }
    }
}