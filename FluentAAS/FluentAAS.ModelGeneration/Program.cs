// src/FluentAas.ModelGeneration/Program.cs
//
// Code generator for IDTA Submodel JSON templates.
// - Scans ALL *.json files under "externals/submodel-templates/published" (recursively).
// - For every *.json file, creates a C# class in FluentAas.Submodels/Generated.
// - The generated class does NOT embed the JSON (no TemplateJsonBase64).
// - Instead, it exposes strongly-typed properties for ALL submodel elements
//   (including nested SubmodelElementCollections), with types derived from
//   modelType + valueType + cardinality.
//
// Notes:
// - JSON in the external submodule is only READ and never modified.
// - You can delete any previously copied JSON artifacts from src/ manually;
//   this generator no longer creates any JSON files in src/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

// Relative paths from repo root
const string PublishedFolderRelative = "externals/submodel-templates/published";
const string SubmodelsProjectRelative = "src/FluentAas.Submodels";

var repoRoot = FindRepoRoot();
var publishedRoot = Path.Combine(repoRoot, PublishedFolderRelative);
var submodelsRoot = Path.Combine(repoRoot, SubmodelsProjectRelative);
var outputRoot = Path.Combine(submodelsRoot, "Generated");

Directory.CreateDirectory(outputRoot);

Console.WriteLine("=== FluentAas.ModelGeneration ===");
Console.WriteLine($"Repo root:              {repoRoot}");
Console.WriteLine($"Published templates:    {publishedRoot}");
Console.WriteLine($"Submodels project root: {submodelsRoot}");
Console.WriteLine($"Output folder:          {outputRoot}");
Console.WriteLine();

if (!Directory.Exists(publishedRoot))
{
    Console.WriteLine("ERROR: Published root folder not found.");
    Console.WriteLine("       Expected at: " + publishedRoot);
    return;
}

// 1. Find ALL JSON files in "published" and ALL subfolders.
var jsonFiles = Directory.EnumerateFiles(publishedRoot, "*.json", SearchOption.AllDirectories).ToList();

Console.WriteLine($"Found {jsonFiles.Count} JSON file(s) in 'published' (recursive).");
Console.WriteLine();

if (jsonFiles.Count == 0)
{
    Console.WriteLine("No JSON templates found. Nothing to do.");
    return;
}

var processed = 0;
var skippedCount = 0;
var failedCount = 0;
var skippedFiles = new List<string>();
var failedFiles = new List<string>();

