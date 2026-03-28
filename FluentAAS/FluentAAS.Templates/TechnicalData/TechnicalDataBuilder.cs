using FluentAAS.Builder;
using FluentAAS.Builder.SubModel;

namespace FluentAAS.Templates.TechnicalData;

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

    public TechnicalDataBuilder WithMotorPerformance(Action<MotorPerformanceGroupBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var groupBuilder = new MotorPerformanceGroupBuilder(this);
        configure(groupBuilder);

        return this;
    }

    public TechnicalDataBuilder WithBearingCharacteristics(Action<BearingCharacteristicsGroupBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var groupBuilder = new BearingCharacteristicsGroupBuilder(this);
        configure(groupBuilder);

        return this;
    }

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

    public sealed class MotorPerformanceGroupBuilder(TechnicalDataBuilder parent)
    {
        public MotorPerformanceGroupBuilder WithRatedVoltage(decimal value, string unit = "V", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.MotorPerformance, TechnicalDataIdentifiers.RatedVoltage, value, unit, semanticId);
            return this;
        }

        public MotorPerformanceGroupBuilder WithRatedCurrent(decimal value, string unit = "A", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.MotorPerformance, TechnicalDataIdentifiers.RatedCurrent, value, unit, semanticId);
            return this;
        }

        public MotorPerformanceGroupBuilder WithRatedPower(decimal value, string unit = "kW", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.MotorPerformance, TechnicalDataIdentifiers.RatedPower, value, unit, semanticId);
            return this;
        }

        public MotorPerformanceGroupBuilder WithRatedSpeed(decimal value, string unit = "1/min", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.MotorPerformance, TechnicalDataIdentifiers.RatedSpeed, value, unit, semanticId);
            return this;
        }
    }

    public sealed class BearingCharacteristicsGroupBuilder(TechnicalDataBuilder parent)
    {
        public BearingCharacteristicsGroupBuilder WithInnerDiameter(decimal value, string unit = "mm", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataIdentifiers.InnerDiameter, value, unit, semanticId);
            return this;
        }

        public BearingCharacteristicsGroupBuilder WithOuterDiameter(decimal value, string unit = "mm", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataIdentifiers.OuterDiameter, value, unit, semanticId);
            return this;
        }

        public BearingCharacteristicsGroupBuilder WithWidth(decimal value, string unit = "mm", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataIdentifiers.Width, value, unit, semanticId);
            return this;
        }

        public BearingCharacteristicsGroupBuilder WithLimitingSpeed(decimal value, string unit = "1/min", string? semanticId = null)
        {
            parent.UpsertDecimalProperty(TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataIdentifiers.LimitingSpeed, value, unit, semanticId);
            return this;
        }
    }
}
