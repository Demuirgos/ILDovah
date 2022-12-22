public record ModuleHeadDecl(bool IsExtern, String Name) : Decl {
    public override string ToString()
        => $".module {(IsExtern ? "extern " : "")}{Name}";

    public static void Parse(ref int index, string source, out ModuleHeadDecl moduleHeadDecl) {
        if(source[index..].StartsWith(".module")) {
            index += 7;
            bool isExtern = false;
            if(source[index..].StartsWith("extern")) {
                index += 6;
                isExtern = true;
            }
            NameDecl.Parse(ref index, source, out NameDecl nameDecl);
            moduleHeadDecl = new ModuleHeadDecl(isExtern, nameDecl.Name);
        } else {
            throw new Exception("ModuleHeadDecl.Parse: Invalid source");
        }
    }
}

public record ModuleDecl() : Decl {
    
}