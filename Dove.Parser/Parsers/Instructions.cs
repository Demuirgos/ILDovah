using System.Reflection;
using System.Text;
using ExtraTools;
using IdentifierDecl;
using LabelDecl;

using MethodDecl;
using SigArgumentDecl;
using TypeDecl;
using static Core;
using static ExtraTools.Extensions;
namespace InstructionDecl;
public record Instruction(String Opcode, InstructionArgument Arguments) : Member, IDeclaration<Instruction>
{
    public record Block(ARRAY<Instruction> Opcodes) : IDeclaration<Block>
    {
        public override string ToString() => Opcodes.ToString(' ');
        public static Parser<Block> AsParser => Map(
            (ARRAY<Instruction> opcodes) => new Block(opcodes),
            ARRAY<Instruction>.MakeParser(new ARRAY<Instruction>.ArrayOptions
            {
                Delimiters = ('\0', '\0', '\0')
            })
        );
    }

    public static Dictionary<string, string[]> OpcodeValues = new()
    {
        ["instr_none"] = new[] { "conv.ovf.i1.un", "conv.ovf.i2.un", "conv.ovf.i4.un", "conv.ovf.i8.un", "conv.ovf.u1.un", "conv.ovf.u2.un", "conv.ovf.u4.un", "conv.ovf.u8.un", "conv.ovf.i.un", "conv.ovf.u.un", "conv.ovf.i1", "conv.ovf.i2", "conv.ovf.i4", "conv.ovf.i8", "conv.ovf.u1", "conv.ovf.u2", "conv.ovf.u4", "conv.ovf.u8", "add.ovf.un", "conv.ovf.i", "conv.ovf.u", "endfinally", "ldelem.ref", "ldelem.any", "mul.ovf.un", "refanytype", "stelem.ref", "stelem.any", "sub.ovf.un", "conv.r.un", "endfilter", "ldc.i4.m1", "ldelem.i1", "ldelem.i2", "ldelem.i4", "ldelem.i8", "ldelem.r4", "ldelem.r8", "ldelem.u1", "ldelem.u2", "ldelem.u4", "ldind.ref", "stelem.i1", "stelem.i2", "stelem.i4", "stelem.i8", "stelem.r4", "stelem.r8", "stind.ref", "volatile.", "ckfinite", "endfault", "ldc.i4.0", "ldc.i4.1", "ldc.i4.2", "ldc.i4.3", "ldc.i4.4", "ldc.i4.5", "ldc.i4.6", "ldc.i4.7", "ldc.i4.8", "ldelem.i", "ldind.i1", "ldind.i2", "ldind.i4", "ldind.i8", "ldind.r4", "ldind.r8", "ldind.u1", "ldind.u2", "ldind.u4", "localloc", "stelem.i", "stind.i1", "stind.i2", "stind.i4", "stind.i8", "stind.r4", "stind.r8", "add.ovf", "arglist", "conv.i1", "conv.i2", "conv.i4", "conv.i8", "conv.r4", "conv.r8", "conv.u1", "conv.u2", "conv.u4", "conv.u8", "initblk", "ldarg.0", "ldarg.1", "ldarg.2", "ldarg.3", "ldind.i", "ldloc.0", "ldloc.1", "ldloc.2", "ldloc.3", "mul.ovf", "rethrow", "stind.i", "stloc.0", "stloc.1", "stloc.2", "stloc.3", "sub.ovf", "cgt.un", "clt.un", "conv.i", "conv.u", "div.un", "ldnull", "rem.un", "shr.un", "break", "cpblk", "ldlen", "tail.", "throw", "add", "and", "ceq", "cgt", "clt", "div", "dup", "mul", "neg", "nop", "not", "pop", "rem", "ret", "shl", "shr", "sub", "xor", "or" },
        ["instr_var"] = new[] { "ldarga.s", "ldloca.s", "ldarg.s", "ldloc.s", "starg.s", "stloc.s", "ldarga", "ldloca", "ldarg", "ldloc", "starg", "stloc" },
        ["instr_i"] = new[] { "unaligned.", "ldc.i4.s", "ldc.i4" },
        ["instr_i8"] = new[] { "ldc.i8" },
        ["instr_r"] = new[] { "ldc.r4", "ldc.r8" },
        ["instr_brtarget"] = new[] { "brfalse.s", "bge.un.s", "bgt.un.s", "ble.un.s", "blt.un.s", "bne.un.s", "brtrue.s" , "brnull.s", "brfalse", "brnull", "brzero.s", "brzero", "brinst.s", "brinst", "leave.s", "bge.un", "bgt.un", "ble.un", "blt.un", "bne.un", "brtrue", "beq.s", "bge.s", "bgt.s", "ble.s", "blt.s", "leave", "br.s", "beq", "bge", "bgt", "ble", "blt", "br" },
        ["instr_method"] = new[] { "ldvirtftn", "callvirt", "newobj", "ldftn", "call", "jmp" },
        ["instr_field"] = new[] { "ldsflda", "ldflda", "ldsfld", "stsfld", "ldfld", "stfld" },
        ["instr_type"] = new[] { "constrained.", "castclass", "unbox.any", "refanyval", "mkrefany", "initobj", "ldelema", "ldelem", "isinst", "newarr", "sizeof", "cpobj", "ldobj", "stobj", "unbox", "box" },
        ["instr_string"] = new[] { "ldstr" },
        ["instr_sig"] = new[] { "calli" },
        ["instr_rva"] = Array.Empty<string>(),
        ["instr_tok"] = new[] { "ldtoken" },
        ["instr_switch"] = new[] { "switch" },
        ["instr_phi"] = Array.Empty<string>()
    };

