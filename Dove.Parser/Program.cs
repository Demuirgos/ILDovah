// Note(Ayman) : Fix whitespace in ToString methods
// Note(Ayman) : Add Extra checks to Nested Self referential Parsers to avoid 
//               infinite loops or stack overflows 
// Note(Ayman) : Fix ToString asap
// Note(Ayman) : write tests for all parsers
using ResourceDecl;

var (source, index) = ("""
.class private auto ansi beforefieldinit Program
    extends [System.Runtime]System.Object
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )
    .method private hidebysig static 
        void '<Main>$' (
            string[] args
        ) cil managed 
    {
        .maxstack 8
        .entrypoint

        IL_0000: newobj instance void test::.ctor()
        IL_0005: ldfld int32 test::'field'
        IL_000a: call void [System.Console]System.Console::Write(int32)
        IL_000f: ret
    } 

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: ret
    } 

} 
""", 0);

TestConstruct<ClassDecl.Class>(ref index, source, out _);

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