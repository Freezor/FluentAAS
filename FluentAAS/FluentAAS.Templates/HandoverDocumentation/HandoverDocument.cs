namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
/// Represents a single document entry in the Handover Documentation submodel.
/// </summary>
public sealed class HandoverDocument
{
    public string?                                                DocumentId     { get; set; }
    public string?                                                Title          { get; set; }
    public string?                                                Description    { get; set; }
    public HandoverDocumentationSemantics.HandoverLifecycleStage? LifecycleStage { get; set; }
    public HandoverDocumentationSemantics.HandoverDocumentClass?  DocumentClass  { get; set; }
    public HandoverDocumentationSemantics.HandoverDocumentFormat? Format         { get; set; }
    public string?                                                Language       { get; set; }
    public string?                                                Revision       { get; set; }
    public DateTimeOffset?                                        Date           { get; set; }

    /// <summary>
    /// File entries attached to the document (at least one is usually required by the template).
    /// </summary>
    public List<HandoverDocumentFile> Files { get; } = [];

    internal SubmodelElementCollection ToSubmodelElementCollection()
    {
        var elements = new List<ISubmodelElement>();

        if (DocumentId is not null)
        {
            elements.Add(
                         new Property(
                                      idShort: HandoverDocumentationSemantics.IdShortDocumentId,
                                      category: null,
                                      semanticId: new Reference(
                                                                    ReferenceTypes.ExternalReference,
                                                                    [
                                                                        new Key(
                                                                                KeyTypes.GlobalReference,
                                                                                HandoverDocumentationSemantics.SemanticIdDocumentId)
                                                                    ]),
                                      valueType: DataTypeDefXsd.String,
                                      value: DocumentId));
        }

        if (Title is not null)
        {
            elements.Add(
                         new MultiLanguageProperty(
                                                       idShort: HandoverDocumentationSemantics.IdShortDocumentTitle,
                                                       category: null,
                                                       semanticId: new Reference(
                                                                                     ReferenceTypes.ExternalReference,
                                                                                     [
                                                                                         new Key(
                                                                                                 KeyTypes.GlobalReference,
                                                                                                 HandoverDocumentationSemantics.SemanticIdDocumentTitle)
                                                                                     ]),
                                                       value: [new LangStringTextType(Language ?? "en", Title)]));
        }

        if (Description is not null)
        {
            elements.Add(
                         new MultiLanguageProperty(
                                                       idShort: HandoverDocumentationSemantics.IdShortDocumentDescription,
                                                       category: null,
                                                       semanticId: new Reference(
                                                                                     ReferenceTypes.ExternalReference,
                                                                                     [
                                                                                         new Key(
                                                                                                 KeyTypes.GlobalReference,
                                                                                                 HandoverDocumentationSemantics.SemanticIdDocumentDescription)
                                                                                     ]),
                                                       value: [new LangStringTextType(Language ?? "en", Description)]));
        }

        if (LifecycleStage.HasValue)
        {
            elements.Add(
                         new Property(
                                          idShort: HandoverDocumentationSemantics.IdShortDocumentLifecycleStage,
                                          category: null,
                                          semanticId: new Reference(
                                                                        ReferenceTypes.ExternalReference,
                                                                        [
                                                                            new Key(
                                                                                    KeyTypes.GlobalReference,
                                                                                    HandoverDocumentationSemantics.SemanticIdDocumentLifecycleStage)
                                                                        ]),
                                          valueType: DataTypeDefXsd.String,
                                          value: LifecycleStage.Value.ToString()));
        }

        if (DocumentClass.HasValue)
        {
            elements.Add(
                         new Property(
                                          idShort: HandoverDocumentationSemantics.IdShortDocumentClass,
                                          category: null,
                                          semanticId: new Reference(
                                                                        ReferenceTypes.ExternalReference,
                                                                        [
                                                                            new Key(
                                                                                    KeyTypes.GlobalReference,
                                                                                    HandoverDocumentationSemantics.SemanticIdDocumentClass)
                                                                        ]),
                                          valueType: DataTypeDefXsd.String,
                                          value: DocumentClass.Value.ToString()));
        }

        if (Format.HasValue)
        {
            elements.Add(
                         new Property(
                                          idShort: HandoverDocumentationSemantics.IdShortDocumentFormat,
                                          category: null,
                                          semanticId: new Reference(
                                                                        ReferenceTypes.ExternalReference,
                                                                        [
                                                                            new Key(
                                                                                    KeyTypes.GlobalReference,
                                                                                    HandoverDocumentationSemantics.SemanticIdDocumentFormat)
                                                                        ]),
                                          valueType: DataTypeDefXsd.String,
                                          value: Format.Value.ToString()));
        }

        if (Language is not null)
        {
            elements.Add(
                         new Property(
                                          idShort: HandoverDocumentationSemantics.IdShortDocumentLanguage,
                                          category: null,
                                          semanticId: new Reference(
                                                                        ReferenceTypes.ExternalReference,
                                                                        [
                                                                            new Key(
                                                                                    KeyTypes.GlobalReference,
                                                                                    HandoverDocumentationSemantics.SemanticIdDocumentLanguage)
                                                                        ]),
                                          valueType: DataTypeDefXsd.String,
                                          value: Language));
        }

        if (Revision is not null)
        {
            elements.Add(
                         new Property(
                                          idShort: HandoverDocumentationSemantics.IdShortDocumentRevision,
                                          category: null,
                                          semanticId: new Reference(
                                                                        ReferenceTypes.ExternalReference,
                                                                        [
                                                                            new Key(
                                                                                    KeyTypes.GlobalReference,
                                                                                    HandoverDocumentationSemantics.SemanticIdDocumentRevision)
                                                                        ]),
                                          valueType: DataTypeDefXsd.String,
                                          value: Revision));
        }

        if (Date.HasValue)
        {
            elements.Add(
                         new Property(
                                          idShort: HandoverDocumentationSemantics.IdShortDocumentDate,
                                          category: null,
                                          semanticId: new Reference(
                                                                        ReferenceTypes.ExternalReference,
                                                                        [
                                                                            new Key(
                                                                                    KeyTypes.GlobalReference,
                                                                                    HandoverDocumentationSemantics.SemanticIdDocumentDate)
                                                                        ]),
                                          valueType: DataTypeDefXsd.DateTime,
                                          value: Date.Value.UtcDateTime.ToString("o")));
        }

        elements.AddRange(Files.Select(file => file.ToFileElement()));

        return new SubmodelElementCollection(
                                             idShort: HandoverDocumentationSemantics.IdShortDocumentCollection,
                                             category: null,
                                             description: null,
                                             semanticId: new Reference(
                                                                       ReferenceTypes.ExternalReference,
                                                                       [
                                                                           new Key(
                                                                                   KeyTypes.GlobalReference,
                                                                                   HandoverDocumentationSemantics.SemanticIdDocument)
                                                                       ]),
                                             value: [..elements.ToArray()]);
    }
}