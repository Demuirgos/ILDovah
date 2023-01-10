using BoundsDecl;
using CallConventionDecl;
using IdentifierDecl;
using MethodDecl;
using ParameterDecl;
using ResourceDecl;
using SigArgumentDecl;
using System.Text;
using static Core;
using static ExtraTools.Extensions;
namespace TypeDecl;
using GenArgs = Type.Collection;

public record TypeReference(ResolutionScope Scope, ARRAY<DottedName> Names) : IDeclaration<TypeReference>
{
    public override string ToString()
    {
        StringBuilder sb = new();
        if (Scope is not null)
        {
            sb.Append($"{Scope.ToString(true)} ");
        }
        sb.Append(Names.ToString());
        return sb.ToString();
    }
    public static Parser<TypeReference> AsParser => RunAll(
        converter: (vals) => new TypeReference(vals[0]?.Scope, vals[1].Names),
        TryRun(
            converter: (scope) => new TypeReference(scope, null),
            ResolutionScope.AsParser,
            Empty<ResolutionScope>()
        ),
        Map(
            converter: (name) => new TypeReference(null, name),
            ARRAY<DottedName>.MakeParser('\0', '/', '\0')
        )
    );
}

[GenerateParser] public partial record TypeSpecification : IDeclaration<TypeSpecification>;
[GenerationOrderParser(Order.Last)] public record NamedModuleSpecification(DottedName Name, bool IsModule) : TypeSpecification, IDeclaration<NamedModuleSpecification>
{
    public override string ToString() => $"{(IsModule ? ".module " : String.Empty)}{Name}";
    public static Parser<NamedModuleSpecification> MainParser => RunAll(
        converter: (vals) => new NamedModuleSpecification(vals[1].Name, vals[0].IsModule),
        TryRun(
            converter: (module) => new NamedModuleSpecification(null, module is not null),
            Discard<NamedModuleSpecification, string>(ConsumeWord(Id, ".module")),
            Empty<NamedModuleSpecification>()
        ),
        Map(
            converter: (name) => new NamedModuleSpecification(name, false),
            DottedName.AsParser
        )
    );
    public static Parser<NamedModuleSpecification> AsParser => RunAll(
        converter: (vals) => vals[1],
        Discard<NamedModuleSpecification, char>(ConsumeChar(Id, '[')),
        NamedModuleSpecification.MainParser,
        Discard<NamedModuleSpecification, char>(ConsumeChar(Id, ']'))
    );
}

[WrapParser<Type>] public partial record TypeSpecificationInlined : TypeSpecification, IDeclaration<TypeSpecificationInlined>;
[WrapParser<TypeReference>] public partial record TypeSpecificationReference : TypeSpecification, IDeclaration<TypeSpecificationInlined>;

public record NativeType(NativeType Type, bool IsArray, INT Length, INT Supplied) : IDeclaration<NativeType>
{
    public override string ToString()
    {
        StringBuilder sb = new();
        if (Type is not null)
        {
            sb.Append(Type.ToString());
        }

        if (IsArray)
        {
            sb.Append("[");
            if (Length is not null)
            {
                sb.Append($"{Length}");
            }
            if (Supplied is not null)
            {
                sb.Append($"+{Supplied}");
            }
            sb.Append("]");
        }
        return sb.ToString();
    }
    public record NativeTypePrimitive(String TypeName) : NativeType(null, false, null, null), IDeclaration<NativeTypePrimitive>
    {
        private static String[] _primitives = new String[] { "[]", "bool", "float32", "float64", "int", "int8", "int16", "int32", "int64", "lpstr", "lpwstr", "method", "unsigned" };
        public override string ToString() => TypeName;
        public static Parser<NativeTypePrimitive> AsParser => TryRun(
            converter: (vals) => new NativeTypePrimitive(vals),
            _primitives.Select((primitive) =>
            {
                if (primitive == "unsigned")
                {
                    return RunAll(
                        converter: (vals) => $"{vals[0]} {vals[1]}",
                        ConsumeWord(Id, primitive),
                        TryRun(
                            converter: (vals) => vals,
                            _primitives.Take(4..9).Select((primitive2) => ConsumeWord(Id, primitive2)).ToArray()
                        )
                    );
                }
                else
                {
                    return ConsumeWord(Id, primitive);
                }
            }).ToArray()
        );
    }

