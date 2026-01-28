namespace FluentAAS.Templates.HandoverDocumentation.Document;

public class HandoverDigitalFile
{
    public string MimeType { get; init; } = "application/octet-stream";
    public string Path     { get; init; } = string.Empty;

    internal Aas.File ToFileElement(string idShort, string? semanticId)
    {
        return new Aas.File(
                            idShort: idShort,
                            category: null,
                            semanticId: semanticId is null ? null : Ref(semanticId),
                            contentType: MimeType,
                            value: Path
                           );
    }

    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(Path))
            throw new InvalidOperationException("DigitalFile.Path is required.");

        if (string.IsNullOrWhiteSpace(MimeType))
            throw new InvalidOperationException("DigitalFile.MimeType is required.");
    }

    private static Reference Ref(string semanticId) =>
        new Reference(ReferenceTypes.ExternalReference, [new Key(KeyTypes.GlobalReference, semanticId)]);
}