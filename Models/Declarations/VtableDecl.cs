public record VtableDecl(string[] values) : Decl {
    public override string ToString()
        => $".vtable = ({ String.Join(" ", values) })";
    
    public static void Parse(ref int index, string source, out VtableDecl vtableDecl) {
        if(source[index..].StartsWith(".vtable")) {
            index += 7 + 2;
            List<string> bytes = new();
            while(source[index] != ')') {
                bytes.Add(source[index..(index + 2)]);
                index += 2;
            }
            index++;
            vtableDecl = new VtableDecl(bytes.ToArray());
        } else {
            vtableDecl = null;
        }
    }
}