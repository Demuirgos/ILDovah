using System.Reflection.Metadata;
using System.Text;
using static Core;
using static Extensions;
public record MethodDeclaration(bool IsConstructor) : IDeclaration<MethodDeclaration> {
    public override string ToString() => throw new NotImplementedException();
    public static Parser<MethodDeclaration> AsParser => throw new NotImplementedException();
}
public record MethodHeader(MethodAttribute.Collection MethodAttributes, CallConvention? Convention, Type Type, NativeType? MarshalledType, MethodName Name, GenericParameter.Collection? TypeParameters, Parameter.Collection Parameters, ImplAttribute.Collection ImplementationAttributes) : IDeclaration<MethodName> {
    public override string ToString() {
        var sb = new StringBuilder();
        sb.Append(MethodAttributes);
        if (Convention != null) {
            sb.Append($" {Convention}");
        }
        sb.Append(Type);
        if (MarshalledType != null) {
            sb.Append($" marshal ({MarshalledType})");
        }
        sb.Append(Name);
        if (TypeParameters != null) {
            sb.Append($" <{TypeParameters}>");
        }
        sb.Append($" ({Parameters})");
        if (ImplementationAttributes != null) {
            sb.Append($" {ImplementationAttributes} ");
        }
        return sb.ToString();
    }
    public static Parser<MethodHeader> AsParser => RunAll(
        converter: parts => new MethodHeader(
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
            converter: attrs => Construct<MethodHeader>(8, 0, attrs),
            MethodAttribute.Collection.AsParser
        ),
        TryRun(
            converter: conv => Construct<MethodHeader>(8, 1, conv),
            CallConvention.AsParser
        ),
        Map(
            converter: type => Construct<MethodHeader>(8, 2, type),
            Type.AsParser
        ),
        TryRun(
            converter: type => Construct<MethodHeader>(8, 3, type),
            RunAll(
                converter: parts => parts[2],
                Discard<NativeType, string>(ConsumeWord(Id, "marshal")),
                Discard<NativeType, char>(ConsumeChar(Id, '(')),
                NativeType.AsParser,
                Discard<NativeType, char>(ConsumeChar(Id, ')'))
            )
        ),
        Map(
            converter: name => Construct<MethodHeader>(8, 4, name),
            MethodName.AsParser
        ),
        TryRun(
            converter: genpars => Construct<MethodHeader>(8, 5, genpars),
            RunAll(
                converter: pars => pars[1],
                Discard<GenericParameter.Collection, char>(ConsumeChar(Id, '<')),
                GenericParameter.Collection.AsParser,
                Discard<GenericParameter.Collection, char>(ConsumeChar(Id, '>'))
            ),
            Empty<GenericParameter.Collection>()
        ),
        RunAll(
            converter: pars => Construct<MethodHeader>(8, 6, pars[1]),
            Discard<Parameter.Collection, char>(ConsumeChar(Id, '(')),
            Parameter.Collection.AsParser,
            Discard<Parameter.Collection, char>(ConsumeChar(Id, ')'))
        ),
        Map(
            converter: implattrs => Construct<MethodHeader>(8, 7, implattrs),
            ImplAttribute.Collection.AsParser
        )
    );
}

public record MethodName(String Name) : IDeclaration<MethodName> {
    public override string ToString() => Name;
    public static Parser<MethodName> AsParser => TryRun(
        converter: (vals) => new MethodName(vals),
        ConsumeWord(Id, ".ctor"),
        ConsumeWord(Id, ".cctor"),
        Map((dname) => dname.ToString(), DottedName.AsParser)
    );
}

public record MethodBodyItem(bool IsEntrypoint = false) : IDeclaration<MethodBodyItem> {

