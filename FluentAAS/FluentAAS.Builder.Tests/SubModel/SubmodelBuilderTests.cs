using AasCore.Aas3_0;
using AutoFixture;
using FluentAAS.Builder.SubModel;
using Shouldly;

namespace FluentAAS.Builder.Tests.SubModel;

public class SubmodelBuilderTests
{
    private readonly Fixture _fixture = new();

    private (ShellBuilder shellBuilder, SubmodelBuilderWithShell submodelBuilder) CreateBuilder(
        string? id      = null,
        string? idShort = null)
    {
        // Create a real Environment + ShellBuilder
        var aasBuilder = new AasBuilder();

        var shellId      = _fixture.Create<string>();
        var shellIdShort = _fixture.Create<string>();
        var shellBuilder = aasBuilder.AddShell(shellId!, shellIdShort!);

        id      ??= _fixture.Create<string>();
        idShort ??= _fixture.Create<string>();
        var submodelBuilder = shellBuilder.AddSubmodel(id!, idShort!);

        return (shellBuilder, submodelBuilder);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithIdAndIdShort()
    {
        // Arrange
        var id      = _fixture.Create<string>();
        var idShort = _fixture.Create<string>();

        var aasBuilder   = new AasBuilder();
        var shellBuilder = aasBuilder.AddShell(_fixture.Create<string>()!, _fixture.Create<string>()!);

        // Act
        var sut      = new SubmodelBuilder(shellBuilder, id!, idShort!);
        var submodel = sut.BuildSubmodel();

        // Assert
        submodel.ShouldNotBeNull();
        submodel.Id.ShouldBe(id);
        submodel.IdShort.ShouldBe(idShort);
    }

    [Fact]
    public void WithSemanticId_ShouldSetSemanticId_AndReturnSameInstance()
    {
        // Arrange
        var (_, sut) = CreateBuilder();

        var semanticId = new Reference(
                                       ReferenceTypes.ExternalReference,
                                       [new Key(KeyTypes.GlobalReference, "https://example.com/semanticId")]);

        // Act
        var returned = sut.WithSemanticId(semanticId);
        var submodel = sut.BuildSubmodel();

        // Assert
        returned.ShouldBeSameAs(sut);
        submodel.SemanticId.ShouldBeSameAs(semanticId);
    }

    [Fact]
    public void AddProperty_ShouldAddPropertyElement_WithCorrectValues_AndReturnSameInstance()
    {
        // Arrange
        var (_, sut) = CreateBuilder();
        var                  idShort   = _fixture.Create<string>();
        var                  value     = _fixture.Create<string>();
        const DataTypeDefXsd valueType = DataTypeDefXsd.String;

        // Act
        var returned = sut.AddProperty(idShort!, value!);
        var submodel = sut.BuildSubmodel();

        // Assert
        returned.ShouldBeSameAs(sut);

        var property = submodel.SubmodelElements!
                               .OfType<Property>()
                               .SingleOrDefault(e => e.IdShort == idShort);

        property.ShouldNotBeNull();
        property.IdShort.ShouldBe(idShort);
        property.Value.ShouldBe(value);
        property.ValueType.ShouldBe(valueType);
    }

    [Fact]
    public void AddProperty_ShouldRespectCustomValueType()
    {
        // Arrange
        var (_, sut) = CreateBuilder();
        var                  idShort   = _fixture.Create<string>();
        var                  value     = _fixture.Create<string>();
        const DataTypeDefXsd valueType = DataTypeDefXsd.Integer;

        // Act
        sut.AddProperty(idShort!, value!, valueType);
        var submodel = sut.BuildSubmodel();

        // Assert
        var property = submodel.SubmodelElements!
                               .OfType<Property>()
                               .Single(e => e.IdShort == idShort);

        property.ValueType.ShouldBe(valueType);
    }

    [Fact]
    public void AddMultiLanguageProperty_ShouldInvokeConfigure_AndAddElement_AndReturnSameInstance()
    {
        // Arrange
        var (_, sut) = CreateBuilder();
        var                   idShort         = _fixture.Create<string>();
        LangStringSetBuilder? capturedBuilder = null;

        // Act
        var returned = sut.AddMultiLanguageProperty(idShort!, Configure);
        var submodel = sut.BuildSubmodel();

        // Assert
        returned.ShouldBeSameAs(sut);
        capturedBuilder.ShouldNotBeNull();

        var mlProp = submodel.SubmodelElements!
                             .OfType<MultiLanguageProperty>()
                             .SingleOrDefault(e => e.IdShort == idShort);

        mlProp.ShouldNotBeNull();
        mlProp.IdShort.ShouldBe(idShort);
        mlProp.Value!.ShouldContain(v => v.Language == "en" && v.Text == "Text");
        return;

        void Configure(LangStringSetBuilder builder)
        {
            capturedBuilder = builder;
            builder.Add("en", "Text");
        }
    }

    [Fact]
    public void AddElement_ShouldAddProvidedElement_AndReturnSameInstance()
    {
        // Arrange
        var (_, sut) = CreateBuilder();
        var idShort = _fixture.Create<string>();

        // Use a real Property as a generic ISubmodelElement
        var element = new Property(DataTypeDefXsd.String)
                      {
                          IdShort = idShort,
                          Value   = "someValue"
                      };

        // Act
        var returned = sut.AddElement(element);
        var submodel = sut.BuildSubmodel();

        // Assert
        returned.ShouldBeSameAs(sut);
        submodel.SubmodelElements!.ShouldContain(element);
    }

    [Fact]
    public void Done_ShouldReturnParentShellBuilder()
    {
        // Arrange
        var (shellBuilder, sut) = CreateBuilder();

        // Act
        var result = sut.Done();

        // Assert
        result.ShouldBeSameAs(shellBuilder);
    }

    [Fact]
    public void BuildSubmodel_ShouldBeIdempotent_WhenCalledMultipleTimes()
    {
        // Arrange
        var (_, sut) = CreateBuilder();
        sut.AddProperty("prop1", "value1");

        // Act
        var first  = sut.BuildSubmodel();
        var second = sut.BuildSubmodel();

        // Assert
        first.ShouldNotBeNull();
        second.ShouldNotBeNull();

        second.Id.ShouldBe(first.Id);
        second.IdShort.ShouldBe(first.IdShort);
        second.SubmodelElements!.Count.ShouldBe(first.SubmodelElements!.Count);
    }

    [Fact]
    public void FluentConfiguration_ShouldSupportChaining()
    {
        // Arrange
        var (_, sut) = CreateBuilder();

        var semanticId = new Reference(
                                       ReferenceTypes.ExternalReference,
                                       [new Key(KeyTypes.GlobalReference, "https://example.com/semId")]);

        // Act
        var returned = sut
                       .WithSemanticId(semanticId)
                       .AddProperty("p1", "v1")
                       .AddProperty("p2", "v2", DataTypeDefXsd.Integer)
                       .AddMultiLanguageProperty("ml1", b => b.Add("en", "X"))
                       .AddElement(new Property(DataTypeDefXsd.String) {IdShort = "extra", Value = "val"});

        var submodel = sut.BuildSubmodel();

        // Assert
        returned.ShouldBeSameAs(sut);
        submodel.SemanticId.ShouldBe(semanticId);
        submodel.SubmodelElements!.Count.ShouldBe(4);
    }

    [Fact]
    public void AddMultiLanguageProperty_ShouldThrow_WhenConfigureIsNull()
    {
        // Arrange
        var (_, sut) = CreateBuilder();
        var idShort = _fixture.Create<string>();

        // Act
        var exception = Record.Exception(() => sut.AddMultiLanguageProperty(idShort!, null!));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<ArgumentNullException>();
    }
}