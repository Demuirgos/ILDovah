// when used with base type TBase, the attribute is used to control generation order of the union parser
public enum Order
{
    First = 0,
    Middle = 1,
    Last = 2,
}
public class GenerationOrderParserAttribute<TBase> : System.Attribute
{
    public GenerationOrderParserAttribute(int Order)
    { }
}

public class GenerationOrderParserAttribute : System.Attribute
{
    public GenerationOrderParserAttribute(Order Order)
    { }
}
