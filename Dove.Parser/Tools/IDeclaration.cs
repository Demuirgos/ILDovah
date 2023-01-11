using static Core;
public interface IDeclaration<in T>
{
    static Parser<T> AsParser
    {
        get
        {
            System.Type current = null;
            Parser<T> defaultP = null;
            do
            {
                current = current is null ? typeof(T) : current.BaseType;
                defaultP = (Parser<T>)current.GetProperty("AsParser")?.GetValue(null);

            } while (defaultP is null);
            return defaultP;
        }
    }
    static bool Parse(ref int index, string source, out T val)
    {
        if (AsParser(source, ref index, out val))
        {
            return true;
        }
        return false;
    }

}