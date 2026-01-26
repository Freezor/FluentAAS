namespace FluentAAS.Templates.HandoverDocumentation;

/// <summary>
/// Constants and enumerations for the IDTA 02004-2-0 "Handover Documentation" submodel template.
/// 
/// All semantic IDs and idShort values MUST be aligned with the official PDF:
/// IDTA 02004-2-0_Submodel_Handover Documentation.
/// </summary>
public static class HandoverDocumentationSemantics
{
    /// <summary>
    /// Submodel semantic ID for "Handover Documentation" (IDTA 02004-2-0).
    /// </summary>
    public const string SubmodelSemanticId = "0173-1#01-AHF578#003";

    // === Root element idShorts =================================================

    public const string IdShortDocuments = "Documents";
    public const string IdShortHandoverInformation = "HandoverInformation";

    // === Document-level idShorts ===============================================

    public const string IdShortDocumentCollection = "Document"; // each instance
    public const string IdShortDocumentId = "DocumentId";
    public const string IdShortDocumentTitle = "Title";
    public const string IdShortDocumentDescription = "Description";
    public const string IdShortDocumentLifecycleStage = "LifecycleStage";
    public const string IdShortDocumentClass = "DocumentClass";
    public const string IdShortDocumentFormat = "DocumentFormat";
    public const string IdShortDocumentLanguage = "Language";
    public const string IdShortDocumentRevision = "Revision";
    public const string IdShortDocumentDate = "Date";
    public const string IdShortDocumentFile = "File";

    // === Semantic IDs (examples; fill from template PDF) =======================

    public const string SemanticIdDocuments =
        "0173-1#02-ABI500#003";
    public const string SemanticIdDocument =
        "0173-1#02-ABI501#003";
    public const string SemanticIdDocumentId =
        "0173-1#02-ABI501#003/0173-1#01-AHF580#003";
    public const string SemanticIdDocumentTitle =
        "0173-1#02-ABG940#003";
    public const string SemanticIdDocumentDescription =
        "0173-1#02-AAN466#004";
    public const string SemanticIdDocumentClass =
        "0173-1#02-ABI502#003/0173-1#01-AHF581#003";
    public const string SemanticIdDocumentLanguage =
        "0173-1#02-ABI500#003";

    // === Enums =================================================================

    /// <summary>
    /// Lifecycle stages from the template (examples – adjust to spec).
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

    /// <summary>
    /// Document classes/categories from the template (examples – adjust to spec).
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
    /// Document formats from the template (examples – adjust to spec).
    /// Usually captured as an enumeration of controlled vocab items.
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
}