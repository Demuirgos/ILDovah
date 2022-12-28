using System.Text;
using static Core;

public record CustomAttribute(MethodDeclaration AttributeCtor, ARRAY<BYTE>? Arguments) : IDeclaration<CustomAttribute> {
    public override string ToString() {
        StringBuilder sb = new();
        sb.Append(AttributeCtor);
        if(Arguments is not null) {
            sb.Append($" = ({Arguments})");
        }
        return sb.ToString();
    }
    public static Parser<CustomAttribute> AsParser => RunAll(
        converter: (vals) => {
            if(vals[0].AttributeCtor.IsConstructor == false)
                throw new Exception("Custom attribute must be a constructor");
            return new CustomAttribute(vals[0].AttributeCtor, vals[1].Arguments);
        },
        Map((methname) => new CustomAttribute(methname, null), MethodDeclaration.AsParser),
        TryRun(
            converter: (vals) => new CustomAttribute(null, vals),
            RunAll(
                converter: (vals) => vals[1],
                ConsumeChar((_) => default(ARRAY<BYTE>), '='),
                Map((bytes) => bytes, ARRAY<BYTE>.MakeParser('(', '\0', ')'))
            ),
            Empty<ARRAY<BYTE>>()
        )
    );
}

public record ImplAttribute(String Name, ImplAttribute.ModifierBehaviour Type) : IDeclaration<ImplAttribute> {

    public enum ModifierBehaviour { Implementation, MemoryManagement, Information }
    public record Collection(ImplAttribute Attributes) : IDeclaration<Collection> {
        public override string ToString() => Attributes.ToString();
        public static Parser<ImplAttribute[]> AsParser => RunMany(
            converter: (attrs) => attrs,
            0, Int32.MaxValue,
            ImplAttribute.AsParser
        );
    }
    private static String[] AttributeWords = { "forwardref", "internalcall", "managed", "noinlining", "nooptimization", "runtime", "synchronized", "unmanaged" };
    public override string ToString() => Name;
    public static Parser<ImplAttribute> AsParser => TryRun(
        converter: (vals) => new ImplAttribute(vals, BehaviourOf(vals)),
        AttributeWords.Select((word) => ConsumeWord(Id, word)).ToArray()
    );

    private static ModifierBehaviour BehaviourOf(String word) => word switch {
        "cil" or "native" or "runtime" => ModifierBehaviour.Implementation,
        "managed" or "unmanaged" => ModifierBehaviour.MemoryManagement,
        _ => ModifierBehaviour.Information,
    };
}

public record MethodAttribute(MethodAttribute.ModifierBehaviour Type) : IDeclaration<MethodAttribute> {
    internal Object Value { get; init; }

    public override string ToString() => Value switch {
        MethodSimpleAttribute simple => simple.ToString(),
        MethodPInvokeAttribute pinvoke => pinvoke.ToString(),
        _ => throw new Exception("Unknown method attribute")
    };

    private static String[] AttributeWords = { "abstract", "assembly", "compilercontrolled", "famandassem", "family", "famorassem", "final", "hidebysig", "newslot", "private", "public", "rtspecialname", "specialname", "static", "virtual", "strict"};
    public record MethodSimpleAttribute(String Name) : IDeclaration<MethodSimpleAttribute> {
        public override string ToString() => Name;
        public static Parser<MethodSimpleAttribute> AsParser => TryRun(
            converter: (vals) => new MethodSimpleAttribute(vals),
            AttributeWords.Select((word) => ConsumeWord(Id, word)).ToArray()
        );
    }
    public record MethodPInvokeAttribute(QSTRING Name, QSTRING Alias, PinvAttribute.Collection Attributes) : MethodAttribute(MethodAttribute.ModifierBehaviour.Interop) , IDeclaration<MethodPInvokeAttribute> {
        public override string ToString() {
            StringBuilder sb = new();
            sb.Append($"pinvokeimpl({Name} ");
            if(Alias is not null) {
                sb.Append($"as {Alias} ");
            }
            sb.Append(Attributes);
            sb.Append(") ");
            return sb.ToString();
        }

