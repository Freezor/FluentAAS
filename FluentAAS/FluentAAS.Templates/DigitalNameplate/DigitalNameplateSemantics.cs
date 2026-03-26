namespace FluentAAS.Templates.DigitalNameplate;

/// <summary>
/// Provides semantic identifiers (IRDI / concept description references)
/// used by the IDTA Digital Nameplate submodel.
/// </summary>
public static class DigitalNameplateSemantics
{
    /// <summary>
    /// Submodel semantic ID for "Digital Nameplate".
    /// </summary>
    public const string SubmodelSemanticId = "https://admin-shell.io/idta/SubmodelTemplate/DigitalNameplate/3/0\n";
    
    /// <summary>
    /// Semantic identifier for the Digital Nameplate submodel itself.
    /// </summary>
    public const string SubmodelDigitalNameplate =
        "https://admin-shell.io/idta/SubmodelTemplate/DigitalNameplate/3/0";

    /// <summary>
    /// Semantic identifier (IRDI) for the manufacturer name element.
    /// </summary>
    public const string ManufacturerName = "0112/2///61987#ABA565[#009](#009)";
    
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
    /// Semantic identifier for the top-level contact information collection.
    /// </summary>
    public const string ContactInformation = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation";

    /// <summary>
    /// Semantic identifier for the manufacturer contact collection.
    /// </summary>
    public const string ManufacturerContact = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/ManufacturerContact";

    /// <summary>
    /// Semantic identifier for the service hotline field.
    /// </summary>
    public const string ServiceHotline = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/ServiceHotline";

    /// <summary>
    /// Semantic identifier for the email field.
    /// </summary>
    public const string EmailAddress = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/Email";

    /// <summary>
    /// Semantic identifier for the website URL field.
    /// </summary>
    public const string WebsiteUrl = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/WebsiteUrl";

    /// <summary>
    /// Semantic identifier for the contact role field.
    /// </summary>
    public const string ContactRole = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/Role";

    /// <summary>
    /// Semantic identifier for the contact name field.
    /// </summary>
    public const string ContactName = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/Name";

    /// <summary>
    /// Semantic identifier for the phone field.
    /// </summary>
    public const string Phone = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/Phone";

    /// <summary>
    /// Semantic identifier for street within address information.
    /// </summary>
    public const string AddressStreet = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/AddressInformation/Street";

    /// <summary>
    /// Semantic identifier for city within address information.
    /// </summary>
    public const string AddressCity = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/AddressInformation/City";

    /// <summary>
    /// Semantic identifier for postal code within address information.
    /// </summary>
    public const string AddressPostalCode = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/AddressInformation/PostalCode";

    /// <summary>
    /// Semantic identifier for country code within address information.
    /// </summary>
    public const string AddressCountryCode = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/AddressInformation/CountryCode";

    /// <summary>
    /// Semantic identifier for state within address information.
    /// </summary>
    public const string AddressState = "https://admin-shell.io/idta/nameplate/3/0/ContactInformation/AddressInformation/State";

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