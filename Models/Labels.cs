using static Core;

public record Label(LabelOrOffset[] Values) : IDeclaration<Label> {
    public override string ToString() => String.Join(',', Values.Select((val) => val.ToString()));
    public static Parser<Label> AsParser => RunAll(
        converter: (vals) => {
            return new Label(vals.Aggregate(new List<LabelOrOffset>(), (acc, val) => {
                acc.AddRange(val);
                return acc;
            }).ToArray());
        },

        Map((looff) => new List<LabelOrOffset>() { looff }, LabelOrOffset.AsParser),
        RunMany(
            converter: (vals) => vals.Where((val) => val != default(LabelOrOffset)).Select((val) => val).ToList(),
            0, Int32.MaxValue, 
            RunAll( converter: (vals) => vals[1],
                    ConsumeChar((_) => default(LabelOrOffset), ','),
                    LabelOrOffset.AsParser)
        )
    );
}

public record DataLabel(Identifier Value) : IDeclaration<DataLabel> {
    public override string ToString() => Value.ToString();
    public static Parser<DataLabel> AsParser => Map((Identifier id) => new DataLabel(id), Identifier.AsParser);
}

public record CodeLabel(Identifier Value) : IDeclaration<CodeLabel> {
    public override string ToString() => $"{Value}:";
    public static Parser<CodeLabel> AsParser => RunAll(
        converter: (vals) => new CodeLabel(vals[0]),
        Map((Identifier id) => id, Identifier.AsParser),
        ConsumeChar((_) => default(Identifier), ':')
    );
}


public record LabelOrOffset(Identifier Value) : IDeclaration<LabelOrOffset> {
    public override string ToString() => Value.ToString();
    public static Parser<LabelOrOffset> AsParser => Map((Identifier id) => new LabelOrOffset(id), Identifier.AsParser);
}
