using static Core;
using static Extensions;
public record ManifestResourceHeader(String Attribute, DottedName Name, ManifestBodyMember.Collection Declarations) : IDeclaration<ManifestResourceHeader> {

    public override string ToString() => $".mresource {Attribute} {Name} {{ {Declarations} }}";

    public static Parser<ManifestResourceHeader> AsParser => RunAll(
        converter: parts => new ManifestResourceHeader(
            parts[1].Attribute, 
            parts[2].Name, 
            parts[4].Declarations
        ),
        Discard<ManifestResourceHeader, string>(ConsumeWord(Core.Id, ".mresource")),
        TryRun(
            converter: attr => Construct<ManifestResourceHeader>(3, 0, attr),
            ConsumeWord(Core.Id, "public"),
            ConsumeWord(Core.Id, "private"),
            Empty<string>()
        ),
        Map(
            converter: name => Construct<ManifestResourceHeader>(3, 1, name),
            DottedName.AsParser
        ),
        Discard<ManifestResourceHeader, string>(ConsumeWord(Core.Id, "{")),
        Map(
            converter: decls => Construct<ManifestResourceHeader>(3, 2, decls),
            ManifestBodyMember.Collection.AsParser
        ),
        Discard<ManifestResourceHeader, string>(ConsumeWord(Core.Id, "}"))
    );
}

public record ManifestBodyMember : IDeclaration<ManifestBodyMember> {
    public record Collection(ARRAY<ManifestBodyMember> Members) : IDeclaration<Collection> {
        public override string ToString() => Members.ToString();
        public static Parser<Collection> AsParser => Map(
            converter: members => new Collection(members),
            ARRAY<ManifestBodyMember>.MakeParser('\0', '\0', '\0')
        );
    }

    public record CustomAttributeMember(CustomAttribute Attribute) : ManifestBodyMember, IDeclaration<CustomAttributeMember> {
        public override string ToString() => Attribute.ToString();
        public static Parser<CustomAttributeMember> AsParser => Map(
            converter: attr => new CustomAttributeMember(attr),
            CustomAttribute.AsParser
        );
    }

    public record ExternAssemblyReferenceMember(ExternAssemblyHeader Reference) : ManifestBodyMember, IDeclaration<ExternAssemblyReferenceMember> {
        public override string ToString() => Reference.ToString();
        public static Parser<ExternAssemblyReferenceMember> AsParser => Map(
            converter: reference => new ExternAssemblyReferenceMember(reference),
            ExternAssemblyHeader.AsParser
        );
    }



}