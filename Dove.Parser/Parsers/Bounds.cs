
using static Core;
using static ExtraTools.Extensions;

namespace BoundsDecl;
public record Bound(INT? LeftBound, INT? RightBound, Bound.BoundType Type) : IDeclaration<Bound>
{
    public record Collection(ARRAY<Bound> Bounds) : IDeclaration<Collection>
    {
        public override string ToString() => Bounds.ToString(',');
        public static Parser<Collection> AsParser => Map(
            converter: (bounds) => new Collection(bounds),
            ARRAY<Bound>.MakeParser('\0', ',', '\0')
        );
    }
    public enum BoundType
    {
        None = 0, SingleBound = 1, Vararg = 2, LowerBound = Vararg | SingleBound, Bounded = SingleBound | Vararg | 4
    }

    public override string ToString() => Type switch
    {
        BoundType.Vararg => "...",
        BoundType.LowerBound => $"{LeftBound}...",
        BoundType.Bounded => $"{LeftBound}...{RightBound}",
        BoundType.SingleBound => $"{LeftBound}",
        _ => String.Empty
    };

    public static Parser<Bound> AsParser => ConsumeIf(
        RunAll(
            converter: (vals) => new Bound(vals[0].LeftBound, vals[2].RightBound, vals.Aggregate(BoundType.None, (acc, val) => acc | val.Type)),
            TryRun(
                converter: (lower) => new Bound(lower, null, lower is null ? BoundType.None : BoundType.SingleBound),
                INT.AsParser, Empty<INT>()
            ),
            TryRun(
                converter: (type) => new Bound(null, null, String.IsNullOrEmpty(type) ? BoundType.None : BoundType.Vararg),
                ConsumeWord(Id, "..."), Empty<String>()
            ),
            TryRun(
                converter: (upper) => new Bound(null, upper, upper is null ? BoundType.None : BoundType.Bounded),
                INT.AsParser, Empty<INT>()
            )
        ),
        bound => bound.Type != BoundType.None
    );
}