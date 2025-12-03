namespace FluentAas.Builder;

public sealed class AasEnvironmentBuilder
{
    private readonly List<IAssetAdministrationShell> _shells    = new();
    private readonly List<ISubmodel>                 _submodels = new();

    public AasEnvironmentBuilder AddShell(AssetAdministrationShell shell)
    {
        _shells.Add(shell ?? throw new ArgumentNullException(nameof(shell)));
        return this;
    }

    public AasEnvironmentBuilder AddSubmodel(Submodel submodel)
    {
        _submodels.Add(submodel ?? throw new ArgumentNullException(nameof(submodel)));
        return this;
    }

    public IAasEnvironmentAdapter Build()
    {
        var env = new Environment(
                                  assetAdministrationShells: _shells,
                                  submodels: _submodels,
                                  conceptDescriptions: new List<IConceptDescription>()
                                 );

        return new DefaultAasEnvironmentAdapter(env);
    }
}