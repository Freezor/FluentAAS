namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
/// Fluent builder for the IDTA 02004-2-0 "Handover Documentation" submodel.
/// </summary>
public sealed class HandoverDocumentationSubmodelBuilder
{
    private readonly string _id;
    private readonly string _idShort = HandoverDocumentationSemantics.SubmodelIdShort;

    private          string?                  _category;
    private readonly List<LangStringTextType> _description = [];

    private readonly List<HandoverDocument> _documents = [];

    // Optional root list (Entities)
    private readonly List<IReferable> _entities = []; // Placeholder type. Replace with your entity type if you model it.

    public HandoverDocumentationSubmodelBuilder(string id, string? idShort = null)
    {
        _id = id ?? throw new ArgumentNullException(nameof(id));
        if (!string.IsNullOrWhiteSpace(idShort))
            _idShort = idShort!;
    }

    public HandoverDocumentationSubmodelBuilder WithCategory(string category)
    {
        _category = category;
        return this;
    }

    public HandoverDocumentationSubmodelBuilder WithDescription(string language, string text)
    {
        if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException("Language must not be empty.", nameof(language));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Text must not be empty.", nameof(text));

        _description.Add(new LangStringTextType(language, text));
        return this;
    }

    public HandoverDocumentationSubmodelBuilder AddDocument(Action<HandoverDocumentBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new HandoverDocumentBuilder();
        configure(builder);

        _documents.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Optional: add an entity entry to the root "Entities" list.
    /// This template references VDI 2770 entities; model it properly once you have an entity representation.
    /// </summary>
    public HandoverDocumentationSubmodelBuilder AddEntity(IReferable entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _entities.Add(entity);
        return this;
    }

    public HandoverDocumentationV20 Build()
    {
        Validate();

        var submodel = new Submodel(
                                    id: _id,
                                    idShort: _idShort,
                                    category: _category,
                                    description: _description.Count > 0 ? _description.Cast<ILangStringTextType>().ToList() : new List<ILangStringTextType>(),
                                    semanticId: Ref(HandoverDocumentationSemantics.SubmodelSemanticId),
                                    kind: ModellingKind.Instance,
                                    qualifiers: null,
                                    administration: null,
                                    submodelElements: BuildSubmodelElements()
                                   );

        return new HandoverDocumentationV20(submodel);
    }

    private List<ISubmodelElement> BuildSubmodelElements()
    {
        var elements = new List<ISubmodelElement>();

        // Documents: MUST be SubmodelElementList (Table 3)
        var documentsList = NewList(
                                    idShort: HandoverDocumentationSemantics.IdShortDocuments,
                                    semanticId: HandoverDocumentationSemantics.SemanticIdDocuments,
                                    listElementSemanticId: HandoverDocumentationSemantics.SemanticIdDocument,
                                    value: _documents.Select(d => d.ToDocumentCollection()).Cast<ISubmodelElement>().ToList(),
                                    orderRelevant: false,
                                    typeValueListElement: AasSubmodelElements.SubmodelElementCollection
                                   );

        elements.Add(documentsList);

        // Entities: optional, only add if user provided (Table 4)
        // NOTE: The spec expects SubmodelElementList with Entity SMC elements.
        // We keep this intentionally minimal; implement proper entity mapping later.
        // if (_entities.Count > 0) { ... }

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

    private static Reference Ref(string semanticId) =>
        new Reference(ReferenceTypes.ExternalReference, [new Key(KeyTypes.GlobalReference, semanticId)]);

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
                                           idShort: idShort,
                                           category: null,
                                           description: null,
                                           semanticId: Ref(semanticId),
                                           value: value.ToArray()
                                          );

        // List metadata (critical for compliance)
        list.OrderRelevant         = orderRelevant;
        list.SemanticIdListElement = Ref(listElementSemanticId);
        list.TypeValueListElement  = typeValueListElement;

        return list;
    }
}