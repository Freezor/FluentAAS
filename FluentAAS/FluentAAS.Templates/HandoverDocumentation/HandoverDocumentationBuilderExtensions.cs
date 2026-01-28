namespace FluentAAS.Templates.HandoverDocumentation;

public static class HandoverDocumentationBuilderExtensions
{
    /// <summary>
    ///     Add a Handover Documentation submodel using the fluent handover builder.
    ///     Example:
    ///     <code>
    /// aasBuilder
    ///     .AddHandoverDocumentation("urn:submodel:handover:123", sm => sm
    ///         .AddDocument(doc => doc
    ///             .WithTitle("Operating Manual")
    ///             .WithDocumentId("DOC-001")
    ///             .WithLifecycleStage(HandoverLifecycleStage.Operation)
    ///             .AddFile("manual.pdf", mimeType: "application/pdf")))
    ///     .BuildHandoverDocumentation();
    /// </code>
    /// </summary>
    public static HandoverDocumentationSubmodelBuilder AddHandoverDocumentation(string  submodelId,
                                                                                string? idShort = null)
    {
        return new HandoverDocumentationSubmodelBuilder(submodelId, idShort);
    }

    /// <summary>
    ///     Builds and adds the configured Handover Documentation submodel to the root builder.
    /// </summary>
    public static TRootBuilder BuildHandoverDocumentation<TRootBuilder>(
        this HandoverDocumentationSubmodelBuilder handoverBuilder,
        TRootBuilder rootBuilder)
        where TRootBuilder : ISubmodelCollector
    {
        ArgumentNullException.ThrowIfNull(handoverBuilder);
        if (rootBuilder is null) throw new ArgumentNullException(nameof(rootBuilder));

        var typedSubmodel = handoverBuilder.Build();
        rootBuilder.AddSubmodel(typedSubmodel.Submodel);

        return rootBuilder;
    }
}