namespace FluentAas.Core;

public sealed class DefaultAasEnvironmentAdapter : IAasEnvironmentAdapter
{
    public Environment Environment { get; }

    public DefaultAasEnvironmentAdapter(Environment environment)
    {
        Environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }
}