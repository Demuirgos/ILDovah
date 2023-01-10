using AttributeDecl;
using IdentifierDecl;
using RootDecl;
using static Core;
using static ExtraTools.Extensions;

namespace ManifestDecl;
public record ManifestResource(Prefix Header, Member.Collection Declarations) : Declaration, IDeclaration<ManifestResource>
{

    public override string ToString() => $".mresource {Header} \n{{\n{Declarations}\n}}";

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
}
public record Prefix(String Attribute, DottedName Name) : IDeclaration<Prefix>
{
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

[GenerateParser]
public partial record Member : IDeclaration<Member>
{
    public record Collection(ARRAY<Member> Members) : IDeclaration<Collection>
    {
        public override string ToString() => Members.ToString('\n');
        public static Parser<Collection> AsParser => Map(
            converter: members => new Collection(members),
            ARRAY<Member>.MakeParser('\0', '\0', '\0')
        );
    }
}
[WrapParser<CustomAttribute>] public partial record CustomAttributeMember : Member, IDeclaration<CustomAttributeMember>;
[WrapParser<ExternAssemblyDecl.Prefix>] public partial record ExternAssemblyReferenceMember : Member, IDeclaration<ExternAssemblyReferenceMember>;