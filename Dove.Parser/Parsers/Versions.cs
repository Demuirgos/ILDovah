using static Core;
using static ExtraTools.Extensions;

public record Version(ARRAY<INT> SubVersions) : IDeclaration<Version> {
    public override string ToString() => $".ver {SubVersions.ToString(':')} ";

    public static Parser<Version> AsParser => RunAll(
        converter: parts => new Version(
            parts[1].SubVersions
        ),
        Discard<Version, string>(ConsumeWord(Core.Id, ".ver")),
        Map(
            converter: subversions => Construct<Version>(1, 0, subversions),
            ARRAY<INT>.MakeParser('\0', ':', '\0')
        )
    );
}