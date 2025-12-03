using System.Text.Json;
using System.Text.RegularExpressions;

namespace FluentAas.ModelGeneration;

internal static class JsonContent
{
    public static (int version, int revision) ExtractVersionAndRevisionFromId(string id)
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
    
    public static string ToSafeIdentifier(string input)
    {
        var s = Regex.Replace(input, "[^A-Za-z0-9_]", "_");
        if (string.IsNullOrEmpty(s))
            return "_";
        if (char.IsDigit(s[0]))
            s = "_" + s;
        return s;
    }

    public static string ToPascalCase(string input)
    {
        var parts = Regex.Split(input, "[^A-Za-z0-9]+")
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => char.ToUpperInvariant(p[0]) + p[1..]);

        var result = string.Concat(parts);
        return string.IsNullOrEmpty(result) ? "Unnamed" : result;
    }

    public static string Escape(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"");

    public static string GetCardinality(JsonElement element)
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

        return "One"; // default
    }

    private static string MapValueType(string? valueType)
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

    public static string MapElementType(JsonElement element, out bool isMultiLang)
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
}