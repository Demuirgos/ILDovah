using static Core;
using static Extensions;

public record ManifestResource(ManifestResource.Prefix Header, ManifestResource.Member.Collection Declarations) : Declaration, IDeclaration<ManifestResource> {

    public override string ToString() => $".mresource {Header} {{ {Declarations} }}";

    public static Parser<ManifestResource> AsParser => RunAll(
        converter: parts => new ManifestResource(
            parts[1].Header, 
            parts[3].Declarations
        ),
        Discard<ManifestResource, string>(ConsumeWord(Core.Id, ".mresource")),
        Map(
            converter: header => Construct<ManifestResource>(2, 0, header),
            Prefix.AsParser
        ),
        Discard<ManifestResource, string>(ConsumeWord(Core.Id, "{")),
        Map(
            converter: decls => Construct<ManifestResource>(2, 1, decls),
            Member.Collection.AsParser
        ),
        Discard<ManifestResource, string>(ConsumeWord(Core.Id, "}"))
    );

    public record Prefix(String Attribute, DottedName Name) : IDeclaration<Prefix> {
        public override string ToString() => $"{Attribute} {Name}";
        public static Parser<Prefix> AsParser => RunAll(
            converter: parts => new Prefix(
                parts[0].Attribute, 
                parts[1].Name
            ),
            TryRun(
                converter: attr => Construct<Prefix>(2, 0, attr),
                ConsumeWord(Core.Id, "public"),
                ConsumeWord(Core.Id, "private"),
                Empty<string>()
            ),
            Map(
                converter: name => Construct<Prefix>(2, 1, name),
                DottedName.AsParser
            )
        );
    }

    public record Member : IDeclaration<Member> {
        public record Collection(ARRAY<Member> Members) : IDeclaration<Collection> {
            public override string ToString() => Members.ToString();
            public static Parser<Collection> AsParser => Map(
                converter: members => new Collection(members),
                ARRAY<Member>.MakeParser('\0', '\0', '\0')
            );
        }

        public record CustomAttributeMember(CustomAttribute Attribute) : Member, IDeclaration<CustomAttributeMember> {
            public override string ToString() => Attribute.ToString();
            public static Parser<CustomAttributeMember> AsParser => Map(
                converter: attr => new CustomAttributeMember(attr),
                CustomAttribute.AsParser
            );
        }

        public record ExternAssemblyReferenceMember(ExternAssembly.Prefix Reference) : Member, IDeclaration<ExternAssemblyReferenceMember> {
            public override string ToString() => Reference.ToString();
            public static Parser<ExternAssemblyReferenceMember> AsParser => Map(
                converter: reference => new ExternAssemblyReferenceMember(reference),
                ExternAssembly.Prefix.AsParser
            );
        }

        public static Parser<Member> AsParser => TryRun(
            converter: parts => parts,
            Cast<Member, CustomAttributeMember>(CustomAttributeMember.AsParser),
            Cast<Member, ExternAssemblyReferenceMember>(ExternAssemblyReferenceMember.AsParser)
        );
    }
}