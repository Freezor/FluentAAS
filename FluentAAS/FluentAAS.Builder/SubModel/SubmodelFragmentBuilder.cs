namespace FluentAAS.Builder.SubModel;

/// <summary>
/// Represents a staged contribution to an existing <see cref="Submodel"/>.
/// Instances are created by <see cref="AasBuilder.AddSubmodelFragment"/> and applied at build-time.
/// </summary>
public sealed class SubmodelFragmentBuilder
{
    private readonly Submodel _target;

    internal SubmodelFragmentBuilder(Submodel target)
    {
        _target = target ?? throw new ArgumentNullException(nameof(target));
        _target.SubmodelElements ??= [];
    }

    /// <summary>
    /// Adds a <see cref="Property"/> to the target submodel fragment.
    /// </summary>
    /// <param name="idShort">The short identifier of the property.</param>
    /// <param name="value">The property value.</param>
    /// <param name="valueType">The property value type. Defaults to <see cref="DataTypeDefXsd.String"/>.</param>
    /// <returns>The current <see cref="SubmodelFragmentBuilder"/>.</returns>
    /// <remarks>Throws <see cref="ArgumentException"/> when <paramref name="idShort"/> or <paramref name="value"/> is null, empty, or whitespace.</remarks>
    public SubmodelFragmentBuilder AddProperty(string idShort, string value, DataTypeDefXsd valueType = DataTypeDefXsd.String)
    {
        if (string.IsNullOrWhiteSpace(idShort))
        {
            throw new ArgumentException("Property idShort must not be empty.", nameof(idShort));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Property value must not be empty.", nameof(value));
        }

        _target.SubmodelElements!.Add(
                                     new Property(valueType)
                                     {
                                         IdShort = idShort,
                                         Value = value
                                     });

        return this;
    }

    /// <summary>
    /// Adds a <see cref="MultiLanguageProperty"/> element to the target submodel.
    /// </summary>
    /// <param name="idShort">The short identifier of the multi-language property.</param>
    /// <param name="configure">Configuration callback for language strings.</param>
    /// <returns>The current <see cref="SubmodelFragmentBuilder"/>.</returns>
    /// <remarks>Throws <see cref="ArgumentException"/> when <paramref name="idShort"/> is invalid and <see cref="ArgumentNullException"/> when <paramref name="configure"/> is null.</remarks>
    public SubmodelFragmentBuilder AddMultiLanguageProperty(string idShort, Action<LangStringSetBuilder> configure)
    {
        if (string.IsNullOrWhiteSpace(idShort))
        {
            throw new ArgumentException("Multi-language property idShort must not be empty.", nameof(idShort));
        }

        ArgumentNullException.ThrowIfNull(configure);

        var langBuilder = new LangStringSetBuilder(idShort);
        configure(langBuilder);
        _target.SubmodelElements!.Add(langBuilder.Build());

        return this;
    }

    /// <summary>
    /// Adds an arbitrary <see cref="ISubmodelElement"/> instance to the target submodel.
    /// </summary>
    /// <param name="element">The submodel element to add.</param>
    /// <returns>The current <see cref="SubmodelFragmentBuilder"/>.</returns>
    /// <remarks>Throws <see cref="ArgumentNullException"/> when <paramref name="element"/> is null.</remarks>
    public SubmodelFragmentBuilder AddElement(ISubmodelElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _target.SubmodelElements!.Add(element);
        return this;
    }
}