    public static Dictionary<string, string> OpcodeValuesInverse = OpcodeValues
        .SelectMany(kvp => kvp.Value
            .Select(value => new KeyValuePair<string, string>(value, kvp.Key)))
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    public override string ToString()
        => $"{Opcode} {Arguments}";

    public static Parser<Instruction> AsParser => TryRun(
        converter: Id,
        OpcodeValuesInverse.Keys
            .Select(opcode =>
            {
                var parserKind = OpcodeValuesInverse[opcode];
                var opcodeParser = ConsumeWord(Id, opcode);

                return parserKind switch
                {
                    "instr_var" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_VAR, string>(opcodeParser),
                        InstructionArgument_INSTR_VAR.AsParser
                    ),
                    "instr_i" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_I, string>(opcodeParser),
                        InstructionArgument_INSTR_I.AsParser
                    ),
                    "instr_i8" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_I8, string>(opcodeParser),
                        InstructionArgument_INSTR_I8.AsParser
                    ),
                    "instr_r" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_R, string>(opcodeParser),
                        InstructionArgument_INSTR_R.AsParser
                    ),
                    "instr_brtarget" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_BRTARGET, string>(opcodeParser),
                        InstructionArgument_INSTR_BRTARGET.AsParser
                    ),
                    "instr_method" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_METHOD, string>(opcodeParser),
                        InstructionArgument_INSTR_METHOD.AsParser
                    ),
                    "instr_field" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_FIELD, string>(opcodeParser),
                        InstructionArgument_INSTR_FIELD.AsParser
                    ),
                    "instr_type" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_TYPE, string>(opcodeParser),
                        InstructionArgument_INSTR_TYPE.AsParser
                    ),
                    "instr_string" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_STRING, string>(opcodeParser),
                        InstructionArgument_INSTR_STRING.AsParser
                    ),
                    "instr_sig" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_SIG, string>(opcodeParser),
                        InstructionArgument_INSTR_SIG.AsParser
                    ),
                    "instr_rva" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_RVA, string>(opcodeParser),
                        InstructionArgument_INSTR_RVA.AsParser
                    ),
                    "instr_tok" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_TOK, string>(opcodeParser),
                        InstructionArgument_INSTR_TOK.AsParser
                    ),
                    "instr_switch" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_SWITCH, string>(opcodeParser),
                        InstructionArgument_INSTR_SWITCH.AsParser
                    ),
                    "instr_phi" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_INSTR_PHI, string>(opcodeParser),
                        InstructionArgument_INSTR_PHI.AsParser
                    ),
                    "instr_none" => RunAll(
                        converter: parts => new Instruction(opcode, parts[1]),
                        Discard<InstructionArgument_None, string>(opcodeParser),
                        InstructionArgument_None.AsParser
                    ),
                    _ => throw new Exception($"Unknown parser kind {parserKind} for opcode {opcode}")
                };
            })
            .ToArray()
    );
}


