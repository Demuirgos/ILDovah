using static Core;
using static Extensions;
public record Class(Class.Prefix Header, Class.Member.Collection Members) : IDeclaration<Class> {
    public override string ToString() => $".class {Header} {{ {Members} }}";
    public static Parser<Class> AsParser => RunAll(
        converter : class_ => new Class(class_[1].Header, class_[3].Members),
        Discard<Class, string>(ConsumeWord(Core.Id, ".class")),
        Map(
            converter: header => Construct<Class>(2, 0, header),
            Class.Prefix.AsParser
        ),
        Discard<Class, char>(ConsumeChar(Core.Id, '{')),
        Map(
            converter: members => Construct<Class>(2, 1, members),
            Member.Collection.AsParser
        ),
        Discard<Class, char>(ConsumeChar(Core.Id, '}'))
    );

    public record Member : IDeclaration<Member> {
        public record Collection(ARRAY<Member> Members) : IDeclaration<Collection> {
            public override string ToString() => Members.ToString(' ');
            public static Parser<Collection> AsParser => Map(
                converter: members => new Collection(members),
                ARRAY<Member>.MakeParser('\0', '\0', '\0')
            );
        }

        public record NestedClass(Class Type) : Member, IDeclaration<NestedClass> {
            public override string ToString() => Type.ToString();
            public static Parser<NestedClass> AsParser => Map(
                converter: type => new NestedClass(type),
                Class.AsParser
            );
        }
        
        public record DataClause(Data Datum) : Member, IDeclaration<DataClause> {
            public override string ToString() => Datum.ToString();
            public static Parser<DataClause> AsParser => Map(
                converter: datum => new DataClause(datum),
                Data.AsParser
            );
        }

        public record CustomAttributeClause(CustomAttribute Attribute) : Member, IDeclaration<CustomAttributeClause> {
            public override string ToString() => $".custom {Attribute}";
            public static Parser<CustomAttributeClause> AsParser => RunAll(
                converter: attr => new CustomAttributeClause(attr[1]),
                Discard<CustomAttribute, string>(ConsumeWord(Core.Id, ".custom")),
                CustomAttribute.AsParser
            );
        }

        public record MethodDefinition(Method Method) : Member, IDeclaration<MethodDefinition> {
            public override string ToString() => Method.ToString();
            public static Parser<MethodDefinition> AsParser => Map(
                converter: method => new MethodDefinition(method),
                Method.AsParser
            );
        }

        public record PropertyDefinition(Property Property) : Member, IDeclaration<PropertyDefinition> {
            public override string ToString() => Property.ToString();
            public static Parser<PropertyDefinition> AsParser => Map(
                converter: prop => new PropertyDefinition(prop),
                Property.AsParser
            );
        }

        public record EventDefinition(Event Event) : Member, IDeclaration<EventDefinition> {
            public override string ToString() => Event.ToString();
            public static Parser<EventDefinition> AsParser => Map(
                converter: evt => new EventDefinition(evt),
                Event.AsParser
            );
        }

        public record FieldDefinition(Field Field) : Member, IDeclaration<FieldDefinition> {
            public override string ToString() => Field.ToString();
            public static Parser<FieldDefinition> AsParser => Map(
                converter: field => new FieldDefinition(field),
                Field.AsParser
            );
        }

        public record SizeClause(INT Sizeof) : Member, IDeclaration<SizeClause> {
            public override string ToString() => $".size {Sizeof} ";
            public static Parser<SizeClause> AsParser => Map(
                converter: size => new SizeClause(size),
                RunAll(
                    converter: size => size[1],
                    Discard<INT, string>(ConsumeWord(Core.Id, ".size")),
                    INT.AsParser
                )
            );
        }

        public record PackingClause(INT Sizeof) : Member, IDeclaration<PackingClause> {
            public override string ToString() => $".pack {Sizeof} ";
            public static Parser<PackingClause> AsParser => Map(
                converter: size => new PackingClause(size),
                RunAll(
                    converter: size => size[1],
                    Discard<INT, string>(ConsumeWord(Core.Id, ".pack")),
                    INT.AsParser
                )
            );
        }

        public record ParamAttributeClause(INT Index) : Member, IDeclaration<ParamAttributeClause> {
            public override string ToString() => $".param type [{Index}]";
            public static Parser<ParamAttributeClause> AsParser => RunAll(
                converter: parts => new ParamAttributeClause(parts[3]),
                Discard<INT, string>(ConsumeWord(Id, ".param")),
                Discard<INT, string>(ConsumeWord(Id, "type")),
                Discard<INT, char>(ConsumeChar(Id, '[')),
                INT.AsParser,
                Discard<INT, char>(ConsumeChar(Id, ']'))
            );            
        }

        public record SecurityClause(SecurityBlock Declaration) : Member, IDeclaration<SecurityClause> {
            public override string ToString() => Declaration.ToString();
            public static Parser<SecurityClause> AsParser => Map(
                converter: declaration => new SecurityClause(declaration),
                SecurityBlock.AsParser
            );
        }

