using AasCore.Aas3_0;
using FluentAAS.Builder;
using FluentAAS.Templates.HandoverDocumentation;
using JetBrains.Annotations;
using Moq;
using Shouldly;

namespace FluentAAS.Templates.Tests.HandoverDocumentation;

[TestSubject(typeof(HandoverDocumentationBuilderExtensions))]
public class HandoverDocumentationBuilderExtensionsTests
{
    private readonly Mock<IShellBuilder> _mockShellBuilder;
    private const string TestSubmodelId = "urn:submodel:example:handover-documentation:V2_0";
    private const string TestIdShort = "TestHandoverDoc";

    public HandoverDocumentationBuilderExtensionsTests()
    {
        _mockShellBuilder = new Mock<IShellBuilder>();
        _mockShellBuilder
            .Setup(x => x.AddSubmodelReference(It.IsAny<Submodel>(), It.IsAny<KeyTypes>()))
            .Callback<Submodel, KeyTypes>((submodel, keyTypes) => { });
    }

    [Fact]
    public void AddHandoverDocumentation_ShouldReturnHandoverDocumentationSubmodelBuilder_WithValidParameters()
    {
        // Act
        var result = _mockShellBuilder.Object.AddHandoverDocumentation(TestSubmodelId);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<HandoverDocumentationSubmodelBuilder>();
    }

    [Fact]
    public void AddHandoverDocumentation_ShouldReturnHandoverDocumentationSubmodelBuilder_WithCustomIdShort()
    {
        // Act
        var result = _mockShellBuilder.Object.AddHandoverDocumentation(TestSubmodelId, TestIdShort);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<HandoverDocumentationSubmodelBuilder>();
    }

    [Fact]
    public void AddHandoverDocumentation_ShouldUseDefaultIdShort_WhenIdShortNotProvided()
    {
        // Act
        var result = _mockShellBuilder.Object.AddHandoverDocumentation(TestSubmodelId);

        // Assert - Build to verify the IdShort is set correctly
        result.AddDocument(doc => doc
                .AddDocumentId("URI", "DOC-001")
                .WithDocumentClassification("01-01", "Test Document")
                .AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Title")
                    .WithDescription("Test Description")
                    .WithOrganization("short", "long")
                    .AddDigitalFile("test.pdf", "application/pdf")));

        var shellBuilder = result.BuildHandoverDocumentation();
        shellBuilder.ShouldBe(_mockShellBuilder.Object);