    public static Parser<NativeType> AsParser => RunAll(
        converter: (vals) => vals[1].Aggregate(vals[0][0], (acc, val) => new NativeType(acc, val.IsArray, val.Length, val.Supplied)),
        Map(primType => new[] { primType as NativeType }, NativeTypePrimitive.AsParser),
        RunMany(
            converter: Id,
            0, Int32.MaxValue, true,
            RunAll(
                converter: (vals) => new NativeType(null, true, vals[1], vals[2]),
                Discard<INT, char>(ConsumeChar(Id, '[')),
                TryRun(Id, Map(Id, INT.AsParser), Empty<INT>()),
                TryRun(Id,
                    RunAll(
                        converter: (vals) => vals[1],
                        Discard<INT, char>(ConsumeChar(Id, '+')),
                        Map(Id, INT.AsParser)
                    ),
                    Empty<INT>()
                ),
                Discard<INT, char>(ConsumeChar(Id, ']'))
            )
        )
    );
}

public record Type(Prefix Basic, Suffix[] Suffixes) : IDeclaration<Type>
{
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"{Basic}");
        if (Suffixes is not null)
        {
            sb.Append(" ");
            sb.Append(String.Join(" ", Suffixes.Select((suffix) => suffix.ToString())));
        }
        return sb.ToString();
    }
    public record Collection(ARRAY<Type> Types) : IDeclaration<Collection>
    {
        public override string ToString() => Types.ToString(',');
        public static Parser<Collection> AsParser => Map(
            converter: (types) => new Collection(types),
            ARRAY<Type>.MakeParser('\0', ',', '\0')
        );
    }

    public static Parser<Type> AsParser => RunAll(
        converter: parts => new Type(
            Basic: parts[0].Basic,
            Suffixes: parts[1].Suffixes
        ),
        Map(
            converter: (type) => Construct<TypeDecl.Type>(2, 0, type),
            Prefix.AsParser
        ),
        TryRun(
            converter: (suffixes) => Construct<TypeDecl.Type>(2, 1, suffixes),
            RunMany(
                converter: Id,
                0, Int32.MaxValue, true,
                Suffix.AsParser
            )
        )
    );
}

[GenerateParser] public partial record Suffix : IDeclaration<Suffix>;
public record ReferenceSuffix(bool IsRawPointer) : Suffix, IDeclaration<ReferenceSuffix>
{
    public override string ToString() => IsRawPointer ? "*" : "&";
    public static Parser<ReferenceSuffix> AsParser => TryRun(
        converter: (vals) => new ReferenceSuffix(vals == '*'),
        ConsumeChar(Id, '*'), ConsumeChar(Id, '&')
    );
}

public record BoundedSuffix(Bound.Collection Bounds) : Suffix, IDeclaration<BoundedSuffix>
{
    public override string ToString() => $"[{Bounds}]";
    public static Parser<BoundedSuffix> AsParser => RunAll(
        converter: (vals) => new BoundedSuffix(vals[1]),
        Discard<Bound.Collection, char>(ConsumeChar(Id, '[')),
        Bound.Collection.AsParser,
        Discard<Bound.Collection, char>(ConsumeChar(Id, ']'))
    );
}

public record GenericSuffix(GenArgs GenericArguments) : Suffix, IDeclaration<GenericSuffix>
{
    public override string ToString() => $"<{GenericArguments}>";
    public static Parser<GenericSuffix> AsParser => RunAll(
        converter: (vals) => new GenericSuffix(vals[1]),
        Discard<GenArgs, char>(ConsumeChar(Id, '<')),
        Lazy(() => GenArgs.AsParser),
        Discard<GenArgs, char>(ConsumeChar(Id, '>'))
    );
}

public record ModifierSuffix(String Modifier, TypeReference ReferencedType) : Suffix, IDeclaration<ModifierSuffix>
{
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"{Modifier}");
        if (ReferencedType is not null)
        {
            sb.Append($" ({ReferencedType})");
        }
        return sb.ToString();
    }
    public static Parser<ModifierSuffix> AsParser => RunAll(
        converter: (vals) => new ModifierSuffix(vals[0].Modifier, vals[1].ReferencedType),
        Map(val => new ModifierSuffix(val, null), TryRun(Id, ConsumeWord(Id, "modopt"), ConsumeWord(Id, "modreq"))),
        Map(val => new ModifierSuffix(null, val), TypeReference.AsParser)
    );
}

[GenerateParser] public partial record Prefix : IDeclaration<Prefix>;
public record TypePrimitive(String TypeName) : Prefix, IDeclaration<TypePrimitive>
{
    private static String[] _primitives = new String[] { "bool", "char", "float32", "float64", "int8", "int16", "int32", "int64", "object", "string", "typedref", "valuetype", "void", "unsigned", "native" };