        public record ExternSourceReference(ExternSource Source) : Member, IDeclaration<ExternSourceReference> {
            public override string ToString() => Source.ToString();
            public static Parser<ExternSourceReference> AsParser => Map(
                converter: source => new ExternSourceReference(source),
                ExternSource.AsParser
            );
        }
        
        public record SubstitutionClause(MethodBodyItem.OverrideMethodItem.OverrideMethodSignature Target, MethodBodyItem.OverrideMethodItem.OverrideMethodSignature Substitution) : Member, IDeclaration<SubstitutionClause> {
            public override string ToString() => $".override {Target} with {Substitution}";
            public static Parser<SubstitutionClause> AsParser => RunAll(
                converter: parts => new SubstitutionClause(parts[1].Target, parts[3].Substitution),
                Discard<SubstitutionClause, string>(ConsumeWord(Id, ".override")),
                Map(
                    converter: target => Construct<SubstitutionClause>(2, 0, target),
                    MethodBodyItem.OverrideMethodItem.OverrideMethodSignature.AsParser
                ),
                Discard<SubstitutionClause, string>(ConsumeWord(Id, "with")),
                Map(
                    converter: substitution => Construct<SubstitutionClause>(2, 1, substitution),
                    MethodBodyItem.OverrideMethodItem.OverrideMethodSignature.AsParser
                )
            );
        }

        public static Parser<Member> AsParser => TryRun(Id,
            Cast<Member, DataClause>(DataClause.AsParser),
            Cast<Member, CustomAttributeClause>(CustomAttributeClause.AsParser),
            Cast<Member, PropertyDefinition>(PropertyDefinition.AsParser),
            Cast<Member, EventDefinition>(EventDefinition.AsParser),
            Cast<Member, FieldDefinition>(FieldDefinition.AsParser),
            Cast<Member, SizeClause>(SizeClause.AsParser),
            Cast<Member, PackingClause>(PackingClause.AsParser),
            Cast<Member, ParamAttributeClause>(ParamAttributeClause.AsParser),
            Cast<Member, SecurityClause>(SecurityClause.AsParser),
            Cast<Member, ExternSourceReference>(ExternSourceReference.AsParser),
            Cast<Member, SubstitutionClause>(SubstitutionClause.AsParser),
            Lazy(() => Cast<Member, MethodDefinition>(MethodDefinition.AsParser)),
            Lazy(() => Cast<Member, NestedClass>(NestedClass.AsParser))
        );
    }

    public record Prefix(ClassAttribute.Collection Attributes, Identifier Id, GenericParameter.Collection TypeParameters, Prefix.ExtensionClause Extends, Prefix.ImplementationClause Implements) : IDeclaration<Prefix> {
        public record ExtensionClause(TypeSpecification Type) : IDeclaration<ExtensionClause> {
            public override string ToString() => $"extends {Type}";
            public static Parser<ExtensionClause> AsParser => RunAll(
                converter: spec => new ExtensionClause(spec[1]),
                Discard<TypeSpecification, string>(ConsumeWord(Core.Id, "extends")),
                TypeSpecification.AsParser
            );
        }

        public record ImplementationClause(ARRAY<TypeSpecification> Types) : IDeclaration<ImplementationClause> {
            public override string ToString() => $"implements {Types}";
            public static Parser<ImplementationClause> AsParser => RunAll(
                converter: specs => new ImplementationClause(specs[1]),
                Discard<ARRAY<TypeSpecification>, string>(ConsumeWord(Core.Id, "implements")),
                ARRAY<TypeSpecification>.MakeParser('\0', ',', '\0')
            );
        }
        public override string ToString() => $"{Attributes} {Id}{TypeParameters} {Extends} {Implements}";
        public static Parser<Prefix> AsParser => RunAll(
            converter: header => new Prefix(
                header[0].Attributes, 
                header[1].Id, 
                header[2].TypeParameters, 
                header[3].Extends, 
                header[4].Implements
            ),
            
            Map(
                converter: attrs => Construct<Prefix>(5, 0, attrs),
                ClassAttribute.Collection.AsParser
            ),
            Map(
                converter: id => Construct<Prefix>(5, 1, id),
                Identifier.AsParser
            ),
            TryRun(
                converter: genArgs => Construct<Prefix>(5, 2, genArgs),
                RunAll(
                    converter: typeParams => typeParams[1],
                    Discard<GenericParameter.Collection, char>(ConsumeChar(Core.Id, '<')),
                    GenericParameter.Collection.AsParser,
                    Discard<GenericParameter.Collection, char>(ConsumeChar(Core.Id, '>'))
                ),
                Empty<GenericParameter.Collection>()
            ),
            TryRun(
                converter: ext => Construct<Prefix>(5, 3, ext),
                ExtensionClause.AsParser,
                Empty<ExtensionClause>()
            ),
            TryRun(
                converter: impl => Construct<Prefix>(5, 4, impl),
                ImplementationClause.AsParser,
                Empty<ImplementationClause>()
            )
        );
    }

}

