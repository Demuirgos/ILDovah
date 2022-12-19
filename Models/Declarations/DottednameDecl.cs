public record DottednameDecl(String idDecls) : Decl {
    public override string ToString()
        => idDecls;

    public static void Parse(ref int index, string source, out DottednameDecl dottednameDecl) {
        List<IdDecl> idDeclList = new();
        IdDecl.Parse(ref index, source, out IdDecl idDecl);
        idDeclList.Add(idDecl);
        while(source[index] == '.') {
            index++;
            IdDecl.Parse(ref index, source, out IdDecl idDecl2);
            idDeclList.Add(idDecl2);
        }
        dottednameDecl = new DottednameDecl(idDeclList.Aggregate(String.Empty, (acc, id) => $"{acc}.{id.Name}"));
        
    }
}
