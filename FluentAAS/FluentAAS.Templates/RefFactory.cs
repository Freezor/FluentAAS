namespace FluentAAS.Templates;

/// <summary>
/// Provides helper methods for constructing commonly used AAS <see cref="IReference"/> instances,
/// particularly references to globally defined Concept Descriptions.
/// </summary>
public static class RefFactory
{
    /// <summary>
    /// Creates a reference to a globally defined Concept Description (CD),
    /// typically using an IRDI or IRI identifier.
    /// </summary>
    /// <param name="conceptId">The identifier of the concept description.</param>
    /// <returns>
    /// An <see cref="IReference"/> containing a single key of type
    /// <see cref="KeyTypes.ConceptDescription"/> pointing to the given <paramref name="conceptId"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="conceptId"/> is null, empty, or whitespace.
    /// </exception>
    public static IReference GlobalConceptDescription(string conceptId)
    {
        if (string.IsNullOrWhiteSpace(conceptId))
        {
            throw new ArgumentException("Concept ID must not be empty.", nameof(conceptId));
        }

        return new Reference(
                             type: ReferenceTypes.ExternalReference,
                             keys:
                             [
                                 new Key(
                                         type: KeyTypes.GlobalReference,
                                         value: conceptId)
                             ]);
    }
}