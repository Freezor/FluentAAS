namespace FluentAAS.Builder;

/// <summary>
/// Provides a fluent builder for constructing an <see cref="AssetAdministrationShell"/> instance.
/// </summary>
public sealed class AssetAdministrationShellBuilder
{
    private readonly string            _id;
    private          AssetInformation? _assetInformation;
    private readonly List<IReference>  _submodelReferences = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetAdministrationShellBuilder"/> class.
    /// </summary>
    /// <param name="id">The identifier of the Asset Administration Shell.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    private AssetAdministrationShellBuilder(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("AAS id must not be empty.", nameof(id));

        _id = id;
    }

    /// <summary>
    /// Creates a new <see cref="AssetAdministrationShellBuilder"/> for the provided AAS identifier.
    /// </summary>
    /// <param name="id">The identifier of the Asset Administration Shell.</param>
    /// <returns>A configured <see cref="AssetAdministrationShellBuilder"/>.</returns>
    public static AssetAdministrationShellBuilder Create(string id) =>
        new AssetAdministrationShellBuilder(id);

    /// <summary>
    /// Adds the <see cref="AssetInformation"/> metadata to the AAS being built.
    /// </summary>
    /// <param name="assetInfo">The asset information to assign.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assetInfo"/> is null.</exception>
    public AssetAdministrationShellBuilder WithAssetInformation(AssetInformation assetInfo)
    {
        _assetInformation = assetInfo ?? throw new ArgumentNullException(nameof(assetInfo));
        return this;
    }

    /// <summary>
    /// Adds a reference to a submodel associated with this AAS.
    /// </summary>
    /// <param name="submodelReference">The submodel reference to add.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodelReference"/> is null.</exception>
    public AssetAdministrationShellBuilder AddSubmodelReference(IReference submodelReference)
    {
        ArgumentNullException.ThrowIfNull(submodelReference);
        _submodelReferences.Add(submodelReference);
        return this;
    }

    /// <summary>
    /// Builds and returns the fully configured <see cref="AssetAdministrationShell"/> instance.
    /// </summary>
    /// <returns>A constructed <see cref="AssetAdministrationShell"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required configuration (e.g., <see cref="AssetInformation"/>) is missing.
    /// </exception>
    public AssetAdministrationShell Build()
    {
        if (_assetInformation is null)
        {
            throw new InvalidOperationException("AssetInformation must be set before building the AAS.");
        }

        return new AssetAdministrationShell(
                                            id: _id,
                                            assetInformation: _assetInformation,
                                            submodels: _submodelReferences
                                           );
    }
}