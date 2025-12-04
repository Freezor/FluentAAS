namespace FluentAAS.Builder.SubModel;

/// <summary>
/// Provides a fluent API for constructing and configuring a <see cref="Submodel"/>
/// that is directly associated with a parent <see cref="ShellBuilder"/>.
/// </summary>
public sealed class SubmodelBuilderWithShell
{
    private readonly ShellBuilder           _parentShell;
    private readonly Submodel               _submodel;
    private readonly List<ISubmodelElement> _elements = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmodelBuilderWithShell"/> class.
    /// </summary>
    /// <param name="parentShell">The parent <see cref="ShellBuilder"/> to which the submodel will be attached.</param>
    /// <param name="id">The identifier of the submodel.</param>
    /// <param name="idShort">The short identifier of the submodel.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="parentShell"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> or <paramref name="idShort"/> is null, empty, or whitespace.
    /// </exception>
    internal SubmodelBuilderWithShell(ShellBuilder parentShell, string id, string idShort)
    {
        _parentShell = parentShell ?? throw new ArgumentNullException(nameof(parentShell));

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(idShort))
        {
            throw new ArgumentException("Submodel idShort must not be empty.", nameof(idShort));
        }

        _submodel = new Submodel(id: id)
                    {
                        IdShort          = idShort,
                        SubmodelElements = _elements
                    };
    }

    /// <summary>
    /// Sets the semantic identifier for the submodel.
    /// </summary>
    /// <param name="semanticId">The semantic reference of the submodel.</param>
    /// <returns>The current <see cref="SubmodelBuilderWithShell"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="semanticId"/> is <c>null</c>.</exception>
    public SubmodelBuilderWithShell WithSemanticId(IReference semanticId)
    {
        ArgumentNullException.ThrowIfNull(semanticId);
        _submodel.SemanticId = semanticId;
        return this;
    }

    /// <summary>
    /// Adds a simple <see cref="Property"/> element to the submodel.
    /// </summary>
    /// <param name="idShort">The short identifier of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="valueType">
    /// The data type of the property. Defaults to <see cref="DataTypeDefXsd.String"/>.
    /// </param>
    /// <returns>The current <see cref="SubmodelBuilderWithShell"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="idShort"/> or <paramref name="value"/> is null, empty, or whitespace.
    /// </exception>
    public SubmodelBuilderWithShell AddProperty(
        string         idShort,
        string         value,
        DataTypeDefXsd valueType = DataTypeDefXsd.String)
    {
        if (string.IsNullOrWhiteSpace(idShort))
        {
            throw new ArgumentException("Property idShort must not be empty.", nameof(idShort));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Property value must not be empty.", nameof(value));
        }

        var property = new Property(valueType: valueType)
                       {
                           IdShort = idShort,
                           Value   = value
                       };

        _elements.Add(property);
        return this;
    }

    /// <summary>
    /// Adds a <see cref="MultiLanguageProperty"/> element to the submodel,
    /// configured via a <see cref="LangStringSetBuilder"/>.
    /// </summary>
    /// <param name="idShort">The short identifier of the multi-language property.</param>
    /// <param name="configure">
    /// A configuration action that receives a <see cref="LangStringSetBuilder"/> used
    /// to add localized text values.
    /// </param>
    /// <returns>The current <see cref="SubmodelBuilderWithShell"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="idShort"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="configure"/> is <c>null</c>.
    /// </exception>
    public SubmodelBuilderWithShell AddMultiLanguageProperty(
        string                       idShort,
        Action<LangStringSetBuilder> configure)
    {
        if (string.IsNullOrWhiteSpace(idShort))
        {
            throw new ArgumentException("Multi-language property idShort must not be empty.", nameof(idShort));
        }

        ArgumentNullException.ThrowIfNull(configure);

        var langBuilder = new LangStringSetBuilder(idShort);
        configure(langBuilder);

        _elements.Add(langBuilder.Build());
        return this;
    }

    /// <summary>
    /// Adds an arbitrary <see cref="ISubmodelElement"/> instance to the submodel.
    /// </summary>
    /// <param name="element">The submodel element to add.</param>
    /// <returns>The current <see cref="SubmodelBuilderWithShell"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is <c>null</c>.</exception>
    public SubmodelBuilderWithShell AddElement(ISubmodelElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _elements.Add(element);
        return this;
    }

    /// <summary>
    /// Completes the submodel configuration, attaches it to the parent shell,
    /// and returns the parent <see cref="ShellBuilder"/>.
    /// </summary>
    /// <returns>The parent <see cref="ShellBuilder"/>.</returns>
    public ShellBuilder Done()
    {
        _parentShell.AddSubmodelReference(_submodel);
        return _parentShell;
    }
    
    /// <summary>
    /// Convenience wrapper to add a simple Property submodel element.
    /// </summary>
    public SubmodelBuilderWithShell AddElement(string idShort, string value)
        => AddProperty(idShort, value);

    /// <summary>
    /// Builds and returns the configured <see cref="Submodel"/> instance
    /// without attaching it to the parent shell.
    /// </summary>
    /// <returns>The constructed <see cref="Submodel"/>.</returns>
    internal Submodel BuildSubmodel() => _submodel;
}