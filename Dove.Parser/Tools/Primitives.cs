using static Core;

public record INT(Int64 Value, int BitsSize, bool IsHex) : IDeclaration<INT>
{
    public override string ToString() => IsHex ? $"0x{Value:X2}" : Value.ToString();
    public static Parser<INT> Body(int baseCount, Parser<char> parser, bool inHex, int sign = 1) => RunMany(
        converter: cs => new INT(cs.Aggregate(0, (acc, c) => acc * baseCount + sign * BYTE.charVal(c)), cs.Length, inHex),
        1, Int32.MaxValue, false, parser);
    public static Parser<INT> AsParser => Map(
        converter: result => result.Item2,
        If(
            condP: ConsumeWord(_ => 0l, "0x"),
            thenP: Body(16, ConsumeIf(Id, BYTE.hexChars.Contains), true),
            elseP: Map(
                converter: (val) => val.Item2,
                If(
                    condP: ConsumeChar(Id, '-'),
                    thenP: Body(10, ConsumeIf(Id, Char.IsDigit), false, -1),
                    elseP: Body(10, ConsumeIf(Id, Char.IsDigit), false)
                )
            )
        )
    );
}
public record FLOAT(double Value, int BitSize, bool IsCast) : IDeclaration<FLOAT>
{
    public override string ToString() => IsCast ? $"float64({Value})" : Value.ToString();
    public static Parser<FLOAT> CastParser => TryRun(
            converter: (val) => new FLOAT(val.Item2, val.Item1, true),
            new[] { "float64", "float32" }.Select(castWord =>
            {
                return RunAll(
                    converter: (vals) => (castWord == "float64" ? 64 : 32, vals[2]),
                    ConsumeWord(_ => 0l, castWord),
                    ConsumeChar(_ => 0l, '('),
                    Map((intVal) => intVal.Value, INT.AsParser),
                    ConsumeChar(_ => 0l, ')')
                );
            }).ToArray()
        );
    private static Parser<FLOAT> StraightParser => RunAll(
        converter: (vals) => new FLOAT(double.Parse($"{vals[0]}.{vals[2]}"), 64, false),
        Map((intVal) => intVal.Value, INT.AsParser),
        ConsumeChar(_ => 0l, '.'),
        Map((intVal) => intVal.Value, INT.AsParser)
    );
    public static Parser<FLOAT> AsParser => TryRun(Id,
        CastParser,
        StraightParser
    );

    public static explicit operator FLOAT(INT value) => new FLOAT((double)value.Value, value.BitsSize, false);
}

public record BYTE(byte Value) : IDeclaration<BYTE>
{
    public override string ToString() => Convert.ToHexString(new byte[] { Value });
    public static byte charVal(char c)
    {
        if (c >= '0' && c <= '9') return (byte)(c - '0');
        if (c >= 'A' && c <= 'F') return (byte)(c - 'A' + 10);
        if (c >= 'a' && c <= 'f') return (byte)(c - 'a' + 10);
        return 0;
    }
    internal static char[] hexChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f' };
    public static Parser<BYTE> AsParser => RunMany(
        converter: (bytes) => new BYTE((byte)(bytes[0] * 16 + bytes[1])),
        2, 2, false,
        ConsumeIf(converter: charVal, x => hexChars.Contains(x))
    );

}

/*
a contiguous string of characters which starts with either an alphabetic character (A–Z, a–z) or
one of “_”, “$”, “@”, “`” (grave accent), or “?”, and is followed by any number of alphanumeric
characters (A–Z, a–z, 0–9) or the characters “_”, “$”, “@”, “`” (grave accent), and “?”. 
*/
public record ID(String Value) : IDeclaration<ID>
{
    public override string ToString() => Value;
    public static Parser<ID> AsParser => ConsumeIf(
        RunAll(
            converter: (vals) => new ID(vals[0]),
            RunMany(
                converter: chars => new string(chars.ToArray()),
                1, Int32.MaxValue, false,
                ConsumeIf(Id, c => Char.IsLetterOrDigit(c) || c == '_' || c == '$' || c == '@' || c == '`' || c == '?')
            )
        ),
        word => !IdentifierDecl.Identifier.reservedWords.Contains(word.Value)
    );
}

public record BOOL(bool Value) : IDeclaration<BOOL>
{
    public override string ToString() => Value.ToString();
    private static string[] boolValues = { "true", "false" };
    public static Parser<BOOL> AsParser => TryRun(
        converter: (result) => new BOOL(bool.Parse(result)),
        boolValues.Select(x => ConsumeWord(Id, x)).ToArray()
    );
}

