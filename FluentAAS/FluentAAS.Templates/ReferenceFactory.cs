namespace FluentAAS.Templates;

/// <summary>
/// Provides helper methods for constructing commonly used AAS <see cref="IReference"/> instances,
/// particularly references to globally defined Concept Descriptions.
/// </summary>
public static class ReferenceFactory
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

    /// <summary>
    /// Creates an external reference to a globally defined semantic identifier, 
    /// typically used for linking AAS elements to their semantic definitions in standardized vocabularies.
    /// </summary>
    /// <param name="semanticId">
    /// The semantic identifier string, which can be an IRDI (International Registration Data Identifier),
    /// IRI (Internationalized Resource Identifier), or other globally unique identifier that defines
    /// the semantic meaning of an AAS element.
    /// </param>
    /// <returns>
    /// A <see cref="Reference"/> of type <see cref="ReferenceTypes.ExternalReference"/> containing
    /// a single key of type <see cref="KeyTypes.GlobalReference"/> that points to the specified
    /// semantic identifier.
    /// </returns>
    /// <remarks>
    /// This method is commonly used when assigning semantic IDs to AAS elements such as Properties,
    /// SubmodelElementCollections, and Files. The resulting reference establishes a link between
    /// the AAS element and its semantic definition in external vocabularies or concept descriptions.
    /// <para>
    /// Example semantic identifiers include:
    /// <list type="bullet">
    /// <item><description>IRDI format: "0112/2///61987#ABA565#009"</description></item>
    /// <item><description>IRI format: "https://admin-shell.io/idta/SubmodelTemplate/DigitalNameplate/3/0"</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="semanticId"/> is null, empty, or contains only whitespace characters.
    /// </exception>
    public static Reference GlobalSemanticReference(string semanticId)
    {
        return new Reference(ReferenceTypes.ExternalReference, [new Key(KeyTypes.GlobalReference, semanticId)]);
    }
}