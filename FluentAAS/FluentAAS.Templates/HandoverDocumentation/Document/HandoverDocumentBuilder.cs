namespace FluentAAS.Templates.HandoverDocumentation.Document;

/// <summary>
///     Fluent builder for a single Handover Document item.
/// </summary>
public sealed class HandoverDocumentBuilder
{
    private readonly HandoverDocument                _document = new();
    private          HandoverDocumentVersionBuilder? _defaultVersionBuilder;

    /// <summary>
    ///     Adds a DocumentId entry (DocumentIds list). The template requires at least one.
    /// </summary>
    /// <param name="domainId">The domain identifier for the document ID.</param>
    /// <param name="identifier">The unique identifier for the document within the domain.</param>
    /// <param name="isPrimary">Indicates whether this document ID is the primary identifier. Defaults to true.</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when domainId or identifier is null.</exception>
    public HandoverDocumentBuilder AddDocumentId(string domainId, string identifier, bool? isPrimary = true)
    {
        var handoverDocumentId = new HandoverDocumentId
                 {
                     DocumentDomainId   = domainId ?? throw new ArgumentNullException(nameof(domainId)),
                     DocumentIdentifier = identifier ?? throw new ArgumentNullException(nameof(identifier)),
                     DocumentIsPrimary  = isPrimary
                 };
        handoverDocumentId.Validate();
        _document.DocumentIds.Add(handoverDocumentId);
        return this;
    }

    /// <summary>
    ///     Sets the document title for the default version. Creates a default version if none exists.
    /// </summary>
    /// <param name="title">The title of the document.</param>
    /// <param name="language">The language code for the title. Defaults to "en".</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    public HandoverDocumentBuilder WithTitle(string title, string language = "en")
    {
        EnsureDefaultVersion().WithTitle(title, language);
        return this;
    }

    /// <summary>
    /// Sets the document description for the default version. Creates a default version if none exists.
    /// </summary>
    /// <param name="description">The description of the document.</param>
    /// <param name="language">The language code for the description. Defaults to "en".</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    public HandoverDocumentBuilder WithDescription(string description, string language = "en")
    {
        EnsureDefaultVersion().WithDescription(description, language);
        return this;
    }

    /// <summary>
    /// Sets the document version number for the default version. Creates a default version if none exists.
    /// </summary>
    /// <param name="version">The version number or string of the document.</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    public HandoverDocumentBuilder WithVersion(string version)
    {
        EnsureDefaultVersion().WithVersion(version);
        return this;
    }

    /// <summary>
    /// Sets the document status for the default version. Creates a default version if none exists.
    /// </summary>
    /// <param name="statusValue">The status value of the document.</param>
    /// <param name="statusSetDateUtc">The date when the status was set, in UTC. If null, the current date is used.</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    public HandoverDocumentBuilder WithStatus(string statusValue, DateTime? statusSetDateUtc = null)
    {
        EnsureDefaultVersion().WithStatus(statusValue, statusSetDateUtc);
        return this;
    }

    /// <summary>
    /// Sets the organization information for the default version. Creates a default version if none exists.
    /// </summary>
    /// <param name="shortName">The short name of the organization.</param>
    /// <param name="officialName">The official name of the organization.</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    public HandoverDocumentBuilder WithOrganization(string shortName, string officialName)
    {
        EnsureDefaultVersion().WithOrganization(shortName, officialName);
        return this;
    }

    /// <summary>
    /// Adds a language to the supported languages list for the default version. Creates a default version if none exists.
    /// </summary>
    /// <param name="language">The language code to add to the document's supported languages.</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    public HandoverDocumentBuilder AddLanguage(string language)
    {
        EnsureDefaultVersion().AddLanguage(language);
        return this;
    }

