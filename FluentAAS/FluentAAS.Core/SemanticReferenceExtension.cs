using Aas = AasCore.Aas3_0;

namespace FluentAAS.Core;

public static class SemanticReferenceExtension
{
    public static AasCore.Aas3_0.IReference ToSemanticReference(this string semanticId)
    {
        if (string.IsNullOrWhiteSpace(semanticId))
            throw new ArgumentException("semanticId must not be null/empty", nameof(semanticId));

        return new AasCore.Aas3_0.Reference(
                                            type: AasCore.Aas3_0.ReferenceTypes.ExternalReference,
                                            keys: new List<AasCore.Aas3_0.IKey>
                                                  {
                                                      new AasCore.Aas3_0.Key(AasCore.Aas3_0.KeyTypes.GlobalReference, semanticId)
                                                  }
                                           );
    }
}