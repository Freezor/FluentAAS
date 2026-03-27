using FluentAAS.Builder.SubModel;

namespace FluentAAS.Builder;

/// <summary>
/// Provides a fluent API for constructing an AAS <see cref="Environment"/> 
/// with asset administration shells and submodels.
/// </summary>
public sealed class AasBuilder : IAasBuilder
{
    private readonly List<IAssetAdministrationShell>                _shells            = [];
    private readonly List<ISubmodel>                                _submodels         = [];
    private readonly List<(string SubmodelId, Action<Submodel> Fn)> _submodelFragments = [];

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
    /// <remarks>Throws <see cref="ArgumentException"/> when <paramref name="id"/> or <paramref name="idShort"/> is null, empty, or whitespace.</remarks>
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
    /// Adds an existing, fully constructed <see cref="Submodel"/> to the environment.
    /// The submodel participates in build-time uniqueness validation.
    /// </summary>
    /// <param name="submodel">The submodel instance to add.</param>
    /// <returns>The current <see cref="IAasBuilder"/> instance for fluent chaining.</returns>
    /// <remarks>Throws <see cref="ArgumentNullException"/> when <paramref name="submodel"/> is null and <see cref="ArgumentException"/> when required submodel fields are invalid.</remarks>
    public IAasBuilder AddSubmodel(Submodel submodel)
    {
        ValidateSubmodelForPublicAdd(submodel);
        AddSubmodelInternal(submodel);
        return this;
    }

    /// <summary>
    /// Adds an existing <see cref="Submodel"/> to the environment.
    /// </summary>
    /// <param name="submodel">The submodel instance to add.</param>
    /// <returns>The current <see cref="AasBuilder"/> instance for fluent chaining.</returns>
    /// <remarks>Throws <see cref="ArgumentNullException"/> when <paramref name="submodel"/> is null.</remarks>
    public IAasBuilder AddExistingSubmodel(Submodel submodel)
    {
        AddSubmodelInternal(submodel);
        return this;
    }

