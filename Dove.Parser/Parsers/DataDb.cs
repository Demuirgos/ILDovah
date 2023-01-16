using IdentifierDecl;

using LabelDecl;
using RootDecl;

using static Core;
using static ExtraTools.Extensions;

namespace DataDecl;
using DataBody = DbItem.Collection;

public record Data(Prefix? Label, DbItem Body) : Declaration, IDeclaration<Data>
{
    public override string ToString() {
        if (Label is not null)
        {
            return $".data {Label} = {Body}";
        }
        return $".data {Body}";
    }

    public static Parser<Data> AsParser => RunAll(
        converter: parts => new Data(parts?[1]?.Label, parts[2].Body),
        Discard<Data, string>(ConsumeWord(Id, ".data")),
        TryRun(
            converter: item => Construct<Data>(2, 0, item),
            RunAll(
                converter: items => items[0],
                Prefix.AsParser,
                Discard<Prefix, char>(ConsumeChar(Id, '='))
            ),
            Empty<Prefix>()
        ),
        Map(
            converter: body => Construct<Data>(2, 1, body),
            DbItem.AsParser
        )
    );
}

public record Prefix(DataLabel Label, AttributeDecl.DataAttribute Attribute) : IDeclaration<Prefix>
{
    public override string ToString() => $"{Attribute} {Label}";
    public static Parser<Prefix> AsParser => RunAll(
        converter: parts => new Prefix(parts[1].Label, parts[0].Attribute),
        TryRun(
            converter: attr => Construct<Prefix>(2, 1, attr),
            AttributeDecl.DataAttribute.AsParser,
            Empty<AttributeDecl.DataAttribute>()
        ),
        Map(
            converter: label => Construct<Prefix>(2, 0, label),
            DataLabel.AsParser
        )
    );
}

[GenerateParser]
public partial record DbItem : IDeclaration<DbItem>
{
    [GenerationOrderParser(Order.Last)]
    public record Collection(ARRAY<DbItem> Items) : DbItem, IDeclaration<DataBody>
    {
        public override string ToString() => Items.ToString(',');
        public static Parser<DataBody> AsParser => Map(
            converter: items => new DataBody(items),
            ARRAY<DbItem>.MakeParser(new ARRAY<DbItem>.ArrayOptions {
                Delimiters = ('{', ',', '}')
            })
        );
    }
}
public record LabelPointer(Identifier Id) : DbItem, IDeclaration<DataLabel>
{
    public override string ToString() => $"&({Id})";
    public static Parser<LabelPointer> AsParser => RunAll(
        converter: parts => new LabelPointer(parts[2]),
        Discard<Identifier, char>(ConsumeChar(Core.Id, '&')),
        Discard<Identifier, char>(ConsumeChar(Core.Id, '(')),
        Identifier.AsParser,
        Discard<Identifier, char>(ConsumeChar(Core.Id, ')'))
    );
}

public record BytearrayItem(ARRAY<BYTE> Bytes) : DbItem, IDeclaration<BytearrayItem>
{
    public override string ToString() => $"bytearray{Bytes.ToString(' ')}";
    public static Parser<BytearrayItem> AsParser => RunAll(
        converter: parts => new BytearrayItem(parts[1]),
        Discard<ARRAY<BYTE>, string>(ConsumeWord(Core.Id, "bytearray")),
        ARRAY<BYTE>.MakeParser(new ARRAY<BYTE>.ArrayOptions {
            Delimiters = ('(', '\0', ')')
        })
    );
}

public record StringItem(QSTRING String) : DbItem, IDeclaration<StringItem>
{
    public override string ToString() => $"char*({String})";
    public static Parser<StringItem> AsParser => RunAll(
        converter: parts => new StringItem(parts[2]),
        Discard<QSTRING, string>(ConsumeWord(Core.Id, "char*")),
        Discard<QSTRING, char>(ConsumeChar(Core.Id, '(')),
        QSTRING.AsParser,
        Discard<QSTRING, char>(ConsumeChar(Core.Id, ')'))
    );
}

[GenerationOrderParser(Order.Middle)]
public record IntegralItem(string Typename, long BitSize, INT? ReplicationCount) : DbItem, IDeclaration<IntegralItem>
{
    private Object Value;

    private static Parser<IntegralItem> TryParseMap(string typename) =>
        typename == "int"
        ? Map(
            converter: value => new IntegralItem(typename, default, null) { Value = value },
            INT.AsParser
        )
        : Map(
            converter: value => new IntegralItem(typename, default, null) { Value = value },
            FLOAT.AsParser
        );

    private static String[] IntegralTypes = new String[] { "float", "int" };
    public override string ToString() => $"{Typename}{BitSize}({Value}){(ReplicationCount is null ? string.Empty : $"[{ReplicationCount}]")}";
    public static Parser<IntegralItem> AsParser => TryRun(
        converter: Id,
        IntegralTypes.Select(typeword =>
            RunAll(
                converter: parts =>
                {
                    var result = new IntegralItem(parts[0].Typename, parts[0].BitSize, parts[4]?.ReplicationCount);
                    result.Value = parts[2].Value;
                    return result;
                },
                RunAll(
                    converter: items => new IntegralItem(items[0].Typename, items[1].BitSize, null),
                    skipWhitespace: false,
                    Map(
                        converter: typename => Construct<IntegralItem>(3, 0, typename),
                        ConsumeWord(Core.Id, typeword)
                    ),
                    Map(
                        converter: bitsize => Construct<IntegralItem>(3, 1, bitsize.Value),
                        INT.AsParser
                    )
                ),
                Discard<IntegralItem, char>(ConsumeChar(Core.Id, '(')),
                IntegralItem.TryParseMap(typeword),
                Discard<IntegralItem, char>(ConsumeChar(Core.Id, ')')),
                TryRun(
                    converter: item => Construct<IntegralItem>(3, 2, item),
                    RunAll(
                        converter: items => items[1],
                        Discard<INT, char>(ConsumeChar(Core.Id, '[')),
                        INT.AsParser,
                        Discard<INT, char>(ConsumeChar(Core.Id, ']'))
                    ),
                    Empty<INT>()
                )
            )
        ).ToArray()
    );
}
