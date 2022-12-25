using System.Text;
using System.Linq;
public record INT(Int64 Value, int ByteCount) {
    public override string ToString() => Value.ToString();
    public static void Parse(ref int index, string source, out INT intVal) {
        if(Char.IsDigit(source[index])) {
            List<int> sb = new();
            while(Char.IsDigit(source[index])) {
                sb.Add(source[index++] - '0');
            }
            long acc = 0;
            for(int i = 0; i < sb.Count; i++) {
                acc = acc * 10 + sb[i];
            }
            intVal = new INT(acc, sb.Count);
            return;
        }
        intVal = null;
    }
}

public record FLOAT(float Value) {
    public override string ToString() => Value.ToString();
    public static void Parse(ref int index, string source, out FLOAT floatVal) {
        if(Char.IsDigit(source[index])) {
            List<int> postDot = new();
            List<int> preDot = new();
            while(Char.IsDigit(source[index]) || source[index] == '.') {
                if(source[index] == '.') {
                    index++;
                    while(Char.IsDigit(source[index])) {
                        postDot.Add(source[index++] - '0');
                    }
                    postDot.Reverse();
                    break;
                } else {
                    preDot.Add(source[index++] - '0');
                }
            }

            float dacc = preDot.Aggregate(0f, (acc, i) => (acc * (float)10) + i);
            float facc = postDot.Aggregate(0f, (acc, i) => (acc / (float)10) + i);
            floatVal = new FLOAT(facc + dacc);
            return;
        }
        floatVal = null;
    }
}

public record BYTE(byte Value) {
    public static byte charVal(char c) {
        if(c >= '0' && c <= '9') return (byte)(c - '0');
        if(c >= 'A' && c <= 'F') return (byte)(c - 'A' + 10);
        if(c >= 'a' && c <= 'f') return (byte)(c - 'a' + 10);
        return 0;
    } 
    public override string ToString() => Value.ToString();
    public static void Parse(ref int index, string source, out BYTE byteval) {
        char[] hexChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f' };
        if(hexChars.Contains(source[index]) && hexChars.Contains(source[index + 1])) {
            int b = charVal(source[index]) * 16 + charVal(source[index + 1]);
            byteval = new BYTE((byte)b);
            return;
        }
        byteval = null;
    }
}


public record BOOL(bool Value) {
    public override string ToString() => Value.ToString();
    public static void Parse(ref int index, string source, out BOOL byteval) {
        String[] boolValue = { "true", "false" };
        if(source[index..].StartsWith(boolValue, out string word)) {
            byteval = new BOOL(word == "true");
            index += word.Length;
            return;
        }
        byteval = null;
    }
}


