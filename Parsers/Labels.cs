using static Core;

public record Label(ARRAY<LabelOrOffset> Values) : IDeclaration<Label> {
    public override string ToString() => Values.ToString(); 
    public static Parser<Label> AsParser => Map(
        converter: (vals) => new Label(vals),
        ARRAY<LabelOrOffset>.MakeParser('\0', ',', '\0')
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
