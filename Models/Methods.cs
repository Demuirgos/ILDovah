using System.Reflection.Metadata;
using System.Text;
using static Core;
using static Extensions;
public record MethodDeclaration(bool IsConstructor) : IDeclaration<MethodDeclaration> {
    public override string ToString() => throw new NotImplementedException();
    public static Parser<MethodDeclaration> AsParser => throw new NotImplementedException();
}
public record MethodHeader(MethodAttribute.Collection MethodAttributes, CallConvention? Convention, Type Type, NativeType? MarshalledType, MethodName Name, GenericParameter.Collection? TypeParameters, Parameter.Collection Parameters, ImplAttribute.Collection ImplementationAttributes) : IDeclaration<MethodName> {
    public override string ToString() {
        var sb = new StringBuilder();
        sb.Append(MethodAttributes);
        if (Convention != null) {
            sb.Append($" {Convention}");
        }
        sb.Append(Type);
        if (MarshalledType != null) {
            sb.Append($" marshal ({MarshalledType})");
        }
        sb.Append(Name);
        if (TypeParameters != null) {
            sb.Append($" <{TypeParameters}>");
        }
        sb.Append($" ({Parameters})");
        if (ImplementationAttributes != null) {
            sb.Append($" {ImplementationAttributes} ");
        }
        return sb.ToString();
    }
    public static Parser<MethodHeader> AsParser => RunAll(
        converter: parts => new MethodHeader(
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
            converter: attrs => Construct<MethodHeader>(8, 0, attrs),
            MethodAttribute.Collection.AsParser
        ),
        TryRun(
            converter: conv => Construct<MethodHeader>(8, 1, conv),
            CallConvention.AsParser
        ),
        Map(
            converter: type => Construct<MethodHeader>(8, 2, type),
            Type.AsParser
        ),
        TryRun(
            converter: type => Construct<MethodHeader>(8, 3, type),
            RunAll(
                converter: parts => parts[2],
                Discard<NativeType, string>(ConsumeWord(Id, "marshal")),
                Discard<NativeType, char>(ConsumeChar(Id, '(')),
                NativeType.AsParser,
                Discard<NativeType, char>(ConsumeChar(Id, ')'))
            )
        ),
        Map(
            converter: name => Construct<MethodHeader>(8, 4, name),
            MethodName.AsParser
        ),
        TryRun(
            converter: genpars => Construct<MethodHeader>(8, 5, genpars),
            RunAll(
                converter: pars => pars[1],
                Discard<GenericParameter.Collection, char>(ConsumeChar(Id, '<')),
                GenericParameter.Collection.AsParser,
                Discard<GenericParameter.Collection, char>(ConsumeChar(Id, '>'))
            ),
            Empty<GenericParameter.Collection>()
        ),
        RunAll(
            converter: pars => Construct<MethodHeader>(8, 6, pars[1]),
            Discard<Parameter.Collection, char>(ConsumeChar(Id, '(')),
            Parameter.Collection.AsParser,
            Discard<Parameter.Collection, char>(ConsumeChar(Id, ')'))
        ),
        Map(
            converter: implattrs => Construct<MethodHeader>(8, 7, implattrs),
            ImplAttribute.Collection.AsParser
        )
    );
}
public record MethodName(String Name) : IDeclaration<MethodName> {
    public override string ToString() => Name;
    public static Parser<MethodName> AsParser => TryRun(
        converter: (vals) => new MethodName(vals),
        ConsumeWord(Id, ".ctor"),
        ConsumeWord(Id, ".cctor"),
        Map((dname) => dname.ToString(), DottedName.AsParser)
    );
}
