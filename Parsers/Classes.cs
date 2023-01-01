using static Core;
using static Extensions;
public record Class : IDeclaration<Class> {
    public override string ToString() => throw new NotImplementedException();
    public static Parser<Class> AsParser => throw new NotImplementedException();
}