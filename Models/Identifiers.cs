using static Core;

public record DottedName(String Value) : IDeclaration<DottedName> {
    public override string ToString() => Value;
    public static Parser<DottedName> AsParser => RunAll(
        converter: (vals) => new DottedName(String.Join('.', vals)),

        Map((Identifier id) => id.ToString(), Identifier.AsParser),
        RunMany(
            converter: (vals) => String.Join('.', vals),
            0, Int32.MaxValue, RunAll(
                converter: (vals) => vals[1],

                ConsumeChar((_) => String.Empty, '.'),
                Map((Identifier id) => id.ToString(), Identifier.AsParser)
            )
        )
    );
}

public record SlashedName(String Value) : IDeclaration<SlashedName> {
    public override string ToString() => Value;
    public static Parser<SlashedName> AsParser => RunAll(
        converter: (vals) => new SlashedName(String.Join('/', vals)),

        Map((Identifier id) => id.ToString(), Identifier.AsParser),
        RunMany(
            converter: (vals) => String.Join('/', vals),
            0, Int32.MaxValue, RunAll(
                converter: (vals) => vals[1],

                ConsumeChar((_) => String.Empty, '/'),
                Map((id) => id.ToString(), Identifier.AsParser)
            ) 
        )
    );
}

public record Identifier(string Value) : IDeclaration<Identifier> {
    public override string ToString() => Value;
    public static Parser<Identifier> AsParser => TryRun(
        converter: (vals) => new Identifier(vals),
        Map((id) => id.ToString(), ID.AsParser), 
        Map((qstring) => qstring.ToString(), QSTRING.AsParser)
    );
}