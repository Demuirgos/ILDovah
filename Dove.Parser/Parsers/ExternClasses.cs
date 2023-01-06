using System.Reflection.Emit;
using static Core;
using static ExtraTools.Extensions;
using RootDecl;

public record ExternClass(ExternClass.Prefix Header, ExternClass.Member.Collection Members) : Declaration, IDeclaration<ExternClass> {
    public override string ToString() => $".class {Header} {{ {Members} }}";

    public record Prefix(ExportAttribute.Collection Attribute, DottedName Name) : IDeclaration<Prefix> {
        public override string ToString() => $"extern {Attribute} {Name}";
        public static Parser<Prefix> AsParser => RunAll(
            converter: parts => new Prefix(parts[1].Attribute, parts[2].Name),
            Discard<Prefix, string>(ConsumeWord(Core.Id, "extern")),
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

    public record Member : IDeclaration<Member> {

        public record Collection(ARRAY<Member> Members) : IDeclaration<Collection> {
            public override string ToString() => Members.ToString();
            public static Parser<Collection> AsParser => Map(
                converter: arr => new Collection(arr),
                ARRAY<Member>.MakeParser('\0', '\0', '\0')
            );
        }

        public record FileExternClassMember(FileReference File) : Member, IDeclaration<FileExternClassMember> {
            public override string ToString() => File.ToString();
            public static Parser<FileExternClassMember> AsParser => Map(
                converter: file => new FileExternClassMember(file),
                FileReference.AsParser
            );
        }

        public record NamedExternClassMember(DottedName Name) : Member, IDeclaration<NamedExternClassMember> {
            public override string ToString() => $".class extern {Name}";
            public static Parser<NamedExternClassMember> AsParser => RunAll(
                converter: name => new NamedExternClassMember(name[2]),
                Discard<DottedName, string>(ConsumeWord(Core.Id, ".class")),
                Discard<DottedName, string>(ConsumeWord(Core.Id, "extern")),
                DottedName.AsParser
            );
        }

        public record CustomExternClassMember(CustomAttribute Attribute) : Member, IDeclaration<CustomExternClassMember> {
            public override string ToString() => Attribute.ToString();
            public static Parser<CustomExternClassMember> AsParser => RunAll(
                converter: attr => new CustomExternClassMember(attr[1]),
                Discard<CustomAttribute, string>(ConsumeWord(Core.Id, ".custom")),
                CustomAttribute.AsParser
            );
        }

        public static Parser<Member> AsParser => TryRun(
            converter: Id,
            Cast<Member, FileExternClassMember>(FileExternClassMember.AsParser),
            Cast<Member, NamedExternClassMember>(NamedExternClassMember.AsParser),
            Cast<Member, CustomExternClassMember>(CustomExternClassMember.AsParser)
        );
    }

    public static Parser<ExternClass> AsParser => RunAll(
        converter: parts => new ExternClass(parts[1].Header, parts[3].Members),
        Discard<ExternClass, string>(ConsumeWord(Core.Id, ".class")),
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