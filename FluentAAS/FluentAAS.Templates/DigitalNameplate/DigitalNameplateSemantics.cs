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

    /// <summary>
    /// Semantic identifier for the manufacturer product root element.
    /// </summary>
    public const string ManufacturerProductRoot = "0173-1#02-<AAXXXX>#001";

    /// <summary>
    /// Semantic identifier for the manufacturer product family element.
    /// </summary>
    public const string ManufacturerProductFamily = "0173-1#02-<AAYYYY>#001";

    /// <summary>
    /// Semantic identifier for the URI of the product element.
    /// </summary>
    public const string UriOfTheProduct = "0173-1#02-<AAZURI>#001";

    /// <summary>
    /// Semantic identifier for the manufacturer product type element.
    /// </summary>
    public const string ManufacturerProductType = "0173-1#02-<AATYPE>#001";

    /// <summary>
    /// Semantic identifier for the order code of manufacturer element.
    /// </summary>
    public const string OrderCodeOfManufacturer = "0173-1#02-<AAORDC>#001";

    /// <summary>
    /// Semantic identifier for the product article number of manufacturer element.
    /// </summary>
    public const string ProductArticleNumberOfManufacturer = "0173-1#02-<AAARTN>#001";

    /// <summary>
    /// Semantic identifier for the year of construction element.
    /// </summary>
    public const string YearOfConstruction = "0173-1#02-<AAYEAR>#001";

    /// <summary>
    /// Semantic identifier for the date of manufacture element.
    /// </summary>
    public const string DateOfManufacture = "0173-1#02-<AADATE>#001";

    /// <summary>
    /// Semantic identifier for the hardware version element.
    /// </summary>
    public const string HardwareVersion = "0173-1#02-<AAHWVS>#001";

    /// <summary>
    /// Semantic identifier for the firmware version element.
    /// </summary>
    public const string FirmwareVersion = "0173-1#02-<AAFWVS>#001";

    /// <summary>
    /// Semantic identifier for the software version element.
    /// </summary>
    public const string SoftwareVersion = "0173-1#02-<AASWVS>#001";

    /// <summary>
    /// Semantic identifier for the country of origin element.
    /// </summary>
    public const string CountryOfOrigin = "0173-1#02-<AACNTR>#001";

    /// <summary>
    /// Semantic identifier for the unique facility identifier element.
    /// </summary>
    public const string UniqueFacilityIdentifier = "0173-1#02-<AAUFID>#001";

    /// <summary>
    /// Semantic identifier for the address information collection.
    /// </summary>
    public const string AddressInformation = "0173-1#02-<AAADDR>#001";

    /// <summary>
    /// Semantic identifier for the company logo file element.
    /// </summary>
    public const string CompanyLogo = "0173-1#02-<AALOGO>#001";

    /// <summary>
    /// Semantic identifier for the markings list element.
    /// </summary>
    public const string Markings = "0173-1#02-<AAMARK>#001";

    /// <summary>
    /// Semantic identifier for the asset-specific properties collection.
    /// </summary>
    public const string AssetSpecificProperties = "0173-1#02-<AAASSP>#001";
}