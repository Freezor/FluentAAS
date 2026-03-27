using FluentAAS.Builder.SubModel;

namespace FluentAAS.Builder;

public interface IAasBuilder
{
    /// <summary>
    /// Declares a new AssetAdministrationShell to add to the environment.
    /// </summary>
    /// <param name="id">Unique identifier of the shell.</param>
    /// <param name="idShort">Short name (IdShort) for the shell.</param>
    /// <param name="kind">Specifies the asset kind; defaults to AssetKind.Instance.</param>
    /// <returns>An IShellBuilder for configuring the newly declared shell.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> or <paramref name="idShort"/> is null, empty, or consists only of whitespace.</exception>
    IShellBuilder AddShell(string id, string idShort, AssetKind kind = AssetKind.Instance);

    /// <summary>
    /// Registers a fully constructed Submodel with the builder so it becomes part of the resulting environment.
    /// </summary>
    /// <param name="submodel">The fully constructed Submodel to register; it will participate in build-time uniqueness validation.</param>
    /// <returns>The same IAasBuilder instance for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when required submodel fields (for example, <c>IReferable.IdShort</c>) are null, empty, or otherwise invalid.</exception>
    IAasBuilder AddSubmodel(Submodel submodel);

    /// <summary>
    /// Adds an existing Submodel instance to the builder's configuration.
    /// </summary>
    /// <param name="submodel">The Submodel instance to add to the environment.</param>
    /// <returns>The current <see cref="IAasBuilder"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    IAasBuilder AddExistingSubmodel(Submodel submodel);

    /// <summary>
    /// Register a staged submodel fragment to be applied to an existing submodel identified by <paramref name="submodelId"/> during Build.
    /// </summary>
    /// <param name="submodelId">The identifier of an already registered submodel to which the fragment will be applied.</param>
    /// <param name="configure">An action that configures the staged fragment using a <see cref="SubmodelFragmentBuilder"/>; fragments are applied in registration order when Build is called.</param>
    /// <returns>The same <see cref="IAasBuilder"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="submodelId"/> is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
    IAasBuilder AddSubmodelFragment(string submodelId, Action<SubmodelFragmentBuilder> configure);

    /// <summary>
    /// Builds and returns the configured <see cref="Environment"/> instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="Environment"/> containing the configured asset administration
    /// shells and submodels.
    /// </returns>
    IEnvironment Build();

    /// <summary>
    /// Adds a fully constructed <see cref="AssetAdministrationShell"/> to the environment.
    /// Intended for internal use by <see cref="ShellBuilder"/>.
    /// </summary>
    /// <param name="shell">The shell instance to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="shell"/> is null.</exception>
    void AddShellInternal(AssetAdministrationShell shell);

    /// <summary>
    /// Adds a fully constructed Submodel to the builder's environment.
    /// </summary>
    /// <param name="submodel">The fully constructed Submodel to add to the environment.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    void AddSubmodelInternal(Submodel submodel);
}