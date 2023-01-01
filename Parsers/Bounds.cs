using static Core;
public record Bound(INT Lower, INT Upper, Bound.BoundType Type) : IDeclaration<Bound> {
    public record Collection(ARRAY<Bound> Bounds) : IDeclaration<Collection> {
        public override string ToString() => Bounds.ToString();
        public static Parser<Collection> AsParser => Map(
            converter: (bounds) => new Collection(bounds),
            ARRAY<Bound>.MakeParser('\0', ',', '\0')
        );
    }
    public enum BoundType {
        None = 0, LowerBound = 1, UpperBound = 2, BothBounds = LowerBound | UpperBound
    }

    public override string ToString() => Type switch {
        BoundType.None => "...",
        BoundType.BothBounds => $"{Lower}...{Upper}",
        BoundType.UpperBound => $"{Upper}",
        BoundType.LowerBound => $"{Lower}...",
        _ => throw new System.Diagnostics.UnreachableException()
    };

    public static Parser<Bound> AsParser => RunAll(
        // Align BoundType with Spec 
        converter: (vals) => new Bound(vals[0].Lower, vals[2].Upper, vals.Aggregate(BoundType.None, (acc, val) => acc | val.Type)),
        TryRun(
            converter: (lower) => new Bound(lower, null, lower is null ? BoundType.None : BoundType.LowerBound),
            INT.AsParser, Empty<INT>()
        ),
        TryRun(
            converter: (type) => new Bound(null, null, BoundType.None),
            ConsumeWord(Id, "..."), Empty<String>()
        ),
        TryRun(
            converter: (upper) => new Bound(null, upper, upper is null ? BoundType.None : BoundType.UpperBound),
            INT.AsParser, Empty<INT>()
        )
    );
}