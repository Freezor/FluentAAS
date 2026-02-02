using FluentAAS.Builder;

namespace FluentAAS.Templates.HandoverDocumentation;

public static class HandoverDocumentationBuilderExtensions
{
    /// <summary>
    ///     Add a Handover Documentation submodel using the fluent handover builder.
    ///     Example:
    ///     <code>
    /// aasBuilder
    ///     .AddHandoverDocumentation("urn:submodel:handover:123")
    ///     .AddDocument(doc => doc
    ///         .WithTitle("Operating Manual")
    ///         .WithDocumentId("DOC-001")
    ///         .WithLifecycleStage(HandoverLifecycleStage.Operation)
    ///         .AddFile("manual.pdf", mimeType: "application/pdf"))
    ///     .BuildHandoverDocumentation();
    /// </code>
    /// </summary>
    public static HandoverDocumentationSubmodelBuilder AddHandoverDocumentation(this IShellBuilder shellBuilder,
                                                                                string             submodelId,
                                                                                string?            idShort = "HandoverDocumentation")
    {
        ArgumentNullException.ThrowIfNull(shellBuilder);

        if (string.IsNullOrWhiteSpace(submodelId))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(submodelId));
        }

        return new HandoverDocumentationSubmodelBuilder(shellBuilder, submodelId, idShort);
    }
}