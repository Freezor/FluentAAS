using System.Collections;
namespace FluentAas.Validation.Rules;

public sealed class NonEmptyIdRule : IValidationRule
{
    public IEnumerable<ValidationResult> Evaluate(IAasEnvironmentAdapter env)
    {
        foreach (AssetAdministrationShell shell in (IEnumerable) env.Environment.AssetAdministrationShells ?? Enumerable.Empty<AssetAdministrationShell>())
        {
            if (string.IsNullOrWhiteSpace(shell.Id))
            {
                yield return new ValidationResult(
                                                  ValidationLevel.Error,
                                                  "AAS.ID.EMPTY",
                                                  "AssetAdministrationShell.Id must not be empty.",
                                                  Path: $"AAS[{shell.Id}]"
                                                 );
            }
        }

        foreach (Submodel submodel in (IEnumerable) env.Environment.Submodels ?? Enumerable.Empty<Submodel>())
        {
            if (string.IsNullOrWhiteSpace(submodel.Id))
            {
                yield return new ValidationResult(
                                                  ValidationLevel.Error,
                                                  "SUBMODEL.ID.EMPTY",
                                                  "Submodel.Id must not be empty.",
                                                  Path: $"Submodel[{submodel.Id}]"
                                                 );
            }
        }
    }
}