using System.Text.Json;

namespace FluentAas.ModelGeneration;

internal static class SubmodelFinder
{
    public static bool LooksLikeSubmodel(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.Object
               && element.TryGetProperty("id", out _)
               && element.TryGetProperty("idShort", out _)
               && element.TryGetProperty("submodelElements", out var sme)
               && sme.ValueKind == JsonValueKind.Array;
    }
}