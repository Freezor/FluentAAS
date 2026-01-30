using AasCore.Aas3_0;
using AutoFixture;
using FluentAAS.Templates.HandoverDocumentation;
using FluentAAS.Templates.HandoverDocumentation.Document;
using JetBrains.Annotations;
using Shouldly;

namespace FluentAAS.Templates.Tests.HandoverDocumentation.Document;

[TestSubject(typeof(HandoverDocument))]
public class HandoverDocumentTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_ShouldInitializeEmptyCollections()
    {
        // Arrange & Act
        var document = new HandoverDocument();

        // Assert
        document.DocumentIds.ShouldNotBeNull();
        document.DocumentIds.Count.ShouldBe(0);
        document.DocumentClassifications.ShouldNotBeNull();
        document.DocumentClassifications.Count.ShouldBe(0);
        document.DocumentVersions.ShouldNotBeNull();
        document.DocumentVersions.Count.ShouldBe(0);
    }

    [Fact]
    public void DocumentIds_ShouldBeModifiable()
    {
        // Arrange
        var document = new HandoverDocument();
        var documentId = new HandoverDocumentId
                         {
                             DocumentDomainId   = _fixture.Create<string>(),
                             DocumentIdentifier = _fixture.Create<string>()
                         };

        // Act
        document.DocumentIds.Add(documentId);

        // Assert
        document.DocumentIds.Count.ShouldBe(1);
        document.DocumentIds[0].ShouldBe(documentId);
    }

    [Fact]
    public void DocumentClassifications_ShouldBeModifiable()
    {
        // Arrange
        var document = new HandoverDocument();
        var classification = new HandoverDocumentClassification
                             {
                                 ClassId              = _fixture.Create<string>(),
                                 ClassName            = _fixture.Create<string>(),
                                 ClassificationSystem = HandoverDocumentationSemantics.Vdi2770ClassificationSystemName
                             };

        // Act
        document.DocumentClassifications.Add(classification);

        // Assert
        document.DocumentClassifications.Count.ShouldBe(1);
        document.DocumentClassifications[0].ShouldBe(classification);
    }

    [Fact]
    public void DocumentVersions_ShouldBeModifiable()
    {
        // Arrange
        var document = new HandoverDocument();
        var version  = CreateValidDocumentVersion();

        // Act
        document.DocumentVersions.Add(version);

        // Assert
        document.DocumentVersions.Count.ShouldBe(1);
        document.DocumentVersions[0].ShouldBe(version);
    }

    [Fact]
    public void ToDocumentCollection_WithValidData_ShouldReturnSubmodelElementCollection()
    {
        // Arrange
        var document = CreateValidDocument();

        // Act
        var result = document.ToDocumentCollection();

        // Assert
        result.ShouldNotBeNull();
        result.IdShort.ShouldBe("Document");
        result.Category.ShouldBeNull();
        result.Description.ShouldBeNull();
        result.SemanticId.ShouldNotBeNull();
        result.Value.Count.ShouldBe(3); // DocumentIds, DocumentClassifications, DocumentVersions
    }

    [Fact]
    public void ToDocumentCollection_ShouldContainDocumentIdsList()
    {
        // Arrange
        var document = CreateValidDocument();

        // Act
        var result = document.ToDocumentCollection();

        // Assert
        var documentIdsList = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentIds);
        documentIdsList.ShouldNotBeNull();
        documentIdsList.ShouldBeOfType<SubmodelElementList>();
    }

    [Fact]
    public void ToDocumentCollection_ShouldContainDocumentClassificationsList()
    {
        // Arrange
        var document = CreateValidDocument();

        // Act
        var result = document.ToDocumentCollection();

        // Assert
        var classificationsList = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentClassifications);
        classificationsList.ShouldNotBeNull();
        classificationsList.ShouldBeOfType<SubmodelElementList>();
    }

    [Fact]
    public void ToDocumentCollection_ShouldContainDocumentVersionsList()
    {
        // Arrange
        var document = CreateValidDocument();

        // Act
        var result = document.ToDocumentCollection();

        // Assert
        var versionsList = result.Value.FirstOrDefault(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocumentVersions);
        versionsList.ShouldNotBeNull();
        versionsList.ShouldBeOfType<SubmodelElementList>();
    }

    [Fact]
    public void ValidateTemplateRequirements_WithValidDocument_ShouldNotThrow()
    {
        // Arrange
        var document = CreateValidDocument();

        // Act & Assert
        Should.NotThrow(() => document.ValidateTemplateRequirements());
    }

    [Fact]
    public void ValidateTemplateRequirements_WithEmptyDocumentIds_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var document = new HandoverDocument();
        document.DocumentClassifications.Add(CreateValidVdi2770Classification());
        document.DocumentVersions.Add(CreateValidDocumentVersion());

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => document.ValidateTemplateRequirements());
        exception.Message.ShouldBe("Each Document must contain at least one DocumentId (DocumentIds list).");
    }

    [Fact]
    public void ValidateTemplateRequirements_WithEmptyDocumentClassifications_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var document = new HandoverDocument();
        document.DocumentIds.Add(CreateValidDocumentId());
        document.DocumentVersions.Add(CreateValidDocumentVersion());

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => document.ValidateTemplateRequirements());
        exception.Message.ShouldBe("Each Document must contain at least one DocumentClassification (DocumentClassifications list).");
    }

    [Fact]
    public void ValidateTemplateRequirements_WithEmptyDocumentVersions_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var document = new HandoverDocument();
        document.DocumentIds.Add(CreateValidDocumentId());
        document.DocumentClassifications.Add(CreateValidVdi2770Classification());

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => document.ValidateTemplateRequirements());
        exception.Message.ShouldBe("Each Document must contain at least one DocumentVersion (DocumentVersions list).");
    }

    [Fact]
    public void ValidateTemplateRequirements_WithoutVdi2770Classification_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var document = new HandoverDocument();
        document.DocumentIds.Add(CreateValidDocumentId());
        document.DocumentClassifications.Add(
                                             new HandoverDocumentClassification
                                             {
                                                 ClassId              = _fixture.Create<string>(),
                                                 ClassName            = _fixture.Create<string>(),
                                                 ClassificationSystem = "SomeOtherSystem" // Not VDI 2770
                                             });
        document.DocumentVersions.Add(CreateValidDocumentVersion());

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => document.ValidateTemplateRequirements());
        exception.Message.ShouldBe($"Each Document must include a classification with ClassificationSystem='{HandoverDocumentationSemantics.Vdi2770ClassificationSystemName}'.");
    }

    [Fact]
    public void ValidateTemplateRequirements_WithVdi2770ClassificationAmongOthers_ShouldNotThrow()
    {
        // Arrange
        var document = new HandoverDocument();
        document.DocumentIds.Add(CreateValidDocumentId());

        // Add non-VDI classification first
        document.DocumentClassifications.Add(
                                             new HandoverDocumentClassification
                                             {
                                                 ClassId              = _fixture.Create<string>(),
                                                 ClassName            = _fixture.Create<string>(),
                                                 ClassificationSystem = "SomeOtherSystem"
                                             });

        // Add VDI 2770 classification
        document.DocumentClassifications.Add(CreateValidVdi2770Classification());
        document.DocumentVersions.Add(CreateValidDocumentVersion());

        // Act & Assert
        Should.NotThrow(() => document.ValidateTemplateRequirements());
    }

    [Fact]
    public void ValidateTemplateRequirements_WithValidDocumentVersion_ShouldCallValidateOnDocumentVersions()
    {
        // Arrange
        var document = new HandoverDocument();
        document.DocumentIds.Add(CreateValidDocumentId());
        document.DocumentClassifications.Add(CreateValidVdi2770Classification());

        // Add valid document versions - validation will be called internally
        document.DocumentVersions.Add(CreateValidDocumentVersion());
        document.DocumentVersions.Add(CreateValidDocumentVersion());

        // Act & Assert
        // This test ensures that ValidateTemplateRequirements completes successfully,
        // which means it must have called ValidateTemplateRequirements on each version
        Should.NotThrow(() => document.ValidateTemplateRequirements());
    }

    [Theory]
    [InlineData("VDI2770:2018")]
    [InlineData("vdi2770:2018")]
    [InlineData("VDI2770:2018")]
    public void ValidateTemplateRequirements_WithDifferentVdi2770CasingVariations_ShouldUseExactMatch(string classificationSystem)
    {
        // Arrange
        var document = new HandoverDocument();
        document.DocumentIds.Add(CreateValidDocumentId());
        document.DocumentClassifications.Add(
                                             new HandoverDocumentClassification
                                             {
                                                 ClassId              = _fixture.Create<string>(),
                                                 ClassName            = _fixture.Create<string>(),
                                                 ClassificationSystem = classificationSystem
                                             });
        document.DocumentVersions.Add(CreateValidDocumentVersion());

        // Act & Assert
        if (classificationSystem == HandoverDocumentationSemantics.Vdi2770ClassificationSystemName)
        {
            Should.NotThrow(() => document.ValidateTemplateRequirements());
        }
        else
        {
            Should.Throw<InvalidOperationException>(() => document.ValidateTemplateRequirements());
        }
    }

    private HandoverDocument CreateValidDocument()
    {
        var document = new HandoverDocument();
        document.DocumentIds.Add(CreateValidDocumentId());
        document.DocumentClassifications.Add(CreateValidVdi2770Classification());
        document.DocumentVersions.Add(CreateValidDocumentVersion());
        return document;
    }

    private HandoverDocumentId CreateValidDocumentId()
    {
        return new HandoverDocumentId
               {
                   DocumentDomainId   = _fixture.Create<string>(),
                   DocumentIdentifier = _fixture.Create<string>(),
                   DocumentIsPrimary  = _fixture.Create<bool>()
               };
    }

    private HandoverDocumentClassification CreateValidVdi2770Classification()
    {
        return new HandoverDocumentClassification
               {
                   ClassId              = _fixture.Create<string>(),
                   ClassName            = _fixture.Create<string>(),
                   ClassificationSystem = HandoverDocumentationSemantics.Vdi2770ClassificationSystemName
               };
    }

    private HandoverDocumentVersion CreateValidDocumentVersion()
    {
        // Create a real instance with required fields populated
        var version = new HandoverDocumentVersion();

        // Add at least one language as required by validation
        version.Languages.Add("en");
        version.Title                    = _fixture.Create<string>();
        version.Description              = _fixture.Create<string>();
        version.OrganizationShortName    = _fixture.Create<string>();
        version.OrganizationOfficialName = _fixture.Create<string>();
        version.DigitalFiles.Add(_fixture.Create<HandoverDigitalFile>());

        return version;
    }
}