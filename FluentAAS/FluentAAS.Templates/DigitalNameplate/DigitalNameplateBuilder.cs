using FluentAAS.Builder;
using FluentAAS.Builder.SubModel;
using static FluentAAS.Templates.DigitalNameplate.DigitalNameplateSemantics;

namespace FluentAAS.Templates.DigitalNameplate;

/// <summary>
/// Provides a guided fluent builder for the IDTA Digital Nameplate submodel.
/// The builder enforces mandatory core properties and attaches the submodel
/// to a given <see cref="ShellBuilder"/>.
/// </summary>
public sealed class DigitalNameplateBuilder
{
    private readonly IShellBuilder _shellBuilder;

    private string _id;
    private string _idShort;

    // Mandatory multi-language fields (language code → text)
    private readonly Dictionary<string, string> _manufacturerName               = new();
    private readonly Dictionary<string, string> _manufacturerProductDesignation = new();

    // Optional multi-language fields from the template
    private readonly Dictionary<string, string> _manufacturerProductRoot   = new();
    private readonly Dictionary<string, string> _manufacturerProductFamily = new();

    // Mandatory single-value field
    private string? _serialNumber;

    // Additional single-value properties from the template
    private string? _uriOfTheProduct;
    private string? _manufacturerProductType;
    private string? _orderCodeOfManufacturer;
    private string? _productArticleNumberOfManufacturer;
    private string? _yearOfConstruction;
    private string? _dateOfManufactureIso;
    private string? _hardwareVersion;
    private string? _firmwareVersion;
    private string? _softwareVersion;
    private string? _countryOfOrigin;
    private string? _uniqueFacilityIdentifier;

    // Complex AAS elements from the template
    private SubmodelElementCollection? _addressInformation;
    private Aas.File?                  _companyLogo;
    private SubmodelElementList?       _markings;
    private SubmodelElementCollection? _assetSpecificProperties;

    /// <summary>
    /// Initializes a new instance of the <see cref="DigitalNameplateBuilder"/> class.
    /// </summary>
    /// <param name="shellBuilder">The parent <see cref="ShellBuilder"/> to which the submodel will be attached.</param>
    /// <param name="id">The identifier of the digital nameplate submodel.</param>
    /// <param name="idShort">The short identifier of the digital nameplate submodel.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="shellBuilder"/>, <paramref name="id"/>, or <paramref name="idShort"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> or <paramref name="idShort"/> is empty or whitespace.
    /// </exception>
    internal DigitalNameplateBuilder(IShellBuilder shellBuilder, string id, string idShort)
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

