using System.Reflection;
using static Core;

var (source, index) = ("!!23", 0);

if(IDeclaration<Type>.Parse(ref index, source, out Type resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}