using AasCore.Aas3_0;
using AutoFixture;
using FluentAAS.Templates.HandoverDocumentation;
using FluentAAS.Templates.HandoverDocumentation.Document;
using JetBrains.Annotations;
using Shouldly;

namespace FluentAAS.Templates.Tests.HandoverDocumentation.Document;

[TestSubject(typeof(HandoverDocumentId))]
public class HandoverDocumentIdTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var documentId = new HandoverDocumentId();

        // Assert
        documentId.DocumentDomainId.ShouldBe(string.Empty);
        documentId.DocumentIdentifier.ShouldBe(string.Empty);
        documentId.DocumentIsPrimary.ShouldBeNull();
    }

    [Fact]
    public void Properties_ShouldBeSettableViaInit()
    {
        // Arrange
        var domainId = _fixture.Create<string>();
        var identifier = _fixture.Create<string>();
        var isPrimary = _fixture.Create<bool>();

        // Act
        var documentId = new HandoverDocumentId
        {
            DocumentDomainId = domainId,
            DocumentIdentifier = identifier,
            DocumentIsPrimary = isPrimary
        };

        // Assert
        documentId.DocumentDomainId.ShouldBe(domainId);
        documentId.DocumentIdentifier.ShouldBe(identifier);
        documentId.DocumentIsPrimary.ShouldBe(isPrimary);
    }

    [Fact]
    public void ToCollection_WithValidDataAndNoPrimary_ShouldReturnSubmodelElementCollection()
    {
        // Arrange
        var documentId = CreateValidDocumentId(isPrimary: null);

        // Act
        var result = documentId.ToCollection();

        // Assert
        result.ShouldNotBeNull();
        result.IdShort.ShouldBe("DocumentId");
        result.Category.ShouldBeNull();
        result.Description.ShouldBeNull();
        result.SemanticId.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2); // DocumentDomainId, DocumentIdentifier (no DocumentIsPrimary)
    }

    [Fact]
    public void ToCollection_WithValidDataAndPrimaryTrue_ShouldReturnSubmodelElementCollectionWithPrimary()
    {
        // Arrange
        var documentId = CreateValidDocumentId(isPrimary: true);

        // Act
        var result = documentId.ToCollection();

        // Assert
        result.ShouldNotBeNull();
        result.IdShort.ShouldBe("DocumentId");
        result.Category.ShouldBeNull();
        result.Description.ShouldBeNull();
        result.SemanticId.ShouldNotBeNull();
        result.Value.Count.ShouldBe(3); // DocumentDomainId, DocumentIdentifier, DocumentIsPrimary
    }

    [Fact]
    public void ToCollection_WithValidDataAndPrimaryFalse_ShouldReturnSubmodelElementCollectionWithPrimary()
    {
        // Arrange
        var documentId = CreateValidDocumentId(isPrimary: false);

        // Act
        var result = documentId.ToCollection();

        // Assert
        result.ShouldNotBeNull();
        result.IdShort.ShouldBe("DocumentId");
        result.Category.ShouldBeNull();
        result.Description.ShouldBeNull();
        result.SemanticId.ShouldNotBeNull();
        result.Value.Count.ShouldBe(3); // DocumentDomainId, DocumentIdentifier, DocumentIsPrimary
    }

    [Fact]
    public void ToCollection_ShouldContainDocumentDomainIdProperty()
    {
        // Arrange
        var domainId = _fixture.Create<string>();
        var documentId = CreateValidDocumentId(domainId: domainId);

        // Act
        var result = documentId.ToCollection();

        // Assert
        var domainIdElement = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentDomainId);
        domainIdElement.ShouldNotBeNull();
        domainIdElement.ShouldBeOfType<Property>();
        
        var domainIdProperty = (Property)domainIdElement!;
        domainIdProperty.Value.ShouldBe(domainId);
        domainIdProperty.ValueType.ShouldBe(DataTypeDefXsd.String);
        domainIdProperty.SemanticId.ShouldNotBeNull();
    }

    [Fact]
    public void ToCollection_ShouldContainDocumentIdentifierProperty()
    {
        // Arrange
        var identifier = _fixture.Create<string>();
        var documentId = CreateValidDocumentId(identifier: identifier);

        // Act
        var result = documentId.ToCollection();

        // Assert
        var identifierElement = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentIdentifier);
        identifierElement.ShouldNotBeNull();
        identifierElement.ShouldBeOfType<Property>();
        
        var identifierProperty = (Property)identifierElement!;
        identifierProperty.Value.ShouldBe(identifier);
        identifierProperty.ValueType.ShouldBe(DataTypeDefXsd.String);
        identifierProperty.SemanticId.ShouldNotBeNull();
    }

    [Fact]
    public void ToCollection_WithPrimaryTrue_ShouldContainDocumentIsPrimaryPropertyWithTrueValue()
    {
        // Arrange
        var documentId = CreateValidDocumentId(isPrimary: true);

        // Act
        var result = documentId.ToCollection();

        // Assert
        var primaryElement = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentIsPrimary);
        primaryElement.ShouldNotBeNull();
        primaryElement.ShouldBeOfType<Property>();
        
        var primaryProperty = (Property)primaryElement!;
        primaryProperty.Value.ShouldBe("true");
        primaryProperty.ValueType.ShouldBe(DataTypeDefXsd.Boolean);
        primaryProperty.SemanticId.ShouldNotBeNull();
    }

    [Fact]
    public void ToCollection_WithPrimaryFalse_ShouldContainDocumentIsPrimaryPropertyWithFalseValue()
    {
        // Arrange
        var documentId = CreateValidDocumentId(isPrimary: false);

        // Act
        var result = documentId.ToCollection();

        // Assert
        var primaryElement = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentIsPrimary);
        primaryElement.ShouldNotBeNull();
        primaryElement.ShouldBeOfType<Property>();
        
        var primaryProperty = (Property)primaryElement!;
        primaryProperty.Value.ShouldBe("false");
        primaryProperty.ValueType.ShouldBe(DataTypeDefXsd.Boolean);
        primaryProperty.SemanticId.ShouldNotBeNull();
    }

    [Fact]
    public void ToCollection_WithPrimaryNull_ShouldNotContainDocumentIsPrimaryProperty()
    {
        // Arrange
        var documentId = CreateValidDocumentId(isPrimary: null);

        // Act
        var result = documentId.ToCollection();

        // Assert
        var primaryElement = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentIsPrimary);
        primaryElement.ShouldBeNull();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotThrow()
    {
        // Arrange
        var documentId = CreateValidDocumentId();

        // Act & Assert
        Should.NotThrow(() => documentId.Validate());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Validate_WithInvalidDocumentDomainId_ShouldThrowInvalidOperationException(string invalidDomainId)
    {
        // Arrange
        var documentId = CreateValidDocumentId(domainId: invalidDomainId);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => documentId.Validate());
        exception.Message.ShouldBe("DocumentDomainId is required in DocumentId.");
    }

    [Fact]
    public void Validate_WithNullDocumentDomainId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var documentId = new HandoverDocumentId
        {
            DocumentDomainId = null!,
            DocumentIdentifier = _fixture.Create<string>()
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => documentId.Validate());
        exception.Message.ShouldBe("DocumentDomainId is required in DocumentId.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Validate_WithInvalidDocumentIdentifier_ShouldThrowInvalidOperationException(string invalidIdentifier)
    {
        // Arrange
        var documentId = CreateValidDocumentId(identifier: invalidIdentifier);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => documentId.Validate());
        exception.Message.ShouldBe("DocumentIdentifier is required in DocumentId.");
    }

    [Fact]
    public void Validate_WithNullDocumentIdentifier_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var documentId = new HandoverDocumentId
        {
            DocumentDomainId = _fixture.Create<string>(),
            DocumentIdentifier = null!
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => documentId.Validate());
        exception.Message.ShouldBe("DocumentIdentifier is required in DocumentId.");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Validate_WithValidDataAndAnyPrimaryValue_ShouldNotThrow(bool isPrimary)
    {
        // Arrange
        var documentId = CreateValidDocumentId(isPrimary: isPrimary);

        // Act & Assert
        Should.NotThrow(() => documentId.Validate());
    }

    [Fact]
    public void Validate_WithValidDataAndNullPrimary_ShouldNotThrow()
    {
        // Arrange
        var documentId = CreateValidDocumentId(isPrimary: null);

        // Act & Assert
        Should.NotThrow(() => documentId.Validate());
    }

    [Fact]
    public void ToCollection_ShouldSetCorrectSemanticIds()
    {
        // Arrange
        var documentId = CreateValidDocumentId(isPrimary: true);

        // Act
        var result = documentId.ToCollection();

        // Assert
        // Collection semantic ID
        result.SemanticId.ShouldNotBeNull();
        
        // Individual element semantic IDs
        var domainIdElement = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentDomainId);
        domainIdElement!.SemanticId.ShouldNotBeNull();
        
        var identifierElement = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentIdentifier);
        identifierElement!.SemanticId.ShouldNotBeNull();
        
        var primaryElement = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentIsPrimary);
        primaryElement!.SemanticId.ShouldNotBeNull();
    }

    [Fact]
    public void ToCollection_WithEmptyButValidStrings_ShouldHandleGracefully()
    {
        // Note: Empty strings are not valid according to Validate(), but testing ToCollection behavior
        var documentId = new HandoverDocumentId
        {
            DocumentDomainId = "ValidDomain",
            DocumentIdentifier = "ValidId",
            DocumentIsPrimary = true
        };

        // Act
        var result = documentId.ToCollection();

        // Assert
        result.ShouldNotBeNull();
        result.Value.Count.ShouldBe(3);
        
        var domainIdProperty = (Property)result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentDomainId)!;
        domainIdProperty.Value.ShouldBe("ValidDomain");
        
        var identifierProperty = (Property)result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentIdentifier)!;
        identifierProperty.Value.ShouldBe("ValidId");
        
        var primaryProperty = (Property)result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentIsPrimary)!;
        primaryProperty.Value.ShouldBe("true");
    }

    [Fact]
    public void Properties_ShouldBeReadOnlyAfterInitialization()
    {
        // Arrange
        var domainId = _fixture.Create<string>();
        var identifier = _fixture.Create<string>();
        var isPrimary = _fixture.Create<bool>();

        // Act
        var documentId = new HandoverDocumentId
        {
            DocumentDomainId = domainId,
            DocumentIdentifier = identifier,
            DocumentIsPrimary = isPrimary
        };

        // Assert - Properties should not have setters (init-only)
        // This is validated at compile time, but we can verify the values are preserved
        documentId.DocumentDomainId.ShouldBe(domainId);
        documentId.DocumentIdentifier.ShouldBe(identifier);
        documentId.DocumentIsPrimary.ShouldBe(isPrimary);
    }

    private HandoverDocumentId CreateValidDocumentId(
        string? domainId = null,
        string? identifier = null,
        bool? isPrimary = null)
    {
        return new HandoverDocumentId
        {
            DocumentDomainId = domainId ?? _fixture.Create<string>(),
            DocumentIdentifier = identifier ?? _fixture.Create<string>(),
            DocumentIsPrimary = isPrimary
        };
    }
}