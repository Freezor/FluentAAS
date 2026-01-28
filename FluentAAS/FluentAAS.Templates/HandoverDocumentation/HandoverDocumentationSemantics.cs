namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
///     Constants and enumerations for the IDTA 02004-2-0 "Handover Documentation" submodel template.
///     All semantic IDs and idShort values MUST be aligned with the official PDF:
///     IDTA 02004-2-0_Submodel_Handover Documentation.
/// </summary>
public static class HandoverDocumentationSemantics
{
    /// <summary>
    ///     Document classes/categories from the template (examples – adjust to spec).
    /// </summary>
    public enum HandoverDocumentClass
    {
        OperatingManual,
        MaintenanceManual,
        SafetyDocument,
        Certificate,
        Datasheet,
        Drawing,
        Other
    }

    /// <summary>
    ///     Document formats from the template (examples – adjust to spec).
    ///     Usually captured as an enumeration of controlled vocab items.
    /// </summary>
    public enum HandoverDocumentFormat
    {
        Pdf,
        Docx,
        Xlsx,
        Image,
        Video,
        Xml,
        Json,
        Other
    }
    // === Enums =================================================================

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

    // -------------------------
    // Submodel
    // -------------------------
    public const string SubmodelIdShort    = "HandoverDocumentation";
    public const string SubmodelSemanticId = "0173-1#01-AHF578#003";

    // -------------------------
    // Root elements (SubmodelElements)
    // -------------------------
    public const string IdShortDocuments    = "Documents";
    public const string SemanticIdDocuments = "0173-1#02-ABI500#003";

    // Optional root list (0..1)
    public const string IdShortEntities    = "Entities";
    public const string SemanticIdEntities = "https://admin-shell.io/vdi/2770/1/0/EntitiesForDocumentation";

    // Documents list element (Document SMC) semantic id (Table 3)
    public const string SemanticIdDocument = "0173-1#02-ABI500#003/0173-1#01-AHF579#003";

    // -------------------------
    // Document (SMC) children
    // -------------------------
    public const string IdShortDocumentIds    = "DocumentIds";
    public const string SemanticIdDocumentIds = "0173-1#02-ABI501#003";
    public const string SemanticIdDocumentId  = "0173-1#02-ABI501#003/0173-1#01-AHF580#003";

    public const string IdShortDocumentClassifications    = "DocumentClassifications";
    public const string SemanticIdDocumentClassifications = "0173-1#02-ABI502#003";
    public const string SemanticIdDocumentClassification  = "0173-1#02-ABI502#003/0173-1#01-AHF581#003";

    public const string IdShortDocumentVersions    = "DocumentVersions";
    public const string SemanticIdDocumentVersions = "0173-1#02-ABI503#003";
    public const string SemanticIdDocumentVersion  = "0173-1#02-ABI503#003/0173-1#01-AHF582#003";

    public const string IdShortDocumentedEntities    = "DocumentedEntities";
    public const string SemanticIdDocumentedEntities = "0173-1#02-ABI504#003";
    public const string SemanticIdDocumentedEntity   = "0173-1#02-ABI504#003/0173-1#01-AHF583#003";

    // -------------------------
    // DocumentId (SMC) fields (Table 6)
    // -------------------------
    public const string IdShortDocumentDomainId    = "DocumentDomainId";
    public const string SemanticIdDocumentDomainId = "0173-1#02-ABH994#003";

    public const string IdShortDocumentIdentifier    = "DocumentIdentifier";
    public const string SemanticIdDocumentIdentifier = "0173-1#02-AAO099#004";

    public const string IdShortDocumentIsPrimary    = "DocumentIsPrimary";
    public const string SemanticIdDocumentIsPrimary = "0173-1#02-ABH995#003";

    // -------------------------
    // DocumentClassification (SMC) fields (Table 7)
    // -------------------------
    public const string IdShortClassId    = "ClassId";
    public const string SemanticIdClassId = "0173-1#02-AAO107#005";

    public const string IdShortClassName    = "ClassName";
    public const string SemanticIdClassName = "0173-1#02-AAO108#005";

    public const string IdShortClassificationSystem    = "ClassificationSystem";
    public const string SemanticIdClassificationSystem = "0173-1#02-AAO109#005";

    // Mandatory classification system per template guidance
    public const string Vdi2770ClassificationSystemName = "VDI 2770 Blatt 1:2020";

    // -------------------------
    // DocumentVersion (SMC) fields (Table 8)
    // -------------------------
    public const string IdShortLanguage    = "Language";
    public const string SemanticIdLanguage = "0173-1#02-AAN468#008";

    public const string IdShortVersion    = "Version";
    public const string SemanticIdVersion = "0173-1#02-AAP003#005";

    public const string IdShortTitle    = "Title";
    public const string SemanticIdTitle = "0173-1#02-AAO105#005";

    public const string IdShortSubtitle    = "Subtitle";
    public const string SemanticIdSubtitle = "0173-1#02-AAO106#005";

    public const string IdShortDescription    = "Description";
    public const string SemanticIdDescription = "0173-1#02-AAO111#005";

    public const string IdShortKeyWords    = "KeyWords";
    public const string SemanticIdKeyWords = "0173-1#02-AAO112#005";

    public const string IdShortStatusSetDate    = "StatusSetDate";
    public const string SemanticIdStatusSetDate = "0173-1#02-AAO113#005";

    public const string IdShortStatusValue    = "StatusValue";
    public const string SemanticIdStatusValue = "0173-1#02-AAO114#005";

    public const string IdShortOrganizationShortName    = "OrganizationShortName";
    public const string SemanticIdOrganizationShortName = "0173-1#02-AAO115#005";

    public const string IdShortOrganizationOfficialName    = "OrganizationOfficialName";
    public const string SemanticIdOrganizationOfficialName = "0173-1#02-AAO116#005";

    // -------------------------
    // Relationships (optional) inside DocumentVersion (Table 8)
    // -------------------------
    public const string IdShortRefersToEntities    = "RefersToEntities";
    public const string SemanticIdRefersToEntities = "0173-1#02-ABI505#003";

    public const string IdShortBasedOnReferences    = "BasedOnReferences";
    public const string SemanticIdBasedOnReferences = "0173-1#02-ABI506#003";

    public const string IdShortTranslationOfEntities    = "TranslationOfEntities";
    public const string SemanticIdTranslationOfEntities = "0173-1#02-ABI507#003";

    // -------------------------
    // Digital files
    // -------------------------
    public const string IdShortDigitalFiles    = "DigitalFiles";
    public const string SemanticIdDigitalFiles = "0173-1#02-ABK126#002";

    public const string IdShortPreviewFile    = "PreviewFile";
    public const string SemanticIdPreviewFile = "0173-1#02-ABK127#002";

    // Suggested status values
    public static class StatusValues
    {
        public const string InReview = "InReview";
        public const string Released = "Released";
    }
}