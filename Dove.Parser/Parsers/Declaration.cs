using static Core;


namespace RootDecl;
[GenerateParser]
public partial record Declaration : IDeclaration<Declaration>
{
    public record Collection(ARRAY<Declaration> Declarations) : IDeclaration<Collection>
    {
        public override string ToString() => Declarations.ToString('\n');
        public static Parser<Collection> AsParser => Map(
            converter: arr => new Collection(arr),
            ARRAY<Declaration>.MakeParser('\0', '\0', '\0')
        );
    }
}
