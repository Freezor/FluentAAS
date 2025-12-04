using AasCore.Aas3_0;
using FluentAAS.Builder;
using FluentAas.IO;
using FluentAAS.IO;
using FluentAAS.Templates;
using Shouldly;
using File = System.IO.File;

namespace FluentAASTests.Integration;

/// <summary>
/// Contains integration tests for building a Digital Nameplate submodel
/// using the fluent FluentAAS builder API.
/// </summary>
public class DigitalNameplateTests
{
    /// <summary>
    /// Verifies that a Digital Nameplate submodel can be created and attached
    /// to an Asset Administration Shell using the fluent API.
    /// Every piece of content configured via the builder is asserted.
    /// </summary>
    [Fact]
    public void CanCreateDigitalNameplateWithFluentApi()
    {
        // Arrange & Act
        var environment = EnvironmentBuilder.Create()
                                            .AddShell("urn:aas:example:my-shell", "MyShell")
                                            .WithGlobalAssetId("urn:asset:example:my-asset")
                                            .AddDigitalNameplate("urn:submodel:example:digital-nameplate:V2_0")
                                            .WithManufacturerName("de", "Muster AG")
                                            .WithManufacturerName("en", "Sample Corp")
                                            .WithManufacturerProductDesignation("de", "Super-Antriebseinheit XS")
                                            .WithManufacturerProductDesignation("en", "Super Drive Unit XS")
                                            .WithSerialNumber("SN-000123")
                                            .Build()
                                            .Done()
                                            .Build();

        // Create De/Serializing environment
        var json               = AasJsonSerializer.ToJson(environment);
        var createdEnvironment = AasJsonSerializer.FromJson(json);

        // Create AASX package
        const string aasxPath = "./test.aasx";
        environment.ToAasx(aasxPath);
        var extractedAasEnvironment = AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath);

        File.Delete(aasxPath);

        // Assert: environment basics
        environment.ShouldNotBeNull();
        environment.AssetAdministrationShells.ShouldHaveSingleItem();
        environment.Submodels.ShouldHaveSingleItem();

        // Assert: shell and asset info
        var shell = environment.AssetAdministrationShells!.First();
        shell.IdShort.ShouldBe("MyShell");
        shell.AssetInformation.GlobalAssetId.ShouldBe("urn:asset:example:my-asset");

        // Assert: submodel basics
        var submodel = environment.Submodels!.First();
        submodel.IdShort.ShouldBe("DigitalNameplate");
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
    /// Verifies that the <see cref="DigitalNameplateBuilder"/> throws an
    /// <see cref="InvalidOperationException"/> when required fields are missing.
    /// </summary>
    [Fact]
    public void DigitalNameplateBuilder_ThrowsOnMissingRequiredFields()
    {
        // Arrange
        var builder = EnvironmentBuilder.Create()
                                        .AddShell("urn:aas:example:my-shell", "MyShell")
                                        .AddDigitalNameplate("urn:submodel:example:digital-nameplate:V2_0")
                                        .WithManufacturerName("de", "Muster AG");

        // Act
        var exception = Should.Throw<InvalidOperationException>(() => builder.Build());

        // Assert
        exception.Message.ShouldContain("Digital Nameplate");
        exception.Message.ShouldContain("ManufacturerProductDesignation");
        exception.Message.ShouldContain("SerialNumber");
    }

    private static Property? GetProperty(ISubmodel submodel, string idShort)
        => submodel.SubmodelElements!
                   .OfType<Property>()
                   .FirstOrDefault(p => p.IdShort == idShort);

    private static MultiLanguageProperty? GetMultiLanguageProperty(ISubmodel submodel, string idShort)
        => submodel.SubmodelElements!
                   .OfType<MultiLanguageProperty>()
                   .FirstOrDefault(p => p.IdShort == idShort);
}