using AasCore.Aas3_0;

namespace FluentAas.Builder;

public sealed class AssetAdministrationShellBuilder
{
    private readonly string            _id;
    private          AssetInformation? _assetInformation;
    private readonly List<IReference>   _submodelRefs = new();

    private AssetAdministrationShellBuilder(string id)
    {
        _id = id;
    }

    public static AssetAdministrationShellBuilder Create(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("AAS id must not be empty", nameof(id));

        return new AssetAdministrationShellBuilder(id);
    }

    public AssetAdministrationShellBuilder WithAssetInformation(AssetInformation assetInfo)
    {
        _assetInformation = assetInfo ?? throw new ArgumentNullException(nameof(assetInfo));
        return this;
    }

    public AssetAdministrationShellBuilder AddSubmodelReference(Reference submodelRef)
    {
        _submodelRefs.Add(submodelRef ?? throw new ArgumentNullException(nameof(submodelRef)));
        return this;
    }

    public AssetAdministrationShell Build()
    {
        if (_assetInformation is null)
            throw new InvalidOperationException("AssetInformation must be set for AAS.");

        return new AssetAdministrationShell(
                                            id: _id,
                                            assetInformation: _assetInformation,
                                            submodels: _submodelRefs
                                           );
    }
}