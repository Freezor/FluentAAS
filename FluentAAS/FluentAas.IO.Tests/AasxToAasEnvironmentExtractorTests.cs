using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;
using AasCore.Aas3.Package;
using FluentAAS.Builder;
using FluentAAS.IO;
using Shouldly;
using File = System.IO.File;

namespace FluentAas.IO.Tests;

/// <summary>
/// Unit tests for <see cref="AasxToAasEnvironmentExtractor"/>.
/// </summary>
public class AasxToAasEnvironmentExtractorTests
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

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("   ")]
    public void ExtractEnvironment_ShouldThrowArgumentException_WhenPathIsNullOrWhitespace(string? aasxPath)
    {
        // Act
        var ex = Should.Throw<ArgumentException>(() => AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath!));

        // Assert
        ex.ParamName.ShouldBe("aasxPath");
        ex.Message.ShouldContain("AASX path must not be null or empty.");
    }

    [Fact]
    public void ExtractEnvironment_ShouldThrowArgumentException_WhenPackageHasNoJsonSpec()
    {
        // Arrange
        var aasxPath = CreateTempAasxPath();

        if (File.Exists(aasxPath))
        {
            File.Delete(aasxPath);
        }

        var packaging = new Packaging();
        using (var pkg = packaging.Create(aasxPath))
        {
            // Intentionally do NOT add any JSON spec parts
            pkg.Flush();
        }

        // Act
        var ex = Should.Throw<ArgumentException>(() => AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath));

        // Assert
        ex.ParamName.ShouldBe("aasxPath");
        ex.Message.ShouldContain("does not contain a JSON spec");
    }

    [Fact]
    public void ExtractEnvironment_ShouldReturnEnvironment_FromApplicationJsonSpec()
    {
        // Arrange
        var          env      = CreateSampleEnvironment();
        var          json     = AasJsonSerializer.ToJson(env);
        var          aasxPath = CreateTempAasxPath();
        const string specPath = "/aasx/spec.json";

        if (File.Exists(aasxPath))
        {
            File.Delete(aasxPath);
        }

        // Use the converter to produce a valid .aasx with application/json spec
        AasJsonToAasxConverter.Convert(json, aasxPath, specPath);

        // Act
        var extracted = AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath);

        // Assert
        extracted.ShouldNotBeNull();
        extracted.AssetAdministrationShells.ShouldNotBeNull();
        extracted.AssetAdministrationShells!.Count.ShouldBe(1);

        var shell = extracted.AssetAdministrationShells[0];
        shell.Id.ShouldBe("urn:aas:example:my-shell");
        shell.IdShort.ShouldBe("MyShell");

        // Basic sanity check that the submodel came along as well
        shell.Submodels.ShouldNotBeNull();
        shell.Submodels!.Count.ShouldBe(1);
    }

    [Fact]
    public void ExtractEnvironment_ShouldWork_WithTextJsonContentType()
    {
        // Arrange
        var env      = CreateSampleEnvironment();
        var json     = AasJsonSerializer.ToJson(env);
        var aasxPath = CreateTempAasxPath();

        if (File.Exists(aasxPath))
        {
            File.Delete(aasxPath);
        }

        var packaging = new Packaging();
        using (var pkg = packaging.Create(aasxPath))
        {
            var specBytes = Encoding.UTF8.GetBytes(json);
            var specUri   = new Uri("/aasx/spec-text-json.json", UriKind.Relative);

            // Note: content type text/json to hit the fallback branch.
            var part = pkg.PutPart(specUri, "text/json", specBytes);
            pkg.MakeSpec(part);
            pkg.Flush();
        }

        // Act
        var extracted = AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath);

        // Assert
        extracted.ShouldNotBeNull();
        extracted.AssetAdministrationShells.ShouldNotBeNull();
        extracted.AssetAdministrationShells!.Count.ShouldBe(1);
        extracted.AssetAdministrationShells[0].Id.ShouldBe("urn:aas:example:my-shell");
        extracted.AssetAdministrationShells[0].IdShort.ShouldBe("MyShell");
    }

    [Fact]
    public void ExtractEnvironment_ShouldThrowInvalidOperationException_WhenJsonSpecIsMalformed()
    {
        // Arrange
        const string invalidJson = "{ this is not valid JSON }";
        var          aasxPath    = CreateTempAasxPath();

        if (File.Exists(aasxPath))
        {
            File.Delete(aasxPath);
        }

        var packaging = new Packaging();
        using (var pkg = packaging.Create(aasxPath))
        {
            var specBytes = Encoding.UTF8.GetBytes(invalidJson);
            var specUri   = new Uri("/aasx/spec-invalid.json", UriKind.Relative);

            var part = pkg.PutPart(specUri, "application/json", specBytes);
            pkg.MakeSpec(part);
            pkg.Flush();
        }

        // Act
        var ex = Should.Throw<InvalidOperationException>(() => AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath));

        // Assert
        ex.Message.ShouldContain("Input is not valid JSON.");
    }

    [Fact]
    public void ExtractEnvironment_ShouldThrowInvalidOperationException_WhenJsonIsValidButNotEnvironment()
    {
        // Arrange
        const string notEnvJson = """{ "foo": "bar" }""";
        var          aasxPath   = CreateTempAasxPath();

        if (File.Exists(aasxPath))
        {
            File.Delete(aasxPath);
        }

        var packaging = new Packaging();
        using (var pkg = packaging.Create(aasxPath))
        {
            var specBytes = Encoding.UTF8.GetBytes(notEnvJson);
            var specUri   = new Uri("/aasx/spec-not-env.json", UriKind.Relative);

            var part = pkg.PutPart(specUri, "application/json", specBytes);
            pkg.MakeSpec(part);
            pkg.Flush();
        }

        // Act
        var ex = Should.Throw<InvalidOperationException>(() => AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath));

        // Assert
        ex.Message.ShouldContain("Failed to deserialize AAS environment from JSON.");
        ex.InnerException.ShouldNotBeNull();
        ex.InnerException.GetType().Name.ShouldBe("Exception"); // AasJsonization.Exception
    }

    [Fact]
    public void ExtractEnvironment_ShouldBeAbleToReadSpecPart_ThatWasWrittenAsZipEntry()
    {
        // Arrange
        var          env      = CreateSampleEnvironment();
        var          json     = AasJsonSerializer.ToJson(env);
        var          aasxPath = CreateTempAasxPath();
        const string specPath = "/aasx/spec.json";

        if (File.Exists(aasxPath))
        {
            File.Delete(aasxPath);
        }

        AasJsonToAasxConverter.Convert(json, aasxPath, specPath);

        // Quick structural check of the .aasx
        using (var stream = File.OpenRead(aasxPath))
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
        {
            var expectedEntryName = specPath.TrimStart('/');  // "aasx/spec.json"

            var jsonEntry = zip.Entries.SingleOrDefault(e => e.FullName == expectedEntryName);
            jsonEntry.ShouldNotBeNull();

            using var entryStream = jsonEntry.Open();
            using var reader      = new StreamReader(entryStream);
            var       content     = reader.ReadToEnd();

            content.ShouldNotBeNullOrWhiteSpace();

            var root = JsonNode.Parse(content)!.AsObject();
            root["assetAdministrationShells"].ShouldNotBeNull();
        }

        // Act
        var extracted = AasxToAasEnvironmentExtractor.ExtractEnvironment(aasxPath);

        // Assert
        extracted.AssetAdministrationShells.ShouldNotBeNull();
        extracted.AssetAdministrationShells!.Count.ShouldBe(1);
        extracted.AssetAdministrationShells[0].Id.ShouldBe("urn:aas:example:my-shell");
    }
}