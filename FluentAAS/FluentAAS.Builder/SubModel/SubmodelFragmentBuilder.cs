namespace FluentAAS.Builder.SubModel;

/// <summary>
/// Represents a staged contribution to an existing <see cref="Submodel"/>.
/// Instances are created by <see cref="AasBuilder.AddSubmodelFragment"/> and applied at build-time.
/// </summary>
public sealed class SubmodelFragmentBuilder
{
    private readonly Submodel _target;

    /// <summary>
    /// Initializes a SubmodelFragmentBuilder for the specified submodel and ensures its SubmodelElements collection is initialized.
    /// </summary>
    /// <param name="target">The submodel to which staged elements will be appended.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
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
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="idShort"/> or <paramref name="value"/> is null, empty, or whitespace.
    /// <summary>
    /// Adds a Property with the specified idShort, value, and data type to the target Submodel.
    /// </summary>
    /// <param name="idShort">Short identifier for the Property; must not be null, empty, or whitespace.</param>
    /// <param name="value">Value for the Property; must not be null, empty, or whitespace.</param>
    /// <param name="valueType">Data type for the Property value. Defaults to <see cref="DataTypeDefXsd.String"/>.</param>
    /// <returns>The current <see cref="SubmodelFragmentBuilder"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="idShort"/> is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, or whitespace.</exception>
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
    /// <exception cref="ArgumentException">Thrown when <paramref name="idShort"/> is null, empty, or whitespace.</exception>
    /// <summary>
    /// Adds a multilingual (language-string) property to the target submodel and stages it for later build operations.
    /// </summary>
    /// <param name="idShort">The short identifier for the property; must not be null, empty, or whitespace.</param>
    /// <param name="configure">A configuration callback that populates the <see cref="LangStringSetBuilder"/> with language entries.</param>
    /// <returns>The current <see cref="SubmodelFragmentBuilder"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="idShort"/> is null, empty, or consists only of whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
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
    /// <summary>
    /// Appends a submodel element to the target submodel and returns this builder for chaining.
    /// </summary>
    /// <param name="element">The submodel element to add to the target submodel.</param>
    /// <returns>The current <see cref="SubmodelFragmentBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
    public SubmodelFragmentBuilder AddElement(ISubmodelElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _target.SubmodelElements!.Add(element);
        return this;
    }
}
