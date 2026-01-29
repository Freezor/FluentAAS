using AasCore.Aas3_0;
using FluentAAS.Builder;
using FluentAas.IO;
using FluentAAS.IO;
using FluentAAS.Templates.DigitalNameplate;
using FluentAAS.Templates.HandoverDocumentation;
using Shouldly;
using File = System.IO.File;


namespace FluentAASTests.Integration;

/// <summary>
///     Contains integration tests for building a Digital Nameplate submodel
///     using the fluent FluentAAS builder API.
/// </summary>
public class FluentAasEndToEndTests
{
    /// <summary>
    ///     Verifies that a Digital Nameplate submodel can be created and attached
    ///     to an Asset Administration Shell using the fluent API.
    ///     Every piece of content configured via the builder is asserted.
    /// </summary>
    [Fact]
    public void CanCreateDigitalNameplateWithFluentApi()
    {
        // Arrange & Act
        var environment = AasBuilder.Create()
                                    .AddShell("urn:aas:example:my-shell", "MyShell")
                                    .WithGlobalAssetId("urn:asset:example:my-asset")
                                    .AddSubmodel("urn:aas:example:generic-submodel:1", "GenericSubmodel")
                                    .WithSemanticId(
                                                    new Reference(
                                                                  ReferenceTypes.ExternalReference,
                                                                  [
                                                                      new Key(
                                                                              KeyTypes.Submodel,
                                                                              "urn:aas:example:generic-submodel:semantic-id")
                                                                  ]
                                                                 ))
                                    .AddMultiLanguageProperty(
                                                              "GenericMultiLanguageProperty", ls => ls
                                                                                                    .Add("en", "Example value")
                                                                                                    .Add("de", "Beispielwert"))
                                    .AddElement("GenericProperty", "example value")
                                    .CompleteSubmodelConfiguration()
                                    .AddDigitalNameplate("urn:submodel:example:digital-nameplate:V2_0")
                                    .WithManufacturerName("de", "Muster AG")
                                    .WithManufacturerName("en", "Sample Corp")
                                    .WithManufacturerProductDesignation("de", "Super-Antriebseinheit XS")
                                    .WithManufacturerProductDesignation("en", "Super Drive Unit XS")
                                    .WithSerialNumber("SN-000123")
                                    .BuildDigitalNameplate()
                                    .CompleteShellConfiguration()
                                    .Build();

        // Create De/Serializing environment
        var json               = AasJsonSerializer.ToJson(environment);
        var createdEnvironment = AasJsonSerializer.FromJson(json);

        // Create AASX package
        const string aasxPath = "./test.aasx";
        environment.ToAasx(aasxPath, "/spec/uri/env.json");
        var extractedAasEnvironment = AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath);

        File.Delete(aasxPath);

        // Assert: environment basics
        environment.ShouldNotBeNull();
        environment.AssetAdministrationShells.ShouldHaveSingleItem();
        environment.Submodels!.Count.ShouldBe(2);

        // Assert: shell and asset info
        var shell = environment.AssetAdministrationShells!.First();
        shell.IdShort.ShouldBe("MyShell");
        shell.AssetInformation.GlobalAssetId.ShouldBe("urn:asset:example:my-asset");

        // Assert: submodel basics
        var submodel = environment.Submodels!.Find(x => x.IdShort!.Equals("DigitalNameplate"));
        submodel.ShouldNotBeNull();
        submodel.SubmodelElements!.Count.ShouldBe(3);

        // Assert: Digital Nameplate content
        var manuName = GetMultiLanguageProperty(submodel, "ManufacturerName");
        manuName.ShouldNotBeNull("ManufacturerName should be present");
        manuName.Value!.First(l => l.Language == "de").Text
                .ShouldBe("Muster AG");
        manuName.Value!.First(l => l.Language == "en").Text
                .ShouldBe("Sample Corp");

        // Manufacturer product designation (multi-language)
        var manuProdDesig = GetMultiLanguageProperty(submodel, "ManufacturerProductDesignation");
        manuProdDesig.ShouldNotBeNull("ManufacturerProductDesignation should be present");
        manuProdDesig.Value!.First(l => l.Language == "de").Text
                     .ShouldBe("Super-Antriebseinheit XS");
        manuProdDesig.Value!.First(l => l.Language == "en").Text
                     .ShouldBe("Super Drive Unit XS");

