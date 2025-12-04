using FluentAAS.IO;

namespace FluentAas.IO;

public static class EnvironmentExtensions
{
    /// <summary>
    /// Serializes this AAS <see cref="Environment"/> to JSON and stores it in an .aasx package.
    /// </summary>
    /// <param name="environment">The AAS environment to package.</param>
    /// <param name="outputPath">The file system path where the .aasx package shall be written.</param>
    /// <param name="specificationUri"></param>
    public static void ToAasx(this Environment environment, string outputPath, string specificationUri)
    {
        ArgumentNullException.ThrowIfNull(environment);

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path must not be null or empty.", nameof(outputPath));
        }

        // 1) Convert the environment to JSON using the existing serializer
        var json = AasJsonSerializer.ToJson(environment);

        // 2) Use the existing converter to package the JSON into an .aasx
        AasJsonToAasxConverter.Convert(json, outputPath, specificationUri);
    }
}
