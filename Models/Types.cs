using System.Text;
using static Core;
using GenArgs = Type.Collection;

public record TypeSpecification() : IDeclaration<TypeSpecification> {
    private Object _value;
    public record NamedModuleSpecification(DottedName Name, bool IsModule) : IDeclaration<NamedModuleSpecification> {
        public override string ToString() => $"{(IsModule ? ".module " : "")} {Name}";
        public static Parser<NamedModuleSpecification> AsParser => RunAll(
            converter: (vals) => new NamedModuleSpecification(vals[1].Name, vals[0].IsModule),
            TryRun(
                converter: (module) => new NamedModuleSpecification(null, module is not null),
                Discard<NamedModuleSpecification, string>(ConsumeWord(Id, ".module"))
            ),
            Map(
                converter: (name) => new NamedModuleSpecification(name, false),
                DottedName.AsParser
            )
        );
    }
    public override string ToString() => _value switch {
        Type t => t.ToString(),
        TypeReference t => t.ToString(),
        NamedModuleSpecification t => $"[{t}]",
        _ => throw new System.Diagnostics.UnreachableException()
    };
    public static Parser<TypeSpecification> AsParser => TryRun(
        converter : Id,
        Map(
            converter : (type) => new TypeSpecification { _value = type },
            Type.AsParser
        ),
        Map(
            converter : (type) => new TypeSpecification { _value = type },
            TypeReference.AsParser
        ),
        Map(
            converter : (type) => new TypeSpecification { _value = type },
            RunAll(
                converter : (vals) => new TypeSpecification { _value = vals[1] },
                Discard<NamedModuleSpecification, string>(ConsumeWord(Id, "[")),
                NamedModuleSpecification.AsParser,
                Discard<NamedModuleSpecification, string>(ConsumeWord(Id, "]"))
            )
        )
    );
}

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
            0, Int32.MaxValue, true,
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
    public override string ToString() {
        StringBuilder sb = new();
        sb.Append($"{Basic} ");
        if(Suffixes is not null) {
            sb.Append(String.Join(" ", Suffixes.Select((suffix) => suffix.ToString())));
        }
        return sb.ToString();
    }
    public Type.TypePrefix Basic {get; set;}
    public Type.TypeSuffix[] Suffixes {get; set;}
    public record TypePrefix : IDeclaration<TypePrefix> {
        public record TypePrimitive(String TypeName) : TypePrefix, IDeclaration<TypePrimitive> {
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

        public record GenericTypeParameter(INT Index, GenericTypeParameter.Type TypeParameterType) : TypePrefix, IDeclaration<GenericTypeParameter> {
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

        public record MethodDefinition(CallConvention CallConvention, Type TypeTarget, Parameter.Collection Parameters) : TypePrefix, IDeclaration<MethodDefinition> {
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
                    Lazy(() => Type.AsParser)
                ),
                Discard<MethodDefinition, char>(ConsumeChar(Id, '*')),
                Discard<MethodDefinition, char>(ConsumeChar(Id, '(')),
                Map(
                    converter: part3 => new MethodDefinition(null, null, part3),
                    Parameter.Collection.AsParser),
                Discard<MethodDefinition, char>(ConsumeChar(Id, ')'))
            );
        }
        public static Parser<TypePrefix> AsParser => TryRun(
            converter: Id,
            Cast<TypePrefix, TypePrimitive>(TypePrimitive.AsParser),
            Cast<TypePrefix, GenericTypeParameter>(GenericTypeParameter.AsParser),
            Lazy(() => Cast<TypePrefix, MethodDefinition>(MethodDefinition.AsParser))
        );
    }
    
    public record Collection(ARRAY<Type> Types) : Type, IDeclaration<Collection> {
        public override string ToString() => Types.ToString();
        public static Parser<Collection> AsParser => Map(
            converter: (types) => new Collection(types),
            ARRAY<Type>.MakeParser('\0', ',', '\0')
        );
    }
    
    public record TypeSuffix : Type, IDeclaration<TypeSuffix> {
        public record ReferenceTypeSuffix(bool IsRawPointer) : TypeSuffix, IDeclaration<ReferenceTypeSuffix> {
            public override string ToString() => IsRawPointer ? "*" : "&";
            public static Parser<ReferenceTypeSuffix> AsParser => TryRun(
                converter: (vals) => new ReferenceTypeSuffix(vals == '*'),
                ConsumeChar(Id, '*'), ConsumeChar(Id, '&')
            );
        }

        public record BoundedTypeSuffix(Bound.Collection Bounds) : TypeSuffix, IDeclaration<BoundedTypeSuffix> {
            public override string ToString() => $"[{Bounds}]";
            public static Parser<BoundedTypeSuffix> AsParser => RunAll(
                converter: (vals) => new BoundedTypeSuffix(vals[1]),
                Discard<Bound.Collection, char>(ConsumeChar(Id, '[')),
                Bound.Collection.AsParser,
                Discard<Bound.Collection, char>(ConsumeChar(Id, ']'))
            );
        }

        public record GenericTypeSuffix(GenArgs GenericArguments) : TypeSuffix, IDeclaration<GenericTypeSuffix> {
            public override string ToString() => $"<{GenericArguments}>";
            public static Parser<GenericTypeSuffix> AsParser => RunAll(
                converter: (vals) => new GenericTypeSuffix(vals[1]),
                Discard<GenArgs, char>(ConsumeChar(Id, '<')),
                Lazy(() => GenArgs.AsParser),
                Discard<GenArgs, char>(ConsumeChar(Id, '>'))
            );
        }

        public record ModifierTypeSuffix(String Modifier, TypeReference ReferencedType) :  TypeSuffix, IDeclaration<ModifierTypeSuffix> {
            public override string ToString() {
                StringBuilder sb = new();
                sb.Append($"{Modifier} ");
                if(ReferencedType is not null) {
                    sb.Append($"({ReferencedType})");
                }
                return sb.ToString();
            }
            public static Parser<ModifierTypeSuffix> AsParser => RunAll(
                converter: (vals) => new ModifierTypeSuffix(vals[0].Modifier, vals[1].ReferencedType),
                Map(val => new ModifierTypeSuffix(val, null), TryRun(Id, ConsumeWord(Id, "modopt"), ConsumeWord(Id, "modreq"))),
                Map(val => new ModifierTypeSuffix(null, val), TypeReference.AsParser)
            );
        }
        
        public static Parser<TypeSuffix> AsParser => TryRun(
            converter: Id,
            Cast<TypeSuffix, BoundedTypeSuffix>(BoundedTypeSuffix.AsParser),
            Cast<TypeSuffix, ModifierTypeSuffix>(ModifierTypeSuffix.AsParser),
            Cast<TypeSuffix, ReferenceTypeSuffix>(ReferenceTypeSuffix.AsParser),
            Lazy(() => Cast<TypeSuffix, GenericTypeSuffix>(GenericTypeSuffix.AsParser))
        );
    }

    public static Parser<Type> AsParser => RunAll(
        converter: parts => new Type {
            Basic = parts[0].Basic,
            Suffixes = parts[1].Suffixes
        },
        Map(
            converter: (type) => new Type {
                Basic = type
            },
            TypePrefix.AsParser
        ),
        TryRun(
            converter: (suffixes) => new Type {
                Suffixes = suffixes
            },
            RunMany(
                converter: Id,
                0, Int32.MaxValue, true,
                TypeSuffix.AsParser
            )
        )
    );
}