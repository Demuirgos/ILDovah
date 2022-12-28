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

public record FLOAT(float Value) : IDeclaration<FLOAT> {
    public override string ToString() => Value.ToString();
    public static Parser<FLOAT> AsParser => RunAll(
        converter: (vals) => new FLOAT(float.Parse(vals[0] + "." + vals[2])),
        RunMany(1, Int32.MaxValue, ConsumeIf(Char.IsDigit, Id), chars => new string(chars.ToArray())),
        ConsumeChar('.', character => $"{character}"),
        RunMany(1, Int32.MaxValue, ConsumeIf(Char.IsDigit, Id), chars => new string(chars.ToArray()))
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
        boolValues.Select(x => ConsumeWord(x, (_) => new BOOL(bool.Parse(x)))).ToArray()
    );
    public static bool Parse(ref int index, string source, out BOOL byteval) {
        if(BOOL.AsParser(source, ref index, out byteval)) {
            return true;
        }
        byteval = null;
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
            0, Int32.MaxValue, TryRun(
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


