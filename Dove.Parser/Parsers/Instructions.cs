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
            ARRAY<Instruction>.MakeParser('\0', '\0', '\0')
        );
    }

    public static Dictionary<string, string[]> OpcodeValues = new()
    {
        ["instr_none"] = new[] { "add", "add.ovf", "add.ovf.un", "and", "arglist", "break", "ceq", "cgt", "cgt.un", "ckfinite", "clt", "clt.un", "conv.i", "conv.i1", "conv.i2", "conv.i4", "conv.i8", "conv.ovf.i", "conv.ovf.i.un", "conv.ovf.i1", "conv.ovf.i1.un", "conv.ovf.i2", "conv.ovf.i2.un", "conv.ovf.i4", "conv.ovf.i4.un", "conv.ovf.i8", "conv.ovf.i8.un", "conv.ovf.u", "conv.ovf.u.un", "conv.ovf.u1", "conv.ovf.u1.un", "conv.ovf.u2", "conv.ovf.u2.un", "conv.ovf.u4", "conv.ovf.u4.un", "conv.ovf.u8", "conv.ovf.u8.un", "conv.r.un", "conv.r4", "conv.r8", "conv.u", "conv.u1", "conv.u2", "conv.u4", "conv.u8", "cpblk", "div", "div.un", "dup", "endfault", "endfilter", "endfinally", "initblk", "ldarg.0", "ldarg.1", "ldarg.2", "ldarg.3", "ldc.i4.0", "ldc.i4.1", "ldc.i4.2", "ldc.i4.3", "ldc.i4.4", "ldc.i4.5", "ldc.i4.6", "ldc.i4.7", "ldc.i4.8", "ldc.i4.M1", "ldelem.i", "ldelem.i1", "ldelem.i2", "ldelem.i4", "ldelem.i8", "ldelem.r4", "ldelem.r8", "ldelem.ref", "ldelem.u1", "ldelem.u2", "ldelem.u4", "ldind.i", "ldind.i1", "ldind.i2", "ldind.i4", "ldind.i8", "ldind.r4", "ldind.r8", "ldind.ref", "ldind.u1", "ldind.u2", "ldind.u4", "ldlen", "ldloc.0", "ldloc.1", "ldloc.2", "ldloc.3", "ldnull", "localloc", "mul", "mul.ovf", "mul.ovf.un", "neg", "nop", "not", "or", "pop", "refanytype", "rem", "rem.un", "ret", "rethrow", "shl", "shr", "shr.un", "stelem.i", "stelem.i1", "stelem.i2", "stelem.i4", "stelem.i8", "stelem.r4", "stelem.r8", "stelem.ref", "stind.i", "stind.i1", "stind.i2", "stind.i4", "stind.i8", "stind.r4", "stind.r8", "stind.ref", "stloc.0", "stloc.1", "stloc.2", "stloc.3", "sub", "sub.ovf", "sub.ovf.un", "tail.", "throw", "volatile.", "xor" },
        ["instr_var"] = new[] { "ldarg", "ldarg.s", "ldarga", "ldarga.s", "ldloc", "ldloc.s", "ldloca", "ldloca.s", "starg", "starg.s", "stloc", "stloc.s" },
        ["instr_i"] = new[] { "ldc.i4", "ldc.i4.s", "unaligned." },
        ["instr_i8"] = new[] { "ldc.i8" },
        ["instr_r"] = new[] { "ldc.r4", "ldc.r8" },
        ["instr_brtarget"] = new[] { "beq", "beq.s", "bge", "bge.s", "bge.un", "bge.un.s", "bgt", "bgt.s", "bgt.un", "bgt.un.s", "ble", "ble.s", "ble.un", "ble.un.s", "blt", "blt.s", "blt.un", "blt.un.s", "bne.un", "bne.un.s", "br", "br.s", "brfalse", "brfalse.s", "brtrue", "brtrue.s", "leave", "leave.s" },
        ["instr_method"] = new[] { "call", "callvirt", "jmp", "ldftn", "ldvirtftn", "newobj" },
        ["instr_field"] = new[] { "ldfld", "ldflda", "ldsfld", "ldsflda", "stfld", "stsfld" },
        ["instr_type"] = new[] { "box", "castclass", "cpobj", "initobj", "isinst", "ldelema", "ldobj", "mkrefany", "newarr", "refanyval", "sizeof", "stobj", "unbox" },
        ["instr_string"] = new[] { "ldstr" },
        ["instr_sig"] = new[] { "calli" },
        ["instr_rva"] = Array.Empty<String>(),
        ["instr_tok"] = new[] { "ldtoken" },
        ["instr_switch"] = new[] { "switch" },
        ["instr_phi"] = Array.Empty<String>(),
    };

    public static Dictionary<string, string> OpcodeValuesInverse = OpcodeValues
        .SelectMany(kvp => kvp.Value
            .Select(value => new KeyValuePair<string, string>(value, kvp.Key)))
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    public override string ToString()
    {
        return $"{Opcode} {Arguments}";
    }

    public static Parser<Instruction> AsParser => TryRun(
        converter : Id,
        OpcodeValuesInverse.Keys
            .Select(opcode => {
                var parserKind = OpcodeValuesInverse[opcode];
                var opcodeParser = ConsumeWord(Id, opcode);

                return parserKind switch {
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


[GenerationOrderParser(Order.Last)] public record InstructionArgument_None : InstructionArgument {
    public override string ToString() => String.Empty;
    public static Parser<InstructionArgument_None> AsParser => Empty<InstructionArgument_None>();
}

public record BytearrayArgument(string Prefix, ARRAY<BYTE> Bytes)
    : IDeclaration<BytearrayArgument> {
    public override string ToString() => $"{Prefix}{Bytes}";
    public static Parser<BytearrayArgument> AsParser => RunAll(
        converter: parts => new BytearrayArgument(parts[0].Prefix, parts[1].Bytes),
        ConsumeWord(
            converter: prefix => Construct<BytearrayArgument>(2, 0, prefix),
            "bytearray"
        ),
        Map(
            converter: labels => Construct<BytearrayArgument>(2, 1, labels),
            ARRAY<BYTE>.MakeParser('(', '\0', ')')
        )
    );
}

public record SigArgs(SigArgument.Collection SigArguments) 
    : IDeclaration<SigArgs> {
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
    : IDeclaration<JumpLabels> {
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