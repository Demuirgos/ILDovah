using IdentifierDecl;
using static Core;
using static ExtraTools.Extensions;


namespace LocalDecl;
public record Local(BoundsDecl.Bound.Collection Index, TypeDecl.Type Type, Identifier Id) : IDeclaration<Local>
{
    public record Collection(ARRAY<Local> Values) : IDeclaration<Collection>
    {
        public override string ToString() => Values.ToString(",\n");
        public static Parser<Collection> AsParser => Map(
            converter: arr => new Collection(arr),
            ARRAY<Local>.MakeParser(new ARRAY<Local>.ArrayOptions
            {
                Delimiters = ('(', ',', ')')
            })
        );
    }

    public override string ToString() => $"{Index} {Type} {Id}";
    public static Parser<Local> AsParser => RunAll(
        converter: parts => new Local(parts[0]?.Index, parts[1].Type, parts[2]?.Id),
        Map(
            converter: index => Construct<Local>(3, 0, index),
            TryRun(Core.Id, 
                ConsumeIf(
                    BoundsDecl.Bound.Collection.AsParser,
                    result => result.Bounds.Values.Length == 1 && result.Bounds.Values[0].Type == BoundsDecl.Bound.BoundType.SingleBound
                ),
                Empty<BoundsDecl.Bound.Collection>()
            )
        ),
        Map(
            converter: type => Construct<Local>(3, 1, type),
            TypeDecl.Type.AsParser
        ),
        TryRun(
            converter: id => Construct<Local>(3, 2, id),
            Identifier.AsParser, Empty<Identifier>()
        )
    );
}