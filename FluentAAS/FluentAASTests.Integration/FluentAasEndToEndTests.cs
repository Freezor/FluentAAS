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
    var environment = AasBuilder.Create()
                                    .AddShell("urn:aas:example:my-shell", "MyShell")
                                    .WithGlobalAssetId("urn:asset:example:my-asset")
                                    .AddHandoverDocumentation("urn:submodel:example:handover-documentation:V2_0")
                                    .WithDescription("en", "Complete handover documentation for the asset")
                                    .WithDescription("de", "Vollständige Übergabedokumentation für das Asset")
                                    .WithCategory("INSTANCE")
                                    .AddDocument(doc => doc
                                                        .AddDocumentId("URI", "DOC-001", true)
                                                        .AddDocumentClassification("01-01", "Installation Manual", "VDI 2770 Blatt 1:2020", "en")
                                                        .AddDocumentVersion(ver => ver
                                                                                    .WithLanguage("en")
                                                                                    .WithVersion("1.0")
                                                                                    .WithTitle("Installation Manual")
                                                                                    .WithStatus("Released")
                                                                                    .AddDigitalFile("installation_manual.pdf", "application/pdf")
                                                                                    .WithDescription("This is a document")
                                                                                    .WithOrganization("CRM","Customer Relations"))
                                                        .WithDescription("A document")
                                                        .WithOrganization("CRM", "Customer Rally Management")
                                                        .WithStatus(HandoverDocumentationSemantics.StatusValues.Released, DateTime.Now)
                                                        .WithPreviewFile("path/to/file")
                                                        .WithTitle("A new document"))
                                    .AddDocument(doc => doc
                                                        .AddDocumentId("URI", "DOC-002", true)
                                                        .AddDocumentClassification("01-02", "Certification Manual", "VDI 2770 Blatt 1:2020", "en")
                                                        .AddDocumentVersion(ver => ver
                                                                                   .WithLanguage("en")
                                                                                   .WithVersion("1.0")
                                                                                   .WithTitle("Certification Manual")
                                                                                   .WithStatus("Released")
                                                                                   .AddDigitalFile("certification_manual.pdf", "application/pdf")
                                                                                   .WithDescription("This is a document")
                                                                                   .WithOrganization("CRM","Customer Relations"))
                                                        .WithDescription("A Certification")
                                                        .WithOrganization("CRM", "Customer Rally Management")
                                                        .WithStatus(HandoverDocumentationSemantics.StatusValues.Released, DateTime.Now)
                                                        .WithPreviewFile("path/to/file")
                                                        .WithTitle("A new Certification document"))
                                    .BuildHandoverDocumentation()
                                    .CompleteShellConfiguration()
                                    .Build();

        // Create De/Serializing environment
        var json = AasJsonSerializer.ToJson(environment);
        var createdEnvironment = AasJsonSerializer.FromJson(json);

        // Create AASX package
        const string aasxPath = "./test_handover.aasx";
        environment.ToAasx(aasxPath, "/spec/uri/env.json");
        var extractedAasEnvironment = AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath);

        File.Delete(aasxPath);

        // Assert: environment basics
        environment.ShouldNotBeNull();
        environment.AssetAdministrationShells.ShouldHaveSingleItem();
        environment.Submodels!.Count.ShouldBe(1);

        // Assert: shell and asset info
        var shell = environment.AssetAdministrationShells!.First();
        shell.IdShort.ShouldBe("MyShell");
        shell.AssetInformation.GlobalAssetId.ShouldBe("urn:asset:example:my-asset");

        // Assert: submodel basics
        var submodel = environment.Submodels!.Find(x => x.IdShort!.Equals("HandoverDocumentation"));
        submodel.ShouldNotBeNull();
        submodel.Category.ShouldBe("INSTANCE");

        // Assert: submodel description
        submodel.Description.ShouldNotBeNull();
        submodel.Description.Count.ShouldBe(2);
        var englishDesc = submodel.Description.First(d => d.Language == "en");
        englishDesc.Text.ShouldBe("Complete handover documentation for the asset");
        var germanDesc = submodel.Description.First(d => d.Language == "de");
        germanDesc.Text.ShouldBe("Vollständige Übergabedokumentation für das Asset");

        // Assert: Documents list exists
        var documentsList = submodel.SubmodelElements!
            .OfType<SubmodelElementList>()
            .FirstOrDefault(sml => sml.IdShort == "Documents");
        documentsList.ShouldNotBeNull("Documents SubmodelElementList should be present");
        documentsList.Value!.Count.ShouldBe(2, "Should contain 2 documents");

        // Assert: round-trip JSON de/serialization
        createdEnvironment.ShouldBeEquivalentTo(
            environment,
            "Serializing and Deserializing the same object should result in identical objects");

        // Assert: round-trip AASX de/serialization
        extractedAasEnvironment.ShouldBeEquivalentTo(
            environment, 
            "Packaging to .aasx and reading from the same object should result in identical objects");
    }

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

    private static Property? GetPropertyFromCollection(SubmodelElementCollection collection, string idShort)
    {
        return collection.Value!
                    .OfType<Property>()
                    .FirstOrDefault(p => p.IdShort == idShort);
    }

    private static MultiLanguageProperty? GetMultiLanguagePropertyFromCollection(SubmodelElementCollection collection, string idShort)
    {
        return collection.Value!
                    .OfType<MultiLanguageProperty>()
                    .FirstOrDefault(p => p.IdShort == idShort);
    }

    private static SubmodelElementList? GetSubmodelElementListFromCollection(SubmodelElementCollection collection, string idShort)
    {
        return collection.Value!
                    .OfType<SubmodelElementList>()
                    .FirstOrDefault(p => p.IdShort == idShort);
    }
}