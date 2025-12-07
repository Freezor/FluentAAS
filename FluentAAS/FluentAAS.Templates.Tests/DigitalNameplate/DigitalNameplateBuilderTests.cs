using AasCore.Aas3_0;
using FluentAAS.Builder;
using FluentAAS.Templates.DigitalNameplate;
using Shouldly;
using static FluentAAS.Templates.DigitalNameplate.DigitalNameplateSemantics;

namespace FluentAAS.Templates.Tests.DigitalNameplate;

/// <summary>
/// Unit tests for <see cref="DigitalNameplateBuilder"/>.
/// </summary>
public class DigitalNameplateBuilderTests
{
    private static (ShellBuilder shellBuilder, DigitalNameplateBuilder sut, Func<IEnvironment> buildEnvironment)
        CreateBuilder(string? id = null, string? idShort = null)
    {
        var envBuilder  = AasBuilder.Create();
        var shellId     = "urn:aas:example:shell:" + Guid.NewGuid().ToString("N");
        var shellIdShort = "Shell-" + Guid.NewGuid().ToString("N")[..6];

        var shellBuilder = envBuilder.AddShell(shellId, shellIdShort);

        id      ??= "urn:aas:example:digital-nameplate:" + Guid.NewGuid().ToString("N");
        idShort ??= "DigitalNameplate-" + Guid.NewGuid().ToString("N")[..6];

        var sut = new DigitalNameplateBuilder(shellBuilder, id, idShort);

        return (shellBuilder, sut, () => envBuilder.Build());
    }
    [Fact]
    public void Constructor_ShouldThrow_WhenShellBuilderIsNull()
    {
        // Act
        var ex = Record.Exception(() => new DigitalNameplateBuilder(null!, "id", "idShort"));

        // Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<ArgumentNullException>();
        ((ArgumentNullException)ex).ParamName.ShouldBe("shellBuilder");
    }

    [Theory]
    [InlineData(null!, "idShort")]
    [InlineData("", "idShort")]
    [InlineData("   ", "idShort")]
    public void Constructor_ShouldThrow_WhenIdIsInvalid(string? id, string idShort)
    {
        var envBuilder   = AasBuilder.Create();
        var shellBuilder = envBuilder.AddShell("urn:aas:shell", "Shell");

        var ex = Record.Exception(() => new DigitalNameplateBuilder(shellBuilder, id!, idShort));

        ex.ShouldBeOfType<ArgumentException>();
        ex.Message.ShouldContain("Submodel id must not be empty.");
    }

    [Theory]
    [InlineData("id", null!)]
    [InlineData("id", "")]
    [InlineData("id", "   ")]
    public void Constructor_ShouldThrow_WhenIdShortIsInvalid(string id, string? idShort)
    {
        var envBuilder   = AasBuilder.Create();
        var shellBuilder = envBuilder.AddShell("urn:aas:shell", "Shell");

        var ex = Record.Exception(() => new DigitalNameplateBuilder(shellBuilder, id, idShort!));

        ex.ShouldBeOfType<ArgumentException>();
        ex.Message.ShouldContain("Submodel idShort must not be empty.");
    }

    [Fact]
    public void WithIds_ShouldUpdateIdsUsedInSubmodel()
    {
        // Arrange
        var (_, sut, buildEnv) = CreateBuilder("old-id", "old-short");
        const string newId      = "urn:aas:example:new-id";
        const string newIdShort = "NewShort";

        sut.WithManufacturerName("en", "ACME")
           .WithManufacturerProductDesignation("en", "Super Unit")
           .WithSerialNumber("SN-123");

        // Act
        sut.WithIds(newId, newIdShort).Build();
        var env = buildEnv();

        // Assert
        env.Submodels.ShouldNotBeNull();
        var submodel = env.Submodels.Single(sm => sm.Id == newId);
        submodel.IdShort.ShouldBe(newIdShort);

        submodel.SemanticId.ShouldNotBeNull();
        submodel.SemanticId!.Type.ShouldBe(ReferenceTypes.ExternalReference);
        submodel.SemanticId.Keys.Single().Value.ShouldBe(SubmodelDigitalNameplate);
    }

