using static Core;

// Note(Ayman) : Add whitespace skipping to all parsers, and add a parser for whitespace

var (source, index) = (".field [23] public static int32 count".Replace(" ", String.Empty), 0);

if(IDeclaration<Field>.Parse(ref index, source, out Field resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}

record PersonTest(string Name, int Age);