using System.Text;

public record Type(string? TypeName) : Decl 
{
    public override string ToString()
        => TypeName;
    /*
    type : 
            | type '[' ']' 
            | type '[' bounds1 ']' 
            | type 'value' '[' int32 ']' 
            | type '&' 
            | type '*' 
            | type 'pinned' 
            | type 'modreq' '(' className ')' 
            | type 'modopt' '(' className ')' 

            | methodSpec callConv type '*' '(' sigArgs0 ')' 
            ;
 */
    public void Parse(ref int index, string source, out Type? typeDecl) {
        string[] PossiblePrimitiveValues = {"typedref", "object", "string", "char", "void", "bool", "int8", "int16", "int32", "int64", "float32", "float64", "unsignedint8", "unsignedint16", "unsignedint32", "unsignedint64", "nativeint", "nativeunsignedint", "nativefloat"};
        string[] PossibleSecondaryValues = {"class", "valueclass",  "valuetype"}; 
        StringBuilder sb = new StringBuilder();
        bool StartsWithSimpleWord(string[] wordPool, string code, out String typeWord) {
            var result = wordPool.Any(x => code.StartsWith(x));
            if(result) {
                typeWord = wordPool.First(x => code.StartsWith(x));
            } else {
                typeWord = null;
            }
            return result;
        } 

        if(StartsWithSimpleWord(PossiblePrimitiveValues, source[index..], out string typeWord)) {
            index += typeWord.Length;
            sb.Append(typeWord);
        } else if(source[index] == '!') {
            index++;
            INT.Parse(ref index, source, out INT intvalue);
            sb.Append($" !{intvalue} ");
        } else if(StartsWithSimpleWord(PossibleSecondaryValues, source[index..], out string attrword)) {
            index+=attrword.Length;
            sb.Append($" {attrword} ");
            ClassName.Parse(ref index, source, out ClassName className);
            sb.Append($" {className} ");
        }
    }
}

internal record VariantType(string Type) : Decl
{
    public override string ToString()
        => Type;

    internal static void Parse(ref int index, string source, out VariantType? marshalTypeDecl)
    {
        string[] VariantTypes = { "variant", "null", "currency", "void", "bool", "int8", "int16", "int32", "int64", "float32", "float64", "*'","decimal", "date", "bstr", "lpstr", "lpwstr", "iunknown", "idispatch", "safearray", "int", "unsigned","error", "hresult", "carray", "userdefined", "record", "filetime", "blob", "stream", "storage", "streamed_object", "stored_object", "blob_object", "cf", "clsid"};
        string[] UnsignedTypes = { "int" ,"int8", "int16", "int32", "int64"};
        string[] Compelementary = { "[]" ,"&", "vector"};
        string marshalType; 
        bool StartsWithSimpleWord(string[] wordPool, string code, out String typeWord) {
            var result = wordPool.Any(x => code.StartsWith(x));
            if(result) {
                typeWord = wordPool.First(x => code.StartsWith(x));
            } else {
                typeWord = null;
            }
            return result;
        } 

        if(StartsWithSimpleWord(VariantTypes, source[index..], out string? typeWord)) {
            if(typeWord == "unsigned") {
                index += typeWord.Length;
                if(StartsWithSimpleWord(UnsignedTypes, source[index..], out typeWord)) {
                    marshalType = $"unsigned {typeWord}";
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

        while(StartsWithSimpleWord(Compelementary, source[index..], out typeWord)) {
            marshalType += typeWord;
            index += typeWord.Length;
        }

        marshalTypeDecl = new VariantType(marshalType);
    }
}

public record NativeType(String TypeName) : Decl
{
    public override string ToString()
        => Type;

    internal static void Parse(ref int index, string source, out NativeType? marshalType)
    {
        string[] NativeTypes = {"variant", "currency", "syschar", "void", "bool", "int8", "int16", "int32", "int64", "float32", "float64", "error", "unsignedint8", "unsignedint16", "unsignedint32", "unsignedint64", "decimal", "date", "bstr", "lpstr", "lpwstr", "lptstr", "objectref", "iunknown", "idispatch", "struct", "interface", "int", "unsignedint", "nested", "byvalstr", "ansibstr", "tbstr", "variantbool", "methodSpec","asany", "lps"};
        StringBuilder sb = new StringBuilder();
        marshalType = null;

        bool StartsWithSimpleWord(string[] wordPool, string code, out String typeWord) {
            var result = wordPool.Any(x => code.StartsWith(x));
            if(result) {
                typeWord = wordPool.First(x => code.StartsWith(x));
            } else {
                typeWord = null;
            }
            return result;
        } 

        if(StartsWithSimpleWord(NativeTypes, source[index..], out string? typeWord)) {
            sb.Append(typeWord);
            index += typeWord.Length;
        } else {
            void consumeIndexer(ref int i, string source, out string result) {
                INT intDecl2 = null;
                INT intDecl1 = null;
                if(source[i] == '[') {
                    i++;
                    INT.Parse(ref i, source, out intDecl1);
                    if(source[i] == '+') {
                        i++;
                        INT.Parse(ref i, source, out intDecl2);
                    }
                }
                i++;
                result = $"[{(intDecl1?.ToString() ?? "")}+{(intDecl2?.ToString() ?? "")}]";
            }
            if(source[index..].StartsWith("custom")) {
                sb.Append(" custom ");
                index += "custom".Length;
                int start = index;
                if(source[index] == '(') {
                    while(source[index] != ')') {
                        index++;
                    }
                    index++;
                }
                sb.Append(source[start..index]);
            } else if(source[index..].StartsWith("fixed")) {
                sb.Append(" fixed ");
                index+= "fixed".Length;
                if(source[index..].StartsWith("sysstring")) {
                    sb.Append(" sysstring ");
                    index+= "sysstring".Length;
                    
                } else if(source[index..].StartsWith("array")) {
                    sb.Append(" array ");
                    index+= "array".Length;
                } else {
                    throw new Exception("NativeType.Parse: Invalid source");
                }
                consumeIndexer(ref index, source, out string s);
            } else if(source[index..].StartsWith("safearray")) {
                sb.Append(" safearray ");
                index+= "safearray".Length;
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

            if(source[index] == '*') {
                sb.Append("*");
                index++;
            } else if(source[index] == '[') {
                consumeIndexer(ref index, source, out string s);
                sb.Append(s);
            }
            
            marshalType = new NativeType(sb.ToString());
        }

    }
}