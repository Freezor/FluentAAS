namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
/// Represents a file attached to a document.
/// </summary>
public sealed class HandoverDocumentFile
{
    public string MimeType { get; init; } = "application/octet-stream";
    public string Path     { get; init; } = string.Empty;

    internal Aas.File ToFileElement()
    {
        return new Aas.File(
                            idShort: HandoverDocumentationSemantics.IdShortDocumentFile,
                            category: null,
                            semanticId: new Reference(
                                                          ReferenceTypes.ExternalReference,
                                                          [
                                                              new Key(
                                                                          KeyTypes.GlobalReference,
                                                                          HandoverDocumentationSemantics.SemanticIdDocumentFile)
                                                          ]),
                            contentType: MimeType,
                            value: Path);
    }
}