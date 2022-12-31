using static Core;
public record FieldInit() : IDeclaration<FieldInit> {
    public override string ToString() => throw new NotImplementedException();
    public static Parser<FieldInit> AsParser => throw new NotImplementedException();
}