public record QSTRING(String Value, bool IsSingleyQuoted) : IDeclaration<QSTRING>
{
    public record Collection(ARRAY<QSTRING> Values) : IDeclaration<Collection>
    {
        public override string ToString() => Values.ToString("+");
        public static Parser<Collection> AsParser => Map((arr) => new Collection(arr), ARRAY<QSTRING>.MakeParser(new ARRAY<QSTRING>.ArrayOptions {
            Delimiters = ('\0', '+', '\0') 
        }));
    }
    public override string ToString()
    {
        char quotationChar = IsSingleyQuoted ? '\'' : '"';
        return $"{quotationChar}{Value}{quotationChar}";
    }
    public static Parser<QSTRING> AsParser => TryRun(
        converter: Id,
        new[] { '"', '\'' }.Select(quotationChar =>
            RunAll(
                converter: (vals) => new QSTRING(vals[1], quotationChar == '\''),
                ConsumeChar((_) => String.Empty, quotationChar),
                RunMany(
                    converter: chars => new string(chars.ToArray()),
                    0, Int32.MaxValue, false,
                    ConsumeIf(Id, c => c != quotationChar)
                ),
                ConsumeChar((_) => String.Empty, quotationChar)
            )
        ).ToArray()
    );
}

public record ARRAY<T>(T[] Values) : IDeclaration<ARRAY<T>> where T : IDeclaration<T>
{
    public class ArrayOptions {
        public (char start, char separator, char end) Delimiters { get; set; } = ('[', ',', ']');
        public bool AllowEmpty { get; set; } = true;
        public bool SkipWhitespace { get; set; } = true;
        public int MinLength { get; set; } = 1;
        public int MaxLength { get; set; } = Int32.MaxValue;

        
    }

    public ArrayOptions Options { get; set; } = new ArrayOptions();

    public override string ToString() => ToString(Options.Delimiters.separator);
    public new string ToString(char? overrideDelim = null) {
        char delim = overrideDelim ?? Options.Delimiters.separator;
        return ToString($"{(delim == '\0' ? String.Empty : $"{delim}")}");
    } 
    public new string ToString(string? overrideDelim = null) => $"{(Options.Delimiters.start == '\0' ? String.Empty : $"{Options.Delimiters.start}")}{string.Join(overrideDelim, Values.Select(v => v.ToString()))}{(Options.Delimiters.end == '\0' ? String.Empty : $"{Options.Delimiters.end}")}";

    [Obsolete("Use MakeParser instead", true)]
    public static Parser<ARRAY<T>> AsParser => throw new TypeLoadException("Use MakeParser instead");
    public static Parser<ARRAY<T>> MakeParser(ArrayOptions options) => RunAll(
        converter: (vals) => new ARRAY<T>(vals[1])
        {
            Options = options
        },
        options.Delimiters.start != '\0' ? ConsumeChar(_ => Array.Empty<T>(), options.Delimiters.start) : Empty<T[]>(),
        Map(
            converter: results =>
            {
                if (results.Item1 is null) return Array.Empty<T>();
                return results.Item1.Concat(results.Item2).ToArray();
            },
            If(
                condP: Map(val => new T[] { val }, IDeclaration<T>.AsParser),
                thenP: RunMany(
                    converter: (vals) => vals,
                    options.MinLength - 1, options.MaxLength, options.SkipWhitespace,
                    RunAll(
                        converter: (vals) => vals[1],
                        options.SkipWhitespace,
                        options.Delimiters.separator == '\0'
                            ? Empty<T>()
                            : Discard<T, char>(ConsumeChar(Id, options.Delimiters.separator)),
                        IDeclaration<T>.AsParser
                    )
                ),
                elseP: options.AllowEmpty ? Empty<T[]>() :  Fail<T[]>()
            )
        ),
        options.Delimiters.end != '\0' ? ConsumeChar(_ => Array.Empty<T>(), options.Delimiters.end) : Empty<T[]>()
    );

    [Obsolete("Use Parse with SpecialCharacters argument instead", true)]
    public static bool Parse(ref int index, string source, out ARRAY<T> arrayVal)
        => throw new TypeLoadException("Use Parse with SpecialCharacters argument instead");

    public static bool Parse(ref int index, string source, out ARRAY<T> arrayVal, out string error, ArrayOptions option)
    {
        if (MakeParser(option)(source, ref index, out arrayVal, out error))
        {
            return true;
        }
        arrayVal = null;
        return false;
    }
}


