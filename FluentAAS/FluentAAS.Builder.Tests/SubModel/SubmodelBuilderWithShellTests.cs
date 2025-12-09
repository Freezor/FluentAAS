using AasCore.Aas3_0;
using AutoFixture;
using FluentAAS.Builder.SubModel;
using Shouldly;

namespace FluentAAS.Builder.Tests.SubModel;

public class SubmodelBuilderWithShellTests
{
    private readonly Fixture _fixture = new();

    /// <summary>
    /// Creates a REAL ShellBuilder similar to your *working* SubmodelBuilderTests.
    /// NO mocking, NO EnvironmentBuilder.
    /// </summary>
    private (IShellBuilder shellBuilder, SubmodelBuilderWithShell sut) CreateBuilder(
        string? id      = null,
        string? idShort = null)
    {
        // Create a shell builder directly
        var shellId      = "urn:aas:" + Guid.NewGuid().ToString("N");
        var shellIdShort = "Shell-" + Guid.NewGuid().ToString("N")[..6];

        var shellBuilder = AasBuilder
                           .Create()
                           .AddShell(shellId, shellIdShort);

        id      ??= _fixture.Create<string>();
        idShort ??= _fixture.Create<string>();

        var sut = new SubmodelBuilderWithShell(shellBuilder, id!, idShort!);
        return (shellBuilder, sut);
    }

