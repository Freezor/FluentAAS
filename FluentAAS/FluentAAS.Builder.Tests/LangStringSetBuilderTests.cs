using AutoFixture;
using Shouldly;

namespace FluentAAS.Builder.Tests;

public class LangStringSetBuilderTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_WithNullIdShort_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new LangStringSetBuilder(null!);

        // Assert
        var ex = act.ShouldThrow<ArgumentException>();
        ex.ParamName.ShouldBe("idShort");
        ex.Message.ShouldContain("idShort must not be empty.");
    }

    [Fact]
    public void Constructor_WithEmptyIdShort_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new LangStringSetBuilder(string.Empty);

        // Assert
        var ex = act.ShouldThrow<ArgumentException>();
        ex.ParamName.ShouldBe("idShort");
        ex.Message.ShouldContain("idShort must not be empty.");
    }

    [Fact]
    public void Constructor_WithWhitespaceIdShort_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new LangStringSetBuilder("   ");

        // Assert
        var ex = act.ShouldThrow<ArgumentException>();
        ex.ParamName.ShouldBe("idShort");
        ex.Message.ShouldContain("idShort must not be empty.");
    }

    [Fact]
    public void Constructor_WithValidIdShort_ShouldInitializeProperty()
    {
        // Arrange
        var idShort = _fixture.Create<string>();

        // Act
        var builder = new LangStringSetBuilder(idShort!);
        var result  = builder.Build();

        // Assert
        result.ShouldNotBeNull();
        result.IdShort.ShouldBe(idShort);
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public void Add_WithNullLanguage_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new LangStringSetBuilder("id");

        // Act
        var act = () => builder.Add(null!, "text");

        // Assert
        var ex = act.ShouldThrow<ArgumentException>();
        ex.ParamName.ShouldBe("language");
        ex.Message.ShouldContain("Language code must not be empty.");
    }

    [Fact]
    public void Add_WithEmptyLanguage_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new LangStringSetBuilder("id");

        // Act
        var act = () => builder.Add(string.Empty, "text");

        // Assert
        var ex = act.ShouldThrow<ArgumentException>();
        ex.ParamName.ShouldBe("language");
        ex.Message.ShouldContain("Language code must not be empty.");
    }

    [Fact]
    public void Add_WithWhitespaceLanguage_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new LangStringSetBuilder("id");

        // Act
        var act = () => builder.Add("   ", "text");

        // Assert
        var ex = act.ShouldThrow<ArgumentException>();
        ex.ParamName.ShouldBe("language");
        ex.Message.ShouldContain("Language code must not be empty.");
    }

    [Fact]
    public void Add_WithNullText_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new LangStringSetBuilder("id");

        // Act
        var act = () => builder.Add("en", null!);

        // Assert
        var ex = act.ShouldThrow<ArgumentException>();
        ex.ParamName.ShouldBe("text");
        ex.Message.ShouldContain("Text must not be empty.");
    }

    [Fact]
    public void Add_WithEmptyText_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new LangStringSetBuilder("id");

        // Act
        var act = () => builder.Add("en", string.Empty);

        // Assert
        var ex = act.ShouldThrow<ArgumentException>();
        ex.ParamName.ShouldBe("text");
        ex.Message.ShouldContain("Text must not be empty.");
    }

    [Fact]
    public void Add_WithWhitespaceText_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new LangStringSetBuilder("id");

        // Act
        var act = () => builder.Add("en", "   ");

        // Assert
        var ex = act.ShouldThrow<ArgumentException>();
        ex.ParamName.ShouldBe("text");
        ex.Message.ShouldContain("Text must not be empty.");
    }

    [Fact]
    public void Add_WithValidLanguageAndText_ShouldAddLangStringTextType()
    {
        // Arrange
        var          builder  = new LangStringSetBuilder("myId");
        const string language = "en";
        const string text     = "Hello";

        // Act
        var resultBuilder = builder.Add(language, text);
        var property      = resultBuilder.Build();

        // Assert (fluent return)
        resultBuilder.ShouldBeSameAs(builder);

        property.Value.ShouldNotBeNull();
        property.Value.Count.ShouldBe(1);

        var entry = property.Value[0];
        entry.Language.ShouldBe(language);
        entry.Text.ShouldBe(text);
    }

    [Fact]
    public void Add_CalledMultipleTimes_ShouldAppendValuesInOrder()
    {
        // Arrange
        var builder = new LangStringSetBuilder("myId");

        // Act
        builder
            .Add("en", "English")
            .Add("de", "Deutsch")
            .Add("fr", "Français");

        var property = builder.Build();

        // Assert
        property.Value.ShouldNotBeNull();
        property.Value.Count.ShouldBe(3);

        property.Value[0].Language.ShouldBe("en");
        property.Value[0].Text.ShouldBe("English");

        property.Value[1].Language.ShouldBe("de");
        property.Value[1].Text.ShouldBe("Deutsch");

        property.Value[2].Language.ShouldBe("fr");
        property.Value[2].Text.ShouldBe("Français");
    }

    [Fact]
    public void Build_WhenCalledMultipleTimes_ShouldReturnSameInstance()
    {
        // Arrange
        var builder = new LangStringSetBuilder("myId");
        builder.Add("en", "Hello");

        // Act
        var first  = builder.Build();
        var second = builder.Build();

        // Assert
        second.ShouldBeSameAs(first);
    }
}