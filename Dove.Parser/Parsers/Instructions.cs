using ExtraTools;
using MethodDecl;
using static Core;


namespace InstructionDecl;

[NotImplemented]
public record Instruction(String Opcode, ARRAY<QSTRING> Arguments) : Member, IDeclaration<Instruction>
{
    public override string ToString() => String.Empty;
    public static Parser<Instruction> AsParser => Empty<Instruction>();
}