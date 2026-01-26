namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
/// Represents a single document entry in the Handover Documentation submodel.
/// </summary>
public sealed class HandoverDocument
{
    public List<HandoverDocumentId>             DocumentIds             { get; } = new();
    public List<HandoverDocumentClassification> DocumentClassifications { get; } = new();
    public List<HandoverDocumentVersion>        DocumentVersions        { get; } = new();

    // Optional: DocumentedEntities (not implemented here as typed items; keep future extension point)
    // public List<ReferenceElement> DocumentedEntities { get; } = new();

    internal SubmodelElementCollection ToDocumentCollection()
    {
        var children = new List<ISubmodelElement>
                       {
                           ToDocumentIdsList(),
                           ToDocumentClassificationsList(),
                           ToDocumentVersionsList(),
                       };

        return new SubmodelElementCollection(
                                             idShort: "Document", // Note: list element idShort is "Document" in many templates; keep stable.
                                             category: null,
                                             description: null,
                                             semanticId: Ref(HandoverDocumentationSemantics.SemanticIdDocument),
                                             value: children.ToArray()
                                            );
    }

    internal void ValidateTemplateRequirements()
    {
        if (DocumentIds.Count < 1)
            throw new InvalidOperationException("Each Document must contain at least one DocumentId (DocumentIds list).");

        if (DocumentClassifications.Count < 1)
            throw new InvalidOperationException("Each Document must contain at least one DocumentClassification (DocumentClassifications list).");

        if (DocumentVersions.Count < 1)
            throw new InvalidOperationException("Each Document must contain at least one DocumentVersion (DocumentVersions list).");

        // Enforce: VDI 2770 Blatt 1:2020 classification is mandatory
        if (!DocumentClassifications.Any(c => string.Equals(c.ClassificationSystem, HandoverDocumentationSemantics.Vdi2770ClassificationSystemName, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException(
                                                $"Each Document must include a classification with ClassificationSystem='{HandoverDocumentationSemantics.Vdi2770ClassificationSystemName}'.");
        }

        foreach (var v in DocumentVersions)
            v.ValidateTemplateRequirements();
    }

    private SubmodelElementList ToDocumentIdsList()
    {
        return NewList(
                       idShort: HandoverDocumentationSemantics.IdShortDocumentIds,
                       semanticId: HandoverDocumentationSemantics.SemanticIdDocumentIds,
                       listElementSemanticId: HandoverDocumentationSemantics.SemanticIdDocumentId,
                       value: DocumentIds.Select(d => d.ToCollection()).Cast<ISubmodelElement>().ToList(),
                       orderRelevant: false,
                       typeValueListElement: AasSubmodelElements.SubmodelElementCollection
                      );
    }

    private SubmodelElementList ToDocumentClassificationsList()
    {
        return NewList(
                       idShort: HandoverDocumentationSemantics.IdShortDocumentClassifications,
                       semanticId: HandoverDocumentationSemantics.SemanticIdDocumentClassifications,
                       listElementSemanticId: HandoverDocumentationSemantics.SemanticIdDocumentClassification,
                       value: DocumentClassifications.Select(c => c.ToCollection()).Cast<ISubmodelElement>().ToList(),
                       orderRelevant: false,
                       typeValueListElement: AasSubmodelElements.SubmodelElementCollection
                      );
    }

    private SubmodelElementList ToDocumentVersionsList()
    {
        return NewList(
                       idShort: HandoverDocumentationSemantics.IdShortDocumentVersions,
                       semanticId: HandoverDocumentationSemantics.SemanticIdDocumentVersions,
                       listElementSemanticId: HandoverDocumentationSemantics.SemanticIdDocumentVersion,
                       value: DocumentVersions.Select(v => v.ToCollection()).Cast<ISubmodelElement>().ToList(),
                       orderRelevant: false,
                       typeValueListElement: AasSubmodelElements.SubmodelElementCollection
                      );
    }

    // Helpers
    private static Reference Ref(string semanticId) =>
        new Reference(ReferenceTypes.ExternalReference, new[] {new Key(KeyTypes.GlobalReference, semanticId)});

    private static SubmodelElementList NewList(
        string                 idShort,
        string                 semanticId,
        string                 listElementSemanticId,
        List<ISubmodelElement> value,
        bool                   orderRelevant,
        AasSubmodelElements    typeValueListElement)
    {
        var list = new SubmodelElementList(
                                           idShort: idShort,
                                           category: null,
                                           description: null,
                                           semanticId: Ref(semanticId),
                                           value: value.ToArray()
                                          );

        list.OrderRelevant         = orderRelevant;
        list.SemanticIdListElement = Ref(listElementSemanticId);
        list.TypeValueListElement  = typeValueListElement;

        return list;
    }
}