namespace FluentAAS.Templates.HandoverDocumentation.Document;

/// <summary>
/// Fluent builder for configuring a HandoverDocumentVersion with its properties and files.
/// </summary>
public class HandoverDocumentVersionBuilder
{
    private readonly HandoverDocumentVersion _handoverDocumentVersion = new();

    /// <summary>
    /// Adds a language to the supported languages list for this document version.
    /// </summary>
    /// <param name="language">The language code to add (e.g., "en", "de", "fr").</param>
    /// <returns>The current HandoverDocumentVersionBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when language is null, empty, or whitespace.</exception>
    public HandoverDocumentVersionBuilder WithLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException("Language must not be empty.", nameof(language));
        if (!_handoverDocumentVersion.Languages.Contains(language, StringComparer.OrdinalIgnoreCase))
            _handoverDocumentVersion.Languages.Add(language);
        return this;
    }

    /// <summary>
    /// Sets the version number or identifier for this document version.
    /// </summary>
    /// <param name="version">The version string (e.g., "1.0", "2.1", "v1.5").</param>
    /// <returns>The current HandoverDocumentVersionBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when version is null.</exception>
    public HandoverDocumentVersionBuilder WithVersion(string version)
    {
        _handoverDocumentVersion.Version = version ?? throw new ArgumentNullException(nameof(version));
        return this;
    }

    /// <summary>
    /// Sets the title for this document version and automatically adds the language to the supported languages list.
    /// </summary>
    /// <param name="title">The document title text.</param>
    /// <param name="language">The language code for the title. Defaults to "en".</param>
    /// <returns>The current HandoverDocumentVersionBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when title is null.</exception>
    public HandoverDocumentVersionBuilder WithTitle(string title, string language = "en")
    {
        _handoverDocumentVersion.Title         = title ?? throw new ArgumentNullException(nameof(title));
        _handoverDocumentVersion.TitleLanguage = language;
        WithLanguage(language);
        return this;
    }

    /// <summary>
    /// Sets the description for this document version and automatically adds the language to the supported languages list.
    /// </summary>
    /// <param name="description">The document description text.</param>
    /// <param name="language">The language code for the description. Defaults to "en".</param>
    /// <returns>The current HandoverDocumentVersionBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when description is null.</exception>
    public HandoverDocumentVersionBuilder WithDescription(string description, string language = "en")
    {
        _handoverDocumentVersion.Description         = description ?? throw new ArgumentNullException(nameof(description));
        _handoverDocumentVersion.DescriptionLanguage = language;
        WithLanguage(language);
        return this;
    }

    /// <summary>
    /// Sets the subtitle for this document version and automatically adds the language to the supported languages list.
    /// </summary>
    /// <param name="subtitle">The document subtitle text.</param>
    /// <param name="language">The language code for the subtitle. Defaults to "en".</param>
    /// <returns>The current HandoverDocumentVersionBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when subtitle is null.</exception>
    public HandoverDocumentVersionBuilder WithSubtitle(string subtitle, string language = "en")
    {
        _handoverDocumentVersion.Subtitle         = subtitle ?? throw new ArgumentNullException(nameof(subtitle));
        _handoverDocumentVersion.SubtitleLanguage = language;
        WithLanguage(language);
        return this;
    }

    /// <summary>
    /// Adds a keyword to the document version's keyword collection and automatically adds the language to the supported languages list.
    /// </summary>
    /// <param name="keyword">The keyword text to add.</param>
    /// <param name="language">The language code for the keyword. Defaults to "en".</param>
    /// <returns>The current HandoverDocumentVersionBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when keyword is null, empty, or whitespace.</exception>
    public HandoverDocumentVersionBuilder AddKeyword(string keyword, string language = "en")
    {
        if (string.IsNullOrWhiteSpace(keyword)) throw new ArgumentException("Keyword must not be empty.", nameof(keyword));
        _handoverDocumentVersion.KeyWords.Add((language, keyword));
        WithLanguage(language);
        return this;
    }

    /// <summary>
    /// Sets the status information for this document version.
    /// </summary>
    /// <param name="statusValue">The status value (e.g., "Released", "Draft", "InReview").</param>
    /// <param name="statusSetDateUtc">The date when the status was set, in UTC. If null, the current date is used.</param>
    /// <returns>The current HandoverDocumentVersionBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when statusValue is null.</exception>
    public HandoverDocumentVersionBuilder WithStatus(string statusValue, DateTime? statusSetDateUtc = null)
    {
        _handoverDocumentVersion.StatusValue   = statusValue ?? throw new ArgumentNullException(nameof(statusValue));
        _handoverDocumentVersion.StatusSetDate = (statusSetDateUtc ?? DateTime.UtcNow).Date;
        return this;
    }

    /// <summary>
    /// Sets the organization information for this document version.
    /// </summary>
    /// <param name="shortName">The short name or abbreviation of the organization.</param>
    /// <param name="officialName">The full official name of the organization.</param>
    /// <returns>The current HandoverDocumentVersionBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when shortName or officialName is null.</exception>
    public HandoverDocumentVersionBuilder WithOrganization(string shortName, string officialName)
    {
        _handoverDocumentVersion.OrganizationShortName    = shortName ?? throw new ArgumentNullException(nameof(shortName));
        _handoverDocumentVersion.OrganizationOfficialName = officialName ?? throw new ArgumentNullException(nameof(officialName));
        return this;
    }

    /// <summary>
    /// Adds a digital file to the document version's file collection.
    /// </summary>
    /// <param name="path">The file path or location of the digital file.</param>
    /// <param name="mimeType">The MIME type of the digital file. Defaults to "application/octet-stream".</param>
    /// <returns>The current HandoverDocumentVersionBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when path is null, empty, or whitespace.</exception>
    public HandoverDocumentVersionBuilder AddDigitalFile(string path, string mimeType = "application/octet-stream")
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path must not be empty.", nameof(path));

        _handoverDocumentVersion.DigitalFiles.Add(
                            new HandoverDigitalFile
                            {
                                Path     = path,
                                MimeType = mimeType
                            });

        return this;
    }

    /// <summary>
    /// Sets the preview file for this document version. This replaces any existing preview file.
    /// </summary>
    /// <param name="path">The file path or location of the preview file.</param>
    /// <param name="mimeType">The MIME type of the preview file. Defaults to "application/octet-stream".</param>
    /// <returns>The current HandoverDocumentVersionBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when path is null, empty, or whitespace.</exception>
    public HandoverDocumentVersionBuilder WithPreviewFile(string path, string mimeType = "application/octet-stream")
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path must not be empty.", nameof(path));

        _handoverDocumentVersion.PreviewFile = new HandoverDigitalFile
                         {
                             Path     = path,
                             MimeType = mimeType
                         };

        return this;
    }

    /// <summary>
    /// Builds and validates the HandoverDocumentVersion instance with all configured settings.
    /// </summary>
    /// <returns>A fully configured and validated HandoverDocumentVersion instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the document version fails template validation requirements.</exception>
    internal HandoverDocumentVersion Build()
    {
        // Reasonable defaults for compliance if the user forgot:
        if (_handoverDocumentVersion.Languages.Count == 0)
            _handoverDocumentVersion.Languages.Add(_handoverDocumentVersion.TitleLanguage);

        if (string.IsNullOrWhiteSpace(_handoverDocumentVersion.StatusValue))
            _handoverDocumentVersion.StatusValue = HandoverDocumentationSemantics.StatusValues.Released;

        _handoverDocumentVersion.ValidateTemplateRequirements();
        return _handoverDocumentVersion;
    }
}