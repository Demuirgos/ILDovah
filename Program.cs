using System.Reflection;
using static Core;

var (source, index) = ("bool[2][23+23]", 0);

if(IDeclaration<NativeType>.Parse(ref index, source, out NativeType resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}