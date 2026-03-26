using System.Globalization;
using AasCore.Aas3_0;
using AutoFixture;
using FluentAAS.Templates.HandoverDocumentation;
using FluentAAS.Templates.HandoverDocumentation.Document;
using JetBrains.Annotations;
using Shouldly;
using File = System.IO.File;

namespace FluentAAS.Templates.Tests.HandoverDocumentation.Document;

[TestSubject(typeof(HandoverDocumentVersion))]
public class HandoverDocumentVersionTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var version = new HandoverDocumentVersion();

        // Assert
        version.Languages.ShouldNotBeNull();
        version.Languages.Count.ShouldBe(0);
        version.Version.ShouldBe("1.0");
        version.Title.ShouldBe(string.Empty);
        version.TitleLanguage.ShouldBe("en");
        version.Description.ShouldBe(string.Empty);
        version.DescriptionLanguage.ShouldBe("en");
        version.StatusSetDate.ShouldBe(DateTime.UtcNow.Date, TimeSpan.FromSeconds(1));
        version.StatusValue.ShouldBe(HandoverDocumentationSemantics.StatusValues.Released);
        version.OrganizationShortName.ShouldBe(string.Empty);
        version.OrganizationOfficialName.ShouldBe(string.Empty);
        version.Subtitle.ShouldBeNull();
        version.SubtitleLanguage.ShouldBeNull();
        version.KeyWords.ShouldNotBeNull();
        version.KeyWords.Count.ShouldBe(0);
        version.DigitalFiles.ShouldNotBeNull();
        version.DigitalFiles.Count.ShouldBe(0);
        version.PreviewFile.ShouldBeNull();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var version = new HandoverDocumentVersion();
        var title = _fixture.Create<string>();
        var description = _fixture.Create<string>();
        var versionNumber = _fixture.Create<string>();
        var statusValue = _fixture.Create<string>();
        var orgShortName = _fixture.Create<string>();
        var orgOfficialName = _fixture.Create<string>();
        var statusDate = _fixture.Create<DateTime>();
        var subtitle = _fixture.Create<string>();
        var subtitleLanguage = "de";

        // Act
        version.Title = title;
        version.Description = description;
        version.Version = versionNumber;
        version.StatusValue = statusValue;
        version.OrganizationShortName = orgShortName;
        version.OrganizationOfficialName = orgOfficialName;
        version.StatusSetDate = statusDate;
        version.Subtitle = subtitle;
        version.SubtitleLanguage = subtitleLanguage;

        // Assert
        version.Title.ShouldBe(title);
        version.Description.ShouldBe(description);
        version.Version.ShouldBe(versionNumber);
        version.StatusValue.ShouldBe(statusValue);
        version.OrganizationShortName.ShouldBe(orgShortName);
        version.OrganizationOfficialName.ShouldBe(orgOfficialName);
        version.StatusSetDate.ShouldBe(statusDate);
        version.Subtitle.ShouldBe(subtitle);
        version.SubtitleLanguage.ShouldBe(subtitleLanguage);
    }

    [Fact]
    public void Languages_ShouldBeModifiable()
    {
        // Arrange
        var version = new HandoverDocumentVersion();
        var languages = new[] { "en", "de", "fr" };

        // Act
        foreach (var lang in languages)
        {
            version.Languages.Add(lang);
        }

        // Assert
        version.Languages.Count.ShouldBe(3);
        version.Languages.ShouldContain("en");
        version.Languages.ShouldContain("de");
        version.Languages.ShouldContain("fr");

    }

    [Fact]
    public void KeyWords_ShouldBeModifiable()
    {
        // Arrange
        var version = new HandoverDocumentVersion();
        var keywords = new[]
        {
            ("en", "keyword1"),
            ("de", "schlüsselwort"),
            ("fr", "mot-clé")
        };

        // Act
        foreach (var (language, keyword) in keywords)
        {
            version.KeyWords.Add((language, keyword));
        }

        // Assert
        version.KeyWords.Count.ShouldBe(3);
        version.KeyWords.ShouldBe(keywords);
    }

    [Fact]
    public void DigitalFiles_ShouldBeModifiable()
    {
        // Arrange
        var version = new HandoverDocumentVersion();
        var digitalFile = CreateValidDigitalFile();

        // Act
        version.DigitalFiles.Add(digitalFile);

        // Assert
        version.DigitalFiles.Count.ShouldBe(1);
        version.DigitalFiles[0].ShouldBe(digitalFile);
    }

    [Fact]
    public void ToCollection_WithMinimalValidData_ShouldReturnSubmodelElementCollection()
    {
        // Arrange
        var version = CreateValidDocumentVersion();

        // Act
        var result = version.ToCollection();

        // Assert
        result.ShouldNotBeNull();
        result.IdShort.ShouldBe("DocumentVersion");
        result.Category.ShouldBeNull();
        result.Description.ShouldBeNull();
        result.SemanticId.ShouldNotBeNull();
        result.Value!.Count.ShouldBeGreaterThan(8); // At least: Languages, Version, Title, Description, StatusSetDate, StatusValue, OrgShortName, OrgOfficialName, DigitalFiles
    }

    [Fact]
    public void ToCollection_ShouldContainLanguageList()
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.Languages.Clear();
        version.Languages.AddRange(new[] { "en", "de" });

        // Act
        var result = version.ToCollection();

        // Assert
        var languageElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortLanguage);
        languageElement.ShouldNotBeNull();
        languageElement.ShouldBeOfType<SubmodelElementList>();

        var languageList = (SubmodelElementList)languageElement!;
        languageList.Value!.Count.ShouldBe(2);
        languageList.TypeValueListElement.ShouldBe(AasSubmodelElements.Property);
        languageList.OrderRelevant.ShouldBe(false);
    }

    [Fact]
    public void ToCollection_ShouldContainVersionProperty()
    {
        // Arrange
        var versionNumber = "2.1";
        var version = CreateValidDocumentVersion();
        version.Version = versionNumber;

        // Act
        var result = version.ToCollection();

        // Assert
        var versionElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortVersion);
        versionElement.ShouldNotBeNull();
        versionElement.ShouldBeOfType<Property>();

        var versionProperty = (Property)versionElement!;
        versionProperty.Value.ShouldBe(versionNumber);
        versionProperty.ValueType.ShouldBe(DataTypeDefXsd.String);
    }

    [Fact]
    public void ToCollection_ShouldContainTitleMultiLanguageProperty()
    {
        // Arrange
        var title = "Test Document";
        var titleLanguage = "en";
        var version = CreateValidDocumentVersion();
        version.Title = title;
        version.TitleLanguage = titleLanguage;

        // Act
        var result = version.ToCollection();

        // Assert
        var titleElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortTitle);
        titleElement.ShouldNotBeNull();
        titleElement.ShouldBeOfType<MultiLanguageProperty>();

        var titleProperty = (MultiLanguageProperty)titleElement!;
        titleProperty.Value!.Count.ShouldBe(1);
        titleProperty.Value[0].Language.ShouldBe(titleLanguage);
        titleProperty.Value[0].Text.ShouldBe(title);
    }

    [Fact]
    public void ToCollection_ShouldContainDescriptionMultiLanguageProperty()
    {
        // Arrange
        var description = "Test Description";
        var descriptionLanguage = "fr";
        var version = CreateValidDocumentVersion();
        version.Description = description;
        version.DescriptionLanguage = descriptionLanguage;

        // Act
        var result = version.ToCollection();

        // Assert
        var descriptionElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDescription);
        descriptionElement.ShouldNotBeNull();
        descriptionElement.ShouldBeOfType<MultiLanguageProperty>();

        var descriptionProperty = (MultiLanguageProperty)descriptionElement!;
        descriptionProperty.Value!.Count.ShouldBe(1);
        descriptionProperty.Value[0].Language.ShouldBe(descriptionLanguage);
        descriptionProperty.Value[0].Text.ShouldBe(description);
    }

    [Fact]
    public void ToCollection_ShouldContainStatusSetDateProperty()
    {
        // Arrange
        var statusDate = new DateTime(2023, 12, 15);
        var version = CreateValidDocumentVersion();
        version.StatusSetDate = statusDate;

        // Act
        var result = version.ToCollection();

        // Assert
        var dateElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortStatusSetDate);
        dateElement.ShouldNotBeNull();
        dateElement.ShouldBeOfType<Property>();

        var dateProperty = (Property)dateElement!;
        dateProperty.Value.ShouldBe("2023-12-15");
        dateProperty.ValueType.ShouldBe(DataTypeDefXsd.Date);
    }

    [Fact]
    public void ToCollection_ShouldContainStatusValueProperty()
    {
        // Arrange
        var statusValue = "Draft";
        var version = CreateValidDocumentVersion();
        version.StatusValue = statusValue;

        // Act
        var result = version.ToCollection();

        // Assert
        var statusElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortStatusValue);
        statusElement.ShouldNotBeNull();
        statusElement.ShouldBeOfType<Property>();

        var statusProperty = (Property)statusElement!;
        statusProperty.Value.ShouldBe(statusValue);
        statusProperty.ValueType.ShouldBe(DataTypeDefXsd.String);
    }

    [Fact]
    public void ToCollection_ShouldContainOrganizationProperties()
    {
        // Arrange
        var shortName = "ACME";
        var officialName = "ACME Corporation";
        var version = CreateValidDocumentVersion();
        version.OrganizationShortName = shortName;
        version.OrganizationOfficialName = officialName;

        // Act
        var result = version.ToCollection();

        // Assert
        var shortNameElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortOrganizationShortName);
        shortNameElement.ShouldNotBeNull();
        shortNameElement.ShouldBeOfType<Property>();
        ((Property)shortNameElement!).Value.ShouldBe(shortName);

        var officialNameElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortOrganizationOfficialName);
        officialNameElement.ShouldNotBeNull();
        officialNameElement.ShouldBeOfType<Property>();
        ((Property)officialNameElement!).Value.ShouldBe(officialName);
    }

    [Fact]
    public void ToCollection_ShouldContainDigitalFilesList()
    {
        // Arrange
        var version = CreateValidDocumentVersion();

        // Act
        var result = version.ToCollection();

        // Assert
        var filesElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDigitalFiles);
        filesElement.ShouldNotBeNull();
        filesElement.ShouldBeOfType<SubmodelElementList>();

        var filesList = (SubmodelElementList)filesElement!;
        filesList.TypeValueListElement.ShouldBe(AasSubmodelElements.File);
        filesList.OrderRelevant.ShouldBe(false);
        filesList.Value!.Count.ShouldBe(version.DigitalFiles.Count);
    }

    [Fact]
    public void ToCollection_WithSubtitle_ShouldIncludeSubtitleProperty()
    {
        // Arrange
        var subtitle = "Test Subtitle";
        var subtitleLanguage = "de";
        var version = CreateValidDocumentVersion();
        version.Subtitle = subtitle;
        version.SubtitleLanguage = subtitleLanguage;

        // Act
        var result = version.ToCollection();

        // Assert
        var subtitleElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortSubtitle);
        subtitleElement.ShouldNotBeNull();
        subtitleElement.ShouldBeOfType<MultiLanguageProperty>();

        var subtitleProperty = (MultiLanguageProperty)subtitleElement!;
        subtitleProperty.Value!.Count.ShouldBe(1);
        subtitleProperty.Value[0].Language.ShouldBe(subtitleLanguage);
        subtitleProperty.Value[0].Text.ShouldBe(subtitle);
    }

    [Fact]
    public void ToCollection_WithSubtitleButNoLanguage_ShouldUseTitleLanguage()
    {
        // Arrange
        var subtitle = "Test Subtitle";
        var titleLanguage = "es";
        var version = CreateValidDocumentVersion();
        version.Subtitle = subtitle;
        version.SubtitleLanguage = null;
        version.TitleLanguage = titleLanguage;

        // Act
        var result = version.ToCollection();

        // Assert
        var subtitleElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortSubtitle);
        subtitleElement.ShouldNotBeNull();
        subtitleElement.ShouldBeOfType<MultiLanguageProperty>();

        var subtitleProperty = (MultiLanguageProperty)subtitleElement!;
        subtitleProperty.Value![0].Language.ShouldBe(titleLanguage);
    }

    [Fact]
    public void ToCollection_WithoutSubtitle_ShouldNotIncludeSubtitleProperty()
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.Subtitle = null;

        // Act
        var result = version.ToCollection();

        // Assert
        var subtitleElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortSubtitle);
        subtitleElement.ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void ToCollection_WithEmptySubtitle_ShouldNotIncludeSubtitleProperty(string emptySubtitle)
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.Subtitle = emptySubtitle;

        // Act
        var result = version.ToCollection();

        // Assert
        var subtitleElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortSubtitle);
        subtitleElement.ShouldBeNull();
    }

    [Fact]
    public void ToCollection_WithKeyWords_ShouldIncludeKeyWordsProperty()
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.KeyWords.Add(("en", "test"));
        version.KeyWords.Add(("de", "prüfung"));

        // Act
        var result = version.ToCollection();

        // Assert
        var keyWordsElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortKeyWords);
        keyWordsElement.ShouldNotBeNull();
        keyWordsElement.ShouldBeOfType<MultiLanguageProperty>();

        var keyWordsProperty = (MultiLanguageProperty)keyWordsElement!;
        keyWordsProperty.Value!.Count.ShouldBe(2);
        keyWordsProperty.Value.ShouldContain(ls => ls.Language == "en" && ls.Text == "test");
        keyWordsProperty.Value.ShouldContain(ls => ls.Language == "de" && ls.Text == "prüfung");
    }

    [Fact]
    public void ToCollection_WithoutKeyWords_ShouldNotIncludeKeyWordsProperty()
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.KeyWords.Clear();

        // Act
        var result = version.ToCollection();

        // Assert
        var keyWordsElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortKeyWords);
        keyWordsElement.ShouldBeNull();
    }

    [Fact]
    public void ToCollection_WithPreviewFile_ShouldIncludePreviewFileElement()
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.PreviewFile = CreateValidDigitalFile();

        // Act
        var result = version.ToCollection();

        // Assert
        var previewElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortPreviewFile);
        previewElement.ShouldNotBeNull();
        previewElement.ShouldBeOfType<AasCore.Aas3_0.File>();
    }

    [Fact]
    public void ToCollection_WithoutPreviewFile_ShouldNotIncludePreviewFileElement()
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.PreviewFile = null;

        // Act
        var result = version.ToCollection();

        // Assert
        var previewElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortPreviewFile);
        previewElement.ShouldBeNull();
    }

    [Fact]
    public void ValidateTemplateRequirements_WithValidData_ShouldNotThrow()
    {
        // Arrange
        var version = CreateValidDocumentVersion();

        // Act & Assert
        Should.NotThrow(() => version.ValidateTemplateRequirements());
    }

    [Fact]
    public void ValidateTemplateRequirements_WithEmptyLanguages_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.Languages.Clear();

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => version.ValidateTemplateRequirements());
        exception.Message.ShouldBe("DocumentVersion must contain at least one Language.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void ValidateTemplateRequirements_WithInvalidVersion_ShouldThrowInvalidOperationException(string invalidVersion)
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.Version = invalidVersion;

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => version.ValidateTemplateRequirements());
        exception.Message.ShouldBe("DocumentVersion.Version is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void ValidateTemplateRequirements_WithInvalidTitle_ShouldThrowInvalidOperationException(string invalidTitle)
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.Title = invalidTitle;

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => version.ValidateTemplateRequirements());
        exception.Message.ShouldBe("DocumentVersion.Title is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void ValidateTemplateRequirements_WithInvalidDescription_ShouldThrowInvalidOperationException(string invalidDescription)
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.Description = invalidDescription;

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => version.ValidateTemplateRequirements());
        exception.Message.ShouldBe("DocumentVersion.Description is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void ValidateTemplateRequirements_WithInvalidStatusValue_ShouldThrowInvalidOperationException(string invalidStatusValue)
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.StatusValue = invalidStatusValue;

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => version.ValidateTemplateRequirements());
        exception.Message.ShouldBe("DocumentVersion.StatusValue is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void ValidateTemplateRequirements_WithInvalidOrganizationShortName_ShouldThrowInvalidOperationException(string invalidShortName)
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.OrganizationShortName = invalidShortName;

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => version.ValidateTemplateRequirements());
        exception.Message.ShouldBe("DocumentVersion.OrganizationShortName is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void ValidateTemplateRequirements_WithInvalidOrganizationOfficialName_ShouldThrowInvalidOperationException(string invalidOfficialName)
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.OrganizationOfficialName = invalidOfficialName;

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => version.ValidateTemplateRequirements());
        exception.Message.ShouldBe("DocumentVersion.OrganizationOfficialName is required.");
    }

    [Fact]
    public void ValidateTemplateRequirements_WithEmptyDigitalFiles_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var version = CreateValidDocumentVersion();
        version.DigitalFiles.Clear();

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => version.ValidateTemplateRequirements());
        exception.Message.ShouldBe("DocumentVersion must contain at least one DigitalFile (DigitalFiles list).");
    }

    [Fact]
    public void StatusSetDate_ShouldFormatCorrectlyInCollection()
    {
        // Arrange
        var testDate = new DateTime(2023, 5, 15, 14, 30, 0, DateTimeKind.Utc);
        var version = CreateValidDocumentVersion();
        version.StatusSetDate = testDate;

        // Act
        var result = version.ToCollection();

        // Assert
        var dateElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortStatusSetDate);
        var dateProperty = (Property)dateElement!;
        dateProperty.Value.ShouldBe("2023-05-15");
    }

    [Theory]
    [InlineData("en")]
    [InlineData("de")]
    [InlineData("fr")]
    [InlineData("es")]
    public void ToCollection_WithDifferentTitleLanguages_ShouldPreserveLanguage(string language)
    {
        // Arrange
        var title = "Test Title";
        var version = CreateValidDocumentVersion();
        version.Title = title;
        version.TitleLanguage = language;

        // Act
        var result = version.ToCollection();

        // Assert
        var titleElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortTitle);
        var titleProperty = (MultiLanguageProperty)titleElement!;
        titleProperty.Value![0].Language.ShouldBe(language);
        titleProperty.Value[0].Text.ShouldBe(title);
    }

    [Fact]
    public void ToCollection_ShouldSetCorrectSemanticIds()
    {
        // Arrange
        var version = CreateValidDocumentVersion(includeOptional: true);

        // Act
        var result = version.ToCollection();

        // Assert
        result.SemanticId.ShouldNotBeNull();
        
        // Check that all elements have semantic IDs
        foreach (var element in result.Value!)
        {
            if (element.IdShort != HandoverDocumentationSemantics.IdShortDigitalFiles && 
                element.IdShort != HandoverDocumentationSemantics.IdShortLanguage)
            {
                element.SemanticId.ShouldNotBeNull($"Element {element.IdShort} should have a semantic ID");
            }
        }
    }

    private HandoverDocumentVersion CreateValidDocumentVersion(bool includeOptional = false)
    {
        var version = new HandoverDocumentVersion
        {
            Title = _fixture.Create<string>(),
            Description = _fixture.Create<string>(),
            Version = _fixture.Create<string>(),
            StatusValue = _fixture.Create<string>(),
            OrganizationShortName = _fixture.Create<string>(),
            OrganizationOfficialName = _fixture.Create<string>(),
            StatusSetDate = DateTime.UtcNow.Date
        };

        version.Languages.Add("en");
        version.DigitalFiles.Add(CreateValidDigitalFile());

        if (includeOptional)
        {
            version.Subtitle = _fixture.Create<string>();
            version.SubtitleLanguage = "de";
            version.KeyWords.Add(("en", "test"));
            version.PreviewFile = CreateValidDigitalFile();
        }

        return version;
    }

    private HandoverDigitalFile CreateValidDigitalFile()
    {
        return new HandoverDigitalFile
        {
            Path = _fixture.Create<string>(),
            MimeType = "application/pdf"
        };
    }
}