namespace FluentAAS.Templates.HandoverDocumentation.Document;

public class HandoverDocumentClassification
{
    public string ClassId              { get; init; } = string.Empty;
    public string ClassName            { get; init; } = string.Empty;
    public string ClassNameLanguage    { get; init; } = "en";
    public string ClassificationSystem { get; init; } = HandoverDocumentationSemantics.Vdi2770ClassificationSystemName;

    internal SubmodelElementCollection ToCollection()
    {
        var elements = new List<ISubmodelElement>
                       {
                           new Property(
                                        idShort: HandoverDocumentationSemantics.IdShortClassId,
                                        category: null,
                                        semanticId: Ref(HandoverDocumentationSemantics.SemanticIdClassId),
                                        valueType: DataTypeDefXsd.String,
                                        value: ClassId
                                       ),
                           new MultiLanguageProperty(
                                                     idShort: HandoverDocumentationSemantics.IdShortClassName,
                                                     category: null,
                                                     semanticId: Ref(HandoverDocumentationSemantics.SemanticIdClassName),
                                                     value: [new LangStringTextType(ClassNameLanguage, ClassName)]
                                                    ),
                           new Property(
                                        idShort: HandoverDocumentationSemantics.IdShortClassificationSystem,
                                        category: null,
                                        semanticId: Ref(HandoverDocumentationSemantics.SemanticIdClassificationSystem),
                                        valueType: DataTypeDefXsd.String,
                                        value: ClassificationSystem
                                       )
                       };

        return new SubmodelElementCollection(
                                             idShort: "DocumentClassification",
                                             category: null,
                                             description: null,
                                             semanticId: Ref(HandoverDocumentationSemantics.SemanticIdDocumentClassification),
                                             value: [..elements.ToArray()]
                                            );
    }

    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(ClassId))
            throw new InvalidOperationException("ClassId is required in DocumentClassification.");

        if (string.IsNullOrWhiteSpace(ClassName))
            throw new InvalidOperationException("ClassName is required in DocumentClassification.");

        if (string.IsNullOrWhiteSpace(ClassificationSystem))
            throw new InvalidOperationException("ClassificationSystem is required in DocumentClassification.");
    }

    private static Reference Ref(string semanticId) =>
        new Reference(ReferenceTypes.ExternalReference, [new Key(KeyTypes.GlobalReference, semanticId)]);
}