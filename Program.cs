using static Core;

var (source, index) = ("<[23]>", 0);

if(IDeclaration<GenericTypeArity>.Parse(ref index, source, out GenericTypeArity resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}

record PersonTest(string Name, int Age);