foreach (var jsonFile in jsonFiles)
{
    processed++;
    var relativePath = Path.GetRelativePath(publishedRoot, jsonFile);
    Console.WriteLine($"[{processed}/{jsonFiles.Count}] Processing: {relativePath}");

    try
    {
        var jsonContent = File.ReadAllText(jsonFile, Encoding.UTF8);

        using var doc = JsonDocument.Parse(jsonContent);
        var root = doc.RootElement;

        // Expect IDTA-style AAS environment:
        // {
        //   "assetAdministrationShells": [...],
        //   "submodels": [ { "idShort": "...", "id": "https://.../SubmodelTemplate/Name/1/0", ... } ],
        //   "conceptDescriptions": [...]
        // }
        if (!root.TryGetProperty("submodels", out var submodelsArray) || submodelsArray.GetArrayLength() == 0)
        {
            Console.WriteLine("  -> Skipped: JSON has no 'submodels' array.");
            skippedCount++;
            skippedFiles.Add(relativePath);
            Console.WriteLine();
            continue;
        }

        var submodel = submodelsArray[0];

        if (!submodel.TryGetProperty("idShort", out var idShortProp))
        {
            Console.WriteLine("  -> Skipped: submodel[0] has no 'idShort'.");
            skippedCount++;
            skippedFiles.Add(relativePath);
            Console.WriteLine();
            continue;
        }

        var idShort = idShortProp.GetString() ?? "UnknownSubmodel";

        if (!submodel.TryGetProperty("id", out var idProp))
        {
            Console.WriteLine("  -> Skipped: submodel[0] has no 'id'.");
            skippedCount++;
            skippedFiles.Add(relativePath);
            Console.WriteLine();
            continue;
        }

        var submodelId = idProp.GetString() ?? "";
        var (version, revision) = ExtractVersionAndRevisionFromId(submodelId);

        var className = ToSafeIdentifier($"{ToPascalCase(idShort)}_{version}_{revision}");
        var ns = $"FluentAas.Submodels.{ToSafeIdentifier(ToPascalCase(idShort))}";

        Console.WriteLine($"  -> idShort:     {idShort}");
        Console.WriteLine($"  -> submodelId:  {submodelId}");
        Console.WriteLine($"  -> version:     {version}, revision: {revision}");
        Console.WriteLine($"  -> class:       {ns}.{className}");

        if (!submodel.TryGetProperty("submodelElements", out var elementsArray) ||
            elementsArray.ValueKind != JsonValueKind.Array ||
            elementsArray.GetArrayLength() == 0)
        {
            Console.WriteLine("  -> Skipped: submodel[0] has no 'submodelElements'.");
            skippedCount++;
            skippedFiles.Add(relativePath);
            Console.WriteLine();
            continue;
        }

        var code = GenerateClassSource(
            ns,
            className,
            idShort,
            submodelId,
            version,
            revision,
            relativePath,
            elementsArray);

        var safeIdShort = SanitizeForPath(idShort);
        var outDir = Path.Combine(outputRoot, safeIdShort);
        Directory.CreateDirectory(outDir);
        var outFile = Path.Combine(outDir, $"{className}.g.cs");

        File.WriteAllText(outFile, code, Encoding.UTF8);

        Console.WriteLine($"  -> Generated:   {Path.GetRelativePath(repoRoot, outFile)}");
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine("  !! ERROR while processing this file:");
        Console.WriteLine("     " + ex.GetType().Name + ": " + ex.Message);
        failedCount++;
        failedFiles.Add(relativePath);
        Console.WriteLine();
    }
}

Console.WriteLine("=== Generation finished ===");
Console.WriteLine($"Total JSON files found: {jsonFiles.Count}");
Console.WriteLine($"Successfully processed: {jsonFiles.Count - skippedCount - failedCount}");
Console.WriteLine($"Skipped:               {skippedCount}");
Console.WriteLine($"Failed:                {failedCount}");
Console.WriteLine();

if (skippedFiles.Count > 0)
{
    Console.WriteLine("Skipped files:");
    foreach (var f in skippedFiles)
        Console.WriteLine("  - " + f);
    Console.WriteLine();
}

if (failedFiles.Count > 0)
{
    Console.WriteLine("Failed files:");
    foreach (var f in failedFiles)
        Console.WriteLine("  - " + f);
    Console.WriteLine();
}


// ---------------- Helpers ----------------

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);

    while (dir != null)
    {
        if (Directory.Exists(Path.Combine(dir.FullName, ".git")) ||
            File.Exists(Path.Combine(dir.FullName, ".gitignore")))
        {
            return dir.FullName;
        }

        dir = dir.Parent!;
    }

    throw new InvalidOperationException("Repository root not found.");
}

static string ToSafeIdentifier(string input)
{
    var s = Regex.Replace(input, "[^A-Za-z0-9_]", "_");
    if (string.IsNullOrEmpty(s))
        return "_";
    if (char.IsDigit(s[0]))
        s = "_" + s;
    return s;
}

static string ToPascalCase(string input)
{
    var parts = Regex.Split(input, "[^A-Za-z0-9]+")
        .Where(p => !string.IsNullOrWhiteSpace(p))
        .Select(p => char.ToUpperInvariant(p[0]) + p[1..]);

    var result = string.Concat(parts);
    return string.IsNullOrEmpty(result) ? "Unnamed" : result;
}

static string SanitizeForPath(string input)
{
    foreach (var c in Path.GetInvalidFileNameChars())
        input = input.Replace(c, '_');
    return input;
}

