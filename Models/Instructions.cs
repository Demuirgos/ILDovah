using static Core;
using static Extensions;

public record Instruction(String Opcode, ARRAY<QSTRING> Arguments) : MethodBodyItem, IDeclaration<Instruction> {
    public override string ToString() => throw new NotImplementedException();
    public static Parser<Instruction> AsParser => throw new NotImplementedException(); 
}