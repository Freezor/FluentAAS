
using FluentAAS.Core.HandoverDocumentation;

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
    public List<HandoverDocumentFile> Files { get; } = new();

    internal Aas.SubmodelElementCollection ToSubmodelElementCollection()
    {
        var elements = new List<Aas.ISubmodelElement>();

        if (DocumentId is not null)
        {
            elements.Add(new Property(
                idShort: IdShort_DocumentId,
                category: null,
                kind: null,
                semanticId: new Aas.Reference(
                    Aas.ReferenceTypes.ExternalReference,
                    new[]
                    {
                        new Aas.Key(
                            Aas.KeyTypes.GlobalReference,
                            SemanticId_DocumentId)
                    }),
                valueType: Aas.DataTypeDefXsd.String,
                value: DocumentId));
        }

        if (Title is not null)
        {
            elements.Add(new Aas.MultiLanguageProperty(
                idShort: IdShort_DocumentTitle,
                category: null,
                kind: null,
                semanticId: new Aas.Reference(
                    Aas.ReferenceTypes.ExternalReference,
                    new[]
                    {
                        new Aas.Key(
                            Aas.KeyTypes.GlobalReference,
                            SemanticId_DocumentTitle)
                    }),
                value: new[]
                {
                    new Aas.LangStringTextType(Language ?? "en", Title)
                }));
        }

        if (Description is not null)
        {
            elements.Add(new Aas.MultiLanguageProperty(
                idShort: IdShort_DocumentDescription,
                category: null,
                kind: null,
                semanticId: new Aas.Reference(
                    Aas.ReferenceTypes.ExternalReference,
                    new[]
                    {
                        new Aas.Key(
                            Aas.KeyTypes.GlobalReference,
                            SemanticId_DocumentDescription)
                    }),
                value: new[]
                {
                    new Aas.LangStringTextType(Language ?? "en", Description)
                }));
        }

        if (LifecycleStage.HasValue)
        {
            elements.Add(new Aas.Property(
                idShort: IdShort_DocumentLifecycleStage,
                category: null,
                kind: null,
                semanticId: new Aas.Reference(
                    Aas.ReferenceTypes.ExternalReference,
                    new[]
                    {
                        new Aas.Key(
                            Aas.KeyTypes.GlobalReference,
                            SemanticId_DocumentLifecycleStage)
                    }),
                valueType: Aas.DataTypeDefXsd.String,
                value: LifecycleStage.Value.ToString()));
        }

        if (DocumentClass.HasValue)
        {
            elements.Add(new Aas.Property(
                idShort: IdShort_DocumentClass,
                category: null,
                kind: null,
                semanticId: new Aas.Reference(
                    Aas.ReferenceTypes.ExternalReference,
                    new[]
                    {
                        new Aas.Key(
                            Aas.KeyTypes.GlobalReference,
                            SemanticId_DocumentClass)
                    }),
                valueType: Aas.DataTypeDefXsd.String,
                value: DocumentClass.Value.ToString()));
        }

        if (Format.HasValue)
        {
            elements.Add(new Aas.Property(
                idShort: IdShort_DocumentFormat,
                category: null,
                kind: null,
                semanticId: new Aas.Reference(
                    Aas.ReferenceTypes.ExternalReference,
                    new[]
                    {
                        new Aas.Key(
                            Aas.KeyTypes.GlobalReference,
                            SemanticId_DocumentFormat)
                    }),
                valueType: Aas.DataTypeDefXsd.String,
                value: Format.Value.ToString()));
        }

        if (Language is not null)
        {
            elements.Add(new Aas.Property(
                idShort: IdShort_DocumentLanguage,
                category: null,
                kind: null,
                semanticId: new Aas.Reference(
                    Aas.ReferenceTypes.ExternalReference,
                    new[]
                    {
                        new Aas.Key(
                            Aas.KeyTypes.GlobalReference,
                            SemanticId_DocumentLanguage)
                    }),
                valueType: Aas.DataTypeDefXsd.String,
                value: Language));
        }

        if (Revision is not null)
        {
            elements.Add(new Aas.Property(
                idShort: IdShort_DocumentRevision,
                category: null,
                kind: null,
                semanticId: new Aas.Reference(
                    Aas.ReferenceTypes.ExternalReference,
                    new[]
                    {
                        new Aas.Key(
                            Aas.KeyTypes.GlobalReference,
                            SemanticId_DocumentRevision)
                    }),
                valueType: Aas.DataTypeDefXsd.String,
                value: Revision));
        }

        if (Date.HasValue)
        {
            elements.Add(new Aas.Property(
                idShort: IdShort_DocumentDate,
                category: null,
                kind: null,
                semanticId: new Aas.Reference(
                    Aas.ReferenceTypes.ExternalReference,
                    new[]
                    {
                        new Aas.Key(
                            Aas.KeyTypes.GlobalReference,
                            SemanticId_DocumentDate)
                    }),
                valueType: Aas.DataTypeDefXsd.DateTime,
                value: Date.Value.UtcDateTime.ToString("o")));
        }

        foreach (var file in Files)
        {
            elements.Add(file.ToFileElement());
        }

        return new Aas.SubmodelElementCollection(
            idShort: IdShort_DocumentCollection,
            category: null,
            description: null,
            kind: null,
            semanticId: new Aas.Reference(
                Aas.ReferenceTypes.ExternalReference,
                new[]
                {
                    new Aas.Key(
                        Aas.KeyTypes.GlobalReference,
                        SemanticId_Document)
                }),
            value: elements.ToArray());
    }
}