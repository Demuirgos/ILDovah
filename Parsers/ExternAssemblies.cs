using static Core;
using static Extensions;

public record ExternAssembly(ExternAssemblyHeader Header, ExternAssemblyBodyMember.Collection Declarations) : IDeclaration<Assembly> {
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
            ExternAssemblyHeader.AsParser
        ),
        Discard<ExternAssembly, string>(ConsumeWord(Core.Id, "{")),
        Map(
            converter: decls => Construct<ExternAssembly>(2, 1, decls),
            ExternAssemblyBodyMember.Collection.AsParser
        ),
        Discard<ExternAssembly, string>(ConsumeWord(Core.Id, "}"))
    );

}

public record ExternAssemblyHeader(DottedName Name, DottedName Alias) : IDeclaration<ExternAssemblyHeader> {
    public override string ToString() => $"{Name} {(Alias is not null ? $"as {Alias} " : String.Empty)}";
    public static Parser<ExternAssemblyHeader> AsParser => RunAll(
        converter: parts => new ExternAssemblyHeader(
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

public record ExternAssemblyBodyMember : IDeclaration<ExternAssemblyBodyMember> {
    public record Collection(ARRAY<ExternAssemblyBodyMember> Members) : IDeclaration<Collection> {
        public override string ToString() => Members.ToString();
        public static Parser<Collection> AsParser => Map(
            converter: members => new Collection(members),
            ARRAY<ExternAssemblyBodyMember>.MakeParser('\0', '\0', '\0')
        );
    }

    public record CustomAttributeMember(CustomAttribute Attribute) : ExternAssemblyBodyMember, IDeclaration<CustomAttributeMember> {
        public override string ToString() => Attribute.ToString();
        public static Parser<CustomAttributeMember> AsParser => Map(
            converter: attr => new CustomAttributeMember(attr),
            CustomAttribute.AsParser
        );
    }

    public record PublicKeyTokenClause(PublicKey.PKTokenClause Token) : ExternAssemblyBodyMember, IDeclaration<PublicKeyTokenClause> {
        public override string ToString() => Token.ToString();
        public static Parser<PublicKeyTokenClause> AsParser => Map(
            converter: token => new PublicKeyTokenClause(token),
            PublicKey.PKTokenClause.AsParser
        );
    }

    public record CultureClauseMember(Culture Culture) : ExternAssemblyBodyMember, IDeclaration<CultureClauseMember> {
        public override string ToString() => Culture.ToString();
        public static Parser<CultureClauseMember> AsParser => Map(
            converter: culture => new CultureClauseMember(culture),
            Culture.AsParser
        );
    }

    public record VersionClauseMember(Version Version) : ExternAssemblyBodyMember, IDeclaration<VersionClauseMember> {
        public override string ToString() => Version.ToString();
        public static Parser<VersionClauseMember> AsParser => Map(
            converter: version => new VersionClauseMember(version),
            Version.AsParser
        );
    }

    public record PublicKeyClauseMember(PublicKey.PKClause Token) : ExternAssemblyBodyMember, IDeclaration<PublicKeyClauseMember> {
        public override string ToString() => Token.ToString();
        public static Parser<PublicKeyClauseMember> AsParser => Map(
            converter: token => new PublicKeyClauseMember(token),
            PublicKey.PKClause.AsParser
        );
    }

    public record HashClauseMember(HashClause Hash) : ExternAssemblyBodyMember, IDeclaration<HashClauseMember> {
        public override string ToString() => Hash.ToString();
        public static Parser<HashClauseMember> AsParser => Map(
            converter: hash => new HashClauseMember(hash),
            HashClause.AsParser
        );
    }

    public static Parser<ExternAssemblyBodyMember> AsParser => RunAll(
        converter: parts => parts[0],
        TryRun(
            converter: attr => Construct<ExternAssemblyBodyMember>(1, 0, attr),
            Cast<ExternAssemblyBodyMember, CustomAttributeMember>(CustomAttributeMember.AsParser),
            Cast<ExternAssemblyBodyMember, PublicKeyTokenClause>(PublicKeyTokenClause.AsParser),
            Cast<ExternAssemblyBodyMember, CultureClauseMember>(CultureClauseMember.AsParser),
            Cast<ExternAssemblyBodyMember, VersionClauseMember>(VersionClauseMember.AsParser),
            Cast<ExternAssemblyBodyMember, PublicKeyClauseMember>(PublicKeyClauseMember.AsParser),
            Cast<ExternAssemblyBodyMember, HashClauseMember>(HashClauseMember.AsParser)
        )
    );
}