    /// <summary>
    /// Adds a digital file to the default version. Creates a default version if none exists.
    /// </summary>
    /// <param name="path">The file path of the digital file.</param>
    /// <param name="mimeType">The MIME type of the digital file. Defaults to "application/octet-stream".</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    public HandoverDocumentBuilder AddDigitalFile(string path, string mimeType = "application/octet-stream")
    {
        EnsureDefaultVersion().AddDigitalFile(path, mimeType);
        return this;
    }

    /// <summary>
    /// Sets the preview file for the default version. Creates a default version if none exists.
    /// </summary>
    /// <param name="path">The file path of the preview file.</param>
    /// <param name="mimeType">The MIME type of the preview file. Defaults to "application/octet-stream".</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    public HandoverDocumentBuilder WithPreviewFile(string path, string mimeType = "application/octet-stream")
    {
        EnsureDefaultVersion().WithPreviewFile(path, mimeType);
        return this;
    }

    /// <summary>
    ///     Adds a classification entry to the document. At least one classification is required by the template.
    ///     Ensure you add one with ClassificationSystem = "VDI 2770 Blatt 1:2020".
    /// </summary>
    /// <param name="classId">The classification ID.</param>
    /// <param name="className">The name of the classification.</param>
    /// <param name="classificationSystem">The classification system used. Defaults to VDI 2770 Blatt 1:2020.</param>
    /// <param name="language">The language code for the classification name. Defaults to "en".</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when classId, className, or classificationSystem is null.</exception>
    public HandoverDocumentBuilder AddClassification(
        string classId,
        string className,
        string classificationSystem = HandoverDocumentationSemantics.Vdi2770ClassificationSystemName,
        string language             = "en")
    {
        var documentClassification = new HandoverDocumentClassification
                {
                    ClassId              = classId ?? throw new ArgumentNullException(nameof(classId)),
                    ClassName            = className ?? throw new ArgumentNullException(nameof(className)),
                    ClassNameLanguage    = language,
                    ClassificationSystem = classificationSystem ?? throw new ArgumentNullException(nameof(classificationSystem))
                };
        documentClassification.Validate();
        _document.DocumentClassifications.Add(documentClassification);
        return this;
    }

    /// <summary>
    ///     Adds an explicit document version with custom configuration. This allows adding multiple versions to the document.
    /// </summary>
    /// <param name="configure">A configuration action that sets up the document version using a HandoverDocumentVersionBuilder.</param>
    /// <returns>The current HandoverDocumentBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when configure is null.</exception>
    public HandoverDocumentBuilder AddDocumentVersion(Action<HandoverDocumentVersionBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var handoverDocumentVersionBuilder = new HandoverDocumentVersionBuilder();
        configure(handoverDocumentVersionBuilder);
        _document.DocumentVersions.Add(handoverDocumentVersionBuilder.Build());

        return this;
    }

    /// <summary>
    /// Builds and validates the HandoverDocument instance with all configured settings.
    /// </summary>
    /// <returns>A fully configured and validated HandoverDocument instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the document fails template validation requirements.</exception>
    internal HandoverDocument Build()
    {
        // If the user used the default version fluent methods, finalize it
        if (_defaultVersionBuilder is not null && _document.DocumentVersions.Count == 0)
            _document.DocumentVersions.Add(_defaultVersionBuilder.Build());

        // Validate template requirements
        foreach (var id in _document.DocumentIds) id.Validate();
        foreach (var documentClassification in _document.DocumentClassifications) documentClassification.Validate();
        foreach (var documentVersion in _document.DocumentVersions) documentVersion.ValidateTemplateRequirements();

        _document.ValidateTemplateRequirements();
        return _document;
    }

    /// <summary>
    /// Ensures that a default document version builder exists and returns it for configuration.
    /// </summary>
    /// <returns>The default HandoverDocumentVersionBuilder instance.</returns>
    private HandoverDocumentVersionBuilder EnsureDefaultVersion()
    {
        _defaultVersionBuilder ??= new HandoverDocumentVersionBuilder();
        return _defaultVersionBuilder;
    }
}