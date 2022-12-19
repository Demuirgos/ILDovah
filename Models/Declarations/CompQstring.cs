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
        while(source[index] == '+') {
            index++;
            QSTRING.Parse(ref index, source, false, out QSTRING qstring2);
            qstringList.Add(qstring2);
        }
        compQstring = new CompQstring(qstringList.ToArray());
    }    
}