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