using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public static class Core
{
    public static T Id<T>(T value) => value;
    public delegate bool Parser<T>(string source, ref int index, [NotNullWhen(true)] out T result, [NotNullWhen(false)] out string error);

    public static Parser<T> Trim<T>(Parser<T> parser)
    {
        Parser<T> Whitespace<T>() => (string code, ref int index, out T result, out string error) =>
        {
            result = default; error = default;
            while (index < code.Length && Char.IsWhiteSpace(code[index]))
            {
                index++;
            }
            return true;
        };

        return RunAll(
            ps => ps[1], false,
            Whitespace<T>(),
            parser,
            Whitespace<T>()
        );
    }

    public static Parser<T> Empty<T>() => (string code, ref int index, out T result, out string error) =>
    {
        result = default; 
        error = default;
        return true;
    };

    public static Parser<T> Discard<T, U>(Parser<U> parser)
    {
        return (string code, ref int index, out T result, out string error) =>
        {
            bool isParsed = parser(code, ref index, out U value, out error);
            result = default(T);
            return isParsed;
        };
    }

    public static Parser<T> Lazy<T>(Func<Parser<T>> parser)
    {
        return (string code, ref int index, out T result, out string error) =>
        {
            return parser()(code, ref index, out result, out error);
        };
    }

    public static Parser<T> Fail<T>(string? message = null)
    {
        return (string code, ref int index, out T result, out string error) =>
        {
            result = default;
            error = message ?? "This parser should never be reached";
            return false;
        };
    }

    public static Parser<T> Cast<T, U>(Parser<U> parser)
    {
        return Map(item => (T)((object)item), parser);
    }

    public static Parser<T> Map<T, U>(Func<U, T> converter, Parser<U> parser)
    {
        return (string code, ref int index, out T result, out string error) =>
        {
            bool isParsed = parser(code, ref index, out U value, out error);
            result = isParsed ? converter(value) : default;
            error = isParsed ? default : $"{typeof(U).Name}->{typeof(T).Name} : {error}";
            return isParsed;
        };
    }

    public static Parser<T> ConsumeIf<T>(Parser<T> parser, Func<T, bool> predicate, [CallerArgumentExpression("predicate")] string predicateArgs = null)
    {
        return (string code, ref int index, out T result, out string error) =>
        {
            result = default;
            bool isParsed = parser(code, ref index, out T value, out error);
            if (isParsed)
            {
                bool isValid = predicate(value);
                if (isValid)
                {
                    result = value;
                    return true;
                }
            }
            error = isParsed ? $"Value: {value} does not match predicate {predicateArgs}" : error;
            result = default(T);
            return false;
        };
    }

    public static Parser<T> ConsumeIf<T>(Func<char, T> converter, Func<char, bool> predicate,  [CallerArgumentExpression("predicate")] string predicateArgs = null)
    {
        return (string code, ref int index, out T result, out string error) =>
        {
            result = default; error = default;
            if (index < code.Length && predicate(code[index]))
            {
                result = converter(code[index]);
                index++;
                return true;
            }
            (var start, var end, index) = (index < 25 ? 0 : index - 25, index + 25 > code.Length - 1 ? code.Length - 1 : index + 25, index > code.Length - 1 ? code.Length - 1 : index);
            error = $"Character c: {code[index]} at index : {index} does not match predicate {predicateArgs} [{code[start..end]}]";
            return false;
        };
    }

    public static Parser<T> ConsumeChar<T>(Func<char, T> converter, char c)
    {
        return (string code, ref int index, out T result, out string error) =>
        {
            bool isParsed = ConsumeIf(Id, x => x == c)
                                     (code, ref index, out char charC, out error);
            if (isParsed)
            {
                result = converter(charC);
            }
            else
            {
                result = default;
            }

            return isParsed;
        };
    }

    public static Parser<T> ConsumeWord<T>(Func<string, T> converter, string word)
    {
        return (string code, ref int index, out T result, out string error) =>
        {
            StringBuilder resultAcc = new();
            int oldIndex = index;
            error = default;
            foreach (char c in word)
            {
                if (!ConsumeChar(Id, c)(code, ref index, out char character, out _))
                {
                    result = default;
                    error = $"Failed to parse word: {word} at index: {oldIndex} at [{code[(index < 25 ? 0 : index - 25)..(index+25 >= code.Length ? code.Length - 1: index + 25)]}]";
                    return false;
                }
                resultAcc.Append(character);
            }
            result = converter(resultAcc.ToString());
            return true;
        };
    }

    public static Parser<U> TryRun<T, U>(Func<T, U> converter, params Parser<T>[] parsers)
    {
        string ConstructError(string sourceT, string targetT, string error, int index)
        {
                return $"\tParser of type {(sourceT != targetT ? $"{sourceT}->{targetT}" : $"{sourceT}")} failed with error: \n\t\t{error.Replace("\n", "\n\t\t")} at index: {index}\n";
        }
        return (string code, ref int index, out U result, out string error) =>
        {
            int oldIndex = index;
            result = default;
            error = default;
            StringBuilder errorAcc = new();
            errorAcc.Append($"Failed to parse {typeof(U).Name} at index: {index}\n{{");
            foreach (Parser<T> parser in parsers)
            {
                if (parser(code, ref index, out T subResult, out error))
                {
                    result = converter(subResult);
                    return true;
                }
                errorAcc.Append(ConstructError(typeof(T).Name, typeof(U).Name, error, index));
                index = oldIndex;
            }
            errorAcc.Append("}");
            error = errorAcc.ToString();
            return false;
        };
    }

    public static Parser<U> RunMany<T, U>(Func<T[], U> converter, int min, int max, bool skipWs, Parser<T> parser)
    {
        return (string code, ref int index, out U result, out string error) =>
        {
            int oldIndex = index;
            error = default;
            var resultAcc = new List<T>();
            for (int i = 0; i < max; i++)
            {
                var parserToUse = skipWs ? Trim(parser) : parser;
                if (!parserToUse(code, ref index, out T single, out error) || index > code.Length)
                {
                    if (i < min)
                    {
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
    public static Parser<U> RunAll<T, U>(Func<T[], U> converter, params Parser<T>[] parsers)
        => RunAll(converter, true, parsers);
    public static Parser<U> RunAll<T, U>(Func<T[], U> converter, bool skipWhitespace, params Parser<T>[] parsers)
    {
        return (string code, ref int index, out U result, out string error) =>
        {
            int oldIndex = index;
            error = default;
            var resultAcc = new List<T>();
            foreach (Parser<T> parser in parsers)
            {
                var parserToUse = skipWhitespace ? Trim(parser) : parser;
                if (!parserToUse(code, ref index, out T single, out error) ||
                    index > code.Length)
                {
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

    public static Parser<(T, U)> If<T, U>(Parser<T> condP, Parser<U> thenP, Parser<U> elseP)
    {
        return (string code, ref int index, out (T, U) result, out string error) =>
        {
            int oldIndex = index;
            (string errorC, string errorT, string errorE) = (default, default, default);
            error = default;
            if (Trim(condP)(code, ref index, out T cond, out errorC))
            {
                if (thenP(code, ref index, out U then, out errorT))
                {
                    result = (cond, then);
                    return true;
                }
            }
            index = oldIndex;
            if (elseP(code, ref index, out U elseResult, out errorE))
            {
                result = (default, elseResult);
                return true;
            }
            error = $"Failed to parse if at index: {oldIndex} with errors: {errorC}, {errorT}, {errorE}";
            result = default;
            return false;
        };
    }
}