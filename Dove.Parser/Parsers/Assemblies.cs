using RootDecl;
using static Core;
using static ExtraTools.Extensions;
public record Assembly(Assembly.Prefix Header, Assembly.Member.Collection Declarations) : Declaration, IDeclaration<Assembly>
{
    public override string ToString() => $".assembly {Header} {{ {Declarations} }}";
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


    public record Prefix(DottedName Name) : IDeclaration<Prefix>
    {
        public override string ToString() => $"{Name} ";
        public static Parser<Prefix> AsParser => RunAll(
            converter: parts => new Prefix(
                parts[1]
            ),
            Discard<DottedName, string>(ConsumeWord(Core.Id, ".assembly")),
            DottedName.AsParser
        );

    }

    public record Member : IDeclaration<Member>
    {
        public record Collection(ARRAY<Member> Members) : IDeclaration<Collection>
        {
            public override string ToString() => Members.ToString();
            public static Parser<Collection> AsParser => Map(
                converter: members => new Collection(members),
                ARRAY<Member>.MakeParser('\0', '\0', '\0')
            );
        }

        public record CustomAttributeMember(CustomAttribute Attribute) : Member, IDeclaration<CustomAttributeMember>
        {
            public override string ToString() => Attribute.ToString();
            public static Parser<CustomAttributeMember> AsParser => Map(
                converter: attr => new CustomAttributeMember(attr),
                CustomAttribute.AsParser
            );
        }

        public record SecurityClauseMember(SecurityBlock Clause) : Member, IDeclaration<SecurityClauseMember>
        {
            public override string ToString() => Clause.ToString();
            public static Parser<SecurityClauseMember> AsParser => Map(
                converter: clause => new SecurityClauseMember(clause),
                SecurityBlock.AsParser
            );
        }

        public record CultureClauseMember(Culture Culture) : Member, IDeclaration<CultureClauseMember>
        {
            public override string ToString() => Culture.ToString();
            public static Parser<CultureClauseMember> AsParser => Map(
                converter: culture => new CultureClauseMember(culture),
                Culture.AsParser
            );
        }

        public record VersionClauseMember(Version Version) : Member, IDeclaration<VersionClauseMember>
        {
            public override string ToString() => Version.ToString();
            public static Parser<VersionClauseMember> AsParser => Map(
                converter: version => new VersionClauseMember(version),
                Version.AsParser
            );
        }

        public record PublicKeyClauseMember(PublicKey.PKClause Token) : Member, IDeclaration<PublicKeyClauseMember>
        {
            public override string ToString() => Token.ToString();
            public static Parser<PublicKeyClauseMember> AsParser => Map(
                converter: token => new PublicKeyClauseMember(token),
                PublicKey.PKClause.AsParser
            );
        }

        public record HashClauseMember(HashClause Hash) : Member, IDeclaration<HashClauseMember>
        {
            public override string ToString() => Hash.ToString();
            public static Parser<HashClauseMember> AsParser => Map(
                converter: hash => new HashClauseMember(hash),
                HashClause.AsParser
            );
        }

        public static Parser<Member> AsParser => RunAll(
            converter: parts => parts[0],
            TryRun(
                converter: attr => Construct<Member>(1, 0, attr),
                Cast<Member, CustomAttributeMember>(CustomAttributeMember.AsParser),
                Cast<Member, SecurityClauseMember>(SecurityClauseMember.AsParser),
                Cast<Member, CultureClauseMember>(CultureClauseMember.AsParser),
                Cast<Member, VersionClauseMember>(VersionClauseMember.AsParser),
                Cast<Member, PublicKeyClauseMember>(PublicKeyClauseMember.AsParser),
                Cast<Member, HashClauseMember>(HashClauseMember.AsParser)
            )
        );

    }
}