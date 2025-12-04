namespace FluentAAS.Builder;

/// <summary>
/// Provides entry points for creating Asset Administration Shell (AAS)
/// environments using the fluent builder API.
/// </summary>
public static class AasFluent
{
    /// <summary>
    /// Creates a new <see cref="AasBuilder"/> instance that can be used
    /// to fluently construct an AAS environment.
    /// </summary>
    /// <returns>
    /// A new <see cref="AasBuilder"/> for configuring an AAS environment.
    /// </returns>
    public static AasBuilder CreateEnvironment() =>
        new AasBuilder();
}