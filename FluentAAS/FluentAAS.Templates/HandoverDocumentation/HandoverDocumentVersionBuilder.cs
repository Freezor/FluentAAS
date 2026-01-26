namespace FluentAAS.Templates.HandoverDocumentation;

public class HandoverDocumentVersionBuilder
{
    
    private readonly HandoverDocumentVersion _v = new();

    public HandoverDocumentVersionBuilder AddLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException("Language must not be empty.", nameof(language));
        if (!_v.Languages.Contains(language, StringComparer.OrdinalIgnoreCase))
            _v.Languages.Add(language);
        return this;
    }

    public HandoverDocumentVersionBuilder WithVersion(string version)
    {
        _v.Version = version ?? throw new ArgumentNullException(nameof(version));
        return this;
    }

    public HandoverDocumentVersionBuilder WithTitle(string title, string language = "en")
    {
        _v.Title = title ?? throw new ArgumentNullException(nameof(title));
        _v.TitleLanguage = language;
        AddLanguage(language);
        return this;
    }

    public HandoverDocumentVersionBuilder WithDescription(string description, string language = "en")
    {
        _v.Description = description ?? throw new ArgumentNullException(nameof(description));
        _v.DescriptionLanguage = language;
        AddLanguage(language);
        return this;
    }

    public HandoverDocumentVersionBuilder WithSubtitle(string subtitle, string language = "en")
    {
        _v.Subtitle = subtitle ?? throw new ArgumentNullException(nameof(subtitle));
        _v.SubtitleLanguage = language;
        AddLanguage(language);
        return this;
    }

    public HandoverDocumentVersionBuilder AddKeyword(string keyword, string language = "en")
    {
        if (string.IsNullOrWhiteSpace(keyword)) throw new ArgumentException("Keyword must not be empty.", nameof(keyword));
        _v.KeyWords.Add((language, keyword));
        AddLanguage(language);
        return this;
    }

    public HandoverDocumentVersionBuilder WithStatus(string statusValue, DateTime? statusSetDateUtc = null)
    {
        _v.StatusValue = statusValue ?? throw new ArgumentNullException(nameof(statusValue));
        _v.StatusSetDate = (statusSetDateUtc ?? DateTime.UtcNow).Date;
        return this;
    }

    public HandoverDocumentVersionBuilder WithOrganization(string shortName, string officialName)
    {
        _v.OrganizationShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
        _v.OrganizationOfficialName = officialName ?? throw new ArgumentNullException(nameof(officialName));
        return this;
    }

    public HandoverDocumentVersionBuilder AddDigitalFile(string path, string mimeType = "application/octet-stream")
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path must not be empty.", nameof(path));

        _v.DigitalFiles.Add(new HandoverDigitalFile
        {
            Path = path,
            MimeType = mimeType
        });

        return this;
    }

    public HandoverDocumentVersionBuilder WithPreviewFile(string path, string mimeType = "application/octet-stream")
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path must not be empty.", nameof(path));

        _v.PreviewFile = new HandoverDigitalFile
        {
            Path = path,
            MimeType = mimeType
        };

        return this;
    }

    internal HandoverDocumentVersion Build()
    {
        // Reasonable defaults for compliance if user forgot:
        if (_v.Languages.Count == 0)
            _v.Languages.Add(_v.TitleLanguage ?? "en");

        if (string.IsNullOrWhiteSpace(_v.StatusValue))
            _v.StatusValue = HandoverDocumentationSemantics.StatusValues.Released;

        _v.ValidateTemplateRequirements();
        return _v;
    }
}