// Extracts version and revision from the submodel "id" URL.
// Example: https://admin-shell.io/idta/SubmodelTemplate/AIDataset/1/0  => (1, 0)
static (int version, int revision) ExtractVersionAndRevisionFromId(string id)
{
    if (string.IsNullOrWhiteSpace(id))
        return (0, 0);

    var parts = id.Split('/', StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length < 2)
        return (0, 0);

    if (int.TryParse(parts[^2], out var version) &&
        int.TryParse(parts[^1], out var revision))
    {
        return (version, revision);
    }

    return (0, 0);
}

static string GetCardinality(JsonElement element)
{
    if (element.TryGetProperty("qualifiers", out var qualifiers) &&
        qualifiers.ValueKind == JsonValueKind.Array)
    {
        foreach (var q in qualifiers.EnumerateArray())
        {
            if (q.TryGetProperty("type", out var typeProp) &&
                typeProp.GetString() == "SMT/Cardinality" &&
                q.TryGetProperty("value", out var valueProp))
            {
                return valueProp.GetString() ?? "One";
            }
        }
    }

    return "One"; // sensible default
}

static string MapValueType(string? valueType)
{
    if (string.IsNullOrWhiteSpace(valueType))
        return "string";

    var v = valueType.Trim().ToLowerInvariant();

    return v switch
    {
        "xs:string" => "string",
        "xs:int" or "xs:integer" => "int",
        "xs:double" => "double",
        "xs:decimal" => "decimal",
        "xs:boolean" => "bool",
        _ => "string"
    };
}

static string MapElementType(JsonElement element, out bool isMultiLang)
{
    isMultiLang = false;

    var modelType = element.TryGetProperty("modelType", out var mt)
        ? mt.GetString()
        : null;

    if (string.Equals(modelType, "MultiLanguageProperty", StringComparison.OrdinalIgnoreCase))
    {
        isMultiLang = true;
        return "LocalizedText";
    }

    if (string.Equals(modelType, "Property", StringComparison.OrdinalIgnoreCase))
    {
        var valueType = element.TryGetProperty("valueType", out var vt)
            ? vt.GetString()
            : null;

        return MapValueType(valueType);
    }

    // For File / ReferenceElement / others, default to string
    return "string";
}

static string GenerateClassSource(
    string ns,
    string className,
    string submodelIdShort,
    string submodelId,
    int version,
    int revision,
    string sourceJsonRelativePath,
    JsonElement submodelElementsArray)
{
    var sb = new StringBuilder();

    var usesLocalizedText = false;

    sb.AppendLine("// <auto-generated />");
    sb.AppendLine("// This file was generated by FluentAas.ModelGeneration.");
    sb.AppendLine("// Do not edit manually.");
    sb.AppendLine();
    sb.AppendLine("using System;");
    sb.AppendLine("using System.Collections.Generic;");
    sb.AppendLine();
    sb.AppendLine($"namespace {ns}");
    sb.AppendLine("{");
    sb.AppendLine("    /// <summary>");
    sb.AppendLine("    /// Strongly-typed representation of the IDTA submodel template.");
    sb.AppendLine($"    /// Source JSON (external): {sourceJsonRelativePath}");
    sb.AppendLine($"    /// Submodel ID: {Escape(submodelId)}");
    sb.AppendLine("    /// </summary>");
    sb.AppendLine($"    public class {className}");
    sb.AppendLine("    {");
    sb.AppendLine("        /// <summary>Submodel idShort from JSON.</summary>");
    sb.AppendLine($"        public const string SubmodelIdShort = \"{Escape(submodelIdShort)}\";");
    sb.AppendLine();
    sb.AppendLine("        /// <summary>Submodel technical ID from JSON.</summary>");
    sb.AppendLine($"        public const string SubmodelId = \"{Escape(submodelId)}\";");
    sb.AppendLine();
    sb.AppendLine("        /// <summary>Semantic version major extracted from submodel id.</summary>");
    sb.AppendLine($"        public const int Version = {version};");
    sb.AppendLine();
    sb.AppendLine("        /// <summary>Semantic version minor extracted from submodel id.</summary>");
    sb.AppendLine($"        public const int Revision = {revision};");
    sb.AppendLine();

    // Generate properties for all submodelElements (recursively).
    sb.AppendLine("        // Root-level submodel elements");
    GenerateElementMembers(
        sb,
        submodelElementsArray,
        indent: "        ",
        nestedTypePrefix: "",
        ref usesLocalizedText);

    sb.AppendLine("    }");

    if (usesLocalizedText)
    {
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Simple representation of a localized text (for MultiLanguageProperty elements).");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public record LocalizedText(string Language, string Text);");
    }

    sb.AppendLine("}");

    return sb.ToString();
}

