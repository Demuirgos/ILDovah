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

public record DataItem(DataLabel Label, DataBody Body) : IDeclaration<DataItem>
{
    public record Collection(ARRAY<DataItem> Items) : IDeclaration<DataBody> {
        public override string ToString() => Items.ToString(',');
        public static Parser<DataBody> AsParser => Map(
            converter: items => new DataBody(items),
            ARRAY<DataItem>.MakeParser('{', ',', '}')
        );
    }

    /*
    DdItem ::= 
        ‘&’ ‘(’ Id ‘)’
        | bytearray ‘(’ Bytes ‘)’
        | char ‘*’ ‘(’ QSTRING ‘)’
        | float32 [ ‘(’ Float64 ‘)’ ] [ ‘[’ Int32 ‘]’ ]
        | float64 [ ‘(’ Float64 ‘)’ ] [ ‘[’ Int32 ‘]’ ]
        | int8 [ ‘(’ Int32 ‘)’ ] [‘[’ Int32 ‘]’ ]
        | int16 [ ‘(’ Int32 ‘)’ ] [ ‘[’ Int32 ‘]’ ]
        | int32 [ ‘(’ Int32 ‘)’ ] [‘[’ Int32 ‘]’ ]
        | int64 [ ‘(’ Int64 ‘)’ ] [ ‘[’ Int32 ‘]’ ]
    */

    public static Parser<DataItem> AsParser => throw new NotImplementedException();
}