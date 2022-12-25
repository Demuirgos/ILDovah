public record ExtSourceSpecDecl(INT line, INT? column, QSTRING str) : Decl {

    public override string ToString() {
        return $".line {line}{(column is null ? String.Empty : $":{column}")}{str}";
    }
    public static bool Parse(ref int index, string source, out ExtSourceSpecDecl extSourceSpec) {
        if(source.ConsumeWord(ref index, ".line")) {
            INT? column = null;
            INT.Parse(ref index, source, out INT line);
            if(source.ConsumeWord(ref index, ":")) {
                INT.Parse(ref index, source, out column);
            } 
            QSTRING.Parse(ref index, source, true, out QSTRING str);
            extSourceSpec = new ExtSourceSpecDecl(line, column, str);
            return true;
        } else {
            extSourceSpec = null;
            return false;
        }
    }
}