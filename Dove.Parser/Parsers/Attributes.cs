using MethodDecl;

using RootDecl;
using System.Text;
using TypeDecl;
using static Core;
using static ExtraTools.Extensions;

namespace AttributeDecl;
public record PropertyAttribute(string Value) : Declaration, IDeclaration<PropertyAttribute>
{
    public record Collection(ARRAY<PropertyAttribute> Attributes) : IDeclaration<Collection>
    {
        public override string ToString() => Attributes.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: items => new Collection(items),
            ARRAY<PropertyAttribute>.MakeParser('\0', '\0', '\0')
        );
    }
    public static String[] ValidNames = { "specialname", "rtspecialname" };
    public override string ToString() => Value;
    public static Parser<PropertyAttribute> AsParser => TryRun(
        converter: (name) => new PropertyAttribute(name),
        ValidNames.Select((name) => ConsumeWord(Id, name)).ToArray()
    );
}

public record FieldAttribute : IDeclaration<FieldAttribute>
{
    public record Collection(ARRAY<FieldAttribute> Attributes) : FieldAttribute, IDeclaration<Collection>
    {
        public override string ToString() => Attributes.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: items => new Collection(items),
            ARRAY<FieldAttribute>.MakeParser('\0', '\0', '\0')
        );
    }
    public record SimpleAttribute(String Name) : FieldAttribute, IDeclaration<SimpleAttribute>
    {
        public static String[] ValidNames = new String[] { "assembly", "famandassem", "family", "famorassem", "initonly", "literal", "notserialized", "private", "compilercontrolled", "public", "rtspecialname", "specialname", "static" };
        public override string ToString() => Name;
        public static Parser<SimpleAttribute> AsParser => TryRun(
            converter: (name) => new SimpleAttribute(name),
            ValidNames.Select((name) => ConsumeWord(Id, name)).ToArray()
        );
    }

    public record MarshalAttribute(NativeType Type) : FieldAttribute, IDeclaration<MarshalAttribute>
    {
        public override string ToString() => $"marshal( {Type} )";
        public static Parser<MarshalAttribute> AsParser => RunAll(
            converter: (vals) => new MarshalAttribute(vals[2]),
            Discard<NativeType, string>(ConsumeWord(Id, "marshal")),
            Discard<NativeType, char>(ConsumeChar(Id, '(')),
            NativeType.AsParser,
            Discard<NativeType, char>(ConsumeChar(Id, ')'))
        );
    }
    public static Parser<FieldAttribute> AsParser => TryRun(
        converter: Id,
        Cast<FieldAttribute, SimpleAttribute>(SimpleAttribute.AsParser),
        Cast<FieldAttribute, MarshalAttribute>(MarshalAttribute.AsParser)
    );
}

public record CustomAttribute(TypeDecl.MethodReference AttributeCtor, ARRAY<BYTE>? Arguments) : Declaration, IDeclaration<CustomAttribute>
{
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(".custom ");
        sb.Append(AttributeCtor);
        if (Arguments is not null)
        {
            sb.Append($"={Arguments}");
        }
        return sb.ToString();
    }
    public static Parser<CustomAttribute> AsParser => RunAll(
        converter: parts => parts[1],
        Discard<CustomAttribute, string>(ConsumeWord(Id, ".custom")),
        Map(
            converter: result => new CustomAttribute(result.Item1, result.Item2),
            If(
                condP: ConsumeIf(TypeDecl.MethodReference.AsParser, methRef => methRef.Name.IsConstructor),
                thenP: TryRun(Id,
                    RunAll(
                        converter: (vals) => vals[1],
                        ConsumeChar((_) => default(ARRAY<BYTE>), '='),
                        Map(Id, ARRAY<BYTE>.MakeParser('(', '\0', ')'))
                    ),
                    Empty<ARRAY<BYTE>>()
                ),
                elseP: Fail<ARRAY<BYTE>>()
            )
        )
    );
}

public record ImplAttribute(String Name, ImplAttribute.ModifierBehaviour Type) : IDeclaration<ImplAttribute>
{
    public enum ModifierBehaviour { Implementation, MemoryManagement, Information }
    public record Collection(ARRAY<ImplAttribute> Attributes) : IDeclaration<Collection>
    {
        public override string ToString() => Attributes.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: (attrs) => new Collection(attrs),
            ARRAY<ImplAttribute>.MakeParser('\0', '\0', '\0')
        );
    }
    private static String[] AttributeWords = { "cil", "native", "forwardref", "internalcall", "managed", "noinlining", "nooptimization", "runtime", "synchronized", "unmanaged" };
    public override string ToString() => Name;
    public static Parser<ImplAttribute> AsParser => TryRun(
        converter: (vals) => new ImplAttribute(vals, BehaviourOf(vals)),
        AttributeWords.Select((word) => ConsumeWord(Id, word)).ToArray()
    );

    private static ModifierBehaviour BehaviourOf(String word) => word switch
    {
        "cil" or "native" or "runtime" => ModifierBehaviour.Implementation,
        "managed" or "unmanaged" => ModifierBehaviour.MemoryManagement,
        _ => ModifierBehaviour.Information,
    };
}

