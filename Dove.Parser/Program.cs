// Note(Ayman) : Fix whitespace in ToString methods
// Note(Ayman) : Add Extra checks to Nested Self referential Parsers to avoid 
//               infinite loops or stack overflows 
// Note(Ayman) : Fix ToString asap
// Note(Ayman) : write tests for all parsers
using ResourceDecl;

var (source, index) = ("""
.method public hidebysig specialname rtspecialname 
    instance void .ctor () cil managed 
{
    .maxstack 8

    IL_0000: ldarg.0
    IL_0001: call instance void [System.Runtime]System.Object::.ctor()
    IL_0006: ret
}
""", 0);

TestConstruct<MethodDecl.Method>(ref index, source);
void TestConstruct<T>(ref int index, string source)
    where T : IDeclaration<T>
{
    if (IDeclaration<T>.Parse(ref index, source, out T resultVal))
    {
        Console.WriteLine(resultVal);
    }
    else
    {
        Console.WriteLine("Failed to parse");
    }
}