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
    /// <exception cref="ArgumentException">Thrown if the submodel's `Id` or `IdShort` is null, empty, or whitespace.</exception>
    public IAasBuilder AddSubmodel(Submodel submodel)
    {
        ValidateSubmodelForPublicAdd(submodel);
        AddSubmodelInternal(submodel);
        return this;
    }

    /// <summary>
    /// Registers an existing <see cref="Submodel"/> instance with the builder without requiring an <c>IdShort</c> value.
    /// </summary>
    /// <param name="submodel">The submodel to register; its <c>Id</c> must be set (non-empty).</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="submodel"/> has a null, empty, or whitespace <c>Id</c>.</exception>
    public IAasBuilder AddExistingSubmodel(Submodel submodel)
    {
        AddSubmodelInternal(submodel);
        return this;
    }

    /// <summary>
    /// Stages a submodel fragment configuration to be applied to the submodel with the specified id during Build.
    /// </summary>
    /// <param name="submodelId">Identifier of the target submodel; must not be null, empty, or whitespace.</param>
    /// <param name="configure">Callback that receives a <see cref="SubmodelFragmentBuilder"/> to configure the fragment.</param>
    /// <returns>The current <see cref="IAasBuilder"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="submodelId"/> is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
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
    /// Builds an Environment containing the builder's registered shells and submodels after applying any staged submodel fragments and validating build-time invariants.
    /// </summary>
    /// <returns>An Environment with copies of the registered AssetAdministrationShells and Submodels.</returns>
    /// <exception cref="InvalidOperationException">Thrown when one or more duplicate submodel ids are detected among registered Submodel instances.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a staged submodel fragment references a base submodel id that has not been added.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a shell contains a submodel reference without a valid identifier.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a shell references a submodel id that is not present among the registered submodels.</exception>
    public IEnvironment Build()
    {
        var duplicateSubmodelIds = _submodels.OfType<Submodel>()
                                             .GroupBy(s => s.Id, StringComparer.Ordinal)
                                             .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                                             .Select(g => g.Key)
                                             .ToList();

        if (duplicateSubmodelIds.Count > 0)
        {
            var duplicateIdList = string.Join(", ", duplicateSubmodelIds.Select(id => $"'{id}'"));
            throw new InvalidOperationException($"Duplicate submodel id(s) detected at build-time: {duplicateIdList}. Each submodel id must be unique.");
        }

        ApplySubmodelFragmentsTransactionally();

        var knownSubmodelIds = _submodels.OfType<Submodel>()
                                         .Select(s => s.Id)
                                         .Where(id => !string.IsNullOrWhiteSpace(id))
                                         .ToHashSet(StringComparer.Ordinal);

        foreach (var shell in _shells.OfType<AssetAdministrationShell>())
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

        // Return a fresh Environment each time, with copies of the internal lists
        return new Environment
               {
                   AssetAdministrationShells = _shells.ToList(),
                   Submodels                 = _submodels.ToList()
               };
    }

    /// <summary>
    /// Applies all staged submodel fragments as a single transactional batch.
    /// If any fragment fails, all element additions made by this batch are rolled back.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when a staged fragment references an unknown submodel id.</exception>
    private void ApplySubmodelFragmentsTransactionally()
    {
        if (_submodelFragments.Count == 0)
        {
            return;
        }

        var submodelsById = _submodels.OfType<Submodel>()
                                      .Where(sm => !string.IsNullOrWhiteSpace(sm.Id))
                                      .ToDictionary(sm => sm.Id!, StringComparer.Ordinal);

        foreach (var (submodelId, _) in _submodelFragments)
        {
            if (!submodelsById.ContainsKey(submodelId))
            {
                throw new InvalidOperationException($"No base submodel with id '{submodelId}' was found for a staged fragment. Add the submodel before adding fragments.");
            }
        }

        var snapshots = new Dictionary<Submodel, (bool HadCollection, int InitialCount)>();

        try
        {
            foreach (var (submodelId, applyFragment) in _submodelFragments)
            {
                var submodel = submodelsById[submodelId];

                if (!snapshots.ContainsKey(submodel))
                {
                    snapshots[submodel] = (submodel.SubmodelElements is not null, submodel.SubmodelElements?.Count ?? 0);
                }

                applyFragment(submodel);
            }

            _submodelFragments.Clear();
        }
        catch
        {
            foreach (var (submodel, snapshot) in snapshots)
            {
                if (!snapshot.HadCollection)
                {
                    submodel.SubmodelElements = null;
                    continue;
                }

                if (submodel.SubmodelElements is null)
                {
                    submodel.SubmodelElements = [];
                    continue;
                }

                while (submodel.SubmodelElements.Count > snapshot.InitialCount)
                {
                    submodel.SubmodelElements.RemoveAt(submodel.SubmodelElements.Count - 1);
                }
            }

            throw;
        }
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
    /// Registers the given submodel in the builder's internal collection if the same instance is not already present.
    /// </summary>
    /// <param name="submodel">The submodel to register; must not be null and must have a non-empty Id.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    public void AddSubmodelInternal(Submodel submodel)
    {
        ArgumentNullException.ThrowIfNull(submodel);
        ValidateSubmodelId(submodel);

        if (!_submodels.Contains(submodel))
        {
            _submodels.Add(submodel);
        }
    }

    /// <summary>
    /// Validates that the provided <paramref name="submodel"/> is not null and has a non-empty Id.
    /// </summary>
    /// <param name="submodel">The submodel to validate; its <c>Id</c> must not be null, empty, or whitespace.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="submodel"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="submodel"/>'s <c>Id</c> is null, empty, or consists only of whitespace.</exception>
    private static void ValidateSubmodelId(Submodel submodel)
    {
        ArgumentNullException.ThrowIfNull(submodel);

        if (string.IsNullOrWhiteSpace(submodel.Id))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(submodel));
        }
    }

    /// <summary>
    /// Validates that a submodel has a non-empty identifier and a non-empty IdShort.
    /// </summary>
    /// <param name="submodel">The submodel to validate; its identifier and IdShort must be set.</param>
    /// <exception cref="ArgumentException">Thrown when <c>submodel.IdShort</c> is null, empty, or whitespace.</exception>
    private static void ValidateSubmodelForPublicAdd(Submodel submodel)
    {
        ValidateSubmodelId(submodel);

        if (string.IsNullOrWhiteSpace(submodel.IdShort))
        {
            throw new ArgumentException("Submodel idShort must not be empty.", nameof(submodel));
        }
    }
}
