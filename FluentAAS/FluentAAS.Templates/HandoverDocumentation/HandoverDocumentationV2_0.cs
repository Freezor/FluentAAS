namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
///     Strongly typed wrapper for the IDTA 02004-2-0 "Handover Documentation" submodel.
/// </summary>
public sealed class HandoverDocumentationV20
{
    internal HandoverDocumentationV20(Submodel submodel)
    {
        Submodel = submodel;
    }

    /// <summary>
    ///     Underlying AAS submodel instance.
    /// </summary>
    public Submodel Submodel { get; }
}