// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

string source1 = "true12ef12.23[de,ad,be,ef]";
int index1 = 0;
BOOL.Parse(ref index1, source1, out BOOL boolVal);
Console.WriteLine(boolVal);
INT.Parse(ref index1, source1, out INT intVal);
Console.WriteLine(intVal);
BYTE.Parse(ref index1, source1, out BYTE byteVal);
Console.WriteLine(byteVal);
FLOAT.Parse(ref index1, source1, out FLOAT floatVal);
Console.WriteLine(floatVal);
ARRAY<BYTE>.Parse(ref index1, source1, out ARRAY<BYTE> bytearrVal, ('[', ',' ,']'));
Console.WriteLine(bytearrVal);
