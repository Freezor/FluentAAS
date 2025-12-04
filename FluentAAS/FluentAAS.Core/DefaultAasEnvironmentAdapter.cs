namespace FluentAas.Core;

/// <summary>
/// Default implementation of <see cref="IAasEnvironmentAdapter"/> that
/// wraps a concrete AAS <see cref="Environment"/> instance.
/// </summary>
public sealed class DefaultAasEnvironmentAdapter : IAasEnvironmentAdapter
{
    /// <summary>
    /// Gets the underlying AAS <see cref="Environment"/> represented by this adapter.
    /// </summary>
    public Environment Environment { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultAasEnvironmentAdapter"/> class.
    /// </summary>
    /// <param name="environment">
    /// The AAS <see cref="Environment"/> instance to be wrapped by this adapter.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="environment"/> is <c>null</c>.
    /// </exception>
    public DefaultAasEnvironmentAdapter(Environment environment)
    {
        Environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }
}