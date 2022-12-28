using System.Text;
using static Core;

public record CustomAttribute(MethodDeclaration AttributeCtor, ARRAY<BYTE>? Arguments) : IDeclaration<CustomAttribute> {
    /*Ctor [ ‘=’ ‘(’ Bytes ‘)’ ]*/
    public override string ToString() {
        StringBuilder sb = new();
        sb.Append(AttributeCtor);
        if(Arguments is not null) {
            sb.Append($" = ({Arguments})");
        }
        return sb.ToString();
    }
    public static Parser<CustomAttribute> AsParser => RunAll(
        converter: (vals) => {
            if(vals[0].AttributeCtor.IsConstructor == false)
                throw new Exception("Custom attribute must be a constructor");
            return new CustomAttribute(vals[0].AttributeCtor, vals[1].Arguments);
        },
        Map((methname) => new CustomAttribute(methname, null), MethodDeclaration.AsParser),
        TryRun(
            converter: (vals) => new CustomAttribute(null, vals),
            RunAll(
                converter: (vals) => vals[1],
                ConsumeChar((_) => default(ARRAY<BYTE>), '='),
                Map((bytes) => bytes, ARRAY<BYTE>.MakeParser('(', '\0', ')'))
            ),
            Empty<ARRAY<BYTE>>()
        )
    );
}

public record ClassAttribute(String[] Attribute) : IDeclaration<ClassAttribute> {
    public override string ToString() => String.Join(' ', Attribute);
    private static String[] AttributeWords = {"public", "private", "value", "enum", "interface", "sealed", "abstract", "auto", "sequential", "explicit", "ansi", "unicode", "autochar", "import", "serializable", "nested", "beforefieldinit", "specialname", "rtspecialname"};
    private static String[] NestedWords = {"public", "private", "family", "assembly", "famandassem", "famorassem"};
    public static Parser<ClassAttribute> AsParser => RunMany(
        converter: (vals) => new ClassAttribute(vals),
        0, Int32.MaxValue,
        TryRun(
            converter: Id,
            AttributeWords.Select((word) => {
            if(word != "nested")
                return ConsumeWord(Id, word);
            else {
                return RunAll(
                    converter: (vals) => $"{vals[0]} {vals[1]}",
                    ConsumeWord(Id, "nested"),
                    TryRun(
                        converter: Id,
                        NestedWords.Select((nestedWord) => ConsumeWord(Id, nestedWord)).ToArray()
                    )
                );
            }
        }).ToArray())
    );
}