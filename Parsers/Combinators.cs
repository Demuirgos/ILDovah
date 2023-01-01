using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public static class Core {
    public static T Id<T>(T value) => value;
    public delegate bool Parser<T>(string source, ref int index, [NotNullWhen(true)] out T result);
    
    public static Parser<T> Whitespace<T>() => (string code, ref int index, out T result) => {
        result = default;
        while(index < code.Length && Char.IsWhiteSpace(code[index])) {
            index++;
        }
        return true;
    };

    public static Parser<T> Empty<T>() => (string code, ref int index, out T result) => {
        result = default;
        return true;
    };

    public static Parser<T> Discard<T, U>(Parser<U> parser) {
        return (string code, ref int index, out T result) => {
            bool isParsed = parser(code, ref index, out U value);
            result = default(T);
            return isParsed;
        };
    }

    public static Parser<T> Lazy<T>(Func<Parser<T>> parser) {
        return (string code, ref int index, out T result) => {
            return parser()(code, ref index, out result);
        };
    }

    public static Parser<T> Fail<T>(string? message = null) {
        return (string code, ref int index, out T result) => {
            result = default;
            throw new UnreachableException(message ?? "This parser should never be reached");
        };
    }

    public static Parser<T> Cast<T, U>(Parser<U> parser) {
        return Map(item => (T)((object)item), parser);
    }

    public static Parser<T> Map<T, U>(Func<U, T> converter, Parser<U> parser) {
        return (string code, ref int index, out T result) => {
            bool isParsed = parser(code, ref index, out U value);
            result = isParsed ? converter(value) : default;
            return isParsed;
        };
    }

    public static Parser<T> ConsumeIf<T>(Func<char, T> converter, Func<char, bool> predicate) {
        return (string code, ref int index, out T result) => {
            result = default;
            if(index < code.Length && predicate(code[index])) {
                index++;
                result = converter(code[index - 1]);
                return true;
            }
            return false;
        };
    }

    public static Parser<T> ConsumeChar<T>(Func<char, T> converter, char c) {
        return (string code, ref int index, out T result) => {
            bool isParsed = ConsumeIf(Id, x => x == c)
                                     (code,  ref index,  out char charC);
            if(isParsed) {
                result = converter(charC);
            }
            else {
                result = default;
            }

            return isParsed;
        };
    }

    public static Parser<T> ConsumeWord<T>(Func<string, T> converter, string word) {
        return (string code, ref int index, out T result) => {
            StringBuilder resultAcc = new();
            foreach (char c in word)
            {
                if(!ConsumeChar(Id, c)(code, ref index, out char character)) {
                    result = default;
                    return false;
                }
                resultAcc.Append(character);
            }
            result = converter(resultAcc.ToString());
            return true;
        };
    }

    public static Parser<U> TryRun<T,U>(Func<T, U> converter, params Parser<T>[] parsers) {
        return (string code, ref int index, out U result) => {
            int oldIndex = index;
            result = default;
            foreach (Parser<T> parser in parsers)
            {
                if(parser(code, ref index, out T subResult)) {
                    result = converter(subResult);
                    return true;
                }
                index = oldIndex;
            }
            return false;
        };
    }

    public static Parser<U> RunMany<T, U>(Func<T[], U> converter, int min, int max, Parser<T> parser) {
        return (string code, ref int index, out U result) => {
            int oldIndex = index;
            var resultAcc = new List<T>();
            for (int i = 0; i < max; i++)
            {
                if(!parser(code, ref index, out T single) || index > code.Length) {
                    if(i < min) {
                        index = oldIndex;
                        result = default;
                        return false;
                    }
                    break;
                }
                resultAcc.Add(single);
            }
            result = converter(resultAcc.ToArray());
            return true;
        };
    }
    public static Parser<U> RunAll<T, U>(Func<T[], U> converter, params Parser<T>[] parsers) {
        return (string code, ref int index, out U result) => {
            int oldIndex = index;
            var resultAcc = new List<T>();
            foreach (Parser<T> parser in parsers)
            {
                if(!parser(code, ref index, out T single) || 
                    index > code.Length) {
                    index = oldIndex;
                    result = default;
                    return false;
                }
                resultAcc.Add(single);
            }
            result = converter(resultAcc.ToArray());
            return true;
        };
    }

    public static Parser<(T, U)> If<T, U>(Parser<T> condP, Parser<U> thenP, Parser<U> elseP) {
        return (string code, ref int index, out (T, U) result) => {
            int oldIndex = index;
            if(condP(code, ref index, out T cond)) {
                if(thenP(code, ref index, out U then)) {
                    result = (cond, then);
                    return true;
                }
            }
            index = oldIndex;
            if(elseP(code, ref index, out U elseResult)) {
                result = (default, elseResult);
                return true;
            }
            result = default;
            return false;
        };
    }
}