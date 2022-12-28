using static Core;
/*
MethAttr* [ CallConv ] Type [ marshal ‘(’ [ NativeType ] ‘)’ ] MethodName [ ‘<’ GenPars‘>’ ] ‘(’ Parameters ‘)’ ImplAttr*
*/
public record MethodDeclaration(bool IsConstructor) : IDeclaration<MethodDeclaration> {
    public override string ToString() => throw new NotImplementedException();
    public static Parser<MethodDeclaration> AsParser => throw new NotImplementedException();
}
public record MethodName(String Name) : IDeclaration<MethodName> {
    public override string ToString() => Name;
    public static Parser<MethodName> AsParser => TryRun(
        converter: (vals) => new MethodName(vals),
        ConsumeWord(Id, ".ctor"),
        ConsumeWord(Id, ".cctor"),
        Map((dname) => dname.Value, DottedName.AsParser)
    );
}
