namespace FluentAAS.Templates.TechnicalData;

/// <summary>
/// Provides stable semantic identifiers for the Technical Data template.
/// Using these constants helps avoid typos and ensures that created AAS elements
/// are linked to the intended ECLASS/IDTA semantics.
/// </summary>
public static class TechnicalDataSemantics
{
    /// <summary>
    /// Semantic identifier of the IDTA Technical Data submodel template (v3.0).
    /// </summary>
    public const string SubmodelTechnicalData = "https://admin-shell.io/idta/TechnicalData/SubmodelTemplate/3/0";

    /// <summary>
    /// ECLASS IRDI for rated voltage.
    /// </summary>
    public const string RatedVoltage = "0173-1#02-BAF053#008";

    /// <summary>
    /// ECLASS IRDI for rated current.
    /// </summary>
    public const string RatedCurrent = "0173-1#02-BAF054#008";

    /// <summary>
    /// ECLASS IRDI for rated power.
    /// </summary>
    public const string RatedPower = "0173-1#02-BAF055#008";

    /// <summary>
    /// ECLASS IRDI for rated speed.
    /// </summary>
    public const string RatedSpeed = "0173-1#02-BAF056#008";

    /// <summary>
    /// ECLASS IRDI for bearing inner diameter.
    /// </summary>
    public const string InnerDiameter = "0173-1#02-AAO677#002";

    /// <summary>
    /// ECLASS IRDI for bearing outer diameter.
    /// </summary>
    public const string OuterDiameter = "0173-1#02-AAO678#002";

    /// <summary>
    /// ECLASS IRDI for bearing width.
    /// </summary>
    public const string Width = "0173-1#02-AAO679#002";

    /// <summary>
    /// ECLASS IRDI for bearing limiting speed.
    /// </summary>
    public const string LimitingSpeed = "0173-1#02-AAO680#002";
}
