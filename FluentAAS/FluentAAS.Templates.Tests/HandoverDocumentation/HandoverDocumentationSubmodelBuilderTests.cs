using AasCore.Aas3_0;
using FluentAAS.Builder;
using FluentAAS.Templates.HandoverDocumentation;
using JetBrains.Annotations;
using Moq;
using Shouldly;

namespace FluentAAS.Templates.Tests.HandoverDocumentation;

[TestSubject(typeof(HandoverDocumentationSubmodelBuilder))]
public class HandoverDocumentationSubmodelBuilderTests
{
    private readonly Mock<IShellBuilder> _mockShellBuilder;
    private const    string              TestId      = "test-handover-documentation-id";
    private const    string              TestIdShort = "test-id-short";

    public HandoverDocumentationSubmodelBuilderTests()
    {
        _mockShellBuilder = new Mock<IShellBuilder>();

        // IMPORTANT: match the overload used by production code:
        // _shellBuilder.AddSubmodelReference(submodel);
        _mockShellBuilder
            .Setup(x => x.AddSubmodelReference(It.IsAny<Submodel>(), It.IsAny<KeyTypes>()))
            .Callback<Submodel, KeyTypes>((submodel, keyTypes) => { });
    }

    [Fact]
    public void Constructor_ShouldInitializeWithValidParameters()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);
        builder.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithCustomIdShort()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId, TestIdShort);
        builder.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenShellBuilderIsNull()
    {
        Should.Throw<ArgumentNullException>(() =>
                                                new HandoverDocumentationSubmodelBuilder(null!, TestId))
              .ParamName.ShouldBe("shellBuilder");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        Should.Throw<ArgumentNullException>(() =>
                                                new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, null!))
              .ParamName.ShouldBe("id");
    }

    [Fact]
    public void WithCategory_ShouldSetCategoryAndReturnBuilder()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);
        var result  = builder.WithCategory("test-category");
        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void WithDescription_ShouldAddDescriptionAndReturnBuilder()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        var result = builder.WithDescription("en", "Test description");

        result.ShouldBeSameAs(builder);
    }

    [Theory]
    [InlineData(null, "text")]
    [InlineData("", "text")]
    [InlineData("   ", "text")]
    public void WithDescription_ShouldThrowArgumentException_WhenLanguageIsNullOrWhitespace(string? language, string text)
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        Should.Throw<ArgumentException>(() => builder.WithDescription(language!, text))
              .ParamName.ShouldBe("language");
    }

    [Theory]
    [InlineData("en", null)]
    [InlineData("en", "")]
    [InlineData("en", "   ")]
    public void WithDescription_ShouldThrowArgumentException_WhenTextIsNullOrWhitespace(string language, string? text)
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        Should.Throw<ArgumentException>(() => builder.WithDescription(language, text!))
              .ParamName.ShouldBe("text");
    }

    [Fact]
    public void AddDocument_ShouldAddDocumentAndReturnBuilder()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        var result = builder.AddDocument(docBuilder =>
                                         {
                                             docBuilder.AddDocumentId("URI", "DOC-001");
                                             docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                                             docBuilder.AddDocumentVersion(ver => ver
                                                                                  .WithVersion("1.0")
                                                                                  .WithTitle("Test Document")
                                                                                  .WithDescription("none")
                                                                                  .WithOrganization("short", "long")
                                                                                  .AddDigitalFile("test.pdf", "application/pdf"));
                                         });

        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void AddDocument_ShouldThrowArgumentNullException_WhenConfigureIsNull()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        Should.Throw<ArgumentNullException>(() => builder.AddDocument(null!))
              .ParamName.ShouldBe("configure");
    }

    [Fact]
    public void AddEntity_ShouldAddEntityAndReturnBuilder()
    {
        var builder    = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);
        var mockEntity = new Mock<IReferable>();

        var result = builder.AddEntity(mockEntity.Object);

        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void AddEntity_ShouldThrowArgumentNullException_WhenEntityIsNull()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        Should.Throw<ArgumentNullException>(() => builder.AddEntity(null!))
              .ParamName.ShouldBe("entity");
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldReturnShellBuilder_WhenValidDocumentAdded()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
            .AddDocument(docBuilder =>
                         {
                             docBuilder.AddDocumentId("URI", "DOC-001");
                             docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                             docBuilder.AddDocumentVersion(ver => ver
                                                                  .WithVersion("1.0")
                                                                  .WithTitle("Test Document")
                                                                  .WithDescription("none")
                                                                  .WithOrganization("short", "long")
                                                                  .AddDigitalFile("test.pdf", "application/pdf"));
                         });

        var result = builder.BuildHandoverDocumentation();

        result.ShouldBeSameAs(_mockShellBuilder.Object);
        _mockShellBuilder.Verify(x => x.AddSubmodelReference(It.IsAny<Submodel>(), It.IsAny<KeyTypes>()), Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldThrowInvalidOperationException_WhenNoDocumentsAdded()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        var exception = Should.Throw<InvalidOperationException>(() => builder.BuildHandoverDocumentation());
        exception.Message.ShouldBe("Handover Documentation submodel must contain at least one Document.");
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldCallAddSubmodelReference()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
            .AddDocument(docBuilder =>
                         {
                             docBuilder.AddDocumentId("URI", "DOC-001");
                             docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                             docBuilder.AddDocumentVersion(ver => ver
                                                                  .WithVersion("1.0")
                                                                  .WithTitle("Test Document")
                                                                  .WithDescription("none")
                                                                  .WithOrganization("short", "long")
                                                                  .AddDigitalFile("test.pdf", "application/pdf"));
                         });

        builder.BuildHandoverDocumentation();

        _mockShellBuilder.Verify(x => x.AddSubmodelReference(It.IsAny<Submodel>(), It.IsAny<KeyTypes>()), Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldCreateSubmodelWithCorrectId()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
            .AddDocument(docBuilder =>
                         {
                             docBuilder.AddDocumentId("URI", "DOC-001");
                             docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                             docBuilder.AddDocumentVersion(ver => ver
                                                                  .WithVersion("1.0")
                                                                  .WithTitle("Test Document")
                                                                  .WithDescription("none")
                                                                  .WithOrganization("short", "long")
                                                                  .AddDigitalFile("test.pdf", "application/pdf"));
                         });

        builder.BuildHandoverDocumentation();

        _mockShellBuilder.Verify(
                                 x =>
                                     x.AddSubmodelReference(It.Is<Submodel>(s => s.Id == TestId), It.IsAny<KeyTypes>()),
                                 Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldUseProvidedCategory_WhenCategorySet()
    {
        const string testCategory = "test-category";

        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
                      .WithCategory(testCategory)
                      .AddDocument(docBuilder =>
                                   {
                                       docBuilder.AddDocumentId("URI", "DOC-001");
                                       docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                                       docBuilder.AddDocumentVersion(ver => ver
                                                                            .WithVersion("1.0")
                                                                            .WithTitle("Test Document")
                                                                            .WithDescription("none")
                                                                            .WithOrganization("short", "long")
                                                                            .AddDigitalFile("test.pdf", "application/pdf"));
                                   });

        builder.BuildHandoverDocumentation();

        _mockShellBuilder.Verify(
                                 x =>
                                     x.AddSubmodelReference(It.Is<Submodel>(s => s.Category == testCategory), It.IsAny<KeyTypes>()),
                                 Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldIncludeDescription_WhenDescriptionsAdded()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
                      .WithDescription("en", "English description")
                      .WithDescription("de", "German description")
                      .AddDocument(docBuilder =>
                                   {
                                       docBuilder.AddDocumentId("URI", "DOC-001");
                                       docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                                       docBuilder.AddDocumentVersion(ver => ver
                                                                            .WithVersion("1.0")
                                                                            .WithTitle("Test Document")
                                                                            .WithDescription("none")
                                                                            .WithOrganization("short", "long")
                                                                            .AddDigitalFile("test.pdf", "application/pdf"));
                                   });

        builder.BuildHandoverDocumentation();

        _mockShellBuilder.Verify(
                                 x =>
                                     x.AddSubmodelReference(It.Is<Submodel>(s => s.Description != null && s.Description.Count == 2), It.IsAny<KeyTypes>()),
                                 Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldSetModellingKindToInstance()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId)
            .AddDocument(docBuilder =>
                         {
                             docBuilder.AddDocumentId("URI", "DOC-001");
                             docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                             docBuilder.AddDocumentVersion(ver => ver
                                                                  .WithVersion("1.0")
                                                                  .WithTitle("Test Document")
                                                                  .WithDescription("none")
                                                                  .WithOrganization("short", "long")
                                                                  .AddDigitalFile("test.pdf", "application/pdf"));
                         });

        builder.BuildHandoverDocumentation();

        _mockShellBuilder.Verify(
                                 x =>
                                     x.AddSubmodelReference(It.Is<Submodel>(s => s.Kind == ModellingKind.Instance), It.IsAny<KeyTypes>()),
                                 Times.Once);
    }

    [Fact]
    public void BuildHandoverDocumentation_ShouldUseCustomIdShort_WhenProvided()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId, TestIdShort)
            .AddDocument(docBuilder =>
                         {
                             docBuilder.AddDocumentId("URI", "DOC-001");
                             docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                             docBuilder.AddDocumentVersion(ver => ver
                                                                  .WithVersion("1.0")
                                                                  .WithTitle("Test Document")
                                                                  .WithDescription("none")
                                                                  .WithOrganization("short", "long")
                                                                  .AddDigitalFile("test.pdf", "application/pdf"));
                         });

        builder.BuildHandoverDocumentation();

        _mockShellBuilder.Verify(
                                 x =>
                                     x.AddSubmodelReference(It.Is<Submodel>(s => s.IdShort == TestIdShort), It.IsAny<KeyTypes>()),
                                 Times.Once);
    }

    [Fact]
    public void AddDocument_ShouldSupportFluentDocumentVersionConfiguration()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

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

        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void AddDocument_ShouldSupportMultipleDocumentVersions()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        var result = builder.AddDocument(docBuilder =>
                                         {
                                             docBuilder.AddDocumentId("URI", "DOC-003");
                                             docBuilder.WithDocumentClassification("03-01", "Technical Specification");
                                             docBuilder.AddDocumentVersion(ver => ver
                                                                                  .WithVersion("1.0")
                                                                                  .WithTitle("Technical Specification v1.0")
                                                                                  .WithDescription("none")
                                                                                  .WithOrganization("short", "long")
                                                                                  .AddDigitalFile("spec-v1.pdf", "application/pdf"));
                                             docBuilder.AddDocumentVersion(ver => ver
                                                                                  .WithVersion("2.0")
                                                                                  .WithTitle("Technical Specification v2.0")
                                                                                  .WithDescription("none")
                                                                                  .WithOrganization("short", "long")
                                                                                  .AddDigitalFile("spec-v2.pdf", "application/pdf"));
                                         });

        result.ShouldBeSameAs(builder);
    }

    [Fact]
    public void AddDocument_ShouldSupportMultipleDocuments()
    {
        var builder = new HandoverDocumentationSubmodelBuilder(_mockShellBuilder.Object, TestId);

        var result = builder
                     .AddDocument(docBuilder =>
                                  {
                                      docBuilder.AddDocumentId("URI", "DOC-001");
                                      docBuilder.WithDocumentClassification("01-01", "Installation Manual");
                                      docBuilder.AddDocumentVersion(ver =>
                                                                        ver.WithVersion("1.0")
                                                                           .WithTitle("Installation Guide")
                                                                           .WithDescription("Installation guide document")
                                                                           .WithOrganization("short", "long")
                                                                           .AddDigitalFile("install.pdf", "application/pdf"));
                                  })
                     .AddDocument(docBuilder =>
                                  {
                                      docBuilder.AddDocumentId("URI", "DOC-002");
                                      docBuilder.WithDocumentClassification("02-01", "User Manual");
                                      docBuilder.AddDocumentVersion(ver =>
                                                                        ver.WithVersion("1.0")
                                                                           .WithTitle("User Guide")
                                                                           .WithDescription("User guide document")
                                                                           .WithOrganization("short", "long")
                                                                           .AddDigitalFile("user.pdf", "application/pdf"));
                                  });

        result.ShouldBeSameAs(builder);
    }
}