        // Serial number (string property)
        var serialNumber = GetProperty(submodel, "SerialNumber");
        serialNumber.ShouldNotBeNull("SerialNumber should be present");
        serialNumber.Value.ShouldBe("SN-000123");

        // Assert: round-trip JSON de/serialization
        createdEnvironment.ShouldBeEquivalentTo(
                                                environment,
                                                "Serializing and Deserializing the same object should result in identical objects");

        // Assert: round-trip AASX de/serialization
        extractedAasEnvironment.ShouldBeEquivalentTo(environment, "Packaging to .aasx and reading from the same object should result in identical objects");
    }

    /// <summary>
    ///     Verifies that the <see cref="DigitalNameplateBuilder" /> throws an
    ///     <see cref="InvalidOperationException" /> when required fields are missing.
    /// </summary>
    [Fact]
    public void DigitalNameplateBuilder_ThrowsOnMissingRequiredFields()
    {
        // Arrange
        var builder = AasBuilder.Create()
                                .AddShell("urn:aas:example:my-shell", "MyShell")
                                .AddDigitalNameplate("urn:submodel:example:digital-nameplate:V2_0")
                                .WithManufacturerName("de", "Muster AG");

        // Act
        var exception = Should.Throw<InvalidOperationException>(() => builder.BuildDigitalNameplate());

        // Assert
        exception.Message.ShouldContain("Digital Nameplate");
        exception.Message.ShouldContain("ManufacturerProductDesignation");
        exception.Message.ShouldContain("SerialNumber");
    }

    /// <summary>
    ///     Verifies that a Handover Documentation submodel can be created and attached
    ///     to an Asset Administration Shell using the fluent API.
    ///     Every piece of content configured via the builder is asserted.
    /// </summary>
    [Fact]
    public void CanCreateHandoverDocumentationWithFluentApi()
    {
        // Arrange & Act
        var testDate = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        
        var environment = AasBuilder.Create()
                                    .AddShell("urn:aas:example:my-shell", "MyShell")
                                    .WithGlobalAssetId("urn:asset:example:my-asset")
                                    .AddHandoverDocumentation("urn:submodel:example:handover-documentation:V2_0")
                                    .WithDescription("en", "Complete handover documentation for the asset")
                                    .WithDescription("de", "Vollständige Übergabedokumentation für das Asset")
                                    .WithCategory("INSTANCE")
                                    .AddDocument(doc => doc
                                            .AddDocumentId("URI", "DOC-001")
                                            .AddDocumentClassification("01-01", "Installation Manual")
                                            .WithTitle("Installation Manual Document")
                                            .WithDescription("A comprehensive installation manual document")
                                            .WithOrganization("CRM", "Customer Rally Management")
                                            .WithStatus(HandoverDocumentationSemantics.StatusValues.Released, testDate)
                                            .WithPreviewFile("path/to/preview.pdf", "application/pdf")
                                            .AddDocumentVersion(ver => ver
                                                                        .WithLanguage("en")
                                                                        .WithLanguage("de")
                                                                        .WithVersion("1.0")
                                                                        .WithTitle("Installation Manual")
                                                                        .WithDescription("Detailed installation instructions")
                                                                        .WithSubtitle("Quick Start Guide")
                                                                        .AddKeyword("installation")
                                                                        .AddKeyword("manual")
                                                                        .AddKeyword("anleitung", "de")
                                                                        .WithStatus("Released")
                                                                        .WithOrganization("CRM", "Customer Relations Management")
                                                                        .AddDigitalFile("installation_manual.pdf", "application/pdf")
                                                                        .AddDigitalFile("installation_guide.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                                                                        .WithPreviewFile("preview_thumbnail.jpg", "image/jpeg")))
                                .BuildHandoverDocumentation()
                                .CompleteShellConfiguration()
                                .Build();

        // Assert - Environment Structure
        environment.ShouldNotBeNull();
        environment.AssetAdministrationShells.ShouldNotBeNull();
        environment.AssetAdministrationShells.Count.ShouldBe(1);
        environment.Submodels.ShouldNotBeNull();
        environment.Submodels.Count.ShouldBe(1);

        // Assert - Shell Properties
        var shell = environment.AssetAdministrationShells.First();
        shell.Id.ShouldBe("urn:aas:example:my-shell");
        shell.IdShort.ShouldBe("MyShell");
        shell.AssetInformation.ShouldNotBeNull();
        shell.AssetInformation.GlobalAssetId.ShouldBe("urn:asset:example:my-asset");
        shell.Submodels.ShouldNotBeNull();
        shell.Submodels.Count.ShouldBe(1);

        // Assert - Submodel Properties
        var submodel = environment.Submodels.First();
        submodel.Id.ShouldBe("urn:submodel:example:handover-documentation:V2_0");
        submodel.IdShort.ShouldBe("HandoverDocumentation");
        submodel.Category.ShouldBe("INSTANCE");
        submodel.SemanticId.ShouldNotBeNull();
        submodel.SemanticId.Type.ShouldBe(ReferenceTypes.ExternalReference);
        submodel.SemanticId.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SubmodelSemanticId);

        // Assert - Submodel Descriptions
        submodel.Description.ShouldNotBeNull();
        submodel.Description.Count.ShouldBe(2);
        submodel.Description.ShouldContain(d => d.Language == "en" && d.Text == "Complete handover documentation for the asset");
        submodel.Description.ShouldContain(d => d.Language == "de" && d.Text == "Vollständige Übergabedokumentation für das Asset");

        // Assert - Documents List Structure
        submodel.SubmodelElements.ShouldNotBeNull();
        submodel.SubmodelElements.Count.ShouldBe(1);
        
        var documentsList = submodel.SubmodelElements.OfType<SubmodelElementList>()
                                  .Single(e => e.IdShort == HandoverDocumentationSemantics.IdShortDocuments);
        documentsList.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdDocuments);
        documentsList.TypeValueListElement.ShouldBe(AasSubmodelElements.SubmodelElementCollection);
        documentsList.Value.ShouldNotBeNull();
        documentsList.Value.Count.ShouldBe(1);

        // Assert - Document Structure
        var documentCollection = documentsList.Value.OfType<SubmodelElementCollection>().Single();
        documentCollection.IdShort.ShouldBe("Document");
        documentCollection.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdDocument);

        // Assert - Document IDs
        var documentIds = GetSubmodelElementList(documentCollection, HandoverDocumentationSemantics.IdShortDocumentIds);
        documentIds.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdDocumentIds);
        documentIds.Value!.Count.ShouldBe(1);
        
        var documentIdCollection = documentIds.Value.OfType<SubmodelElementCollection>().Single();
        GetPropertyLocal(documentIdCollection, HandoverDocumentationSemantics.IdShortDocumentDomainId).Value.ShouldBe("URI");
        GetPropertyLocal(documentIdCollection, HandoverDocumentationSemantics.IdShortDocumentIdentifier).Value.ShouldBe("DOC-001");
        GetPropertyLocal(documentIdCollection, HandoverDocumentationSemantics.IdShortDocumentIsPrimary).Value.ShouldBe("true");

        // Assert - Document Classifications
        var documentClassifications = GetSubmodelElementList(documentCollection, HandoverDocumentationSemantics.IdShortDocumentClassifications);
        documentClassifications.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdDocumentClassifications);
        documentClassifications.Value!.Count.ShouldBe(1);
        
        var classificationCollection = documentClassifications.Value.OfType<SubmodelElementCollection>().Single();
        GetPropertyLocal(classificationCollection, HandoverDocumentationSemantics.IdShortClassId).Value.ShouldBe("01-01");
        GetMultiLanguagePropertyLocal(classificationCollection, HandoverDocumentationSemantics.IdShortClassName)
            .Value!.ShouldContain(v => v.Language == "en" && v.Text == "Installation Manual");
        GetPropertyLocal(classificationCollection, HandoverDocumentationSemantics.IdShortClassificationSystem).Value.ShouldBe("VDI 2770 Blatt 1:2020");

        // Assert - Document Versions
        var documentVersions = GetSubmodelElementList(documentCollection, HandoverDocumentationSemantics.IdShortDocumentVersions);
        documentVersions.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdDocumentVersions);
        documentVersions.Value!.Count.ShouldBe(1);
        
        var versionCollection = documentVersions.Value.OfType<SubmodelElementCollection>().Single();
        
        // Assert - Version Language List
        var languageList = GetSubmodelElementList(versionCollection, HandoverDocumentationSemantics.IdShortLanguage);
        languageList.Value!.Count.ShouldBe(2);
        var languageProperties = languageList.Value.OfType<Property>().ToList();
        languageProperties.Select(p => p.Value).ShouldContain("en");
        languageProperties.Select(p => p.Value).ShouldContain("de");

        // Assert - Version Properties
        GetPropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortVersion).Value.ShouldBe("1.0");
        
        var titleProperty = GetMultiLanguagePropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortTitle);
        titleProperty.Value!.ShouldContain(v => v.Language == "en" && v.Text == "Installation Manual");
        
        var descriptionProperty = GetMultiLanguagePropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortDescription);
        descriptionProperty.Value!.ShouldContain(v => v.Language == "en" && v.Text == "Detailed installation instructions");
        
        var subtitleProperty = GetMultiLanguagePropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortSubtitle);
        subtitleProperty.Value!.ShouldContain(v => v.Language == "en" && v.Text == "Quick Start Guide");
        
        var keywordsProperty = GetMultiLanguagePropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortKeyWords);
        keywordsProperty.Value!.ShouldContain(v => v.Language == "en" && v.Text == "installation");
        keywordsProperty.Value!.ShouldContain(v => v.Language == "en" && v.Text == "manual");
        keywordsProperty.Value!.ShouldContain(v => v.Language == "de" && v.Text == "anleitung");

        GetPropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortStatusValue).Value.ShouldBe("Released");
        GetPropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortOrganizationShortName).Value.ShouldBe("CRM");
        GetPropertyLocal(versionCollection, HandoverDocumentationSemantics.IdShortOrganizationOfficialName).Value.ShouldBe("Customer Relations Management");

        // Assert - Digital Files
        var digitalFilesList = GetSubmodelElementList(versionCollection, HandoverDocumentationSemantics.IdShortDigitalFiles);
        digitalFilesList.Value!.Count.ShouldBe(2);
        
        var digitalFiles = digitalFilesList.Value.OfType<AasCore.Aas3_0.File>().ToList();
        digitalFiles.ShouldContain(f => f.Value == "installation_manual.pdf" && f.ContentType == "application/pdf");
        digitalFiles.ShouldContain(f => f.Value == "installation_guide.docx" && f.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

        // Assert - Preview File
        var previewFile = GetFile(versionCollection, HandoverDocumentationSemantics.IdShortPreviewFile);
        previewFile.Value.ShouldBe("preview_thumbnail.jpg");
        previewFile.ContentType.ShouldBe("image/jpeg");
        previewFile.SemanticId!.Keys.Single().Value.ShouldBe(HandoverDocumentationSemantics.SemanticIdPreviewFile);
    }

    private static AasCore.Aas3_0.File GetFile(SubmodelElementCollection collection, string idShort) => collection.Value!.OfType<AasCore.Aas3_0.File>().Single(p => p.IdShort == idShort);

    private static SubmodelElementList GetSubmodelElementList(SubmodelElementCollection collection, string idShort) => collection.Value!.OfType<SubmodelElementList>().Single(p => p.IdShort == idShort);

    private static MultiLanguageProperty GetMultiLanguagePropertyLocal(SubmodelElementCollection collection, string idShort) => collection.Value!.OfType<MultiLanguageProperty>().Single(p => p.IdShort == idShort);

    private static Property GetPropertyLocal(SubmodelElementCollection collection, string idShort) => collection.Value!.OfType<Property>().Single(p => p.IdShort == idShort);

    /// <summary>
    ///     Verifies that the HandoverDocumentationSubmodelBuilder throws an
    ///     InvalidOperationException when no documents are added.
    /// </summary>
    [Fact]
    public void HandoverDocumentationBuilder_ThrowsOnMissingRequiredDocuments()
    {
        // Arrange
        var builder = AasBuilder.Create()
                                .AddShell("urn:aas:example:my-shell", "MyShell")
                                .AddHandoverDocumentation("urn:submodel:example:handover-documentation:V2_0")
                                .WithDescription("en", "Test documentation");

        // Act
        var exception = Should.Throw<InvalidOperationException>(() => builder.BuildHandoverDocumentation());

        // Assert
        exception.Message.ShouldContain("Handover Documentation");
        exception.Message.ShouldContain("Document");
    }

    private static Property? GetProperty(ISubmodel submodel, string idShort)
    {
        return submodel.SubmodelElements!
                       .OfType<Property>()
                       .FirstOrDefault(p => p.IdShort == idShort);
    }

    private static MultiLanguageProperty? GetMultiLanguageProperty(ISubmodel submodel, string idShort)
    {
        return submodel.SubmodelElements!
                       .OfType<MultiLanguageProperty>()
                       .FirstOrDefault(p => p.IdShort == idShort);
    }
}