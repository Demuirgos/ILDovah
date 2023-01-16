using AttributeDecl;
using IdentifierDecl;
using ResourceDecl;

using RootDecl;
using static Core;
using static ExtraTools.Extensions;

namespace ExterClassDecl;

public record ExternClass(Prefix Header, Member.Collection Members) : Declaration, IDeclaration<ExternClass>
{
    public override string ToString() => $".class {Header} \n{{\n{Members}\n}}";

    public static Parser<ExternClass> AsParser => RunAll(
        converter: parts => new ExternClass(parts[2].Header, parts[4].Members),
        Discard<ExternClass, string>(ConsumeWord(Core.Id, ".class")),
        Discard<ExternClass, string>(ConsumeWord(Core.Id, "extern")),
        Map(
            converter: header => Construct<ExternClass>(2, 0, header),
            Prefix.AsParser
        ),
        Discard<ExternClass, string>(ConsumeWord(Core.Id, "{")),
        Map(
            converter: members => Construct<ExternClass>(2, 1, members),
            Member.Collection.AsParser
        ),
        Discard<ExternClass, string>(ConsumeWord(Core.Id, "}"))
    );
}

public record Prefix(ExportAttribute.Collection Attribute, DottedName Name) : IDeclaration<Prefix>
{
    public override string ToString() => $"extern {Attribute} {Name}";
    public static Parser<Prefix> AsParser => RunAll(
        converter: parts => new Prefix(parts[0].Attribute, parts[1].Name),
        Map(
            converter: attr => Construct<Prefix>(2, 0, attr),
            ExportAttribute.Collection.AsParser
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
            converter: arr => new Collection(arr),
            ARRAY<Member>.MakeParser(new ARRAY<Member>.ArrayOptions {
                Delimiters = ('\0', '\0', '\0')
            })
        );
    }
}

[WrapParser<FileReference>] public partial record FileExternClassMember : Member, IDeclaration<FileExternClassMember>;

public record NamedExternClassMember(DottedName Name) : Member, IDeclaration<NamedExternClassMember>
{
    public override string ToString() => $".class extern {Name}";
    public static Parser<NamedExternClassMember> AsParser => RunAll(
        converter: name => new NamedExternClassMember(name[2]),
        Discard<DottedName, string>(ConsumeWord(Core.Id, ".class")),
        Discard<DottedName, string>(ConsumeWord(Core.Id, "extern")),
        DottedName.AsParser
    );
}

public record CustomExternClassMember(CustomAttribute Attribute) : Member, IDeclaration<CustomExternClassMember>
{
    public override string ToString() => Attribute.ToString();
    public static Parser<CustomExternClassMember> AsParser => RunAll(
        converter: attr => new CustomExternClassMember(attr[1]),
        Discard<CustomAttribute, string>(ConsumeWord(Core.Id, ".custom")),
        CustomAttribute.AsParser
    );
}
