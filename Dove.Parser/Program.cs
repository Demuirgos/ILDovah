// Note(Ayman) : Fix whitespace in ToString methods
// Note(Ayman) : Add Extra checks to Nested Self referential Parsers to avoid 
//               infinite loops or stack overflows 
// Note(Ayman) : Fix ToString asap
// Note(Ayman) : write tests for all parsers
using ResourceDecl;

var (source, index) = ("ldc.i4 23 ldc.i4 3 mul ret", 0);

TestConstruct<InstructionDecl.Instruction.Block>(ref index, source);
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