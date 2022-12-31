using System.Reflection.Metadata;
using System.Text;
using static Core;
using GenericConstraints = Type.Collection;
public record Parameter() : IDeclaration<Parameter> {
    private Object _value;
    public record Collection(ARRAY<Parameter> Parameters) : IDeclaration<Collection> {
        public override string ToString() => Parameters.ToString();
        public static Parser<Collection> AsParser => Map(
            converter: (parameters) => new Collection(parameters),
            ARRAY<Parameter>.MakeParser('\0', ',', '\0')
        );
    }
    public record VarargParameter() : Parameter {
        public override string ToString() => "...";
        public static Parser<VarargParameter> AsParser => ConsumeWord(_ => new VarargParameter(), "...");
    }
    public record DefaultParameter(ParamAttribute.Collection Attributes, Type TypeDecl, NativeType MarshalledType, Identifier Id) : Parameter {
        // Note : Test this after implementing Type.AsParser
        public override string ToString() {
            StringBuilder sb = new();
            if(Attributes is not null) {
                sb.Append($"{Attributes} ");
            }
            if(TypeDecl is not null) {
                sb.Append(TypeDecl);
            }
            if(MarshalledType is not null) {
                sb.Append($" marshal ({MarshalledType})");
            }
            if(Id is not null) {
                sb.Append($" {Id} ");
            }
            return sb.ToString();
        }
        public static Parser<DefaultParameter> AsParser => RunAll(
            converter: parts => new DefaultParameter(parts[0].Attributes, parts[1].TypeDecl, parts[2].MarshalledType, parts[3].Id),
            TryRun(
                converter: attrs => new DefaultParameter(attrs, null, null, null),
                ParamAttribute.Collection.AsParser
            ),
            Map(
                converter: (type) => new DefaultParameter(null, type, null, null),
                Type.AsParser
            ),
            TryRun(
                converter: (type) => new DefaultParameter(null, null, type, null),
                RunAll(
                    converter: vals => vals[2],
                    Discard<NativeType, string>(ConsumeWord(Core.Id, "marshal")),
                    Discard<NativeType, char>(ConsumeChar(Core.Id, '(')),
                    TryRun(Core.Id, NativeType.AsParser, Empty<NativeType>()),
                    Discard<NativeType, char>(ConsumeChar(Core.Id, ')'))
                ), 
                Empty<NativeType>()
            ),
            TryRun(
                converter: (id) => new DefaultParameter(null, null, null, id),
                Identifier.AsParser, Empty<Identifier>()
            )
        );
    }

    public override string ToString() => _value switch {
        DefaultParameter param => param.ToString(),
        VarargParameter param => param.ToString(),
        _ => throw new Exception("Invalid parameter type")
    };
    public static Parser<Parameter> AsParser => TryRun(
        converter: (param) => param,
        Map(
            converter: res => new Parameter() { _value = res },
            DefaultParameter.AsParser
        ),
        Map(
            converter : res => new Parameter() { _value = res },
            VarargParameter.AsParser
        )
    );
}

public record GenericParameter(GenParamAttribute.Collection Attributes, Type.Collection Constraints, Identifier Id) : IDeclaration<GenericParameter> {
    public record Collection(ARRAY<GenericParameter> Parameters) : IDeclaration<Collection> {
        public override string ToString() => Parameters.ToString();
        public static Parser<Collection> AsParser => Map(
            converter: (parameters) => new Collection(parameters),
            ARRAY<GenericParameter>.MakeParser('\0', ',', '\0')
        );
    }

    public override string ToString() {
        StringBuilder sb = new();
        if(Attributes is not null) {
            sb.Append($"{Attributes} ");
        }
        if(Constraints is not null) {
            sb.Append($"( {Constraints}) ");
        }
        if(Id is not null) {
            sb.Append(Id);
        }
        return sb.ToString();
    }
    public static Parser<GenericParameter> AsParser => RunAll(
        converter: parts => new GenericParameter(parts[0].Attributes, parts[1].Constraints, parts[2].Id),
        Map(
            converter: attrs => new GenericParameter(attrs, null, null),
            GenParamAttribute.Collection.AsParser
        ),
        TryRun(
            converter: constraints => new GenericParameter(null, constraints, null),
            RunAll(
                converter: vals => vals[1],
                Discard<Type.Collection, char>(ConsumeChar(Core.Id, '(')),
                GenericConstraints.AsParser,
                Discard<Type.Collection, char>(ConsumeChar(Core.Id, ')'))
            ),
            Empty<Type.Collection>()
        ),
        Map(
            converter: id => new GenericParameter(null, null, id),
            Identifier.AsParser
        )
    );
}

public record GenericTypeArity(INT? Value) : IDeclaration<GenericTypeArity> {
    public override string ToString() => Value is null ? String.Empty : $"<[{Value}]>";
    public static Parser<GenericTypeArity> AsParser => TryRun(
        converter: (arity) => new GenericTypeArity(arity),
        RunAll(
            converter: vals => vals[2],
            Discard<INT, char>(ConsumeChar(Core.Id, '<')),
            Discard<INT, char>(ConsumeChar(Core.Id, '[')),
            INT.AsParser,
            Discard<INT, char>(ConsumeChar(Core.Id, ']')),
            Discard<INT, char>(ConsumeChar(Core.Id, '>'))
        ),
        Empty<INT>()
    );
}
