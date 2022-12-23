using System.Reflection.Metadata.Ecma335;
using System.Text;

public record ClassHeadDecl(ClassAttrDecl Attributes, String id, ExtendsClause Parent, ImplClause Interfaces) : Decl {
    public override string ToString()
        => $".class {Attributes} {id} {Parent} {Interfaces}";

    public static void Parse(ref int index, string source, out ClassHeadDecl classHeadDecl) {
        if(source.ConsumeWord(ref index, ".class")) {
            index += 6 + 1;
            ClassAttrDecl.Parse(ref index, source, out ClassAttrDecl classAttrDecl);
            ID.Parse(ref index, source, out ID id);
            ExtendsClause.Parse(ref index, source, out ExtendsClause extendsClause);
            ImplClause.Parse(ref index, source, out ImplClause implClause);
            classHeadDecl = new ClassHeadDecl(classAttrDecl, id.Value, extendsClause, implClause);
        } else {
            classHeadDecl = null;
        }
    }
}

public record ImplClause(ClassNames? Interfaces) : Decl
{
    public override string ToString()
        => Interfaces is null ? String.Empty : $"implements {Interfaces}";
    public static void Parse(ref int index, string source, out ImplClause implClause)
    {   
        if(source.ConsumeWord(ref index, "implements")) {
            ClassNames.Parse(ref index, source, out ClassNames classNames);
            implClause = new ImplClause(classNames);
        } else {
            implClause = new ImplClause((ClassNames?)null);
        }
    }
}
public record ClassNames(ClassName[] Names) : Decl {
    public override string ToString() 
        => $"{Names.Aggregate(String.Empty, (acc, className) => $"{acc}, {className}")}";

    public static void Parse(ref int index, string source, out ClassNames slashednameDecl) {
        List<ClassName> idDeclList = new();
        ClassName.Parse(ref index, source, out ClassName idDecl);
        idDeclList.Add(idDecl);
        while(source[index] == ',') {
            index++;
            ClassName.Parse(ref index, source, out ClassName idDecl2);
            idDeclList.Add(idDecl2);
        }
        slashednameDecl = new ClassNames(idDeclList.ToArray());
    }
}
public record ClassName(String? InnerName, bool HasDotModule, String OuterName) : Decl
{
    public override string ToString()
    {
        StringBuilder sb = new();
        if(InnerName is not null) {
            sb.Append($"[{(HasDotModule ? ".module" : String.Empty)} {InnerName}]");
        }
        sb.Append(OuterName);
        return sb.ToString();
    }

    public static void Parse(ref int index, string source, out ClassName className)
    {
        bool hasDotModule = false;
        string? innerName = null;
        if(source[index] == '[') {
            index++;
            if(source.ConsumeWord(ref index, ".module")) {
                hasDotModule = true;
            }
            NameDecl.Parse(ref index, source, out NameDecl nameDecl);
            innerName = nameDecl.Name;
            index++;
        }
        SlashednameDecl.Parse(ref index, source, out SlashednameDecl slashednameDecl);
        className = new ClassName(innerName, hasDotModule, slashednameDecl.idDecls);
    }
}

public record ExtendsClause(ClassName? className) : Decl
{
    public override string ToString()
        => className is null ? String.Empty : $"extends {className}";
    public static void Parse(ref int index, string source, out ExtendsClause extendsClause)
    {
        if(source.ConsumeWord(ref index, "extends")) {
            ClassName.Parse(ref index, source, out ClassName className);
            extendsClause = new ExtendsClause(className);
        } else {
            extendsClause = new ExtendsClause((ClassName?)null);
        }
    }
}

public record ClassAttrDecl(string[] Attributes) : Decl
{
    public override string ToString()
        => $"{String.Join(" ", Attributes)}";
    public static void Parse(ref int index, string source, out ClassAttrDecl classAttrDecl) {
        string[] possibleValues = new string[] { "public", "private", "value", "enum", "interface", "sealed", "abstract", "auto", "sequential", "explicit", "ansi", "unicode", "autochar", "import", "serializable", "nested", "public", "private", "beforefieldinit", "specialname", "rtspecialname" };
        string[] possibleNestedValues = new string[] {  "public", "private", "family", "assembly", "famandassem", "famorassem"};

        List<string> attributes = new List<string>();
        while(source[index..].StartsWith(possibleValues, out string word)) {
            if(word == "nested") {
                index += word.Length + 1;
                source[index..].StartsWith(possibleNestedValues,out string nestedWord);
                attributes.Add($"{word} {nestedWord}");
                index += nestedWord.Length;
            }
            else {
                attributes.Add(word);
                index += word.Length;
            }
        }
        
        classAttrDecl = new ClassAttrDecl(attributes.ToArray());
    }

}