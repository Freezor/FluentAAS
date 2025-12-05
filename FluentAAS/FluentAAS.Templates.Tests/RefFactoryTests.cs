using AasCore.Aas3_0;
using FluentAAS.Templates;
using Shouldly;

namespace FluentAas.Templates.Tests;

/// <summary>
/// Unit tests for <see cref="RefFactory"/>.
/// </summary>
public class RefFactoryTests
{
    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("   ")]
    public void GlobalConceptDescription_ShouldThrowArgumentException_WhenConceptIdIsNullOrWhitespace(string? conceptId)
    {
        // Act
        var ex = Should.Throw<ArgumentException>(() => RefFactory.GlobalConceptDescription(conceptId!));

        // Assert
        ex.ParamName.ShouldBe("conceptId");
        ex.Message.ShouldContain("Concept ID must not be empty.");
    }

    [Fact]
    public void GlobalConceptDescription_ShouldCreateReference_WithExpectedStructure()
    {
        // Arrange
        const string conceptId = "0173-1#02-BAF577#003";

        // Act
        var reference = RefFactory.GlobalConceptDescription(conceptId);

        // Assert
        reference.ShouldNotBeNull();
        reference.Type.ShouldBe(ReferenceTypes.ExternalReference);

        reference.Keys.ShouldNotBeNull();
        reference.Keys.Count.ShouldBe(1);

        var key = reference.Keys[0];
        key.Type.ShouldBe(KeyTypes.GlobalReference);
        key.Value.ShouldBe(conceptId);
    }

    [Fact]
    public void GlobalConceptDescription_ShouldReturnNewInstances_ForSameConceptId()
    {
        // Arrange
        const string conceptId = "some-global-concept-id";

        // Act
        var first  = RefFactory.GlobalConceptDescription(conceptId);
        var second = RefFactory.GlobalConceptDescription(conceptId);

        // Assert
        first.ShouldNotBeSameAs(second);
        first.Keys.ShouldNotBeSameAs(second.Keys);

        // Both still structurally equivalent
        first.Type.ShouldBe(ReferenceTypes.ExternalReference);
        second.Type.ShouldBe(ReferenceTypes.ExternalReference);

        first.Keys.Count.ShouldBe(1);
        second.Keys.Count.ShouldBe(1);

        first.Keys[0].Type.ShouldBe(KeyTypes.GlobalReference);
        second.Keys[0].Type.ShouldBe(KeyTypes.GlobalReference);

        first.Keys[0].Value.ShouldBe(conceptId);
        second.Keys[0].Value.ShouldBe(conceptId);
    }
}