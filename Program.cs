using System.Reflection;
using static Core;

var (source, index) = ("...", 0);

if(IDeclaration<Parameter>.Parse(ref index, source, out Parameter resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}