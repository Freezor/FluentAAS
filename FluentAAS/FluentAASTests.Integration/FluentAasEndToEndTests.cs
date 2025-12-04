using FluentAAS.Builder;
using FluentAas.IO;
using FluentAAS.Templates;
using Shouldly;

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

        var json               = AasJsonSerializer.ToJson(environment);
        var createdEnvironment = AasJsonSerializer.FromJson(json);

        // Assert
        environment.ShouldNotBeNull();
        environment.AssetAdministrationShells.ShouldHaveSingleItem();
        environment.Submodels.ShouldHaveSingleItem();

        var shell = environment.AssetAdministrationShells!.First();
        shell.IdShort.ShouldBe("MyShell");
        shell.AssetInformation.GlobalAssetId.ShouldBe("urn:asset:example:my-asset");

        var submodel = environment.Submodels!.First();
        submodel.IdShort.ShouldBe("DigitalNameplate");
        submodel.SubmodelElements!.Count.ShouldBe(3);

        createdEnvironment.ShouldBeEquivalentTo(environment, "Serializing and Deserializing the same object should result in identical objects");
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
}