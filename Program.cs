using static Core;

var (source, index) = ("char*(\"test\")", 0);

if(IDeclaration<DataItem>.Parse(ref index, source, out DataItem resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}

record PersonTest(string Name, int Age);