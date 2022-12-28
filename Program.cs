using static Core;
// See https://aka.ms/new-console-template for more information
var (source, index) = ("2369420.23234", 0);

if(IDeclaration<FLOAT>.Parse(ref index, source, out FLOAT resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}