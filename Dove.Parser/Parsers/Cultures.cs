using static Core;
using static ExtraTools.Extensions;

public record Culture(QSTRING Value) : IDeclaration<Culture> {
    public override string ToString() => $".culture {Value} ";

    public static Parser<Culture> AsParser => RunAll(
        converter: parts => new Culture(
            parts[1].Value
        ),
        Discard<Culture, string>(ConsumeWord(Core.Id, ".culture")),
        Map(
            converter: value => Construct<Culture>(1, 0, value),
            QSTRING.AsParser
        )
    );
}