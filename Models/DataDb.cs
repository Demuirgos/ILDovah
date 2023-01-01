using static Core;
using static Extensions;
using DataBody = DataItem.Collection;


public record Data(DataLabel? Label, DataBody Body) : IDeclaration<Data>
{
    public static Parser<Data> AsParser => RunAll(
        converter: parts => new Data(parts?[0]?.Label, parts[1].Body),
        TryRun(
            converter: item => new Data(item, null),
            RunAll(
                converter: items => items[0],
                DataLabel.AsParser,
                Discard<DataLabel, char>(ConsumeChar(Id, '='))
            ),
            Empty<DataLabel>()
        ),
        Map(
            converter: body => new Data(null, body), 
            DataBody.AsParser
        )
    );
}

public record DataItem : IDeclaration<DataItem>
{   
    private Object _value;
    public record Collection(ARRAY<DataItem> Items) : IDeclaration<DataBody> {
        public override string ToString() => Items.ToString(',');
        public static Parser<DataBody> AsParser => Map(
            converter: items => new DataBody(items),
            ARRAY<DataItem>.MakeParser('{', ',', '}')
        );
    }

    public record LabelPointer(Identifier Id) : IDeclaration<DataLabel> {
        public override string ToString() => $"&({Id})";
        public static Parser<LabelPointer> AsParser => RunAll(
            converter: parts => new LabelPointer(parts[2]),
            Discard<Identifier, char>(ConsumeChar(Core.Id, '&')),
            Discard<Identifier, char>(ConsumeChar(Core.Id, '(')),
            Identifier.AsParser,
            Discard<Identifier, char>(ConsumeChar(Core.Id, ')'))
        );
    }


    public record StringItem(QSTRING String) : IDeclaration<StringItem> {
        public override string ToString() => $"char*({String})";
        public static Parser<StringItem> AsParser => RunAll(
            converter: parts => new StringItem(parts[2]),
            Discard<QSTRING, string>(ConsumeWord(Core.Id, "char*")),
            Discard<QSTRING, char>(ConsumeChar(Core.Id, '(')),
            QSTRING.AsParser,
            Discard<QSTRING, char>(ConsumeChar(Core.Id, ')'))
        );
    }

    public record IntegralItem(string Typename, long BitSize, INT? ReplicationCount) : IDeclaration<IntegralItem> {
        private Object Value;

        private static Parser<IntegralItem> TryParseMap(string typename) => 
            typename == "int" 
            ? Map (
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
            converter:Id,
            IntegralTypes.Select(typeword => 
                RunAll(
                    converter: parts => {
                        var result = new IntegralItem(parts[0].Typename, parts[1].BitSize, parts[5]?.ReplicationCount);
                        result.Value = parts[3].Value;
                        return result; 
                    },
                    Map(
                        converter: typename => Construct<IntegralItem>(3, 0, typename),
                        ConsumeWord(Core.Id, typeword)
                    ),
                    Map(
                        converter: bitsize => Construct<IntegralItem>(3, 1, bitsize.Value),
                        INT.AsParser
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
    
    public override string ToString() => _value.ToString();
    public static Parser<DataItem> AsParser => TryRun(
        converter: item => new DataItem { _value = item },
        Cast<Object, IntegralItem>(IntegralItem.AsParser),
        Cast<Object, StringItem>(StringItem.AsParser),
        Cast<Object, Bytearray>(Bytearray.AsParser),
        Cast<Object, LabelPointer>(LabelPointer.AsParser)
    );
}