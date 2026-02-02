namespace FluentAAS.Templates.HandoverDocumentation;

public static partial class HandoverDocumentationSemantics
{
    /// <summary>
    ///     Document formats from the template (examples – adjust to spec).
    ///     Usually captured as an enumeration of controlled vocab items.
    /// </summary>
    public enum HandoverDocumentFormat
    {
        Pdf,
        Docx,
        Xlsx,
        Image,
        Video,
        Xml,
        Json,
        Other
    }
}