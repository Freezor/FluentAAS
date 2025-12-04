namespace FluentAAS.Templates;

/// <summary>
/// Provides semantic identifiers (IRDI / concept description references)
/// used by the IDTA Digital Nameplate submodel.
/// </summary>
public static class DigitalNameplateSemantics
{
    /// <summary>
    /// Semantic identifier for the Digital Nameplate submodel itself.
    /// </summary>
    public const string SubmodelDigitalNameplate =
        "https://admin-shell.io/zvei/nameplate/2/0/Nameplate";

    /// <summary>
    /// Semantic identifier (IRDI) for the manufacturer name element.
    /// </summary>
    public const string ManufacturerName = "0173-1#02-AAO677#002";

    /// <summary>
    /// Semantic identifier (IRDI) for the manufacturer product designation element.
    /// </summary>
    public const string ManufacturerProductDesignation = "0173-1#02-AAW338#001";

    /// <summary>
    /// Semantic identifier (IRDI) for the serial number element.
    /// </summary>
    public const string SerialNumber = "0173-1#02-AAM556#002";
}