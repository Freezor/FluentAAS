using System.Text.Json;
using System.Text.Json.Nodes;

namespace FluentAas.IO;

/// <summary>
/// Provides JSON serialization and deserialization utilities for Asset Administration Shell (AAS)
/// <see cref="Environment"/> instances using the official AAS Jsonization helpers.
/// </summary>
public static class AasJsonSerializer
{
    // Only used when converting JsonNode -> string
    private static readonly JsonSerializerOptions JsonWriterOptions = new()
    {
        WriteIndented = false
    };

    /// <summary>
    /// Serializes an <see cref="Aas.Environment"/> into a JSON string.
    /// </summary>
    /// <param name="environment">The AAS environment to serialize.</param>
    /// <returns>A JSON representation of the AAS environment.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="environment"/> is <c>null</c>.
    /// </exception>
    public static string ToJson(Environment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        // Use the SDK's Jsonization to get a JsonObject with correct modelType, etc.
        var jsonObject = AasJsonization.Serialize.ToJsonObject(environment);

        // Convert the JsonObject to a string
        return jsonObject.ToJsonString(JsonWriterOptions);
    }

    /// <summary>
    /// Deserializes a JSON string into an <see cref="Aas.Environment"/>.
    /// </summary>
    /// <param name="json">The JSON string representing an AAS environment.</param>
    /// <returns>An <see cref="Aas.Environment"/> instance.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="json"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when deserialization fails or results in an invalid environment instance.
    /// </exception>
    public static Environment FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON must not be empty.", nameof(json));
        }

        JsonNode jsonNode;
        try
        {
            jsonNode = JsonNode.Parse(json)
                       ?? throw new InvalidOperationException("Failed to parse JSON into a JsonNode.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Input is not valid JSON.", ex);
        }

        try
        {
            // Let the AAS SDK do the polymorphic deserialization
            return AasJsonization.Deserialize.EnvironmentFrom(jsonNode);
        }
        catch (AasJsonization.Exception ex)
        {
            throw new InvalidOperationException("Failed to deserialize AAS environment from JSON.", ex);
        }
    }
}
