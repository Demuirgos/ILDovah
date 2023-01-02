using System.Reflection.Emit;
using static Core;
using static Extensions;


public record ExternClassHeader(ExportAttribute.Collection Attribute, DottedName Name) : IDeclaration<ExternClassHeader> {
    public override string ToString() => $".class {Attribute} {Name}";

    public enum ExternClassType {
        ClassExtern,
        TypeForwarder // has forwarder attribute and body is an extern assembly reference
    }
    public static Parser<ExternClassHeader> AsParser => RunAll(
        converter: parts => new ExternClassHeader(parts[2].Attribute, parts[3].Name),
        Discard<ExternClassHeader, string>(ConsumeWord(Core.Id, ".class")),
        Discard<ExternClassHeader, string>(ConsumeWord(Core.Id, "extern")),
        Map(
            converter: attr => Construct<ExternClassHeader>(2, 0, attr),
            ExportAttribute.Collection.AsParser
        ),
        Map(
            converter: name => Construct<ExternClassHeader>(2, 1, name),
            DottedName.AsParser
        )
    );
}

public record ExternClassMember : IDeclaration<ExternClassMember> {

    // Note(Aymen) : Add a an extern assembly reference
    public record FileExternClassMember(FileReference File) : ExternClassMember, IDeclaration<FileExternClassMember> {
        public override string ToString() => File.ToString();
        public static Parser<FileExternClassMember> AsParser => Map(
            converter: file => new FileExternClassMember(file),
            FileReference.AsParser
        );
    }

    public record NamedExternClassMember(DottedName Name) : ExternClassMember, IDeclaration<NamedExternClassMember> {
        public override string ToString() => $".class extern {Name}";
        public static Parser<NamedExternClassMember> AsParser => RunAll(
            converter: name => new NamedExternClassMember(name[2]),
            Discard<DottedName, string>(ConsumeWord(Core.Id, ".class")),
            Discard<DottedName, string>(ConsumeWord(Core.Id, "extern")),
            DottedName.AsParser
        );
    }

    public record CustomExternClassMember(CustomAttribute Attribute) : ExternClassMember, IDeclaration<CustomExternClassMember> {
        public override string ToString() => Attribute.ToString();
        public static Parser<CustomExternClassMember> AsParser => RunAll(
            converter: attr => new CustomExternClassMember(attr[1]),
            Discard<CustomAttribute, string>(ConsumeWord(Core.Id, ".custom")),
            CustomAttribute.AsParser
        );
    }

    public static Parser<ExternClassMember> AsParser => TryRun(
        converter: Id,
        Cast<ExternClassMember, FileExternClassMember>(FileExternClassMember.AsParser),
        Cast<ExternClassMember, NamedExternClassMember>(NamedExternClassMember.AsParser),
        Cast<ExternClassMember, CustomExternClassMember>(CustomExternClassMember.AsParser)
    );
}