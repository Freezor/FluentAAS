// src/FluentAas.ModelGeneration/Program.cs
//
// Code generator for IDTA Submodel JSON templates.
// - Scans ALL *.json files under "externals/submodel-templates/published" (recursively).
// - For every *.json file, generates a C# class PER SUBMODEL in src/FluentAas.Submodels/submodel.
// - Class & file naming:
//      <CleanIdShort>V<Version>_<Revision>
//      e.g. "DigitalNameplateV3_0"
//   where Version/Revision are taken from the submodel's "id" URL.
// - The generated class does NOT embed the JSON.
// - For all elements in the JSON, each submodel class has strongly-typed properties
//   based on modelType/valueType/cardinality.
// - Logging:
//      * shows every processed file
//      * shows total JSON files found
//      * shows SKIPPED and FAILED files with filenames and reasons
//
// Note: JSON files in the externals folder are READ-ONLY and not modified.

namespace FluentAas.ModelGeneration;

internal static class Program
{
    private const string PublishedFolderRelative = "externals/submodel-templates/published";
    private const string OutputFolderRelative    = "src/FluentAas.Submodels/submodel";
    private const string RootNamespace           = "FluentAas.Submodels";

    private static int _processedFiles;

    public static int Main(string[] args)
    {
        try
        {
            var repoRoot      = RepositoryPath.FindRepoRoot();
            var publishedRoot = Path.Combine(repoRoot, PublishedFolderRelative);
            var outputRoot    = Path.Combine(repoRoot, OutputFolderRelative);

            Directory.CreateDirectory(outputRoot);

            Console.WriteLine("=== FluentAas.ModelGeneration ===");
            Console.WriteLine($"Repo root:           {repoRoot}");
            Console.WriteLine($"Published templates: {publishedRoot}");
            Console.WriteLine($"Output folder:       {outputRoot}");
            Console.WriteLine();

            if (!Directory.Exists(publishedRoot))
            {
                Console.WriteLine("ERROR: Published root folder not found.");
                Console.WriteLine("       Expected at: " + publishedRoot);
                return 1;
            }

            // Find ALL JSON files (recursive)
            var jsonFiles = Directory.EnumerateFiles(publishedRoot, "*.json", SearchOption.AllDirectories).ToList();
            Console.WriteLine($"Found {jsonFiles.Count} JSON file(s) under 'published' (recursive).");
            Console.WriteLine();

            if (jsonFiles.Count == 0)
            {
                Console.WriteLine("No JSON templates found. Nothing to do.");
                return 0;
            }

            foreach (var jsonFile in jsonFiles)
            {
                _processedFiles++;
                var relativePath = Path.GetRelativePath(publishedRoot, jsonFile);
                Console.WriteLine($"[{_processedFiles}/{jsonFiles.Count}] Processing: {relativePath}");

                try
                {
                    JsonFileExtraction.ProcessJsonFile(RootNamespace, jsonFile, publishedRoot, outputRoot);
                }
                catch (Exception ex)
                {
                    LoggingMarker.FailedFiles.Add(relativePath);
                    Console.WriteLine("  !! ERROR while processing this file:");
                    Console.WriteLine("     " + ex.GetType().Name + ": " + ex.Message);
                    Console.WriteLine();
                }
            }

            Console.WriteLine("=== Generation finished ===");
            Console.WriteLine($"Total JSON files found: {jsonFiles.Count}");
            Console.WriteLine($"Total classes generated: {LoggingMarker.GetAmountOfGeneratedFiles()}");
            Console.WriteLine($"Skipped files:          {LoggingMarker.SkippedFiles.Count}");
            Console.WriteLine($"Failed files:           {LoggingMarker.FailedFiles.Count}");
            Console.WriteLine();

            if (LoggingMarker.SkippedFiles.Count > 0)
            {
                Console.WriteLine("Skipped files:");
                foreach (var f in LoggingMarker.SkippedFiles)
                    Console.WriteLine("  - " + f);
                Console.WriteLine();
            }

            if (LoggingMarker.FailedFiles.Count > 0)
            {
                Console.WriteLine("Failed files:");
                foreach (var f in LoggingMarker.FailedFiles)
                    Console.WriteLine("  - " + f);
                Console.WriteLine();
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("FATAL ERROR: " + ex);
            return 1;
        }
    }
}