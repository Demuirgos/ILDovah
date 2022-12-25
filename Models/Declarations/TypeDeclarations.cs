using System.Text;
using System.Threading.Channels;

public record TypeSpec() : Decl
{
    public override string ToString()
        => String.Empty;
    public static bool Parse(ref int index, string source, out TypeSpec typeSpec)
    {
        int start = index;
        if(source.ConsumeWord(ref index, "[")) {
            if(source.ConsumeWord(ref index, ".module")) {
                NameDecl.Parse(ref index, source, out NameDecl className);
                typeSpec = new TypeSpecModule(className);
            } else {
                NameDecl.Parse(ref index, source, out NameDecl className);
                typeSpec = new TypeSpecNamed(className);
            }
            source.ConsumeWord(ref index, "]");
        } else {
            if(ClassName.Parse(ref index, source, out ClassName className)) {
                typeSpec = new TypeSpecClassNamed(className);
            } else {
                Type.Parse(ref index, source, out Type type);
                typeSpec = new TypeSpecTyped(type);
            }
        }
        return start != index;
    }
}
 
public record TypeSpecModule(NameDecl Name) : TypeSpec {
    public override string ToString()
        => $"[.module {Name}]";
}
public record TypeSpecNamed(NameDecl Name) : TypeSpec {
    public override string ToString()
        => $"[{Name}]";
}
public record TypeSpecClassNamed(ClassName Name) : TypeSpec {
    public override string ToString()
        => Name.ToString();
}
public record TypeSpecTyped(Type type) : TypeSpec {
    public override string ToString()
        => type.ToString();
}



public record Type(string? TypeName) : Decl 
{
    public override string ToString()
        => TypeName;
    public static bool Parse(ref int index, string source, out Type? typeDecl) {
        string[] PossiblePrimitiveValues = {"typedref", "object", "string", "char", "void", "bool", "int8", "int16", "int32", "int64", "float32", "float64", "unsignedint8", "unsignedint16", "unsignedint32", "unsignedint64", "nativeint", "nativeunsignedint", "nativefloat"};
        string[] PossibleSecondaryValues = {"&", "*",  "pinned"}; 
        string[] PossibleSpecialValues = {"modopt", "modreq"}; 
        int start = index;
        StringBuilder sb = new StringBuilder();

        if(source[index..].StartsWith(PossiblePrimitiveValues, out string typeWord)) {
            index += typeWord.Length;
            sb.Append(typeWord);
        } else if(source.ConsumeWord(ref index, "!")) {
            index++;
            INT.Parse(ref index, source, out INT intvalue);
            sb.Append($" !{intvalue} ");
        } else if(source[index..].StartsWith(PossibleSecondaryValues, out string attrword)) {
            index+=attrword.Length;
            sb.Append($" {attrword} ");
            ClassName.Parse(ref index, source, out ClassName className);
            sb.Append($" {className} ");
        } else {
            MethodSpec.Parse(ref index, source, out MethodSpec spec);
            CallConv.Parse(ref index, source, out CallConv conv);
            Type.Parse(ref index, source, out Type typedecl);
            source.ConsumeWord(ref index, "*");
            source.ConsumeWord(ref index, "(");
            SigArgs.Parse(ref index, source, out SigArgs sigargs);
            source.ConsumeWord(ref index, ")");
        }
        if(source[index] == '[' ||  source[index..].StartsWith("value")) {
            if(source.ConsumeWord(ref index, "value")) {
                index += "value".Length;
                sb.Append("value");
            }

            if(source[index] == '[') {
                index++;
                INT.Parse(ref index, source, out INT intval);
                if(intval is null) {
                    Bounds.Parse(ref index, source, out Bounds bounds);
                    sb.Append($"[{bounds}]");
                } else {
                    sb.Append($"[{intval}]");
                }
                index++;
            }
        } else if(source[index..].StartsWith(PossibleSecondaryValues, out var capturedWord)) {
            index+=capturedWord.Length;
            sb.Append(capturedWord);
        } else if(source[index..].StartsWith(PossibleSpecialValues, out var capturedPrefix)) {
            index+=capturedPrefix.Length;
            source.ConsumeWord(ref index, "(");
            ClassName.Parse(ref index, source, out ClassName className);
            source.ConsumeWord(ref index, ")");
            sb.Append($"{capturedPrefix} ({className})");
        }

        typeDecl = new Type(sb.ToString());
        return start != index;
    }
}

internal record VariantType(string Type) : Decl
{
    public override string ToString()
        => Type;

