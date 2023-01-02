using System.Text;
using static Core;
using static Extensions;

public record FileReference(String Attribute, FileName File, ARRAY<BYTE>? Hash, bool IsEntryPoint) : IDeclaration<FileReference> {
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($".file ");
        sb.Append(Attribute is null ? String.Empty : $"{Attribute} ");
        sb.Append($"{File} ");
        if(Hash is not null) {
            sb.Append($".hash = ({Hash.ToString()})");
        }
        if(IsEntryPoint) {
            sb.Append(" .entrypoint");
        }

        return sb.ToString();
    }

    public static Parser<FileReference> AsParser => RunAll(
        converter: parts => new FileReference(
            parts[1]?.Attribute,
            parts[2].File,
            parts[3]?.Hash,
            parts[4]?.IsEntryPoint ?? false
        ),
        Discard<FileReference, string>(ConsumeWord(Id, ".file")),
        TryRun(
            converter: (attr) => Construct<FileReference>(4, 0, attr),
            ConsumeWord(Id, "nometadata"),
            Empty<String>()
        ),
        Map(
            converter: (name) => {
                var cleanedName = name.Name.EndsWith(".hash") ? name.Name.Substring(0, name.Name.Length - 5) : name.Name;
                return Construct<FileReference>(4, 1, new FileName(cleanedName));
            },
            FileName.AsParser
        ),
        TryRun(
            converter: (hash) => Construct<FileReference>(4, 2, hash),
            RunAll(
                converter: (vals) => vals[2],
                Discard<ARRAY<BYTE>, char>(ConsumeChar(Id, '=')),
                Discard<ARRAY<BYTE>, char>(ConsumeChar(Id, '(')),
                ARRAY<BYTE>.MakeParser('\0', '\0', '\0'),
                Discard<ARRAY<BYTE>, char>(ConsumeChar(Id, ')'))
            ),
            Empty<ARRAY<BYTE>>()
        ),
        TryRun(
            converter: (string res) => Construct<FileReference>(4, 3, res is not null),
            ConsumeWord(Id, ".entrypoint"),
            Empty<string>()
        )
    );
}

public record TypeReference(ResolutionScope Scope, ARRAY<DottedName> Names) : IDeclaration<TypeReference> {
    public override string ToString() {
        StringBuilder sb = new();
        if(Scope is not null) {
            sb.Append($"{Scope.ToString(true)} ");
        }
        sb.Append(Names.ToString());
        return sb.ToString();
    }
    public static Parser<TypeReference> AsParser => RunAll(
        converter: (vals) => new TypeReference(vals[0]?.Scope, vals[1].Names),
        TryRun(
            converter: (scope) =>  new TypeReference(scope, null),
            ResolutionScope.AsParser,
            Empty<ResolutionScope>()
        ),
        Map(
            converter: (name) =>  new TypeReference(null, name),
            ARRAY<DottedName>.MakeParser('\0', '/', '\0')
        )
    );
}

public record ResolutionScope : IDeclaration<ResolutionScope> {
    public record Module(FileName File) : ResolutionScope {
        public override string ToString() => $".module {File}";
        public static Parser<Module> AsParser => RunAll(
            converter: (vals) => new Module(vals[1]),
            Discard<FileName, string>(ConsumeWord(Id, ".module")),
            FileName.AsParser
        );
    }

    public record AssemblyRef(AssemblyRefName Name) : ResolutionScope {
        public override string ToString() => $"{Name}";
        public static Parser<AssemblyRef> AsParser => Map(
            converter: (name) => new AssemblyRef(name),
            AssemblyRefName.AsParser
        );
    }

    public string ToString(bool wrap = true) {
        StringBuilder sb = new();
        if(wrap) sb.Append('[');
        switch(this) {
            case Module m:
                sb.Append(m);
                break;
            case AssemblyRef a:
                sb.Append(a);
                break;
        }
        if(wrap) sb.Append(']');
        return sb.ToString();
    }

    public static Parser<ResolutionScope> AsParser => RunAll(
        converter: (vals) => vals[1],
        Discard<ResolutionScope, char>(ConsumeChar(Id, '[')),
        TryRun(
            converter: (vals) => vals,
            Cast<ResolutionScope, Module>(Module.AsParser),
            Cast<ResolutionScope, AssemblyRef>(AssemblyRef.AsParser)
        ),
        Discard<ResolutionScope, char>(ConsumeChar(Id, ']'))
    );
}

public record AssemblyRefName(DottedName Name) : IDeclaration<AssemblyRefName> {
    public override string ToString() => Name.ToString();
    public static Parser<AssemblyRefName> AsParser => Map(
        converter: (name) => new AssemblyRefName(name),
        DottedName.AsParser
    );
}

public record FileName(String Name) : IDeclaration<FileName> {
    public override string ToString() => Name;
    public static Parser<FileName> AsParser => Map((name) => new FileName(name.ToString()), DottedName.AsParser);
}

public record ExternSource(INT Line, INT? Column, QSTRING? File) : IDeclaration<ExternSource> {
    public override string ToString() {
        StringBuilder sb = new();
        sb.Append($".line {Line}");
        if(Column is not null) {
            sb.Append($" : {Column.Value}");
        }
        if(File is not null) {
            sb.Append($" '{File.Value}'");
        }
        return sb.ToString();
    }
    public static Parser<ExternSource> AsParser => RunAll(
        converter: (vals) => new ExternSource(vals[1].Line, vals[2].Column, vals[3].File),
        ConsumeWord((_) => new ExternSource(null, null, null), ".line"),
        Map((line) => new ExternSource(line, null, null), INT.AsParser),
        TryRun(
            converter: (vals) => new ExternSource(null, vals, null),
            RunAll(
                converter: (vals) => vals[1],
                ConsumeChar((_) => default(INT), ':'),
                Map((column) => column, INT.AsParser)
            ),
            Empty<INT>()
        ),
        TryRun((name) => new ExternSource(null, null, name), QSTRING.AsParser, Empty<QSTRING>())
    );
}