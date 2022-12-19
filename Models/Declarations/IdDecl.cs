public record IdDecl(String Name) : Decl {
    public override string ToString()
        => Name;

    public static void Parse(ref int index, string source, out IdDecl idDecl) {
        if(source[index] == '\'') {
            QSTRING.Parse(ref index, source, false, out QSTRING qstring);
            idDecl = new IdDecl(qstring.Value);
        } else {
            ID.Parse(ref index, source, out ID id);
            idDecl = new IdDecl(id.Value);
        }
    }
}