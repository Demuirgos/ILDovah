using static Core;
using static Extensions;
public record Class(ClassHeader Header, ClassMember.Collection Members) : IDeclaration<Class> {
    public override string ToString() => $".class {Header} {{ {Members} }}";
    public static Parser<Class> AsParser => RunAll(
        converter : class_ => new Class(class_[1].Header, class_[3].Members),
        Discard<Class, string>(ConsumeWord(Core.Id, ".class")),
        Map(
            converter: header => Construct<Class>(2, 0, header),
            ClassHeader.AsParser
        ),
        Discard<Class, char>(ConsumeChar(Core.Id, '{')),
        Map(
            converter: members => Construct<Class>(2, 1, members),
            ClassMember.Collection.AsParser
        ),
        Discard<Class, char>(ConsumeChar(Core.Id, '}'))
    );
}

public record ClassHeader(ClassAttribute.Collection Attributes, Identifier Id, GenericParameter.Collection TypeParameters, ClassHeader.ExtensionClause Extends, ClassHeader.ImplementationClause Implements) : IDeclaration<ClassHeader> {
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
    public static Parser<ClassHeader> AsParser => RunAll(
        converter: header => new ClassHeader(
            header[0].Attributes, 
            header[1].Id, 
            header[2].TypeParameters, 
            header[3].Extends, 
            header[4].Implements
        ),
        
        Map(
            converter: attrs => Construct<ClassHeader>(5, 0, attrs),
            ClassAttribute.Collection.AsParser
        ),
        Map(
            converter: id => Construct<ClassHeader>(5, 1, id),
            Identifier.AsParser
        ),
        TryRun(
            converter: genArgs => Construct<ClassHeader>(5, 2, genArgs),
            RunAll(
                converter: typeParams => typeParams[1],
                Discard<GenericParameter.Collection, char>(ConsumeChar(Core.Id, '<')),
                GenericParameter.Collection.AsParser,
                Discard<GenericParameter.Collection, char>(ConsumeChar(Core.Id, '>'))
            ),
            Empty<GenericParameter.Collection>()
        ),
        TryRun(
            converter: ext => Construct<ClassHeader>(5, 3, ext),
            ExtensionClause.AsParser,
            Empty<ExtensionClause>()
        ),
        TryRun(
            converter: impl => Construct<ClassHeader>(5, 4, impl),
            ImplementationClause.AsParser,
            Empty<ImplementationClause>()
        )
    );
}
public record ClassMember : IDeclaration<ClassMember> {
    public record Collection(ARRAY<ClassMember> Members) : IDeclaration<Collection> {
        public override string ToString() => Members.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: members => new Collection(members),
            ARRAY<ClassMember>.MakeParser('\0', '\0', '\0')
        );
    }

    public record NestedClass(Class Type) : ClassMember, IDeclaration<NestedClass> {
        public override string ToString() => Type.ToString();
        public static Parser<NestedClass> AsParser => Map(
            converter: type => new NestedClass(type),
            Class.AsParser
        );
    }
    
    public record DataMember(Data Datum) : ClassMember, IDeclaration<DataMember> {
        public override string ToString() => Datum.ToString();
        public static Parser<DataMember> AsParser => Map(
            converter: datum => new DataMember(datum),
            Data.AsParser
        );
    }

    public record CustomAttributeMember(CustomAttribute Attribute) : ClassMember, IDeclaration<CustomAttributeMember> {
        public override string ToString() => $".custom {Attribute}";
        public static Parser<CustomAttributeMember> AsParser => RunAll(
            converter: attr => new CustomAttributeMember(attr[1]),
            Discard<CustomAttribute, string>(ConsumeWord(Core.Id, ".custom")),
            CustomAttribute.AsParser
        );
    }

    public record MethodMember(Method Method) : ClassMember, IDeclaration<MethodMember> {
        public override string ToString() => Method.ToString();
        public static Parser<MethodMember> AsParser => Map(
            converter: method => new MethodMember(method),
            Method.AsParser
        );
    }

    public record PropertyMember(Property Property) : ClassMember, IDeclaration<PropertyMember> {
        public override string ToString() => Property.ToString();
        public static Parser<PropertyMember> AsParser => Map(
            converter: prop => new PropertyMember(prop),
            Property.AsParser
        );
    }

    public record EventMember(Event Event) : ClassMember, IDeclaration<EventMember> {
        public override string ToString() => Event.ToString();
        public static Parser<EventMember> AsParser => Map(
            converter: evt => new EventMember(evt),
            Event.AsParser
        );
    }

    public record FieldMember(Field Field) : ClassMember, IDeclaration<FieldMember> {
        public override string ToString() => Field.ToString();
        public static Parser<FieldMember> AsParser => Map(
            converter: field => new FieldMember(field),
            Field.AsParser
        );
    }

    public record SizeofMember(INT Sizeof) : ClassMember, IDeclaration<SizeofMember> {
        public override string ToString() => $".size {Sizeof} ";
        public static Parser<SizeofMember> AsParser => Map(
            converter: size => new SizeofMember(size),
            RunAll(
                converter: size => size[1],
                Discard<INT, string>(ConsumeWord(Core.Id, ".size")),
                INT.AsParser
            )
        );
    }

    public record PackingMember(INT Sizeof) : ClassMember, IDeclaration<PackingMember> {
        public override string ToString() => $".pack {Sizeof} ";
        public static Parser<PackingMember> AsParser => Map(
            converter: size => new PackingMember(size),
            RunAll(
                converter: size => size[1],
                Discard<INT, string>(ConsumeWord(Core.Id, ".pack")),
                INT.AsParser
            )
        );
    }

    public record GenericParamAttribute(INT Index) : ClassMember, IDeclaration<GenericParamAttribute> {
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

    public record SecurityDeclarationItem(SecurityBlock Declaration) : ClassMember, IDeclaration<SecurityDeclarationItem> {
        public override string ToString() => Declaration.ToString();
        public static Parser<SecurityDeclarationItem> AsParser => Map(
            converter: declaration => new SecurityDeclarationItem(declaration),
            SecurityBlock.AsParser
        );
    }

    public record ExternSourceItem(ExternSource Source) : ClassMember, IDeclaration<ExternSourceItem> {
        public override string ToString() => Source.ToString();
        public static Parser<ExternSourceItem> AsParser => Map(
            converter: source => new ExternSourceItem(source),
            ExternSource.AsParser
        );
    }
    
    public record SubstitutionClause(MethodBodyItem.OverrideMethodItem.OverrideMethodSignature Target, MethodBodyItem.OverrideMethodItem.OverrideMethodSignature Substitution) : ClassMember, IDeclaration<SubstitutionClause> {
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

    public static Parser<ClassMember> AsParser => TryRun(Id,
        Cast<ClassMember, DataMember>(DataMember.AsParser),
        Cast<ClassMember, CustomAttributeMember>(CustomAttributeMember.AsParser),
        Cast<ClassMember, PropertyMember>(PropertyMember.AsParser),
        Cast<ClassMember, EventMember>(EventMember.AsParser),
        Cast<ClassMember, FieldMember>(FieldMember.AsParser),
        Cast<ClassMember, SizeofMember>(SizeofMember.AsParser),
        Cast<ClassMember, PackingMember>(PackingMember.AsParser),
        Cast<ClassMember, GenericParamAttribute>(GenericParamAttribute.AsParser),
        Cast<ClassMember, SecurityDeclarationItem>(SecurityDeclarationItem.AsParser),
        Cast<ClassMember, ExternSourceItem>(ExternSourceItem.AsParser),
        Cast<ClassMember, SubstitutionClause>(SubstitutionClause.AsParser),
        Lazy(() => Cast<ClassMember, MethodMember>(MethodMember.AsParser)),
        Lazy(() => Cast<ClassMember, NestedClass>(NestedClass.AsParser))
    );
}