using AttributeDecl;

using IdentifierDecl;
using ResourceDecl;
using RootDecl;
using SecurityDecl;
using static Core;
using static ExtraTools.Extensions;
namespace AssemblyDecl;
public record Assembly(Prefix Header, Member.Collection Declarations) : Declaration, IDeclaration<Assembly>
{
    public override string ToString() => $".assembly {Header} {{\n{Declarations}\n}}";
    public static Parser<Assembly> AsParser => RunAll(
        converter: parts => new Assembly(
            parts[1].Header,
            parts[3].Declarations
        ),
        Discard<Assembly, string>(ConsumeWord(Core.Id, ".assembly")),
        Map(
            converter: header => Construct<Assembly>(2, 0, header),
            Prefix.AsParser
        ),
        Discard<Assembly, string>(ConsumeWord(Core.Id, "{")),
        Map(
            converter: decls => Construct<Assembly>(2, 1, decls),
            Member.Collection.AsParser
        ),
        Discard<Assembly, string>(ConsumeWord(Core.Id, "}"))
    );
}

[WrapParser<DottedName>] public partial record Prefix : IDeclaration<Prefix>;
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
[WrapParser<SecurityBlock>] public partial record SecurityClauseMember : Member, IDeclaration<SecurityClauseMember>;
[WrapParser<Culture>] public partial record CultureClauseMember : Member, IDeclaration<CultureClauseMember>;
[WrapParser<Version>] public partial record VersionClauseMember : Member, IDeclaration<VersionClauseMember>;
[WrapParser<PKClause>] public partial record PublicKeyClauseMember : Member, IDeclaration<PublicKeyClauseMember>;
[WrapParser<HashClause>] public partial record HashClauseMember : Member, IDeclaration<HashClauseMember>;