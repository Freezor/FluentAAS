using System.Text.Json.Nodes;
using FluentAAS.Builder;
using Shouldly;

namespace FluentAas.IO.Tests;

/// <summary>
/// Unit tests for <see cref="AasJsonSerializer"/>.
/// </summary>
public class AasJsonSerializerTests
{
    private static IEnvironment CreateSampleEnvironment()
    {
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
                                    .Done()
                                    .Done()
                                    .Build();

        return environment;
    }

    [Fact]
    public void ToJson_ShouldThrow_WhenEnvironmentIsNull()
    {
        // Arrange
        Environment? env = null;

        // Act
        var ex = Should.Throw<ArgumentNullException>(() => AasJsonSerializer.ToJson(env!));

        // Assert
        ex.ParamName.ShouldBe("environment");
    }

    [Fact]
    public void ToJson_ShouldReturnNonEmptyJson_ForValidEnvironment()
    {
        // Arrange
        var env = CreateSampleEnvironment();

        // Act
        var json = AasJsonSerializer.ToJson(env);

        // Assert
        json.ShouldNotBeNullOrWhiteSpace();

        // Basic sanity check: JSON should be parseable and contain the shell id.
        var root = JsonNode.Parse(json)!.AsObject();
        root["assetAdministrationShells"].ShouldNotBeNull();

        var shells = root["assetAdministrationShells"]!.AsArray();
        shells.Count.ShouldBe(1);
        shells[0]!["id"]!.ToString().ShouldBe("urn:aas:example:my-shell");
        shells[0]!["idShort"]!.ToString().ShouldBe("MyShell");
    }

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("  ")]
    public void FromJson_ShouldThrowArgumentException_WhenJsonIsNullOrWhitespace(string? json)
    {
        // Act
        var ex = Should.Throw<ArgumentException>(() => AasJsonSerializer.FromJson(json!));

        // Assert
        ex.ParamName.ShouldBe("json");
        ex.Message.ShouldContain("JSON must not be empty.");
    }

    [Fact]
    public void FromJson_ShouldThrowInvalidOperationException_WhenJsonIsMalformed()
    {
        // Arrange
        const string invalidJson = "{ this is not valid JSON }";

        // Act
        var ex = Should.Throw<InvalidOperationException>(() => AasJsonSerializer.FromJson(invalidJson));

        // Assert
        ex.Message.ShouldContain("Input is not valid JSON.");
    }

    [Fact]
    public void FromJson_ShouldThrowInvalidOperationException_WhenJsonIsValidButNotAasEnvironment()
    {
        // Arrange
        const string notEnvJson = """{ "foo": "bar" }""";

        // Act
        var ex = Should.Throw<InvalidOperationException>(() => AasJsonSerializer.FromJson(notEnvJson));

        // Assert
        ex.Message.ShouldContain("Failed to deserialize AAS environment from JSON.");
        ex.InnerException.ShouldNotBeNull();
        ex.InnerException.GetType().Name.ShouldBe("Exception"); // AasJsonization.Exception
    }

    [Fact]
    public void ToJsonAndFromJson_ShouldRoundTripEnvironment()
    {
        // Arrange
        var original = CreateSampleEnvironment();

        // Act
        var json      = AasJsonSerializer.ToJson(original);
        var roundTrip = AasJsonSerializer.FromJson(json);

        // Assert
        roundTrip.ShouldNotBeNull();

        roundTrip.AssetAdministrationShells.ShouldNotBeNull();
        roundTrip.AssetAdministrationShells.Count.ShouldBe(1);

        var shell = roundTrip.AssetAdministrationShells[0];
        shell.Id.ShouldBe("urn:aas:example:my-shell");
        shell.IdShort.ShouldBe("MyShell");
    }

    [Fact]
    public void FromJson_ShouldDeserializeEnvironmentProducedByToJson()
    {
        // Arrange
        var env  = CreateSampleEnvironment();
        var json = AasJsonSerializer.ToJson(env);

        // Act
        var result = AasJsonSerializer.FromJson(json);

        // Assert
        result.ShouldNotBeNull();
        result.AssetAdministrationShells!.Count.ShouldBe(1);
        result.AssetAdministrationShells[0].Id.ShouldBe(env.AssetAdministrationShells![0].Id);
        result.AssetAdministrationShells[0].IdShort.ShouldBe(env.AssetAdministrationShells[0].IdShort);
    }
}
