using DataDecl;
using MethodDecl;
using RootDecl;
using static Core;
using static ExtraTools.Extensions;
namespace ClassDecl;

public record Class(Prefix Header, Member.Collection Members) : Declaration, IDeclaration<Class>
{
    public override string ToString() => $".class {Header} {{ {Members} }}";
    public static Parser<Class> AsParser => RunAll(
        converter: class_ => new Class(class_[1].Header, class_[3].Members),
        Discard<Class, string>(ConsumeWord(Core.Id, ".class")),
        Map(
            converter: header => Construct<Class>(2, 0, header),
            Prefix.AsParser
        ),
        Discard<Class, char>(ConsumeChar(Core.Id, '{')),
        Map(
            converter: members => Construct<Class>(2, 1, members),
            Member.Collection.AsParser
        ),
        Discard<Class, char>(ConsumeChar(Core.Id, '}'))
    );
}

public record Prefix(ClassAttribute.Collection Attributes, Identifier Id, GenericParameter.Collection TypeParameters, Prefix.ExtensionClause Extends, Prefix.ImplementationClause Implements) : IDeclaration<Prefix>
{
    public record ExtensionClause(TypeSpecification Type) : IDeclaration<ExtensionClause>
    {
        public override string ToString() => $"extends {Type}";
        public static Parser<ExtensionClause> AsParser => RunAll(
            converter: spec => new ExtensionClause(spec[1]),
            Discard<TypeSpecification, string>(ConsumeWord(Core.Id, "extends")),
            TypeSpecification.AsParser
        );
    }

    public record ImplementationClause(ARRAY<TypeSpecification> Types) : IDeclaration<ImplementationClause>
    {
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

[GenerateParser]
public partial record Member : IDeclaration<Member>
{
    public record Collection(ARRAY<Member> Members) : IDeclaration<Collection>
    {
        public override string ToString() => Members.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: members => new Collection(members),
            ARRAY<Member>.MakeParser('\0', '\0', '\0')
        );
    }
}

[WrapParser<Class>] public partial record NestedClass : Member, IDeclaration<NestedClass>;
[WrapParser<Data>] public partial record DataClause : Member, IDeclaration<DataClause>;
[WrapParser<CustomAttribute>] public partial record CustomAttributeClause : Member, IDeclaration<CustomAttributeClause>;
[WrapParser<Method>] public partial record MethodDefinition : Member, IDeclaration<MethodDefinition>;
[WrapParser<Property>] public partial record PropertyDefinition : Member, IDeclaration<PropertyDefinition>;
[WrapParser<Event>] public partial record EventDefinition : Member, IDeclaration<EventDefinition>;
[WrapParser<Field>] public partial record FieldDefinition : Member, IDeclaration<FieldDefinition>;
[WrapParser<SecurityBlock>] public partial record SecurityClause : Member, IDeclaration<SecurityClause>;
[WrapParser<ExternSource>] public partial record ExternSourceReference : Member, IDeclaration<ExternSourceReference>;

public record SizeClause(INT Sizeof) : Member, IDeclaration<SizeClause>
{
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

public record PackingClause(INT Sizeof) : Member, IDeclaration<PackingClause>
{
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

public record ParamAttributeClause(INT Index) : Member, IDeclaration<ParamAttributeClause>
{
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



public record SubstitutionClause(OverrideMethodItem.OverrideMethodSignature Target, OverrideMethodItem.OverrideMethodSignature Substitution) : Member, IDeclaration<SubstitutionClause>
{
    public override string ToString() => $".override {Target} with {Substitution}";
    public static Parser<SubstitutionClause> AsParser => RunAll(
        converter: parts => new SubstitutionClause(parts[1].Target, parts[3].Substitution),
        Discard<SubstitutionClause, string>(ConsumeWord(Id, ".override")),
        Map(
            converter: target => Construct<SubstitutionClause>(2, 0, target),
            OverrideMethodItem.OverrideMethodSignature.AsParser
        ),
        Discard<SubstitutionClause, string>(ConsumeWord(Id, "with")),
        Map(
            converter: substitution => Construct<SubstitutionClause>(2, 1, substitution),
            OverrideMethodItem.OverrideMethodSignature.AsParser
        )
    );
}
