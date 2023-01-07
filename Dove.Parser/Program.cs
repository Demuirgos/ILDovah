// Note(Ayman) : Fix whitespace in ToString methods
// Note(Ayman) : Add Extra checks to Nested Self referential Parsers to avoid 
//               infinite loops or stack overflows 
// Note(Ayman) : Fix sourge generator clumping types with same names as same type
var (source, index) = (".file File.dll", 0);

TestConstruct<FileReference>(ref index, source);
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