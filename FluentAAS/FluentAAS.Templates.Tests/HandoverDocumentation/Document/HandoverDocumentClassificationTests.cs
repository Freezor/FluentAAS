using AasCore.Aas3_0;
using AutoFixture;
using FluentAAS.Templates.HandoverDocumentation;
using FluentAAS.Templates.HandoverDocumentation.Document;
using JetBrains.Annotations;
using Shouldly;

namespace FluentAAS.Templates.Tests.HandoverDocumentation.Document;

[TestSubject(typeof(HandoverDocumentClassification))]
public class HandoverDocumentClassificationTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var classification = new HandoverDocumentClassification();

        // Assert
        classification.ClassId.ShouldBe(string.Empty);
        classification.ClassName.ShouldBe(string.Empty);
        classification.ClassNameLanguage.ShouldBe("en");
        classification.ClassificationSystem.ShouldBe(HandoverDocumentationSemantics.Vdi2770ClassificationSystemName);
    }

    [Fact]
    public void Properties_ShouldBeSettableViaInit()
    {
        // Arrange
        var classId = _fixture.Create<string>();
        var className = _fixture.Create<string>();
        var classNameLanguage = "de";
        var classificationSystem = "CustomSystem";

        // Act
        var classification = new HandoverDocumentClassification
        {
            ClassId = classId,
            ClassName = className,
            ClassNameLanguage = classNameLanguage,
            ClassificationSystem = classificationSystem
        };

        // Assert
        classification.ClassId.ShouldBe(classId);
        classification.ClassName.ShouldBe(className);
        classification.ClassNameLanguage.ShouldBe(classNameLanguage);
        classification.ClassificationSystem.ShouldBe(classificationSystem);
    }

    [Fact]
    public void ToCollection_WithValidData_ShouldReturnSubmodelElementCollection()
    {
        // Arrange
        var classification = CreateValidClassification();

        // Act
        var result = classification.ToCollection();

        // Assert
        result.ShouldNotBeNull();
        result.IdShort.ShouldBe("DocumentClassification");
        result.Category.ShouldBeNull();
        result.Description.ShouldBeNull();
        result.SemanticId.ShouldNotBeNull();
        result.Value!.Count.ShouldBe(3); // ClassId, ClassName, ClassificationSystem
    }

    [Fact]
    public void ToCollection_ShouldContainClassIdProperty()
    {
        // Arrange
        var classId = _fixture.Create<string>();
        var classification = CreateValidClassification(classId: classId);

        // Act
        var result = classification.ToCollection();

        // Assert
        var classIdElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortClassId);
        classIdElement.ShouldNotBeNull();
        classIdElement.ShouldBeOfType<Property>();
        
        var classIdProperty = (Property)classIdElement!;
        classIdProperty.Value.ShouldBe(classId);
        classIdProperty.ValueType.ShouldBe(DataTypeDefXsd.String);
        classIdProperty.SemanticId.ShouldNotBeNull();
    }

    [Fact]
    public void ToCollection_ShouldContainClassNameMultiLanguageProperty()
    {
        // Arrange
        var className = _fixture.Create<string>();
        var classNameLanguage = "fr";
        var classification = CreateValidClassification(className: className, classNameLanguage: classNameLanguage);

        // Act
        var result = classification.ToCollection();

        // Assert
        var classNameElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortClassName);
        classNameElement.ShouldNotBeNull();
        classNameElement.ShouldBeOfType<MultiLanguageProperty>();
        
        var classNameProperty = (MultiLanguageProperty)classNameElement!;
        classNameProperty.Value!.Count.ShouldBe(1);
        classNameProperty.Value[0].Language.ShouldBe(classNameLanguage);
        classNameProperty.Value[0].Text.ShouldBe(className);
        classNameProperty.SemanticId.ShouldNotBeNull();
    }

    [Fact]
    public void ToCollection_ShouldContainClassificationSystemProperty()
    {
        // Arrange
        var classificationSystem = "CustomSystem";
        var classification = CreateValidClassification(classificationSystem: classificationSystem);

        // Act
        var result = classification.ToCollection();

        // Assert
        var systemElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortClassificationSystem);
        systemElement.ShouldNotBeNull();
        systemElement.ShouldBeOfType<Property>();
        
        var systemProperty = (Property)systemElement!;
        systemProperty.Value.ShouldBe(classificationSystem);
        systemProperty.ValueType.ShouldBe(DataTypeDefXsd.String);
        systemProperty.SemanticId.ShouldNotBeNull();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotThrow()
    {
        // Arrange
        var classification = CreateValidClassification();

        // Act & Assert
        Should.NotThrow(() => classification.Validate());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Validate_WithInvalidClassId_ShouldThrowInvalidOperationException(string invalidClassId)
    {
        // Arrange
        var classification = CreateValidClassification(classId: invalidClassId);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => classification.Validate());
        exception.Message.ShouldBe("ClassId is required in DocumentClassification.");
    }

    [Fact]
    public void Validate_WithNullClassId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var classification = new HandoverDocumentClassification
        {
            ClassId = null!,
            ClassName = _fixture.Create<string>(),
            ClassNameLanguage = "en",
            ClassificationSystem = HandoverDocumentationSemantics.Vdi2770ClassificationSystemName
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => classification.Validate());
        exception.Message.ShouldBe("ClassId is required in DocumentClassification.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Validate_WithInvalidClassName_ShouldThrowInvalidOperationException(string invalidClassName)
    {
        // Arrange
        var classification = CreateValidClassification(className: invalidClassName);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => classification.Validate());
        exception.Message.ShouldBe("ClassName is required in DocumentClassification.");
    }

    [Fact]
    public void Validate_WithNullClassName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var classification = new HandoverDocumentClassification
        {
            ClassId = _fixture.Create<string>(),
            ClassName = null!,
            ClassNameLanguage = "en",
            ClassificationSystem = HandoverDocumentationSemantics.Vdi2770ClassificationSystemName
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => classification.Validate());
        exception.Message.ShouldBe("ClassName is required in DocumentClassification.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Validate_WithInvalidClassNameLanguage_ShouldThrowInvalidOperationException(string invalidLanguage)
    {
        // Arrange
        var classification = CreateValidClassification(classNameLanguage: invalidLanguage);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => classification.Validate());
        exception.Message.ShouldBe("ClassNameLanguage is required in DocumentClassification.");
    }

    [Fact]
    public void Validate_WithNullClassNameLanguage_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var classification = new HandoverDocumentClassification
        {
            ClassId = _fixture.Create<string>(),
            ClassName = _fixture.Create<string>(),
            ClassNameLanguage = null!,
            ClassificationSystem = HandoverDocumentationSemantics.Vdi2770ClassificationSystemName
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => classification.Validate());
        exception.Message.ShouldBe("ClassNameLanguage is required in DocumentClassification.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Validate_WithInvalidClassificationSystem_ShouldThrowInvalidOperationException(string invalidSystem)
    {
        // Arrange
        var classification = CreateValidClassification(classificationSystem: invalidSystem);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => classification.Validate());
        exception.Message.ShouldBe("ClassificationSystem is required in DocumentClassification.");
    }

    [Fact]
    public void Validate_WithNullClassificationSystem_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var classification = new HandoverDocumentClassification
        {
            ClassId = _fixture.Create<string>(),
            ClassName = _fixture.Create<string>(),
            ClassNameLanguage = "en",
            ClassificationSystem = null!
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => classification.Validate());
        exception.Message.ShouldBe("ClassificationSystem is required in DocumentClassification.");
    }

    [Fact]
    public void ToCollection_WithDefaultLanguage_ShouldUseEnglish()
    {
        // Arrange
        var className = _fixture.Create<string>();
        var classification = new HandoverDocumentClassification
        {
            ClassId = _fixture.Create<string>(),
            ClassName = className,
            // ClassNameLanguage defaults to "en"
            ClassificationSystem = HandoverDocumentationSemantics.Vdi2770ClassificationSystemName
        };

        // Act
        var result = classification.ToCollection();

        // Assert
        var classNameElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortClassName);
        var classNameProperty = (MultiLanguageProperty)classNameElement!;
        classNameProperty.Value![0].Language.ShouldBe("en");
        classNameProperty.Value[0].Text.ShouldBe(className);
    }

    [Fact]
    public void ToCollection_WithDefaultClassificationSystem_ShouldUseVdi2770()
    {
        // Arrange
        var classification = new HandoverDocumentClassification
        {
            ClassId = _fixture.Create<string>(),
            ClassName = _fixture.Create<string>(),
            ClassNameLanguage = "en"
            // ClassificationSystem defaults to VDI 2770
        };

        // Act
        var result = classification.ToCollection();

        // Assert
        var systemElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortClassificationSystem);
        var systemProperty = (Property)systemElement!;
        systemProperty.Value.ShouldBe(HandoverDocumentationSemantics.Vdi2770ClassificationSystemName);
    }

    [Fact]
    public void ToCollection_ShouldSetCorrectSemanticIds()
    {
        // Arrange
        var classification = CreateValidClassification();

        // Act
        var result = classification.ToCollection();

        // Assert
        // Collection semantic ID
        result.SemanticId.ShouldNotBeNull();
        
        // Individual element semantic IDs
        var classIdElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortClassId);
        classIdElement!.SemanticId.ShouldNotBeNull();
        
        var classNameElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortClassName);
        classNameElement!.SemanticId.ShouldNotBeNull();
        
        var systemElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortClassificationSystem);
        systemElement!.SemanticId.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("en")]
    [InlineData("de")]
    [InlineData("fr")]
    [InlineData("es")]
    public void ToCollection_WithDifferentLanguages_ShouldPreserveLanguage(string language)
    {
        // Arrange
        var className = _fixture.Create<string>();
        var classification = CreateValidClassification(className: className, classNameLanguage: language);

        // Act
        var result = classification.ToCollection();

        // Assert
        var classNameElement = result.Value!.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortClassName);
        var classNameProperty = (MultiLanguageProperty)classNameElement!;
        classNameProperty.Value![0].Language.ShouldBe(language);
        classNameProperty.Value[0].Text.ShouldBe(className);
    }

    private HandoverDocumentClassification CreateValidClassification(
        string? classId = null,
        string? className = null,
        string? classNameLanguage = null,
        string? classificationSystem = null)
    {
        return new HandoverDocumentClassification
        {
            ClassId = classId ?? _fixture.Create<string>(),
            ClassName = className ?? _fixture.Create<string>(),
            ClassNameLanguage = classNameLanguage ?? "en",
            ClassificationSystem = classificationSystem ?? HandoverDocumentationSemantics.Vdi2770ClassificationSystemName
        };
    }
}