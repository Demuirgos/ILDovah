using System.Reflection;
using System.Text;

public record SigArgs(int n, SigArg[] SigArguments) : Decl
{
    public override string ToString()
        => SigArguments.Aggregate(String.Empty, (acc, arg) => $"{acc}, {arg}");
    public static void Parse(ref int index, string source, out SigArgs sigArg)
    {
        List<SigArg> args = new();
        do 
        { 
            SigArg.Parse(ref index, source, out SigArg arg);
            args.Add(arg);
            index++;
        } while(source[index] != ',');
        sigArg = new SigArgs(args.Count, args.ToArray());
    } 
}
public record SigArg() : Decl
{
    public override string ToString()
    {
        if(this is SigArgEmpty) {
            return (this as SigArgEmpty).ToString();
        } else {
            return (this as SigArgFull).ToString();
        }
    }
    public static void Parse(ref int index, string source, out SigArg sigArg)
    {
        if(source[index..].StartsWith("...")) {
            sigArg = new SigArgEmpty();
        }
        ParameterAttribute attr = null;
        Type type = null;
        NativeType marshalled = null;
        IdDecl id = null;
        ParameterAttribute.Parse(ref index, source, out attr);
        Type.Parse(ref index, source, out type);
        if(source[index..].StartsWith("marshal")) {
            index++;
            NativeType.Parse(ref index, source, out marshalled);
            index++;
        } else {
            IdDecl.Parse(ref index, source, out id);
        }
        sigArg = new SigArgFull(attr, type, id, marshalled);
    } 

}
public record SigArgEmpty() : SigArg {
    public override string ToString() => "...";
}

public record SigArgFull(ParameterAttribute? Attribute, Type? TypeDef, IdDecl? Id, NativeType? MarshalledType) : SigArg {
    public override string ToString() {
        StringBuilder sb = new ();
        sb.Append(Attribute);
        if(Id is not null) {
            sb.Append(Id);
        } else if(MarshalledType  is not null) {
            sb.Append($"marshal ({MarshalledType})");
        }
        return sb.ToString();
    }
}

public record ParameterAttribute(String[] Attributes) : Decl
{
    public override string ToString()
        => Attributes.Aggregate(String.Empty, (acc, attr) => $"acc [{attr}]");
    public static void Parse(ref int index, string source, out ParameterAttribute attribute) {
        String[] options = {"in","out","opt"};
        List<String> attrs = new();
        bool StartsWithSimpleWord(string[] wordPool, string code, out String typeWord) {
            var result = wordPool.Any(x => code.StartsWith(x));
            if(result) {
                typeWord = wordPool.First(x => code.StartsWith(x));
            } else {
                typeWord = null;
            }
            return result;
        } 
        if(source[index] == '[') {
            index++;
            if(StartsWithSimpleWord(options, source[index..], out string word)) {
                attrs.Add(word);
            } else {
                INT.Parse(ref index, source, out INT intval);
                attrs.Add(intval.Value.ToString());
            }
            index++;
        }
        attribute = new ParameterAttribute(attrs.ToArray());
    }
}