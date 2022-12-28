using System.Text;
using static Core;

public record FileName(String Name) : IDeclaration<FileName> {
    public override string ToString() => Name;
    public static Parser<FileName> AsParser => Map((name) => new FileName(name.Value), DottedName.AsParser);
}

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
        ConsumeWord((_) => new ExternSource(null, null, null), ".line"),
        Map((line) => new ExternSource(line, null, null), INT.AsParser),
        TryRun(
            converter: (vals) => new ExternSource(null, vals, null),
            RunAll(
                converter: (vals) => vals[1],
                ConsumeChar((_) => default(INT), ':'),
                Map((column) => column, INT.AsParser)
            ),
            Empty<INT>()
        ),
        TryRun((name) => new ExternSource(null, null, name), QSTRING.AsParser, Empty<QSTRING>())
    );
}