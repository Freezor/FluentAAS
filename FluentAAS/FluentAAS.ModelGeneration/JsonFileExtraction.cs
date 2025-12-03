using System.Text;
using System.Text.Json;

namespace FluentAas.ModelGeneration;

internal static class JsonFileExtraction
{
    public static void ProcessJsonFile(string rootNamespace, string jsonFile, string publishedRoot, string outputRoot)
    {
        string jsonContent;
        try
        {
            jsonContent = File.ReadAllText(jsonFile, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            LoggingMarker.MarkSkipped(jsonFile, publishedRoot, $"Failed to read file: {ex.Message}");
            return;
        }

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(jsonContent);
        }
        catch (Exception ex)
        {
            LoggingMarker.MarkSkipped(jsonFile, publishedRoot, $"Failed to parse JSON: {ex.Message}");
            return;
        }

        using (doc)
        {
            var         root = doc.RootElement;
            JsonElement submodelsArray;

            // Either AAS environment with "submodels" array...
            if (root.TryGetProperty("submodels", out var smArray) &&
                smArray.ValueKind == JsonValueKind.Array &&
                smArray.GetArrayLength() > 0)
            {
                submodelsArray = smArray;
            }
            // ...or a single submodel at root
            else if (SubmodelFinder.LooksLikeSubmodel(root))
            {
                var arr = new JsonElement[1];
                arr[0]         = root;
                submodelsArray = JsonElementCreation.CreateArray(arr);
            }
            else
            {
                LoggingMarker.MarkSkipped(jsonFile, publishedRoot,
                                    "No 'submodels' array and root does not look like a submodel.");
                return;
            }

            Console.WriteLine($"  -> Submodels in file: {submodelsArray.GetArrayLength()}");

            var relativePath = Path.GetRelativePath(publishedRoot, jsonFile);

            var index = 0;
            foreach (var submodel in submodelsArray.EnumerateArray())
            {
                index++;

                if (!submodel.TryGetProperty("idShort", out var idShortProp))
                {
                    Console.WriteLine($"  -> Skipping submodel #{index}: no 'idShort'.");
                    continue;
                }

                var idShort = idShortProp.GetString() ?? "UnnamedSubmodel";

                if (!submodel.TryGetProperty("id", out var idProp))
                {
                    Console.WriteLine($"  -> Skipping submodel '{idShort}': no 'id'.");
                    continue;
                }

                var submodelId = idProp.GetString() ?? "";
                var (version, revision) = JsonContent.ExtractVersionAndRevisionFromId(submodelId);

                var baseName = JsonContent.ToPascalCase(idShort); // e.g. "DigitalNameplate"
                var className = version == 0 && revision == 0
                                    ? JsonContent.ToSafeIdentifier(baseName)
                                    : JsonContent.ToSafeIdentifier($"{baseName}V{version}_{revision}"); // e.g. "DigitalNameplateV3_0"


                Console.WriteLine($"  -> Submodel #{index}");
                Console.WriteLine($"     idShort:   {idShort}");
                Console.WriteLine($"     id:        {submodelId}");
                Console.WriteLine($"     version:   {version}, revision: {revision}");
                Console.WriteLine($"     class:     {rootNamespace}.{className}");

                if (!submodel.TryGetProperty("submodelElements", out var elementsArray) ||
                    elementsArray.ValueKind != JsonValueKind.Array ||
                    elementsArray.GetArrayLength() == 0)
                {
                    Console.WriteLine("     -> Skipped this submodel: no 'submodelElements'.");
                    continue;
                }

                var code = SubmodelGenerator.GenerateClassSource(
                                                       rootNamespace,
                                                       className,
                                                       idShort,
                                                       submodelId,
                                                       version,
                                                       revision,
                                                       relativePath,
                                                       elementsArray);

                Directory.CreateDirectory(outputRoot);
                var outFile = Path.Combine(outputRoot, $"{className}.g.cs");
                File.WriteAllText(outFile, code, Encoding.UTF8);

                LoggingMarker.IncreaseGeneratedClassesCount();
                Console.WriteLine($"     -> Generated: {Path.GetRelativePath(RepositoryPath.FindRepoRoot(), outFile)}");
                Console.WriteLine();
            }
        }
    }
}