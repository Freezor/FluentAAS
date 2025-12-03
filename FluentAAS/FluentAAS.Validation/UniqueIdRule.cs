namespace FluentAas.Validation.Rules;

public sealed class UniqueIdRule : IValidationRule
{
    public IEnumerable<ValidationResult> Evaluate(IAasEnvironmentAdapter env)
    {
        var seen = new HashSet<string>();

        foreach (AssetAdministrationShell shell in (IEnumerable<AssetAdministrationShell>) env.Environment.AssetAdministrationShells ?? Enumerable.Empty<AssetAdministrationShell>())
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

        foreach (Submodel submodel in (IEnumerable<Submodel>) env.Environment.Submodels ?? Enumerable.Empty<Submodel>())
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