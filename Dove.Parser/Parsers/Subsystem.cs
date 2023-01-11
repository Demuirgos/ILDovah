using ModuleDecl;
using RootDecl;

using static Core;

namespace SubSystemDecl;
public record SubSystem(INT Number) : Declaration, IDeclaration<Module>
{
    public override string ToString() => $".subsystem {Number}";
    public static Parser<SubSystem> AsParser => RunAll(
        converter: (vals) => new SubSystem(vals[1].Number),
        Discard<SubSystem, string>(ConsumeWord(Id, ".subsystem")),
        Map(
            converter: (number) => new SubSystem(number),
            INT.AsParser
        )
    );
}