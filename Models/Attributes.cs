using System.Text;
using static Core;

public record CustomAttribute(String Name) : IDeclaration<CustomAttribute> {
    public override string ToString() => Name;
    public static Parser<CustomAttribute> AsParser => null;
    public static bool Parse(ref int index, string source, out CustomAttribute idVal) {
        if(CustomAttribute.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
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