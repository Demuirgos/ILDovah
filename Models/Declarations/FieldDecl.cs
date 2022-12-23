public record FieldDecl(RepeatOpt RepeatOpt, FieldAttr attributes, Type type, IdDecl Id, AtOpt atOpt, InitOpt Init) : Decl {
    public override string ToString()
        => $"{RepeatOpt} {attributes} {type} {Id} {atOpt} {Init}";

}

internal record AtOpt(IdDecl? Id) : Decl
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

internal record RepeatOpt(long? index) : Decl
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

internal record InitOpt(FieldInit init) :  Decl
{
    public override string ToString()
        => $"= {init}";
    /*
        initOpt :   EMPTY 
                    | '=' fieldInit
    */
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

internal record  FieldInit() : Decl
{
    /*
                | 'bytearray' '(' bytes ')' 
                | 'nullref' 
                ; 
    bytes :    EMPTY
                | hexbytes 
                ; 
    */
    internal static void Parse(ref int index, string source, out FieldInit fieldInit)
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
                    break;
                }
                case "bool":
                {
                    string[] bool_values = {"true", "false"};
                    source.ConsumeWord(ref index, "(");
                    if(source[index..].StartsWith(bool_values, out string bool_word)) {
                        index += bool_word.Length + 1;
                    }
                    source.ConsumeWord(ref index, ")");
                    break;
                }
                case "bytearray":
                {
                    source.ConsumeWord(ref index, "(");
                    while(source.ConsumeWord(ref index, ")")) {
                        BYTE.Parse(ref index, source, out BYTE byteValue);
                    }
                    break;
                }
                case "nullref":
                    break;
                case "compQstring":
                    CompQstring.Parse(ref index, source, out CompQstring compQstring);
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

internal record FieldAttr(string[] Attributes, String? MarshalType)
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