using static Core;
public interface IDeclaration<in T> {
    static Parser<T> AsParser => Empty<T>(); 
    static bool Parse(ref int index, string source, out T val) {
        val = default;
        return false;
    }
        
}