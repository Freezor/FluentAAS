using System.Text.Json;

namespace FluentAas.IO;

/// <summary>
/// Provides JSON serialization and deserialization utilities for Asset Administration Shell (AAS)
/// <see cref="Environment"/> instances using <see cref="System.Text.Json"/>.
/// </summary>
public static class AasJsonSerializer
{
    /// <summary>
    /// Shared serializer options used for all AAS JSON operations.
    /// Customize this with AAS-required converters when needed.
    /// </summary>
    private static readonly JsonSerializerOptions Options = new()
                                                            {
                                                                WriteIndented = true
                                                                // TODO: Register AAS-specific converters when applicable (e.g. converters from aas-core-dotnet).
                                                            };

    /// <summary>
    /// Serializes an <see cref="IAasEnvironmentAdapter"/> into a JSON string.
    /// </summary>
    /// <param name="env">The AAS environment adapter containing the <see cref="Environment"/> to serialize.</param>
    /// <returns>A formatted JSON representation of the AAS environment.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="env"/> is <c>null</c>.</exception>
    public static string ToJson(IAasEnvironmentAdapter env)
    {
        ArgumentNullException.ThrowIfNull(env);

        return JsonSerializer.Serialize(env.Environment, Options);
    }

    /// <summary>
    /// Deserializes a JSON string into an <see cref="IAasEnvironmentAdapter"/>.
    /// </summary>
    /// <param name="json">The JSON string representing an AAS <see cref="Environment"/>.</param>
    /// <returns>
    /// An <see cref="IAasEnvironmentAdapter"/> wrapping the deserialized <see cref="Environment"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="json"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when deserialization fails or results in a <c>null</c> environment instance.
    /// </exception>
    public static IAasEnvironmentAdapter FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON must not be empty.", nameof(json));
        }

        var environment = JsonSerializer.Deserialize<Environment>(json, Options)
                          ?? throw new InvalidOperationException("Failed to deserialize AAS environment.");

        return new DefaultAasEnvironmentAdapter(environment);
    }
}