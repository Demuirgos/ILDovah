using System.Text;

public record QSTRING(string Value, bool IsSingleQuoted) {
    public override string ToString() => IsSingleQuoted ? $"'{Value}'" : $"\"{Value}\"";
    public static void Parse(ref int index, string source, bool isSingleQuoted, out QSTRING qstring) {
        char delimiter = isSingleQuoted ? '\'' : '\"';
        StringBuilder sb = new StringBuilder();
        if(source[index] == delimiter) {
            while(source[++index] != delimiter) {
                sb.Append(source[index]);
            }
            qstring = new QSTRING(sb.ToString(), isSingleQuoted);
            return;
        }
        qstring = null;
    }
}

public record CompQstring(QSTRING[] AggregatedStrings) : Decl {
    public override string ToString()
    {
        return AggregatedStrings.Aggregate(String.Empty, (acc, s) => $"{acc} + s.Value");
    }

    public static void Parse(ref int index, string source, out CompQstring compQstring)
    {
        List<QSTRING> qstringList = new();
        QSTRING.Parse(ref index, source, false, out QSTRING qstring);
        qstringList.Add(qstring);
        while(source.ConsumeWord(ref index, "+")) {
            QSTRING.Parse(ref index, source, false, out QSTRING qstring2);
            qstringList.Add(qstring2);
        }
        compQstring = new CompQstring(qstringList.ToArray());
    }    
}