namespace FluentAAS.Templates.DigitalNameplate;

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
        "https://admin-shell.io/idta/SubmodelTemplate/DigitalNameplate/3/0";

    /// <summary>
    /// Semantic identifier (IRDI) for the manufacturer name element.
    /// </summary>
    public const string ManufacturerName = "112/2///61987#ABA565#009";
    
    /// <summary>
    /// Semantic identifier (IRDI) for the manufacturer product designation element.
    /// </summary>
    public const string ManufacturerProductDesignation = "0112/2///61987#ABA567#009";

    /// <summary>
    /// Semantic identifier (IRDI) for the serial number element.
    /// </summary>
    public const string SerialNumber = "0112/2///61987#ABA951#009";

    /// <summary>
    /// Semantic identifier for the manufacturer product root element.
    /// </summary>
    public const string ManufacturerProductRoot = "0112/2///61360_7#AAS011#001";

    /// <summary>
    /// Semantic identifier for the manufacturer product family element.
    /// </summary>
    public const string ManufacturerProductFamily = "0112/2///61987#ABP464#002";

    /// <summary>
    /// Semantic identifier for the URI of the product element.
    /// </summary>
    public const string UriOfTheProduct = "0112/2///61987#ABN590#002";

    /// <summary>
    /// Semantic identifier for the manufacturer product type element.
    /// </summary>
    public const string ManufacturerProductType = "0112/2///61987#ABA300#008";

    /// <summary>
    /// Semantic identifier for the order code of manufacturer element.
    /// </summary>
    public const string OrderCodeOfManufacturer = "0112/2///61987#ABA950#008";

    /// <summary>
    /// Semantic identifier for the product article number of manufacturer element.
    /// </summary>
    public const string ProductArticleNumberOfManufacturer = "0112/2///61987#ABA581#007";

    /// <summary>
    /// Semantic identifier for the year of construction element.
    /// </summary>
    public const string YearOfConstruction = "0112/2///61987#ABP000#002";

    /// <summary>
    /// Semantic identifier for the date of manufacture element.
    /// </summary>
    public const string DateOfManufacture = "0112/2///61987#ABB757#007";

    /// <summary>
    /// Semantic identifier for the hardware version element.
    /// </summary>
    public const string HardwareVersion = "0112/2///61987#ABA926#008";

    /// <summary>
    /// Semantic identifier for the firmware version element.
    /// </summary>
    public const string FirmwareVersion = "0112/2///61987#ABA302#006";

    /// <summary>
    /// Semantic identifier for the software version element.
    /// </summary>
    public const string SoftwareVersion = "0112/2///61987#ABA601#008";

    /// <summary>
    /// Semantic identifier for the country of origin element.
    /// </summary>
    public const string CountryOfOrigin = "0112/2///61987#ABP462#001";

    /// <summary>
    /// Semantic identifier for the unique facility identifier element.
    /// </summary>
    public const string UniqueFacilityIdentifier = "https://admin-shell.io/idta/nameplate/3/0/UniqueFacilityIdentifier";

    /// <summary>
    /// Semantic identifier for the address information collection.
    /// </summary>
    public const string AddressInformation = "https://admin-shell.io/zvei/nameplate/1/0/ContactInformations/AddressInformation";

    /// <summary>
    /// Semantic identifier for the company logo file element.
    /// </summary>
    public const string CompanyLogo = "0112/2///61987#ABP463#001";

    /// <summary>
    /// Semantic identifier for the markings list element.
    /// </summary>
    public const string Markings = "https://admin-shell.io/zvei/nameplate/3/0/Nameplate/Markings";

    /// <summary>
    /// Semantic identifier for the asset-specific properties collection.
    /// </summary>
    public const string AssetSpecificProperties = "0173-1#02-ABI218#003/0173-1#01-AGZ672#004";
}