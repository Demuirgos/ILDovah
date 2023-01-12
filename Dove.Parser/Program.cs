using ResourceDecl;

var (source, index) = (File.ReadAllText("Template.IL"), 0);

TestConstruct<RootDecl.Declaration.Collection>(ref index, source, out _);

void TestConstruct<T>(ref int index, string source, out T resultVal)
    where T : IDeclaration<T>
{
    if (IDeclaration<T>.Parse(ref index, source, out resultVal))
    {
        Console.WriteLine(resultVal);
    }
    else
    {
        Console.WriteLine("Failed to parse");
    }
}
