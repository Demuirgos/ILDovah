public record NameDecl(String Name) : Decl {
    /*
    name1 : id 1
        | DOTTEDNAME 1..n
        | name1 '.' name1  1..n . 1..n
        ;
    */
    public override string ToString()
        => Name;
    
    public static void Parse(ref int index, string source, out NameDecl nameDecl) {
        DottednameDecl.Parse(ref index, source, out DottednameDecl idDecl);
        nameDecl = new NameDecl(idDecl.idDecls);
    }

}
