using FluentAAS.Builder;
using static FluentAAS.Templates.DigitalNameplateSemantics;

namespace FluentAAS.Templates;

/// <summary>
/// Provides a guided fluent builder for the IDTA Digital Nameplate submodel.
/// The builder enforces mandatory core properties and attaches the submodel
/// to a given <see cref="ShellBuilder"/>.
/// </summary>
public sealed class DigitalNameplateBuilder
{
    private readonly ShellBuilder _shellBuilder;

    private string _id;
    private string _idShort;

    // Mandatory multi-language fields (language code → text)
    private readonly Dictionary<string, string> _manufacturerName               = new();
    private readonly Dictionary<string, string> _manufacturerProductDesignation = new();

    // Mandatory single-value field
    private string? _serialNumber;

    // Optional fields could be added later, e.g.:
    // private string? _hardwareVersion;
    // private string? _yearOfConstruction;

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
    internal DigitalNameplateBuilder(ShellBuilder shellBuilder, string id, string idShort)
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
    /// Builds the Digital Nameplate submodel, attaches it to the shell,
    /// and returns the parent <see cref="ShellBuilder"/>.
    /// </summary>
    /// <returns>The parent <see cref="ShellBuilder"/> for further configuration.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when mandatory fields (manufacturer name, product designation, or serial number)
    /// have not been provided.
    /// </exception>
    public ShellBuilder Build()
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

        if (missing.Any())
        {
            throw new InvalidOperationException(
                                                "Digital Nameplate is incomplete. Missing mandatory fields: " +
                                                string.Join(", ", missing));
        }

        // Use the generic submodel builder
        var submodelBuilder = new SubmodelBuilder(_shellBuilder, _id, _idShort)
            .WithSemanticId(RefFactory.GlobalConceptDescription(SubmodelDigitalNameplate));

        var submodel = submodelBuilder.BuildSubmodel();

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

        // SerialNumber
        AddPropertyWithSemantic(
                                submodelBuilder,
                                submodel,
                                "SerialNumber",
                                _serialNumber!,
                                RefFactory.GlobalConceptDescription(SerialNumber));

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
                                         idShort, ml =>
                                                  {
                                                      foreach (var (language, text) in values)
                                                      {
                                                          ml.Add(language, text);
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
}