using static Core;
namespace MethodDecl;
public record CallConvention : IDeclaration<CallConvention>
{
    public record CallConventionPrimitive(string[] values) : CallConvention, IDeclaration<CallConventionPrimitive>
    {
        public override string ToString() => String.Join(" ", values);
        public static Parser<CallConventionPrimitive> AsParser => RunAll(
            converter: labels => new CallConventionPrimitive(labels.Where(x => !String.IsNullOrEmpty(x)).ToArray()),
            ConsumeWord(Id, "instance"),
            TryRun(Id, ConsumeWord(Id, "explicit"), Empty<string>())
        );
    }

    public record CallKind(String Kind) : CallConvention, IDeclaration<CallKind>
    {
        private static String[] PrimaryKeywords = { "default", "vararg", "unmanaged" };
        private static String[] SecondaryWords = { "cdecl", "fastcall", "stdcall", "thiscall" };

        public override string ToString() => Kind;
        public static Parser<CallKind> AsParser => TryRun(
            converter: (vals) => new CallKind(vals),
            PrimaryKeywords.Select(word =>
            {
                if (word == "unmanaged")
                {
                    return RunAll(
                        converter: vals => String.Join(' ', vals.Where(x => !String.IsNullOrEmpty(x))),
                        ConsumeWord(Id, word),
                        TryRun(
                            converter: Id,
                            SecondaryWords.Select(word => ConsumeWord(Id, word)).ToArray()
                        )
                    );
                }
                else
                {
                    return ConsumeWord(Id, word);
                }
            }).ToArray()
        );
    }
    public static Parser<CallConvention> AsParser => TryRun(
        converter: Id,
        Cast<CallConvention, CallConventionPrimitive>(CallConventionPrimitive.AsParser),
        Cast<CallConvention, CallKind>(CallKind.AsParser),
        Empty<CallConvention>()
    );
}
