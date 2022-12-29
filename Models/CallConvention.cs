using static Core;
public record CallConvention : IDeclaration<CallConvention> {
    private Object _value;
    public record CallConventionPrimitive(string[] values) : IDeclaration<CallConventionPrimitive> {
        public override string ToString() => String.Join(" ", values);
        public static Parser<CallConventionPrimitive> AsParser => RunAll(
            converter: labels => new CallConventionPrimitive(labels.Where(x => !String.IsNullOrEmpty(x)).ToArray()),
            ConsumeWord(Id, "instance"),
            TryRun(Id, ConsumeWord(Id, "explicit"), Empty<string>())
        );
    }

    public record CallKind(String Kind) : IDeclaration<CallKind> {
        private static String[] PrimaryKeywords = {"default", "vararg", "unmanaged"};
        private static String[] SecondaryWords  = {"cdecl", "fastcall", "stdcall", "thiscall"};

        public override string ToString() => Kind;
        public static Parser<CallKind> AsParser => TryRun(
            converter: (vals) => new CallKind(vals),
            PrimaryKeywords.Select(word => {
                if(word == "unmanaged") {
                    return RunAll(
                        converter: vals => String.Join(' ', vals.Where(x => !String.IsNullOrEmpty(x))), 
                        ConsumeWord(Id, word),
                        TryRun(
                            converter: Id,
                            SecondaryWords.Select(word => ConsumeWord(Id, word)).ToArray()
                        )
                    );
                } else {
                    return ConsumeWord(Id, word);
                }
            }).ToArray()
        );
    }

    public override string ToString() => _value switch {
        CallConventionPrimitive primitive => primitive.ToString(),
        CallKind kind => kind.ToString(),
        _ => throw new NotImplementedException()
    };
    public static Parser<CallConvention> AsParser => TryRun(
        converter: Id,
        Map(
            converter: (kind) => new CallConvention { _value = kind },
            CallConventionPrimitive.AsParser
        ),
        Map(
            converter: (kind) => new CallConvention { _value = kind },
            CallKind.AsParser
        )
    );
}
