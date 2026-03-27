using FluentAAS.Builder.SubModel;

namespace FluentAAS.Builder;

public interface IAasBuilder
{
    /// <summary>
    /// Adds a new <see cref="AssetAdministrationShell"/> definition to the environment
    /// and returns a <see cref="ShellBuilder"/> to configure it further.
    /// </summary>
    /// <param name="id">The globally unique identifier of the Asset Administration Shell.</param>
    /// <param name="idShort">A short, human-readable identifier for the shell.</param>
    /// <param name="kind">
    /// The <see cref="AssetKind"/> describing whether the asset is an instance or a type.
    /// Defaults to <see cref="AssetKind.Instance"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ShellBuilder"/> instance for further configuration of the shell.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> or <paramref name="idShort"/> is null, empty, or whitespace.
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
    /// Adds an existing, fully constructed <see cref="Submodel"/> to the environment.
    /// The submodel participates in build-time uniqueness validation.
    /// </summary>
    /// <param name="submodel">The submodel instance to add.</param>
    /// <returns>The current <see cref="IAasBuilder"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when required submodel fields (for example <see cref="IReferable.IdShort"/>) are invalid.
    /// <summary>
/// Registers a fully constructed Submodel with the builder so it becomes part of the resulting environment.
/// </summary>
/// <param name="submodel">The fully constructed Submodel to register; it will participate in build-time uniqueness validation.</param>
/// <returns>The same IAasBuilder instance for fluent chaining.</returns>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
/// <exception cref="ArgumentException">Thrown when required submodel fields (for example, <c>IReferable.IdShort</c>) are null, empty, or otherwise invalid.</exception>
    IAasBuilder AddSubmodel(Submodel submodel);

    /// <summary>
    /// Adds an existing <see cref="Submodel"/> to the environment.
    /// </summary>
    /// <param name="submodel">The submodel instance to add.</param>
    /// <returns>The current <see cref="AasBuilder"/> instance for fluent chaining.</returns>
    /// <summary>
/// Adds an existing Submodel instance to the builder's configuration.
/// </summary>
/// <param name="submodel">The Submodel instance to add to the environment.</param>
/// <returns>The current <see cref="IAasBuilder"/> instance for fluent chaining.</returns>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    IAasBuilder AddExistingSubmodel(Submodel submodel);

    /// <summary>
    /// Adds a staged fragment to an already registered submodel, identified by submodel id.
    /// Fragments are applied in registration order during <see cref="Build"/>.
    /// </summary>
    /// <param name="submodelId">The identifier of the target submodel.</param>
    /// <param name="configure">Callback that contributes submodel elements.</param>
    /// <returns>The current <see cref="IAasBuilder"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="submodelId"/> is null, empty, or whitespace.</exception>
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
    /// Adds a fully constructed <see cref="Submodel"/> to the environment.
    /// Intended for internal use by shell or submodel builders.
    /// </summary>
    /// <param name="submodel">The submodel instance to add.</param>
    /// <summary>
/// Adds a fully constructed Submodel to the builder's environment.
/// </summary>
/// <param name="submodel">The fully constructed Submodel to add to the environment.</param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    void AddSubmodelInternal(Submodel submodel);
}
