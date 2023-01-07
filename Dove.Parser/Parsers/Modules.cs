using ResourceDecl;
using static Core;
using static ExtraTools.Extensions;

namespace ModuleDecl;
public record Module(FileName File, bool IsExtern) : IDeclaration<Module>
{
    public override string ToString() => $".module {(IsExtern ? "extern" : String.Empty)} {File}";
    public static Parser<Module> AsParser => RunAll(
        converter: (vals) => new Module(vals[2].File, vals[1].IsExtern),
        Discard<Module, string>(ConsumeWord(Id, ".module")),
        TryRun(
            converter: (vals) => Construct<Module>(2, 1, vals == "extern"),
            ConsumeWord(Id, "extern"),
            Empty<string>()
        ),
        Map(
            converter: (filename) => Construct<Module>(2, 0, filename),
            FileName.AsParser
        )
    );
}