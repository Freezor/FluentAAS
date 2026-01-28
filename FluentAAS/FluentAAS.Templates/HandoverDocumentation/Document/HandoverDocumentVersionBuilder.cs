namespace FluentAAS.Templates.HandoverDocumentation.Document;

public class HandoverDocumentVersionBuilder
{
    private readonly HandoverDocumentVersion _handoverDocumentVersion = new();

    public HandoverDocumentVersionBuilder AddLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException("Language must not be empty.", nameof(language));
        if (!_handoverDocumentVersion.Languages.Contains(language, StringComparer.OrdinalIgnoreCase))
            _handoverDocumentVersion.Languages.Add(language);
        return this;
    }

    public HandoverDocumentVersionBuilder WithVersion(string version)
    {
        _handoverDocumentVersion.Version = version ?? throw new ArgumentNullException(nameof(version));
        return this;
    }

    public HandoverDocumentVersionBuilder WithTitle(string title, string language = "en")
    {
        _handoverDocumentVersion.Title         = title ?? throw new ArgumentNullException(nameof(title));
        _handoverDocumentVersion.TitleLanguage = language;
        AddLanguage(language);
        return this;
    }

    public HandoverDocumentVersionBuilder WithDescription(string description, string language = "en")
    {
        _handoverDocumentVersion.Description         = description ?? throw new ArgumentNullException(nameof(description));
        _handoverDocumentVersion.DescriptionLanguage = language;
        AddLanguage(language);
        return this;
    }

    public HandoverDocumentVersionBuilder WithSubtitle(string subtitle, string language = "en")
    {
        _handoverDocumentVersion.Subtitle         = subtitle ?? throw new ArgumentNullException(nameof(subtitle));
        _handoverDocumentVersion.SubtitleLanguage = language;
        AddLanguage(language);
        return this;
    }

    public HandoverDocumentVersionBuilder AddKeyword(string keyword, string language = "en")
    {
        if (string.IsNullOrWhiteSpace(keyword)) throw new ArgumentException("Keyword must not be empty.", nameof(keyword));
        _handoverDocumentVersion.KeyWords.Add((language, keyword));
        AddLanguage(language);
        return this;
    }

    public HandoverDocumentVersionBuilder WithStatus(string statusValue, DateTime? statusSetDateUtc = null)
    {
        _handoverDocumentVersion.StatusValue   = statusValue ?? throw new ArgumentNullException(nameof(statusValue));
        _handoverDocumentVersion.StatusSetDate = (statusSetDateUtc ?? DateTime.UtcNow).Date;
        return this;
    }

    public HandoverDocumentVersionBuilder WithOrganization(string shortName, string officialName)
    {
        _handoverDocumentVersion.OrganizationShortName    = shortName ?? throw new ArgumentNullException(nameof(shortName));
        _handoverDocumentVersion.OrganizationOfficialName = officialName ?? throw new ArgumentNullException(nameof(officialName));
        return this;
    }

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