using static Core;
// See https://aka.ms/new-console-template for more information
var (source, index) = ("(010000010000)", 0);

if(ARRAY<BYTE>.Parse(ref index, source, out ARRAY<BYTE> resultVal, ('(', '\0', ')'))) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}