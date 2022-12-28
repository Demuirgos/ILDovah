using static Core;
// See https://aka.ms/new-console-template for more information
var (source, index) = ("pinvokeimpl(\"user32.dll\"stdcall)", 0);
//var (source, index) = ("assembly", 0);

if(IDeclaration<MethodAttribute>.Parse(ref index, source, out MethodAttribute resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}