        _id      = id;
        _idShort = idShort;
    }

    /// <summary>
    /// Updates the identifiers of the digital nameplate submodel.
    /// </summary>
    /// <param name="id">The new identifier of the submodel.</param>
    /// <param name="idShort">The new short identifier of the submodel.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> or <paramref name="idShort"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithIds(string id, string idShort)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(idShort))
        {
            throw new ArgumentException("Submodel idShort must not be empty.", nameof(idShort));
        }

        _id      = id;
        _idShort = idShort;
        return this;
    }

    /// <summary>
    /// Adds or updates a localized manufacturer name entry.
    /// </summary>
    /// <param name="language">The language code (e.g. <c>"en"</c>, <c>"de"</c>).</param>
    /// <param name="text">The localized manufacturer name.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="language"/> or <paramref name="text"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithManufacturerName(string language, string text)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentException("Language must not be empty.", nameof(language));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Manufacturer name must not be empty.", nameof(text));
        }

        _manufacturerName[language] = text;
        return this;
    }

    /// <summary>
    /// Adds or updates a localized manufacturer product designation entry.
    /// </summary>
    /// <param name="language">The language code (e.g. <c>"en"</c>, <c>"de"</c>).</param>
    /// <param name="text">The localized product designation.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="language"/> or <paramref name="text"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithManufacturerProductDesignation(string language, string text)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentException("Language must not be empty.", nameof(language));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Product designation must not be empty.", nameof(text));
        }

        _manufacturerProductDesignation[language] = text;
        return this;
    }

    /// <summary>
    /// Adds or updates a localized manufacturer product root entry.
    /// </summary>
    /// <param name="language">The language code (e.g. <c>"en"</c>, <c>"de"</c>).</param>
    /// <param name="text">The localized product root text.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="language"/> or <paramref name="text"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithManufacturerProductRoot(string language, string text)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentException("Language must not be empty.", nameof(language));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Manufacturer product root must not be empty.", nameof(text));
        }

        _manufacturerProductRoot[language] = text;
        return this;
    }

    /// <summary>
    /// Adds or updates a localized manufacturer product family entry.
    /// </summary>
    /// <param name="language">The language code (e.g. <c>"en"</c>, <c>"de"</c>).</param>
    /// <param name="text">The localized product family text.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="language"/> or <paramref name="text"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithManufacturerProductFamily(string language, string text)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentException("Language must not be empty.", nameof(language));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Manufacturer product family must not be empty.", nameof(text));
        }

        _manufacturerProductFamily[language] = text;
        return this;
    }

    /// <summary>
    /// Sets the serial number of the asset.
    /// </summary>
    /// <param name="serialNumber">The serial number value.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="serialNumber"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithSerialNumber(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            throw new ArgumentException("Serial number must not be empty.", nameof(serialNumber));
        }

        _serialNumber = serialNumber;
        return this;
    }

    /// <summary>
    /// Sets the URI of the product (<c>UriOfTheProduct</c>).
    /// </summary>
    /// <param name="uri">The product URI (e.g. a product URL or digital twin URI).</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="uri"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithUriOfTheProduct(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new ArgumentException("URI of the product must not be empty.", nameof(uri));
        }

        _uriOfTheProduct = uri;
        return this;
    }

    /// <summary>
    /// Sets the manufacturer product type (<c>ManufacturerProductType</c>).
    /// </summary>
    /// <param name="type">The manufacturer product type string.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="type"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithManufacturerProductType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Manufacturer product type must not be empty.", nameof(type));
        }

        _manufacturerProductType = type;
        return this;
    }

    /// <summary>
    /// Sets the order code of the manufacturer (<c>OrderCodeOfManufacturer</c>).
    /// </summary>
    /// <param name="orderCode">The order code of the product.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="orderCode"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithOrderCodeOfManufacturer(string orderCode)
    {
        if (string.IsNullOrWhiteSpace(orderCode))
        {
            throw new ArgumentException("Order code must not be empty.", nameof(orderCode));
        }

        _orderCodeOfManufacturer = orderCode;
        return this;
    }

    /// <summary>
    /// Sets the product article number of the manufacturer (<c>ProductArticleNumberOfManufacturer</c>).
    /// </summary>
    /// <param name="articleNumber">The article number of the product.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="articleNumber"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithProductArticleNumberOfManufacturer(string articleNumber)
    {
        if (string.IsNullOrWhiteSpace(articleNumber))
        {
            throw new ArgumentException("Product article number must not be empty.", nameof(articleNumber));
        }

        _productArticleNumberOfManufacturer = articleNumber;
        return this;
    }

    /// <summary>
    /// Sets the year of construction (<c>YearOfConstruction</c>).
    /// </summary>
    /// <param name="year">The year of construction as a four-digit string (e.g. <c>"2025"</c>).</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="year"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithYearOfConstruction(string year)
    {
        if (string.IsNullOrWhiteSpace(year))
        {
            throw new ArgumentException("Year of construction must not be empty.", nameof(year));
        }

        _yearOfConstruction = year;
        return this;
    }

    /// <summary>
    /// Sets the date of manufacture (<c>DateOfManufacture</c>).
    /// </summary>
    /// <param name="date">The manufacturing date.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    public DigitalNameplateBuilder WithDateOfManufacture(DateTime date)
    {
        _dateOfManufactureIso = date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        return this;
    }

    /// <summary>
    /// Sets the hardware version (<c>HardwareVersion</c>).
    /// </summary>
    /// <param name="version">The hardware version string.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="version"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithHardwareVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Hardware version must not be empty.", nameof(version));
        }

        _hardwareVersion = version;
        return this;
    }

    /// <summary>
    /// Sets the firmware version (<c>FirmwareVersion</c>).
    /// </summary>
    /// <param name="version">The firmware version string.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="version"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithFirmwareVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Firmware version must not be empty.", nameof(version));
        }

        _firmwareVersion = version;
        return this;
    }

    /// <summary>
    /// Sets the software version (<c>SoftwareVersion</c>).
    /// </summary>
    /// <param name="version">The software version string.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="version"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithSoftwareVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Software version must not be empty.", nameof(version));
        }

        _softwareVersion = version;
        return this;
    }

    /// <summary>
    /// Sets the country of origin (<c>CountryOfOrigin</c>).
    /// </summary>
    /// <param name="countryCode">
    /// The country code according to ISO 3166-1 alpha-2 (e.g. <c>"DE"</c>, <c>"US"</c>).
    /// </param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="countryCode"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithCountryOfOrigin(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            throw new ArgumentException("Country of origin must not be empty.", nameof(countryCode));
        }

        _countryOfOrigin = countryCode;
        return this;
    }

    /// <summary>
    /// Sets the unique facility identifier (<c>UniqueFacilityIdentifier</c>).
    /// </summary>
    /// <param name="identifier">The unique facility identifier string.</param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="identifier"/> is null, empty, or whitespace.
    /// </exception>
    public DigitalNameplateBuilder WithUniqueFacilityIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException("Unique facility identifier must not be empty.", nameof(identifier));
        }

        _uniqueFacilityIdentifier = identifier;
        return this;
    }

    /// <summary>
    /// Sets the address information structure (<c>AddressInformation</c>).
    /// </summary>
    /// <param name="addressInformation">
    /// A <see cref="SubmodelElementCollection"/> representing the address information
    /// as defined by the corresponding SMT drop-in.
    /// </param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="addressInformation"/> is <c>null</c>.
    /// </exception>
    public DigitalNameplateBuilder WithAddressInformation(SubmodelElementCollection addressInformation)
    {
        _addressInformation = addressInformation ?? throw new ArgumentNullException(nameof(addressInformation));
        return this;
    }

    /// <summary>
    /// Sets the company logo (<c>CompanyLogo</c>) as a File submodel element.
    /// </summary>
    /// <param name="companyLogo">
    /// The <see cref="AasCore.Aas3_0.File"/> element representing the company logo.
    /// </param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="companyLogo"/> is <c>null</c>.
    /// </exception>
    public DigitalNameplateBuilder WithCompanyLogo(Aas.File companyLogo)
    {
        _companyLogo = companyLogo ?? throw new ArgumentNullException(nameof(companyLogo));
        return this;
    }

    /// <summary>
    /// Sets the markings list (<c>Markings</c>) as a <see cref="SubmodelElementList"/>.
    /// </summary>
    /// <param name="markings">
    /// The <see cref="SubmodelElementList"/> representing the markings collection.
    /// </param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="markings"/> is <c>null</c>.
    /// </exception>
    public DigitalNameplateBuilder WithMarkings(SubmodelElementList markings)
    {
        _markings = markings ?? throw new ArgumentNullException(nameof(markings));
        return this;
    }

    /// <summary>
    /// Sets the asset specific properties (<c>AssetSpecificProperties</c>) as a 
    /// <see cref="SubmodelElementCollection"/>.
    /// </summary>
    /// <param name="assetSpecificProperties">
    /// The <see cref="SubmodelElementCollection"/> grouping the asset-specific properties.
    /// </param>
    /// <returns>The current <see cref="DigitalNameplateBuilder"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="assetSpecificProperties"/> is <c>null</c>.
    /// </exception>
    public DigitalNameplateBuilder WithAssetSpecificProperties(SubmodelElementCollection assetSpecificProperties)
    {
        _assetSpecificProperties = assetSpecificProperties ?? throw new ArgumentNullException(nameof(assetSpecificProperties));
        return this;
    }

    /// <summary>
    /// Builds the Digital Nameplate submodel, attaches it to the shell,
    /// and returns the parent <see cref="ShellBuilder"/>.
    /// </summary>
    /// <returns>The parent <see cref="ShellBuilder"/> for further configuration.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when mandatory fields (manufacturer name, product designation, or serial number)
    /// have not been provided.
    /// </exception>
    public IShellBuilder Build()
    {
        var missing = new List<string>();

        if (_manufacturerName.Count == 0)
        {
            missing.Add("ManufacturerName");
        }

        if (_manufacturerProductDesignation.Count == 0)
        {
            missing.Add("ManufacturerProductDesignation");
        }

        if (string.IsNullOrWhiteSpace(_serialNumber))
        {
            missing.Add("SerialNumber");
        }

        if (missing.Count != 0)
        {
            throw new InvalidOperationException(
                                                "Digital Nameplate is incomplete. Missing mandatory fields: " +
                                                string.Join(", ", missing));
        }

        // Use the generic submodel builder
        var submodelBuilder = new SubmodelBuilder(_shellBuilder, _id, _idShort)
            .WithSemanticId(RefFactory.GlobalConceptDescription(SubmodelDigitalNameplate));

        var submodel = submodelBuilder.BuildSubmodel();

        // UriOfTheProduct
        if (!string.IsNullOrWhiteSpace(_uriOfTheProduct))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "UriOfTheProduct",
                                    _uriOfTheProduct!,
                                    RefFactory.GlobalConceptDescription(UriOfTheProduct));
        }

        // ManufacturerName
        AddMultiLanguagePropertyWithSemantic(
                                             submodelBuilder,
                                             submodel,
                                             "ManufacturerName",
                                             RefFactory.GlobalConceptDescription(ManufacturerName),
                                             _manufacturerName);

        // ManufacturerProductDesignation
        AddMultiLanguagePropertyWithSemantic(
                                             submodelBuilder,
                                             submodel,
                                             "ManufacturerProductDesignation",
                                             RefFactory.GlobalConceptDescription(ManufacturerProductDesignation),
                                             _manufacturerProductDesignation);

        // ManufacturerProductRoot
        if (_manufacturerProductRoot.Count > 0)
        {
            AddMultiLanguagePropertyWithSemantic(
                                                 submodelBuilder,
                                                 submodel,
                                                 "ManufacturerProductRoot",
                                                 RefFactory.GlobalConceptDescription(ManufacturerProductRoot),
                                                 _manufacturerProductRoot);
        }

        // ManufacturerProductFamily
        if (_manufacturerProductFamily.Count > 0)
        {
            AddMultiLanguagePropertyWithSemantic(
                                                 submodelBuilder,
                                                 submodel,
                                                 "ManufacturerProductFamily",
                                                 RefFactory.GlobalConceptDescription(ManufacturerProductFamily),
                                                 _manufacturerProductFamily);
        }

        // ManufacturerProductType
        if (!string.IsNullOrWhiteSpace(_manufacturerProductType))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "ManufacturerProductType",
                                    _manufacturerProductType!,
                                    RefFactory.GlobalConceptDescription(ManufacturerProductType));
        }

        // OrderCodeOfManufacturer
        if (!string.IsNullOrWhiteSpace(_orderCodeOfManufacturer))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "OrderCodeOfManufacturer",
                                    _orderCodeOfManufacturer!,
                                    RefFactory.GlobalConceptDescription(OrderCodeOfManufacturer));
        }

        // ProductArticleNumberOfManufacturer
        if (!string.IsNullOrWhiteSpace(_productArticleNumberOfManufacturer))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "ProductArticleNumberOfManufacturer",
                                    _productArticleNumberOfManufacturer!,
                                    RefFactory.GlobalConceptDescription(ProductArticleNumberOfManufacturer));
        }

        // SerialNumber
        AddPropertyWithSemantic(
                                submodelBuilder,
                                submodel,
                                "SerialNumber",
                                _serialNumber!,
                                RefFactory.GlobalConceptDescription(SerialNumber));

        // YearOfConstruction
        if (!string.IsNullOrWhiteSpace(_yearOfConstruction))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "YearOfConstruction",
                                    _yearOfConstruction!,
                                    RefFactory.GlobalConceptDescription(YearOfConstruction));
        }

        // DateOfManufacture
        if (!string.IsNullOrWhiteSpace(_dateOfManufactureIso))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "DateOfManufacture",
                                    _dateOfManufactureIso!,
                                    RefFactory.GlobalConceptDescription(DateOfManufacture));
        }

        // HardwareVersion
        if (!string.IsNullOrWhiteSpace(_hardwareVersion))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "HardwareVersion",
                                    _hardwareVersion!,
                                    RefFactory.GlobalConceptDescription(HardwareVersion));
        }

        // FirmwareVersion
        if (!string.IsNullOrWhiteSpace(_firmwareVersion))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "FirmwareVersion",
                                    _firmwareVersion!,
                                    RefFactory.GlobalConceptDescription(FirmwareVersion));
        }

        // SoftwareVersion
        if (!string.IsNullOrWhiteSpace(_softwareVersion))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "SoftwareVersion",
                                    _softwareVersion!,
                                    RefFactory.GlobalConceptDescription(SoftwareVersion));
        }

        // CountryOfOrigin
        if (!string.IsNullOrWhiteSpace(_countryOfOrigin))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "CountryOfOrigin",
                                    _countryOfOrigin!,
                                    RefFactory.GlobalConceptDescription(CountryOfOrigin));
        }

        // UniqueFacilityIdentifier
        if (!string.IsNullOrWhiteSpace(_uniqueFacilityIdentifier))
        {
            AddPropertyWithSemantic(
                                    submodelBuilder,
                                    submodel,
                                    "UniqueFacilityIdentifier",
                                    _uniqueFacilityIdentifier!,
                                    RefFactory.GlobalConceptDescription(UniqueFacilityIdentifier));
        }

        // Complex elements (attached directly to the submodel)

        var elements = EnsureSubmodelElements(submodel);

        if (_addressInformation is not null)
        {
            if (string.IsNullOrWhiteSpace(_addressInformation.IdShort))
            {
                _addressInformation.IdShort = "AddressInformation";
            }

            _addressInformation.SemanticId ??= RefFactory.GlobalConceptDescription(AddressInformation);
            elements.Add(_addressInformation);
        }

        if (_companyLogo is not null)
        {
            if (string.IsNullOrWhiteSpace(_companyLogo.IdShort))
            {
                _companyLogo.IdShort = "CompanyLogo";
            }

            _companyLogo.SemanticId ??= RefFactory.GlobalConceptDescription(CompanyLogo);
            elements.Add(_companyLogo);
        }

        if (_markings is not null)
        {
            if (string.IsNullOrWhiteSpace(_markings.IdShort))
            {
                _markings.IdShort = "Markings";
            }

            _markings.SemanticId ??= RefFactory.GlobalConceptDescription(Markings);
            elements.Add(_markings);
        }

        if (_assetSpecificProperties is not null)
        {
            if (string.IsNullOrWhiteSpace(_assetSpecificProperties.IdShort))
            {
                _assetSpecificProperties.IdShort = "AssetSpecificProperties";
            }

            _assetSpecificProperties.SemanticId ??= RefFactory.GlobalConceptDescription(AssetSpecificProperties);
            elements.Add(_assetSpecificProperties);
        }

        // Attach the submodel to the shell
        _shellBuilder.AddSubmodelReference(submodel);

        // Return to the shell API
        return _shellBuilder;
    }

    /// <summary>
    /// Adds a multi-language property to the submodel and assigns its semantic identifier.
    /// </summary>
    /// <param name="builder">The submodel builder used to add the property.</param>
    /// <param name="submodel">The submodel instance containing the elements.</param>
    /// <param name="idShort">The idShort of the multi-language property.</param>
    /// <param name="semanticId">The semantic reference to assign.</param>
    /// <param name="values">The language-to-text mapping.</param>
    private static void AddMultiLanguagePropertyWithSemantic(
        SubmodelBuilder                     builder,
        Submodel                            submodel,
        string                              idShort,
        IReference                          semanticId,
        IReadOnlyDictionary<string, string> values)
    {
        builder.AddMultiLanguageProperty(
                                         idShort,
                                         langStringSetBuilder =>
                                         {
                                             foreach (var (language, text) in values)
                                             {
                                                 langStringSetBuilder.Add(language, text);
                                             }
                                         });

        var element = (MultiLanguageProperty) (submodel.SubmodelElements ?? []).Last();
        element.SemanticId = semanticId;
    }

    /// <summary>
    /// Adds a simple property to the submodel and assigns its semantic identifier.
    /// </summary>
    /// <param name="builder">The submodel builder used to add the property.</param>
    /// <param name="submodel">The submodel instance containing the elements.</param>
    /// <param name="idShort">The idShort of the property.</param>
    /// <param name="value">The property value.</param>
    /// <param name="semanticId">The semantic reference to assign.</param>
    private static void AddPropertyWithSemantic(
        SubmodelBuilder builder,
        Submodel        submodel,
        string          idShort,
        string          value,
        IReference      semanticId)
    {
        builder.AddProperty(idShort, value);

        var property = (Property) (submodel.SubmodelElements ?? [])
            .First(e => e.IdShort == idShort);

        property.SemanticId = semanticId;
    }

    /// <summary>
    /// Ensures that the <see cref="Submodel.SubmodelElements"/> list is initialized
    /// and returns it.
    /// </summary>
    /// <param name="submodel">The submodel instance.</param>
    /// <returns>
    /// The initialized collection of <see cref="ISubmodelElement"/> instances.
    /// </returns>
    private static IList<ISubmodelElement> EnsureSubmodelElements(Submodel submodel)
    {
        if (submodel.SubmodelElements is null)
        {
            submodel.SubmodelElements = new List<ISubmodelElement>();
        }

        return submodel.SubmodelElements;
    }
}