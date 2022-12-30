using System.Reflection;
using static Core;

var (source, index) = ("...,23...69", 0);

if(IDeclaration<Bound.Collection>.Parse(ref index, source, out Bound.Collection resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}