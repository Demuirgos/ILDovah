using static Core;


namespace IdentifierDecl;

[GenerateParser] public partial record Identifier : IDeclaration<Identifier>; 
public record DottedName(ARRAY<SimpleName> Values) : Identifier, IDeclaration<DottedName>
{
    public override string ToString() => Values.ToString(".");
    public static Parser<DottedName> AsParser => Map(
        converter: Ids => new DottedName(Ids),
        ARRAY<SimpleName>.MakeParser('\0', '.', '\0')
    );
}

public record SlashedName(ARRAY<SimpleName> Values) : Identifier, IDeclaration<SlashedName>
{
    public override string ToString() => Values.ToString("/");
    public static Parser<SlashedName> AsParser => Map(
        converter: Ids => new SlashedName(Ids),
        ARRAY<SimpleName>.MakeParser('\0', '/', '\0')
    );
}

[GenerationOrderParser(Order.Last)]
public record SimpleName(string Value) : Identifier, IDeclaration<SimpleName>
{
    public override string ToString() => Value;
    public static Parser<SimpleName> AsParser => TryRun(
        converter: (vals) => new SimpleName(vals),
        Map((id) => id.ToString(), ID.AsParser),
        Map((qstring) => qstring.ToString(), QSTRING.AsParser)
    );
}