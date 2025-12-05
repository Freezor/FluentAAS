using System.IO.Compression;
using System.Text.Json.Nodes;
using FluentAAS.Builder;
using FluentAAS.IO;
using Shouldly;
using File = System.IO.File;

namespace FluentAas.IO.Tests;

/// <summary>
/// Unit tests for <see cref="EnvironmentExtensions"/>.
/// </summary>
public class EnvironmentExtensionsTests
{
    private static Environment CreateSampleEnvironment()
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

    private static string CreateTempAasxPath()
    {
        var fileName = $"test-{Guid.NewGuid():N}.aasx";
        return Path.Combine(Path.GetTempPath(), fileName);
    }

    [Fact]
    public void ToAasx_ShouldThrowArgumentNullException_WhenEnvironmentIsNull()
    {
        // Arrange
        Environment? env        = null;
        var          outputPath = CreateTempAasxPath();
        const string specPath   = "aasx/spec.json";

        // Act
        var ex = Should.Throw<ArgumentNullException>(() => env!.ToAasx(outputPath, specPath));

        // Assert
        ex.ParamName.ShouldBe("environment");
    }

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToAasx_ShouldThrowArgumentException_WhenOutputPathIsNullOrWhitespace(string? outputPath)
    {
        // Arrange
        var          env      = CreateSampleEnvironment();
        const string specPath = "aasx/spec.json";

        // Act
        var ex = Should.Throw<ArgumentException>(() => env.ToAasx(outputPath!, specPath));

        // Assert
        ex.ParamName.ShouldBe("outputPath");
        ex.Message.ShouldContain("Output path must not be null or empty.");
    }

    [Fact]
    public void ToAasx_ShouldCreateNonEmptyAasxFile_ForValidEnvironment()
    {
        // Arrange
        var          env        = CreateSampleEnvironment();
        var          outputPath = CreateTempAasxPath();
        const string specPath   = "/aasx/spec.json";

        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        env.ToAasx(outputPath, specPath);

        // Assert
        File.Exists(outputPath).ShouldBeTrue();

        var fileInfo = new FileInfo(outputPath);
        fileInfo.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void ToAasx_ShouldWriteJsonSpecPart_ThatContainsAasEnvironment()
    {
        // Arrange
        var          env        = CreateSampleEnvironment();
        var          outputPath = CreateTempAasxPath();
        const string specPath   = "/aasx/spec.json";

        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        env.ToAasx(outputPath, specPath);

        // Assert
        File.Exists(outputPath).ShouldBeTrue();

        using var stream = File.OpenRead(outputPath);
        using var zip    = new ZipArchive(stream, ZipArchiveMode.Read);

        // Ensure a JSON entry exists with the expected relative path.

        var expectedEntryName = specPath.TrimStart('/');
        var jsonEntry         = zip.Entries.SingleOrDefault(e => e.FullName == expectedEntryName);
        jsonEntry.ShouldNotBeNull();

        using var entryStream = jsonEntry.Open();
        using var reader      = new StreamReader(entryStream);
        var       content     = reader.ReadToEnd();

        content.ShouldNotBeNullOrWhiteSpace();

        // Basic sanity check: JSON spec inside the .aasx still represents an AAS environment.
        var root = JsonNode.Parse(content)!.AsObject();
        root["assetAdministrationShells"].ShouldNotBeNull();
        var shells = root["assetAdministrationShells"]!.AsArray();
        shells.Count.ShouldBe(1);
        shells[0]!["id"]!.ToString().ShouldBe("urn:aas:example:my-shell");
        shells[0]!["idShort"]!.ToString().ShouldBe("MyShell");
    }

    [Fact]
    public void ToAasx_ShouldProducePackage_ThatCanBeReadBackWithExtractor()
    {
        // Arrange
        var          original   = CreateSampleEnvironment();
        var          outputPath = CreateTempAasxPath();
        const string specPath   = "/aasx/spec.json";

        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        original.ToAasx(outputPath, specPath);
        var extracted = AasxToAasEnvironmentExtractor.ExtractEnvironment(outputPath);

        // Assert
        extracted.ShouldNotBeNull();
        extracted.AssetAdministrationShells.ShouldNotBeNull();
        extracted.AssetAdministrationShells!.Count.ShouldBe(1);

        var shell = extracted.AssetAdministrationShells[0];
        shell.Id.ShouldBe("urn:aas:example:my-shell");
        shell.IdShort.ShouldBe("MyShell");
    }
}