using System.Text;
using static Core;

public record Parameter() : IDeclaration<Parameter> {
    private Object _value;
    public record Collection(Parameter[] Parameters) : IDeclaration<Collection> {
        public override string ToString() => $"({string.Join(", ", Parameters.Select(x => x.ToString()))})";
        public static Parser<Collection> AsParser => RunAll(
            converter: (parameters) => new Collection(parameters.SelectMany(Id).ToArray()),
            Map(
                converter: (parameter) => new Parameter[] { parameter },
                Parameter.AsParser
            ),
            RunMany(
                converter: (parameters) => parameters,
                0, Int32.MaxValue,
                RunAll(
                    converter: (vals) => vals[1],
                    Discard<Parameter, string>(ConsumeWord(Id, ",")),
                    Parameter.AsParser
                )
            )
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