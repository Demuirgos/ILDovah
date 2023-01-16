#define MSFTSPEC
using IdentifierDecl;
using RootDecl;
using System.Text;

using static Core;
using static ExtraTools.Extensions;

namespace ResourceDecl;


[GenerateParser] public partial record FileReference : Declaration, IDeclaration<FileReference>;
[GenerationOrderParser(Order.Last)]
public record FileReferenceCIL(FileReferenceCIL.Prefix Header, FileReferenceCIL.Body Member) : FileReference, IDeclaration<FileReferenceCIL>
{
    public record Prefix(String Attribute, FileName File) : IDeclaration<Prefix>
    {
        public override string ToString() => $"{Attribute} {File}";

        public static Parser<Prefix> AsParser => RunAll(
            converter: parts => new Prefix(
                parts[0]?.Attribute,
                parts[1].File
            ),
            TryRun(
                converter: (attr) => Construct<Prefix>(2, 0, attr),
                ConsumeWord(Id, "nometadata"),
                Empty<String>()
            ),
            Map(
                converter: (name) =>  Construct<Prefix>(2, 1, name),
                FileName.AsParser
            )
        );
    }

    public record Body(ARRAY<BYTE>? Hash, bool IsEntryPoint) : IDeclaration<Prefix>
    {
        public override string ToString()
        {
            StringBuilder sb = new();
            if (Hash is not null)
            {
                sb.Append($".hash = ({Hash.ToString(String.Empty)})");
            }
            if (IsEntryPoint)
            {
                sb.Append(" .entrypoint");
            }
            return sb.ToString();
        }


        public static Parser<Body> AsParser => RunAll(
            converter: parts => new Body(
                parts[0]?.Hash,
                parts[1].IsEntryPoint
            ),
            TryRun(
                converter: (hash) => Construct<Body>(2, 0, hash),
                RunAll(
                    converter: (vals) => vals[3],
                    Discard<ARRAY<BYTE>, string>(ConsumeWord(Id, ".hash")),
                    Discard<ARRAY<BYTE>, char>(ConsumeChar(Id, '=')),
                    Discard<ARRAY<BYTE>, char>(ConsumeChar(Id, '(')),
                    ARRAY<BYTE>.MakeParser(new ARRAY<BYTE>.ArrayOptions {
                        Delimiters = ('\0', '\0', '\0')
                    }),
                    Discard<ARRAY<BYTE>, char>(ConsumeChar(Id, ')'))
                ),
                Empty<ARRAY<BYTE>>()
            ),
            TryRun(
                converter: (string res) => Construct<Body>(2, 1, res is not null),
                ConsumeWord(Id, ".entrypoint"),
                Empty<string>()
            )
        );
    }

    public override string ToString() => $".file {Header} {Member}";

    public static Parser<FileReferenceCIL> AsParser => RunAll(
        converter: parts => new FileReferenceCIL(
            parts[1].Header,
            parts[2]?.Member
        ),
        Discard<FileReferenceCIL, string>(ConsumeWord(Id, ".file")),
        Map(
            converter: (header) => Construct<FileReferenceCIL>(2, 0, header),
            Prefix.AsParser
        ),
        TryRun(
            converter: (body) => Construct<FileReferenceCIL>(2, 1, body),
            Body.AsParser,
            Empty<Body>()
        )
    );
}


[WrapParser<ModuleDecl.Module>] public partial record ModuleScope : ResolutionScope;

[WrapParser<AssemblyRefName>] public partial record AssemblyRef : ResolutionScope;
public record ResolutionScope : Declaration, IDeclaration<ResolutionScope>
{
    

    public string ToString(bool wrap = true)
    {
        StringBuilder sb = new();
        if (wrap) sb.Append('[');
        switch (this)
        {
            case ModuleScope m:
                sb.Append(m);
                break;
            case AssemblyRef a:
                sb.Append(a);
                break;
        }
        if (wrap) sb.Append(']');
        return sb.ToString();
    }

    public static Parser<ResolutionScope> AsParser => RunAll(
        converter: (vals) => vals[1],
        Discard<ResolutionScope, char>(ConsumeChar(Id, '[')),
        TryRun(
            converter: (vals) => vals,
            Cast<ResolutionScope, ModuleScope>(ModuleScope.AsParser),
            Cast<ResolutionScope, AssemblyRef>(AssemblyRef.AsParser)
        ),
        Discard<ResolutionScope, char>(ConsumeChar(Id, ']'))
    );
}

public record AssemblyRefName(DottedName Name) : IDeclaration<AssemblyRefName>
{
    public override string ToString() => Name.ToString();
    public static Parser<AssemblyRefName> AsParser => Map(
        converter: (name) => new AssemblyRefName(name),
        DottedName.AsParser
    );
}

public record FileName(String Name) : IDeclaration<FileName>
{
    public override string ToString() => Name;
    public static Parser<FileName> AsParser => Map((name) => new FileName(name.ToString()), DottedName.AsParser);
}

public record ExternSource(INT Line, INT? Column, QSTRING? File) : Declaration, IDeclaration<ExternSource>
{
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($".line {Line}");
        if (Column is not null)
        {
            sb.Append($" : {Column.Value}");
        }
        if (File is not null)
        {
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
public record Culture(QSTRING Value) : Declaration, IDeclaration<Culture>
{
    public override string ToString() => $".culture {Value}";

    public static Parser<Culture> AsParser => RunAll(
        converter: parts => new Culture(
            parts[1].Value
        ),
        Discard<Culture, string>(ConsumeWord(Core.Id, ".culture")),
        Map(
            converter: value => Construct<Culture>(1, 0, value),
            QSTRING.AsParser
        )
    );
}

public record StackReserve(INT Value) : Declaration, IDeclaration<StackReserve>
{
    public override string ToString() => $".stackreserve {Value}";

    public static Parser<StackReserve> AsParser => RunAll(
        converter: parts => new StackReserve(
            parts[1].Value
        ),
        Discard<StackReserve, string>(ConsumeWord(Core.Id, ".stackreserve")),
        Map(
            converter: value => Construct<StackReserve>(1, 0, value),
            INT.AsParser
        )
    );
}

public record Corflags(INT Value) : Declaration, IDeclaration<Corflags>
{
    public override string ToString() => $".corflags {Value}";

    public static Parser<Corflags> AsParser => RunAll(
        converter: parts => new Corflags(
            parts[1].Value
        ),
        Discard<Corflags, string>(ConsumeWord(Core.Id, ".corflags")),
        Map(
            converter: value => Construct<Corflags>(1, 0, value),
            INT.AsParser
        )
    );
}

#if MSFTSPEC
public record ImageBase(INT Value) : Declaration, IDeclaration<ImageBase>
{
    public override string ToString() => $".imagebase {Value}";

    public static Parser<ImageBase> AsParser => RunAll(
        converter: parts => new ImageBase(
            parts[1].Value
        ),
        Discard<ImageBase, string>(ConsumeWord(Core.Id, ".imagebase")),
        Map(
            converter: value => Construct<ImageBase>(1, 0, value),
            INT.AsParser
        )
    );
}

public record FileAlignment(INT Value) : FileReference, IDeclaration<FileAlignment>
{
    public override string ToString() => $".file alignment {Value}";

    public static Parser<FileAlignment> AsParser => RunAll(
        converter: parts => parts[2],
        Discard<FileAlignment, string>(ConsumeWord(Core.Id, ".file")),
        Discard<FileAlignment, string>(ConsumeWord(Core.Id, "alignment")),
        Map(
            converter: value => Construct<FileAlignment>(1, 0, value),
            INT.AsParser
        )
    );
}
#endif