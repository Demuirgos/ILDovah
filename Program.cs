using static Core;
// See https://aka.ms/new-console-template for more information
string source1 = ".line1:23'program.cs'";
int index1 = 0;

if(ExternSource.Parse(ref index1, source1, out ExternSource floatVal1)) {
    Console.WriteLine(floatVal1);
} else {
    Console.WriteLine("Failed to parse");
}