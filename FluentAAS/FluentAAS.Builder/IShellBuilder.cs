using FluentAAS.Builder.SubModel;

namespace FluentAAS.Builder;

public interface IShellBuilder
{
    AssetAdministrationShell   Shell  { get; }
    AasBuilder Parent { get; }

    /// <summary>
    ///     Sets the global asset identifier of the underlying asset.
    /// </summary>
    /// <param name="globalAssetId">The global asset identifier to assign.</param>
    /// <returns>The current <see cref="ShellBuilder" /> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="globalAssetId" /> is null, empty, or whitespace.
    /// </exception>
    ShellBuilder WithGlobalAssetId(string globalAssetId);

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
    ShellBuilder WithSpecificAssetId(string key, string value, string nameSpace);

    /// <summary>
    ///     Adds a reference to an existing <see cref="Submodel" /> to the shell.
    /// </summary>
    /// <param name="submodel">The submodel to reference.</param>
    /// <param name="idType">
    ///     The <see cref="KeyTypes" /> used for the submodel reference. Defaults to <see cref="KeyTypes.Submodel" />.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel" /> is <c>null</c>.</exception>
    void AddSubmodelReference(Submodel submodel, KeyTypes idType = KeyTypes.Submodel);

    /// <summary>
    ///     Creates and associates a new <see cref="Submodel" /> via a <see cref="SubmodelBuilderWithShell" />.
    /// </summary>
    /// <param name="id">The identifier of the new submodel.</param>
    /// <param name="idShort">The short identifier of the new submodel.</param>
    /// <returns>
    ///     A <see cref="SubmodelBuilderWithShell" /> to further configure the submodel
    ///     while remaining attached to this shell and environment.
    /// </returns>
    SubmodelBuilderWithShell AddSubmodel(string id, string idShort);

    /// <summary>
    ///     Completes configuration of this shell and returns to the parent <see cref="AasBuilder" />.
    /// </summary>
    /// <returns>The parent <see cref="AasBuilder" />.</returns>
    AasBuilder Done();
}