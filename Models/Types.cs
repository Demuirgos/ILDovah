using System.Runtime.CompilerServices;
using System.Text;
using static Core;

public record NativeType(NativeType Type, bool IsArray, INT Length, INT Supplied) : IDeclaration<NativeType> {
    public override string ToString() {
        StringBuilder sb = new();
        if(Type is not null) {
            sb.Append(Type.ToString());
        }

        if(IsArray) {
            sb.Append("[");
            if(Length is not null) {
                sb.Append($"{Length}");
            }
            if(Supplied is not null) {
                sb.Append($" + {Supplied}");
            }
            sb.Append("]");
        }
        return sb.ToString();
    }
    public record NativeTypePrimitive(String TypeName) : NativeType(null, false, null, null), IDeclaration<NativeTypePrimitive> {
        private static String[] _primitives = new String[] { "[]", "bool", "float32", "float64", "int", "int8", "int16", "int32", "int64", "lpstr", "lpwstr", "method", "unsigned" };
        public override string ToString() => TypeName;
        public static Parser<NativeTypePrimitive> AsParser => TryRun(
            converter: (vals) => new NativeTypePrimitive(vals),
            _primitives.Select((primitive) => {
                if(primitive == "unsigned") {
                    return RunAll(
                        converter: (vals) => $"{vals[0]} {vals[1]}",
                        ConsumeWord(Id, primitive),
                        TryRun(
                            converter: (vals) => vals,
                            _primitives.Take(4..9).Select((primitive2) => ConsumeWord(Id, primitive2)).ToArray()
                        )
                    );
                } else {
                    return ConsumeWord(Id, primitive);
                }
            }).ToArray()
        );
    }

    public static Parser<NativeType> AsParser => RunAll(
        converter: (vals) => vals[1].Aggregate(vals[0][0], (acc, val) => new NativeType(acc, val.IsArray, val.Length, val.Supplied)),
        Map(primType => new[] { primType as NativeType }, NativeTypePrimitive.AsParser),
        RunMany(
            converter: Id,
            0, Int32.MaxValue,
            RunAll(
                converter: (vals) => new NativeType(null, true, vals[1], vals[2]),
                Discard<INT, char>(ConsumeChar(Id, '[')),
                TryRun(Id, Map(Id, INT.AsParser), Empty<INT>()),
                TryRun(Id,
                    RunAll(
                        converter: (vals) => vals[1],
                        Discard<INT, char>(ConsumeChar(Id, '+')),
                        Map(Id ,INT.AsParser)
                    ),
                    Empty<INT>()
                ),
                Discard<INT, char>(ConsumeChar(Id, ']'))
            )
        )
    );
}

public record Type : IDeclaration<Type> {
    // Note : this is incomplete 
    // Add following constructs : 
    /*
    Type := Type ‘&’
            | Type ‘*’ 
            | Type ‘<’ GenArgs ‘>’ 
            | Type ‘[’ [ Bound [ ‘,’ Bound ]*] ‘]’
            | Type modopt ‘(’ TypeReference ‘)’
            | Type modreq ‘(’ TypeReference ‘)’
            | Type pinned
    */
    private Object _type;
    public record TypePrimitive(String TypeName) : Type, IDeclaration<TypePrimitive> {
        private static String[] _primitives = new String[] { "bool","char","class","float32","float64","int8","int16","int32","int64","object","string","typedref","valuetype","void", "unsigned","native" };

        public override string ToString() => TypeName;
        public static Parser<TypePrimitive> AsParser => TryRun(
            converter: (vals) => new TypePrimitive(vals),
            _primitives.Select((primitive) => {
                if(primitive == "unsigned") {
                    return RunAll(
                        converter: (vals) => $"{vals[0]} {vals[1]}",
                        ConsumeWord(Id, primitive),
                        TryRun(
                            converter: (vals) => vals,
                            _primitives.Take(5..8).Select((primitive2) => ConsumeWord(Id, primitive2)).ToArray()
                        )
                    );
                }
                else if(primitive == "native") {
                    return RunAll(
                        converter: (vals) => {
                            StringBuilder sb = new();
                            sb.Append(vals[0]);
                            if(vals[1] is not null) {
                                sb.Append($" {vals[1]}");
                            }
                            sb.Append($" {vals[2]} ");
                            return sb.ToString();
                        },
                        ConsumeWord(Id, primitive),
                        TryRun(Id, ConsumeWord(Id, "unsigned"), Empty<String>()),
                        ConsumeWord(Id, "int")
                    );
                } else {
                    return ConsumeWord(Id, primitive);
                }
            }).ToArray()
        );
    }

    public record GenericTypeParameter(INT Index, GenericTypeParameter.Type TypeParameterType) : Type, IDeclaration<GenericTypeParameter> {
        public enum Type { Method, Class }
        public override string ToString() => $"{(TypeParameterType is Type.Method ? "!!" : "!")}{Index}";
        public static Parser<GenericTypeParameter> AsParser => RunAll(
            converter: (vals) => new GenericTypeParameter(vals[1].Index, vals[0].TypeParameterType),
            TryRun(
                converter: (indicator) => new GenericTypeParameter(null, indicator == "!!" ? Type.Method : Type.Class),
                ConsumeWord(Id, "!!"),
                ConsumeWord(Id,  "!")
            ),
            Map(val => new GenericTypeParameter(val, Type.Class), INT.AsParser)
        );
    }

    public record MethodDefinition(CallConvention CallConvention, Type TypeTarget, Parameter.Collection Parameters) : Type, IDeclaration<MethodDefinition> {
        public override string ToString() {
            StringBuilder sb = new();
            sb.Append($"method {CallConvention} {TypeTarget}* (");
            sb.Append(Parameters.ToString());
            sb.Append(")");
            return sb.ToString();            
        }
        public static Parser<MethodDefinition> AsParser => RunAll(
            converter: parts => new MethodDefinition(parts[1].CallConvention, parts[2].TypeTarget, parts[5].Parameters),
            Discard<MethodDefinition, String>(ConsumeWord(Id, "method")),
            Map(
                converter: part1 => new MethodDefinition(part1, null, null),
                CallConvention.AsParser
            ),
            Map(
                converter: part2 => new MethodDefinition(null, part2, null),
                Type.AsParser
            ),
            Discard<MethodDefinition, char>(ConsumeChar(Id, '*')),
            Discard<MethodDefinition, char>(ConsumeChar(Id, '(')),
            Map(
                converter: part3 => new MethodDefinition(null, null, part3),
                Parameter.Collection.AsParser),
            Discard<MethodDefinition, char>(ConsumeChar(Id, ')'))
        );
    }

    public override string ToString() => throw new NotImplementedException();
    public static Parser<Type> AsParser => throw new NotImplementedException();
}