    [Fact]
    public void Build_ShouldThrow_WhenMandatoryFieldsAreMissing_All()
    {
        // Arrange
        var (_, sut, _) = CreateBuilder();

        // Act
        var ex = Record.Exception(() => sut.Build());

        // Assert
        ex.ShouldBeOfType<InvalidOperationException>();
        ex.Message.ShouldContain("ManufacturerName");
        ex.Message.ShouldContain("ManufacturerProductDesignation");
        ex.Message.ShouldContain("SerialNumber");
    }

    [Fact]
    public void Build_ShouldThrow_WhenSerialNumberMissing()
    {
        // Arrange
        var (_, sut, _) = CreateBuilder();

        sut.WithManufacturerName("en", "ACME")
           .WithManufacturerProductDesignation("en", "Super Unit");

        // Act
        var ex = Record.Exception(() => sut.Build());

        // Assert
        ex.ShouldBeOfType<InvalidOperationException>();
        ex.Message.ShouldContain("SerialNumber");
    }

    [Fact]
    public void WithManufacturerName_ShouldThrow_WhenArgumentsInvalid()
    {
        var (_, sut, _) = CreateBuilder();

        var ex1 = Record.Exception(() => sut.WithManufacturerName("  ", "ACME"));
        ex1.ShouldBeOfType<ArgumentException>();
        ex1.Message.ShouldContain("Language must not be empty.");

        var ex2 = Record.Exception(() => sut.WithManufacturerName("en", "  "));
        ex2.ShouldBeOfType<ArgumentException>();
        ex2.Message.ShouldContain("Manufacturer name must not be empty.");
    }

    [Fact]
    public void WithSerialNumber_ShouldThrow_WhenInvalid()
    {
        var (_, sut, _) = CreateBuilder();

        var ex = Record.Exception(() => sut.WithSerialNumber("  "));
        ex.ShouldBeOfType<ArgumentException>();
        ex.Message.ShouldContain("Serial number must not be empty.");
    }

    [Fact]
    public void Build_ShouldCreateSubmodel_WithMandatoryElements_AndAttachToShell()
    {
        // Arrange
        var (shell, sut, buildEnv) = CreateBuilder("urn:aas:example:dnp", "DNP");

        sut.WithManufacturerName("en", "ACME Inc.")
           .WithManufacturerProductDesignation("en", "Super Motor")
           .WithSerialNumber("SN-XYZ-001");

        // Act
        var returnedShell = sut.Build();
        var env           = buildEnv();

        // Assert
        returnedShell.ShouldBeSameAs(shell);

        env.Submodels.ShouldNotBeNull();
        var submodel = env.Submodels.Single(sm => sm.Id == "urn:aas:example:dnp");
        submodel.IdShort.ShouldBe("DNP");

        // ManufacturerName
        var mName = submodel.SubmodelElements!
                            .OfType<MultiLanguageProperty>()
                            .Single(e => e.IdShort == "ManufacturerName");

        mName.Value!.ShouldContain(ls => ls.Language == "en" && ls.Text == "ACME Inc.");
        mName.SemanticId.ShouldNotBeNull();
        mName.SemanticId!.Keys.Single().Value.ShouldBe(ManufacturerName);

        // ManufacturerProductDesignation
        var mProd = submodel.SubmodelElements!
                            .OfType<MultiLanguageProperty>()
                            .Single(e => e.IdShort == "ManufacturerProductDesignation");

        mProd.Value!.ShouldContain(ls => ls.Language == "en" && ls.Text == "Super Motor");
        mProd.SemanticId.ShouldNotBeNull();
        mProd.SemanticId!.Keys.Single().Value.ShouldBe(ManufacturerProductDesignation);

        // SerialNumber
        var sn = submodel.SubmodelElements!
                         .OfType<Property>()
                         .Single(e => e.IdShort == "SerialNumber");

        sn.Value.ShouldBe("SN-XYZ-001");
        sn.SemanticId.ShouldNotBeNull();
        sn.SemanticId!.Keys.Single().Value.ShouldBe(SerialNumber);

        // Shell reference is present
        shell.Shell.Submodels.ShouldNotBeNull();
        shell.Shell.Submodels!.Count.ShouldBe(1);
    }