[GenerateParser] public partial record InstructionArgument : IDeclaration<InstructionArgument>;
[WrapParser<INT>] public partial record InstructionArgument_INSTR_I : InstructionArgument, IDeclaration<InstructionArgument_INSTR_I>;
[WrapParser<INT>] public partial record InstructionArgument_INSTR_I8 : InstructionArgument, IDeclaration<InstructionArgument_INSTR_I8>;
[WrapParser<TypeSpecification>] public partial record InstructionArgument_INSTR_TYPE : InstructionArgument, IDeclaration<InstructionArgument_INSTR_TYPE>;
[WrapParser<INT>] public partial record InstructionArgument_INSTR_PHI : InstructionArgument, IDeclaration<InstructionArgument_INSTR_PHI>;
[WrapParser<MethodReference>] public partial record InstructionArgument_INSTR_METHOD : InstructionArgument, IDeclaration<InstructionArgument_INSTR_METHOD>;
[WrapParser<FieldTypeReference>] public partial record InstructionArgument_INSTR_FIELD : InstructionArgument, IDeclaration<InstructionArgument_INSTR_FIELD>;
[WrapParser<JumpLabels>] public partial record InstructionArgument_INSTR_SWITCH : InstructionArgument, IDeclaration<InstructionArgument_INSTR_SWITCH>;
[WrapParser<SigArgs>] public partial record InstructionArgument_INSTR_SIG : InstructionArgument, IDeclaration<InstructionArgument_INSTR_SIG>;
[WrapParser<OwnerType>] public partial record InstructionArgument_INSTR_TOK : InstructionArgument, IDeclaration<InstructionArgument_INSTR_TOK>;

[WrapParser<INT, Identifier>] public partial record InstructionArgument_INSTR_VAR : InstructionArgument, IDeclaration<InstructionArgument_INSTR_VAR>;
[WrapParser<INT, Identifier>] public partial record InstructionArgument_INSTR_BRTARGET : InstructionArgument, IDeclaration<InstructionArgument_INSTR_BRTARGET>;
[WrapParser<INT, Identifier>] public partial record InstructionArgument_INSTR_RVA : InstructionArgument, IDeclaration<InstructionArgument_INSTR_RVA>;

[WrapParser<INT, FLOAT, BytearrayArgument>] public partial record InstructionArgument_INSTR_R : InstructionArgument, IDeclaration<InstructionArgument_INSTR_R>;
[WrapParser<BytearrayArgument, QSTRING.Collection>] public partial record InstructionArgument_INSTR_STRING : InstructionArgument, IDeclaration<InstructionArgument_INSTR_STRING>;


[GenerationOrderParser(Order.Last)]
public record InstructionArgument_None : InstructionArgument
{
    public override string ToString() => String.Empty;
    public static Parser<InstructionArgument_None> AsParser => Empty<InstructionArgument_None>();
}

public record BytearrayArgument(string Prefix, ARRAY<BYTE> Bytes)
    : IDeclaration<BytearrayArgument>
{
    public override string ToString() => $"{Prefix}{Bytes}";
    public static Parser<BytearrayArgument> AsParser => RunAll(
        converter: parts => new BytearrayArgument(parts[0].Prefix, parts[1].Bytes),
        ConsumeWord(
            converter: prefix => Construct<BytearrayArgument>(2, 0, prefix),
            "bytearray"
        ),
        Map(
            converter: labels => Construct<BytearrayArgument>(2, 1, labels),
            ARRAY<BYTE>.MakeParser(new ARRAY<BYTE>.ArrayOptions {
                Delimiters = ('(', '\0', ')')
            })
        )
    );
}

public record SigArgs(SigArgument.Collection SigArguments)
    : IDeclaration<SigArgs>
{
    public override string ToString() => $"({SigArguments})";
    public static Parser<SigArgs> AsParser => RunAll(
        converter: parts => new SigArgs(parts[1]),
        Discard<SigArgs, char>(ConsumeChar(Id, '(')),
        Map(
            converter: labels => Construct<SigArgs>(1, 0, labels),
            SigArgument.Collection.AsParser
        ),
        Discard<SigArgs, char>(ConsumeChar(Id, ')'))
    );
}
public record JumpLabels(LabelOrOffset.Collection TargetLabels)
    : IDeclaration<JumpLabels>
{
    public override string ToString() => $"({TargetLabels})";
    public static Parser<JumpLabels> AsParser => RunAll(
        converter: parts => new JumpLabels(parts[1]),
        Discard<JumpLabels, char>(ConsumeChar(Id, '(')),
        Map(
            converter: labels => Construct<JumpLabels>(1, 0, labels),
            LabelOrOffset.Collection.AsParser
        ),
        Discard<JumpLabels, char>(ConsumeChar(Id, ')'))
    );
}