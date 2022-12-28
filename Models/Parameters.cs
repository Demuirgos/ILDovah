using static Core;

public record Parameter() : IDeclaration<Parameter> {
    private Object _value;
    public record VarargParameter() : Parameter {
        public override string ToString() => "...";
        public static Parser<VarargParameter> AsParser => ConsumeWord(_ => new VarargParameter(), "...");
    }
    public record DefaultParameter(ParamAttribute[] Attributes, Type TypeDecl, NativeType MarshalledType, Identifier Id) : Parameter {
        public override string ToString() => "...";
        public static Parser<VarargParameter> AsParser => ConsumeWord(_ => new VarargParameter(), "...");
    }
}


/*
Parameters ::= [ Param [ ‘,’ Param ]* ]
Param ::=
 ...
| [ ParamAttr* ] Type [ marshal ‘(’ [ NativeType ] ‘)’ ] [ Id ]
*/