namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
/// Represents a file attached to a document.
/// </summary>
public sealed class HandoverDocumentFile
{
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = "application/octet-stream";
    public string Path     { get; set; } = string.Empty;

    internal Aas.File ToFileElement()
    {
        return new Aas.File(
                            idShort: HandoverDocumentationSemantics.IdShort_DocumentFile,
                            category: null,
                            semanticId: new Aas.Reference(
                                                          Aas.ReferenceTypes.ExternalReference,
                                                          [
                                                              new Aas.Key(
                                                                          Aas.KeyTypes.GlobalReference,
                                                                          HandoverDocumentationSemantics.SemanticId_DocumentFile)
                                                          ]),
                            contentType: MimeType,
                            value: Path);
    }
}