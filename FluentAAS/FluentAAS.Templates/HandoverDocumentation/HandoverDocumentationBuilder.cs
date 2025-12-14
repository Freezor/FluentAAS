namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
/// Fluent builder for the IDTA 02004-2-0 "Handover Documentation" submodel.
/// </summary>
public sealed class HandoverDocumentationSubmodelBuilder
{
    private readonly string _id;
    private readonly string? _idShort;
    private readonly List<HandoverDocument> _documents = [];

    private string? _category;
    private string? _descriptionLanguage;
    private string? _descriptionText;

    /// <summary>
    /// Create a new builder instance.
    /// </summary>
    /// <param name="id">Submodel identification (e.g. "urn:submodel:handover:123").</param>
    /// <param name="idShort">Optional idShort; if null, an idShort is derived from the template name.</param>
    public HandoverDocumentationSubmodelBuilder(string id, string? idShort = null)
    {
        _id = id ?? throw new ArgumentNullException(nameof(id));
        _idShort = idShort ?? "HandoverDocumentation";
    }

    /// <summary>
    /// Set a category for the submodel (optional).
    /// </summary>
    public HandoverDocumentationSubmodelBuilder WithCategory(string category)
    {
        _category = category;
        return this;
    }

    /// <summary>
    /// Set a multi-language description for the submodel (optional).
    /// </summary>
    public HandoverDocumentationSubmodelBuilder WithDescription(
        string language,
        string text)
    {
        _descriptionLanguage = language;
        _descriptionText = text;
        return this;
    }

    /// <summary>
    /// Add a document using a nested document builder.
    /// </summary>
    public HandoverDocumentationSubmodelBuilder AddDocument(
        Action<HandoverDocumentBuilder> configure)
    {
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        var builder = new HandoverDocumentBuilder();
        configure(builder);
        var document = builder.Build();

        _documents.Add(document);

        return this;
    }

    /// <summary>
    /// Build the strongly typed Handover Documentation submodel.
    /// Performs inline validation of required fields.
    /// </summary>
    public HandoverDocumentationV20 Build()
    {
        Validate();

        var semanticId = new Reference(
            ReferenceTypes.ExternalReference,
            [new Key(KeyTypes.GlobalReference, HandoverDocumentationSemantics.SubmodelSemanticId)]);

        List<ILangStringTextType> description = _descriptionText is not null && _descriptionLanguage is not null
                                                    ?
                                                    [
                                                        new LangStringTextType(_descriptionLanguage, _descriptionText)
                                                    ]
                                                    : [];

        var submodelElements = new List<ISubmodelElement>();

        // Documents collection

        var documentsCollection = new SubmodelElementCollection(
                                                                    idShort: HandoverDocumentationSemantics.IdShortDocuments,
                                                                    category: null,
                                                                    description: null,
                                                                    semanticId: new Reference(
                                                                                                  ReferenceTypes.ExternalReference,
                                                                                                  [
                                                                                                      new Key(
                                                                                                                  KeyTypes.GlobalReference,
                                                                                                                  HandoverDocumentationSemantics.SemanticIdDocuments)
                                                                                                  ]),
                                                                    value: [.._documents.Select(doc => doc.ToSubmodelElementCollection()).Cast<ISubmodelElement>().ToArray()]);

        submodelElements.Add(documentsCollection);

        var submodel = new Submodel(
            id: _id,
            idShort: _idShort,
            category: _category,
            description: description,
            semanticId: semanticId,
            kind: ModellingKind.Instance,
            qualifiers: null,
            administration: null,
            submodelElements: [..submodelElements.ToArray()]);

        return new HandoverDocumentationV20(submodel);
    }

    private void Validate()
    {
        if (_documents.Count == 0)
        {
            throw new InvalidOperationException(
                "Handover Documentation submodel must contain at least one document.");
        }
    }
}