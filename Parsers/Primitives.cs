using System.Text;
using System.Linq;
using static Core;
public record INT(Int64 Value, int ByteCount) : IDeclaration<INT> {
    public override string ToString() => Value.ToString();
    public static Parser<INT> AsParser => RunMany(1, Int32.MaxValue, ConsumeIf(Char.IsDigit, Id), chars => new INT(Int64.Parse(new string(chars.ToArray())), chars.Length));
    public static bool Parse(ref int index, string source, out INT intVal) {
        if(INT.AsParser(source, ref index, out intVal)) {
            return true;
        }
        intVal = null;
        return false;
    }
}

public record FLOAT(float Value, bool IsCast) : IDeclaration<FLOAT> {
    public override string ToString() => IsCast ? $"float64({Value})" : Value.ToString();
    public static Parser<FLOAT> CastParser => TryRun (
            converter: (val) => new FLOAT((float)val, true),
            new[] {"float64", "float32"}.Select(castWord => {
                return RunAll(
                    converter: (vals) => vals[2],
                    ConsumeWord(castWord, _ => 0l),
                    ConsumeChar('(', _ => 0l),
                    Map(INT.AsParser, (intVal) => intVal.Value),
                    ConsumeChar(')', _ => 0l)
                );
            }).ToArray()
        );
    private static Parser<FLOAT> StraightParser => RunAll(
        converter: (vals) => new FLOAT(float.Parse(vals[0] + "." + vals[2]), false),
        RunMany(1, Int32.MaxValue, ConsumeIf(Char.IsDigit, Id), chars => new string(chars.ToArray())),
        ConsumeChar('.', _ => String.Empty),
        RunMany(1, Int32.MaxValue, ConsumeIf(Char.IsDigit, Id), chars => new string(chars.ToArray()))
    );
    public static Parser<FLOAT> AsParser => TryRun(Id,
        CastParser,
        StraightParser
    );

    public static bool Parse(ref int index, string source, out FLOAT floatVal) {
        if(FLOAT.AsParser(source, ref index, out floatVal)) {
            return true;
        }
        floatVal = null;
        return false;
    }
}

public record BYTE(byte Value) : IDeclaration<BYTE> {
    public static byte charVal(char c) {
        if(c >= '0' && c <= '9') return (byte)(c - '0');
        if(c >= 'A' && c <= 'F') return (byte)(c - 'A' + 10);
        if(c >= 'a' && c <= 'f') return (byte)(c - 'a' + 10);
        return 0;
    } 
    private static char[] hexChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f' };
    public static Parser<BYTE> AsParser => RunMany(
        2, 2, ConsumeIf(x => hexChars.Contains(x), charVal),
        (bytes) => new BYTE((byte)(bytes[0] * 16 + bytes[1]))
    );

    public override string ToString() => Value.ToString();
    public static bool Parse(ref int index, string source, out BYTE byteval) {
        if(BYTE.AsParser(source, ref index, out byteval)) {
            return true;
        }
        byteval = null;
        return false;
    }
}

public record BOOL(bool Value) : IDeclaration<BOOL> {
    public override string ToString() => Value.ToString();
    private static string[] boolValues = { "true", "false" };
    public static Parser<BOOL> AsParser => TryRun(
        (result) => new BOOL(bool.Parse(result)),
        boolValues.Select(x => ConsumeWord(x, Id)).ToArray()
    );
    public static bool Parse(ref int index, string source, out BOOL byteval) {
        if(BOOL.AsParser(source, ref index, out byteval)) {
            return true;
        }
        byteval = null;
        return false;
    }
}

public record QSTRING(String Value, bool IsSingleyQuoted) : IDeclaration<QSTRING> {
    public override string ToString() => $"\"{Value}\"";
    public static Parser<QSTRING> AsParser => TryRun(
        converter: Id,
        new[] {'"', '\''}.Select(quotationChar=>  
            RunAll(
                converter: (vals) => new QSTRING(vals[1], quotationChar == '\''),
                ConsumeChar(quotationChar, (_) => String.Empty),
                RunMany(1, Int32.MaxValue, ConsumeIf(c => c != quotationChar, Id), chars => new string(chars.ToArray())),
                ConsumeChar(quotationChar, (_) => String.Empty)
                )
        ).ToArray()
    );
    public static bool Parse(ref int index, string source, out QSTRING idVal) {
        if(QSTRING.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
}

public record ARRAY<T>(T[] Values) : IDeclaration<ARRAY<T>> where T : IDeclaration<T> {
    public override string ToString() => $"[{string.Join(", ", Values.Select(v => v.ToString()))}]";
    public static Parser<T> UnitParser => (Parser<T>)typeof(T).GetProperty("AsParser").GetValue(null); 
    public static Parser<ARRAY<T>> AsParser => throw new NotImplementedException();

    public static Parser<ARRAY<T>> MakeParser((char start, char separator, char end) specialCharacters) => RunAll(
        converter: (vals) => new ARRAY<T>(vals[1]),
        ConsumeChar(specialCharacters.start, _ => Array.Empty<T>()),
        RunMany(
            0, Int32.MaxValue, TryRun(Id,
                UnitParser, 
                ConsumeChar(specialCharacters.separator, _ => default(T))
            ),
            converter: (values) => values.Where(item => !EqualityComparer<T>.Default.Equals(item , default(T))).Select(x => (T)x).ToArray()
        ),
        ConsumeChar(specialCharacters.end, _ => Array.Empty<T>())
    );

    public static bool Parse(ref int index, string source, out ARRAY<T> arrayVal)
        => throw new NotImplementedException();

    
    public static bool Parse(ref int index, string source, out ARRAY<T> arrayVal, (char start, char separator, char end) specialCharacters) {
        if(MakeParser(specialCharacters)(source, ref index, out arrayVal)) {
            return true;
        }
        arrayVal = null;
        return false;
    }
}  


