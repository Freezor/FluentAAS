namespace FluentAas.Validation.Rules;

public sealed class UniqueIdRule : IValidationRule
{
    public IEnumerable<ValidationResult> Evaluate(IAasEnvironmentAdapter env)
    {
        var seen = new HashSet<string>();

        foreach (IAssetAdministrationShell shell in (IEnumerable<IAssetAdministrationShell>) env.Environment.AssetAdministrationShells ?? Enumerable.Empty<IAssetAdministrationShell>())
        {
            if (!seen.Add(shell.Id))
            {
                yield return new ValidationResult(
                                                  ValidationLevel.Error,
                                                  "AAS.ID.DUPLICATE",
                                                  $"Duplicate AAS Id '{shell.Id}'.",
                                                  Path: $"AAS[{shell.Id}]"
                                                 );
            }
        }

        foreach (ISubmodel submodel in (IEnumerable<ISubmodel>) env.Environment.Submodels ?? Enumerable.Empty<ISubmodel>())
        {
            if (!seen.Add(submodel.Id))
            {
                yield return new ValidationResult(
                                                  ValidationLevel.Error,
                                                  "SUBMODEL.ID.DUPLICATE",
                                                  $"Duplicate Submodel Id '{submodel.Id}'.",
                                                  Path: $"Submodel[{submodel.Id}]"
                                                 );
            }
        }
    }
}