    [Fact]
    public void Build_ShouldAddOptionalScalarProperties_WhenProvided()
    {
        // Arrange
        var (_, sut, buildEnv) = CreateBuilder();

        sut.WithManufacturerName("en", "ACME")
           .WithManufacturerProductDesignation("en", "Unit")
           .WithSerialNumber("SN-1")
           .WithUriOfTheProduct("https://example.com/product")
           .WithManufacturerProductType("Type-123")
           .WithOrderCodeOfManufacturer("ORDER-777")
           .WithProductArticleNumberOfManufacturer("ART-999")
           .WithYearOfConstruction("2025")
           .WithDateOfManufacture(new DateTime(2025, 3, 15))
           .WithHardwareVersion("HW-1.0")
           .WithFirmwareVersion("FW-2.0")
           .WithSoftwareVersion("SW-3.0")
           .WithCountryOfOrigin("DE")
           .WithUniqueFacilityIdentifier("FAC-001");

        // Act
        sut.Build();
        var env = buildEnv();

        var submodel = env.Submodels!.Single();

        Property GetProp(string idShort) =>
            submodel.SubmodelElements!.OfType<Property>().Single(p => p.IdShort == idShort);

        // UriOfTheProduct
        var uriProp = GetProp("UriOfTheProduct");
        uriProp.Value.ShouldBe("https://example.com/product");
        uriProp.SemanticId!.Keys.Single().Value.ShouldBe(UriOfTheProduct);

        GetProp("ManufacturerProductType").Value.ShouldBe("Type-123");
        GetProp("ManufacturerProductType").SemanticId!.Keys.Single().Value.ShouldBe(ManufacturerProductType);

        GetProp("OrderCodeOfManufacturer").Value.ShouldBe("ORDER-777");
        GetProp("OrderCodeOfManufacturer").SemanticId!.Keys.Single().Value.ShouldBe(OrderCodeOfManufacturer);

        GetProp("ProductArticleNumberOfManufacturer").Value.ShouldBe("ART-999");
        GetProp("ProductArticleNumberOfManufacturer").SemanticId!.Keys.Single().Value.ShouldBe(ProductArticleNumberOfManufacturer);

        GetProp("YearOfConstruction").Value.ShouldBe("2025");
        GetProp("YearOfConstruction").SemanticId!.Keys.Single().Value.ShouldBe(YearOfConstruction);

        GetProp("DateOfManufacture").Value.ShouldBe("2025-03-15");
        GetProp("DateOfManufacture").SemanticId!.Keys.Single().Value.ShouldBe(DateOfManufacture);

        GetProp("HardwareVersion").Value.ShouldBe("HW-1.0");
        GetProp("HardwareVersion").SemanticId!.Keys.Single().Value.ShouldBe(HardwareVersion);

        GetProp("FirmwareVersion").Value.ShouldBe("FW-2.0");
        GetProp("FirmwareVersion").SemanticId!.Keys.Single().Value.ShouldBe(FirmwareVersion);

        GetProp("SoftwareVersion").Value.ShouldBe("SW-3.0");
        GetProp("SoftwareVersion").SemanticId!.Keys.Single().Value.ShouldBe(SoftwareVersion);

        GetProp("CountryOfOrigin").Value.ShouldBe("DE");
        GetProp("CountryOfOrigin").SemanticId!.Keys.Single().Value.ShouldBe(CountryOfOrigin);

        GetProp("UniqueFacilityIdentifier").Value.ShouldBe("FAC-001");
        GetProp("UniqueFacilityIdentifier").SemanticId!.Keys.Single().Value.ShouldBe(UniqueFacilityIdentifier);
    }

