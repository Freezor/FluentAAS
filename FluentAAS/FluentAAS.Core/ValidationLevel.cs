namespace FluentAas.Core;

/// <summary>
/// Specifies the severity level of a validation result produced by
/// an AAS validation process.
/// </summary>
public enum ValidationLevel
{
    /// <summary>
    /// Indicates informational feedback that does not represent a problem.
    /// </summary>
    Info,

    /// <summary>
    /// Indicates a non-critical issue that may require attention,
    /// but does not prevent processing.
    /// </summary>
    Warning,

    /// <summary>
    /// Indicates a critical validation failure that prevents correct
    /// interpretation or processing of the AAS model.
    /// </summary>
    Error
}