        // Verify that AddSubmodelReference was called with the correct IdShort
        _mockShellBuilder.Verify(
            x => x.AddSubmodelReference(
                It.Is<Submodel>(s => s.IdShort == "HandoverDocumentation"), 
                It.IsAny<KeyTypes>()), 
            Times.Once);
    }

    [Fact]
    public void AddHandoverDocumentation_ShouldUseProvidedIdShort_WhenIdShortProvided()
    {
        // Act
        var result = _mockShellBuilder.Object.AddHandoverDocumentation(TestSubmodelId, TestIdShort);

        // Assert - Build to verify the IdShort is set correctly
        result.AddDocument(doc => doc
                .AddDocumentId("URI", "DOC-001")
                .WithDocumentClassification("01-01", "Test Document")
                .AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Title")
                    .WithDescription("Test Description")
                    .WithOrganization("short", "long")
                    .AddDigitalFile("test.pdf", "application/pdf")));

        var shellBuilder = result.BuildHandoverDocumentation();
        shellBuilder.ShouldBe(_mockShellBuilder.Object);

        // Verify that AddSubmodelReference was called with the custom IdShort
        _mockShellBuilder.Verify(
            x => x.AddSubmodelReference(
                It.Is<Submodel>(s => s.IdShort == TestIdShort), 
                It.IsAny<KeyTypes>()), 
            Times.Once);
    }

    [Fact]
    public void AddHandoverDocumentation_ShouldThrowArgumentNullException_WhenShellBuilderIsNull()
    {
        // Arrange
        IShellBuilder? nullShellBuilder = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            nullShellBuilder!.AddHandoverDocumentation(TestSubmodelId))
            .ParamName.ShouldBe("shellBuilder");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddHandoverDocumentation_ShouldThrowArgumentException_WhenSubmodelIdIsNullOrWhitespace(string? submodelId)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => 
            _mockShellBuilder.Object.AddHandoverDocumentation(submodelId!))
            .ParamName.ShouldBe("submodelId");
    }

    [Fact]
    public void AddHandoverDocumentation_ShouldCreateBuilderWithCorrectSubmodelId()
    {
        // Act
        var result = _mockShellBuilder.Object.AddHandoverDocumentation(TestSubmodelId);

        // Assert - Build to verify the submodel ID is set correctly
        result.AddDocument(doc => doc
                .AddDocumentId("URI", "DOC-001")
                .WithDocumentClassification("01-01", "Test Document")
                .AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Title")
                    .WithDescription("Test Description")
                    .WithOrganization("short", "long")
                    .AddDigitalFile("test.pdf", "application/pdf")));

        var shellBuilder = result.BuildHandoverDocumentation();
        shellBuilder.ShouldBe(_mockShellBuilder.Object);

        // Verify that AddSubmodelReference was called with the correct submodel ID
        _mockShellBuilder.Verify(
            x => x.AddSubmodelReference(
                It.Is<Submodel>(s => s.Id == TestSubmodelId), 
                It.IsAny<KeyTypes>()), 
            Times.Once);
    }

    [Fact]
    public void AddHandoverDocumentation_ShouldAllowFluentChaining()
    {
        // Act
        var result = _mockShellBuilder.Object
            .AddHandoverDocumentation(TestSubmodelId)
            .WithDescription("en", "Test description")
            .WithCategory("INSTANCE")
            .AddDocument(doc => doc
                .AddDocumentId("URI", "DOC-001")
                .WithDocumentClassification("01-01", "Test Document")
                .AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Title")
                    .WithDescription("Test Description")
                    .WithOrganization("short", "long")
                    .AddDigitalFile("test.pdf", "application/pdf")));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<HandoverDocumentationSubmodelBuilder>();

        // Verify the fluent chain works by building
        var shellBuilder = result.BuildHandoverDocumentation();
        shellBuilder.ShouldBe(_mockShellBuilder.Object);
    }

    [Fact]
    public void AddHandoverDocumentation_ShouldHandleNullIdShortCorrectly()
    {
        // Act
        var result = _mockShellBuilder.Object.AddHandoverDocumentation(TestSubmodelId, null);

        // Assert - Build to verify null IdShort is handled (should use default)
        result.AddDocument(doc => doc
                .AddDocumentId("URI", "DOC-001")
                .WithDocumentClassification("01-01", "Test Document")
                .AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Title")
                    .WithDescription("Test Description")
                    .WithOrganization("short", "long")
                    .AddDigitalFile("test.pdf", "application/pdf")));

        var shellBuilder = result.BuildHandoverDocumentation();
        shellBuilder.ShouldBe(_mockShellBuilder.Object);

        // Verify that AddSubmodelReference was called with the default IdShort
        _mockShellBuilder.Verify(
            x => x.AddSubmodelReference(
                It.Is<Submodel>(s => s.IdShort == "HandoverDocumentation"), 
                It.IsAny<KeyTypes>()), 
            Times.Once);
    }

    [Fact]
    public void AddHandoverDocumentation_ShouldHandleEmptyIdShortCorrectly()
    {
        // Act
        var result = _mockShellBuilder.Object.AddHandoverDocumentation(TestSubmodelId, "");

        // Assert - Build to verify empty IdShort is handled (should use default)
        result.AddDocument(doc => doc
                .AddDocumentId("URI", "DOC-001")
                .WithDocumentClassification("01-01", "Test Document")
                .AddDocumentVersion(ver => ver
                    .WithVersion("1.0")
                    .WithTitle("Test Title")
                    .WithDescription("Test Description")
                    .WithOrganization("short", "long")
                    .AddDigitalFile("test.pdf", "application/pdf")));

        var shellBuilder = result.BuildHandoverDocumentation();
        shellBuilder.ShouldBe(_mockShellBuilder.Object);

        // Verify that AddSubmodelReference was called with the default IdShort
        _mockShellBuilder.Verify(
            x => x.AddSubmodelReference(
                It.Is<Submodel>(s => s.IdShort == "HandoverDocumentation"), 
                It.IsAny<KeyTypes>()), 
            Times.Once);
    }
}