    [Fact]
    public void Build_ShouldAddOptionalMultiLanguageFields_WhenProvided()
    {
        // Arrange
        var (_, sut, buildEnv) = CreateBuilder();

        sut.WithManufacturerName("en", "ACME")
           .WithManufacturerProductDesignation("en", "Unit")
           .WithSerialNumber("SN-1")
           .WithManufacturerProductRoot("en", "Root-X")
           .WithManufacturerProductFamily("en", "Family-Y");

        // Act
        sut.Build();
        var env = buildEnv();
        var submodel = env.Submodels!.Single();

        MultiLanguageProperty GetMl(string idShort) =>
            submodel.SubmodelElements!.OfType<MultiLanguageProperty>().Single(p => p.IdShort == idShort);

        var root = GetMl("ManufacturerProductRoot");
        root.Value!.ShouldContain(v => v.Language == "en" && v.Text == "Root-X");
        root.SemanticId!.Keys.Single().Value.ShouldBe(ManufacturerProductRoot);

        var fam = GetMl("ManufacturerProductFamily");
        fam.Value!.ShouldContain(v => v.Language == "en" && v.Text == "Family-Y");
        fam.SemanticId!.Keys.Single().Value.ShouldBe(ManufacturerProductFamily);
    }

    [Fact]
    public void Build_ShouldAttachComplexElements_WithDefaultIdShortAndSemanticId()
    {
        // Arrange
        var (_, sut, buildEnv) = CreateBuilder();

        var address = new SubmodelElementCollection();

        var logo = new AasCore.Aas3_0.File(contentType: "image/png")
        {
            IdShort = " ",
            Value   = "file:///logo.png"
        };

        var markings = new SubmodelElementList(AasSubmodelElements.SubmodelElement)
        {
            IdShort = " "
        };

        var assetProps = new SubmodelElementCollection();

        sut.WithManufacturerName("en", "ACME")
           .WithManufacturerProductDesignation("en", "Unit")
           .WithSerialNumber("SN-1")
           .WithAddressInformation(address)
           .WithCompanyLogo(logo)
           .WithMarkings(markings)
           .WithAssetSpecificProperties(assetProps);

        // Act
        sut.Build();
        var env = buildEnv();
        var submodel = env.Submodels!.Single();

        var elements = submodel.SubmodelElements!;

        var addr = elements.OfType<SubmodelElementCollection>()
                           .Single(e => e.IdShort == "AddressInformation");
        addr.SemanticId!.Keys.Single().Value.ShouldBe(AddressInformation);

        var fileLogo = elements.OfType<AasCore.Aas3_0.File>()
                               .Single(e => e.IdShort == "CompanyLogo");
        fileLogo.SemanticId!.Keys.Single().Value.ShouldBe(CompanyLogo);

        var markList = elements.OfType<SubmodelElementList>()
                               .Single(e => e.IdShort == "Markings");
        markList.SemanticId!.Keys.Single().Value.ShouldBe(Markings);

        var assetSpec = elements.OfType<SubmodelElementCollection>()
                                .Single(e => e.IdShort == "AssetSpecificProperties");
        assetSpec.SemanticId!.Keys.Single().Value.ShouldBe(AssetSpecificProperties);
    }

    [Fact]
    public void ComplexElementSetters_ShouldThrow_WhenArgumentIsNull()
    {
        var (_, sut, _) = CreateBuilder();

        Record.Exception(() => sut.WithAddressInformation(null!))
              .ShouldBeOfType<ArgumentNullException>();

        Record.Exception(() => sut.WithCompanyLogo(null!))
              .ShouldBeOfType<ArgumentNullException>();

        Record.Exception(() => sut.WithMarkings(null!))
              .ShouldBeOfType<ArgumentNullException>();

        Record.Exception(() => sut.WithAssetSpecificProperties(null!))
              .ShouldBeOfType<ArgumentNullException>();
    }
}
