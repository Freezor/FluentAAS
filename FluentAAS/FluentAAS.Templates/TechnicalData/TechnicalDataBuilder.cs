using FluentAAS.Builder;
using FluentAAS.Builder.SubModel;

namespace FluentAAS.Templates.TechnicalData;

/// <summary>
/// Fluent builder for composing an IDTA Technical Data submodel with typed groups,
/// ECLASS semantic mapping and IEC 61360-oriented validation.
/// The builder fails fast so invalid technical parameter sets are rejected during composition.
/// </summary>
public sealed class TechnicalDataBuilder
{
    private readonly IShellBuilder _shellBuilder;
    private string _id;
    private string _idShort;

    private readonly Dictionary<string, GroupAssignment> _groups = new(StringComparer.Ordinal);

    internal TechnicalDataBuilder(IShellBuilder shellBuilder, string id, string idShort)
    {
        _shellBuilder = shellBuilder ?? throw new ArgumentNullException(nameof(shellBuilder));

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(idShort))
        {
            throw new ArgumentException("Submodel idShort must not be empty.", nameof(idShort));
        }

        _id = id;
        _idShort = idShort;
    }

    /// <summary>
    /// Overrides the submodel identifiers used for the generated Technical Data submodel.
    /// This allows callers to align the template with project-specific naming and ID strategies.
    /// </summary>
    /// <param name="id">Globally unique submodel identifier.</param>
    /// <param name="idShort">Human-readable short identifier.</param>
    /// <returns>The current builder for fluent chaining.</returns>
    public TechnicalDataBuilder WithIds(string id, string idShort)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(idShort))
        {
            throw new ArgumentException("Submodel idShort must not be empty.", nameof(idShort));
        }

        _id = id;
        _idShort = idShort;
        return this;
    }

    /// <summary>
    /// Configures the strongly typed motor performance group.
    /// Using this method guarantees that only motor-performance properties are assigned in this group.
    /// </summary>
    /// <param name="configure">Callback that fills the motor performance properties.</param>
    /// <returns>The current builder for fluent chaining.</returns>
    public TechnicalDataBuilder WithMotorPerformance(Action<MotorPerformanceGroupBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var groupBuilder = new MotorPerformanceGroupBuilder(this);
        configure(groupBuilder);

        return this;
    }

    /// <summary>
    /// Configures the strongly typed bearing characteristics group.
    /// This prevents accidental mixing of unrelated technical parameters.
    /// </summary>
    /// <param name="configure">Callback that fills the bearing characteristic properties.</param>
    /// <returns>The current builder for fluent chaining.</returns>
    public TechnicalDataBuilder WithBearingCharacteristics(Action<BearingCharacteristicsGroupBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var groupBuilder = new BearingCharacteristicsGroupBuilder(this);
        configure(groupBuilder);

        return this;
    }

    /// <summary>
    /// Validates and builds the Technical Data submodel, then attaches it to the parent shell.
    /// This central build step ensures required parameters, units and semantics are validated before persistence.
    /// </summary>
    /// <returns>The parent shell builder to continue composing the AAS.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required properties are missing or semantic mappings are invalid.
    /// </exception>
    public IShellBuilder BuildTechnicalData()
    {
        ValidateRequiredParameters();

        var submodelBuilder = new SubmodelBuilder(_shellBuilder, _id, _idShort)
            .WithSemanticId(ReferenceFactory.GlobalConceptDescription(TechnicalDataSemantics.SubmodelTechnicalData));

        foreach (var group in _groups.Values.OrderBy(x => x.IdShort, StringComparer.Ordinal))
        {
            var collection = new SubmodelElementCollection
            {
                IdShort = group.IdShort,
                Value = [],
                SemanticId = ReferenceFactory.GlobalSemanticReference($"{TechnicalDataSemantics.SubmodelTechnicalData}/{group.IdShort}")
            };

            foreach (var assignment in group.Assignments.Values.OrderBy(x => x.Definition.IdShort, StringComparer.Ordinal))
            {
                ValidateEclassCatalogMapping(assignment.Definition.EclassIrdi);

                var property = new Property(assignment.Definition.ValueType)
                {
                    IdShort = assignment.Definition.IdShort,
                    Value = assignment.Value,
                    ValueId = ReferenceFactory.GlobalSemanticReference(assignment.Definition.EclassIrdi),
                    SemanticId = ReferenceFactory.GlobalConceptDescription(assignment.Definition.EclassIrdi),
                    Category = assignment.Unit
                };

                collection.Value!.Add(property);
            }

            submodelBuilder.AddElement(collection);
        }

        var submodel = submodelBuilder.BuildSubmodel();
        _shellBuilder.AddSubmodelReference(submodel);

        return _shellBuilder;
    }

    private void ValidateRequiredParameters()
    {
        var missing = new List<string>();

        foreach (var groupIdShort in new[] { TechnicalDataIdentifiers.MotorPerformance, TechnicalDataIdentifiers.BearingCharacteristics })
        {
            if (!_groups.TryGetValue(groupIdShort, out var group))
            {
                missing.AddRange(TechnicalDataEclassCatalog.RequiredInGroup(groupIdShort).Select(x => $"{groupIdShort}.{x.IdShort}"));
                continue;
            }

            var required = TechnicalDataEclassCatalog.RequiredInGroup(groupIdShort);
            missing.AddRange(required.Where(x => !group.Assignments.ContainsKey(x.IdShort)).Select(x => $"{groupIdShort}.{x.IdShort}"));
        }

        if (missing.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException("Technical data is incomplete. Missing mandatory parameters: " + string.Join(", ", missing));
    }

    private static void ValidateEclassCatalogMapping(string eclassIrdi)
    {
        if (!TechnicalDataEclassCatalog.IsKnownEclassReference(eclassIrdi))
        {
            throw new InvalidOperationException($"The ECLASS semantic ID '{eclassIrdi}' is not available in the integrated ECLASS catalog map.");
        }
    }

    internal void UpsertDecimalProperty(string groupIdShort, string idShort, decimal value, string unit, string? semanticIdOverride)
    {
        var definition = TechnicalDataEclassCatalog.ForIdShort(idShort);

        if (!string.Equals(definition.GroupIdShort, groupIdShort, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Property '{idShort}' is not allowed in group '{groupIdShort}'.");
        }

        if (definition.ValueType != DataTypeDefXsd.Decimal)
        {
            throw new InvalidOperationException($"Property '{idShort}' does not accept decimal values according to IEC 61360 type constraints.");
        }

        if (!definition.AllowedUnits.Contains(unit, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Unit '{unit}' is invalid for '{idShort}'. Allowed IEC 61360 units: {string.Join(", ", definition.AllowedUnits)}.");
        }

        if (!string.IsNullOrWhiteSpace(semanticIdOverride) && !string.Equals(semanticIdOverride, definition.EclassIrdi, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Semantic ID mismatch for '{idShort}'. Expected '{definition.EclassIrdi}' but got '{semanticIdOverride}'.");
        }

        var group = GetOrCreateGroup(groupIdShort);
        group.Assignments[idShort] = new PropertyAssignment(definition, value.ToString(System.Globalization.CultureInfo.InvariantCulture), unit);
    }

    private GroupAssignment GetOrCreateGroup(string groupIdShort)
    {
        if (_groups.TryGetValue(groupIdShort, out var existing))
        {
            return existing;
        }

        var created = new GroupAssignment(groupIdShort);
        _groups[groupIdShort] = created;
        return created;
    }

    private sealed class GroupAssignment(string idShort)
    {
        public string IdShort { get; } = idShort;

        public Dictionary<string, PropertyAssignment> Assignments { get; } = new(StringComparer.Ordinal);
    }

    private sealed record PropertyAssignment(
        TechnicalDataEclassCatalog.PropertyDefinition Definition,
        string Value,
        string Unit);

    /// <summary>
    /// Group builder for motor performance properties.
    /// The API is deliberately constrained to valid motor fields for safer, more predictable template usage.
    /// </summary>
    public sealed class MotorPerformanceGroupBuilder(TechnicalDataBuilder parent)
    {
        /// <summary>
        /// Sets rated voltage with IEC 61360-conform unit and optional explicit semantic check.
        /// </summary>
        /// <param name="value">Numeric rated voltage value.</param>
        /// <param name="unit">Unit symbol (default: V).</param>
        /// <param name="semanticId">Optional ECLASS IRDI for explicit semantic conformity check.</param>
        /// <returns>The current motor performance builder.</returns>
        public MotorPerformanceGroupBuilder WithRatedVoltage(decimal value, string unit = "V", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.MotorPerformance, TechnicalDataIdentifiers.RatedVoltage, value, unit, semanticId);
            return this;
        }

        /// <summary>
        /// Sets rated current with IEC 61360-conform unit and optional explicit semantic check.
        /// </summary>
        /// <param name="value">Numeric rated current value.</param>
        /// <param name="unit">Unit symbol (default: A).</param>
        /// <param name="semanticId">Optional ECLASS IRDI for explicit semantic conformity check.</param>
        /// <returns>The current motor performance builder.</returns>
        public MotorPerformanceGroupBuilder WithRatedCurrent(decimal value, string unit = "A", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.MotorPerformance, TechnicalDataIdentifiers.RatedCurrent, value, unit, semanticId);
            return this;
        }

        /// <summary>
        /// Sets rated power with IEC 61360-conform unit and optional explicit semantic check.
        /// </summary>
        /// <param name="value">Numeric rated power value.</param>
        /// <param name="unit">Unit symbol (default: kW).</param>
        /// <param name="semanticId">Optional ECLASS IRDI for explicit semantic conformity check.</param>
        /// <returns>The current motor performance builder.</returns>
        public MotorPerformanceGroupBuilder WithRatedPower(decimal value, string unit = "kW", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.MotorPerformance, TechnicalDataIdentifiers.RatedPower, value, unit, semanticId);
            return this;
        }

        /// <summary>
        /// Sets rated speed with IEC 61360-conform unit and optional explicit semantic check.
        /// </summary>
        /// <param name="value">Numeric rated speed value.</param>
        /// <param name="unit">Unit symbol (default: 1/min).</param>
        /// <param name="semanticId">Optional ECLASS IRDI for explicit semantic conformity check.</param>
        /// <returns>The current motor performance builder.</returns>
        public MotorPerformanceGroupBuilder WithRatedSpeed(decimal value, string unit = "1/min", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.MotorPerformance, TechnicalDataIdentifiers.RatedSpeed, value, unit, semanticId);
            return this;
        }
    }

    /// <summary>
    /// Group builder for bearing characteristics.
    /// The focused API helps callers provide structurally valid bearing data with consistent semantics.
    /// </summary>
    public sealed class BearingCharacteristicsGroupBuilder(TechnicalDataBuilder parent)
    {
        /// <summary>
        /// Sets bearing inner diameter with IEC 61360-conform unit and optional explicit semantic check.
        /// </summary>
        /// <param name="value">Numeric inner diameter value.</param>
        /// <param name="unit">Unit symbol (default: mm).</param>
        /// <param name="semanticId">Optional ECLASS IRDI for explicit semantic conformity check.</param>
        /// <returns>The current bearing characteristics builder.</returns>
        public BearingCharacteristicsGroupBuilder WithInnerDiameter(decimal value, string unit = "mm", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataIdentifiers.InnerDiameter, value, unit, semanticId);
            return this;
        }

        /// <summary>
        /// Sets bearing outer diameter with IEC 61360-conform unit and optional explicit semantic check.
        /// </summary>
        /// <param name="value">Numeric outer diameter value.</param>
        /// <param name="unit">Unit symbol (default: mm).</param>
        /// <param name="semanticId">Optional ECLASS IRDI for explicit semantic conformity check.</param>
        /// <returns>The current bearing characteristics builder.</returns>
        public BearingCharacteristicsGroupBuilder WithOuterDiameter(decimal value, string unit = "mm", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataIdentifiers.OuterDiameter, value, unit, semanticId);
            return this;
        }

        /// <summary>
        /// Sets bearing width with IEC 61360-conform unit and optional explicit semantic check.
        /// </summary>
        /// <param name="value">Numeric width value.</param>
        /// <param name="unit">Unit symbol (default: mm).</param>
        /// <param name="semanticId">Optional ECLASS IRDI for explicit semantic conformity check.</param>
        /// <returns>The current bearing characteristics builder.</returns>
        public BearingCharacteristicsGroupBuilder WithWidth(decimal value, string unit = "mm", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataIdentifiers.Width, value, unit, semanticId);
            return this;
        }

        /// <summary>
        /// Sets bearing limiting speed with IEC 61360-conform unit and optional explicit semantic check.
        /// </summary>
        /// <param name="value">Numeric limiting speed value.</param>
        /// <param name="unit">Unit symbol (default: 1/min).</param>
        /// <param name="semanticId">Optional ECLASS IRDI for explicit semantic conformity check.</param>
        /// <returns>The current bearing characteristics builder.</returns>
        public BearingCharacteristicsGroupBuilder WithLimitingSpeed(decimal value, string unit = "1/min", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataIdentifiers.LimitingSpeed, value, unit, semanticId);
            return this;
        }
    }
}
