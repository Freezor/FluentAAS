using FluentAAS.Builder;

namespace FluentAAS.Templates.TechnicalData;

public static class TechnicalDataShellBuilderExtensions
{
    public static TechnicalDataBuilder AddTechnicalData(this IShellBuilder shellBuilder, string id, string idShort = "TechnicalData")
    {
        ArgumentNullException.ThrowIfNull(shellBuilder);

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Submodel id must not be empty.", nameof(id));
        }

        return new TechnicalDataBuilder(shellBuilder, id, idShort);
    }
}
