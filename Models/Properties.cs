using static Core;
using static Extensions;

public record Property(PropertyHeader Header, PropertyMember.Collection Members) : IDeclaration<Property> {
    public override string ToString() => $".property {Header} {{ {Members} }}";
    public static Parser<Property> AsParser => RunAll(
        converter: parts => new Property(parts[0].Header, parts[1].Members),
        RunAll(
            converter : header => Construct<Property>(2, 0, header[1]),
            Discard<PropertyHeader, string>(ConsumeWord(Core.Id, ".property")),
            PropertyHeader.AsParser
        ),
        RunAll(
            converter: parts => Construct<Property>(2, 1, parts[1]),
            Discard<PropertyMember.Collection, char>(ConsumeChar(Core.Id, '{')),
            PropertyMember.Collection.AsParser,
            Discard<PropertyMember.Collection, char>(ConsumeChar(Core.Id, '}'))
        )
    );
}

public record PropertyHeader(PropertyAttribute.Collection Attributes, CallConvention Convention, Type Type, Identifier Id, Parameter.Collection Parameters) : IDeclaration<PropertyHeader> {
    public override string ToString() => $"{Attributes} {Convention} {Type} {Id}({Parameters})";
    public static Parser<PropertyHeader> AsParser => RunAll(
        converter: parts => new PropertyHeader(
            parts[0].Attributes, 
            parts[1].Convention, 
            parts[2].Type, 
            parts[3].Id, 
            parts[4].Parameters
        ),
        Map(
            converter: attrs => Construct<PropertyHeader>(5, 0, attrs),
            PropertyAttribute.Collection.AsParser
        ),
        Map(
            converter: conv => Construct<PropertyHeader>(5, 1, conv),
            CallConvention.AsParser
        ),
        Map(
            converter: type => Construct<PropertyHeader>(5, 2, type),
            Type.AsParser
        ),
        Map(
            converter: id => Construct<PropertyHeader>(5, 3, id),
            Identifier.AsParser
        ),
        Map(
            converter: pars => Construct<PropertyHeader>(5, 4, pars),
            RunAll(
                converter: parts => parts[1],
                Discard<Parameter.Collection, char>(ConsumeChar(Core.Id, '(')),
                Parameter.Collection.AsParser,
                Discard<Parameter.Collection, char>(ConsumeChar(Core.Id, ')'))
            )
        )
    );
}

public record PropertyMember : IDeclaration<PropertyMember> {
    public record Collection(ARRAY<PropertyMember> Members) : IDeclaration<Collection> {
        public override string ToString() => Members.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: members => new Collection(members),
            ARRAY<PropertyMember>.MakeParser('\0', '\0', '\0')
        );
    }
    public record PropertyAttributeItem(CustomAttribute Attribute) : PropertyMember, IDeclaration<PropertyAttributeItem> {
        public override string ToString() => $".custom {Attribute}";
        public static Parser<PropertyAttributeItem> AsParser => RunAll(
            converter: parts => new PropertyAttributeItem(parts[1]),
            Discard<CustomAttribute, string>(ConsumeWord(Core.Id, ".custom")),
            CustomAttribute.AsParser
        );
    }

    public record ExternalSourceItem(ExternSource Attribute) : PropertyMember, IDeclaration<ExternalSourceItem> {
        public override string ToString() => $".extern {Attribute}";
        public static Parser<ExternalSourceItem> AsParser => RunAll(
            converter: parts => new ExternalSourceItem(parts[1]),
            Discard<ExternSource, string>(ConsumeWord(Core.Id, ".extern")),
            ExternSource.AsParser
        );
    }

    public record SpecialMethodReference(String SpecialName, CallConvention Convention, Type Type, TypeSpecification? Specification, MethodName Name, Parameter.Collection Parameters) : PropertyMember, IDeclaration<SpecialMethodReference> {
        public override string ToString() => $"{SpecialName} {Convention} {(Specification is null ? "" : $"{Specification}::")}{Name}({Parameters})";
        public static string[] SpecialNames = new string[] { ".get", ".other", ".set" };
        public static Parser<SpecialMethodReference> AsParser => RunAll(
            converter:parts => new SpecialMethodReference(
                parts[0].SpecialName, 
                parts[1].Convention, 
                parts[2].Type,
                parts[3].Specification,
                parts[4].Name, 
                parts[5].Parameters
            ),
            TryRun(
                converter: name => Construct<SpecialMethodReference>(6, 0, name),
                SpecialNames.Select(methname => ConsumeWord(Id, methname)).ToArray()
            ),
            Map(
                converter: conv => Construct<SpecialMethodReference>(6, 1, conv),
                CallConvention.AsParser
            ),
            Map(
                converter: type => Construct<SpecialMethodReference>(6, 2, type),
                Type.AsParser
            ),
            TryRun(
                converter: spec => Construct<SpecialMethodReference>(6, 3, spec),
                RunAll(
                    converter: specs => specs[0],
                    TypeSpecification.AsParser,
                    Discard<TypeSpecification, string>(ConsumeWord(Core.Id, "::"))
                ),
                Empty<TypeSpecification>()
            ),
            Map(
                converter: name => Construct<SpecialMethodReference>(6, 4, name),
                MethodName.AsParser
            ),
            RunAll(
                converter: parts => Construct<SpecialMethodReference>(6, 5, parts[1]),
                Discard<Parameter.Collection, char>(ConsumeChar(Core.Id, '(')),
                Parameter.Collection.AsParser,
                Discard<Parameter.Collection, char>(ConsumeChar(Core.Id, ')'))
            )
        );  
    }

    public static Parser<PropertyMember> AsParser => TryRun(Id,
        Cast<PropertyMember, PropertyAttributeItem>(PropertyAttributeItem.AsParser),
        Cast<PropertyMember, ExternalSourceItem>(ExternalSourceItem.AsParser),
        Cast<PropertyMember, SpecialMethodReference>(SpecialMethodReference.AsParser)
    );
} 