    [Fact]
    public void Constructor_ShouldInitializeSubmodel_WithIdAndIdShort()
    {
        // Arrange
        var id      = _fixture.Create<string>();
        var idShort = _fixture.Create<string>();

        var shellBuilder = AasBuilder.Create().AddShell("urn:test:shell", "Shell");

        // Act
        var sut      = new SubmodelBuilderWithShell(shellBuilder, id!, idShort!);
        var submodel = sut.BuildSubmodel();

        // Assert
        submodel.ShouldNotBeNull();
        submodel.Id.ShouldBe(id);
        submodel.IdShort.ShouldBe(idShort);
        submodel.SubmodelElements.ShouldNotBeNull();
        submodel.SubmodelElements.ShouldBeEmpty();
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenParentShellIsNull()
    {
        // Act
        var ex = Record.Exception(() =>
                                      new SubmodelBuilderWithShell(null!, "id", "idShort"));

        // Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenIdIsInvalid()
    {
        var shell = AasBuilder.Create().AddShell("s", "s");

        var ex = Record.Exception(() =>
                                      new SubmodelBuilderWithShell(shell, " ", "idShort"));

        ex.ShouldBeOfType<ArgumentException>();
        ex.Message.ShouldContain("Submodel id must not be empty");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenIdShortIsInvalid()
    {
        var shell = AasBuilder.Create().AddShell("s", "s");

        var ex = Record.Exception(() =>
                                      new SubmodelBuilderWithShell(shell, "id", " "));

        ex.ShouldBeOfType<ArgumentException>();
        ex.Message.ShouldContain("Submodel idShort must not be empty");
    }

    [Fact]
    public void WithSemanticId_ShouldAssignSemanticId_AndReturnBuilder()
    {
        // Arrange
        var (_, sut) = CreateBuilder();
        var sem = new Reference(
                                ReferenceTypes.ExternalReference,
                                [new Key(KeyTypes.GlobalReference, "namespace")]
                               );

        // Act
        var returned = sut.WithSemanticId(sem);
        var submodel = sut.BuildSubmodel();

        // Assert
        returned.ShouldBeSameAs(sut);
        submodel.SemanticId.ShouldBe(sem);
    }

    [Fact]
    public void WithSemanticId_ShouldThrow_WhenNull()
    {
        var (_, sut) = CreateBuilder();

        var ex = Record.Exception(() => sut.WithSemanticId(null!));

        ex.ShouldBeOfType<ArgumentNullException>();
    }

    [Fact]
    public void AddProperty_ShouldAddProperty_AndReturnBuilder()
    {
        // Arrange
        var (_, sut) = CreateBuilder();
        var          idShort = "prop-" + Guid.NewGuid().ToString("N")[..6];
        const string value   = "test-value";

        // Act
        var returned = sut.AddProperty(idShort, value);
        var submodel = sut.BuildSubmodel();

        // Assert
        returned.ShouldBeSameAs(sut);

        var prop = submodel.SubmodelElements!
                           .OfType<Property>()
                           .SingleOrDefault(p => p.IdShort == idShort);

        prop.ShouldNotBeNull();
        prop.Value.ShouldBe(value);
        prop.ValueType.ShouldBe(DataTypeDefXsd.String);
    }

    [Fact]
    public void AddProperty_ShouldRespectCustomValueType()
    {
        var (_, sut) = CreateBuilder();

        sut.AddProperty("intProp", "123", DataTypeDefXsd.Integer);
        var submodel = sut.BuildSubmodel();

        var prop = submodel.SubmodelElements!.OfType<Property>().Single();
        prop.ValueType.ShouldBe(DataTypeDefXsd.Integer);
    }

    [Fact]
    public void AddProperty_ShouldThrow_WhenIdShortInvalid()
    {
        var (_, sut) = CreateBuilder();

        var ex = Record.Exception(() => sut.AddProperty(" ", "value"));

        ex.ShouldBeOfType<ArgumentException>();
        ex.Message.ShouldContain("idShort");
    }

    [Fact]
    public void AddProperty_ShouldThrow_WhenValueInvalid()
    {
        var (_, sut) = CreateBuilder();

        var ex = Record.Exception(() => sut.AddProperty("prop", " "));

        ex.ShouldBeOfType<ArgumentException>();
        ex.Message.ShouldContain("value");
    }

    [Fact]
    public void AddMultiLanguageProperty_ShouldAddElement_AndReturnBuilder()
    {
        var (_, sut) = CreateBuilder();
        var idShort = "ml-" + Guid.NewGuid().ToString("N")[..6];

        LangStringSetBuilder? captured = null;

        sut.AddMultiLanguageProperty(
                                     idShort, b =>
                                              {
                                                  captured = b;
                                                  b.Add("en", "Hello");
                                              });

        var submodel = sut.BuildSubmodel();

        captured.ShouldNotBeNull();

        var ml = submodel.SubmodelElements!
                         .OfType<MultiLanguageProperty>()
                         .SingleOrDefault(x => x.IdShort == idShort);

        ml.ShouldNotBeNull();
        ml.Value!.ShouldContain(v => v.Language == "en" && v.Text == "Hello");
    }

    [Fact]
    public void AddMultiLanguageProperty_ShouldThrow_WhenIdShortInvalid()
    {
        var (_, sut) = CreateBuilder();

        var ex = Record.Exception(() =>
                                      sut.AddMultiLanguageProperty(" ", _ => { }));

        ex.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    public void AddMultiLanguageProperty_ShouldThrow_WhenConfigureNull()
    {
        var (_, sut) = CreateBuilder();

        var ex = Record.Exception(() =>
                                      sut.AddMultiLanguageProperty("ml", null!));

        ex.ShouldBeOfType<ArgumentNullException>();
    }

    [Fact]
    public void AddElement_ShouldAddElement_AndReturnBuilder()
    {
        // Arrange
        var (_, sut) = CreateBuilder();

        var element = new Property(DataTypeDefXsd.String)
                      {
                          IdShort = "extra",
                          Value   = "V"
                      };

        // Act
        var returned = sut.AddElement(element);
        var submodel = sut.BuildSubmodel();

        // Assert
        returned.ShouldBeSameAs(sut);
        submodel.SubmodelElements!.ShouldContain(element);
    }

    [Fact]
    public void AddElement_ShouldThrow_WhenNull()
    {
        var (_, sut) = CreateBuilder();

        var ex = Record.Exception(() => sut.AddElement(null!));

        ex.ShouldBeOfType<ArgumentNullException>();
    }

    [Fact]
    public void AddElement_ConvenienceOverload_ShouldCreateProperty()
    {
        var (_, sut) = CreateBuilder();

        sut.AddElement("pX", "123");
        var submodel = sut.BuildSubmodel();

        var prop = submodel.SubmodelElements!.OfType<Property>().Single();

        prop.IdShort.ShouldBe("pX");
        prop.Value.ShouldBe("123");
    }

    [Fact]
    public void Done_ShouldAttachSubmodelToShell_AndReturnShellBuilder()
    {
        // Arrange
        var (shell, sut) = CreateBuilder();

        // Act
        var returned = sut.Done();

        // Assert
        returned.ShouldBeSameAs(shell);
        shell.Shell.Submodels.ShouldNotBeNull();
        shell.Shell.Submodels.Count.ShouldBe(1);
    }

    [Fact]
    public void FluentConfiguration_ShouldAllowFullChaining()
    {
        var (_, sut) = CreateBuilder();

        var sem = new Reference(
                                ReferenceTypes.ExternalReference,
                                [new Key(KeyTypes.FragmentReference, "abc")]);

        sut.WithSemanticId(sem)
           .AddProperty("p1", "v1")
           .AddMultiLanguageProperty("ml", b => b.Add("en", "Hello"))
           .AddElement("x", "y");

        var submodel = sut.BuildSubmodel();

        submodel.SemanticId.ShouldBe(sem);
        submodel.SubmodelElements!.Count.ShouldBe(3);
    }
}