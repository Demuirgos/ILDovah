using AttributeDecl;
using IdentifierDecl;
using ResourceDecl;
using RootDecl;
using SecurityDecl;
using static Core;

using static ExtraTools.Extensions;

namespace ExternAssemblyDecl;
public record ExternAssembly(Prefix Header, Member.Collection Declarations) : Declaration, IDeclaration<ExternAssembly>
{
    public override string ToString() => $".assembly extern {Header} \n{{\n{Declarations}\n}}";
    public static Parser<ExternAssembly> AsParser => RunAll(
        converter: parts => new ExternAssembly(
            parts[1].Header,
            parts[3].Declarations
        ),
        Discard<ExternAssembly, string>(ConsumeWord(Core.Id, ".assembly")),
        Discard<ExternAssembly, string>(ConsumeWord(Core.Id, "extern")),
        Map(
            converter: header => Construct<ExternAssembly>(2, 0, header),
            Prefix.AsParser
        ),
        Discard<ExternAssembly, string>(ConsumeWord(Core.Id, "{")),
        Map(
            converter: decls => Construct<ExternAssembly>(2, 1, decls),
            Member.Collection.AsParser
        ),
        Discard<ExternAssembly, string>(ConsumeWord(Core.Id, "}"))
    );
}

public record Prefix(DottedName Name, DottedName Alias) : IDeclaration<Prefix>
{
    public override string ToString() => $"{Name} {(Alias is not null ? $"as {Alias}" : String.Empty)}";
    public static Parser<Prefix> AsParser => RunAll(
        converter: parts => new Prefix(
            parts[1], parts[2]
        ),
        Discard<DottedName, string>(ConsumeWord(Core.Id, ".assembly")),
        DottedName.AsParser,
        TryRun(
            converter: Id,
            RunAll(
                converter: parts => parts[1],
                Discard<DottedName, string>(ConsumeWord(Core.Id, "as")),
                DottedName.AsParser
            ),
            Empty<DottedName>()
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
[WrapParser<PKTokenClause>] public partial record PublicKeyTokenClause : Member, IDeclaration<PublicKeyTokenClause>;
[WrapParser<Culture>] public partial record CultureClauseMember : Member, IDeclaration<CultureClauseMember>;
[WrapParser<Version>] public partial record VersionClauseMember : Member, IDeclaration<VersionClauseMember>;
[WrapParser<PKClause>] public partial record PublicKeyClauseMember : Member, IDeclaration<PublicKeyClauseMember>;
[WrapParser<HashClause>] public partial record HashClauseMember : Member, IDeclaration<HashClauseMember>;
