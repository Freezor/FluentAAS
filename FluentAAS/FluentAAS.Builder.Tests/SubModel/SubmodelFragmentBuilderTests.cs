using AasCore.Aas3_0;
using AutoFixture;
using FluentAAS.Builder.SubModel;
using Shouldly;

namespace FluentAAS.Builder.Tests.SubModel;

/// <summary>
/// Tests for <see cref="SubmodelFragmentBuilder"/>.
/// Because the constructor is internal, the builder is accessed through
/// <see cref="AasBuilder.AddSubmodelFragment"/>.
/// Exceptions thrown inside the configure callback propagate through
/// <see cref="AasBuilder.Build"/>, so tests use
/// <c>Should.Throw&lt;T&gt;(() => builder.Build())</c> where appropriate.
/// </summary>
public class SubmodelFragmentBuilderTests
{
    private readonly Fixture _fixture = new();

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates an <see cref="AasBuilder"/> pre-loaded with a single submodel and
    /// returns both the builder and the submodel id so tests can register fragments.
    /// </summary>
    private (AasBuilder builder, string submodelId) CreateBuilderWithSubmodel(string idShort = "test-submodel")
    {
        var builder    = new AasBuilder();
        var submodelId = _fixture.Create<string>();
        builder.AddSubmodel(new Submodel(id: submodelId, idShort: idShort));
        return (builder, submodelId);
    }

    /// <summary>
    /// Builds the environment and returns the single submodel from it.
    /// </summary>
    private static Submodel BuildAndGetSubmodel(AasBuilder builder)
    {
        var env = builder.Build();
        return env.Submodels.ShouldHaveSingleItem().ShouldBeOfType<Submodel>();
    }

    // -------------------------------------------------------------------------
    // AddProperty – validation
    // -------------------------------------------------------------------------

