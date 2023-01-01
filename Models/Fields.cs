using System.Reflection.Emit;
using static Core;
using static Extensions;
public record FieldInit : IDeclaration<FieldInit> {
    public record BoolItem(BOOL Value) : FieldInit, IDeclaration<BoolItem> {
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
    public record StringItem(QSTRING String) : FieldInit, IDeclaration<StringItem> {
        public override string ToString() => String.ToString();
        public static Parser<StringItem> AsParser => Map(
            converter: str => new StringItem(str),
            QSTRING.AsParser
        );
    }

    public record CharItem(INT Unicode) : FieldInit, IDeclaration<CharItem> {
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

    public record IntegralItem : FieldInit, IDeclaration<IntegralItem> {
        private Object _value;
        private IntegralItem(Object value) => _value = value;
        public record FloatItem(FLOAT Value, INT BitSize) : IntegralItem(Value), IDeclaration<FloatItem> {
            public override string ToString() => $"float{BitSize}({Value})";
            public static Parser<FloatItem> AsParser => RunAll(
                converter: parts => new FloatItem(parts[3].Value, parts[1].BitSize), 
                Discard<FloatItem, string>(ConsumeWord(Id, "float")),
                Map(
                    converter: num => Construct<FloatItem>(2, 1, num),
                    INT.AsParser
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

        public record IntegerItem(INT Value, INT BitSize, bool IsUnsigned) : IntegralItem(Value), IDeclaration<IntegerItem> {
            public override string ToString() => $"{(IsUnsigned ? "unsigned" : String.Empty)} int{BitSize}({Value})";
            public static Parser<IntegerItem> AsParser => RunAll(
                converter: parts => new IntegerItem(parts[4].Value, parts[2].BitSize, parts[0]?.IsUnsigned ?? false), 
                TryRun(
                    converter: word => Construct<IntegerItem>(3, 2, word is not null),
                    ConsumeWord(Id, "unsigned"),
                    Empty<string>()
                ),
                Discard<IntegerItem, string>(ConsumeWord(Id, "int")),
                Map(
                    converter: num => Construct<IntegerItem>(3, 1, num),
                    INT.AsParser
                ),
                Discard<IntegerItem, char>(ConsumeChar(Id, '(')),
                Map(
                    converter: val => Construct<IntegerItem>(3, 0, val),
                    INT.AsParser
                ),
                Discard<IntegerItem, char>(ConsumeChar(Id, ')'))
            );
        }
     
        public override string ToString() => _value switch {
            FloatItem f => f.ToString(),
            IntegerItem i => i.ToString(),
            _ => throw new NotImplementedException()
        };
        public static Parser<IntegralItem> AsParser => TryRun(
            Id,
            Cast<IntegralItem, FloatItem>(FloatItem.AsParser),
            Cast<IntegralItem, IntegerItem>(IntegerItem.AsParser)
        );
    }

    public record ReferenceItem : FieldInit, IDeclaration<ReferenceItem> {
        public override string ToString() => "nullref";
        public static Parser<ReferenceItem> AsParser => Map(
            converter: _ => new ReferenceItem(),
            ConsumeWord(Id, "nullref")
        );
    }

    public static Parser<FieldInit> AsParser => TryRun(
        Id,
        Cast<FieldInit, BoolItem>(BoolItem.AsParser),
        Cast<FieldInit, StringItem>(StringItem.AsParser),
        Cast<FieldInit, CharItem>(CharItem.AsParser),
        Cast<FieldInit, IntegralItem>(IntegralItem.AsParser),
        Cast<FieldInit, ReferenceItem>(ReferenceItem.AsParser)
    );
}