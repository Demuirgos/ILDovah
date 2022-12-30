using static Core;
public record INT(Int64 Value, int ByteCount) : IDeclaration<INT> {
    public override string ToString() => Value.ToString();
    public static Parser<INT> AsParser => RunMany(
        converter: chars => {
            Console.WriteLine($"chars: {new string(chars.ToArray())}");
            return new INT(Int64.Parse(new string(chars.ToArray())), chars.Length);
        },
        1, Int32.MaxValue, ConsumeIf(Id, Char.IsDigit)
    );
}

public record FLOAT(double Value, bool IsCast) : IDeclaration<FLOAT> {
    public override string ToString() => IsCast ? $"float64({Value})" : Value.ToString();
    public static Parser<FLOAT> CastParser => TryRun (
            converter: (val) => new FLOAT((float)val, true),
            new[] {"float64", "float32"}.Select(castWord => {
                return RunAll(
                    converter: (vals) => vals[2],
                    ConsumeWord(_ => 0l, castWord),
                    ConsumeChar(_ => 0l, '('),
                    Map((intVal) => intVal.Value, INT.AsParser),
                    ConsumeChar(_ => 0l, ')')
                );
            }).ToArray()
        );
    private static Parser<FLOAT> StraightParser => RunAll(
        converter: (vals) => new FLOAT(double.Parse($"{vals[0]}.{vals[2]}"), false),
        Map((intVal) => intVal.Value, INT.AsParser),
        ConsumeChar(_ => 0l, '.'),
        Map((intVal) => intVal.Value, INT.AsParser)
    );
    public static Parser<FLOAT> AsParser => TryRun(Id,
        CastParser,
        StraightParser
    );
}

public record BYTE(byte Value) : IDeclaration<BYTE> {
    public override string ToString() => Convert.ToHexString(new byte[] { Value });
    public static byte charVal(char c) {
        if(c >= '0' && c <= '9') return (byte)(c - '0');
        if(c >= 'A' && c <= 'F') return (byte)(c - 'A' + 10);
        if(c >= 'a' && c <= 'f') return (byte)(c - 'a' + 10);
        return 0;
    } 
    private static char[] hexChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f' };
    public static Parser<BYTE> AsParser => RunMany(
        converter: (bytes) => new BYTE((byte)(bytes[0] * 16 + bytes[1])),
        2, 2, ConsumeIf(converter: charVal, x => hexChars.Contains(x))
    );

}

public record ID(String Value) : IDeclaration<ID> {
    public override string ToString() => Value;
    public static Parser<ID> AsParser => RunAll(
        converter: (vals) => new ID(vals[0]),
        RunMany(
            converter:chars => new string(chars.ToArray()),
            1, Int32.MaxValue, ConsumeIf(Id, c => Char.IsLetterOrDigit(c) || c == '_')
        )
    );
}

public record BOOL(bool Value) : IDeclaration<BOOL> {
    public override string ToString() => Value.ToString();
    private static string[] boolValues = { "true", "false" };
    public static Parser<BOOL> AsParser => TryRun(
        converter:(result) => new BOOL(bool.Parse(result)),
        boolValues.Select(x => ConsumeWord(Id, x)).ToArray()
    );
}

public record QSTRING(String Value, bool IsSingleyQuoted) : IDeclaration<QSTRING> {
    public override string ToString() => $"\"{Value}\"";
    public static Parser<QSTRING> AsParser => TryRun(
        converter: Id,
        new[] {'"', '\''}.Select(quotationChar=>  
            RunAll(
                converter: (vals) => new QSTRING(vals[1], quotationChar == '\''),
                ConsumeChar((_) => String.Empty, quotationChar),
                RunMany (
                    converter: chars => new string(chars.ToArray()),
                    1, Int32.MaxValue, ConsumeIf(Id, c => c != quotationChar)
                ),
                ConsumeChar((_) => String.Empty, quotationChar)
                )
        ).ToArray()
    );
}

public record ARRAY<T>(T[] Values) : IDeclaration<ARRAY<T>> where T : IDeclaration<T> {
    public (char start, char separator, char end) Delimiters = ('[', ',', ']');
    public override string ToString() => $"{Delimiters.start}{string.Join(Delimiters.separator, Values.Select(v => v.ToString()))}{Delimiters.end}";
    public static Parser<ARRAY<T>> AsParser => throw new NotImplementedException();
    public static Parser<ARRAY<T>> MakeParser(char start, char separator, char end) => RunAll(
        converter: (vals) => new ARRAY<T>(vals[1]) {
            Delimiters = (start, separator, end)
        },
        ConsumeChar(_ => Array.Empty<T>(), start),
        RunMany(
            converter: (values) => values.Where(item => !EqualityComparer<T>.Default.Equals(item , default(T)))
                                             .Select(x => (T)x)
                                             .ToArray(),
            0, Int32.MaxValue, TryRun(Id,
                IDeclaration<T>.AsParser, 
                ConsumeChar(_ => default(T), separator)
            )
        ),
        ConsumeChar(_ => Array.Empty<T>(), end)
    );

    public static bool Parse(ref int index, string source, out ARRAY<T> arrayVal)
        => throw new NotImplementedException();
    
    public static bool Parse(ref int index, string source, out ARRAY<T> arrayVal, (char start, char separator, char end) specialCharacters) {
        if(MakeParser(specialCharacters.start, specialCharacters.separator, specialCharacters.end)(source, ref index, out arrayVal)) {
            return true;
        }
        arrayVal = null;
        return false;
    }
}  


