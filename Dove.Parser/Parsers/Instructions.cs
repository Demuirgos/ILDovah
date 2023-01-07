using ExtraTools;
using MethodDecl;
using TypeDecl;
using static Core;

namespace InstructionDecl;

[NotImplemented]
public record Instruction(IdentifierDecl.DottedName Id, ARRAY<QSTRING> Arguments) : Member, IDeclaration<Instruction>
{
    public static Dictionary<string, string[]> OpcodeFormats = new() {
        ["instr_var"]       = new[]{"InlineVar", "ShortInlineVar"},
        ["instr_i"]         = new[]{"InlineI", "ShortInlineI"},
        ["instr_i8"]        = new[]{"InlineI8"},
        ["instr_r"]         = new[]{"InlineR", "ShortInlineR"},
        ["instr_brtarget"]  = new[]{"InlineBrTarget", "ShortInlineBrTarget"},
        ["instr_method"]    = new[]{"InlineMethod"},
        ["instr_field"]     = new[]{"InlineField"},
        ["instr_type"]      = new[]{"InlineType"},
        ["instr_string"]    = new[]{"InlineString"},
        ["instr_sig"]       = new[]{"InlineSig"},
        ["instr_rva"]       = new[]{"InlineRVA"},
        ["instr_tok"]       = new[]{"InlineTok"},
        ["instr_switch"]    = new[]{"InlineSwitch"},
        ["instr_phi"]       = new[]{"InlinePhi"},
        ["instr_none"]      = new[]{"InlineNone"},
    };

    public static Dictionary<string, string[]> OpcodeValues = new() {
        ["instr_none"]      = new[]{"add", "add.ovf", "add.ovf.un", "and", "arglist", "break", "ceq", "cgt", "cgt.un", "ckfinite", "clt", "clt.un", "conv.i", "conv.i1", "conv.i2", "conv.i4", "conv.i8", "conv.ovf.i", "conv.ovf.i.un", "conv.ovf.i1|conv.ovf.i1.un", "conv.ovf.i2", "conv.ovf.i2.un", "conv.ovf.i4|conv.ovf.i4.un", "conv.ovf.i8", "conv.ovf.i8.un", "conv.ovf.u", "conv.ovf.u.un", "conv.ovf.u1", "conv.ovf.u1.un", "conv.ovf.u2|conv.ovf.u2.un", "conv.ovf.u4", "conv.ovf.u4.un", "conv.ovf.u8|conv.ovf.u8.un", "conv.r.un", "conv.r4", "conv.r8", "conv.u", "conv.u1", "conv.u2", "conv.u4", "conv.u8", "cpblk", "div", "div.un", "dup", "endfault", "endfilter", "endfinally", "initblk", "| ldarg.0", "ldarg.1", "ldarg.2", "ldarg.3", "ldc.i4.0", "ldc.i4.1", "ldc.i4.2", "ldc.i4.3", "ldc.i4.4", "ldc.i4.5", "ldc.i4.6", "ldc.i4.7", "ldc.i4.8", "ldc.i4.M1", "ldelem.i", "ldelem.i1", "ldelem.i2", "ldelem.i4", "ldelem.i8", "ldelem.r4", "ldelem.r8", "ldelem.ref", "ldelem.u1", "ldelem.u2", "ldelem.u4", "ldind.i", "ldind.i1", "ldind.i2", "ldind.i4", "ldind.i8", "ldind.r4", "ldind.r8", "ldind.ref", "ldind.u1", "ldind.u2", "ldind.u4", "ldlen", "ldloc.0", "ldloc.1", "ldloc.2", "ldloc.3", "ldnull", "localloc", "mul", "mul.ovf", "mul.ovf.un", "neg", "nop", "not", "or", "pop", "refanytype", "rem", "rem.un", "ret", "rethrow", "shl", "shr", "shr.un", "stelem.i", "stelem.i1", "stelem.i2", "stelem.i4", "stelem.i8", "stelem.r4", "stelem.r8", "stelem.ref", "stind.i", "stind.i1", "stind.i2", "stind.i4", "stind.i8", "stind.r4", "stind.r8", "stind.ref", "stloc.0", "stloc.1", "stloc.2", "stloc.3", "sub", "sub.ovf", "sub.ovf.un", "tail.", "throw", "volatile.", "xor"},
        ["instr_var"]       = new[]{"ldarg", "ldarg.s", "ldarga", "ldarga.s", "ldloc", "ldloc.s", "ldloca", "ldloca.s", "starg", "starg.s", "stloc", "stloc.s"},
        ["instr_i"]         = new[]{"ldc.i4", "ldc.i4.s", "unaligned."},
        ["instr_i8"]        = new[]{"ldc.i8"},
        ["instr_r"]         = new[]{"ldc.r4", "ldc.r8"},
        ["instr_brtarget"]  = new[]{"beq", "beq.s", "bge", "bge.s", "bge.un", "bge.un.s", "bgt", "bgt.s", "bgt.un", "bgt.un.s", "ble", "ble.s", "ble.un", "ble.un.s", "blt", "blt.s", "blt.un", "blt.un.s", "bne.un", "bne.un.s", "br", "br.s", "brfalse", "brfalse.s", "brtrue", "brtrue.s", "leave", "leave.s"},
        ["instr_method"]    = new[]{"call", "callvirt", "jmp", "ldftn", "ldvirtftn", "newobj"},
        ["instr_field"]     = new[]{"ldfld", "ldflda", "ldsfld", "ldsflda", "stfld", "stsfld"},
        ["instr_type"]      = new[]{"box", "castclass", "cpobj", "initobj", "isinst", "ldelema", "ldobj", "mkrefany", "newarr", "refanyval", "sizeof", "stobj", "unbox"},
        ["instr_string"]    = new[]{"ldstr"},
        ["instr_sig"]       = new[]{"calli"},
        ["instr_rva"]       = Array.Empty<String>(),
        ["instr_tok"]       = new[]{"ldtoken"},
        ["instr_switch"]    = new[]{"switch"},
        ["instr_phi"]       = Array.Empty<String>(),
    };