static void GenerateElementMembers(
    StringBuilder sb,
    JsonElement elementsArray,
    string indent,
    string nestedTypePrefix,
    ref bool usesLocalizedText)
{
    foreach (var element in elementsArray.EnumerateArray())
    {
        if (!element.TryGetProperty("idShort", out var idShortProp))
            continue;

        var idShort = idShortProp.GetString() ?? "Unnamed";
        var propertyName = ToPascalCase(idShort);

        var modelType = element.TryGetProperty("modelType", out var mt)
            ? mt.GetString() ?? "Property"
            : "Property";

        var cardinality = GetCardinality(element);
        var isCollectionCardinality =
            string.Equals(cardinality, "OneToMany", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(cardinality, "ZeroToMany", StringComparison.OrdinalIgnoreCase);

        // Try to extract semanticId for documentation
        string? semanticIdValue = null;
        if (element.TryGetProperty("semanticId", out var semId) &&
            semId.ValueKind == JsonValueKind.Object &&
            semId.TryGetProperty("keys", out var keysArr) &&
            keysArr.ValueKind == JsonValueKind.Array &&
            keysArr.GetArrayLength() > 0)
        {
            var key0 = keysArr[0];
            if (key0.TryGetProperty("value", out var valProp))
            {
                semanticIdValue = valProp.GetString();
            }
        }

        // Add XML doc comment with modelType, cardinality, semanticId
        sb.AppendLine();
        sb.AppendLine($"{indent}/// <summary>");
        sb.AppendLine($"{indent}/// Element '{idShort}' (modelType: {modelType}, cardinality: {cardinality}" +
                      (semanticIdValue is not null ? $", semanticId: {Escape(semanticIdValue)}" : "") +
                      ").");
        sb.AppendLine($"{indent}/// </summary>");

        if (string.Equals(modelType, "SubmodelElementCollection", StringComparison.OrdinalIgnoreCase))
        {
            // Nested class representing this collection
            var nestedTypeName = nestedTypePrefix + propertyName;

            // Define nested type once
            sb.AppendLine($"{indent}public class {nestedTypeName}");
            sb.AppendLine($"{indent}{{");

            if (element.TryGetProperty("value", out var valueArr) &&
                valueArr.ValueKind == JsonValueKind.Array)
            {
                GenerateElementMembers(
                    sb,
                    valueArr,
                    indent + "    ",
                    nestedTypeName + "_",
                    ref usesLocalizedText);
            }

            sb.AppendLine($"{indent}}}");
            sb.AppendLine();

            var propType = isCollectionCardinality
                ? $"List<{nestedTypeName}>"
                : nestedTypeName + "?";

            sb.AppendLine($"{indent}public {propType} {propertyName} {{ get; set; }}");
        }
        else
        {
            var elementType = MapElementType(element, out var isMultiLang);
            if (isMultiLang)
                usesLocalizedText = true;

            var typeName = isCollectionCardinality
                ? $"List<{elementType}>"
                : elementType + "?";

            sb.AppendLine($"{indent}public {typeName} {propertyName} {{ get; set; }}");
        }
    }
}

static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
