using static Core;
using static ExtraTools.Extensions;
using LocalsSignature = Local.Collection; 

public record Local(Type Type, Identifier Id) : IDeclaration<Local> {
    public record Collection(ARRAY<Local> Values) : IDeclaration<Collection> {
        public override string ToString() => Values.ToString();
        public static Parser<Collection> AsParser => Map(
            converter: arr => new Collection(arr),
            ARRAY<Local>.MakeParser('\0', ',', '\0')
        );
    }

    public override string ToString() => $"{Type}{Id}";
    public static Parser<Local> AsParser => RunAll(
        converter: parts => new Local(parts[0].Type, parts[1]?.Id),
        Map(
            converter: type => Construct<Local>(2, 0, type), 
            Type.AsParser
        ),
        TryRun(
            converter: id => Construct<Local>(2, 1, id), 
            Identifier.AsParser, Empty<Identifier>()
        )
    );
}