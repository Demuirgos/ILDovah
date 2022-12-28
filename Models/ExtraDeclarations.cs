using System.Text;
using static Core;

public record ExternSource(INT Line, INT? Column, QSTRING? File) : IDeclaration<ExternSource> {
    public override string ToString() {
        StringBuilder sb = new();
        sb.Append($".line {Line}");
        if(Column is not null) {
            sb.Append($" : {Column.Value}");
        }
        if(File is not null) {
            sb.Append($" '{File.Value}'");
        }
        return sb.ToString();
    }
    public static Parser<ExternSource> AsParser => RunAll(
        converter: (vals) => new ExternSource(vals[1].Line, vals[2].Column, vals[3].File),
        ConsumeWord(".line", (_) => new ExternSource(null, null, null)),
        Map(INT.AsParser, (line) => new ExternSource(line, null, null)),
        TryRun(
            converter: (vals) => new ExternSource(null, vals, null),
            RunAll(
                converter: (vals) => vals[1],
                ConsumeChar(':', (_) => default(INT)),
                Map(INT.AsParser, (column) => column)
            ),
            Empty<INT>()
        ),
        TryRun((name) => new ExternSource(null, null, name), QSTRING.AsParser, Empty<QSTRING>())
    );
    public static bool Parse(ref int index, string source, out ExternSource idVal) {
        if(ExternSource.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
}