    /// <summary>
    /// Adds a staged fragment to an already registered submodel, identified by submodel id.
    /// Fragments are applied in registration order during <see cref="Build"/>.
    /// </summary>
    /// <param name="submodelId">The identifier of the target submodel.</param>
    /// <param name="configure">Callback that contributes submodel elements.</param>
    /// <returns>The current <see cref="IAasBuilder"/> instance for fluent chaining.</returns>
    /// <remarks>Throws <see cref="ArgumentException"/> when <paramref name="submodelId"/> is invalid and <see cref="ArgumentNullException"/> when <paramref name="configure"/> is null.</remarks>
    public IAasBuilder AddSubmodelFragment(string submodelId, Action<SubmodelFragmentBuilder> configure)
    {
        if (string.IsNullOrWhiteSpace(submodelId))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(submodelId));
        }

        ArgumentNullException.ThrowIfNull(configure);

        _submodelFragments.Add((submodelId, submodel =>
        {
            var fragment = new SubmodelFragmentBuilder(submodel);
            configure(fragment);
        }));

        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="Environment"/> instance.
    /// </summary>
    /// <returns>A new <see cref="Environment"/> containing the configured asset administration shells and submodels.</returns>
    public IEnvironment Build()
    {
        var workingSubmodels = CloneSubmodels(_submodels);
        var workingShells    = CloneShells(_shells);

        var duplicateSubmodelIds = workingSubmodels.OfType<Submodel>()
                                             .GroupBy(s => s.Id, StringComparer.Ordinal)
                                             .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                                             .Select(g => g.Key)
                                             .ToList();

        if (duplicateSubmodelIds.Count > 0)
        {
            var duplicateIdList = string.Join(", ", duplicateSubmodelIds.Select(id => $"'{id}'"));
            throw new InvalidOperationException($"Duplicate submodel id(s) detected at build-time: {duplicateIdList}. Each submodel id must be unique.");
        }

        var submodelsById = workingSubmodels.OfType<Submodel>()
                                            .ToDictionary(sm => sm.Id, StringComparer.Ordinal);

        foreach (var (submodelId, _) in _submodelFragments)
        {
            if (!submodelsById.ContainsKey(submodelId))
            {
                throw new InvalidOperationException($"No base submodel with id '{submodelId}' was found for a staged fragment. Add the submodel before adding fragments.");
            }
        }

        foreach (var (submodelId, applyFragment) in _submodelFragments)
        {
            applyFragment(submodelsById[submodelId]);
        }

        var knownSubmodelIds = workingSubmodels.OfType<Submodel>()
                                               .Select(s => s.Id)
                                               .Where(id => !string.IsNullOrWhiteSpace(id))
                                               .ToHashSet(StringComparer.Ordinal);

        foreach (var shell in workingShells.OfType<AssetAdministrationShell>())
        {
            foreach (var reference in shell.Submodels ?? [])
            {
                var referencedSubmodelId = reference.Keys.FirstOrDefault()?.Value;
                if (string.IsNullOrWhiteSpace(referencedSubmodelId))
                {
                    throw new InvalidOperationException($"Shell '{shell.Id}' contains a submodel reference without a valid identifier.");
                }

                if (!knownSubmodelIds.Contains(referencedSubmodelId))
                {
                    throw new InvalidOperationException($"Shell '{shell.Id}' references unknown submodel id '{referencedSubmodelId}'.");
                }
            }
        }

        _submodels.Clear();
        _submodels.AddRange(workingSubmodels);
        _shells.Clear();
        _shells.AddRange(workingShells);
        _submodelFragments.Clear();

        // Return a fresh Environment each time, with copies of the internal lists
        return new Environment
               {
                   AssetAdministrationShells = CloneShells(_shells),
                   Submodels                 = CloneSubmodels(_submodels)
               };
    }

    /// <summary>
    /// Adds a fully constructed <see cref="AssetAdministrationShell"/> to the environment.
    /// Intended for internal use by <see cref="ShellBuilder"/>.
    /// </summary>
    /// <param name="shell">The shell instance to add.</param>
    /// <remarks>Throws <see cref="ArgumentNullException"/> when <paramref name="shell"/> is null.</remarks>
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
    /// <remarks>Throws <see cref="ArgumentNullException"/> when <paramref name="submodel"/> is null.</remarks>
    public void AddSubmodelInternal(Submodel submodel)
    {
        ArgumentNullException.ThrowIfNull(submodel);
        ValidateSubmodelId(submodel);

        if (!_submodels.Contains(submodel))
        {
            _submodels.Add(submodel);
        }
    }

    private static void ValidateSubmodelId(Submodel submodel)
    {
        ArgumentNullException.ThrowIfNull(submodel);

        if (string.IsNullOrWhiteSpace(submodel.Id))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(submodel));
        }
    }

    private static void ValidateSubmodelForPublicAdd(Submodel submodel)
    {
        ValidateSubmodelId(submodel);

        if (string.IsNullOrWhiteSpace(submodel.IdShort))
        {
            throw new ArgumentException("Submodel idShort must not be empty.", nameof(submodel));
        }
    }

    private static List<ISubmodel> CloneSubmodels(IEnumerable<ISubmodel> submodels)
    {
        return submodels.Select(submodel => submodel is Submodel concreteSubmodel
                                                ? (ISubmodel)CloneSubmodel(concreteSubmodel)
                                                : submodel)
                        .ToList();
    }

    private static Submodel CloneSubmodel(Submodel source)
    {
        return new Submodel(id: source.Id)
               {
                   IdShort          = source.IdShort,
                   SubmodelElements = source.SubmodelElements?.ToList()
               };
    }

    private static List<IAssetAdministrationShell> CloneShells(IEnumerable<IAssetAdministrationShell> shells)
    {
        return shells.Select(shell => shell is AssetAdministrationShell concreteShell
                                          ? (IAssetAdministrationShell)CloneShell(concreteShell)
                                          : shell)
                     .ToList();
    }

    private static AssetAdministrationShell CloneShell(AssetAdministrationShell source)
    {
        return new AssetAdministrationShell(id: source.Id, assetInformation: source.AssetInformation)
               {
                   IdShort   = source.IdShort,
                   Submodels = source.Submodels?.Select(CloneReference).ToList()
               };
    }

    private static Reference CloneReference(Reference source)
    {
        return new Reference(
                             source.Type,
                             source.Keys.Select(key => new Key(key.Type, key.Value)).ToList());
    }
}
