namespace FluentAAS.Builder;

/// <summary>
/// Provides entry points for creating Asset Administration Shell (AAS)
/// environments using the fluent builder API.
/// </summary>
public static class AasFluent
{
    /// <summary>
    /// Creates a new <see cref="EnvironmentBuilder"/> instance that can be used
    /// to fluently construct an AAS environment.
    /// </summary>
    /// <returns>
    /// A new <see cref="EnvironmentBuilder"/> for configuring an AAS environment.
    /// </returns>
    public static EnvironmentBuilder CreateEnvironment() =>
        new EnvironmentBuilder();
}