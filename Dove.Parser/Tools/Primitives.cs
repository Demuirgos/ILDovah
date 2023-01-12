using static Core;

public record INT(Int64 Value, int BitsSize, bool IsHex) : IDeclaration<INT>
{
    public override string ToString() => IsHex ? $"0x{Value:X2}" : Value.ToString();
    public static Parser<INT> Body(int baseCount, Parser<char> parser, bool inHex) => RunMany(
        converter: cs => new INT(cs.Aggregate(0l, (acc, c) => acc * baseCount + BYTE.charVal(c)), cs.Length, inHex),
        1, Int32.MaxValue, false, parser);
    public static Parser<INT> AsParser => Map(
        converter: result => result.Item2,
        If(
            condP: ConsumeWord(_ => 0l, "0x"),
            thenP: Body(16, ConsumeIf(Id, BYTE.hexChars.Contains), true),
            elseP: Body(10, ConsumeIf(Id, Char.IsDigit), false)
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
    public static Parser<ID> AsParser => RunAll(
        converter: (vals) => new ID(vals[0]),
        RunMany(
            converter: chars => new string(chars.ToArray()),
            1, Int32.MaxValue, false,
            ConsumeIf(Id, c => Char.IsLetterOrDigit(c) || c == '_' || c == '$' || c == '@' || c == '`' || c == '?')
        )
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
        public static Parser<Collection> AsParser => Map((arr) => new Collection(arr), ARRAY<QSTRING>.MakeParser('\0', '+', '\0'));
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
    public virtual (char start, char separator, char end) Delimiters { get; set; } = ('[', ',', ']');
    public override string ToString() => ToString(Delimiters.separator);
    public new string ToString(char? overrideDelim = null) => ToString($"{overrideDelim ?? Delimiters.separator}");
    public new string ToString(string? overrideDelim = null) => $"{Delimiters.start}{string.Join(overrideDelim, Values.Select(v => v.ToString()))}{Delimiters.end}";

    [Obsolete("Use MakeParser instead", true)]
    public static Parser<ARRAY<T>> AsParser => throw new TypeLoadException("Use MakeParser instead");
    public static Parser<ARRAY<T>> MakeParser(char start, char separator, char end) => RunAll(
        converter: (vals) => new ARRAY<T>(vals[1])
        {
            Delimiters = (start, separator, end)
        },
        start != '\0' ? ConsumeChar(_ => Array.Empty<T>(), start) : Empty<T[]>(),
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
                    0, Int32.MaxValue, false,
                    RunAll(
                        converter: (vals) => vals[1],
                        separator == '\0'
                            ? Empty<T>()
                            : Discard<T, char>(ConsumeChar(Id, separator)),
                        IDeclaration<T>.AsParser
                    )
                ),
                elseP: Empty<T[]>()
            )
        ),
        end != '\0' ? ConsumeChar(_ => Array.Empty<T>(), end) : Empty<T[]>()
    );

    [Obsolete("Use Parse with SpecialCharacters argument instead", true)]
    public static bool Parse(ref int index, string source, out ARRAY<T> arrayVal)
        => throw new TypeLoadException("Use Parse with SpecialCharacters argument instead");

    public static bool Parse(ref int index, string source, out ARRAY<T> arrayVal, (char start, char separator, char end) specialCharacters)
    {
        if (MakeParser(specialCharacters.start, specialCharacters.separator, specialCharacters.end)(source, ref index, out arrayVal))
        {
            return true;
        }
        arrayVal = null;
        return false;
    }
}


