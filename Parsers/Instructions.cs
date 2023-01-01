using static Core;
using static Extensions;

[NotImplemented]
public record Instruction(String Opcode, ARRAY<QSTRING> Arguments) : MethodBodyItem, IDeclaration<Instruction> {
    public override string ToString() => String.Empty;
    public static Parser<Instruction> AsParser => Empty<Instruction>();
}