public record FieldDecl(RepeatOpt RepeatOpt, FieldAttr attributes, Type type, IdDecl Id, AtOpt atOpt, InitOpt Init) : Decl {
    public override string ToString()
        => $".field {RepeatOpt} {attributes} {type} {Id} {atOpt} {Init}";

    public static void Parse(ref int index, string source, out FieldDecl fieldDecl) {
        source.ConsumeWord(ref index, ".field");
        RepeatOpt.Parse(ref index, source, out RepeatOpt repeatOpt);
        FieldAttr.Parse(ref index, source, out FieldAttr fieldAttr);
        Type.Parse(ref index, source, out Type type);
        IdDecl.Parse(ref index, source, out IdDecl id);
        AtOpt.Parse(ref index, source, out AtOpt atOpt);
        InitOpt.Parse(ref index, source, out InitOpt initOpt);
        fieldDecl = new FieldDecl(repeatOpt, fieldAttr, type, id, atOpt, initOpt);
    }

}

public record AtOpt(IdDecl? Id) : Decl
{
    public override string ToString()
        => Id is null ? String.Empty : $"at {Id}";

    public static void Parse(ref int index, string source, out AtOpt atOpt)
    {   
        if(source[index..].StartsWith("at")) {
            index += 10 + 1;
            IdDecl.Parse(ref index, source, out IdDecl idDecl);
            atOpt = new AtOpt(idDecl);
        } else {
            atOpt = new AtOpt((IdDecl?)null);
        }
    }
}

public record RepeatOpt(long? index) : Decl
{
    public override string ToString()
        => index is null ? String.Empty : $"[{index}]";

    public static void Parse(ref int index, string source, out RepeatOpt repeatOpt)
    {   
        if(source[index] == '[') {
            index += 10 + 1;
            INT.Parse(ref index, source, out INT indexDecl);
            repeatOpt = new RepeatOpt(indexDecl.Value);
            index++;
        } else {
            repeatOpt = new RepeatOpt((long?)null);
        }
    }
}

public record InitOpt(FieldInit init) :  Decl
{
    public override string ToString()
        => $"= {init}";
    public static void Parse(ref int index, string source, out InitOpt initOpt)
    {   
        if(source.ConsumeWord(ref index, "=")) {
            FieldInit.Parse(ref index, source, out FieldInit fieldInit);
            initOpt = new InitOpt(fieldInit);
        } else {
            initOpt = new InitOpt((FieldInit)null);
        }
    }
}

public record  FieldInit() : Decl
{
    public static void Parse(ref int index, string source, out FieldInit fieldInit)
    {
        string[] possible_tokens = {"float32","float64","float32","float64","int64","int32","int16","char","int8","bool","bytearray","nullref", "compQstring"};
        if(source[index..].StartsWith(possible_tokens, out string word)) {
            if(word != "CompQstring") {
                source.ConsumeWord(ref index, word);
            }

            switch(word) {
                case "float32":
                case "float64":
                {
                    source.ConsumeWord(ref index, "(");
                    FLOAT.Parse(ref index, source, out FLOAT real_number);
                    source.ConsumeWord(ref index, ")");
                    fieldInit = new FieldInitFloat(real_number);
                    break;
                }
                case "int64":
                case "int32":
                case "int16":
                case "char":
                case "int8":
                {
                    source.ConsumeWord(ref index, "(");
                    INT.Parse(ref index, source, out INT int_number);
                    source.ConsumeWord(ref index, ")");
                    fieldInit = new FieldInitInt(int_number);
                    break;
                }
                case "bool":
                {
                    source.ConsumeWord(ref index, "(");
                    BOOL.Parse(ref index, source, out BOOL bool_word);
                    source.ConsumeWord(ref index, ")");
                    fieldInit = new FieldInitBool(bool_word);
                    break;
                }
                case "bytearray":
                {
                    List<BYTE> bytes = new();
                    source.ConsumeWord(ref index, "(");
                    while(source.ConsumeWord(ref index, ")")) {
                        BYTE.Parse(ref index, source, out BYTE byteValue);
                        bytes.Add(byteValue);
                    }
                    fieldInit = new FieldInitByteArray(bytes.ToArray());
                    break;
                }
                case "nullref":
                    fieldInit = new FieldInitRef();
                    break;
                case "compQstring":
                    CompQstring.Parse(ref index, source, out CompQstring compQstring);
                    fieldInit = new FieldInitString(compQstring);
                    break;
                default:
                    throw new NotImplementedException();
            }
        } else {
            throw new NotImplementedException();
        }
        throw new NotImplementedException();
    }
}

public record  FieldInitFloat(FLOAT f) : FieldInit {
    public override string ToString()
        => f.Value.ToString();
}
public record  FieldInitInt(INT n) : FieldInit {
    public override string ToString()
        => n.Value.ToString();
}
public record  FieldInitBool(BOOL n) : FieldInit {
    public override string ToString()
        => n.Value.ToString();
}
public record  FieldInitByteArray(BYTE[] bs) : FieldInit {
    public override string ToString()
        => $"{{{String.Join(", ", bs.Select(b => b.ToString()))}}}";
}
public record  FieldInitString(CompQstring str) : FieldInit {
    public override string ToString()
        => str.ToString();
}
public record  FieldInitRef() : FieldInit {
    public override string ToString()
        => "nullref";
}



public record FieldAttr(string[] Attributes, String? MarshalType)
{
    public override string ToString()
        => String.Join(" ", Attributes) + (MarshalType is null ? String.Empty : $"marshal ({MarshalType})");
        
    public static void Parse(ref int index, string source, out FieldAttr fieldAttr) {
        string[] possibleValues = new string[] { "static", "public", "private", "family", "initonly", "rtspecialname", "specialname", "pinvokeimpl", "marshal", "assembly", "famandassem", "famorassem", "privatescope", "literal", "notserialized"};
        NativeType marshalType = null;        
        List<string> attributes = new List<string>();
        while(source[index..].StartsWith(possibleValues, out string word)) {
            if(word == "pinvokeimpl") {
                // commented out because PInvoke for fields is not supported by EE
            } else {
                if(word == "marshal") {
                    source.ConsumeWord(ref index, "(");
                    NativeType.Parse(ref index, source, out marshalType);
                    source.ConsumeWord(ref index, ")");
                }
                attributes.Add(word);
                index += word.Length;
            }
        }
        
        fieldAttr = new FieldAttr(attributes.ToArray(), marshalType.TypeName);
    }
}