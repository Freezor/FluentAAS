
using FluentAAS.Core.HandoverDocumentation;

namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
/// Fluent builder for a single Handover Document item.
/// </summary>
public sealed class HandoverDocumentBuilder
{
    private readonly HandoverDocument _document = new();
    private readonly List<HandoverDocumentFile> _files = new();

    public HandoverDocumentBuilder WithDocumentId(string documentId)
    {
        _document.DocumentId = documentId;
        return this;
    }

    public HandoverDocumentBuilder WithTitle(string title, string language = "en")
    {
        _document.Title = title;
        _document.Language ??= language;
        return this;
    }

    public HandoverDocumentBuilder WithDescription(string description, string? language = null)
    {
        _document.Description = description;
        if (!string.IsNullOrWhiteSpace(language))
        {
            _document.Language = language;
        }

        return this;
    }

    public HandoverDocumentBuilder WithLifecycleStage(HandoverDocumentationSemantics.HandoverLifecycleStage stage)
    {
        _document.LifecycleStage = stage;
        return this;
    }

    public HandoverDocumentBuilder WithDocumentClass(HandoverDocumentationSemantics.HandoverDocumentClass documentClass)
    {
        _document.DocumentClass = documentClass;
        return this;
    }

    public HandoverDocumentBuilder WithFormat(HandoverDocumentationSemantics.HandoverDocumentFormat format)
    {
        _document.Format = format;
        return this;
    }

    public HandoverDocumentBuilder WithLanguage(string language)
    {
        _document.Language = language;
        return this;
    }

    public HandoverDocumentBuilder WithRevision(string revision)
    {
        _document.Revision = revision;
        return this;
    }

    public HandoverDocumentBuilder WithDate(DateTimeOffset date)
    {
        _document.Date = date;
        return this;
    }

    /// <summary>
    /// Add a file reference to the document.
    /// </summary>
    /// <param name="path">
    /// AAS File.value – typically a relative or absolute path, URI, etc.
    /// </param>
    /// <param name="fileName">
    /// Optional. If omitted, the file name can be derived from the path.
    /// </param>
    /// <param name="mimeType">
    /// MIME type, e.g. "application/pdf".
    /// </param>
    public HandoverDocumentBuilder AddFile(
        string path,
        string? fileName = null,
        string mimeType = "application/octet-stream")
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File path must not be null or empty.", nameof(path));

        var file = new HandoverDocumentFile
        {
            Path = path,
            FileName = fileName ?? System.IO.Path.GetFileName(path),
            MimeType = mimeType
        };

        _files.Add(file);
        return this;
    }

    internal HandoverDocument Build()
    {
        Validate();

        _document.Files.Clear();
        _document.Files.AddRange(_files);

        return _document;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(_document.DocumentId))
        {
            throw new InvalidOperationException("DocumentId is required for a handover document.");
        }

        if (string.IsNullOrWhiteSpace(_document.Title))
        {
            throw new InvalidOperationException("Title is required for a handover document.");
        }

        if (_files.Count == 0)
        {
            throw new InvalidOperationException(
                "A handover document must define at least one file (AddFile).");
        }

        // Additional validations can be added here:
        // - check allowed MIME types (according to template)
        // - check lifecycle stage is set, if template requires it
    }
}
