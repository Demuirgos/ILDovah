using static Core;


namespace IdentifierDecl;
public record DottedName(ARRAY<Identifier> Values) : IDeclaration<DottedName>
{
    public override string ToString() => Values.ToString();
    public static Parser<DottedName> AsParser => Map(
        converter: Ids => new DottedName(Ids),
        ARRAY<Identifier>.MakeParser('\0', '.', '\0')
    );
}

public record SlashedName(ARRAY<Identifier> Values) : IDeclaration<SlashedName>
{
    public override string ToString() => Values.ToString();
    public static Parser<SlashedName> AsParser => Map(
        converter: Ids => new SlashedName(Ids),
        ARRAY<Identifier>.MakeParser('\0', '/', '\0')
    );
}

public record Identifier(string Value) : IDeclaration<Identifier>
{
    public override string ToString() => Value;
    public static Parser<Identifier> AsParser => TryRun(
        converter: (vals) => new Identifier(vals),
        Map((id) => id.ToString(), ID.AsParser),
        Map((qstring) => qstring.ToString(), QSTRING.AsParser)
    );
}