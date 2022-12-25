using System.Text;
public record MethodName(String Name, bool IsConstructor) : Decl 
{
    public override string ToString()
        => Name;
    
    public static bool Parse(ref int index, string source, out MethodName methodName) {
        string[] SpecialNames = { ".ctor", ".cctor" };
        int start = index;
        if(source[index..].StartsWith(SpecialNames, out string name)) {
            index += name.Length;
            methodName = new MethodName(name, true);
        } else {
            NameDecl.Parse(ref index, source, out NameDecl id);
            methodName = new MethodName(id.Name, false);
        }
        return start != index;
    }
}
public record CallConv(string? keyword) : Decl {
    public override string ToString()
        => keyword;

    public static bool Parse(ref int index, string source, out CallConv conv) {
        String[] PossibleAttributes = { "instance" ,"explicit"};
        StringBuilder sb = new();
        int start = index; 
        if(source[index..].StartsWith(PossibleAttributes, out string word)) {
            index += word.Length;
            CallConv.Parse(ref index, source, out CallConv subConv);
            sb.Append($"{word} {subConv}");
        } else {
            CallKind.Parse(ref index, source, out CallKind kind);
            sb.Append(kind);
        }
        conv = new CallConv(sb.ToString());
        return start != index;
    }
}

public record CallKind(string? keyword) : Decl {
    public override string ToString()
        => keyword;

    public static bool Parse(ref int index, string source, out CallKind kind) {
        String[] PossibleWords = { "default" ,"vararg" ,"unmanaged"};
        String[] PossibleSubWords = {"cdecl", "stdcall", "thiscall", "fastcall"};
        StringBuilder sb = new();
        if(source[index..].StartsWith(PossibleWords, out string word)) {
            index += word.Length;
            sb.Append(word);
            if(word == "unmanaged") {
                source[index..].StartsWith(PossibleWords, out string subword);
                index += subword.Length;
                sb.Append(subword);
            }
            kind = new CallKind(sb.ToString());
            return true;
        } else {
            kind = null;
            return false;
        }
    }
}

public record MethodSpec(string keyword) : Decl {
    public override string ToString()
        => keyword;
    public static bool Parse(ref int index, string source, out MethodSpec spec) {
        string word = "method";
        if(source.ConsumeWord(ref index, word)) {
            spec = new MethodSpec(word);
            return true;
        }
        spec = null;
        return false;
    }
}

public record MethodDecl() : Decl {
    
    
}