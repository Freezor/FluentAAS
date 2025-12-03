using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FluentAas.ModelGeneration;

internal static class Program
{
    // Adjust this if your submodule sits in a different folder relative to the main repo root
    private const string PublishedFolderRelative = "externals/submodel-templates/published";

    private const string OutputRootNamespace = "FluentAas.Submodels";
    private const string OutputFolderRelative = "src/FluentAas.Submodels/Generated";

    private static int _generatedClassCount;
    private static int _skippedFileCount;

    private static async Task<int> Main(string[] args)
    {
        try
        {
            var repoRoot = FindRepoRoot();
            if (repoRoot == null)
            {
                Console.Error.WriteLine("ERROR: Could not locate repository root (no .git directory found).");
                return 1;
            }

            Console.WriteLine($"Repository root: {repoRoot}");

            var publishedFolder = Path.Combine(repoRoot, PublishedFolderRelative);
            if (!Directory.Exists(publishedFolder))
            {
                Console.Error.WriteLine($"ERROR: Published folder not found: {publishedFolder}");
                return 1;
            }

            var outputFolder = Path.Combine(repoRoot, OutputFolderRelative);
            Directory.CreateDirectory(outputFolder);
            Console.WriteLine($"Output folder: {outputFolder}");

            var jsonFiles = Directory.EnumerateFiles(publishedFolder, "*.json", SearchOption.AllDirectories)
                                     .ToList();

            Console.WriteLine($"Discovered {jsonFiles.Count} JSON files under '{publishedFolder}'.");

            foreach (var jsonFile in jsonFiles)
            {
                Console.WriteLine($"Processing JSON file: {jsonFile}");

                try
                {
                    await ProcessJsonFileAsync(jsonFile, outputFolder, publishedFolder);
                }
                catch (Exception ex)
                {
                    _skippedFileCount++;
                    Console.Error.WriteLine(
                        $"[SKIPPED] {jsonFile}: Unhandled exception during processing: {ex.Message}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Generation complete. Classes generated:   {_generatedClassCount}");
            Console.WriteLine($"Files skipped / failed:              {_skippedFileCount}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FATAL: {ex}");
            return 1;
        }
    }

    private static string? FindRepoRoot()
    {
        var dir = AppContext.BaseDirectory;
        var current = new DirectoryInfo(dir);

        while (current != null)
        {
            if (Directory.Exists(Path.Combine(current.FullName, ".git")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return null;
    }

    private static async Task ProcessJsonFileAsync(
        string jsonFile,
        string outputRootFolder,
        string publishedRootFolder)
    {
        string jsonContent;
        try
        {
            jsonContent = await File.ReadAllTextAsync(jsonFile);
        }
        catch (Exception ex)
        {
            _skippedFileCount++;
            Console.Error.WriteLine($"[SKIPPED] {jsonFile}: Failed to read JSON ({ex.Message}).");
            return;
        }

        JsonNode? root;
        try
        {
            root = JsonNode.Parse(jsonContent);
        }
        catch (Exception ex)
        {
            _skippedFileCount++;
            Console.Error.WriteLine($"[SKIPPED] {jsonFile}: Failed to parse JSON ({ex.Message}).");
            return;
        }

        if (root is null)
        {
            _skippedFileCount++;
            Console.Error.WriteLine($"[SKIPPED] {jsonFile}: Root JSON node is null.");
            return;
        }

        // Normalize the path for nicer logging / namespaces
        var relativePath = Path.GetRelativePath(publishedRootFolder, jsonFile);
        var safeFileBaseName = MakeSafeIdentifier(Path.GetFileNameWithoutExtension(jsonFile));

        // Try to locate "submodels" array (AAS environment style)
        var submodelsNode = root["submodels"] as JsonArray;

        // Some templates might be a single submodel at the root.
        // If no "submodels" array exists, check if root looks like a submodel.
        if (submodelsNode == null)
        {
            if (LooksLikeSubmodel(root))
            {
                submodelsNode = new JsonArray(root);
            }
            else
            {
                _skippedFileCount++;
                Console.Error.WriteLine(
                    $"[SKIPPED] {jsonFile}: No 'submodels' array and root does not look like a submodel.");
                return;
            }
        }

        if (submodelsNode.Count == 0)
        {
            _skippedFileCount++;
            Console.Error.WriteLine($"[SKIPPED] {jsonFile}: 'submodels' array is empty.");
            return;
        }

        // Create a folder tree under /Generated mirroring the relative path to some degree
        // Example: published/Digital nameplate/3/0/1/file.json
        // => Generated/Digital_nameplate/3_0_1/IDTA_02006_3_0_1_Template_Digital_Nameplate.g.cs
        var folderFromPathParts = MakeFolderFromRelativePath(relativePath);
        var targetFolder = Path.Combine(outputRootFolder, folderFromPathParts);
        Directory.CreateDirectory(targetFolder);

        // Root class name based on file name so you can always find it
        var rootClassName = safeFileBaseName;

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("// This file was generated by FluentAas.ModelGeneration.");
        sb.AppendLine($"// Source JSON: {relativePath}");
        sb.AppendLine();
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine($"namespace {OutputRootNamespace}.{folderFromPathParts.Replace(Path.DirectorySeparatorChar, '.')}");
        sb.AppendLine("{");

        // Emit a root container class that wraps all submodels from this file
        sb.AppendLine($"    public sealed class {rootClassName}");
        sb.AppendLine("    {");
        sb.AppendLine("        public IReadOnlyList<ISubmodelTemplate> Submodels { get; init; }");
        sb.AppendLine();
        sb.AppendLine($"        public {rootClassName}(IReadOnlyList<ISubmodelTemplate> submodels)");
        sb.AppendLine("        {");
        sb.AppendLine("            Submodels = submodels;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public interface ISubmodelTemplate");
        sb.AppendLine("    {");
        sb.AppendLine("        string Id { get; }");
        sb.AppendLine("        string IdShort { get; }");
        sb.AppendLine("    }");
        sb.AppendLine();

        // For each submodel in the file, generate a concrete class
        var submodelIndex = 0;
        foreach (var submodelNode in submodelsNode.OfType<JsonObject>())
        {
            submodelIndex++;

            var idShort = submodelNode["idShort"]?.GetValue<string>()?.Trim();
            var id = submodelNode["id"]?.GetValue<string>()?.Trim();

            var baseName = !string.IsNullOrWhiteSpace(idShort)
                ? MakeSafeIdentifier(idShort)
                : $"{safeFileBaseName}_Submodel_{submodelIndex.ToString(CultureInfo.InvariantCulture)}";

            var className = baseName.EndsWith("Submodel", StringComparison.OrdinalIgnoreCase)
                ? baseName
                : baseName + "Submodel";

            EmitSubmodelClass(sb, className, idShort ?? className, id ?? string.Empty, submodelNode);
        }

        sb.AppendLine("}");

        var outputFilePath = Path.Combine(targetFolder, $"{rootClassName}.g.cs");
        await File.WriteAllTextAsync(outputFilePath, sb.ToString(), Encoding.UTF8);
        _generatedClassCount++;

        Console.WriteLine($"[OK] Generated '{rootClassName}' from {jsonFile} -> {outputFilePath}");
    }

    private static bool LooksLikeSubmodel(JsonNode node)
    {
        if (node is not JsonObject obj) return false;

        var hasId = obj.ContainsKey("id");
        var hasIdShort = obj.ContainsKey("idShort");
        var hasSubmodelElements = obj["submodelElements"] is JsonArray;

        return hasId && hasIdShort && hasSubmodelElements;
    }

    private static void EmitSubmodelClass(
        StringBuilder sb,
        string className,
        string idShort,
        string id,
        JsonObject submodel)
    {
        sb.AppendLine($"    public sealed class {className} : ISubmodelTemplate");
        sb.AppendLine("    {");
        sb.AppendLine($"        public string Id {{ get; }} = \"{EscapeString(id)}\";");
        sb.AppendLine($"        public string IdShort {{ get; }} = \"{EscapeString(idShort)}\";");
        sb.AppendLine();

        // We now generate strongly typed properties for every submodelElement, recursively.
        var elements = submodel["submodelElements"] as JsonArray;
        if (elements != null)
        {
            foreach (var elementNode in elements.OfType<JsonObject>())
            {
                EmitElementProperty(sb, elementNode, indent: "        ");
            }
        }

        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitElementProperty(StringBuilder sb, JsonObject element, string indent)
    {
        var idShort = element["idShort"]?.GetValue<string>()?.Trim();
        var modelType = element["modelType"]?.GetValue<string>()?.Trim();
        var valueType = element["valueType"]?.GetValue<string>()?.Trim();

        if (string.IsNullOrWhiteSpace(idShort))
        {
            // Without an idShort we cannot produce a meaningful property name
            return;
        }

        var propName = MakeSafeIdentifier(idShort);

        // Decide C# type based on modelType + valueType
        var (typeName, isComplex, nestedCollection) = ResolveTypeForElement(modelType, valueType, element);

        if (!isComplex)
        {
            sb.AppendLine($"{indent}public {typeName}? {propName} {{ get; init; }}");
        }
        else
        {
            // Complex type -> nested class + property
            var nestedClassName = propName + "Element";

            if (nestedCollection)
            {
                sb.AppendLine($"{indent}public List<{nestedClassName}>? {propName} {{ get; init; }}");
            }
            else
            {
                sb.AppendLine($"{indent}public {nestedClassName}? {propName} {{ get; init; }}");
            }

            EmitNestedClass(sb, nestedClassName, element, indent);
        }
    }

    private static void EmitNestedClass(StringBuilder sb, string nestedClassName, JsonObject element, string indent)
    {
        var childIndent = indent + "    ";

        sb.AppendLine();
        sb.AppendLine($"{indent}public sealed class {nestedClassName}");
        sb.AppendLine($"{indent}{{");

        // For collections, children are in "value"
        var children = element["value"] as JsonArray;
        if (children != null)
        {
            foreach (var child in children.OfType<JsonObject>())
            {
                EmitElementProperty(sb, child, childIndent);
            }
        }

        sb.AppendLine($"{indent}}}");
        sb.AppendLine();
    }

    private static (string TypeName, bool IsComplex, bool IsCollection) ResolveTypeForElement(
        string? modelType,
        string? valueType,
        JsonObject element)
    {
        // Very simple heuristics; can be refined as needed
        modelType = modelType ?? element["modelType"]?.GetValue<string>()?.Trim();

        switch (modelType)
        {
            case "Property":
                return (MapPrimitiveType(valueType), false, false);
            case "MultiLanguageProperty":
                return ("Dictionary<string, string>", false, false);
            case "File":
                // Could be a richer "FileElement" record later
                return ("string", false, false);
            case "ReferenceElement":
                return ("string", false, false); // reference as string/IRI for now
            case "SubmodelElementCollection":
                return ("object", true, true);
            default:
                // Fallback: treat as complex collection if it has "value" array, otherwise string
                var value = element["value"];
                if (value is JsonArray)
                {
                    return ("object", true, true);
                }
                return ("string", false, false);
        }
    }

    private static string MapPrimitiveType(string? valueType)
    {
        var vt = valueType?.Trim() ?? string.Empty;
        return vt switch
        {
            "xs:int" or "xs:integer" => "int",
            "xs:long" => "long",
            "xs:double" => "double",
            "xs:float" => "float",
            "xs:decimal" => "decimal",
            "xs:boolean" => "bool",
            _ => "string"
        };
    }

    private static string MakeSafeIdentifier(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "Unnamed";

        // Replace problematic chars with underscore
        var cleaned = new StringBuilder(raw.Length);
        var first = true;

        foreach (var ch in raw)
        {
            if (char.IsLetterOrDigit(ch))
            {
                if (first && char.IsDigit(ch))
                {
                    cleaned.Append('_');
                }
                cleaned.Append(ch);
            }
            else
            {
                cleaned.Append('_');
            }

            first = false;
        }

        var result = cleaned.ToString();
        if (string.IsNullOrWhiteSpace(result))
            result = "Unnamed";

        // PascalCase-ish: split by '_' and capitalize
        var parts = result.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        var pascal = string.Concat(parts.Select(p =>
            char.ToUpperInvariant(p[0]) + (p.Length > 1 ? p.Substring(1) : string.Empty)));

        return pascal;
    }

    private static string MakeFolderFromRelativePath(string relativePath)
    {
        var dir = Path.GetDirectoryName(relativePath) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(dir))
            return "Root";

        // Normalize separators and "clean" each segment
        var segments = dir.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var cleanedSegments = segments
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(MakeSafeIdentifier);

        var combined = Path.Combine(cleanedSegments.ToArray());
        return string.IsNullOrWhiteSpace(combined) ? "Root" : combined;
    }

    private static string EscapeString(string s)
        => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
