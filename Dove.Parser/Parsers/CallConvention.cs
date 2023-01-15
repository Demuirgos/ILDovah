using System.Text;

using static Core;
using static ExtraTools.Extensions;
namespace CallConventionDecl;

public record CallConvention(CallAttribute Attribute, CallKind Kind) : IDeclaration<CallConvention>
{
    public override string ToString()
    {
        StringBuilder sb = new();
        if (Attribute is not null)
        {
            sb.Append($"{Attribute} ");
        }

        if (Kind is not null)
        {
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

[GenerateParser] public partial record CallKind : IDeclaration<CallKind>;
public record CallKindPrimitive(String Kind) : CallKind, IDeclaration<CallKindPrimitive>
{
    private static String[] PrimaryKeywords = { "default", "vararg", "unmanaged" };
    private static String[] SecondaryWords = { "cdecl", "fastcall", "stdcall", "thiscall" };

    public override string ToString() => Kind;
    public static Parser<CallKindPrimitive> AsParser => TryRun(
        converter: (vals) => new CallKindPrimitive(vals),
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


public record CallKindCustom(INT Index) : CallKind, IDeclaration<CallKindCustom>
{
    public override string ToString() => $"callconv({Index})";
    public static Parser<CallKindCustom> AsParser => RunAll(
        converter: vals => new CallKindCustom(vals[2]),
        Discard<INT, String>(ConsumeWord(Id, "callconv")),
        Discard<INT, char>(ConsumeChar(Id, '(')),
        INT.AsParser,
        Discard<INT, char>(ConsumeChar(Id, ')'))
    );
}
