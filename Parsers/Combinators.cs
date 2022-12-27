using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public static class Core {
    public static T Id<T>(T value) => value;
    public delegate bool Parser<T>(string source, ref int index, [NotNullWhen(true)] out T result);
    public static Parser<T> Empty<T>() => (string code, ref int index, out T result) => {
        result = default;
        return true;
    };

    public static Parser<T> ConsumeIf<T>(Func<char, bool> predicate, Func<char, T> converter) {
        return (string code, ref int index, out T result) => {
            result = default;
            if(predicate(code[index])) {
                index++;
                result = converter(code[index - 1]);
                return true;
            }
            return false;
        };
    }

    public static Parser<T> ConsumeChar<T>(char c, Func<char, T> converter) {
        return (string code, ref int index, out T result) => {
            bool isParsed = ConsumeIf<char>(x => x == c, (id) => id)
                                         (code,  ref index, out char charC);
            if(isParsed) {
                result = converter(charC);
            }
            else {
                result = default;
            }

            return isParsed;
        };
    }

    public static Parser<T> ConsumeWord<T>(string word, Func<string, T> converter) {
        return (string code, ref int index, out T result) => {
            StringBuilder resultAcc = new();
            foreach (char c in word)
            {
                if(!ConsumeChar(c, Id)(code, ref index, out char character)) {
                    result = default;
                    return false;
                }
                resultAcc.Append(character);
            }
            result = converter(resultAcc.ToString());
            return true;
        };
    }

    public static Parser<T> TryRun<T>(params Parser<T>[] parsers) {
        return (string code, ref int index, out T result) => {
            int oldIndex = index;
            result = default;
            foreach (Parser<T> parser in parsers)
            {
                if(parser(code, ref index, out result)) {
                    return true;
                }
                index = oldIndex;
            }
            return false;
        };
    }

    public static Parser<U> RunMany<T, U>(int min, int max, Parser<T> parser, Func<T[], U> converter) {
        return (string code, ref int index, out U result) => {
            int oldIndex = index;
            var resultAcc = new List<T>();
            for (int i = 0; i < max && index < code.Length; i++)
            {
                if(!parser(code, ref index, out T single)) {
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
}