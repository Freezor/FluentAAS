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

    public const string IdShort_Documents = "Documents";
    public const string IdShort_HandoverInformation = "HandoverInformation";

    // === Document-level idShorts ===============================================

    public const string IdShort_DocumentCollection = "Document"; // each instance
    public const string IdShort_DocumentId = "DocumentId";
    public const string IdShort_DocumentTitle = "Title";
    public const string IdShort_DocumentDescription = "Description";
    public const string IdShort_DocumentLifecycleStage = "LifecycleStage";
    public const string IdShort_DocumentClass = "DocumentClass";
    public const string IdShort_DocumentFormat = "DocumentFormat";
    public const string IdShort_DocumentLanguage = "Language";
    public const string IdShort_DocumentRevision = "Revision";
    public const string IdShort_DocumentDate = "Date";
    public const string IdShort_DocumentFile = "File";

    // === Semantic IDs (examples; fill from template PDF) =======================

    public const string SemanticId_Documents =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Documents";
    public const string SemanticId_Document =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Document";
    public const string SemanticId_DocumentId =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:DocumentId";
    public const string SemanticId_DocumentTitle =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Title";
    public const string SemanticId_DocumentDescription =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Description";
    public const string SemanticId_DocumentLifecycleStage =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:LifecycleStage";
    public const string SemanticId_DocumentClass =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:DocumentClass";
    public const string SemanticId_DocumentFormat =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:DocumentFormat";
    public const string SemanticId_DocumentLanguage =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Language";
    public const string SemanticId_DocumentRevision =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Revision";
    public const string SemanticId_DocumentDate =
        "urn:idta:02004:2:0:submodel:HandoverDocumentation:Date";
    public const string SemanticId_DocumentFile =
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