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
    public const string SubmodelSemanticId =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation"; // TODO: verify with spec

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
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Documents";
    public const string SemanticIdDocument =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Document";
    public const string SemanticIdDocumentId =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:DocumentId";
    public const string SemanticIdDocumentTitle =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Title";
    public const string SemanticIdDocumentDescription =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Description";
    public const string SemanticIdDocumentLifecycleStage =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:LifecycleStage";
    public const string SemanticIdDocumentClass =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:DocumentClass";
    public const string SemanticIdDocumentFormat =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:DocumentFormat";
    public const string SemanticIdDocumentLanguage =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Language";
    public const string SemanticIdDocumentRevision =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Revision";
    public const string SemanticIdDocumentDate =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Date";
    public const string SemanticIdDocumentFile =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:File";

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