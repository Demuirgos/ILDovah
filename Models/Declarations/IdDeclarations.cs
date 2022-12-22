using System.Text;

public record ID(string Value) {
    public override string ToString() => Value;
    public static void Parse(ref int index, string source, out ID id) {
        if(Char.IsLetter(source[index])) {
            StringBuilder sb = new StringBuilder();
            while(Char.IsLetterOrDigit(source[index])) {
                sb.Append(source[index++]);
            }
            id = new ID(sb.ToString());
            return;
        }
        id = null;
        return;
    }
}

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

public record NameDecl(String Name) : Decl {
    public override string ToString()
        => Name;
    
    public static void Parse(ref int index, string source, out NameDecl nameDecl) {
        DottednameDecl.Parse(ref index, source, out DottednameDecl idDecl);
        nameDecl = new NameDecl(idDecl.idDecls);
    }
}

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

public record SlashednameDecl(String idDecls) : Decl {
    public override string ToString()
        => idDecls;
    public static void Parse(ref int index, string source, out SlashednameDecl slashednameDecl) {
        List<NameDecl> idDeclList = new();
        NameDecl.Parse(ref index, source, out NameDecl idDecl);
        idDeclList.Add(idDecl);
        while(source[index] == '/') {
            index++;
            NameDecl.Parse(ref index, source, out NameDecl idDecl2);
            idDeclList.Add(idDecl2);
        }
        slashednameDecl = new SlashednameDecl(idDeclList.Aggregate(String.Empty, (acc, id) => $"{acc}/{id.Name}"));
    }
}
