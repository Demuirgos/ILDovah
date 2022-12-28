using static Core;
public interface IDeclaration<in T> {
    static Parser<T> AsParser => (Parser<T>)typeof(T).GetProperty("AsParser").GetValue(null);
    static bool Parse(ref int index, string source, out T val) {
        if(AsParser(source, ref index, out val)) {
            return true;
        }
        return false;
    }
        
}