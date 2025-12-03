// src/FluentAas.ModelGeneration/Program.cs
//
// Code generator for IDTA Submodel JSON templates.
// - Scans a "published" folder (from git submodule).
// - For every *.json file, creates a C# class into FluentAas.Submodels/Generated.
// - Each generated class wraps the full JSON and exposes typed metadata + factory methods.

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

const string PublishedFolderRelative = "externals/submodel-templates/published";
const string SubmodelsProjectRelative = "src/FluentAas.Submodels";

var repoRoot = FindRepoRoot();
var publishedRoot = Path.Combine(repoRoot, PublishedFolderRelative);
var submodelsRoot = Path.Combine(repoRoot, SubmodelsProjectRelative);
var outputRoot = Path.Combine(submodelsRoot, "Generated");
var resourcesRoot = Path.Combine(submodelsRoot, "Resources");

Directory.CreateDirectory(outputRoot);
Directory.CreateDirectory(resourcesRoot);

Console.WriteLine($"Repo root:       {repoRoot}");
Console.WriteLine($"Published root:  {publishedRoot}");
Console.WriteLine($"Submodels root:  {submodelsRoot}");
Console.WriteLine($"Output root:     {outputRoot}");
Console.WriteLine($"Resources root:  {resourcesRoot}");

// 1. Find all JSON files in "published".
var jsonFiles = Directory.EnumerateFiles(publishedRoot, "*.json", SearchOption.AllDirectories).ToList();
Console.WriteLine($"Found {jsonFiles.Count} JSON templates.");

foreach (var jsonFile in jsonFiles)
{
    var relativePath = Path.GetRelativePath(publishedRoot, jsonFile);
    Console.WriteLine($"Processing: {relativePath}");

    using var stream = File.OpenRead(jsonFile);
    using var doc = JsonDocument.Parse(stream);

    var root = doc.RootElement;

    // We assume the IDTA environment structure:
    // {
    //   "assetAdministrationShells": [...],
    //   "submodels": [ { "idShort": "...", "id": "https://.../SubmodelTemplate/Name/1/0", ... } ],
    //   "conceptDescriptions": [...]
    // }
    if (!root.TryGetProperty("submodels", out var submodelsArray) || submodelsArray.GetArrayLength() == 0)
    {
        Console.WriteLine("  Skipped (no submodels array).");
        continue;
    }

    var submodel = submodelsArray[0];

    var idShort = submodel.GetProperty("idShort").GetString() ?? "UnknownSubmodel";
    var submodelId = submodel.GetProperty("id").GetString() ?? "";
    var (version, revision) = ExtractVersionAndRevisionFromId(submodelId);

    var className = ToSafeIdentifier($"{ToPascalCase(idShort)}_{version}_{revision}");
    var ns = $"FluentAas.Submodels.{ToSafeIdentifier(ToPascalCase(idShort))}";

    // Copy JSON to Resources with a deterministic file name.
    var safeIdShort = SanitizeForPath(idShort);
    var resourceSubDir = Path.Combine(resourcesRoot, safeIdShort, $"{version}_{revision}");
    Directory.CreateDirectory(resourceSubDir);

    var jsonFileName = Path.GetFileName(jsonFile);
    var destJsonPath = Path.Combine(resourceSubDir, jsonFileName);
    File.Copy(jsonFile, destJsonPath, overwrite: true);

    // Resource name for EmbeddedResource resolution
    // Example: FluentAas.Submodels.Resources.AIDataset.1_0.Template.json
    var resourceName = $"FluentAas.Submodels.Resources.{SanitizeForResource(idShort)}.{version}_{revision}.{jsonFileName}";

    // Collect the most important submodel element IDs as constants (optional but handy).
    var elementIdShorts = new List<string>();
    if (submodel.TryGetProperty("submodelElements", out var elements))
    {
        foreach (var el in elements.EnumerateArray())
        {
            if (el.TryGetProperty("idShort", out var elIdShortProp))
            {
                var elIdShort = elIdShortProp.GetString();
                if (!string.IsNullOrWhiteSpace(elIdShort))
                    elementIdShorts.Add(elIdShort);
            }
        }
    }

    var code = GenerateClassSource(
        ns,
        className,
        idShort,
        submodelId,
        version,
        revision,
        resourceName,
        relativePath,
        elementIdShorts);

    // Write C# file.
    var outDir = Path.Combine(outputRoot, safeIdShort);
    Directory.CreateDirectory(outDir);
    var outFile = Path.Combine(outDir, $"{className}.g.cs");
    File.WriteAllText(outFile, code, Encoding.UTF8);
}

Console.WriteLine("Generation finished.");


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

