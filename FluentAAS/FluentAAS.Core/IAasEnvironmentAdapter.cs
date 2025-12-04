namespace FluentAas.Core;

/// <summary>
/// Defines a contract for adapting or wrapping an AAS <see cref="Environment"/> instance,
/// allowing different environment implementations to be handled in a unified manner.
/// </summary>
public interface IAasEnvironmentAdapter
{
    /// <summary>
    /// Gets the underlying AAS <see cref="Environment"/> associated with this adapter.
    /// </summary>
    Environment Environment { get; }
}