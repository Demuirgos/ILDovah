using AttributeDecl;
using CallConventionDecl;
using DataDecl;
using ExceptionDecl;
using FieldDecl;
using IdentifierDecl;
using InstructionDecl;
using LabelDecl;
using LocalDecl;
using ParameterDecl;
using ResourceDecl;
using RootDecl;
using SecurityDecl;
using SigArgumentDecl;
using System.Text;
using TypeDecl;
using static Core;
using static ExtraTools.Extensions;
namespace MethodDecl;

public record Method(Prefix Header, Member.Collection Body) : Declaration, IDeclaration<Method>
{
    public bool IsConstructor => Header.Name.IsConstructor;
    public bool IsEntrypoint => Body?.Items.Values.Any(item => item is EntrypointClause) ?? false;

    public override string ToString() => $".method {Header} \n{{\n{Body}\n}}";
    public static Parser<Method> AsParser => RunAll(
        converter: parts => new Method(parts[1].Header, parts[3]?.Body),
        Discard<Method, string>(ConsumeWord(Id, ".method")),
        Map(
            converter: header => Construct<Method>(2, 0, header),
            Prefix.AsParser
        ),
        Discard<Method, char>(ConsumeChar(Id, '{')),
        Map( 
            converter: blocks => Construct<Method>(2, 1, blocks),
            Member.Collection.AsParser
        ),
        Discard<Method, char>(ConsumeChar(Id, '}'))
    );
}
public record MethodName(String Name) : IDeclaration<MethodName>
{
    public bool IsConstructor => Name == ".ctor";
    public override string ToString() => Name;
    public static Parser<MethodName> AsParser => TryRun(
        converter: (vals) => new MethodName(vals),
        ConsumeWord(Id, ".ctor"),
        ConsumeWord(Id, ".cctor"),
        Map((dname) => dname.ToString(), DottedName.AsParser)
    );
}

public record Prefix(MethodAttribute.Collection MethodAttributes, CallConvention? Convention, TypeDecl.Type Type, NativeType? MarshalledType, MethodName Name, GenericParameter.Collection? TypeParameters, Parameter.Collection Parameters, ImplAttribute.Collection ImplementationAttributes) : IDeclaration<Prefix>
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(MethodAttributes);
        if (Convention != null)
        {
            sb.Append($" {Convention}");
        }
        sb.Append($"\n\t{Type}");
        if (MarshalledType != null)
        {
            sb.Append($" marshal ({MarshalledType})");
        }
        sb.Append($" {Name}");
        if (TypeParameters != null)
        {
            sb.Append($" <{TypeParameters}>");
        }
        sb.Append($" ({Parameters})");
        if (ImplementationAttributes != null)
        {
            sb.Append($" {ImplementationAttributes} ");
        }
        return sb.ToString();
    }
    public static Parser<Prefix> AsParser => RunAll(
        converter: parts => new Prefix(
            parts[0].MethodAttributes,
            parts[1].Convention,
            parts[2].Type,
            parts[3].MarshalledType,
            parts[4].Name,
            parts[5].TypeParameters,
            parts[6].Parameters,
            parts[7].ImplementationAttributes
        ),
        Map(
            converter: attrs => Construct<Prefix>(8, 0, attrs),
            MethodAttribute.Collection.AsParser
        ),
        TryRun(
            converter: conv => Construct<Prefix>(8, 1, conv),
            CallConvention.AsParser
        ),
        Map(
            converter: type => Construct<Prefix>(8, 2, type),
            TypeDecl.Type.AsParser
        ),
        TryRun(
            converter: type => Construct<Prefix>(8, 3, type),
            RunAll(
                converter: parts => parts[2],
                Discard<NativeType, string>(ConsumeWord(Id, "marshal")),
                Discard<NativeType, char>(ConsumeChar(Id, '(')),
                NativeType.AsParser,
                Discard<NativeType, char>(ConsumeChar(Id, ')'))
            ),
            Empty<NativeType>()
        ),
        Map(
            converter: name => Construct<Prefix>(8, 4, name),
            MethodName.AsParser
        ),
        TryRun(
            converter: genpars => Construct<Prefix>(8, 5, genpars),
            RunAll(
                converter: pars => pars[1],
                Discard<GenericParameter.Collection, char>(ConsumeChar(Id, '<')),
                GenericParameter.Collection.AsParser,
                Discard<GenericParameter.Collection, char>(ConsumeChar(Id, '>'))
            ),
            Empty<GenericParameter.Collection>()
        ),
        RunAll(
            converter: pars => Construct<Prefix>(8, 6, pars[1]),
            Discard<Parameter.Collection, char>(ConsumeChar(Id, '(')),
            Parameter.Collection.AsParser,
            Discard<Parameter.Collection, char>(ConsumeChar(Id, ')'))
        ),
        Map(
            converter: implattrs => Construct<Prefix>(8, 7, implattrs),
            ImplAttribute.Collection.AsParser
        )
    );
}

