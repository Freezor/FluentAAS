#region

    using FluentAAS.Core;

#endregion

    namespace FluentAAS.Templates.HandoverDocumentation.Document;

    /// <summary>
    ///     Represents a single document entry in the Handover Documentation submodel.
    /// </summary>
    public sealed class HandoverDocument
    {
        /// <summary>
        /// Gets the collection of document identifiers for this handover document.
        /// </summary>
        public List<HandoverDocumentId>             DocumentIds             { get; } = [];
        
        /// <summary>
        /// Gets the collection of document classifications for this handover document.
        /// </summary>
        public List<HandoverDocumentClassification> DocumentClassifications { get; } = [];
        
        /// <summary>
        /// Gets the collection of document versions for this handover document.
        /// </summary>
        public List<HandoverDocumentVersion>        DocumentVersions        { get; } = [];

        // Optional: DocumentedEntities (not implemented here as typed items; keep future extension point)
        // public List<ReferenceElement> DocumentedEntities { get; } = new();

        /// <summary>
        /// Converts this handover document to a SubmodelElementCollection that represents the document structure in the AAS model.
        /// </summary>
        /// <returns>A SubmodelElementCollection containing the document's structure and data.</returns>
        internal SubmodelElementCollection ToDocumentCollection()
        {
            var children = new List<ISubmodelElement>
                           {
                               ToDocumentIdsList(),
                               ToDocumentClassificationsList(),
                               ToDocumentVersionsList()
                           };

            return new SubmodelElementCollection(
                                                 idShort: "Document", // Note: list element idShort is "Document" in many templates; keep stable.
                                                 category: null,
                                                 description: null,
                                                 semanticId: Ref(HandoverDocumentationSemantics.SemanticIdDocument),
                                                 value: [..children.ToArray()]
                                                );
        }

        /// <summary>
        /// Validates this handover document against template requirements and throws exceptions for any violations.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when mandatory fields are missing or invalid.</exception>
        internal void ValidateTemplateRequirements()
        {
            if (DocumentIds.Count < 1)
                throw new InvalidOperationException("Each Document must contain at least one DocumentId (DocumentIds list).");

            if (DocumentClassifications.Count < 1)
                throw new InvalidOperationException("Each Document must contain at least one DocumentClassification (DocumentClassifications list).");

            if (DocumentVersions.Count < 1)
                throw new InvalidOperationException("Each Document must contain at least one DocumentVersion (DocumentVersions list).");

            // Enforce: VDI 2770 Blatt 1:2020 classification is mandatory
            if (!DocumentClassifications.Any(c => string.Equals(c.ClassificationSystem, HandoverDocumentationSemantics.Vdi2770ClassificationSystemName, StringComparison.Ordinal)))
                throw new InvalidOperationException(
                                                    $"Each Document must include a classification with ClassificationSystem='{HandoverDocumentationSemantics.Vdi2770ClassificationSystemName}'.");

            foreach (var v in DocumentVersions)
                v.ValidateTemplateRequirements();
        }

        /// <summary>
        /// Creates a SubmodelElementList containing all document IDs for this handover document.
        /// </summary>
        /// <returns>A SubmodelElementList representing the document IDs collection.</returns>
        private SubmodelElementList ToDocumentIdsList()
        {
            return NewList(
                           HandoverDocumentationSemantics.IdShortDocumentIds,
                           HandoverDocumentationSemantics.SemanticIdDocumentIds,
                           HandoverDocumentationSemantics.SemanticIdDocumentId,
                           DocumentIds.Select(d => d.ToCollection()).Cast<ISubmodelElement>().ToList(),
                           false,
                           AasSubmodelElements.SubmodelElementCollection
                          );
        }

        /// <summary>
        /// Creates a SubmodelElementList containing all document classifications for this handover document.
        /// </summary>
        /// <returns>A SubmodelElementList representing the document classifications collection.</returns>
        private SubmodelElementList ToDocumentClassificationsList()
        {
            return NewList(
                           HandoverDocumentationSemantics.IdShortDocumentClassifications,
                           HandoverDocumentationSemantics.SemanticIdDocumentClassifications,
                           HandoverDocumentationSemantics.SemanticIdDocumentClassification,
                           DocumentClassifications.Select(c => c.ToCollection()).Cast<ISubmodelElement>().ToList(),
                           false,
                           AasSubmodelElements.SubmodelElementCollection
                          );
        }

        /// <summary>
        /// Creates a SubmodelElementList containing all document versions for this handover document.
        /// </summary>
        /// <returns>A SubmodelElementList representing the document versions collection.</returns>
        private SubmodelElementList ToDocumentVersionsList()
        {
            return NewList(
                           HandoverDocumentationSemantics.IdShortDocumentVersions,
                           HandoverDocumentationSemantics.SemanticIdDocumentVersions,
                           HandoverDocumentationSemantics.SemanticIdDocumentVersion,
                           DocumentVersions.Select(v => v.ToCollection()).Cast<ISubmodelElement>().ToList(),
                           false,
                           AasSubmodelElements.SubmodelElementCollection
                          );
        }

        // Helpers
        /// <summary>
        /// Creates a Reference object from a semantic ID string.
        /// </summary>
        /// <param name="semanticId">The semantic ID string to convert to a Reference.</param>
        /// <returns>A Reference object containing the semantic ID.</returns>
        private static Reference Ref(string semanticId)
        {
            return new Reference(ReferenceTypes.ExternalReference, [new Key(KeyTypes.GlobalReference, semanticId)]);
        }

        /// <summary>
        /// Creates a new SubmodelElementList with the specified configuration and elements.
        /// </summary>
        /// <param name="idShort">The short identifier for the list.</param>
        /// <param name="semanticId">The semantic ID for the list.</param>
        /// <param name="listElementSemanticId">The semantic ID for individual elements in the list.</param>
        /// <param name="value">The collection of submodel elements to include in the list.</param>
        /// <param name="orderRelevant">Whether the order of elements in the list is relevant.</param>
        /// <param name="typeValueListElement">The type of elements that the list can contain.</param>
        /// <returns>A configured SubmodelElementList containing the specified elements.</returns>
        private static SubmodelElementList NewList(
            string                 idShort,
            string                 semanticId,
            string                 listElementSemanticId,
            List<ISubmodelElement> value,
            bool                   orderRelevant,
            AasSubmodelElements    typeValueListElement)
        {
            var list = new SubmodelElementList(
                                               AasSubmodelElements.SubmodelElement,
                                               idShort: idShort,
                                               category: null,
                                               description: null,
                                               semanticId: semanticId.ToSemanticReference(),
                                               value: value
                                              )
                       {
                           OrderRelevant         = orderRelevant,
                           SemanticIdListElement = Ref(listElementSemanticId),
                           TypeValueListElement  = typeValueListElement
                       };

            return list;
        }
    }