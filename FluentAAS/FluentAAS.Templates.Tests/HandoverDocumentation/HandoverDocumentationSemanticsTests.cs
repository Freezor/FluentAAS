using FluentAAS.Templates.HandoverDocumentation;
using JetBrains.Annotations;
using Shouldly;

namespace FluentAAS.Templates.Tests.HandoverDocumentation;

[TestSubject(typeof(HandoverDocumentationSemantics))]
public class HandoverDocumentationSemanticsTests
{
    #region Submodel Constants Tests

    [Fact]
    public void SubmodelIdShort_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SubmodelIdShort.ShouldBe("HandoverDocumentation");
    }

    [Fact]
    public void SubmodelSemanticId_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SubmodelSemanticId.ShouldBe("0173-1#01-AHF578#003");
    }

    #endregion

    #region Root Elements Constants Tests

    [Fact]
    public void IdShortDocuments_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortDocuments.ShouldBe("Documents");
    }

    [Fact]
    public void SemanticIdDocuments_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocuments.ShouldBe("0173-1#02-ABI500#003");
    }

    [Fact]
    public void IdShortEntities_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortEntities.ShouldBe("Entities");
    }

    [Fact]
    public void SemanticIdEntities_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdEntities.ShouldBe("https://admin-shell.io/vdi/2770/1/0/EntitiesForDocumentation");
    }

    [Fact]
    public void SemanticIdDocument_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocument.ShouldBe("0173-1#02-ABI500#003/0173-1#01-AHF579#003");
    }

    #endregion

    #region Document SMC Children Constants Tests

    [Fact]
    public void IdShortDocumentIds_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortDocumentIds.ShouldBe("DocumentIds");
    }

    [Fact]
    public void SemanticIdDocumentIds_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentIds.ShouldBe("0173-1#02-ABI501#003");
    }

    [Fact]
    public void SemanticIdDocumentId_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentId.ShouldBe("0173-1#02-ABI501#003/0173-1#01-AHF580#003");
    }

    [Fact]
    public void IdShortDocumentClassifications_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortDocumentClassifications.ShouldBe("DocumentClassifications");
    }

    [Fact]
    public void SemanticIdDocumentClassifications_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentClassifications.ShouldBe("0173-1#02-ABI502#003");
    }

    [Fact]
    public void SemanticIdDocumentClassification_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentClassification.ShouldBe("0173-1#02-ABI502#003/0173-1#01-AHF581#003");
    }

    [Fact]
    public void IdShortDocumentVersions_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortDocumentVersions.ShouldBe("DocumentVersions");
    }

    [Fact]
    public void SemanticIdDocumentVersions_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentVersions.ShouldBe("0173-1#02-ABI503#003");
    }

    [Fact]
    public void SemanticIdDocumentVersion_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentVersion.ShouldBe("0173-1#02-ABI503#003/0173-1#01-AHF582#003");
    }

    [Fact]
    public void IdShortDocumentedEntities_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortDocumentedEntities.ShouldBe("DocumentedEntities");
    }

    [Fact]
    public void SemanticIdDocumentedEntities_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentedEntities.ShouldBe("0173-1#02-ABI504#003");
    }

    [Fact]
    public void SemanticIdDocumentedEntity_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentedEntity.ShouldBe("0173-1#02-ABI504#003/0173-1#01-AHF583#003");
    }

    #endregion

    #region Document ID Fields Constants Tests

    [Fact]
    public void IdShortDocumentDomainId_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortDocumentDomainId.ShouldBe("DocumentDomainId");
    }

    [Fact]
    public void SemanticIdDocumentDomainId_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentDomainId.ShouldBe("0173-1#02-ABH994#003");
    }

    [Fact]
    public void IdShortDocumentIdentifier_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortDocumentIdentifier.ShouldBe("DocumentIdentifier");
    }

    [Fact]
    public void SemanticIdDocumentIdentifier_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentIdentifier.ShouldBe("0173-1#02-AAO099#004");
    }

    [Fact]
    public void IdShortDocumentIsPrimary_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortDocumentIsPrimary.ShouldBe("DocumentIsPrimary");
    }

    [Fact]
    public void SemanticIdDocumentIsPrimary_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDocumentIsPrimary.ShouldBe("0173-1#02-ABH995#003");
    }

    #endregion

    #region Document Classification Fields Constants Tests

    [Fact]
    public void IdShortClassId_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortClassId.ShouldBe("ClassId");
    }

    [Fact]
    public void SemanticIdClassId_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdClassId.ShouldBe("0173-1#02-AAO107#005");
    }

    [Fact]
    public void IdShortClassName_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortClassName.ShouldBe("ClassName");
    }

    [Fact]
    public void SemanticIdClassName_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdClassName.ShouldBe("0173-1#02-AAO108#005");
    }

    [Fact]
    public void IdShortClassificationSystem_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortClassificationSystem.ShouldBe("ClassificationSystem");
    }

    [Fact]
    public void SemanticIdClassificationSystem_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdClassificationSystem.ShouldBe("0173-1#02-AAO109#005");
    }

    [Fact]
    public void Vdi2770ClassificationSystemName_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.Vdi2770ClassificationSystemName.ShouldBe("VDI 2770 Blatt 1:2020");
    }

    #endregion

    #region Document Version Fields Constants Tests

    [Fact]
    public void IdShortLanguage_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortLanguage.ShouldBe("Language");
    }

    [Fact]
    public void SemanticIdLanguage_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdLanguage.ShouldBe("0173-1#02-AAN468#008");
    }

    [Fact]
    public void IdShortVersion_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortVersion.ShouldBe("Version");
    }

    [Fact]
    public void SemanticIdVersion_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdVersion.ShouldBe("0173-1#02-AAP003#005");
    }

    [Fact]
    public void IdShortTitle_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortTitle.ShouldBe("Title");
    }

    [Fact]
    public void SemanticIdTitle_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdTitle.ShouldBe("0173-1#02-AAO105#005");
    }

    [Fact]
    public void IdShortSubtitle_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortSubtitle.ShouldBe("Subtitle");
    }

    [Fact]
    public void SemanticIdSubtitle_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdSubtitle.ShouldBe("0173-1#02-AAO106#005");
    }

    [Fact]
    public void IdShortDescription_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortDescription.ShouldBe("Description");
    }

    [Fact]
    public void SemanticIdDescription_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDescription.ShouldBe("0173-1#02-AAO111#005");
    }

    [Fact]
    public void IdShortKeyWords_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortKeyWords.ShouldBe("KeyWords");
    }

    [Fact]
    public void SemanticIdKeyWords_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdKeyWords.ShouldBe("0173-1#02-AAO112#005");
    }

    [Fact]
    public void IdShortStatusSetDate_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortStatusSetDate.ShouldBe("StatusSetDate");
    }

    [Fact]
    public void SemanticIdStatusSetDate_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdStatusSetDate.ShouldBe("0173-1#02-AAO113#005");
    }

    [Fact]
    public void IdShortStatusValue_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortStatusValue.ShouldBe("StatusValue");
    }

    [Fact]
    public void SemanticIdStatusValue_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdStatusValue.ShouldBe("0173-1#02-AAO114#005");
    }

    [Fact]
    public void IdShortOrganizationShortName_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortOrganizationShortName.ShouldBe("OrganizationShortName");
    }

    [Fact]
    public void SemanticIdOrganizationShortName_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdOrganizationShortName.ShouldBe("0173-1#02-AAO115#005");
    }

    [Fact]
    public void IdShortOrganizationOfficialName_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortOrganizationOfficialName.ShouldBe("OrganizationOfficialName");
    }

    [Fact]
    public void SemanticIdOrganizationOfficialName_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdOrganizationOfficialName.ShouldBe("0173-1#02-AAO116#005");
    }

    #endregion

    #region Relationships Inside Document Version Constants Tests

    [Fact]
    public void IdShortRefersToEntities_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortRefersToEntities.ShouldBe("RefersToEntities");
    }

    [Fact]
    public void SemanticIdRefersToEntities_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdRefersToEntities.ShouldBe("0173-1#02-ABI505#003");
    }

    [Fact]
    public void IdShortBasedOnReferences_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortBasedOnReferences.ShouldBe("BasedOnReferences");
    }

    [Fact]
    public void SemanticIdBasedOnReferences_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdBasedOnReferences.ShouldBe("0173-1#02-ABI506#003");
    }

    [Fact]
    public void IdShortTranslationOfEntities_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortTranslationOfEntities.ShouldBe("TranslationOfEntities");
    }

    [Fact]
    public void SemanticIdTranslationOfEntities_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdTranslationOfEntities.ShouldBe("0173-1#02-ABI507#003");
    }

    #endregion

    #region Digital Files Constants Tests

    [Fact]
    public void IdShortDigitalFiles_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortDigitalFiles.ShouldBe("DigitalFiles");
    }

    [Fact]
    public void SemanticIdDigitalFiles_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdDigitalFiles.ShouldBe("0173-1#02-ABK126#002");
    }

    [Fact]
    public void IdShortPreviewFile_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.IdShortPreviewFile.ShouldBe("PreviewFile");
    }

    [Fact]
    public void SemanticIdPreviewFile_ShouldHaveCorrectValue()
    {
        HandoverDocumentationSemantics.SemanticIdPreviewFile.ShouldBe("0173-1#02-ABK127#002");
    }

    #endregion

    #region Semantic ID Pattern Validation Tests

    [Theory]
    [InlineData(HandoverDocumentationSemantics.SubmodelSemanticId)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdDocuments)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdDocumentIds)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdDocumentClassifications)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdDocumentVersions)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdDocumentedEntities)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdDocumentDomainId)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdDocumentIdentifier)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdDocumentIsPrimary)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdClassId)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdClassName)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdClassificationSystem)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdLanguage)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdVersion)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdTitle)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdSubtitle)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdDescription)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdKeyWords)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdStatusSetDate)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdStatusValue)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdOrganizationShortName)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdOrganizationOfficialName)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdRefersToEntities)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdBasedOnReferences)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdTranslationOfEntities)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdDigitalFiles)]
    [InlineData(HandoverDocumentationSemantics.SemanticIdPreviewFile)]
    public void SemanticIds_ShouldFollowEclassPattern_WhenApplicable(string semanticId)
    {
        // Most semantic IDs should follow the eCLASS pattern: 0173-1#XX-XXXXXXX#XXX
        // Some may be URLs or have different patterns
        if (semanticId.StartsWith("0173-1#"))
        {
            // Validate eCLASS pattern - more flexible to handle varying lengths
            // Pattern: 0173-1#XX-[alphanumeric]{6,7}#XXX with optional compound parts
            semanticId.ShouldMatch(@"^0173-1#\d{2}-[A-Z0-9]{6,7}#\d{3}(/0173-1#\d{2}-[A-Z0-9]{6,7}#\d{3})*$");
        }
        else if (semanticId.StartsWith("https://"))
        {
            // Validate URL pattern
            semanticId.ShouldStartWith("https://");
        }
        
        // All semantic IDs should be non-empty
        semanticId.ShouldNotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(HandoverDocumentationSemantics.IdShortDocuments)]
    [InlineData(HandoverDocumentationSemantics.IdShortEntities)]
    [InlineData(HandoverDocumentationSemantics.IdShortDocumentIds)]
    [InlineData(HandoverDocumentationSemantics.IdShortDocumentClassifications)]
    [InlineData(HandoverDocumentationSemantics.IdShortDocumentVersions)]
    [InlineData(HandoverDocumentationSemantics.IdShortDocumentedEntities)]
    [InlineData(HandoverDocumentationSemantics.IdShortDocumentDomainId)]
    [InlineData(HandoverDocumentationSemantics.IdShortDocumentIdentifier)]
    [InlineData(HandoverDocumentationSemantics.IdShortDocumentIsPrimary)]
    [InlineData(HandoverDocumentationSemantics.IdShortClassId)]
    [InlineData(HandoverDocumentationSemantics.IdShortClassName)]
    [InlineData(HandoverDocumentationSemantics.IdShortClassificationSystem)]
    [InlineData(HandoverDocumentationSemantics.IdShortLanguage)]
    [InlineData(HandoverDocumentationSemantics.IdShortVersion)]
    [InlineData(HandoverDocumentationSemantics.IdShortTitle)]
    [InlineData(HandoverDocumentationSemantics.IdShortSubtitle)]
    [InlineData(HandoverDocumentationSemantics.IdShortDescription)]
    [InlineData(HandoverDocumentationSemantics.IdShortKeyWords)]
    [InlineData(HandoverDocumentationSemantics.IdShortStatusSetDate)]
    [InlineData(HandoverDocumentationSemantics.IdShortStatusValue)]
    [InlineData(HandoverDocumentationSemantics.IdShortOrganizationShortName)]
    [InlineData(HandoverDocumentationSemantics.IdShortOrganizationOfficialName)]
    [InlineData(HandoverDocumentationSemantics.IdShortRefersToEntities)]
    [InlineData(HandoverDocumentationSemantics.IdShortBasedOnReferences)]
    [InlineData(HandoverDocumentationSemantics.IdShortTranslationOfEntities)]
    [InlineData(HandoverDocumentationSemantics.IdShortDigitalFiles)]
    [InlineData(HandoverDocumentationSemantics.IdShortPreviewFile)]
    public void IdShorts_ShouldFollowPascalCaseNamingConvention(string idShort)
    {
        // IdShort should start with uppercase letter
        char.IsUpper(idShort[0]).ShouldBeTrue($"IdShort '{idShort}' should start with uppercase letter");
        
        // Should not contain spaces or special characters (basic validation)
        idShort.ShouldNotContain(" ");
        idShort.ShouldNotBeNullOrEmpty();
    }

    #endregion

    #region Cross-Reference Validation Tests

    [Fact]
    public void CompoundSemanticIds_ShouldContainBaseSemanticId()
    {
        // Document semantic ID should be based on Documents semantic ID
        HandoverDocumentationSemantics.SemanticIdDocument
            .ShouldStartWith(HandoverDocumentationSemantics.SemanticIdDocuments + "/");

        // DocumentId semantic ID should be based on DocumentIds semantic ID
        HandoverDocumentationSemantics.SemanticIdDocumentId
            .ShouldStartWith(HandoverDocumentationSemantics.SemanticIdDocumentIds + "/");

        // DocumentClassification semantic ID should be based on DocumentClassifications semantic ID
        HandoverDocumentationSemantics.SemanticIdDocumentClassification
            .ShouldStartWith(HandoverDocumentationSemantics.SemanticIdDocumentClassifications + "/");

        // DocumentVersion semantic ID should be based on DocumentVersions semantic ID
        HandoverDocumentationSemantics.SemanticIdDocumentVersion
            .ShouldStartWith(HandoverDocumentationSemantics.SemanticIdDocumentVersions + "/");

        // DocumentedEntity semantic ID should be based on DocumentedEntities semantic ID
        HandoverDocumentationSemantics.SemanticIdDocumentedEntity
            .ShouldStartWith(HandoverDocumentationSemantics.SemanticIdDocumentedEntities + "/");
    }

    #endregion
}