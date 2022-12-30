using System.Runtime.CompilerServices;
using System.Text;
using static Core;
using GenArgs = Type.Collection;
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
    private Object _type;
    public record Collection(Type[] Types) : Type, IDeclaration<Collection> {
        public override string ToString() => String.Join(", ", Types.Select((type) => type.ToString()));
        public static Parser<Collection> AsParser => RunAll(
            converter: (bounds) => new Collection(bounds.SelectMany(Id).ToArray()),
            Map(
                converter: (type) => new Type[] { type },
                Type.AsParser
            ),
            RunMany(
                converter: Id,
                0, Int32.MaxValue,
                RunAll(
                    converter: (vals) => vals[1],
                    Discard<Type, string>(ConsumeWord(Id, ",")),
                    Type.AsParser
                )
            )
        );
    }
    
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
    // Maybe try strat : composable subparser only parse their supposed suffix, main parser will just do a RunMany 0..n on a TryRun on all of them 
    public record ComposableType(Type TypeTarget) : Type, IDeclaration<ComposableType> {
        public record ReferenceType(Type TypeTarget, bool IsRawPointer) : ComposableType(TypeTarget), IDeclaration<ReferenceType> {
            public override string ToString() => $"{TypeTarget}{(IsRawPointer ? "*" : "&")}";
            public static Parser<ReferenceType> AsParser => RunAll(
                converter: (vals) => new ReferenceType(vals[0].TypeTarget, vals[1].IsRawPointer),
                Map(val => new ReferenceType(val, false), Type.AsParser),
                TryRun(
                    converter: (vals) => new ReferenceType(null, vals == '*'),
                    ConsumeChar(Id, '*'), ConsumeChar(Id, '&')
                )
            );
        }

        public record BoundedType(Type TypeTarget, Bound.Collection Bounds) : ComposableType(TypeTarget), IDeclaration<BoundedType> {
            public override string ToString() => $"{TypeTarget}[{Bounds}]";
            public static Parser<BoundedType> AsParser => RunAll(
                converter: (vals) => new BoundedType(vals[0].TypeTarget, vals[1].Bounds),
                Map(val => new BoundedType(val, null), Type.AsParser),
                Map(val => new BoundedType(null, val), Bound.Collection.AsParser)
            );
        }

        public record GenericType(Type TypeTarget, GenArgs GenericArguments) : ComposableType(TypeTarget), IDeclaration<GenericType> {
            public override string ToString() => $"{TypeTarget}<{GenericArguments}>";
            public static Parser<GenericType> AsParser => RunAll(
                converter: (vals) => new GenericType(vals[0].TypeTarget, vals[1].GenericArguments),
                Map(val => new GenericType(val, null), Type.AsParser),
                Map(val => new GenericType(null, val), GenArgs.AsParser)
            );
        }

        public record ModifierType(Type TypeTarget, String Modifier, TypeReference ReferencedType) : ComposableType(TypeTarget), IDeclaration<ModifierType> {
            public override string ToString() {
                StringBuilder sb = new();
                sb.Append($"{TypeTarget} {Modifier} ");
                if(ReferencedType is not null) {
                    sb.Append($"({ReferencedType})");
                }
                return sb.ToString();
            }
            public static Parser<ModifierType> AsParser => RunAll(
                converter: (vals) => new ModifierType(vals[0].TypeTarget, vals[1].Modifier, vals[2].ReferencedType),
                Map(val => new ModifierType(val, null, null), Type.AsParser),
                Map(val => new ModifierType(null, val, null), TryRun(Id, ConsumeWord(Id, "modopt"), ConsumeWord(Id, "modreq"))),
                Map(val => new ModifierType(null, null, val), TypeReference.AsParser)
            );
        }
        public static Parser<ComposableType> AsParser => TryRun(
            converter: Id,
            Cast<ComposableType, ReferenceType>(ReferenceType.AsParser),
            Cast<ComposableType, BoundedType>(BoundedType.AsParser),
            Cast<ComposableType, GenericType>(GenericType.AsParser),
            Cast<ComposableType, ModifierType>(ModifierType.AsParser)
        );
    }

    public override string ToString() => throw new NotImplementedException();
    public static Parser<Type> AsParser => throw new NotImplementedException();
}

public record TypeReference() : IDeclaration<TypeReference> {
    public override string ToString() => throw new NotImplementedException();
    public static Parser<TypeReference> AsParser => throw new NotImplementedException();
}