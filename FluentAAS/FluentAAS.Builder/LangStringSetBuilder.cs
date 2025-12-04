namespace FluentAAS.Builder;

/// <summary>
/// Provides a fluent builder for constructing an AAS <see cref="MultiLanguageProperty"/>
/// containing a set of localized text values.
/// </summary>
public sealed class LangStringSetBuilder
{
    private readonly MultiLanguageProperty _multiLanguageProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LangStringSetBuilder"/> class.
    /// </summary>
    /// <param name="idShort">The short identifier of the multi-language property.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="idShort"/> is null, empty, or whitespace.
    /// </exception>
    internal LangStringSetBuilder(string idShort)
    {
        if (string.IsNullOrWhiteSpace(idShort))
            throw new ArgumentException("idShort must not be empty.", nameof(idShort));

        _multiLanguageProperty = new MultiLanguageProperty
                                 {
                                     IdShort = idShort,
                                     Value   = []
                                 };
    }

    /// <summary>
    /// Adds a language-text pair to the multi-language property.
    /// </summary>
    /// <param name="language">The ISO language code (e.g., <c>"en"</c>, <c>"de"</c>).</param>
    /// <param name="text">The localized text value.</param>
    /// <returns>
    /// The current <see cref="LangStringSetBuilder"/> for fluent chaining.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="language"/> or <paramref name="text"/> is null or empty.
    /// </exception>
    public LangStringSetBuilder Add(string language, string text)
    {
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language code must not be empty.", nameof(language));

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text must not be empty.", nameof(text));

        _multiLanguageProperty.Value?.Add(
                                          new LangStringTextType(
                                                                 language: language,
                                                                 text: text));

        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="MultiLanguageProperty"/> instance.
    /// </summary>
    /// <returns>A fully constructed <see cref="MultiLanguageProperty"/>.</returns>
    public MultiLanguageProperty Build() => _multiLanguageProperty;
}