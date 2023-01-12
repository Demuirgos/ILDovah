using AttributeDecl;
using IdentifierDecl;
using LabelDecl;
using RootDecl;

using static Core;
using static ExtraTools.Extensions;

namespace FieldDecl;
public record Field(INT? Index, FieldAttribute.Collection Attributes, TypeDecl.Type Type, Identifier Id, Initialization? Value) : Declaration, IDeclaration<Field>
{
    public override string ToString() => $".field {(Index is null ? String.Empty : $"[{Index}] ")}{Attributes} {Type} {Id} {Value?.ToString() ?? String.Empty}";
    public static Parser<Field> AsParser => RunAll(
        converter: parts => new Field(
            parts[1].Index,
            parts[2].Attributes,
            parts[3].Type,
            parts[4].Id,
            parts[5].Value
        ),
        Discard<Field, string>(ConsumeWord(Core.Id, ".field")),
        TryRun(
            converter: idx => Construct<Field>(5, 0, idx),
            RunAll(
                parts => parts[1],
                Discard<INT, char>(ConsumeChar(Core.Id, '[')),
                INT.AsParser,
                Discard<INT, char>(ConsumeChar(Core.Id, ']'))
            ),
            Empty<INT>()
        ),
        Map(
            converter: attr => Construct<Field>(5, 1, attr),
            FieldAttribute.Collection.AsParser
        ),
        Map(
            converter: type => Construct<Field>(5, 2, type),
            TypeDecl.Type.AsParser
        ),
        Map(
            converter: id => Construct<Field>(5, 3, id),
            SimpleName.AsParser
        ),
        TryRun(
            converter: val => Construct<Field>(5, 4, val),
            Initialization.AsParser,
            Empty<Initialization>()
        )
    );
}

[GenerateParser]
public partial record Initialization : IDeclaration<Initialization>;
public record LabelReference(DataLabel Label) : Initialization, IDeclaration<LabelReference>
{
    public override string ToString() => $"at {Label}";
    public static Parser<LabelReference> AsParser => Map(
        converter: label => new LabelReference(label),
        RunAll(
            converter: parts => parts[1],
            Discard<DataLabel, string>(ConsumeWord(Core.Id, "at")),
            DataLabel.AsParser
        )
    );
}

public record FieldValue(FieldInit Value) : Initialization, IDeclaration<FieldValue>
{
    public override string ToString() => $"= {Value}";
    public static Parser<FieldValue> AsParser => Map(
        converter: val => new FieldValue(val),
        RunAll(
            converter: parts => parts[1],
            Discard<FieldInit, string>(ConsumeWord(Core.Id, "=")),
            FieldInit.AsParser
        )
    );
}

[GenerateParser] public partial record FieldInit : IDeclaration<FieldInit>;
public record BoolItem(BOOL Value) : FieldInit, IDeclaration<BoolItem>
{
    public override string ToString() => $"bool({Value})";
    public static Parser<BoolItem> AsParser => Map(
        converter: BoolVal => new BoolItem(BoolVal),
        RunAll(
            converter: parts => parts[2],
            Discard<BOOL, string>(ConsumeWord(Id, "bool")),
            Discard<BOOL, char>(ConsumeChar(Id, '(')),
            BOOL.AsParser,
            Discard<BOOL, char>(ConsumeChar(Id, ')'))
        )
    );
}
public record StringItem(QSTRING String) : FieldInit, IDeclaration<StringItem>
{
    public override string ToString() => String.ToString();
    public static Parser<StringItem> AsParser => Map(
        converter: str => new StringItem(str),
        QSTRING.AsParser
    );
}

public record CharItem(INT Unicode) : FieldInit, IDeclaration<CharItem>
{
    public override string ToString() => $"char({Unicode})";
    public static Parser<CharItem> AsParser => Map(
        converter: IntVal => new CharItem(IntVal),
        RunAll(
            converter: parts => parts[2],
            Discard<INT, string>(ConsumeWord(Id, "char")),
            Discard<INT, char>(ConsumeChar(Id, '(')),
            INT.AsParser,
            Discard<INT, char>(ConsumeChar(Id, ')'))
        )
    );
}

[GenerateParser] public partial record IntegralItem : FieldInit, IDeclaration<IntegralItem>;
public record FloatItem(FLOAT Value, INT BitSize) : IntegralItem, IDeclaration<FloatItem>
{
    public override string ToString() => $"float{BitSize}({Value})";
    public static Parser<FloatItem> AsParser => RunAll(
        converter: parts => new FloatItem(parts[2].Value, parts[0].BitSize),
        RunAll(
            converter: parts => parts[1],
            skipWhitespace: false,
            Discard<FloatItem, string>(ConsumeWord(Id, "float")),
            Map(
                converter: num => Construct<FloatItem>(2, 1, num),
                INT.AsParser
            )
        ),
        Discard<FloatItem, char>(ConsumeChar(Id, '(')),
        Map(
            converter: val => Construct<FloatItem>(2, 0, val),
            TryRun(
                Id,
                FLOAT.AsParser,
                Map(
                    converter: intval => new FLOAT((double)intval.Value, 32, false),
                    INT.AsParser
                )
            )
        ),
        Discard<FloatItem, char>(ConsumeChar(Id, ')'))
    );
}

public record IntegerItem(INT Value, INT BitSize, bool IsUnsigned) : IntegralItem, IDeclaration<IntegerItem>
{
    public override string ToString() => $"{(IsUnsigned ? "unsigned" : String.Empty)} int{BitSize}({Value})";
    public static Parser<IntegerItem> AsParser => RunAll(
        converter: parts => new IntegerItem(parts[3].Value, parts[1].BitSize, parts[0]?.IsUnsigned ?? false),
        TryRun(
            converter: word => Construct<IntegerItem>(3, 2, word is not null),
            ConsumeWord(Id, "unsigned"),
            Empty<string>()
        ),
        RunAll(
            converter: parts => parts[1],
            skipWhitespace: false,
            Discard<IntegerItem, string>(ConsumeWord(Id, "int")),
            Map(
                converter: num => Construct<IntegerItem>(3, 1, num),
                INT.AsParser
            )
        ),
        Discard<IntegerItem, char>(ConsumeChar(Id, '(')),
        Map(
            converter: val => Construct<IntegerItem>(3, 0, val),
            INT.AsParser
        ),
        Discard<IntegerItem, char>(ConsumeChar(Id, ')'))
    );
}

public record BytearrayItem(ARRAY<BYTE> Bytes) : FieldInit, IDeclaration<BytearrayItem>
{
    public override string ToString() => $"bytearray({Bytes.ToString(' ')})";
    public static Parser<BytearrayItem> AsParser => RunAll(
        converter: parts => new BytearrayItem(parts[2]),
        Discard<ARRAY<BYTE>, string>(ConsumeWord(Core.Id, "bytearray")),
        Discard<ARRAY<BYTE>, char>(ConsumeChar(Core.Id, '(')),
        ARRAY<BYTE>.MakeParser('\0', '\0', '\0'),
        Discard<ARRAY<BYTE>, char>(ConsumeChar(Core.Id, ')'))
    );
}

public record ReferenceItem : FieldInit, IDeclaration<ReferenceItem>
{
    public override string ToString() => "nullref";
    public static Parser<ReferenceItem> AsParser => Map(
        converter: _ => new ReferenceItem(),
        ConsumeWord(Id, "nullref")
    );
}