[GenerateParser]
public partial record Member : IDeclaration<Member>
{
    public record Collection(ARRAY<Member> Items) : IDeclaration<Collection>
    {
        public override string ToString() => Items.ToString('\n');
        public static Parser<Collection> AsParser => Map(
            converter: items => new Collection(items),
            ARRAY<Member>.MakeParser('\0', '\0', '\0')
        );
    }
}
[WrapParser<CodeLabel>] public partial record LabelItem : Member, IDeclaration<LabelItem>;
[WrapParser<Instruction>] public partial record InstructionItem : Member, IDeclaration<InstructionItem>;
[WrapParser<Data>] public partial record DataBodyItem : Member, IDeclaration<DataBodyItem>;
[WrapParser<SecurityBlock>] public partial record SecurityDeclarationItem : Member, IDeclaration<SecurityDeclarationItem>;
[WrapParser<ExternSource>] public partial record ExternSourceItem : Member, IDeclaration<ExternSourceItem>;
[WrapParser<StructuralExceptionBlock>] public partial record ExceptionHandlingItem : Member, IDeclaration<ExceptionHandlingItem>;
[WrapParser<CustomAttribute>] public partial record CustomAttributeItem: Member, IDeclaration<CustomAttributeItem>;

public record EmitByteItem(INT Value) : Member, IDeclaration<EmitByteItem>
{
    public override string ToString() => $".emitbyte {Value}";
    public static Parser<EmitByteItem> AsParser => RunAll(
        converter: parts => new EmitByteItem(parts[1]),
        Discard<INT, string>(ConsumeWord(Id, ".emitbyte")),
        INT.AsParser
    );
}

public record MaxStackItem(INT Value) : Member, IDeclaration<MaxStackItem>
{
    public override string ToString() => $".maxstack {Value}";
    public static Parser<MaxStackItem> AsParser => RunAll(
        converter: parts => new MaxStackItem(parts[1]),
        Discard<INT, string>(ConsumeWord(Id, ".maxstack")),
        INT.AsParser
    );
}

[GenerateParser]
public partial record ParamAttribute: Member, IDeclaration<ParamAttribute>;
[WrapParser<GenericParameterSelector>] public partial record ParamAttributeClause : ParamAttribute, IDeclaration<ParamAttributeClause>;

public record InitializeParamAttribute(INT Index, FieldInit Value) : ParamAttribute
{
    public override string ToString() => $".param [{Index}] {(Value is null ? String.Empty : $"= {Value}")}";
    public static Parser<InitializeParamAttribute> AsParser => RunAll(
        converter: parts => new InitializeParamAttribute(
            parts[0].Index,
            parts[1]?.Value
        ),
        RunAll(
            converter: parts => new InitializeParamAttribute(parts[2], null),
            Discard<INT, string>(ConsumeWord(Id, ".param")),
            Discard<INT, char>(ConsumeChar(Id, '[')),
            INT.AsParser,
            Discard<INT, char>(ConsumeChar(Id, ']'))
        ),
        TryRun(
            converter: finit => new InitializeParamAttribute(null, finit),
            RunAll(
                converter: parts => parts[1],
                Discard<FieldInit, char>(ConsumeChar(Id, '=')),
                FieldInit.AsParser
            )
        )
    );
}


public record LocalsItem(bool IsInit, Local.Collection Signatures) : Member, IDeclaration<LocalsItem>
{
    public override string ToString() => $".locals {(IsInit ? "init" : String.Empty)} ({Signatures})";
    public static Parser<LocalsItem> AsParser => RunAll(
        converter: parts => new LocalsItem(parts[1].IsInit, parts[2].Signatures),
        Discard<LocalsItem, string>(ConsumeWord(Id, ".locals")),
        TryRun(
            converter: result => new LocalsItem(result is null, null),
            Discard<LocalsItem, string>(ConsumeWord(Id, "init")),
            Empty<LocalsItem>()
        ),
        RunAll(
            converter: sigs => new LocalsItem(false, sigs[1]),
            Discard<Local.Collection, char>(ConsumeChar(Id, '(')),
            Local.Collection.AsParser,
            Discard<Local.Collection, char>(ConsumeChar(Id, ')'))
        )
    );
}

