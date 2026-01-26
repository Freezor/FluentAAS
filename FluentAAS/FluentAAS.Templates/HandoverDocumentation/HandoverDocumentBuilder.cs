namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
/// Fluent builder for a single Handover Document item.
/// </summary>
public sealed class HandoverDocumentBuilder
{
    private readonly HandoverDocument                _doc = new();
    private          HandoverDocumentVersionBuilder? _defaultVersionBuilder;

    /// <summary>
    /// Adds a DocumentId entry (DocumentIds list). The template requires at least one.
    /// </summary>
    public HandoverDocumentBuilder AddDocumentId(string domainId, string identifier, bool? isPrimary = true)
    {
        var id = new HandoverDocumentId
                 {
                     DocumentDomainId   = domainId ?? throw new ArgumentNullException(nameof(domainId)),
                     DocumentIdentifier = identifier ?? throw new ArgumentNullException(nameof(identifier)),
                     DocumentIsPrimary  = isPrimary
                 };
        id.Validate();
        _doc.DocumentIds.Add(id);
        return this;
    }

    /// <summary>
    /// Convenience: sets/ensures a default DocumentVersion and sets Title there (per template).
    /// </summary>
    public HandoverDocumentBuilder WithTitle(string title, string language = "en")
    {
        EnsureDefaultVersion().WithTitle(title, language);
        return this;
    }

    public HandoverDocumentBuilder WithDescription(string description, string language = "en")
    {
        EnsureDefaultVersion().WithDescription(description, language);
        return this;
    }

    public HandoverDocumentBuilder WithVersion(string version)
    {
        EnsureDefaultVersion().WithVersion(version);
        return this;
    }

    public HandoverDocumentBuilder WithStatus(string statusValue, DateTime? statusSetDateUtc = null)
    {
        EnsureDefaultVersion().WithStatus(statusValue, statusSetDateUtc);
        return this;
    }

    public HandoverDocumentBuilder WithOrganization(string shortName, string officialName)
    {
        EnsureDefaultVersion().WithOrganization(shortName, officialName);
        return this;
    }

    public HandoverDocumentBuilder AddLanguage(string language)
    {
        EnsureDefaultVersion().AddLanguage(language);
        return this;
    }

    public HandoverDocumentBuilder AddDigitalFile(string path, string mimeType = "application/octet-stream")
    {
        EnsureDefaultVersion().AddDigitalFile(path, mimeType);
        return this;
    }

    public HandoverDocumentBuilder WithPreviewFile(string path, string mimeType = "application/octet-stream")
    {
        EnsureDefaultVersion().WithPreviewFile(path, mimeType);
        return this;
    }

    /// <summary>
    /// Adds a classification entry (DocumentClassifications list). At least one is required.
    /// Ensure you add one with ClassificationSystem = "VDI 2770 Blatt 1:2020".
    /// </summary>
    public HandoverDocumentBuilder AddClassification(
        string classId,
        string className,
        string classificationSystem = HandoverDocumentationSemantics.Vdi2770ClassificationSystemName,
        string language             = "en")
    {
        var c = new HandoverDocumentClassification
                {
                    ClassId              = classId ?? throw new ArgumentNullException(nameof(classId)),
                    ClassName            = className ?? throw new ArgumentNullException(nameof(className)),
                    ClassNameLanguage    = language,
                    ClassificationSystem = classificationSystem ?? throw new ArgumentNullException(nameof(classificationSystem))
                };
        c.Validate();
        _doc.DocumentClassifications.Add(c);
        return this;
    }

    /// <summary>
    /// Advanced: add an explicit version with nested configuration.
    /// (You can use this to add multiple versions.)
    /// </summary>
    public HandoverDocumentBuilder AddDocumentVersion(Action<HandoverDocumentVersionBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var vb = new HandoverDocumentVersionBuilder();
        configure(vb);
        _doc.DocumentVersions.Add(vb.Build());

        return this;
    }

    internal HandoverDocument Build()
    {
        // If user used the default version fluent methods, finalize it
        if (_defaultVersionBuilder is not null && !_doc.DocumentVersions.Any())
            _doc.DocumentVersions.Add(_defaultVersionBuilder.Build());

        // Validate template requirements
        foreach (var id in _doc.DocumentIds) id.Validate();
        foreach (var c in _doc.DocumentClassifications) c.Validate();
        foreach (var v in _doc.DocumentVersions) v.ValidateTemplateRequirements();

        _doc.ValidateTemplateRequirements();
        return _doc;
    }

    private HandoverDocumentVersionBuilder EnsureDefaultVersion()
    {
        _defaultVersionBuilder ??= new HandoverDocumentVersionBuilder();
        return _defaultVersionBuilder;
    }
}