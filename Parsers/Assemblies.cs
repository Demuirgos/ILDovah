using static Core;
using static Extensions;

public record Assembly(AssemblyHeader Header, AssemblyBodyMember.Collection Declarations) : IDeclaration<Assembly> {
    public override string ToString() => $".assembly {Header} {{ {Declarations} }}";
    public static Parser<Assembly> AsParser => RunAll(
        converter: parts => new Assembly(
            parts[1].Header, 
            parts[3].Declarations
        ),
        Discard<Assembly, string>(ConsumeWord(Core.Id, ".assembly")),
        Map(
            converter: header => Construct<Assembly>(2, 0, header),
            AssemblyHeader.AsParser
        ),
        Discard<Assembly, string>(ConsumeWord(Core.Id, "{")),
        Map(
            converter: decls => Construct<Assembly>(2, 1, decls),
            AssemblyBodyMember.Collection.AsParser
        ),
        Discard<Assembly, string>(ConsumeWord(Core.Id, "}"))
    );

}

public record AssemblyHeader(DottedName Name) : IDeclaration<AssemblyHeader> {
    public override string ToString() => $"{Name} ";
    public static Parser<AssemblyHeader> AsParser => RunAll(
        converter: parts => new AssemblyHeader(
            parts[1]
        ),
        Discard<DottedName, string>(ConsumeWord(Core.Id, ".assembly")),
        DottedName.AsParser
    );

}

public record AssemblyBodyMember : IDeclaration<AssemblyBodyMember> {
    public record Collection(ARRAY<AssemblyBodyMember> Members) : IDeclaration<Collection> {
        public override string ToString() => Members.ToString();
        public static Parser<Collection> AsParser => Map(
            converter: members => new Collection(members),
            ARRAY<AssemblyBodyMember>.MakeParser('\0', '\0', '\0')
        );
    }

    public record CustomAttributeMember(CustomAttribute Attribute) : AssemblyBodyMember, IDeclaration<CustomAttributeMember> {
        public override string ToString() => Attribute.ToString();
        public static Parser<CustomAttributeMember> AsParser => Map(
            converter: attr => new CustomAttributeMember(attr),
            CustomAttribute.AsParser
        );
    }

    public record SecurityClauseMember(SecurityBlock Clause) : AssemblyBodyMember, IDeclaration<SecurityClauseMember> {
        public override string ToString() => Clause.ToString();
        public static Parser<SecurityClauseMember> AsParser => Map(
            converter: clause => new SecurityClauseMember(clause),
            SecurityBlock.AsParser
        );
    }

    public record CultureClauseMember(Culture Culture) : AssemblyBodyMember, IDeclaration<CultureClauseMember> {
        public override string ToString() => Culture.ToString();
        public static Parser<CultureClauseMember> AsParser => Map(
            converter: culture => new CultureClauseMember(culture),
            Culture.AsParser
        );
    }

    public record VersionClauseMember(Version Version) : AssemblyBodyMember, IDeclaration<VersionClauseMember> {
        public override string ToString() => Version.ToString();
        public static Parser<VersionClauseMember> AsParser => Map(
            converter: version => new VersionClauseMember(version),
            Version.AsParser
        );
    }

    public record PublicKeyClauseMember(PublicKey.PKClause Token) : AssemblyBodyMember, IDeclaration<PublicKeyClauseMember> {
        public override string ToString() => Token.ToString();
        public static Parser<PublicKeyClauseMember> AsParser => Map(
            converter: token => new PublicKeyClauseMember(token),
            PublicKey.PKClause.AsParser
        );
    }

    public record HashClauseMember(HashClause Hash) : AssemblyBodyMember, IDeclaration<HashClauseMember> {
        public override string ToString() => Hash.ToString();
        public static Parser<HashClauseMember> AsParser => Map(
            converter: hash => new HashClauseMember(hash),
            HashClause.AsParser
        );
    }

    public static Parser<AssemblyBodyMember> AsParser => RunAll(
        converter: parts => parts[0],
        TryRun(
            converter: attr => Construct<AssemblyBodyMember>(1, 0, attr),
            Cast<AssemblyBodyMember, CustomAttributeMember>(CustomAttributeMember.AsParser),
            Cast<AssemblyBodyMember, SecurityClauseMember>(SecurityClauseMember.AsParser),
            Cast<AssemblyBodyMember, CultureClauseMember>(CultureClauseMember.AsParser),
            Cast<AssemblyBodyMember, VersionClauseMember>(VersionClauseMember.AsParser),
            Cast<AssemblyBodyMember, PublicKeyClauseMember>(PublicKeyClauseMember.AsParser),
            Cast<AssemblyBodyMember, HashClauseMember>(HashClauseMember.AsParser)
        )
    );

}