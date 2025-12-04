namespace FluentAAS.Builder;

/// <summary>
///     Provides a fluent API to configure an <see cref="AssetAdministrationShell" />
///     within an <see cref="Environment" />.
/// </summary>
public sealed class ShellBuilder
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ShellBuilder" /> class.
    /// </summary>
    /// <param name="parent">The parent <see cref="AasBuilder" />.</param>
    /// <param name="shell">The <see cref="AssetAdministrationShell" /> being configured.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="parent" /> or <paramref name="shell" /> is <c>null</c>.
    /// </exception>
    internal ShellBuilder(AasBuilder parent, AssetAdministrationShell shell)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Shell  = shell ?? throw new ArgumentNullException(nameof(shell));
    }

    private AssetAdministrationShell Shell { get; }

    private AasBuilder Parent { get; }

    /// <summary>
    ///     Sets the global asset identifier of the underlying asset.
    /// </summary>
    /// <param name="globalAssetId">The global asset identifier to assign.</param>
    /// <returns>The current <see cref="ShellBuilder" /> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="globalAssetId" /> is null, empty, or whitespace.
    /// </exception>
    public ShellBuilder WithGlobalAssetId(string globalAssetId)
    {
        if (string.IsNullOrWhiteSpace(globalAssetId)) throw new ArgumentException("Global asset id must not be empty.", nameof(globalAssetId));

        if (Shell.AssetInformation is null) throw new InvalidOperationException("AssetInformation must be initialized before setting the global asset id.");

        Shell.AssetInformation.GlobalAssetId = globalAssetId;
        return this;
    }

    /// <summary>
    ///     Adds a specific asset identifier to the underlying asset information.
    /// </summary>
    /// <param name="key">The key or name of the specific identifier.</param>
    /// <param name="value">The value of the specific identifier.</param>
    /// <param name="nameSpace">The namespace of the specific identifier.</param>
    /// <returns>The current <see cref="ShellBuilder" /> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="key" />, <paramref name="value" />, or <paramref name="nameSpace" /> is null, empty, or whitespace.
    /// </exception>
    public ShellBuilder WithSpecificAssetId(string key, string value, string nameSpace)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Specific asset id key must not be empty.", nameof(key));

        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Specific asset id value must not be empty.", nameof(value));

        if (string.IsNullOrWhiteSpace(nameSpace)) throw new ArgumentException("Specific asset id namespace must not be empty.", nameof(nameSpace));

        if (Shell.AssetInformation is null) throw new InvalidOperationException("AssetInformation must be initialized before adding specific asset ids.");

        Shell.AssetInformation.SpecificAssetIds ??= new List<ISpecificAssetId>();

        // Use constructor parameters instead of a non-existent ExternalSubjectId property.
        // NameSpace is the correct property name for the meta-model field "nameSpace".
        var externalSubjectRef = new Reference(
                                               ReferenceTypes.ExternalReference,
                                               []);

        Shell.AssetInformation.SpecificAssetIds.Add(
                                                    new SpecificAssetId(
                                                                        key,
                                                                        value,
                                                                        externalSubjectId: externalSubjectRef));

        return this;
    }

    /// <summary>
    ///     Adds a reference to an existing <see cref="Submodel" /> to the shell.
    /// </summary>
    /// <param name="submodel">The submodel to reference.</param>
    /// <param name="idType">
    ///     The <see cref="KeyTypes" /> used for the submodel reference. Defaults to <see cref="KeyTypes.Submodel" />.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel" /> is <c>null</c>.</exception>
    public void AddSubmodelReference(Submodel submodel, KeyTypes idType = KeyTypes.Submodel)
    {
        if (submodel is null) throw new ArgumentNullException(nameof(submodel));

        Shell.Submodels ??= [];

        Shell.Submodels.Add(
                            new Reference(
                                          ReferenceTypes.ModelReference,
                                          [
                                              new Key(
                                                      idType,
                                                      submodel.Id)
                                          ]));

        Parent.AddSubmodelInternal(submodel);
    }

    /// <summary>
    ///     Creates and associates a new <see cref="Submodel" /> via a <see cref="SubmodelBuilderWithShell" />.
    /// </summary>
    /// <param name="id">The identifier of the new submodel.</param>
    /// <param name="idShort">The short identifier of the new submodel.</param>
    /// <returns>
    ///     A <see cref="SubmodelBuilderWithShell" /> to further configure the submodel
    ///     while remaining attached to this shell and environment.
    /// </returns>
    public SubmodelBuilderWithShell AddSubmodel(string id, string idShort)
    {
        return new SubmodelBuilderWithShell(this, id, idShort);
    }

    /// <summary>
    ///     Completes configuration of this shell and returns to the parent <see cref="AasBuilder" />.
    /// </summary>
    /// <returns>The parent <see cref="AasBuilder" />.</returns>
    public AasBuilder Done()
    {
        Parent.AddShellInternal(Shell);
        return Parent;
    }
}