using System.Text;
using System.Text.Json;
using AasCore.Aas3.Package;
using FluentAas.IO;

namespace FluentAAS.IO;

public static class AasJsonToAasxConverter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Convert an AAS JSON Environment (provided as a string) to a .aasx package.
    /// </summary>
    /// <param name="aasJson">
    /// JSON string containing an AAS Environment as defined by aas-core-works.
    /// </param>
    /// <param name="aasxOutputPath">
    /// File system path where the resulting .aasx package will be written.
    /// </param>
    /// <param name="specificationUri"></param>
    public static void Convert(string aasJson, string aasxOutputPath, string specificationUri)
    {
        if (string.IsNullOrWhiteSpace(aasJson))
        {
            throw new ArgumentException("JSON content must not be null or empty.", nameof(aasJson));
        }

        if (string.IsNullOrWhiteSpace(aasxOutputPath))
        {
            throw new ArgumentException("Output path must not be null or empty.", nameof(aasxOutputPath));
        }

        // 1) Parse to JsonDocument (kept for basic validation, even though we don't use `doc` later)
        using var doc = JsonDocument.Parse(aasJson);

        // 2) Deserialize to AAS Environment using the official Jsonization helpers
        var environment = AasJsonSerializer.FromJson(aasJson);

        // 3) Serialize Environment to JSON structure expected by aasx packaging
        var jsonObject = AasJsonization.Serialize.ToJsonObject(environment);

        var serializedJson = JsonSerializer.Serialize(
            jsonObject,
            SerializerOptions);

        var specContent = Encoding.UTF8.GetBytes(serializedJson);

        // 4) Create the .aasx package
        var packaging = new Packaging();

        using var pkg = packaging.Create(aasxOutputPath);

        // 5) Add the JSON spec part with a canonical relative URI
        var specUri = new Uri(specificationUri, UriKind.Relative);
        var specPart = pkg.PutPart(
            specUri,
            "application/json",
            specContent);

        // Mark this part as the main AAS spec
        pkg.MakeSpec(specPart);

        // 6) Flush to disk
        pkg.Flush();
    }
}