[GenerateParser]
public partial record OverrideMethodSignature : IDeclaration<OverrideMethodSignature>;
public record OverrideMethodDefault(TypeSpecification Specification, MethodName Name) : OverrideMethodSignature, IDeclaration<OverrideMethodDefault>
{
    public override string ToString() => $"{Specification}::{Name}";
    public static Parser<OverrideMethodDefault> AsParser => RunAll(
        converter: parts => new OverrideMethodDefault(parts[0].Specification, parts[2].Name),
        Map(
            converter: spec => Construct<OverrideMethodDefault>(2, 0, spec),
            TypeSpecification.AsParser
        ),
        Discard<OverrideMethodDefault, string>(ConsumeWord(Id, "::")),
        Map(
            converter: name => Construct<OverrideMethodDefault>(2, 1, name),
            MethodName.AsParser
        )
    );
}

public record OverrideMethodGeneric(CallConvention Convention, TypeDecl.Type Type, TypeSpecification Specification, MethodName Name, GenericTypeArity Arity, Parameter.Collection Parameters) : OverrideMethodSignature, IDeclaration<OverrideMethodGeneric>
{
    public override string ToString() => $"method {Convention} {Type} {Specification}::{Name} {Arity} ({Parameters})";
    public static Parser<OverrideMethodGeneric> AsParser => RunAll(
        converter: parts => new OverrideMethodGeneric(
            parts[1].Convention,
            parts[2].Type,
            parts[3].Specification,
            parts[5].Name,
            parts[6].Arity,
            parts[8].Parameters
        ),
        TryRun(
            Id,
            Discard<OverrideMethodGeneric, string>(ConsumeWord(Id, "method")),
            Empty<OverrideMethodGeneric>()
        ),
        Map(
            converter: conv => Construct<OverrideMethodGeneric>(6, 0, conv),
            CallConvention.AsParser
        ),
        Map(
            converter: type => Construct<OverrideMethodGeneric>(6, 1, type),
            TypeDecl.Type.AsParser
        ),
        Map(
            converter: spec => Construct<OverrideMethodGeneric>(6, 2, spec),
            TypeSpecification.AsParser
        ),
        Discard<OverrideMethodGeneric, string>(ConsumeWord(Id, "::")),
        Map(
            converter: name => Construct<OverrideMethodGeneric>(6, 3, name),
            MethodName.AsParser
        ),
        Map(
            converter: arity => Construct<OverrideMethodGeneric>(6, 4, arity),
            GenericTypeArity.AsParser
        ),
        Discard<OverrideMethodGeneric, char>(ConsumeChar(Id, '(')),
        Map(
            converter: parameters => Construct<OverrideMethodGeneric>(6, 5, parameters),
            Parameter.Collection.AsParser
        ),
        Discard<OverrideMethodGeneric, char>(ConsumeChar(Id, ')'))
    );
}

public record OverrideMethodItem(OverrideMethodSignature Target) : Member, IDeclaration<OverrideMethodItem>
{
    public override string ToString() => $".override {Target}";

    public static Parser<OverrideMethodItem> AsParser => RunAll(
        converter: target => new OverrideMethodItem(target[1]),
        Discard<OverrideMethodSignature, string>(ConsumeWord(Id, ".override")),
        OverrideMethodSignature.AsParser
    );

};

public record ScopeBlock(Member.Collection Blocks) : Member, IDeclaration<ScopeBlock>
{
    public override string ToString() => $"{{\n{Blocks}\n}}";
    public static Parser<ScopeBlock> AsParser => RunAll(
        converter: parts => new ScopeBlock(parts[1]),
        Discard<Member.Collection, char>(ConsumeChar(Id, '{')),
        Member.Collection.AsParser,
        Discard<Member.Collection, char>(ConsumeChar(Id, '}'))
    );
}

public record EntrypointClause() : Member, IDeclaration<EntrypointClause>
{
    public override string ToString() => ".entrypoint";
    public static Parser<EntrypointClause> AsParser => Map(
        converter: item => new EntrypointClause(),
        ConsumeWord(Id, ".entrypoint")
    );
}