[GenerateParser]
public partial record MethodAttribute : IDeclaration<MethodAttribute>
{
    public record Collection(ARRAY<MethodAttribute> Attributes) : IDeclaration<Collection>
    {
        public override string ToString() => Attributes.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: (attrs) => new Collection(attrs),
            ARRAY<MethodAttribute>.MakeParser('\0', '\0', '\0')
        );
    }

    public enum ModifierBehaviour
    {
        Access, Contract, Interop, Override, Handling,
    }

    public ModifierBehaviour BehaviourOf => this is MethodSimpleAttribute attr ?
        attr.Name switch
        {
            "assembly" or "compilercontrolled" or "famandassem" or "famorassem" or "private" or "family" or "public" => ModifierBehaviour.Access,
            "final" or "hidebysig" or "static" or "virtual" or "strict" => ModifierBehaviour.Contract,
            "newslot" or "abstract" => ModifierBehaviour.Override,
            "rtspecialname" or "specialname" => ModifierBehaviour.Handling,
            "pinvokeimpl" => ModifierBehaviour.Interop,
            _ => throw new System.Diagnostics.UnreachableException()
        }
        : ModifierBehaviour.Interop;
}

public record MethodSimpleAttribute(String Name) : MethodAttribute, IDeclaration<MethodSimpleAttribute>
{
    private static String[] AttributeWords = { "abstract", "assembly", "compilercontrolled", "famandassem", "family", "famorassem", "final", "hidebysig", "newslot", "private", "public", "rtspecialname", "specialname", "static", "virtual", "strict" };
    public override string ToString() => Name;
    public static Parser<MethodSimpleAttribute> AsParser => TryRun(
        converter: (vals) => new MethodSimpleAttribute(vals),
        AttributeWords.Select((word) => ConsumeWord(Id, word)).ToArray()
    );
}

public record DataAttribute(String Name) : IDeclaration<DataAttribute>
{
    private static String[] AttributeWords = { "tls", "cil" };
    public override string ToString() => Name;
    public static Parser<DataAttribute> AsParser => TryRun(
        converter: (vals) => new DataAttribute(vals),
        AttributeWords.Select((word) => ConsumeWord(Id, word)).ToArray()
    );
}
public record MethodPInvokeAttribute(QSTRING Name, QSTRING Alias, PinvAttribute.Collection Attributes) : MethodAttribute, IDeclaration<MethodPInvokeAttribute>
{
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"pinvokeimpl({Name} ");
        if (Alias is not null)
        {
            sb.Append($"as {Alias} ");
        }
        sb.Append(Attributes);
        sb.Append(") ");
        return sb.ToString();
    }

    public static Parser<MethodPInvokeAttribute> AsParser => RunAll(
        converter: (vals) =>
        {
            vals = vals.Where((val) => val is not null).ToArray();
            return new MethodPInvokeAttribute(vals[0].Name, vals[1].Alias, vals[2].Attributes);
        },

        Discard<MethodPInvokeAttribute, string>(ConsumeWord(Id, "pinvokeimpl")),
        Discard<MethodPInvokeAttribute, char>(ConsumeChar(Id, '(')),
        Map((name) => new MethodPInvokeAttribute(name, null, null), QSTRING.AsParser),
        TryRun(
            converter: (vals) => new MethodPInvokeAttribute(null, vals, null),
            RunAll(
                converter: (vals) => vals[1],
                Discard<QSTRING, string>(ConsumeWord(Id, "as")),
                Map(Id, QSTRING.AsParser)
            ),
            Empty<QSTRING>()
        ),
        Map((attrs) => new MethodPInvokeAttribute(null, null, attrs), PinvAttribute.Collection.AsParser),
        Discard<MethodPInvokeAttribute, char>(ConsumeChar(Id, ')'))
    );
}

public record ParamAttribute(string Attribute) : IDeclaration<ParamAttribute>
{
    private static String[] AttributeWords = { "in", "opt", "out" };
    public record Collection(ARRAY<ParamAttribute> Attributes) : IDeclaration<Collection>
    {
        public override string ToString() => Attributes.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: (attrs) => new Collection(attrs),
            ARRAY<ParamAttribute>.MakeParser('\0', '\0', '\0')
        );
    }

    public override string ToString() => $"[{Attribute}]";
    public static Parser<ParamAttribute> AsParser => TryRun(
        converter: (vals) => new ParamAttribute(vals),
        AttributeWords.Select((word) => RunAll(
            converter: (vals) => vals[1],
            Discard<string, char>(ConsumeChar(Id, '[')),
            ConsumeWord(Id, word),
            Discard<string, char>(ConsumeChar(Id, ']'))
        )).ToArray()
    );
}

