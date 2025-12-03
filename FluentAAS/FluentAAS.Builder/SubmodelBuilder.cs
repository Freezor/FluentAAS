using AasCore.Aas3_0;

namespace FluentAas.Builder;

public sealed class SubmodelBuilder
{
    private readonly string                 _id;
    private readonly List<ISubmodelElement> _elements = new();

    private SubmodelBuilder(string id)
    {
        _id = id;
    }

    public static SubmodelBuilder Create(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Submodel id must not be empty", nameof(id));

        return new SubmodelBuilder(id);
    }

    public SubmodelBuilder AddProperty(
        string         idShort,
        string         value,
        DataTypeDefXsd type = DataTypeDefXsd.String)
    {
        if (string.IsNullOrWhiteSpace(idShort))
            throw new ArgumentException("idShort must not be empty", nameof(idShort));

        var prop = new Property(
                                idShort: idShort,
                                valueType: type,
                                value: value
                               );

        _elements.Add(prop);
        return this;
    }

    public SubmodelBuilder AddElement(ISubmodelElement element)
    {
        _elements.Add(element ?? throw new ArgumentNullException(nameof(element)));
        return this;
    }

    public Submodel Build()
    {
        return new Submodel(
                            id: _id,
                            submodelElements: _elements
                           );
    }
}