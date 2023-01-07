using IdentifierDecl;
using static Core;

namespace LabelDecl;
[WrapParser<Identifier>] public partial record LabelOrOffset : IDeclaration<LabelOrOffset>;
[WrapParser<Identifier>] public partial record DataLabel : IDeclaration<DataLabel>;

public record Label(ARRAY<LabelOrOffset> Values) : IDeclaration<Label>
{
    public override string ToString() => Values.ToString();
    public static Parser<Label> AsParser => Map(
        converter: (vals) => new Label(vals),
        ARRAY<LabelOrOffset>.MakeParser('\0', ',', '\0')
    );
}
public record CodeLabel(Identifier Value) : IDeclaration<CodeLabel>
{
    public override string ToString() => $"{Value}:";
    public static Parser<CodeLabel> AsParser => RunAll(
        converter: (vals) => new CodeLabel(vals[0]),
        Map((Identifier id) => id, Identifier.AsParser),
        ConsumeChar((_) => default(Identifier), ':')
    );
}