public record PinvAttribute(string Attribute) : IDeclaration<PinvAttribute>
{
    private static String[] AttributeWords = { "ansi", "autochar", "cdecl", "fastcall", "stdcall", "thiscall", "unicode", "platformapi" };
    public record Collection(ARRAY<PinvAttribute> Attributes) : IDeclaration<Collection>
    {
        public override string ToString() => Attributes.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: (attrs) => new Collection(attrs),
            ARRAY<PinvAttribute>.MakeParser('\0', '\0', '\0')
        );
    }

    public override string ToString() => Attribute;
    public static Parser<PinvAttribute> AsParser => TryRun(
        converter: (vals) => new PinvAttribute(vals),
        AttributeWords.Select((word) => ConsumeWord(Id, word)).ToArray()
    );
}

public record ClassAttribute(String Attribute) : IDeclaration<ClassAttribute>
{
    public record Collection(ARRAY<ClassAttribute> Attributes) : IDeclaration<Collection>
    {
        public override string ToString() => Attributes.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: (attrs) => new Collection(attrs),
            ARRAY<ClassAttribute>.MakeParser('\0', '\0', '\0')
        );
    }

    public override string ToString() => Attribute;
    private static String[] AttributeWords = { "public", "private", "value", "enum", "interface", "sealed", "abstract", "sequential", "explicit", "ansi", "unicode", "autochar", "import", "serializable", "nested", "beforefieldinit", "specialname", "rtspecialname", "auto" };
    private static String[] NestedWords = { "public", "private", "family", "assembly", "famandassem", "famorassem" };
    public static Parser<ClassAttribute> AsParser => TryRun(
        converter: (attr) => new ClassAttribute(attr),
        AttributeWords.Select((word) =>
        {
            if (word != "nested")
                return ConsumeWord(Id, word);
            else
            {
                return RunAll(
                    converter: (vals) => $"{vals[0]} {vals[1]}",
                    ConsumeWord(Id, "nested"),
                    TryRun(
                        converter: Id,
                        NestedWords.Select((nestedWord) => ConsumeWord(Id, nestedWord)).ToArray()
                    )
                );
            }
        }).ToArray()
    );
}

public record GenParamAttribute(string keyword) : IDeclaration<GenParamAttribute>
{
    private static String[] PossibleValues = { "+", "-", "class", "valuetype", "new" };

    public record Collection(ARRAY<GenParamAttribute> Attributes) : IDeclaration<Collection>
    {
        public override string ToString() => Attributes.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: (attrs) => new Collection(attrs),
            ARRAY<GenParamAttribute>.MakeParser('\0', '\0', '\0')
        );
    }

    public override string ToString() => keyword;

    public static Parser<GenParamAttribute> AsParser => TryRun(
        converter: (vals) => new GenParamAttribute(vals),
        PossibleValues.Select((word) => ConsumeWord(Id, word)).ToArray()
    );
}

public record VTFixupAttribute(String keyword) : IDeclaration<VTFixupAttribute>
{
    private static String[] PossibleValues = { "fromunmanaged", "int32", "int64" };
    public record Collection(ARRAY<VTFixupAttribute> Attributes) : IDeclaration<Collection>
    {
        public override string ToString() => Attributes.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: (attrs) => new Collection(attrs),
            ARRAY<VTFixupAttribute>.MakeParser('\0', '\0', '\0')
        );
    }

    public override string ToString() => keyword;
    public static Parser<VTFixupAttribute> AsParser => TryRun(
        converter: (vals) => new VTFixupAttribute(vals),
        PossibleValues.Select((word) => ConsumeWord(Id, word)).ToArray()
    );
}


public record ExportAttribute(String keyword) : IDeclaration<ExportAttribute>
{
    private static String[] PossibleValues = { "public", "nested", "extern", "forwarder" };
    public record Collection(ARRAY<ExportAttribute> Attributes) : IDeclaration<Collection>
    {
        public override string ToString() => Attributes.ToString(' ');
        public static Parser<Collection> AsParser => Map(
            converter: (attrs) => new Collection(attrs),
            ARRAY<ExportAttribute>.MakeParser('\0', '\0', '\0')
        );
    }

    public override string ToString() => keyword;
    public static Parser<ExportAttribute> AsParser => TryRun(
        converter: (vals) => new ExportAttribute(vals),
        PossibleValues.Select((word) => ConsumeWord(Id, word)).ToArray()
    );
}