        public static Parser<MethodPInvokeAttribute> AsParser => RunAll(
            converter: (vals) => {
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
    public record Collection(MethodAttribute[] Attributes) : IDeclaration<MethodAttribute.Collection> {
        public override string ToString() => String.Join(' ', Attributes.Select((attr) => attr.ToString()));
        public static Parser<MethodAttribute.Collection> AsParser => RunMany(
            converter: (attrs) => new MethodAttribute.Collection(attrs),
            0, Int32.MaxValue,
            MethodAttribute.AsParser
        );
    }

    public enum ModifierBehaviour {
        Access, Contract, Interop, Override, Handling,  
    }

    private static ModifierBehaviour BehaviourOf(String word) => word switch {
        "assembly" or "compilercontrolled" or "famandassem" or "famorassem" or "private" or "family" or "public" => ModifierBehaviour.Access,
        "final" or "hidebysig" or "static" or "virtual" or "strict" => ModifierBehaviour.Contract,
        "newslot" or "abstract" => ModifierBehaviour.Override,
        "rtspecialname" or "specialname" => ModifierBehaviour.Handling,
        "pinvokeimpl" => ModifierBehaviour.Interop,
        _ => throw new Exception("Unknown modifier")
    };

    public static Parser<MethodAttribute> AsParser => TryRun(
        converter: (vals) => new MethodAttribute(vals),
        Map((smplAttr) => new MethodAttribute(BehaviourOf(smplAttr.Name)) { Value = smplAttr }, MethodSimpleAttribute.AsParser),
        Map((pinvAttr) => new MethodAttribute(ModifierBehaviour.Interop) { Value = pinvAttr }, MethodPInvokeAttribute.AsParser)
    );
}

public record ParamAttribute(string Attribute) : IDeclaration<ParamAttribute> {
    private static String[] AttributeWords = { "in", "opt", "out" };
    public record Collection(ParamAttribute[] Attributes) : IDeclaration<ParamAttribute.Collection> {
        public override string ToString() => String.Join(' ', Attributes.Select((attr) => attr.ToString()));
        public static Parser<ParamAttribute.Collection> AsParser => RunMany(
            converter: (attrs) => new ParamAttribute.Collection(attrs),
            0, Int32.MaxValue,
            ParamAttribute.AsParser
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

public record PinvAttribute(string Attribute) : IDeclaration<PinvAttribute> {
    private static String[] AttributeWords = { "ansi", "autochar", "cdecl", "fastcall","stdcall", "thiscall", "unicode","platformapi"};
    public record Collection(PinvAttribute[] Attributes) : IDeclaration<PinvAttribute.Collection> {
        public override string ToString() => String.Join(' ', Attributes.Select((attr) => attr.ToString()));
        public static Parser<PinvAttribute.Collection> AsParser => RunMany(
            converter: (attrs) => new PinvAttribute.Collection(attrs),
            0, Int32.MaxValue,
            PinvAttribute.AsParser
        );
    }

    public override string ToString() => Attribute;
    public static Parser<PinvAttribute> AsParser => TryRun(
        converter: (vals) => new PinvAttribute(vals),
        AttributeWords.Select((word) => ConsumeWord(Id, word)).ToArray()
    );
}

public record ClassAttribute(String Attribute) : IDeclaration<ClassAttribute> {
    public record Collection(ClassAttribute[] Attributes) : IDeclaration<ClassAttribute.Collection> {
        public override string ToString() => String.Join(' ', Attributes.Select((attr) => attr.ToString()));
        public static Parser<ClassAttribute.Collection> AsParser => RunMany(
            converter: (attrs) => new ClassAttribute.Collection(attrs),
            0, Int32.MaxValue,
            ClassAttribute.AsParser
        );
    }

    public override string ToString() => Attribute;
    private static String[] AttributeWords = {"public", "private", "value", "enum", "interface", "sealed", "abstract", "auto", "sequential", "explicit", "ansi", "unicode", "autochar", "import", "serializable", "nested", "beforefieldinit", "specialname", "rtspecialname"};
    private static String[] NestedWords = {"public", "private", "family", "assembly", "famandassem", "famorassem"};
    public static Parser<ClassAttribute> AsParser => TryRun(
        converter: (attr) => new ClassAttribute(attr),
        AttributeWords.Select((word) => {
            if(word != "nested")
                return ConsumeWord(Id, word);
            else {
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