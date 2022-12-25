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
    public static void Parse(ref int index, string source, out SigArg sigArg)
    {
        if(source.ConsumeWord(ref index ,"...")) {
            sigArg = new SigArgEmpty();
        } else {
            ParameterAttribute.Parse(ref index, source, out var attr);
            Type.Parse(ref index, source, out var type);
            if(source.ConsumeWord(ref index, "marshal")) {
                source.ConsumeWord(ref index, "(");
                NativeType.Parse(ref index, source, out var marshalled);
                source.ConsumeWord(ref index, ")");
                sigArg = new SigArgMarshal(attr, type, marshalled);

            } else {
                IdDecl.Parse(ref index, source, out var id);
                sigArg = new SigArgId(attr, type, id);
            }
        }
    } 

}
public record SigArgEmpty() : SigArg {
    public override string ToString() => "...";
}

public record SigArgId(ParameterAttribute? Attribute, Type TypeDef, IdDecl Id) : SigArg {
    public override string ToString() {
        StringBuilder sb = new ();
        sb.Append(Attribute);
        sb.Append(Id);
        return sb.ToString();
    }
}

public record SigArgMarshal(ParameterAttribute? Attribute, Type TypeDef, NativeType MarshalledType) : SigArg {
    public override string ToString() {
        StringBuilder sb = new ();
        sb.Append(Attribute);
        sb.Append($"marshal ({MarshalledType})");
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
        if(source.ConsumeWord(ref index, "[")) {
            if(source[index..].StartsWith(options, out string word)) {
                attrs.Add(word);
                index += word.Length;
            } else {
                INT.Parse(ref index, source, out INT intval);
                attrs.Add(intval.Value.ToString());
            }
            source.ConsumeWord(ref index, "]");
        }
        attribute = new ParameterAttribute(attrs.ToArray());
    }
}