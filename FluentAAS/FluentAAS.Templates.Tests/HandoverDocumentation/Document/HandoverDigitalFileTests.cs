using AutoFixture;
using FluentAAS.Templates.HandoverDocumentation.Document;
using JetBrains.Annotations;
using Shouldly;

namespace FluentAAS.Templates.Tests.HandoverDocumentation.Document;

[TestSubject(typeof(HandoverDigitalFile))]
public class HandoverDigitalFileTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToFileElement_WithValidData_ShouldReturnFileElement()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            MimeType = "application/pdf",
            Path = "/documents/manual.pdf"
        };
        var idShort = _fixture.Create<string>();
        var semanticId = _fixture.Create<string>();

        // Act
        var result = digitalFile.ToFileElement(idShort, semanticId);

        // Assert
        result.IdShort.ShouldBe(idShort);
        result.ContentType.ShouldBe("application/pdf");
        result.Value.ShouldBe("/documents/manual.pdf");
        result.Category.ShouldBeNull();
        result.SemanticId.ShouldNotBeNull();
    }

    [Fact]
    public void ToFileElement_WithNullSemanticId_ShouldReturnFileElementWithNullSemanticId()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            MimeType = "text/plain",
            Path = "/documents/readme.txt"
        };
        var idShort = _fixture.Create<string>();

        // Act
        var result = digitalFile.ToFileElement(idShort, null);

        // Assert
        result.IdShort.ShouldBe(idShort);
        result.ContentType.ShouldBe("text/plain");
        result.Value.ShouldBe("/documents/readme.txt");
        result.Category.ShouldBeNull();
        result.SemanticId.ShouldBeNull();
    }

    [Fact]
    public void ToFileElement_WithDefaultValues_ShouldUseDefaults()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile(); // Using default values
        var idShort = _fixture.Create<string>();
        var semanticId = _fixture.Create<string>();

        // Act
        var result = digitalFile.ToFileElement(idShort, semanticId);

        // Assert
        result.IdShort.ShouldBe(idShort);
        result.ContentType.ShouldBe("application/octet-stream");
        result.Value.ShouldBe(string.Empty);
        result.Category.ShouldBeNull();
        result.SemanticId.ShouldNotBeNull();
    }

    [Fact]
    public void Validate_WithValidPath_ShouldNotThrow()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            Path = "/valid/path/file.pdf",
            MimeType = "application/pdf"
        };

        // Act & Assert
        Should.NotThrow(() => digitalFile.Validate());
    }

    [Fact]
    public void Validate_WithValidMimeType_ShouldNotThrow()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            Path = "/valid/path/file.pdf",
            MimeType = "application/pdf"
        };

        // Act & Assert
        Should.NotThrow(() => digitalFile.Validate());
    }

    [Fact]
    public void Validate_WithEmptyPath_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            Path = string.Empty,
            MimeType = "application/pdf"
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => digitalFile.Validate());
        exception.Message.ShouldBe("DigitalFile.Path is required.");
    }

    [Fact]
    public void Validate_WithWhitespacePath_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            Path = "   ",
            MimeType = "application/pdf"
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => digitalFile.Validate());
        exception.Message.ShouldBe("DigitalFile.Path is required.");
    }

    [Fact]
    public void Validate_WithNullPath_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            Path = null!,
            MimeType = "application/pdf"
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => digitalFile.Validate());
        exception.Message.ShouldBe("DigitalFile.Path is required.");
    }

    [Fact]
    public void Validate_WithEmptyMimeType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            Path = "/valid/path/file.pdf",
            MimeType = string.Empty
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => digitalFile.Validate());
        exception.Message.ShouldBe("DigitalFile.MimeType is required.");
    }

    [Fact]
    public void Validate_WithWhitespaceMimeType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            Path = "/valid/path/file.pdf",
            MimeType = "   "
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => digitalFile.Validate());
        exception.Message.ShouldBe("DigitalFile.MimeType is required.");
    }

    [Fact]
    public void Validate_WithNullMimeType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            Path = "/valid/path/file.pdf",
            MimeType = null!
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => digitalFile.Validate());
        exception.Message.ShouldBe("DigitalFile.MimeType is required.");
    }

    [Theory]
    [InlineData("application/pdf", "/documents/manual.pdf")]
    [InlineData("text/plain", "/readme.txt")]
    [InlineData("image/jpeg", "/images/photo.jpg")]
    [InlineData("application/json", "/config/settings.json")]
    public void Validate_WithVariousValidInputs_ShouldNotThrow(string mimeType, string path)
    {
        // Arrange
        var digitalFile = new HandoverDigitalFile
        {
            MimeType = mimeType,
            Path = path
        };

        // Act & Assert
        Should.NotThrow(() => digitalFile.Validate());
    }

    [Fact]
    public void Properties_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var digitalFile = new HandoverDigitalFile();

        // Assert
        digitalFile.MimeType.ShouldBe("application/octet-stream");
        digitalFile.Path.ShouldBe(string.Empty);
    }

    [Fact]
    public void Properties_ShouldBeSettableViaInit()
    {
        // Arrange
        var expectedMimeType = _fixture.Create<string>();
        var expectedPath = _fixture.Create<string>();

        // Act
        var digitalFile = new HandoverDigitalFile
        {
            MimeType = expectedMimeType,
            Path = expectedPath
        };

        // Assert
        digitalFile.MimeType.ShouldBe(expectedMimeType);
        digitalFile.Path.ShouldBe(expectedPath);
    }
}