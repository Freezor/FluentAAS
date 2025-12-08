using System;
using FluentAAS.Templates.HandoverDocumentation;

namespace FluentAAS.Core.HandoverDocumentation;

public static class HandoverDocumentationBuilderExtensions
{
    /// <summary>
    /// Add a Handover Documentation submodel using the fluent handover builder.
    /// 
    /// Example:
    /// 
    /// <code>
    /// aasBuilder
    ///     .AddHandoverDocumentation("urn:submodel:handover:123", sm => sm
    ///         .AddDocument(doc => doc
    ///             .WithTitle("Operating Manual")
    ///             .WithDocumentId("DOC-001")
    ///             .WithLifecycleStage(HandoverLifecycleStage.Operation)
    ///             .AddFile("manual.pdf", mimeType: "application/pdf")))
    ///     .Build();
    /// </code>
    /// </summary>
    public static TRootBuilder AddHandoverDocumentation<TRootBuilder>(
        this TRootBuilder rootBuilder,
        string submodelId,
        Action<HandoverDocumentationSubmodelBuilder> configureSubmodel)
        where TRootBuilder : ISubmodelCollector
    {
        if (rootBuilder is null) throw new ArgumentNullException(nameof(rootBuilder));
        if (configureSubmodel is null) throw new ArgumentNullException(nameof(configureSubmodel));

        var smBuilder = new HandoverDocumentationSubmodelBuilder(submodelId);
        configureSubmodel(smBuilder);

        var typedSubmodel = smBuilder.Build();

        rootBuilder.AddSubmodel(typedSubmodel.Submodel);

        return rootBuilder;
    }
}

/// <summary>
/// Contract for root builders that can collect submodels.
/// Adjust to match your actual root builder abstraction.
/// </summary>
public interface ISubmodelCollector
{
    void AddSubmodel(AasCore.Aas3_0.Submodel submodel);
}
