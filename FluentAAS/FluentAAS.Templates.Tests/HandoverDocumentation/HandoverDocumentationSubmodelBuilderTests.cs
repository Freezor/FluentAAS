using AasCore.Aas3_0;
using FluentAAS.Builder;
using FluentAAS.Templates.HandoverDocumentation;
using FluentAAS.Templates.HandoverDocumentation.Document;
using JetBrains.Annotations;
using Moq;
using Shouldly;

namespace FluentAAS.Templates.Tests.HandoverDocumentation;

[TestSubject(typeof(HandoverDocumentationSubmodelBuilder))]
public class HandoverDocumentationSubmodelBuilderTests
{
    private readonly Mock<IShellBuilder> _mockShellBuilder;
    private const string TestId = "test-handover-documentation-id";
    private const string TestIdShort = "test-id-short";

    public HandoverDocumentationSubmodelBuilderTests()
    {
        _mockShellBuilder = new Mock<IShellBuilder>();
        _mockShellBuilder.Setup(x => x.AddSubmodelReference(It.IsAny<Submodel>(),It.IsAny<KeyTypes>())).Returns(_mockShellBuilder.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithValidParameters()
    {
        // Act
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        // Assert
        builder.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithCustomIdShort()
    {
        // Act
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId, TestIdShort);

        // Assert
        builder.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenShellBuilderIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new HandoverDocumentationSubmodelBuilder(null!, TestId))
            .ParamName.ShouldBe("shellBuilder");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, null!))
            .ParamName.ShouldBe("id");
    }

    [Fact]
    public void WithCategory_ShouldSetCategoryAndReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);
        const string category = "test-category";

        // Act
        var result = builder.WithCategory(category);

        // Assert
        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void WithDescription_ShouldAddDescriptionAndReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);
        const string language = "en";
        const string text = "Test description";

        // Act
        var result = builder.WithDescription(language, text);

        // Assert
        result.ShouldBeSameAs(builder);
    }

    [Theory]
    [InlineData(null, "text")]
    [InlineData("", "text")]
    [InlineData("   ", "text")]
    public void WithDescription_ShouldThrowArgumentException_WhenLanguageIsNullOrWhitespace(string? language, string text)
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        // Act & Assert
        Should.Throw<ArgumentException>(() => builder.WithDescription(language!, text))
            .ParamName.ShouldBe("language");
    }

    [Theory]
    [InlineData("en", null)]
    [InlineData("en", "")]
    [InlineData("en", "   ")]
    public void WithDescription_ShouldThrowArgumentException_WhenTextIsNullOrWhitespace(string language, string? text)
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        // Act & Assert
        Should.Throw<ArgumentException>(() => builder.WithDescription(language, text!))
            .ParamName.ShouldBe("text");
    }

    [Fact]
    public void AddDocument_ShouldAddDocumentAndReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        // Act
        var result = builder.AddDocument(docBuilder =>
        {
            docBuilder.AddDocumentId("URI", "DOC-001");
            docBuilder.WithDocumentClassification("01-01", "Installation Manual");
            docBuilder.AddDocumentVersion(ver => ver
                .WithVersion("1.0")
                .WithTitle("Test Document"));
        });

        // Assert
        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void AddDocument_ShouldThrowArgumentNullException_WhenConfigureIsNull()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => builder.AddDocument(null!))
            .ParamName.ShouldBe("configure");
    }

    [Fact]
    public void AddEntity_ShouldAddEntityAndReturnBuilder()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);
        var mockEntity = new Mock<IReferable>();

        // Act
        var result = builder.AddEntity(mockEntity.Object);

        // Assert
        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void AddEntity_ShouldThrowArgumentNullException_WhenEntityIsNull()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => builder.AddEntity(null!))
            .ParamName.ShouldBe("entity");
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldReturnShellBuilder_WhenValidDocumentAdded()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
            .AddDocument(docBuilder =>
            {
                docBuilder.AddDocumentId("URI", "DOC-001");
                docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                docBuilder.AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Document"));
            });

        // Act
        var result = builder.BuildHandoverDocumentation();

        // Assert
        result.ShouldBeSameAs(_mockShellBuilder.Object);
        _mockShellBuilder.Verify(x => x.AddSubmodelReference(It.IsAny<ISubmodel>()), Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldThrowInvalidOperationException_WhenNoDocumentsAdded()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => builder.BuildHandoverDocumentation());
        exception.Message.ShouldBe("Handover Documentation submodel must contain at least one Document.");
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldCallAddSubmodelReference()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
            .AddDocument(docBuilder =>
            {
                docBuilder.AddDocumentId("URI", "DOC-001");
                docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                docBuilder.AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Document"));
            });

        // Act
        builder.BuildHandoverDocumentation();

        // Assert
        _mockShellBuilder.Verify(x => x.AddSubmodelReference(It.IsAny<ISubmodel>()), Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldCreateSubmodelWithCorrectId()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
            .AddDocument(docBuilder =>
            {
                docBuilder.AddDocumentId("URI", "DOC-001");
                docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                docBuilder.AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Document"));
            });

        // Act
        builder.BuildHandoverDocumentation();

        // Assert
        _mockShellBuilder.Verify(x => x.AddSubmodelReference(
            It.Is<ISubmodel>(s => s.Id == TestId)), Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldUseProvidedCategory_WhenCategorySet()
    {
        // Arrange
        const string testCategory = "test-category";
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
            .WithCategory(testCategory)
            .AddDocument(docBuilder =>
            {
                docBuilder.AddDocumentId("URI", "DOC-001");
                docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                docBuilder.AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Document"));
            });

        // Act
        builder.BuildHandoverDocumentation();

        // Assert
        _mockShellBuilder.Verify(x => x.AddSubmodelReference(
            It.Is<ISubmodel>(s => s.Category == testCategory)), Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldIncludeDescription_WhenDescriptionsAdded()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
            .WithDescription("en", "English description")
            .WithDescription("de", "German description")
            .AddDocument(docBuilder =>
            {
                docBuilder.AddDocumentId("URI", "DOC-001");
                docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                docBuilder.AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Document"));
            });

        // Act
        builder.BuildHandoverDocumentation();

        // Assert
        _mockShellBuilder.Verify(x => x.AddSubmodelReference(
            It.Is<ISubmodel>(s => s.Description != null && s.Description.Count == 2)), Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldSetModellingKindToInstance()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
            .AddDocument(docBuilder =>
            {
                docBuilder.AddDocumentId("URI", "DOC-001");
                docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                docBuilder.AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Document"));
            });

        // Act
        builder.BuildHandoverDocumentation();

        // Assert
        _mockShellBuilder.Verify(x => x.AddSubmodelReference(
            It.Is<ISubmodel>(s => s.Kind == ModellingKind.Instance)), Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldUseCustomIdShort_WhenProvided()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId, TestIdShort)
            .AddDocument(docBuilder =>
            {
                docBuilder.AddDocumentId("URI", "DOC-001");
                docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                docBuilder.AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Document"));
            });

        // Act
        builder.BuildHandoverDocumentation();

        // Assert
        _mockShellBuilder.Verify(x => x.AddSubmodelReference(
            It.Is<ISubmodel>(s => s.IdShort == TestIdShort)), Times.Once);
    }

    [Fact]
    public void AddDocument_ShouldSupportFluentDocumentVersionConfiguration()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        // Act
        var result = builder.AddDocument(docBuilder =>
        {
            docBuilder.AddDocumentId("URI", "DOC-002");
            docBuilder.WithDocumentClassification("02-01", "User Manual");
            docBuilder.AddDocumentVersion(ver => ver
                .WithLanguage("en")
                .WithLanguage("de")
                .WithVersion("2.0")
                .WithTitle("Advanced User Manual")
                .WithDescription("Comprehensive user documentation")
                .WithSubtitle("Professional Edition")
                .AddKeyword("user")
                .AddKeyword("manual")
                .WithStatus("Released")
                .WithOrganization("ACME", "ACME Corporation")
                .AddDigitalFile("manual.pdf", "application/pdf")
                .WithPreviewFile("preview.jpg", "image/jpeg"));
        });

        // Assert
        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void AddDocument_ShouldSupportMultipleDocumentVersions()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        // Act
        var result = builder.AddDocument(docBuilder =>
        {
            docBuilder.AddDocumentId("URI", "DOC-003");
            docBuilder.WithDocumentClassification("03-01", "Technical Specification");
            docBuilder.AddDocumentVersion(ver => ver
                .WithVersion("1.0")
                .WithTitle("Technical Specification v1.0"));
            docBuilder.AddDocumentVersion(ver => ver
                .WithVersion("2.0")
                .WithTitle("Technical Specification v2.0"));
        });

        // Assert
        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void AddDocument_ShouldSupportMultipleDocuments()
    {
        // Arrange
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        // Act
        var result = builder
            .AddDocument(docBuilder =>
            {
                docBuilder.AddDocumentId("URI", "DOC-001");
                docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                docBuilder.AddDocumentVersion(ver => ver.WithVersion("1.0").WithTitle("Installation Guide"));
            })
            .AddDocument(docBuilder =>
            {
                docBuilder.AddDocumentId("URI", "DOC-002");
                docBuilder.WithDocumentClassification("02-01", "User Manual");
                docBuilder.AddDocumentVersion(ver => ver.WithVersion("1.0").WithTitle("User Guide"));
            });

        // Assert
        result.ShouldBeSameAs(builder);
    }
}