    public override string ToString() => TypeName;
    public static Parser<TypePrimitive> AsParser => TryRun(
        converter: (vals) => new TypePrimitive(vals),
        _primitives.Select((primitive) =>
        {
            if (primitive == "unsigned")
            {
                return RunAll(
                    converter: (vals) => $"{vals[0]} {vals[1]}",
                    ConsumeWord(Id, primitive),
                    TryRun(
                        converter: (vals) => vals,
                        _primitives.Take(5..8).Select((primitive2) => ConsumeWord(Id, primitive2)).ToArray()
                    )
                );
            }
            else if (primitive == "native")
            {
                return RunAll(
                    converter: (vals) =>
                    {
                        StringBuilder sb = new();
                        sb.Append(vals[0]);
                        if (vals[1] is not null)
                        {
                            sb.Append($" {vals[1]}");
                        }
                        sb.Append($" {vals[2]} ");
                        return sb.ToString();
                    },
                    ConsumeWord(Id, primitive),
                    TryRun(Id, ConsumeWord(Id, "unsigned"), Empty<String>()),
                    ConsumeWord(Id, "int")
                );
            }
            else
            {
                return ConsumeWord(Id, primitive);
            }
        }).ToArray()
    );
}

public record GenericTypeParameter(INT Index, GenericTypeParameter.Type TypeParameterType) : Prefix, IDeclaration<GenericTypeParameter>
{
    public enum Type { Method, Class }
    public override string ToString() => $"{(TypeParameterType is Type.Method ? "!!" : "!")}{Index}";
    public static Parser<GenericTypeParameter> AsParser => RunAll(
        converter: (vals) => new GenericTypeParameter(vals[1].Index, vals[0].TypeParameterType),
        TryRun(
            converter: (indicator) => new GenericTypeParameter(null, indicator == "!!" ? Type.Method : Type.Class),
            ConsumeWord(Id, "!!"),
            ConsumeWord(Id, "!")
        ),
        Map(val => new GenericTypeParameter(val, Type.Class), INT.AsParser)
    );
}

public record MethodDefinition(CallConvention CallConvention, Type TypeTarget, Parameter.Collection Parameters) : Prefix, IDeclaration<MethodDefinition>
{
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"method {CallConvention} {TypeTarget}* \n(");
        sb.Append(Parameters);
        sb.Append("\n)");
        return sb.ToString();
    }
    public static Parser<MethodDefinition> AsParser => RunAll(
        converter: parts => new MethodDefinition(parts[1].CallConvention, parts[2].TypeTarget, parts[5].Parameters),
        Discard<MethodDefinition, String>(ConsumeWord(Id, "method")),
        Map(
            converter: part1 => new MethodDefinition(part1, null, null),
            CallConvention.AsParser
        ),
        Map(
            converter: part2 => new MethodDefinition(null, part2, null),
            Lazy(() => Type.AsParser)
        ),
        Discard<MethodDefinition, char>(ConsumeChar(Id, '*')),
        Discard<MethodDefinition, char>(ConsumeChar(Id, '(')),
        Map(
            converter: part3 => new MethodDefinition(null, null, part3),
            Parameter.Collection.AsParser),
        Discard<MethodDefinition, char>(ConsumeChar(Id, ')'))
    );
}

public record ClassTypeReference(TypeReference Reference) : Prefix, IDeclaration<ClassTypeReference>
{
    public override string ToString() => $"class {Reference}";
    public static Parser<ClassTypeReference> AsParser => RunAll(
        converter: (vals) => new ClassTypeReference(vals[1].Reference),
        Discard<ClassTypeReference, string>(ConsumeWord(Id, "class")),
        Map(
            converter: (vals) => new ClassTypeReference(vals),
            Lazy(() => TypeReference.AsParser)
        )
    );
}


/*
memberRef : 
  'method' callConv type [typeSpec'::'] methodName '(' sigArgs0 ')'
| 'field' type [typeSpec '::'] id
*/
[GenerateParser] public partial record OwnerType : IDeclaration<OwnerType>;
[WrapParser<TypeSpecification>] public partial record TypeSpecReference : OwnerType, IDeclaration<OwnerType>;
[GenerateParser] public partial record MemberReference : OwnerType, IDeclaration<OwnerType>;

