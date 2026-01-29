using FluentAAS.Builder;
using FluentAAS.Templates.HandoverDocumentation.Document;

namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
///     Fluent builder for the IDTA 02004-2-0 "Handover Documentation" submodel.
/// </summary>
public sealed class HandoverDocumentationSubmodelBuilder
{
    private readonly List<LangStringTextType> _description = [];

    private readonly List<HandoverDocument> _documents = [];

    private readonly IShellBuilder _shellBuilder;
    // Optional root list (Entities)
    private readonly List<IReferable> _entities = [];
    private readonly string           _id;
    private readonly string           _idShort = HandoverDocumentationSemantics.SubmodelIdShort;

    private string? _category;

    /// <summary>
    ///     Initializes a new instance of the HandoverDocumentationSubmodelBuilder with the specified ID and optional IdShort.
    /// </summary>
    /// <param name="shellBuilder">The parent <see cref="ShellBuilder"/> to which the submodel will be attached.</param>
    /// <param name="id">The unique identifier for the submodel.</param>
    /// <param name="idShort">The optional short identifier for the submodel. If not provided, uses the default from semantics.</param>
    /// <exception cref="ArgumentNullException">Thrown when id is null.</exception>
    public HandoverDocumentationSubmodelBuilder(IShellBuilder shellBuilder,string id, string? idShort = null)
    {
        _shellBuilder = shellBuilder ?? throw new ArgumentNullException(nameof(shellBuilder));
        
        _id           = id ?? throw new ArgumentNullException(nameof(id));
        if (!string.IsNullOrWhiteSpace(idShort))
            _idShort = idShort;
    }

    /// <summary>
    ///     Sets the category for the handover documentation submodel.
    /// </summary>
    /// <param name="category">The category to assign to the submodel.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public HandoverDocumentationSubmodelBuilder WithCategory(string category)
    {
        _category = category;
        return this;
    }

    /// <summary>
    ///     Adds a description in the specified language to the handover documentation submodel.
    /// </summary>
    /// <param name="language">The language code for the description (e.g., "en", "de").</param>
    /// <param name="text">The description text in the specified language.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when language or text is null, empty, or whitespace.</exception>
    public HandoverDocumentationSubmodelBuilder WithDescription(string language, string text)
    {
        if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException("Language must not be empty.", nameof(language));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Text must not be empty.", nameof(text));

        _description.Add(new LangStringTextType(language, text));
        return this;
    }

    /// <summary>
    ///     Adds a handover document to the submodel using a configuration action to set up the document properties.
    /// </summary>
    /// <param name="configure">An action that configures the HandoverDocumentBuilder to define the document structure and content.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when configure action is null.</exception>
    public HandoverDocumentationSubmodelBuilder AddDocument(Action<HandoverDocumentBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new HandoverDocumentBuilder();
        configure(builder);

        _documents.Add(builder.Build());
        return this;
    }

    /// <summary>
    ///     Optional: add an entity entry to the root "Entities" list.
    ///     This template references VDI 2770 entities; model it properly once you have an entity representation.
    /// </summary>
    /// <param name="entity">The entity to add to the entities collection.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
    public HandoverDocumentationSubmodelBuilder AddEntity(IReferable entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _entities.Add(entity);
        return this;
    }

    /// <summary>
    ///     Constructs and returns the complete HandoverDocumentationV20 submodel with all configured documents, descriptions, and settings.
    /// </summary>
    /// <returns>A fully configured HandoverDocumentationV20 submodel instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the submodel configuration is invalid or incomplete.</exception>
    public IShellBuilder BuildHandoverDocumentation()
    {
        Validate();

        var submodel = new Submodel(
                                    _id,
                                    idShort: _idShort,
                                    category: _category,
                                    description: _description.Count > 0 ? _description.Cast<ILangStringTextType>().ToList() : [],
                                    semanticId: Ref(HandoverDocumentationSemantics.SubmodelSemanticId),
                                    kind: ModellingKind.Instance,
                                    qualifiers: null,
                                    administration: null,
                                    submodelElements: BuildSubmodelElements()
                                   );

        _shellBuilder.AddSubmodelReference(submodel);

        return _shellBuilder;
    }

    private List<ISubmodelElement> BuildSubmodelElements()
    {
        var elements = new List<ISubmodelElement>();

        // Documents: MUST be SubmodelElementList (Table 3)
        var documentsList = NewList(
                                    HandoverDocumentationSemantics.IdShortDocuments,
                                    HandoverDocumentationSemantics.SemanticIdDocuments,
                                    HandoverDocumentationSemantics.SemanticIdDocument,
                                    _documents.Select(d => d.ToDocumentCollection()).Cast<ISubmodelElement>().ToList(),
                                    false,
                                    AasSubmodelElements.SubmodelElementCollection
                                   );

        elements.Add(documentsList);

        // Entities: optional, only add if the user provided (Table 4)
        // NOTE: The spec expects SubmodelElementList with Entity SMC elements.
        // We keep this intentionally minimal; implement proper entity mapping later.
        // If (_entities.Count > 0) { ... }

        return elements;
    }

    private void Validate()
    {
        if (_documents.Count == 0)
            throw new InvalidOperationException("Handover Documentation submodel must contain at least one Document.");

        // Template requires: DocumentIds + DocumentClassifications + DocumentVersions each at least one
        foreach (var doc in _documents)
            doc.ValidateTemplateRequirements();
    }

    private static Reference Ref(string semanticId)
    {
        return new Reference(ReferenceTypes.ExternalReference, [new Key(KeyTypes.GlobalReference, semanticId)]);
    }

    private static SubmodelElementList NewList(
        string                 idShort,
        string                 semanticId,
        string                 listElementSemanticId,
        List<ISubmodelElement> value,
        bool                   orderRelevant,
        AasSubmodelElements    typeValueListElement)
    {
        // Depending on your AAS SDK, this might be different:
        // - constructor params
        // - property names (OrderRelevant, SemanticIdListElement, TypeValueListElement)
        var list = new SubmodelElementList(
                                           AasSubmodelElements.SubmodelElement,
                                           idShort: idShort,
                                           category: null,
                                           description: null,
                                           semanticId: Ref(semanticId),
                                           value: [..value.ToArray()]
                                          )
                   {
                       // List metadata (critical for compliance)
                       OrderRelevant         = orderRelevant,
                       SemanticIdListElement = Ref(listElementSemanticId),
                       TypeValueListElement  = typeValueListElement
                   };

        return list;
    }
}