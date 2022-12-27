using System.Diagnostics.CodeAnalysis;
using System.Text;

public static class Core {
    public delegate bool Parser(string source, ref int index, [NotNullWhen(true)] out string[] result);
    public static Parser ConsumeIf(Func<char, bool> predicate) {
        return (string code, ref int index, out string[] result) => {
            result = Array.Empty<string>();
            if(predicate(code[index])) {
                index++;
                result = new[] { code[index - 1].ToString() };
                return true;
            }
            return false;
        };
    }

    public static Parser ConsumeChar(char c) {
        return (string code, ref int index, out string[] result) => {
            return ConsumeIf(x => x == c)
                            (code,  ref index, out result);
        };
    }

    public static Parser ConsumeString(string word) {
        return (string code, ref int index, out string[] result) => {
            StringBuilder resultAcc = new();
            foreach (char c in word)
            {
                if(!ConsumeChar(c)(code, ref index, out result)) {
                    return false;
                }
                resultAcc.Append(result[0]);
            }
            result = new[] { resultAcc.ToString() };
            return true;
        };
    }

    public static Parser TryRun(params Parser[] parsers) {
        return (string code, ref int index, out string[] result) => {
            int oldIndex = index;
            result = null;
            foreach (Parser parser in parsers)
            {
                if(parser(code, ref index, out result)) {
                    return true;
                }
                index = oldIndex;
            }
            return false;
        };
    }

    public static Parser RunAll(params Parser[] parsers) {
        return (string code, ref int index, out string[] result) => {
            int oldIndex = index;
            var resultAcc = new List<string>();
            foreach (Parser parser in parsers)
            {
                if(!parser(code, ref index, out result)) {
                    index = oldIndex;
                    return false;
                }
                resultAcc.AddRange(result);
            }
            result = resultAcc.ToArray();
            return true;
        };
    }
}