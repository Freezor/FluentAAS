namespace FluentAas.ModelGeneration;

internal static class LoggingMarker
{
    private static int _generatedClasses;

    public static readonly List<string> FailedFiles  = new();
    public static readonly  List<string> SkippedFiles = new();
    public static void MarkSkipped(string jsonFile, string publishedRoot, string reason)
    {
        var rel = Path.GetRelativePath(publishedRoot, jsonFile);
        SkippedFiles.Add(rel);
        Console.WriteLine($"  -> SKIPPED: {rel} | Reason: {reason}");
        Console.WriteLine();
    }

    public static void IncreaseGeneratedClassesCount()
    {
        _generatedClasses++;
    
    }

    public static int GetAmountOfGeneratedFiles()
    {
        return _generatedClasses;
    }
}