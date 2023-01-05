using System.Reflection.Metadata;
using System.Text;
using static Core;
using static Extensions;
public record Method(Method.Prefix Header, Method.Member.Collection Body) : Declaration, IDeclaration<Method> {
    public bool IsConstructor => Header.Name.IsConstructor;
    public bool IsEntrypoint => Body?.Items.Values.Any(item => item.IsEntrypoint) ?? false;
    
    public override string ToString() => $".method {Header} {{ {Body} }}";
    public static Parser<Method> AsParser => RunAll(
        converter: parts => new Method(parts[1].Header, parts[3]?.Body),
        Discard<Method, string>(ConsumeWord(Id, ".method")),
        Map(
            converter : header => Construct<Method>(2, 0, header),
            Prefix.AsParser
        ),
        Discard<Method, char>(ConsumeChar(Id, '{')),
        Map(
            converter: blocks => blocks.Item2,
            If(
                condP: Discard<Method, char>(ConsumeChar(Id, '}')),
                thenP: Empty<Method>(),
                elseP: RunAll(
                    converter: blocks => blocks[0],
                    Map(
                        converter : blocks => Construct<Method>(2, 1, blocks),
                        Member.Collection.AsParser
                    ),
                    Discard<Method, char>(ConsumeChar(Id, '}'))
                )
            )
        )
    );
    public record Prefix(MethodAttribute.Collection MethodAttributes, CallConvention? Convention, Type Type, NativeType? MarshalledType, MethodName Name, GenericParameter.Collection? TypeParameters, Parameter.Collection Parameters, ImplAttribute.Collection ImplementationAttributes) : IDeclaration<Prefix> 
    {
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(MethodAttributes);
            if (Convention != null) {
                sb.Append($" {Convention}");
            }
            sb.Append($" {Type}");
            if (MarshalledType != null) {
                sb.Append($" marshal ({MarshalledType})");
            }
            sb.Append($" {Name}");
            if (TypeParameters != null) {
                sb.Append($" <{TypeParameters}>");
            }
            sb.Append($" ({Parameters})");
            if (ImplementationAttributes != null) {
                sb.Append($" {ImplementationAttributes} ");
            }
            return sb.ToString();
        }
        public static Parser<Prefix> AsParser => RunAll(
            converter: parts => new Prefix(
                parts[0].MethodAttributes,
                parts[1].Convention,
                parts[2].Type,
                parts[3].MarshalledType,
                parts[4].Name,
                parts[5].TypeParameters,
                parts[6].Parameters,
                parts[7].ImplementationAttributes
            ),
            Map(
                converter: attrs => Construct<Prefix>(8, 0, attrs),
                MethodAttribute.Collection.AsParser
            ),
            TryRun(
                converter: conv => Construct<Prefix>(8, 1, conv),
                CallConvention.AsParser
            ),
            Map(
                converter: type => Construct<Prefix>(8, 2, type),
                Type.AsParser
            ),
            TryRun(
                converter: type => Construct<Prefix>(8, 3, type),
                RunAll(
                    converter: parts => parts[2],
                    Discard<NativeType, string>(ConsumeWord(Id, "marshal")),
                    Discard<NativeType, char>(ConsumeChar(Id, '(')),
                    NativeType.AsParser,
                    Discard<NativeType, char>(ConsumeChar(Id, ')'))
                ),
                Empty<NativeType>()
            ),
            Map(
                converter: name => Construct<Prefix>(8, 4, name),
                MethodName.AsParser
            ),
            TryRun(
                converter: genpars => Construct<Prefix>(8, 5, genpars),
                RunAll(
                    converter: pars => pars[1],
                    Discard<GenericParameter.Collection, char>(ConsumeChar(Id, '<')),
                    GenericParameter.Collection.AsParser,
                    Discard<GenericParameter.Collection, char>(ConsumeChar(Id, '>'))
                ),
                Empty<GenericParameter.Collection>()
            ),
            RunAll(
                converter: pars => Construct<Prefix>(8, 6, pars[1]),
                Discard<Parameter.Collection, char>(ConsumeChar(Id, '(')),
                Parameter.Collection.AsParser,
                Discard<Parameter.Collection, char>(ConsumeChar(Id, ')'))
            ),
            Map(
                converter: implattrs => Construct<Prefix>(8, 7, implattrs),
                ImplAttribute.Collection.AsParser
            )
        );
    }

    public record Member(bool IsEntrypoint = false) : IDeclaration<Member> {

        public record InstructionItem(Instruction Instr) : Member, IDeclaration<InstructionItem> {
            public override string ToString() => Instr.ToString();
            public static Parser<InstructionItem> AsParser => Map(
                converter: instr => new InstructionItem(instr),
                Instruction.AsParser
            );
        }


        public record Collection(ARRAY<Member> Items) : IDeclaration<Collection> {
            public override string ToString() => Items.ToString(' ');
            public static Parser<Collection> AsParser => Map(
                converter: items => new Collection(items),
                ARRAY<Member>.MakeParser('\0', '\0', '\0')
            );
        }
        public record EmitByteItem(INT Value) : Member, IDeclaration<EmitByteItem> {
            public override string ToString() => $".emitbyte {Value} ";
            public static Parser<EmitByteItem> AsParser => RunAll(
                converter: parts => new EmitByteItem(parts[1]),
                Discard<INT, string>(ConsumeWord(Id, ".emitbyte")),
                INT.AsParser
            );
        }

        public record MaxStackItem(INT Value) : Member, IDeclaration<MaxStackItem> {
            public override string ToString() => $".maxstack {Value} ";
            public static Parser<MaxStackItem> AsParser => RunAll(
                converter: parts => new MaxStackItem(parts[1]),
                Discard<INT, string>(ConsumeWord(Id, ".emitbyte")),
                INT.AsParser
            );
        } 

        public record CustomAttributeItem(CustomAttribute Attribute) : Member, IDeclaration<CustomAttributeItem> {
            public override string ToString() => $".custom {Attribute} ";
            public static Parser<CustomAttributeItem> AsParser => RunAll(
                converter: parts => new CustomAttributeItem(parts[1]),
                Discard<CustomAttribute, string>(ConsumeWord(Id, ".custom")),
                CustomAttribute.AsParser
            );
        }
        
        public record ParamAttribute(INT Index) : Member, IDeclaration<ParamAttribute> {
            public record GenericParamAttribute(INT Index) : ParamAttribute(Index), IDeclaration<GenericParamAttribute> {
                public override string ToString() => $".param type [{Index}]";
                public static Parser<GenericParamAttribute> AsParser => RunAll(
                    converter: parts => new GenericParamAttribute(parts[3]),
                    Discard<INT, string>(ConsumeWord(Id, ".param")),
                    Discard<INT, string>(ConsumeWord(Id, "type")),
                    Discard<INT, char>(ConsumeChar(Id, '[')),
                    INT.AsParser,
                    Discard<INT, char>(ConsumeChar(Id, ']'))
                );            
            }

            public record InitializeParamAttribute(INT Index, FieldInit Value) : ParamAttribute(Index) {
                public override string ToString() => $".param [{Index}] {(Value is null ? String.Empty : $"= {Value}")}";
                public static Parser<InitializeParamAttribute> AsParser => RunAll(
                    converter: parts => new InitializeParamAttribute(
                        parts[0].Index,
                        parts[1]?.Value
                    ),
                    RunAll(
                        converter : parts => new InitializeParamAttribute(parts[2], null),
                        Discard<INT, string>(ConsumeWord(Id, ".param")),
                        Discard<INT, char>(ConsumeChar(Id, '[')),
                        INT.AsParser,
                        Discard<INT, char>(ConsumeChar(Id, ']'))
                    ),
                    TryRun(
                        converter: finit => new InitializeParamAttribute(null, finit),
                        RunAll(
                            converter: parts => parts[1],
                            Discard<FieldInit, char>(ConsumeChar(Id, '=')),
                            FieldInit.AsParser
                        )
                    )
                );                
            }
        
            public static Parser<ParamAttribute> AsParser => TryRun(
                converter: Id,
                Cast<ParamAttribute, GenericParamAttribute>(GenericParamAttribute.AsParser),
                Cast<ParamAttribute, InitializeParamAttribute>(InitializeParamAttribute.AsParser)
            );
        }
        
        public record LocalsItem(bool IsInit, Local.Collection Signatures) : Member, IDeclaration<LocalsItem> {
            public override string ToString() => $".locals {(IsInit ? "init" : String.Empty)} ({Signatures})";
            public static Parser<LocalsItem> AsParser => RunAll(
                converter: parts => new LocalsItem(parts[0].IsInit, parts[1].Signatures),
                Discard<LocalsItem, string>(ConsumeWord(Id, ".locals")),
                TryRun(
                    converter: result => new LocalsItem(result is null, null),
                    Discard<LocalsItem, string>(ConsumeWord(Id, "init")),
                    Empty<LocalsItem>()
                ),
                RunAll(
                    converter: sigs => new LocalsItem(false, sigs[1]),
                    Discard<Local.Collection, char>(ConsumeChar(Id, '(')),
                    Local.Collection.AsParser,
                    Discard<Local.Collection, char>(ConsumeChar(Id, ')'))
                )
            );
        }

        public record LabelItem(CodeLabel Label) : Member, IDeclaration<LabelItem> {
            public override string ToString() => Label.ToString();
            public static Parser<LabelItem> AsParser => Map(
                converter: label => new LabelItem(label),
                CodeLabel.AsParser
            );
        }
        
        public record OverrideMethodItem(OverrideMethodItem.OverrideMethodSignature Target) : Member, IDeclaration<OverrideMethodItem> {
            public record OverrideMethodSignature : IDeclaration<OverrideMethodSignature> {
                public record OverrideMethodDefault(TypeSpecification Specification, MethodName Name) : OverrideMethodSignature, IDeclaration<OverrideMethodDefault> {
                    public override string ToString() => $"{Specification}::{Name}";
                    public static Parser<OverrideMethodDefault> AsParser => RunAll(
                        converter: parts => new OverrideMethodDefault(parts[0].Specification, parts[2].Name),
                        Map(
                            converter: spec => Construct<OverrideMethodDefault>(2, 0, spec), 
                            TypeSpecification.AsParser
                        ),
                        Discard<OverrideMethodDefault, string>(ConsumeWord(Id, "::")),
                        Map(
                            converter: name => Construct<OverrideMethodDefault>(2, 1, name), 
                            MethodName.AsParser
                        )
                    );
                }

                public record OverrideMethodGeneric(CallConvention Convention, Type Type, TypeSpecification Specification, MethodName Name, GenericTypeArity Arity, Parameter.Collection Parameters) : OverrideMethodSignature, IDeclaration<OverrideMethodGeneric> {
                    public override string ToString() => $"method {Convention} {Type} {Specification}::{Name} {Arity} ({Parameters})";
                    public static Parser<OverrideMethodGeneric> AsParser => RunAll(
                        converter: parts => new OverrideMethodGeneric(
                            parts[1].Convention,
                            parts[2].Type,
                            parts[3].Specification,
                            parts[5].Name,
                            parts[6].Arity,
                            parts[8].Parameters
                        ),
                        TryRun(
                            Id, 
                            Discard<OverrideMethodGeneric, string>(ConsumeWord(Id, "method")),
                            Empty<OverrideMethodGeneric>()
                        ),
                        Map(
                            converter: conv => Construct<OverrideMethodGeneric>(6, 0, conv), 
                            CallConvention.AsParser
                        ),
                        Map(
                            converter: type => Construct<OverrideMethodGeneric>(6, 1, type), 
                            Type.AsParser
                        ),
                        Map(
                            converter: spec => Construct<OverrideMethodGeneric>(6, 2, spec), 
                            TypeSpecification.AsParser
                        ),
                        Discard<OverrideMethodGeneric, string>(ConsumeWord(Id, "::")),
                        Map(
                            converter: name => Construct<OverrideMethodGeneric>(6, 3, name), 
                            MethodName.AsParser
                        ),
                        Map(
                            converter: arity => Construct<OverrideMethodGeneric>(6, 4, arity), 
                            GenericTypeArity.AsParser
                        ),
                        Discard<OverrideMethodGeneric, char>(ConsumeChar(Id, '(')),
                        Map(
                            converter: parameters => Construct<OverrideMethodGeneric>(6, 5, parameters), 
                            Parameter.Collection.AsParser
                        ),
                        Discard<OverrideMethodGeneric, char>(ConsumeChar(Id, ')'))
                    );
                }

                public static Parser<OverrideMethodSignature> AsParser => Map(
                    converter: item => item,
                    TryRun(
                        converter: result => result,
                        Cast<OverrideMethodSignature, OverrideMethodDefault>(OverrideMethodDefault.AsParser),
                        Cast<OverrideMethodSignature, OverrideMethodGeneric>(OverrideMethodGeneric.AsParser)
                    )
                );
            }

            public override string ToString() => $".override {Target}";
        
            public static Parser<OverrideMethodItem> AsParser => RunAll(
                converter: target => new OverrideMethodItem(target[1]),
                Discard<OverrideMethodSignature, string>(ConsumeWord(Id, ".override")),
                OverrideMethodSignature.AsParser
            );

        };

        public record DataBodyItem(Data Data) : Member, IDeclaration<DataBodyItem> {
            public override string ToString() => Data.ToString();
            public static Parser<DataBodyItem> AsParser => Map(
                converter: data => new DataBodyItem(data),
                Data.AsParser
            );
        }

        public record ScopeBlock(Collection Blocks) : Member, IDeclaration<ScopeBlock> {
            public override string ToString() => $"{{ {Blocks} }}";
            public static Parser<ScopeBlock> AsParser => RunAll(
                converter: parts => new ScopeBlock(parts[1]),
                Discard<Member.Collection, char>(ConsumeChar(Id, '{')),
                Member.Collection.AsParser,
                Discard<Member.Collection, char>(ConsumeChar(Id, '}'))
            );
        }

        public record SecurityDeclarationItem(SecurityBlock Declaration) : Member, IDeclaration<SecurityDeclarationItem> {
            public override string ToString() => Declaration.ToString();
            public static Parser<SecurityDeclarationItem> AsParser => Map(
                converter: declaration => new SecurityDeclarationItem(declaration),
                SecurityBlock.AsParser
            );
        }

        public record ExternSourceItem(ExternSource Source) : Member, IDeclaration<ExternSourceItem> {
            public override string ToString() => Source.ToString();
            public static Parser<ExternSourceItem> AsParser => Map(
                converter: source => new ExternSourceItem(source),
                ExternSource.AsParser
            );
        }
        
        public record ExceptionHandlingBlock(StructuralExceptionBlock Block) : Member, IDeclaration<ExceptionHandlingBlock> {
            public override string ToString() => Block.ToString();
            public static Parser<ExceptionHandlingBlock> AsParser => Map(
                converter: block => new ExceptionHandlingBlock(block),
                StructuralExceptionBlock.AsParser
            );
        }
        
        public static Parser<Member> AsParser => TryRun(
            converter: result => result,
            Map(
                converter: item => new Member(true),
                ConsumeWord(Id, ".entrypoint")
            ),
            Cast<Member, EmitByteItem>(EmitByteItem.AsParser),
            Cast<Member, MaxStackItem>(MaxStackItem.AsParser),
            Cast<Member, CustomAttributeItem>(CustomAttributeItem.AsParser),
            Cast<Member, ParamAttribute>(ParamAttribute.AsParser),
            Cast<Member, LocalsItem>(LocalsItem.AsParser),
            Cast<Member, LabelItem>(LabelItem.AsParser),
            Cast<Member, ExternSourceItem>(ExternSourceItem.AsParser),
            Cast<Member, DataBodyItem>(DataBodyItem.AsParser),
            Cast<Member, OverrideMethodItem>(OverrideMethodItem.AsParser),
            Lazy(() => Cast<Member, ScopeBlock>(ScopeBlock.AsParser)),
            Lazy(() => Cast<Member, SecurityDeclarationItem>(SecurityDeclarationItem.AsParser)),
            Lazy(() => Cast<Member, ExceptionHandlingBlock>(ExceptionHandlingBlock.AsParser)),

            Cast<Member, InstructionItem>(InstructionItem.AsParser)
        );
    }

}
public record MethodName(String Name) : IDeclaration<MethodName> {
    public bool IsConstructor => Name == ".ctor";
    public override string ToString() => Name;
    public static Parser<MethodName> AsParser => TryRun(
        converter: (vals) => new MethodName(vals),
        ConsumeWord(Id, ".ctor"),
        ConsumeWord(Id, ".cctor"),
        Map((dname) => dname.ToString(), DottedName.AsParser)
    );
}