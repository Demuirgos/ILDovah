record class Bounds(Bound[] Boundaries) : Decl
{
    public override string ToString()
        => String.Join(", ", Boundaries.Select(obj => obj.ToString()));
    internal static void Parse(ref int index, string source, out Bounds bounds)
    {
        var boundsList = new List<Bound>();
        do
        {
            Bound.Parse(ref index, source, out Bound bound);
            boundsList.Add(bound);
        } while (source.ConsumeWord(ref index, ","));
        bounds = new Bounds(boundsList.ToArray());
    }
}
record class Bound(long? Min, long? Max, bool HasDots) : Decl
{
    public override string ToString()
        => HasDots ? $"{Min}...{Max}" : $"{Min} {Max}";
    internal static void Parse(ref int index, string source, out Bound bound)
    {
        INT.Parse(ref index, source, out INT? min);
        bool hasDots = source.ConsumeWord(ref index, "...");
        INT.Parse(ref index, source, out INT? max);
        bound = new Bound(min?.Value, max?.Value, hasDots);
    }
}