using static Core;

using static ExtraTools.Extensions;

namespace VersionDecl;
public record Version(ARRAY<INT> SubVersions) : IDeclaration<Version>
{
    public override string ToString() => $".ver {SubVersions.ToString(':')}";

    public static Parser<Version> AsParser => RunAll(
        converter: parts => new Version(
            parts[1].SubVersions
        ),
        Discard<Version, string>(ConsumeWord(Core.Id, ".ver")),
        Map(
            converter: subversions => Construct<Version>(1, 0, subversions),
            ARRAY<INT>.MakeParser(new ARRAY<INT>.ArrayOptions
            {
                Delimiters = ('\0', ':', '\0')
            })
        )
    );
}