    /*
    MethodBodyItem ::= 
        | .data DataDecl  
        | .entrypoint 
        
        | Instr 
        | ScopeBlock 
        | SecurityDecl
        | SEHBlock
    */
    public record EmitByteItem(INT Value) : IDeclaration<EmitByteItem> {
        public override string ToString() => $".emitbyte {Value} ";
        public static Parser<EmitByteItem> AsParser => RunAll(
            converter: parts => new EmitByteItem(parts[1]),
            Discard<INT, string>(ConsumeWord(Id, ".emitbyte")),
            INT.AsParser
        );
    }

    public record MaxStackItem(INT Value) : IDeclaration<MaxStackItem> {
        public override string ToString() => $".maxstack {Value} ";
        public static Parser<MaxStackItem> AsParser => RunAll(
            converter: parts => new MaxStackItem(parts[1]),
            Discard<INT, string>(ConsumeWord(Id, ".emitbyte")),
            INT.AsParser
        );
    } 

    public record CustomAttributeItem(CustomAttribute Attribute) : IDeclaration<CustomAttributeItem> {
        public override string ToString() => $".custom {Attribute} ";
        public static Parser<CustomAttributeItem> AsParser => RunAll(
            converter: parts => new CustomAttributeItem(parts[1]),
            Discard<CustomAttribute, string>(ConsumeWord(Id, ".custom")),
            CustomAttribute.AsParser
        );
    }
    
    public record ParamAttribute(INT Index) : IDeclaration<ParamAttribute> {
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
    
    public record LocalsItem(bool IsInit, Local.Collection Signatures) {
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

    public record LabelItem(CodeLabel Label) : IDeclaration<LabelItem> {
        public override string ToString() => Label.ToString();
        public static Parser<LabelItem> AsParser => Map(
            converter: label => new LabelItem(label),
            CodeLabel.AsParser
        );
    }
    
    public record ExternSourceItem(ExternSource Source) : IDeclaration<ExternSourceItem> {
        public override string ToString() => Source.ToString();
        public static Parser<ExternSourceItem> AsParser => Map(
            converter: source => new ExternSourceItem(source),
            ExternSource.AsParser
        );
    }
    
    public record OverrideMethodItem : IDeclaration<OverrideMethodItem> {
        public record OverrideMethodDefault(TypeSpecification Specification, MethodName Name) : OverrideMethodItem, IDeclaration<OverrideMethodDefault> {
            public override string ToString() => $".override {Specification}::{Name}";
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

        public record OverrideMethodGeneric(CallConvention Convention, Type Type, TypeSpecification Specification, MethodName Name, GenericTypeArity Arity, Parameter.Collection Parameters) : OverrideMethodItem, IDeclaration<OverrideMethodGeneric> {
            public override string ToString() => $".override method {Convention} {Type} {Specification}::{Name} {Arity} ({Parameters})";
            public static Parser<OverrideMethodGeneric> AsParser => RunAll(
                converter: parts => new OverrideMethodGeneric(
                    parts[1].Convention,
                    parts[2].Type,
                    parts[3].Specification,
                    parts[5].Name,
                    parts[6].Arity,
                    parts[8].Parameters
                ),
                Discard<OverrideMethodGeneric, string>(ConsumeWord(Id, "method")),
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
    
        public static Parser<OverrideMethodItem> AsParser => RunAll(
            converter: items => items[1],
            Discard<OverrideMethodItem, string>(ConsumeWord(Id, ".override")),
            TryRun(
                converter: result => result,
                Cast<OverrideMethodItem, OverrideMethodDefault>(OverrideMethodDefault.AsParser),
                Cast<OverrideMethodItem, OverrideMethodGeneric>(OverrideMethodGeneric.AsParser)
            )
        );
    };

    public record DataItem(Data Data) : IDeclaration<DataItem> {
        public override string ToString() => Data.ToString();
        public static Parser<DataItem> AsParser => Map(
            converter: data => new DataItem(data),
            Data.AsParser
        );
    }

    public static Parser<MethodBodyItem> AsParser => throw  new NotImplementedException();
}

