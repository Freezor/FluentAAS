namespace FluentAAS.Templates.HandoverDocumentation.Document;

public class HandoverDocumentId
{
    public string DocumentDomainId   { get; init; } = string.Empty;
    public string DocumentIdentifier { get; init; } = string.Empty;
    public bool?  DocumentIsPrimary  { get; init; }

    internal SubmodelElementCollection ToCollection()
    {
        var elements = new List<ISubmodelElement>
                       {
                           new Property(
                                        idShort: HandoverDocumentationSemantics.IdShortDocumentDomainId,
                                        category: null,
                                        semanticId: Ref(HandoverDocumentationSemantics.SemanticIdDocumentDomainId),
                                        valueType: DataTypeDefXsd.String,
                                        value: DocumentDomainId
                                       ),
                           new Property(
                                        idShort: HandoverDocumentationSemantics.IdShortDocumentIdentifier,
                                        category: null,
                                        semanticId: Ref(HandoverDocumentationSemantics.SemanticIdDocumentIdentifier),
                                        valueType: DataTypeDefXsd.String,
                                        value: DocumentIdentifier
                                       )
                       };

        if (DocumentIsPrimary.HasValue)
        {
            elements.Add(
                         new Property(
                                      idShort: HandoverDocumentationSemantics.IdShortDocumentIsPrimary,
                                      category: null,
                                      semanticId: Ref(HandoverDocumentationSemantics.SemanticIdDocumentIsPrimary),
                                      valueType: DataTypeDefXsd.Boolean,
                                      value: DocumentIsPrimary.Value ? "true" : "false"
                                     )
                        );
        }

        return new SubmodelElementCollection(
                                             idShort: "DocumentId",
                                             category: null,
                                             description: null,
                                             semanticId: Ref(HandoverDocumentationSemantics.SemanticIdDocumentId),
                                             value: [..elements.ToArray()]
                                            );
    }

    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(DocumentDomainId))
            throw new InvalidOperationException("DocumentDomainId is required in DocumentId.");

        if (string.IsNullOrWhiteSpace(DocumentIdentifier))
            throw new InvalidOperationException("DocumentIdentifier is required in DocumentId.");
    }

    private static Reference Ref(string semanticId) =>
        new Reference(ReferenceTypes.ExternalReference, [new Key(KeyTypes.GlobalReference, semanticId)]);
}