static string SanitizeForResource(string input)
{
    return Regex.Replace(input, "[^A-Za-z0-9_]", "_");
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

static string GenerateClassSource(
    string ns,
    string className,
    string submodelIdShort,
    string submodelId,
    int version,
    int revision,
    string resourceName,
    string sourceJsonRelativePath,
    IReadOnlyList<string> elementIdShorts)
{
    var sb = new StringBuilder();

    sb.AppendLine("// <auto-generated />");
    sb.AppendLine("// This file was generated by FluentAas.ModelGeneration.");
    sb.AppendLine("// Do not edit manually.");
    sb.AppendLine();
    sb.AppendLine("using System;");
    sb.AppendLine("using FluentAas.Core.Models;");
    sb.AppendLine("using FluentAas.IO;");
    sb.AppendLine();
    sb.AppendLine($"namespace {ns}");
    sb.AppendLine("{");
    sb.AppendLine($"    /// <summary>");
    sb.AppendLine($"    /// Strongly-typed wrapper for the IDTA submodel template JSON.");
    sb.AppendLine($"    /// Source JSON: {sourceJsonRelativePath}");
    sb.AppendLine($"    /// Submodel ID: {submodelId}");
    sb.AppendLine($"    /// </summary>");
    sb.AppendLine($"    public static class {className}");
    sb.AppendLine("    {");
    sb.AppendLine($"        /// <summary>Submodel idShort from JSON.</summary>");
    sb.AppendLine($"        public const string SubmodelIdShort = \"{Escape(submodelIdShort)}\";");
    sb.AppendLine();
    sb.AppendLine($"        /// <summary>Submodel technical ID from JSON.</summary>");
    sb.AppendLine($"        public const string SubmodelId = \"{Escape(submodelId)}\";");
    sb.AppendLine();
    sb.AppendLine($"        /// <summary>Semantic version major extracted from submodel id.</summary>");
    sb.AppendLine($"        public const int Version = {version};");
    sb.AppendLine();
    sb.AppendLine($"        /// <summary>Semantic version minor extracted from submodel id.</summary>");
    sb.AppendLine($"        public const int Revision = {revision};");
    sb.AppendLine();
    sb.AppendLine($"        /// <summary>");
    sb.AppendLine($"        /// Fully qualified embedded resource name for the JSON template.");
    sb.AppendLine($"        /// </summary>");
    sb.AppendLine($"        public const string ResourceName = \"{Escape(resourceName)}\";");
    sb.AppendLine();

    sb.AppendLine("        /// <summary>");
    sb.AppendLine("        /// Creates a full AAS environment (asset administration shells, submodels, concept descriptions)");
    sb.AppendLine("        /// from the embedded JSON template.");
    sb.AppendLine("        /// </summary>");
    sb.AppendLine("        public static AasEnvironment CreateEnvironment(IAasJsonSerializer serializer)");
    sb.AppendLine("        {");
    sb.AppendLine("            if (serializer is null) throw new ArgumentNullException(nameof(serializer));");
    sb.AppendLine();
    sb.AppendLine("            var assembly = typeof(" + className + ").Assembly;");
    sb.AppendLine("            using var stream = assembly.GetManifestResourceStream(ResourceName)");
    sb.AppendLine("                ?? throw new InvalidOperationException($\"Embedded resource '{ResourceName}' not found.\");");
    sb.AppendLine();
    sb.AppendLine("            return serializer.DeserializeEnvironment(stream);");
    sb.AppendLine("        }");
    sb.AppendLine();

    sb.AppendLine("        /// <summary>");
    sb.AppendLine("        /// Creates the first (template) Submodel from the embedded JSON template.");
    sb.AppendLine("        /// </summary>");
    sb.AppendLine("        public static Submodel CreateSubmodel(IAasJsonSerializer serializer)");
    sb.AppendLine("        {");
    sb.AppendLine("            var env = CreateEnvironment(serializer);");
    sb.AppendLine("            if (env.Submodels is null || env.Submodels.Count == 0)");
    sb.AppendLine("                throw new InvalidOperationException(\"Template JSON does not contain any submodels.\");");
    sb.AppendLine();
    sb.AppendLine("            return env.Submodels[0];");
    sb.AppendLine("        }");
    sb.AppendLine();

    if (elementIdShorts.Count > 0)
    {
        sb.AppendLine("        /// <summary>");
        sb.AppendLine("        /// Constants for root-level SubmodelElement idShorts from the template.");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine("        public static class Elements");
        sb.AppendLine("        {");
        foreach (var e in elementIdShorts.Distinct())
        {
            var constName = ToSafeIdentifier(e);
            sb.AppendLine($"            public const string {constName} = \"{Escape(e)}\";");
        }
        sb.AppendLine("        }");
    }

    sb.AppendLine("    }");
    sb.AppendLine("}");

    return sb.ToString();

    static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");

    static string ToSafeIdentifier(string input)
    {
        var s = Regex.Replace(input, "[^A-Za-z0-9_]", "_");
        if (string.IsNullOrEmpty(s))
            return "_";
        if (char.IsDigit(s[0]))
            s = "_" + s;
        return s;
    }
}
