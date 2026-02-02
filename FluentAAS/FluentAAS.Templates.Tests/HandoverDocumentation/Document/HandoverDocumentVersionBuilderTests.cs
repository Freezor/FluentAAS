using AutoFixture;
using FluentAAS.Templates.HandoverDocumentation;
using FluentAAS.Templates.HandoverDocumentation.Document;
using JetBrains.Annotations;
using Shouldly;

namespace FluentAAS.Templates.Tests.HandoverDocumentation.Document;

[TestSubject(typeof(HandoverDocumentVersionBuilder))]
public class HandoverDocumentVersionBuilderTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_ShouldInitializeWithEmptyDocumentVersion()
    {
        // Arrange & Act
        var builder = new HandoverDocumentVersionBuilder();

        // Assert
        builder.ShouldNotBeNull();
    }

    [Fact]
    public void WithLanguage_WithValidLanguage_ShouldAddLanguageAndReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var language = "de";

        // Act
        var result = builder.WithLanguage(language);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Languages.ShouldContain(language);
    }

    [Fact]
    public void WithLanguage_WithMultipleLanguages_ShouldAddAllUniqueLanguages()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var languages = new[] { "en", "de", "fr" };

        // Act
        foreach (var lang in languages)
        {
            builder.WithLanguage(lang);
        }

        // Assert
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Languages.Count.ShouldBe(3);
        documentVersion.Languages.ShouldContain("en");
        documentVersion.Languages.ShouldContain("de");
        documentVersion.Languages.ShouldContain("fr");
    }

    [Fact]
    public void WithLanguage_WithDuplicateLanguage_ShouldNotAddDuplicate()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var language = "en";

        // Act
        builder.WithLanguage(language);
        builder.WithLanguage(language);

        // Assert
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Languages.Count.ShouldBe(1);
        documentVersion.Languages[0].ShouldBe(language);
    }

    [Fact]
    public void WithLanguage_WithDuplicateLanguageDifferentCase_ShouldNotAddDuplicate()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act
        builder.WithLanguage("EN");
        builder.WithLanguage("en");

        // Assert
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Languages.Count.ShouldBe(1);
        documentVersion.Languages[0].ShouldBe("EN");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void WithLanguage_WithInvalidLanguage_ShouldThrowArgumentException(string invalidLanguage)
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => builder.WithLanguage(invalidLanguage));
        exception.ParamName.ShouldBe("language");
        exception.Message.ShouldStartWith("Language must not be empty.");
    }

    [Fact]
    public void WithLanguage_WithNullLanguage_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => builder.WithLanguage(null!));
        exception.ParamName.ShouldBe("language");
    }

    [Fact]
    public void WithVersion_WithValidVersion_ShouldSetVersionAndReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var version = "2.1";

        // Act
        var result = builder.WithVersion(version);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Version.ShouldBe(version);
    }

    [Fact]
    public void WithVersion_WithNullVersion_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.WithVersion(null!));
        exception.ParamName.ShouldBe("version");
    }

    [Fact]
    public void WithTitle_WithValidTitleAndDefaultLanguage_ShouldSetTitleAndAddLanguage()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var title = "Test Document";

        // Act
        var result = builder.WithTitle(title);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Title.ShouldBe(title);
        documentVersion.TitleLanguage.ShouldBe("en");
        documentVersion.Languages.ShouldContain("en");
    }

    [Fact]
    public void WithTitle_WithValidTitleAndCustomLanguage_ShouldSetTitleAndAddLanguage()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var title = "Test Dokument";
        var language = "de";

        // Act
        var result = builder.WithTitle(title, language);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Title.ShouldBe(title);
        documentVersion.TitleLanguage.ShouldBe(language);
        documentVersion.Languages.ShouldContain(language);
    }

    [Fact]
    public void WithTitle_WithNullTitle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.WithTitle(null!));
        exception.ParamName.ShouldBe("title");
    }

    [Fact]
    public void WithDescription_WithValidDescriptionAndDefaultLanguage_ShouldSetDescriptionAndAddLanguage()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var description = "Test description";

        // Act
        var result = builder.WithDescription(description);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Description.ShouldBe(description);
        documentVersion.DescriptionLanguage.ShouldBe("en");
        documentVersion.Languages.ShouldContain("en");
    }

    [Fact]
    public void WithDescription_WithValidDescriptionAndCustomLanguage_ShouldSetDescriptionAndAddLanguage()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var description = "Test Beschreibung";
        var language = "de";

        // Act
        var result = builder.WithDescription(description, language);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Description.ShouldBe(description);
        documentVersion.DescriptionLanguage.ShouldBe(language);
        documentVersion.Languages.ShouldContain(language);
    }

    [Fact]
    public void WithDescription_WithNullDescription_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.WithDescription(null!));
        exception.ParamName.ShouldBe("description");
    }

    [Fact]
    public void WithSubtitle_WithValidSubtitleAndDefaultLanguage_ShouldSetSubtitleAndAddLanguage()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var subtitle = "Test subtitle";

        // Act
        var result = builder.WithSubtitle(subtitle);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Subtitle.ShouldBe(subtitle);
        documentVersion.SubtitleLanguage.ShouldBe("en");
        documentVersion.Languages.ShouldContain("en");
    }

    [Fact]
    public void WithSubtitle_WithValidSubtitleAndCustomLanguage_ShouldSetSubtitleAndAddLanguage()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var subtitle = "Test Untertitel";
        var language = "de";

        // Act
        var result = builder.WithSubtitle(subtitle, language);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.Subtitle.ShouldBe(subtitle);
        documentVersion.SubtitleLanguage.ShouldBe(language);
        documentVersion.Languages.ShouldContain(language);
    }

    [Fact]
    public void WithSubtitle_WithNullSubtitle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.WithSubtitle(null!));
        exception.ParamName.ShouldBe("subtitle");
    }

    [Fact]
    public void AddKeyword_WithValidKeywordAndDefaultLanguage_ShouldAddKeywordAndLanguage()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var keyword = "test";

        // Act
        var result = builder.AddKeyword(keyword);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.KeyWords.ShouldContain((kw) => kw.Language == "en" && kw.Keyword == keyword);
        documentVersion.Languages.ShouldContain("en");
    }

    [Fact]
    public void AddKeyword_WithValidKeywordAndCustomLanguage_ShouldAddKeywordAndLanguage()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var keyword = "prüfung";
        var language = "de";

        // Act
        var result = builder.AddKeyword(keyword, language);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.KeyWords.ShouldContain((kw) => kw.Language == language && kw.Keyword == keyword);
        documentVersion.Languages.ShouldContain(language);
    }

    [Fact]
    public void AddKeyword_WithMultipleKeywords_ShouldAddAllKeywords()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var keywords = new[] 
        {
            ("en", "test"),
            ("de", "prüfung"),
            ("fr", "essai")
        };

        // Act
        foreach (var (language, keyword) in keywords)
        {
            builder.AddKeyword(keyword, language);
        }

        // Assert
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.KeyWords.Count.ShouldBe(3);
        foreach (var (language, keyword) in keywords)
        {
            documentVersion.KeyWords.ShouldContain((kw) => kw.Language == language && kw.Keyword == keyword);
            documentVersion.Languages.ShouldContain(language);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void AddKeyword_WithInvalidKeyword_ShouldThrowArgumentException(string invalidKeyword)
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => builder.AddKeyword(invalidKeyword));
        exception.ParamName.ShouldBe("keyword");
        exception.Message.ShouldStartWith("Keyword must not be empty.");
    }

    [Fact]
    public void AddKeyword_WithNullKeyword_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => builder.AddKeyword(null!));
        exception.ParamName.ShouldBe("keyword");
    }

    [Fact]
    public void WithStatus_WithValidStatusAndDefaultDate_ShouldSetStatusWithCurrentDate()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var statusValue = "InProgress";
        var testDate = DateTime.UtcNow.Date;

        // Act
        var result = builder.WithStatus(statusValue);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.StatusValue.ShouldBe(statusValue);
        documentVersion.StatusSetDate.ShouldBe(testDate, TimeSpan.FromDays(1));
    }

    [Fact]
    public void WithStatus_WithValidStatusAndCustomDate_ShouldSetStatusWithSpecifiedDate()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var statusValue = "Released";
        var statusDate = new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Utc);
        var expectedDate = statusDate.Date;

        // Act
        var result = builder.WithStatus(statusValue, statusDate);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.StatusValue.ShouldBe(statusValue);
        documentVersion.StatusSetDate.ShouldBe(expectedDate);
    }

    [Fact]
    public void WithStatus_WithNullStatusValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.WithStatus(null!));
        exception.ParamName.ShouldBe("statusValue");
    }

    [Fact]
    public void WithOrganization_WithValidNames_ShouldSetOrganizationAndReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var shortName = "ACME";
        var officialName = "ACME Corporation";

        // Act
        var result = builder.WithOrganization(shortName, officialName);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.OrganizationShortName.ShouldBe(shortName);
        documentVersion.OrganizationOfficialName.ShouldBe(officialName);
    }

    [Fact]
    public void WithOrganization_WithNullShortName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.WithOrganization(null!, "Official Name"));
        exception.ParamName.ShouldBe("shortName");
    }

    [Fact]
    public void WithOrganization_WithNullOfficialName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.WithOrganization("Short", null!));
        exception.ParamName.ShouldBe("officialName");
    }

    [Fact]
    public void AddDigitalFile_WithValidPathAndDefaultMimeType_ShouldAddFileAndReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var path = "/path/to/file.pdf";

        // Act
        var result = builder.AddDigitalFile(path);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.DigitalFiles.Count.ShouldBe(1);
        documentVersion.DigitalFiles[0].Path.ShouldBe(path);
        documentVersion.DigitalFiles[0].MimeType.ShouldBe("application/octet-stream");
    }

    [Fact]
    public void AddDigitalFile_WithValidPathAndCustomMimeType_ShouldAddFileWithMimeType()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var path = "/path/to/file.pdf";
        var mimeType = "application/pdf";

        // Act
        var result = builder.AddDigitalFile(path, mimeType);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.DigitalFiles.Count.ShouldBe(1);
        documentVersion.DigitalFiles[0].Path.ShouldBe(path);
        documentVersion.DigitalFiles[0].MimeType.ShouldBe(mimeType);
    }

    [Fact]
    public void AddDigitalFile_WithMultipleFiles_ShouldAddAllFiles()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var files = new[]
        {
            ("/path/to/file1.pdf", "application/pdf"),
            ("/path/to/file2.txt", "text/plain"),
            ("/path/to/file3.doc", "application/msword")
        };

        // Act
        foreach (var (path, mimeType) in files)
        {
            builder.AddDigitalFile(path, mimeType);
        }

        // Assert
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.DigitalFiles.Count.ShouldBe(3);
        for (int i = 0; i < files.Length; i++)
        {
            documentVersion.DigitalFiles[i].Path.ShouldBe(files[i].Item1);
            documentVersion.DigitalFiles[i].MimeType.ShouldBe(files[i].Item2);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void AddDigitalFile_WithInvalidPath_ShouldThrowArgumentException(string invalidPath)
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => builder.AddDigitalFile(invalidPath));
        exception.ParamName.ShouldBe("path");
        exception.Message.ShouldStartWith("Path must not be empty.");
    }

    [Fact]
    public void AddDigitalFile_WithNullPath_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => builder.AddDigitalFile(null!));
        exception.ParamName.ShouldBe("path");
    }

    [Fact]
    public void WithPreviewFile_WithValidPathAndDefaultMimeType_ShouldSetPreviewFileAndReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var path = "/path/to/preview.jpg";

        // Act
        var result = builder.WithPreviewFile(path);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.PreviewFile.ShouldNotBeNull();
        documentVersion.PreviewFile!.Path.ShouldBe(path);
        documentVersion.PreviewFile.MimeType.ShouldBe("application/octet-stream");
    }

    [Fact]
    public void WithPreviewFile_WithValidPathAndCustomMimeType_ShouldSetPreviewFileWithMimeType()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var path = "/path/to/preview.jpg";
        var mimeType = "image/jpeg";

        // Act
        var result = builder.WithPreviewFile(path, mimeType);

        // Assert
        result.ShouldBe(builder);
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.PreviewFile.ShouldNotBeNull();
        documentVersion.PreviewFile!.Path.ShouldBe(path);
        documentVersion.PreviewFile.MimeType.ShouldBe(mimeType);
    }

    [Fact]
    public void WithPreviewFile_CalledMultipleTimes_ShouldReplaceExistingPreviewFile()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        var firstPath = "/path/to/first.jpg";
        var secondPath = "/path/to/second.jpg";

        // Act
        builder.WithPreviewFile(firstPath);
        builder.WithPreviewFile(secondPath);

        // Assert
        var documentVersion = GetBuiltVersion(builder);
        documentVersion.PreviewFile.ShouldNotBeNull();
        documentVersion.PreviewFile!.Path.ShouldBe(secondPath);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void WithPreviewFile_WithInvalidPath_ShouldThrowArgumentException(string invalidPath)
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => builder.WithPreviewFile(invalidPath));
        exception.ParamName.ShouldBe("path");
        exception.Message.ShouldStartWith("Path must not be empty.");
    }

    [Fact]
    public void WithPreviewFile_WithNullPath_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => builder.WithPreviewFile(null!));
        exception.ParamName.ShouldBe("path");
    }

    [Fact]
    public void Build_WithValidMinimalData_ShouldReturnValidDocumentVersion()
    {
        // Arrange
        var builder = CreateValidBuilder();

        // Act
        var result = builder.Build();

        // Assert
        result.ShouldNotBeNull();
        result.Languages.Count.ShouldBeGreaterThan(0);
        result.Version.ShouldNotBeNullOrEmpty();
        result.Title.ShouldNotBeNullOrEmpty();
        result.Description.ShouldNotBeNullOrEmpty();
        result.StatusValue.ShouldNotBeNullOrEmpty();
        result.OrganizationShortName.ShouldNotBeNullOrEmpty();
        result.OrganizationOfficialName.ShouldNotBeNullOrEmpty();
        result.DigitalFiles.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Build_WithNoLanguagesSet_ShouldAddTitleLanguageAsDefault()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder()
            .WithTitle("Test", "fr")
            .WithDescription("Description")
            .WithOrganization("ACME", "ACME Corp")
            .AddDigitalFile("/path/file.pdf");

        // Act
        var result = builder.Build();

        // Assert
        result.Languages.ShouldContain("fr");
        result.Languages.Count.ShouldBe(2); // "fr" from title and "en" from description
    }

    [Fact]
    public void Build_WithEmptyStatusValue_ShouldSetDefaultReleasedStatus()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder()
            .WithTitle("Test")
            .WithDescription("Description")
            .WithOrganization("ACME", "ACME Corp")
            .AddDigitalFile("/path/file.pdf");

        // Act
        var result = builder.Build();

        // Assert
        result.StatusValue.ShouldBe(HandoverDocumentationSemantics.StatusValues.Released);
    }

    [Fact]
    public void Build_WithInvalidData_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = new HandoverDocumentVersionBuilder();
        // Missing required data

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void FluentChaining_ShouldAllowMethodChaining()
    {
        // Arrange & Act
        var result = new HandoverDocumentVersionBuilder()
            .WithLanguage("en")
            .WithVersion("1.5")
            .WithTitle("Test Document")
            .WithDescription("Test Description")
            .WithSubtitle("Test Subtitle")
            .AddKeyword("test")
            .WithStatus("Released")
            .WithOrganization("ACME", "ACME Corporation")
            .AddDigitalFile("/path/file.pdf", "application/pdf")
            .WithPreviewFile("/path/preview.jpg", "image/jpeg")
            .Build();

        // Assert
        result.ShouldNotBeNull();
        result.Languages.ShouldContain("en");
        result.Version.ShouldBe("1.5");
        result.Title.ShouldBe("Test Document");
        result.Description.ShouldBe("Test Description");
        result.Subtitle.ShouldBe("Test Subtitle");
        result.KeyWords.ShouldContain(kw => kw.Keyword == "test");
        result.StatusValue.ShouldBe("Released");
        result.OrganizationShortName.ShouldBe("ACME");
        result.OrganizationOfficialName.ShouldBe("ACME Corporation");
        result.DigitalFiles.Count.ShouldBe(1);
        result.PreviewFile.ShouldNotBeNull();
    }

    [Fact]
    public void Build_WithComplexScenario_ShouldHandleAllFeatures()
    {
        var statusDate = new DateTime(2023, 5, 15);
        // Arrange
        
        var builder = new HandoverDocumentVersionBuilder()
            .WithLanguage("en")
            .WithLanguage("de")
            .WithLanguage("fr")
            .WithVersion("2.0")
            .WithTitle("Multilingual Document", "en")
            .WithDescription("Eine mehrsprachige Beschreibung", "de")
            .WithSubtitle("Sous-titre français", "fr")
            .AddKeyword("english", "en")
            .AddKeyword("deutsch", "de")
            .AddKeyword("français", "fr")
            .WithStatus("InReview", statusDate)
            .WithOrganization("INTL", "International Corporation")
            .AddDigitalFile("/docs/manual_en.pdf", "application/pdf")
            .AddDigitalFile("/docs/manual_de.pdf", "application/pdf")
            .AddDigitalFile("/docs/manual_fr.pdf", "application/pdf")
            .WithPreviewFile("/previews/cover.png", "image/png");

        // Act
        var result = builder.Build();

        // Assert
        result.Languages.Count.ShouldBe(3);
        result.Languages.ShouldContain("en");
        result.Languages.ShouldContain("de");
        result.Languages.ShouldContain("fr");
        result.Version.ShouldBe("2.0");
        result.Title.ShouldBe("Multilingual Document");
        result.TitleLanguage.ShouldBe("en");
        result.Description.ShouldBe("Eine mehrsprachige Beschreibung");
        result.DescriptionLanguage.ShouldBe("de");
        result.Subtitle.ShouldBe("Sous-titre français");
        result.SubtitleLanguage.ShouldBe("fr");
        result.KeyWords.Count.ShouldBe(3);
        result.StatusValue.ShouldBe("InReview");
        result.StatusSetDate.ShouldBe(statusDate.Date);
        result.OrganizationShortName.ShouldBe("INTL");
        result.OrganizationOfficialName.ShouldBe("International Corporation");
        result.DigitalFiles.Count.ShouldBe(3);
        result.PreviewFile.ShouldNotBeNull();
        result.PreviewFile!.Path.ShouldBe("/previews/cover.png");
        result.PreviewFile.MimeType.ShouldBe("image/png");
    }

    private HandoverDocumentVersionBuilder CreateValidBuilder()
    {
        return new HandoverDocumentVersionBuilder()
            .WithTitle(_fixture.Create<string>())
            .WithDescription(_fixture.Create<string>())
            .WithOrganization(_fixture.Create<string>(), _fixture.Create<string>())
            .AddDigitalFile(_fixture.Create<string>());
    }

    private HandoverDocumentVersion GetBuiltVersion(HandoverDocumentVersionBuilder builder)
    {
        // Create a minimal valid builder to ensure Build() succeeds
        var validBuilder = CreateValidBuilder();
        
        // This is a workaround since Build() is internal - we'll use reflection to access the internal field
        var builderType = typeof(HandoverDocumentVersionBuilder);
        var field = builderType.GetField("_handoverDocumentVersion", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        return (HandoverDocumentVersion)field!.GetValue(builder)!;
    }
}