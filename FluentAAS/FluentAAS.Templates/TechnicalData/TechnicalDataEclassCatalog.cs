namespace FluentAAS.Templates.TechnicalData;

internal static class TechnicalDataEclassCatalog
{
    private static readonly IReadOnlyDictionary<string, PropertyDefinition> DefinitionsByIdShort =
        new Dictionary<string, PropertyDefinition>(StringComparer.Ordinal)
        {
            [TechnicalDataIdentifiers.RatedVoltage] = new PropertyDefinition(TechnicalDataIdentifiers.RatedVoltage, TechnicalDataIdentifiers.MotorPerformance, TechnicalDataSemantics.RatedVoltage, DataTypeDefXsd.Decimal, true, ["V"]),
            [TechnicalDataIdentifiers.RatedCurrent] = new PropertyDefinition(TechnicalDataIdentifiers.RatedCurrent, TechnicalDataIdentifiers.MotorPerformance, TechnicalDataSemantics.RatedCurrent, DataTypeDefXsd.Decimal, true, ["A"]),
            [TechnicalDataIdentifiers.RatedPower] = new PropertyDefinition(TechnicalDataIdentifiers.RatedPower, TechnicalDataIdentifiers.MotorPerformance, TechnicalDataSemantics.RatedPower, DataTypeDefXsd.Decimal, true, ["kW", "W"]),
            [TechnicalDataIdentifiers.RatedSpeed] = new PropertyDefinition(TechnicalDataIdentifiers.RatedSpeed, TechnicalDataIdentifiers.MotorPerformance, TechnicalDataSemantics.RatedSpeed, DataTypeDefXsd.Decimal, true, ["1/min", "rpm"]),

            [TechnicalDataIdentifiers.InnerDiameter] = new PropertyDefinition(TechnicalDataIdentifiers.InnerDiameter, TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataSemantics.InnerDiameter, DataTypeDefXsd.Decimal, true, ["mm"]),
            [TechnicalDataIdentifiers.OuterDiameter] = new PropertyDefinition(TechnicalDataIdentifiers.OuterDiameter, TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataSemantics.OuterDiameter, DataTypeDefXsd.Decimal, true, ["mm"]),
            [TechnicalDataIdentifiers.Width] = new PropertyDefinition(TechnicalDataIdentifiers.Width, TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataSemantics.Width, DataTypeDefXsd.Decimal, true, ["mm"]),
            [TechnicalDataIdentifiers.LimitingSpeed] = new PropertyDefinition(TechnicalDataIdentifiers.LimitingSpeed, TechnicalDataIdentifiers.BearingCharacteristics, TechnicalDataSemantics.LimitingSpeed, DataTypeDefXsd.Decimal, true, ["1/min", "rpm"])
        };

    public static PropertyDefinition ForIdShort(string idShort)
    {
        if (!DefinitionsByIdShort.TryGetValue(idShort, out var definition))
        {
            throw new InvalidOperationException($"No ECLASS catalog mapping exists for technical property '{idShort}'.");
        }

        return definition;
    }

    public static IReadOnlyCollection<PropertyDefinition> RequiredInGroup(string groupIdShort)
    {
        return DefinitionsByIdShort.Values.Where(x => x.GroupIdShort == groupIdShort && x.IsRequired).ToArray();
    }

    public static bool IsKnownEclassReference(string eclassIrdi)
    {
        return DefinitionsByIdShort.Values.Any(x => string.Equals(x.EclassIrdi, eclassIrdi, StringComparison.Ordinal));
    }

    internal sealed record PropertyDefinition(
        string IdShort,
        string GroupIdShort,
        string EclassIrdi,
        DataTypeDefXsd ValueType,
        bool IsRequired,
        IReadOnlyCollection<string> AllowedUnits);
}
