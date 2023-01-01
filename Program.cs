using static Core;

// Note(Ayman) : Add whitespace skipping to all parsers, and add a parser for whitespace

var (source, index) = (".field public static int32 count = int32(23)", 0);

TestConstruct<Field>(ref index, source);

void TestConstruct<T>(ref int index, string source)
    where T : IDeclaration<T>
    {
        if(IDeclaration<T>.Parse(ref index, source, out T resultVal)) {
            Console.WriteLine(resultVal);
        } else {
            Console.WriteLine("Failed to parse");
        }
    }