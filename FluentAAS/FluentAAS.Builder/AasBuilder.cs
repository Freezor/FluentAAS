namespace FluentAAS.Builder;

/// <summary>
/// Provides a fluent API for constructing an AAS <see cref="Environment"/> 
/// with asset administration shells and submodels.
/// </summary>
public sealed class AasBuilder : IAasBuilder
{
    private readonly List<IAssetAdministrationShell> _shells    = [];
    private readonly List<ISubmodel>                 _submodels = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="AasBuilder"/> class.
    /// Use <see cref="Create"/> or <see cref="AasFluent.CreateEnvironment"/> to get an instance.
    /// </summary>
    internal AasBuilder()
    {
    }

    /// <summary>
    /// Creates a new <see cref="AasBuilder"/> instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="AasBuilder"/> that can be used to configure an AAS environment.
    /// </returns>
    public static AasBuilder Create() => new();

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
    /// </exception>
    public IShellBuilder AddShell(string id, string idShort, AssetKind kind = AssetKind.Instance)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("AAS id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(idShort))
        {
            throw new ArgumentException("AAS idShort must not be empty.", nameof(idShort));
        }

        var shell = new AssetAdministrationShell(
                                                 id: id,
                                                 assetInformation: new AssetInformation(kind))
                    {
                        IdShort   = idShort,
                        Submodels = []
                    };

        AddShellInternal(shell);

        return new ShellBuilder(this, shell);
    }

    /// <summary>
    /// Adds an existing <see cref="Submodel"/> to the environment.
    /// </summary>
    /// <param name="submodel">The submodel instance to add.</param>
    /// <returns>The current <see cref="AasBuilder"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    public AasBuilder AddExistingSubmodel(Submodel submodel)
    {
        AddSubmodelInternal(submodel);
        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="Environment"/> instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="Environment"/> containing the configured asset administration
    /// shells and submodels.
    /// </returns>
    public IEnvironment Build()
    {
        // Return a fresh Environment each time, with copies of the internal lists
        return new Environment
               {
                   AssetAdministrationShells = _shells.ToList(),
                   Submodels                 = _submodels.ToList()
               };
    }

    /// <summary>
    /// Adds a fully constructed <see cref="AssetAdministrationShell"/> to the environment.
    /// Intended for internal use by <see cref="ShellBuilder"/>.
    /// </summary>
    /// <param name="shell">The shell instance to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="shell"/> is null.</exception>
    public void AddShellInternal(AssetAdministrationShell shell)
    {
        ArgumentNullException.ThrowIfNull(shell);

        if (!_shells.Contains(shell))
        {
            _shells.Add(shell);
        }
    }

    /// <summary>
    /// Adds a fully constructed <see cref="Submodel"/> to the environment.
    /// Intended for internal use by shell or submodel builders.
    /// </summary>
    /// <param name="submodel">The submodel instance to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    public void AddSubmodelInternal(Submodel submodel)
    {
        ArgumentNullException.ThrowIfNull(submodel);

        if (!_submodels.Contains(submodel))
        {
            _submodels.Add(submodel);
        }
    }
}