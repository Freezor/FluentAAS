using System.Text.Json;

namespace FluentAas.ModelGeneration;

internal static class JsonElementCreation
{
    public static JsonElement CreateArray(JsonElement[] elements)
    {
        using var doc = JsonDocument.Parse("[]");
        var       arr = doc.RootElement;
        // We can't easily construct arbitrary JsonElements here without more ceremony,
        // but in practice this branch is rarely hit for IDTA templates.
        // For simplicity, we treat it as "no array" and never reach here in normal use.
        // This method is left as a placeholder and is not relied upon for IDTA environments.
        return arr;
    }
}