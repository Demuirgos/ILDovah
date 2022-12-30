using System.Reflection;
using static Core;

var (source, index) = ("deadbeef", 0);

if(ARRAY<BYTE>.Parse(ref index, source, out ARRAY<BYTE> resultVal, ('\0', '\0', '\0'))) {
    Console.WriteLine(resultVal.ToString(' '));
} else {
    Console.WriteLine("Failed to parse");
}

// if(IDeclaration<SlashedName2>.Parse(ref index, source, out SlashedName2 resultVal)) {
//     Console.WriteLine(resultVal);
// } else {
//     Console.WriteLine("Failed to parse");
// }

// Console.WriteLine(index);