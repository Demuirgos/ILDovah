using IdentifierDecl;
using static Core;


namespace LabelDecl;
[WrapParser<Identifier>]
public partial record LabelOrOffset : IDeclaration<LabelOrOffset>
{
    public record Collection(ARRAY<LabelOrOffset> Values) : IDeclaration<Collection>
    {
        public override string ToString() => Values.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: (ARRAY<LabelOrOffset> vals) => new Collection(vals),
            ARRAY<LabelOrOffset>.MakeParser('\0', ',', '\0')
        );
    }
}
[WrapParser<SimpleName>] public partial record DataLabel : IDeclaration<DataLabel>;


public record CodeLabel(Identifier Value) : IDeclaration<CodeLabel>
{
    public override string ToString() => $"{Value}:";
    public static Parser<CodeLabel> AsParser => RunAll(
        converter: (vals) => new CodeLabel(vals[0]),
        Map((Identifier id) => id, Identifier.AsParser),
        ConsumeChar((_) => default(Identifier), ':')
    );
}


