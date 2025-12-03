namespace FluentAAS;

public class Class1
{
    public void init()
    {
        var element = new AasCore.Aas3_0.Property(AasCore.Aas3_0.DataTypeDefXsd.Int)
                      {
                          IdShort = "someElement",
                          Value   = "1984"
                      };
    }
}