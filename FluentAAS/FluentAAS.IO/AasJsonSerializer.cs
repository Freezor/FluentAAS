// FluentAas.IO/AasJsonSerializer.cs
using System.Text.Json;

namespace FluentAas.IO;

public static class AasJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
                                                            {
                                                                WriteIndented = true
                                                                // TODO: configure converters if AasCore requires them
                                                            };

    public static string ToJson(IAasEnvironmentAdapter env)
    {
        if (env is null)
            throw new ArgumentNullException(nameof(env));

        return JsonSerializer.Serialize(env.Environment, Options);
    }

    public static IAasEnvironmentAdapter FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON must not be empty", nameof(json));

        var environment = JsonSerializer.Deserialize<Environment>(json, Options)
                          ?? throw new InvalidOperationException("Failed to deserialize AAS Environment");

        return new DefaultAasEnvironmentAdapter(environment);
    }
}