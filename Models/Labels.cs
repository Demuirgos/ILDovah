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

        Map(LabelOrOffset.AsParser, (looff) => new List<LabelOrOffset>() { looff }),
        RunMany(0, Int32.MaxValue, RunAll(converter: (vals) => vals[1],
                                          ConsumeChar(',', (_) => default(LabelOrOffset)),
                                          Map(LabelOrOffset.AsParser, Id)), 
            converter: (vals) => vals.Where((val) => val != default(LabelOrOffset)).Select((val) => val).ToList())
    );

    public static bool Parse(ref int index, string source, out Label idVal) {
        if(Label.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
}

public record DataLabel(Identifier Value) : IDeclaration<DataLabel> {
    public override string ToString() => Value.ToString();
    public static Parser<DataLabel> AsParser => Map(Identifier.AsParser, (Identifier id) => new DataLabel(id));

    public static bool Parse(ref int index, string source, out DataLabel idVal) {
        if(DataLabel.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
}

public record CodeLabel(Identifier Value) : IDeclaration<CodeLabel> {
    public override string ToString() => $"{Value}:";
    public static Parser<CodeLabel> AsParser => RunAll(
        converter: (vals) => new CodeLabel(vals[0]),
        Map(Identifier.AsParser, (Identifier id) => id),
        ConsumeChar(':', (_) => default(Identifier))
    );

    public static bool Parse(ref int index, string source, out CodeLabel idVal) {
        if(CodeLabel.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
}


public record LabelOrOffset(Identifier Value) : IDeclaration<LabelOrOffset> {
    public override string ToString() => Value.ToString();
    public static Parser<LabelOrOffset> AsParser => Map(Identifier.AsParser, (Identifier id) => new LabelOrOffset(id));

    public static bool Parse(ref int index, string source, out LabelOrOffset idVal) {
        if(LabelOrOffset.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
}
