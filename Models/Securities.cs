using static Core;
using static Extensions;

public record SecurityBlock :IDeclaration<SecurityBlock> {
    public record PermissionSet(SecurityAction Action, ARRAY<BYTE> Bytes) : SecurityBlock, IDeclaration<PermissionSet> {
        public override string ToString() => $".permissionset {Action} = ({Bytes.ToString(' ')})";
        public static Parser<PermissionSet> AsParser => RunAll(
            converter: parts => new PermissionSet(parts[1].Action, parts[4].Bytes),
            Discard<PermissionSet, string>(ConsumeWord(Core.Id, ".permissionset")),
            Map(
                converter: action => new PermissionSet(action, null),
                SecurityAction.AsParser
            ),
            Discard<PermissionSet, char>(ConsumeChar(Core.Id, '=')),
            Discard<PermissionSet, char>(ConsumeChar(Core.Id, '(')),
            Map(
                converter: bytes => new PermissionSet(null, bytes),
                ARRAY<BYTE>.MakeParser('\0', '\0', '\0')
            ),
            Discard<PermissionSet, char>(ConsumeChar(Core.Id, ')'))
        );
    }

    public record Permission(SecurityAction Action, TypeReference TypeReference, NameValPair.Collection NameValPairs) : SecurityBlock, IDeclaration<Permission> {
        public override string ToString() => $".permission {Action} {TypeReference} ({NameValPairs})";
        public static Parser<Permission> AsParser => RunAll(
            converter: parts => new Permission(parts[1].Action, parts[2].TypeReference, parts[4].NameValPairs),
            Discard<Permission, string>(ConsumeWord(Core.Id, ".permission")),
            Map(
                converter: action => new Permission(action, null, null),
                SecurityAction.AsParser
            ),
            Map(
                converter: type => new Permission(null, type, null),
                TypeReference.AsParser
            ),
            Discard<Permission, char>(ConsumeChar(Core.Id, '(')),
            Map(
                converter: nameValPairs => new Permission(null, null, nameValPairs),
                NameValPair.Collection.AsParser
            ),
            Discard<Permission, char>(ConsumeChar(Core.Id, ')'))
        );
    }

    public override string ToString() => this switch {
        PermissionSet permissionSet => permissionSet.ToString(),
        Permission permission => permission.ToString(),
        _ => throw new NotImplementedException()
    };

    public static Parser<SecurityBlock> AsParser => TryRun(
        converter: Id,
        Cast<SecurityBlock, PermissionSet>(PermissionSet.AsParser),
        Cast<SecurityBlock, Permission>(Permission.AsParser)
    );
}

public record NameValPair(QSTRING Name, QSTRING Value) : IDeclaration<NameValPair> {
    public record Collection(ARRAY<NameValPair> Items) {
        public override string ToString() => Items.ToString(',');
        public static Parser<Collection> AsParser => Map(
            converter: items => new Collection(items),
            ARRAY<NameValPair>.MakeParser('\0', ',', '\0')
        );
    }
    public override string ToString() => $"{Name}={Value}";
    public static Parser<NameValPair> AsParser => RunAll(
        converter: parts => new NameValPair(parts[0], parts[2]),
        QSTRING.AsParser,
        Discard<QSTRING, char>(ConsumeChar(Core.Id, '=')),
        QSTRING.AsParser
    );
}
public record SecurityAction(String Action) {
    public static String[] ActionsWords = {"assert","demand","deny","inheritcheck","linkcheck","permitonly","reqopt","reqrefuse"};
    public override string ToString() => Action;
    public static Parser<SecurityAction> AsParser => TryRun(
        converter: action => new SecurityAction(action),
        ActionsWords.Select(word => ConsumeWord(Core.Id, word)).ToArray()
    );
}