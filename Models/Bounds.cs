using System.Text;
using static Core;
public record Bound(INT Lower, INT Upper, Bound.BoundType Type) : IDeclaration<Parameter> {
    public record Collection(Bound[] Bounds) : IDeclaration<Collection> {
        public override string ToString() => $"{string.Join(", ", Bounds.Select(x => x.ToString()))}";
        public static Parser<Collection> AsParser => RunAll(
            converter: (bounds) => new Collection(bounds.SelectMany(Id).ToArray()),
            Map(
                converter: (bound) => new Bound[] { bound },
                Bound.AsParser
            ),
            RunMany(
                converter: (bounds) => bounds,
                0, Int32.MaxValue,
                RunAll(
                    converter: (vals) => vals[1],
                    Discard<Bound, string>(ConsumeWord(Id, ",")),
                    Bound.AsParser
                )
            )
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
        _ => throw new NotImplementedException()
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