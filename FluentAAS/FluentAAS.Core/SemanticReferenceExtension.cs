using Aas = AasCore.Aas3_0;

namespace FluentAAS.Core;

public static class SemanticReferenceExtension
{
    public static Aas.IReference ToSemanticReference(this string semanticId)
    {
        if (string.IsNullOrWhiteSpace(semanticId))
            throw new ArgumentException("semanticId must not be null/empty", nameof(semanticId));

        return new Aas.Reference(
                                 type: Aas.ReferenceTypes.ExternalReference,
                                 keys: [new Aas.Key(Aas.KeyTypes.GlobalReference, semanticId)]
                                );
    }
}