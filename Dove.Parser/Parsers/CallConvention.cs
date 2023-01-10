using System.Text;
using static Core;
using static ExtraTools.Extensions;
namespace CallConventionDecl;
public record CallConvention(CallAttribute Attribute, CallKind Kind) : IDeclaration<CallConvention>
{
    public override string ToString() {
        StringBuilder sb = new();
        if(Attribute is not null) {
            sb.Append($"{Attribute} ");
        }

        if(Kind is not null) {
            sb.Append(Kind);
        }
        return sb.ToString();
    }
    
    public static Parser<CallConvention> AsParser => RunAll(
        converter: vals => new CallConvention(vals[0].Attribute, vals[1].Kind),
        TryRun(
            converter: vals => Construct<CallConvention>(2, 0, vals),
            CallAttribute.AsParser,
            Empty<CallAttribute>()
        ),
        TryRun(
            converter: vals => Construct<CallConvention>(2, 1, vals),
            CallKind.AsParser,
            Empty<CallKind>()
        )
    );    
}
public record CallAttribute(string[] values) : IDeclaration<CallAttribute>
{
    public override string ToString() => String.Join(" ", values);
    public static Parser<CallAttribute> AsParser => RunAll(
        converter: labels => new CallAttribute(labels.Where(x => !String.IsNullOrEmpty(x)).ToArray()),
        ConsumeWord(Id, "instance"),
        TryRun(Id, ConsumeWord(Id, "explicit"), Empty<string>())
    );
}

public record CallKind(String Kind) : IDeclaration<CallKind>
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
