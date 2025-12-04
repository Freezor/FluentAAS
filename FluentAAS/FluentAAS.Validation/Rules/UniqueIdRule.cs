using FluentAAS.Validation.Rules;

namespace FluentAas.Validation.Rules;

/// <summary>
/// Validation rule ensuring that all Asset Administration Shell and Submodel
/// identifiers within an environment are unique across the entire model.
/// </summary>
public sealed class UniqueIdRule : IValidationRule
{
    /// <summary>
    /// Evaluates the environment for duplicate identifiers among Asset Administration Shells
    /// and Submodels. If the same identifier is used more than once, an error is reported.
    /// </summary>
    /// <param name="environment">The environment adapter providing access to the AAS environment.</param>
    /// <returns>
    /// A sequence of <see cref="ValidationResult"/> entries describing any duplicate IDs found.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="environment"/> is <c>null</c>.</exception>
    public IEnumerable<ValidationResult> Evaluate(IEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        var seen = new HashSet<string>(StringComparer.Ordinal);

        // Check Asset Administration Shell IDs
        if (environment.AssetAdministrationShells is not null)
        {
            for (var i = 0; i < environment.AssetAdministrationShells.Count; i++)
            {
                var shell = environment.AssetAdministrationShells[i];
                var id = shell.Id ?? string.Empty;

                if (!seen.Add(id))
                {
                    yield return new ValidationResult(
                        ValidationLevel.Error,
                        "AAS.ID.DUPLICATE",
                        $"Duplicate AssetAdministrationShell Id '{id}'.",
                        Path: $"AAS[{i}]");
                }
            }
        }

        // Check Submodel IDs
        if (environment.Submodels is null) yield break;
        {
            for (var i = 0; i < environment.Submodels.Count; i++)
            {
                var submodel = environment.Submodels[i];
                var id       = submodel.Id ?? string.Empty;

                if (!seen.Add(id))
                {
                    yield return new ValidationResult(
                                                      ValidationLevel.Error,
                                                      "SUBMODEL.ID.DUPLICATE",
                                                      $"Duplicate Submodel Id '{id}'.",
                                                      Path: $"Submodel[{i}]");
                }
            }
        }
    }
}
