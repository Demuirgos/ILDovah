using System.Text;

public record CallConv(string? keyword) : Decl {
    public override string ToString()
        => keyword;

    public static void Parse(ref int index, string source, out CallConv conv) {
        String[] PossibleAttributes = { "instance" ,"explicit"};
        StringBuilder sb = new();
        bool StartsWith(string[] wordPool, string code, out String typeWord) {
            var result = wordPool.Any(x => code.StartsWith(x));
            if(result) {
                typeWord = wordPool.First(x => code.StartsWith(x));
            } else {
                typeWord = null;
            }
            return result;
        } 

        if(StartsWith(PossibleAttributes, source[index..], out string word)) {
            index += word.Length;
            CallConv.Parse(ref index, source, out CallConv subConv);
            sb.Append($"{word} {subConv}");
        } else {
            CallKind.Parse(ref index, source, out CallKind kind);
            sb.Append(kind);
        }
        conv = new CallConv(sb.ToString());
    }
}

public record CallKind(string? keyword) : Decl {
    public override string ToString()
        => keyword;

    /**/
    public static void Parse(ref int index, string source, out CallKind kind) {
        String[] PossibleWords = { "default" ,"vararg" ,"unmanaged"};
        String[] PossibleSubWords = {"cdecl", "stdcall", "thiscall", "fastcall"};
        StringBuilder sb = new();
        bool StartsWith(string[] wordPool, string code, out String typeWord) {
            var result = wordPool.Any(x => code.StartsWith(x));
            if(result) {
                typeWord = wordPool.First(x => code.StartsWith(x));
            } else {
                typeWord = null;
            }
            return result;
        } 

        if(StartsWith(PossibleWords, source[index..], out string word)) {
            index += word.Length;
            sb.Append(word);
            if(word == "unmanaged") {
                if(StartsWith(PossibleWords, source[index..], out string subword)) {
                    index += subword.Length;
                    sb.Append(subword);
                } else {
                    throw new Exception("Value Extraction Failed");
                }
            }
        }
        kind = new CallKind(sb.ToString());
    }
}

public record MethodSpec(string keyword) : Decl {
    public override string ToString()
        => keyword;
    public static void Parse(ref int index, string source, out MethodSpec spec) {
        string? word  = "method";
        index += word.Length;
        spec = new MethodSpec(word);
    }
}

public record MethodDecl() : Decl {
    
    
}