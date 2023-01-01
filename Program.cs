using static Core;

var (source, index) = ("nullref", 0);

if(IDeclaration<FieldInit>.Parse(ref index, source, out FieldInit resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}

record PersonTest(string Name, int Age);