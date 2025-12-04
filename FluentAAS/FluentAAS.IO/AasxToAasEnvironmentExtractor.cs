using System.Text;
using AasCore.Aas3.Package;
using FluentAas.IO;

namespace FluentAAS.IO;

/// <summary>
/// Extracts an AAS Environment from an .aasx package.
/// </summary>
public static class AasxToAasEnvironmentExtractor
{
    /// <summary>
    /// Open an .aasx file, read the JSON environment spec and return it as an
    /// <see cref="Environment"/>.
    /// </summary>
    /// <param name="aasxPath">
    /// File system path to the .aasx package.
    /// </param>
    /// <returns>
    /// The deserialized <see cref="Environment"/> contained in the package.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the package does not contain a JSON spec.
    /// </exception>
    public static Environment ExtractEnvironment(string aasxPath)
    {
        if (string.IsNullOrWhiteSpace(aasxPath))
        {
            throw new ArgumentException("AASX path must not be null or empty.", nameof(aasxPath));
        }

        var packaging = new Packaging();

        using var packageOrException = packaging.OpenRead(aasxPath);
        var       packageRead      = packageOrException.Must();

        // 1) Find the JSON spec part
        var specsByContentType = packageRead.SpecsByContentType();

        // Prefer application/json if present, but fall back to text/json like in example.
        string[] preferredContentTypes =
        [
            "application/json",
            "text/json"
        ];

        byte[]? specContent = null;

        foreach (var contentType in preferredContentTypes)
        {
            if (!specsByContentType.TryGetValue(contentType, out var specs) || specs.Count == 0)
            {
                continue;
            }

            var specPart = specs.First();
            specContent = specPart.ReadAllBytes();
            break;
        }

        if (specContent is null)
        {
            throw new ArgumentException(
                                        "The .aasx package does not contain a JSON spec (content-type application/json or text/json).",
                                        nameof(aasxPath));
        }

        // 2) Decode JSON and deserialize to Environment
        var json = Encoding.UTF8.GetString(specContent);

        // Uses the same serializer as the converter going the other way
        var environment = AasJsonSerializer.FromJson(json);

        return environment;
    }
}