    [Fact]
    public void AddProperty_WithNullIdShort_ShouldThrowArgumentException()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        builder.AddSubmodelFragment(submodelId, fragment => fragment.AddProperty(null!, "value"));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => builder.Build());
        ex.ParamName.ShouldBe("idShort");
    }

    [Fact]
    public void AddProperty_WithEmptyIdShort_ShouldThrowArgumentException()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        builder.AddSubmodelFragment(submodelId, fragment => fragment.AddProperty("", "value"));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => builder.Build());
        ex.ParamName.ShouldBe("idShort");
    }

    [Fact]
    public void AddProperty_WithWhitespaceIdShort_ShouldThrowArgumentException()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        builder.AddSubmodelFragment(submodelId, fragment => fragment.AddProperty("   ", "value"));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => builder.Build());
        ex.ParamName.ShouldBe("idShort");
    }

    [Fact]
    public void AddProperty_WithNullValue_ShouldThrowArgumentException()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        builder.AddSubmodelFragment(submodelId, fragment => fragment.AddProperty("propId", null!));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => builder.Build());
        ex.ParamName.ShouldBe("value");
    }

    [Fact]
    public void AddProperty_WithEmptyValue_ShouldThrowArgumentException()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        builder.AddSubmodelFragment(submodelId, fragment => fragment.AddProperty("propId", ""));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => builder.Build());
        ex.ParamName.ShouldBe("value");
    }

    [Fact]
    public void AddProperty_WithWhitespaceValue_ShouldThrowArgumentException()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        builder.AddSubmodelFragment(submodelId, fragment => fragment.AddProperty("propId", "   "));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => builder.Build());
        ex.ParamName.ShouldBe("value");
    }

    // -------------------------------------------------------------------------
    // AddProperty – success
    // -------------------------------------------------------------------------

    [Fact]
    public void AddProperty_WithValidArguments_ShouldAddPropertyWithCorrectIdShortAndValue()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        const string expectedIdShort = "Pressure";
        const string expectedValue   = "101.3";

        // Act
        builder.AddSubmodelFragment(submodelId, fragment => fragment.AddProperty(expectedIdShort, expectedValue));
        var submodel = BuildAndGetSubmodel(builder);

        // Assert
        submodel.SubmodelElements.ShouldNotBeNull();
        var property = submodel.SubmodelElements.OfType<Property>().ShouldHaveSingleItem();

        property.IdShort.ShouldBe(expectedIdShort);
        property.Value.ShouldBe(expectedValue);
    }

    [Fact]
    public void AddProperty_WithDefaultValueType_ShouldDefaultToXsdString()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();

        // Act
        builder.AddSubmodelFragment(submodelId, f => f.AddProperty("myProp", "hello"));
        var submodel = BuildAndGetSubmodel(builder);

        // Assert
        var property = submodel.SubmodelElements!.OfType<Property>().ShouldHaveSingleItem();
        property.ValueType.ShouldBe(DataTypeDefXsd.String);
    }

    [Fact]
    public void AddProperty_WithExplicitValueType_ShouldUseSpecifiedType()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();

        // Act
        builder.AddSubmodelFragment(submodelId,
            f => f.AddProperty("temp", "42.5", DataTypeDefXsd.Float));
        var submodel = BuildAndGetSubmodel(builder);

        // Assert
        var property = submodel.SubmodelElements!.OfType<Property>().ShouldHaveSingleItem();
        property.ValueType.ShouldBe(DataTypeDefXsd.Float);
    }

    [Fact]
    public void AddProperty_ShouldReturnSameInstanceForFluentChaining()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();

        // Act – verify inside the callback that AddProperty returns this
        builder.AddSubmodelFragment(submodelId, fragment =>
        {
            var returned = fragment.AddProperty("p", "v");
            returned.ShouldBeSameAs(fragment);
        });

        builder.Build();
    }

    [Fact]
    public void AddProperty_CalledMultipleTimes_ShouldAddAllPropertiesInOrder()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();

        // Act
        builder.AddSubmodelFragment(submodelId, f =>
        {
            f.AddProperty("alpha", "1")
             .AddProperty("beta", "2")
             .AddProperty("gamma", "3");
        });

        var submodel = BuildAndGetSubmodel(builder);

        // Assert
        submodel.SubmodelElements.ShouldNotBeNull();
        submodel.SubmodelElements.Count.ShouldBe(3);

        var idShorts = submodel.SubmodelElements.Select(e => e.IdShort).ToList();
        idShorts[0].ShouldBe("alpha");
        idShorts[1].ShouldBe("beta");
        idShorts[2].ShouldBe("gamma");
    }

    // -------------------------------------------------------------------------
    // AddMultiLanguageProperty – validation
    // -------------------------------------------------------------------------

    [Fact]
    public void AddMultiLanguageProperty_WithNullIdShort_ShouldThrowArgumentException()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        builder.AddSubmodelFragment(submodelId,
            fragment => fragment.AddMultiLanguageProperty(null!, ls => ls.Add("en", "text")));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => builder.Build());
        ex.ParamName.ShouldBe("idShort");
    }

    [Fact]
    public void AddMultiLanguageProperty_WithWhitespaceIdShort_ShouldThrowArgumentException()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        builder.AddSubmodelFragment(submodelId,
            fragment => fragment.AddMultiLanguageProperty("   ", ls => ls.Add("en", "text")));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => builder.Build());
        ex.ParamName.ShouldBe("idShort");
    }

    [Fact]
    public void AddMultiLanguageProperty_WithNullConfigure_ShouldThrowArgumentNullException()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        builder.AddSubmodelFragment(submodelId,
            fragment => fragment.AddMultiLanguageProperty("mlProp", null!));

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => builder.Build());
    }

    // -------------------------------------------------------------------------
    // AddMultiLanguageProperty – success
    // -------------------------------------------------------------------------

    [Fact]
    public void AddMultiLanguageProperty_WithValidArguments_ShouldAddMultiLanguagePropertyToSubmodel()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        const string expectedIdShort = "Description";

        // Act
        builder.AddSubmodelFragment(submodelId, f =>
            f.AddMultiLanguageProperty(expectedIdShort, ls => ls
                .Add("en", "Running")
                .Add("de", "Läuft")));

        var submodel = BuildAndGetSubmodel(builder);

        // Assert
        submodel.SubmodelElements.ShouldNotBeNull();
        var mlProp = submodel.SubmodelElements
                             .OfType<MultiLanguageProperty>()
                             .ShouldHaveSingleItem();

        mlProp.IdShort.ShouldBe(expectedIdShort);
        mlProp.Value.ShouldNotBeNull();
        mlProp.Value!.Count.ShouldBe(2);
        mlProp.Value.ShouldContain(ls => ls.Language == "en" && ls.Text == "Running");
        mlProp.Value.ShouldContain(ls => ls.Language == "de" && ls.Text == "Läuft");
    }

    [Fact]
    public void AddMultiLanguageProperty_ShouldReturnSameInstanceForFluentChaining()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();

        // Act – verify inside the callback that AddMultiLanguageProperty returns this
        builder.AddSubmodelFragment(submodelId, fragment =>
        {
            var returned = fragment.AddMultiLanguageProperty("label", ls => ls.Add("en", "ok"));
            returned.ShouldBeSameAs(fragment);
        });

        builder.Build();
    }

    [Fact]
    public void AddMultiLanguageProperty_WithSingleLanguage_ShouldContainOneLangString()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();

        // Act
        builder.AddSubmodelFragment(submodelId,
            f => f.AddMultiLanguageProperty("Status", ls => ls.Add("en", "Active")));

        var submodel = BuildAndGetSubmodel(builder);

        // Assert
        var mlProp = submodel.SubmodelElements!.OfType<MultiLanguageProperty>().ShouldHaveSingleItem();
        mlProp.Value.ShouldHaveSingleItem();
        mlProp.Value!.Single().Language.ShouldBe("en");
        mlProp.Value!.Single().Text.ShouldBe("Active");
    }

    // -------------------------------------------------------------------------
    // AddElement – validation
    // -------------------------------------------------------------------------

    [Fact]
    public void AddElement_WithNullElement_ShouldThrowArgumentNullException()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        builder.AddSubmodelFragment(submodelId, fragment => fragment.AddElement(null!));

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => builder.Build());
    }

    // -------------------------------------------------------------------------
    // AddElement – success
    // -------------------------------------------------------------------------

    [Fact]
    public void AddElement_WithValidElement_ShouldAddElementToSubmodel()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        var element = new Property(DataTypeDefXsd.Boolean)
        {
            IdShort = "IsActive",
            Value   = "true"
        };

        // Act
        builder.AddSubmodelFragment(submodelId, f => f.AddElement(element));
        var submodel = BuildAndGetSubmodel(builder);

        // Assert
        submodel.SubmodelElements.ShouldNotBeNull();
        submodel.SubmodelElements.ShouldContain(e => e.IdShort == "IsActive");
    }

    [Fact]
    public void AddElement_ShouldReturnSameInstanceForFluentChaining()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        var element = new Property(DataTypeDefXsd.String) { IdShort = "X" };

        // Act – verify inside the callback that AddElement returns this
        builder.AddSubmodelFragment(submodelId, fragment =>
        {
            var returned = fragment.AddElement(element);
            returned.ShouldBeSameAs(fragment);
        });

        builder.Build();
    }

    [Fact]
    public void AddElement_CalledMultipleTimes_ShouldAddAllElements()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        var e1 = new Property(DataTypeDefXsd.String) { IdShort = "Elem1" };
        var e2 = new Property(DataTypeDefXsd.Int)    { IdShort = "Elem2" };

        // Act
        builder.AddSubmodelFragment(submodelId, f => f.AddElement(e1).AddElement(e2));
        var submodel = BuildAndGetSubmodel(builder);

        // Assert
        submodel.SubmodelElements.ShouldNotBeNull();
        submodel.SubmodelElements.Count.ShouldBe(2);
        submodel.SubmodelElements.ShouldContain(e1);
        submodel.SubmodelElements.ShouldContain(e2);
    }

    // -------------------------------------------------------------------------
    // Mixed / Combined
    // -------------------------------------------------------------------------

    [Fact]
    public void MixedFragmentMethods_ShouldAddAllElementTypesInRegistrationOrder()
    {
        // Arrange
        var (builder, submodelId) = CreateBuilderWithSubmodel();
        var rawElement = new Property(DataTypeDefXsd.Boolean) { IdShort = "Flag" };

        // Act
        builder.AddSubmodelFragment(submodelId, f =>
        {
            f.AddProperty("Voltage", "230")
             .AddMultiLanguageProperty("Label", ls => ls.Add("en", "Main voltage"))
             .AddElement(rawElement);
        });

        var submodel = BuildAndGetSubmodel(builder);

        // Assert
        submodel.SubmodelElements.ShouldNotBeNull();
        submodel.SubmodelElements.Count.ShouldBe(3);

        submodel.SubmodelElements[0].IdShort.ShouldBe("Voltage");
        submodel.SubmodelElements[1].IdShort.ShouldBe("Label");
        submodel.SubmodelElements[2].IdShort.ShouldBe("Flag");
    }

    [Fact]
    public void SubmodelStartingWithNullElements_ShouldBeInitializedByFragment()
    {
        // Arrange – create a submodel whose SubmodelElements is explicitly null
        var builder    = new AasBuilder();
        var submodelId = _fixture.Create<string>();
        var submodel   = new Submodel(id: submodelId, idShort: "null-elements") { SubmodelElements = null };
        builder.AddSubmodel(submodel);

        // Act
        builder.AddSubmodelFragment(submodelId, f => f.AddProperty("temp", "21"));
        var env = builder.Build();

        // Assert
        var built = env.Submodels.ShouldHaveSingleItem().ShouldBeOfType<Submodel>();
        built.SubmodelElements.ShouldNotBeNull();
        built.SubmodelElements.ShouldContain(e => e.IdShort == "temp");
    }

    [Fact]
    public void AddProperty_IntegerValueType_ShouldBeReflectedInBuiltProperty()
    {
        // Arrange – boundary case: non-string XSD type
        var (builder, submodelId) = CreateBuilderWithSubmodel();

        // Act
        builder.AddSubmodelFragment(submodelId,
            f => f.AddProperty("count", "99", DataTypeDefXsd.Integer));
        var submodel = BuildAndGetSubmodel(builder);

        // Assert
        var prop = submodel.SubmodelElements!.OfType<Property>().ShouldHaveSingleItem();
        prop.ValueType.ShouldBe(DataTypeDefXsd.Integer);
        prop.Value.ShouldBe("99");
    }
}