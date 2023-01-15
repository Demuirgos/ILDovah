using RootDecl;
using TypeDecl;

using static Core;
using static ExtraTools.Extensions;

namespace SecurityDecl;
public record HashClause(INT AlgorithmId) : IDeclaration<HashClause>
{
    public override string ToString() => $".hash algorithm {AlgorithmId}";
    public static Parser<HashClause> AsParser => RunAll(
        converter: parts => new HashClause(parts[2].AlgorithmId),
        Discard<HashClause, string>(ConsumeWord(Core.Id, ".hash")),
        Discard<HashClause, string>(ConsumeWord(Core.Id, "algorithm")),
        Map(
            converter: id => Construct<HashClause>(1, 0, id),
            INT.AsParser
        )
    );
}
public record PKClause(ARRAY<BYTE> Bytes) : IDeclaration<PKClause>
{
    public override string ToString() => $".publickey = ({Bytes.ToString(' ')})";
    public static Parser<PKClause> AsParser => RunAll(
        converter: parts => new PKClause(parts[2].Bytes),
        Discard<PKClause, string>(ConsumeWord(Core.Id, ".publickey")),
        Discard<PKClause, char>(ConsumeChar(Core.Id, '=')),
        Map(
            converter: bytes => Construct<PKClause>(1, 0, bytes),
            ARRAY<BYTE>.MakeParser('(', '\0', ')')
        )
    );
}

public record PKTokenClause(ARRAY<BYTE> Bytes) : IDeclaration<PKTokenClause>
{
    public override string ToString() => $".publickeytoken = ({Bytes.ToString(' ')})";
    public static Parser<PKTokenClause> AsParser => RunAll(
        converter: parts => new PKTokenClause(parts[2].Bytes),
        Discard<PKTokenClause, string>(ConsumeWord(Core.Id, ".publickeytoken")),
        Discard<PKTokenClause, char>(ConsumeChar(Core.Id, '=')),
        Map(
            converter: bytes => Construct<PKTokenClause>(1, 0, bytes),
            ARRAY<BYTE>.MakeParser('(', '\0', ')')
        )
    );
}

[GenerateParser]
public partial record SecurityBlock : Declaration, IDeclaration<SecurityBlock>;
public record PermissionSet(SecurityAction Action, ARRAY<BYTE> Bytes) : SecurityBlock, IDeclaration<PermissionSet>
{
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

public record Permission(SecurityAction Action, TypeReference TypeReference, NameValPair.Collection NameValPairs) : SecurityBlock, IDeclaration<Permission>
{
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

public record NameValPair(QSTRING Name, QSTRING Value) : IDeclaration<NameValPair>
{
    public record Collection(ARRAY<NameValPair> Items)
    {
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
public record SecurityAction(String Action)
{
    public static String[] ActionsWords = { "prejitgrant", "prejitdeny", "noncasdemand", "noncaslinkdemand", "noncasinheritance", "reqmin", "request", "assert", "demand", "deny", "inheritcheck", "linkcheck", "permitonly", "reqopt", "reqrefuse" };
    public override string ToString() => Action;
    public static Parser<SecurityAction> AsParser => TryRun(
        converter: action => new SecurityAction(action),
        ActionsWords.Select(word => ConsumeWord(Core.Id, word)).ToArray()
    );
}