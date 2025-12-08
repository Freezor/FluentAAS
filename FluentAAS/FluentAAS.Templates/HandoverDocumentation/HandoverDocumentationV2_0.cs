namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
/// Strongly typed wrapper for the IDTA 02004-2-0 "Handover Documentation" submodel.
/// </summary>
public sealed class HandoverDocumentationV20
{
    /// <summary>
    /// Underlying AAS submodel instance.
    /// </summary>
    public Aas.Submodel Submodel { get; }

    internal HandoverDocumentationV20(Aas.Submodel submodel)
    {
        Submodel = submodel;
    }
}
