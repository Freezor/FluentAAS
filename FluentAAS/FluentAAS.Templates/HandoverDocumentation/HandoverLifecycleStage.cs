namespace FluentAAS.Templates.HandoverDocumentation;

public static partial class HandoverDocumentationSemantics
{
    /// <summary>
    ///     Lifecycle stages from the template (examples – adjust to spec).
    /// </summary>
    public enum HandoverLifecycleStage
    {
        Engineering,
        Manufacturing,
        Commissioning,
        Operation,
        Maintenance,
        Decommissioning
    }
}