    public static Dictionary<string, string> OpcodeValuesInverse = OpcodeValues
        .SelectMany(kvp => kvp.Value
            .Select(value => new KeyValuePair<string, string>(value, kvp.Key)))
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    
    public override string ToString() => String.Empty;
    public static Parser<Instruction> AsParser => Empty<Instruction>();
}


/*
instr : 
    INSTR_NONE
    | INSTR_VAR (int32 | id)
    | INSTR_R (float64 | int64 | '(' bytes ')')
    | INSTR_BRTARGET (int32 | id)
    | INSTR_FIELD type [typeSpec '::'] id
    | INSTR_STRING compQstring | 'bytearray' '(' bytes ')'
    | INSTR_SIG callConv type '(' sigArgs0 ')'
    | INSTR_RVA (id | int32)
    | INSTR_TOK (memberRef | typeSpec)
    | INSTR_SWITCH '(' labels ')'
;

*/
[GenerateParser] public partial record InstructionArgument : IDeclaration<InstructionArgument>;
public record InstructionArgument_None : InstructionArgument;
[WrapParser<INT>] public partial record InstructionArgument_INSTR_I: InstructionArgument, IDeclaration<InstructionArgument_INSTR_I>;
[WrapParser<INT>] public partial record InstructionArgument_INSTR_I8: InstructionArgument, IDeclaration<InstructionArgument_INSTR_I8>;
[WrapParser<TypeSpecification>] public partial record InstructionArgument_INSTR_TYPE: InstructionArgument, IDeclaration<InstructionArgument_INSTR_TYPE>;
[WrapParser<INT>] public partial record InstructionArgument_INSTR_PHI: InstructionArgument, IDeclaration<InstructionArgument_INSTR_PHI>;
[WrapParser<MethodReference>] public partial record InstructionArgument_INSTR_METHOD: InstructionArgument, IDeclaration<InstructionArgument_INSTR_METHOD>;



public record InlinedOrRef : IDeclaration<InlinedOrRef>;
public record Inlined() : InlinedOrRef;