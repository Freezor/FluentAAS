using AutoFixture;
using FluentAAS.Templates.HandoverDocumentation;
using FluentAAS.Templates.HandoverDocumentation.Document;
using JetBrains.Annotations;
using Shouldly;

namespace FluentAAS.Templates.Tests.HandoverDocumentation.Document;

[TestSubject(typeof(HandoverDocumentBuilder))]
public class HandoverDocumentBuilderTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_ShouldCreateInstanceWithEmptyDocument()
    {
        // Arrange & Act
        var builder = new HandoverDocumentBuilder();

        // Assert
        builder.ShouldNotBeNull();
    }

    [Fact]
    public void AddDocumentId_WithValidParameters_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var domainId = _fixture.Create<string>();
        var identifier = _fixture.Create<string>();

        // Act
        var result = builder.AddDocumentId(domainId, identifier);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void AddDocumentId_WithNullDomainId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var identifier = _fixture.Create<string>();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.AddDocumentId(null!, identifier));
        exception.ParamName.ShouldBe("domainId");
    }

    [Fact]
    public void AddDocumentId_WithNullIdentifier_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var domainId = _fixture.Create<string>();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.AddDocumentId(domainId, null!));
        exception.ParamName.ShouldBe("identifier");
    }

    [Fact]
    public void AddDocumentId_WithIsPrimaryTrue_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var domainId = _fixture.Create<string>();
        var identifier = _fixture.Create<string>();

        // Act
        var result = builder.AddDocumentId(domainId, identifier, true);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void AddDocumentId_WithIsPrimaryFalse_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var domainId = _fixture.Create<string>();
        var identifier = _fixture.Create<string>();

        // Act
        var result = builder.AddDocumentId(domainId, identifier, false);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithTitle_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var title = _fixture.Create<string>();

        // Act
        var result = builder.WithTitle(title);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithTitle_WithCustomLanguage_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var title = _fixture.Create<string>();
        var language = "de";

        // Act
        var result = builder.WithTitle(title, language);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithDescription_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var description = _fixture.Create<string>();

        // Act
        var result = builder.WithDescription(description);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithDescription_WithCustomLanguage_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var description = _fixture.Create<string>();
        var language = "fr";

        // Act
        var result = builder.WithDescription(description, language);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithVersion_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var version = _fixture.Create<string>();

        // Act
        var result = builder.WithVersion(version);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithStatus_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var status = _fixture.Create<string>();

        // Act
        var result = builder.WithStatus(status);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithStatus_WithCustomDate_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var status = _fixture.Create<string>();
        var date = _fixture.Create<DateTime>();

        // Act
        var result = builder.WithStatus(status, date);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithOrganization_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var shortName = _fixture.Create<string>();
        var officialName = _fixture.Create<string>();

        // Act
        var result = builder.WithOrganization(shortName, officialName);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void AddLanguage_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var language = "es";

        // Act
        var result = builder.AddLanguage(language);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void AddDigitalFile_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var path = _fixture.Create<string>();

        // Act
        var result = builder.AddDigitalFile(path);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void AddDigitalFile_WithCustomMimeType_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var path = _fixture.Create<string>();
        var mimeType = "application/pdf";

        // Act
        var result = builder.AddDigitalFile(path, mimeType);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithPreviewFile_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var path = _fixture.Create<string>();

        // Act
        var result = builder.WithPreviewFile(path);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithPreviewFile_WithCustomMimeType_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var path = _fixture.Create<string>();
        var mimeType = "image/jpeg";

        // Act
        var result = builder.WithPreviewFile(path, mimeType);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithDocumentClassification_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var classId = _fixture.Create<string>();
        var className = _fixture.Create<string>();

        // Act
        var result = builder.WithDocumentClassification(classId, className);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithDocumentClassification_WithCustomParameters_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var classId = _fixture.Create<string>();
        var className = _fixture.Create<string>();
        var classificationSystem = "CustomSystem";
        var language = "de";

        // Act
        var result = builder.WithDocumentClassification(classId, className, classificationSystem, language);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void WithDocumentClassification_WithNullClassId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var className = _fixture.Create<string>();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.WithDocumentClassification(null!, className));
        exception.ParamName.ShouldBe("classId");
    }

    [Fact]
    public void WithDocumentClassification_WithNullClassName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var classId = _fixture.Create<string>();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.WithDocumentClassification(classId, null!));
        exception.ParamName.ShouldBe("className");
    }

    [Fact]
    public void WithDocumentClassification_WithNullClassificationSystem_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var classId = _fixture.Create<string>();
        var className = _fixture.Create<string>();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.WithDocumentClassification(classId, className, null!));
        exception.ParamName.ShouldBe("classificationSystem");
    }

    [Fact]
    public void AddDocumentVersion_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();

        // Act
        var result = builder.AddDocumentVersion(versionBuilder =>
        {
            versionBuilder.WithLanguage("en");
            versionBuilder.WithTitle("Title");
            versionBuilder.WithDescription("none");
            versionBuilder.WithOrganization("short", "long");
            versionBuilder.AddDigitalFile("path/to/file.pdf");
        });

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void AddDocumentVersion_WithNullConfigure_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => builder.AddDocumentVersion(null!));
        exception.ParamName.ShouldBe("configure");
    }

    [Fact]
    public void Build_WithExplicitDocumentVersion_ShouldReturnHandoverDocument()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var domainId = _fixture.Create<string>();
        var identifier = _fixture.Create<string>();
        var classId = _fixture.Create<string>();
        var className = _fixture.Create<string>();

        builder.AddDocumentId(domainId, identifier)
               .WithDocumentClassification(classId, className)
               .AddDocumentVersion(versionBuilder =>
               {
                   versionBuilder.WithLanguage("en");
                   versionBuilder.WithTitle("Title");
                   versionBuilder.WithDescription("none");
                   versionBuilder.WithOrganization("short", "long");
                   versionBuilder.AddDigitalFile("path/to/file.pdf");
               });

        // Act
        var result = builder.Build();

        // Assert
        result.ShouldNotBeNull();
        result.DocumentIds.Count.ShouldBe(1);
        result.DocumentClassifications.Count.ShouldBe(1);
        result.DocumentVersions.Count.ShouldBe(1);
    }

    [Fact]
    public void Build_WithMixedUsage_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var domainId = _fixture.Create<string>();
        var identifier = _fixture.Create<string>();
        var classId = _fixture.Create<string>();
        var className = _fixture.Create<string>();
        var title = _fixture.Create<string>();

        builder.AddDocumentId(domainId, identifier)
               .WithDocumentClassification(classId, className)
               .WithTitle(title) // This creates a default version
               .AddDocumentVersion(versionBuilder => // This adds an explicit version
               {
                   versionBuilder.WithLanguage("en");
                   versionBuilder.WithTitle("Title");
                   versionBuilder.WithDescription("none");
                   versionBuilder.WithOrganization("short", "long");
                   versionBuilder.AddDigitalFile("path/to/file.pdf");
               });

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => builder.Build());
        exception.Message.ShouldContain("Mixed usage detected");
        exception.Message.ShouldContain("default version fluent methods");
        exception.Message.ShouldContain("AddDocumentVersion");
    }

    [Fact]
    public void Build_WithoutDocumentIds_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var classId = _fixture.Create<string>();
        var className = _fixture.Create<string>();

        builder.WithDocumentClassification(classId, className)
               .WithTitle(_fixture.Create<string>())
               .WithDescription(_fixture.Create<string>())
               .AddLanguage("en");

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => builder.Build());
        exception.Message.ShouldNotBe("DocumentVersion.Description is required.");
    }

    [Fact]
    public void Build_WithoutDocumentClassifications_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var domainId = _fixture.Create<string>();
        var identifier = _fixture.Create<string>();

        builder.AddDocumentId(domainId, identifier)
               .WithTitle(_fixture.Create<string>())
               .WithDescription(_fixture.Create<string>())
               .AddLanguage("en");

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => builder.Build());
        exception.Message.ShouldNotBe("DocumentVersion.Description is required.");
    }

    [Fact]
    public void Build_WithoutVdi2770Classification_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var domainId = _fixture.Create<string>();
        var identifier = _fixture.Create<string>();
        var classId = _fixture.Create<string>();
        var className = _fixture.Create<string>();

        builder.AddDocumentId(domainId, identifier)
               .WithDocumentClassification(classId, className, "SomeOtherSystem")
               .WithTitle(_fixture.Create<string>())
               .WithDescription(_fixture.Create<string>())
               .AddLanguage("en");

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => builder.Build());
        exception.Message.ShouldNotBe("DocumentVersion.Description is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddDocumentId_WithInvalidDomainId_ShouldThrowInvalidOperationException(string invalidDomainId)
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var identifier = _fixture.Create<string>();

        // Act & Assert
        // The validation happens immediately in AddDocumentId, not during Build
        var exception = Should.Throw<InvalidOperationException>(() => builder.AddDocumentId(invalidDomainId, identifier));
        exception.Message.ShouldBe("DocumentDomainId is required in DocumentId.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddDocumentId_WithInvalidIdentifier_ShouldThrowInvalidOperationException(string invalidIdentifier)
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var domainId = _fixture.Create<string>();

        // Act & Assert
        // The validation happens immediately in AddDocumentId, not during Build
        var exception = Should.Throw<InvalidOperationException>(() => builder.AddDocumentId(domainId, invalidIdentifier));
        exception.Message.ShouldBe("DocumentIdentifier is required in DocumentId.");
    }

    [Fact]
    public void FluentChaining_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var builder = new HandoverDocumentBuilder();
        var result = builder
            .AddDocumentId(_fixture.Create<string>(), _fixture.Create<string>())
            .WithDocumentClassification(_fixture.Create<string>(), _fixture.Create<string>())
            .WithTitle(_fixture.Create<string>())
            .WithDescription(_fixture.Create<string>())
            .WithVersion(_fixture.Create<string>())
            .WithStatus(_fixture.Create<string>())
            .WithOrganization(_fixture.Create<string>(), _fixture.Create<string>())
            .AddLanguage("en")
            .AddDigitalFile(_fixture.Create<string>())
            .WithPreviewFile(_fixture.Create<string>());

        // Assert
        result.ShouldBe(builder);
        Should.NotThrow(() => builder.Build());
    }

    [Fact]
    public void MultipleDocumentIds_ShouldBeSupported()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var domainId1 = _fixture.Create<string>();
        var identifier1 = _fixture.Create<string>();
        var domainId2 = _fixture.Create<string>();
        var identifier2 = _fixture.Create<string>();

        builder.AddDocumentId(domainId1, identifier1)
               .AddDocumentId(domainId2, identifier2)
               .WithDocumentClassification(_fixture.Create<string>(), _fixture.Create<string>())
               .WithTitle(_fixture.Create<string>())
               .WithDescription("None")
               .WithTitle("Untitled")
               .AddDigitalFile("path/to/file.pdf")
               .WithOrganization("-", "-")
               .AddLanguage("en");

        // Act
        var result = builder.Build();

        // Assert
        result.DocumentIds.Count.ShouldBe(2);
    }

    [Fact]
    public void MultipleClassifications_ShouldBeSupported()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();
        var classId1 = _fixture.Create<string>();
        var className1 = _fixture.Create<string>();
        var classId2 = _fixture.Create<string>();
        var className2 = _fixture.Create<string>();

        builder.AddDocumentId(_fixture.Create<string>(), _fixture.Create<string>())
               .WithDocumentClassification(classId1, className1) // VDI 2770 by default
               .WithDocumentClassification(classId2, className2, "CustomSystem")
               .WithTitle(_fixture.Create<string>())
               .AddLanguage("en");

        // Act
        var result = builder.WithTitle("Untitled")
                              .WithDescription("None")
                              .WithOrganization("-","-")
                              .AddDigitalFile("path/to/file.pdf").Build();

        // Assert
        result.DocumentClassifications.Count.ShouldBe(2);
    }

    [Fact]
    public void MultipleExplicitVersions_ShouldBeSupported()
    {
        // Arrange
        var builder = new HandoverDocumentBuilder();

        builder.AddDocumentId(_fixture.Create<string>(), _fixture.Create<string>())
               .WithDocumentClassification(_fixture.Create<string>(), _fixture.Create<string>())
               .AddDocumentVersion(vb => vb.WithLanguage("en").WithTitle("Title 1").WithDescription("none").WithOrganization("short", "long").AddDigitalFile("path/to/file.pdf"))
               .AddDocumentVersion(vb => vb.WithLanguage("de").WithTitle("Title 2").WithDescription("none").WithOrganization("short", "long").AddDigitalFile("path/to/file.pdf"));
        
        // Act
        var result = builder.Build();

        // Assert
        result.DocumentVersions.Count.ShouldBe(2);
    }
}