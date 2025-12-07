using FluentAAS.Builder;

namespace FluentAAS.Templates.DigitalNameplate;

/// <summary>
/// Provides extension methods for <see cref="ShellBuilder"/> to simplify
/// the creation of an IDTA Digital Nameplate submodel.
/// </summary>
public static class DigitalNameplateShellBuilderExtensions
{
    /// <summary>
    /// Starts the specialized <see cref="DigitalNameplateBuilder"/> for constructing
    /// a Digital Nameplate submodel attached to the given <see cref="ShellBuilder"/>.
    /// </summary>
    /// <param name="shellBuilder">The shell to which the digital nameplate will be added.</param>
    /// <param name="id">The identifier of the digital nameplate submodel.</param>
    /// <param name="idShort">The short identifier of the digital nameplate submodel. Defaults to <c>"DigitalNameplate"</c>.</param>
    /// <returns>A new <see cref="DigitalNameplateBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="shellBuilder"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> is null, empty, or whitespace.
    /// </exception>
    public static DigitalNameplateBuilder AddDigitalNameplate(
        this IShellBuilder shellBuilder,
        string            id,
        string            idShort = "DigitalNameplate")
    {
        ArgumentNullException.ThrowIfNull(shellBuilder);

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(id));
        }

        return new DigitalNameplateBuilder(shellBuilder, id, idShort);
    }
}