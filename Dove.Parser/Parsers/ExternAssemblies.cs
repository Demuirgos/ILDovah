using static Core;
using static Extensions;

public record ExternAssembly(ExternAssembly.Prefix Header, ExternAssembly.Member.Collection Declarations) : Declaration, IDeclaration<Assembly> {
    public override string ToString() => $".assembly extern {Header} {{ {Declarations} }}";
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


    public record Prefix(DottedName Name, DottedName Alias) : IDeclaration<Prefix> {
        public override string ToString() => $"{Name} {(Alias is not null ? $"as {Alias} " : String.Empty)}";
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

        public record PublicKeyTokenClause(PublicKey.PKTokenClause Token) : Member, IDeclaration<PublicKeyTokenClause> {
            public override string ToString() => Token.ToString();
            public static Parser<PublicKeyTokenClause> AsParser => Map(
                converter: token => new PublicKeyTokenClause(token),
                PublicKey.PKTokenClause.AsParser
            );
        }

        public record CultureClauseMember(Culture Culture) : Member, IDeclaration<CultureClauseMember> {
            public override string ToString() => Culture.ToString();
            public static Parser<CultureClauseMember> AsParser => Map(
                converter: culture => new CultureClauseMember(culture),
                Culture.AsParser
            );
        }

        public record VersionClauseMember(Version Version) : Member, IDeclaration<VersionClauseMember> {
            public override string ToString() => Version.ToString();
            public static Parser<VersionClauseMember> AsParser => Map(
                converter: version => new VersionClauseMember(version),
                Version.AsParser
            );
        }

        public record PublicKeyClauseMember(PublicKey.PKClause Token) : Member, IDeclaration<PublicKeyClauseMember> {
            public override string ToString() => Token.ToString();
            public static Parser<PublicKeyClauseMember> AsParser => Map(
                converter: token => new PublicKeyClauseMember(token),
                PublicKey.PKClause.AsParser
            );
        }

        public record HashClauseMember(HashClause Hash) : Member, IDeclaration<HashClauseMember> {
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
                Cast<Member, PublicKeyTokenClause>(PublicKeyTokenClause.AsParser),
                Cast<Member, CultureClauseMember>(CultureClauseMember.AsParser),
                Cast<Member, VersionClauseMember>(VersionClauseMember.AsParser),
                Cast<Member, PublicKeyClauseMember>(PublicKeyClauseMember.AsParser),
                Cast<Member, HashClauseMember>(HashClauseMember.AsParser)
            )
        );
    }
}