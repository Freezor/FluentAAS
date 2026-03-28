using FluentAAS.Builder;

namespace FluentAAS.Templates.TechnicalData;

/// <summary>
/// Extension methods to start Technical Data composition directly from a shell builder.
/// This keeps Technical Data authoring aligned with existing FluentAAS composition workflows.
/// </summary>
public static class TechnicalDataShellBuilderExtensions
{
    /// <summary>
    /// Starts a <see cref="TechnicalDataBuilder"/> for the current shell.
    /// Callers get a focused fluent API that validates technical parameters before they are attached.
    /// </summary>
    /// <param name="shellBuilder">The shell that will receive the Technical Data submodel.</param>
    /// <param name="id">Unique identifier of the Technical Data submodel.</param>
    /// <param name="idShort">Short readable name of the Technical Data submodel.</param>
    /// <returns>A specialized technical data builder.</returns>
    public static TechnicalDataBuilder AddTechnicalData(this IShellBuilder shellBuilder, string id, string idShort = "TechnicalData")
    {
        ArgumentNullException.ThrowIfNull(shellBuilder);

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(id));
        }

        return new TechnicalDataBuilder(shellBuilder, id, idShort);
    }
}