    internal static bool Parse(ref int index, string source, out VariantType? marshalTypeDecl)
    {
        string[] VariantTypes = { "variant", "null", "currency", "void", "bool", "int8", "int16", "int32", "int64", "float32", "float64", "*","decimal", "date", "bstr", "lpstr", "lpwstr", "iunknown", "idispatch", "safearray", "int", "unsigned","error", "hresult", "carray", "userdefined", "record", "filetime", "blob", "stream", "storage", "streamed_object", "stored_object", "blob_object", "cf", "clsid"};
        string[] UnsignedTypes = { "int" ,"int8", "int16", "int32", "int64"};
        string[] Complementary = { "[]" ,"&", "vector"};
        string marshalType; 
        int start = index;

        if(source[index..].StartsWith(VariantTypes, out string? typeWord)) {
            if(typeWord == "unsigned") {
                index += typeWord.Length;
                if(source[index..].StartsWith(UnsignedTypes, out typeWord)) {
                    marshalType = $" unsigned {typeWord} ";
                    index += typeWord.Length;
                } else {
                    throw new Exception("VariantType.Parse: Invalid source");
                }
            } else {
                marshalType = typeWord;
                index += typeWord.Length;
            }
        } else {
            marshalType = String.Empty;
        }

        while(source[index..].StartsWith(Complementary, out typeWord)) {
            marshalType += typeWord;
            index += typeWord.Length;
        }

        marshalTypeDecl = new VariantType(marshalType);
        return start != index;
    }
}

public record NativeType(String TypeName) : Decl
{
    public override string ToString()
        => TypeName;

    internal static bool Parse(ref int index, string source, out NativeType? marshalType)
    {
        string[] NativeTypes = {"variant", "currency", "syschar", "void", "bool", "int8", "int16", "int32", "int64", "float32", "float64", "error", "unsignedint8", "unsignedint16", "unsignedint32", "unsignedint64", "decimal", "date", "bstr", "lpstr", "lpwstr", "lptstr", "objectref", "iunknown", "idispatch", "struct", "interface", "int", "unsignedint", "nested", "byvalstr", "ansibstr", "tbstr", "variantbool", "methodSpec","asany", "lps"};
        StringBuilder sb = new StringBuilder();
        int start = index;
        marshalType = null;

        if(source[index..].StartsWith(NativeTypes, out string? typeWord)) {
            sb.Append(typeWord);
            index += typeWord.Length;
        } else {
            void consumeIndexer(ref int i, string source, out string result) {
                INT intDecl2 = null;
                INT intDecl1 = null;
                
                if(source.ConsumeWord(ref i, "[")) {
                    INT.Parse(ref i, source, out intDecl1);
                    if(source.ConsumeWord(ref i, "+")) {
                        INT.Parse(ref i, source, out intDecl2);
                    }
                }
                source.ConsumeWord(ref i, "]");

                result = $"[{intDecl1?.ToString() ?? ""}+{intDecl2?.ToString() ?? ""}]";
            }

            if(source.ConsumeWord(ref index, "custom")) {
                sb.Append(" custom ");
                int substart = index;
                if(source.ConsumeWord(ref index, "(")) {
                    source.ConsumeUntil(ref index, (rest) => rest[0] == ')');
                    source.ConsumeWord(ref index, ")");
                }
                sb.Append(source[substart..index]);
            } else if(source.ConsumeWord(ref index, "fixed")) {
                sb.Append(" fixed ");
                if(source.ConsumeWord(ref index, "sysstring")) {
                    sb.Append(" sysstring ");
                } else if(source.ConsumeWord(ref index, "array")) {
                    sb.Append(" array ");
                } else {
                    throw new Exception("NativeType.Parse: Invalid source");
                }
                consumeIndexer(ref index, source, out string s);
            } else if(source.ConsumeWord(ref index, "safearray")) {
                sb.Append(" safearray ");
                VariantType.Parse(ref index, source, out VariantType? variantType);
                sb.Append(variantType);
                if(source[index] == ',') {
                    index++;
                    CompQstring.Parse(ref index, source, out CompQstring? compQstring);
                    sb.Append(compQstring);
                }
            } else {
                throw new Exception("NativeType.Parse: Invalid source");
            }

            if(source.ConsumeWord(ref index, "*")) {
                sb.Append("*");
            } else if(source[index] == '[') {
                consumeIndexer(ref index, source, out string s);
                sb.Append(s);
            }
            
            marshalType = new NativeType(sb.ToString());
        }
        return start != index;
    }
}