public record MethodMemberReference(MethodReference MethodRef) : MemberReference, IDeclaration<MethodMemberReference>
{
    public override string ToString() => $"method {MethodRef}";
    public static Parser<MethodMemberReference> AsParser => RunAll(
        converter: (vals) => new MethodMemberReference(vals[1].MethodRef),
        Discard<MethodMemberReference, string>(ConsumeWord(Id, "method")),
        Map(
            converter: (vals) => new MethodMemberReference(vals),
            Lazy(() => MethodReference.AsParser)
        )
    );
}

public record FieldMemberReference(FieldTypeReference FieldRef) : MemberReference, IDeclaration<FieldMemberReference>
{
    public override string ToString() => $"field {FieldRef}";
    public static Parser<FieldMemberReference> AsParser => RunAll(
        converter: (vals) => new FieldMemberReference(vals[1].FieldRef),
        Discard<FieldMemberReference, string>(ConsumeWord(Id, "field")),
        Map(
            converter: (vals) => new FieldMemberReference(vals),
            Lazy(() => FieldTypeReference.AsParser)
        )
    );
}

public record FieldTypeReference(TypeDecl.Type Type, TypeDecl.TypeSpecification Spec, Identifier Name) 
    : IDeclaration<FieldTypeReference> {
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"{Type} ");
        if (Spec != null)
        {
            sb.Append($"{Spec}::");
        }
        sb.Append($"{Name}");
        return sb.ToString();
    }
    public static Parser<FieldTypeReference> AsParser => RunAll(
        converter: parts => new FieldTypeReference(
            parts[0].Type,
            parts[1]?.Spec,
            parts[2].Name
        ),
        Map(
            converter: type => Construct<FieldTypeReference>(3, 0, type),
            TypeDecl.Type.AsParser
        ),
        TryRun(
            converter: spec => Construct<FieldTypeReference>(3, 1, spec),
            RunAll(
                converter : parts => parts[0],
                TypeDecl.TypeSpecification.AsParser,
                Discard<TypeDecl.TypeSpecification, string>(ConsumeWord(Core.Id, "::"))
            ),
            Empty<TypeDecl.TypeSpecification>()
        ),
        Map(
            converter: name => Construct<FieldTypeReference>(3, 2, name),
            Identifier.AsParser
        )
    );
}

public record MethodReference(CallConvention? Convention, TypeDecl.Type Type, TypeSpecification Spec, MethodName Name, GenArgs? TypeParameters, SigArgumentDecl.SigArgument.Collection SigArgs) 
    : IDeclaration<MethodReference> {
    public override string ToString()
    {
        var sb = new StringBuilder();
        if (Convention is not null)
        {
            sb.Append($"{Convention} ");
        }
        sb.Append($"{Type} ");
        if (Spec is not  null)
        {
            sb.Append($"{Spec}::");
        }
        sb.Append($"{Name} ");
        if (TypeParameters is not null)
        {
            sb.Append($"<{TypeParameters}> ");
        }
        sb.Append($"({SigArgs})");
        return sb.ToString();
    }
    public static Parser<MethodReference> AsParser => RunAll(
        converter: parts => new MethodReference(
            parts[0].Convention,
            parts[1].Type,
            parts[2]?.Spec,
            parts[3].Name,
            parts[4]?.TypeParameters,
            parts[5].SigArgs
        ),
        Map(
            converter: conv => Construct<MethodReference>(6, 0, conv),
            CallConvention.AsParser
        ),
        Map(
            converter: type => Construct<MethodReference>(6, 1, type),
            TypeDecl.Type.AsParser
        ),
        TryRun(
            converter: type => Construct<MethodReference>(6, 2, type),
            RunAll(
                converter : parts => parts[0],
                TypeSpecification.AsParser,
                Discard<TypeSpecification, string>(ConsumeWord(Core.Id, "::"))
            ),
            Empty<TypeSpecification>()
        ),
        Map(
            converter: name => Construct<MethodReference>(6, 3, name),
            MethodName.AsParser
        ),
        TryRun(
            converter: typeParams => Construct<MethodReference>(6, 4, typeParams),
            RunAll(
                converter : parts => parts[1],
                Discard<GenArgs, char>(ConsumeChar(Id, '<')),
                GenArgs.AsParser,
                Discard<GenArgs, char>(ConsumeChar(Id, '>'))
            ),
            Empty<GenArgs>()
        ),
        RunAll(
            converter: pars => Construct<MethodReference>(6, 5, pars[1]),
            Discard<SigArgument.Collection, char>(ConsumeChar(Id, '(')),
            SigArgument.Collection.AsParser,
            Discard<SigArgument.Collection, char>(ConsumeChar(Id, ')'))
        )
    );
}