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
    /// <remarks>Throws <see cref="ArgumentException"/> when <paramref name="id"/> or <paramref name="idShort"/> is null, empty, or whitespace.</remarks>
    IShellBuilder AddShell(string id, string idShort, AssetKind kind = AssetKind.Instance);

    /// <summary>
    /// Adds an existing, fully constructed <see cref="Submodel"/> to the environment.
    /// The submodel participates in build-time uniqueness validation.
    /// </summary>
    /// <param name="submodel">The submodel instance to add.</param>
    /// <returns>The current <see cref="IAasBuilder"/> instance for fluent chaining.</returns>
    /// <remarks>Throws <see cref="ArgumentNullException"/> when <paramref name="submodel"/> is null and <see cref="ArgumentException"/> when required submodel fields are invalid.</remarks>
    IAasBuilder AddSubmodel(Submodel submodel);

    /// <summary>
    /// Adds an existing <see cref="Submodel"/> to the environment.
    /// </summary>
    /// <param name="submodel">The submodel instance to add.</param>
    /// <returns>The current <see cref="AasBuilder"/> instance for fluent chaining.</returns>
    /// <remarks>Throws <see cref="ArgumentNullException"/> when <paramref name="submodel"/> is null.</remarks>
    IAasBuilder AddExistingSubmodel(Submodel submodel);

    /// <summary>
    /// Adds a staged fragment to an already registered submodel, identified by submodel id.
    /// Fragments are applied in registration order during <see cref="Build"/>.
    /// </summary>
    /// <param name="submodelId">The identifier of the target submodel.</param>
    /// <param name="configure">Callback that contributes submodel elements.</param>
    /// <returns>The current <see cref="IAasBuilder"/> instance for fluent chaining.</returns>
    /// <remarks>Throws <see cref="ArgumentException"/> when <paramref name="submodelId"/> is invalid and <see cref="ArgumentNullException"/> when <paramref name="configure"/> is null.</remarks>
    IAasBuilder AddSubmodelFragment(string submodelId, Action<SubmodelFragmentBuilder> configure);

    /// <summary>
    /// Builds and returns the configured <see cref="Environment"/> instance.
    /// </summary>
    /// <returns>A new <see cref="Environment"/> containing the configured asset administration shells and submodels.</returns>
    IEnvironment Build();

    /// <summary>
    /// Adds a fully constructed <see cref="AssetAdministrationShell"/> to the environment.
    /// Intended for internal use by <see cref="ShellBuilder"/>.
    /// </summary>
    /// <param name="shell">The shell instance to add.</param>
    /// <remarks>Throws <see cref="ArgumentNullException"/> when <paramref name="shell"/> is null.</remarks>
    void AddShellInternal(AssetAdministrationShell shell);

    /// <summary>
    /// Adds a fully constructed <see cref="Submodel"/> to the environment.
    /// Intended for internal use by shell or submodel builders.
    /// </summary>
    /// <param name="submodel">The submodel instance to add.</param>
    /// <remarks>Throws <see cref="ArgumentNullException"/> when <paramref name="submodel"/> is null.</remarks>
    void AddSubmodelInternal(Submodel submodel);
}
