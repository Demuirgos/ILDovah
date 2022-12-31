using static Core;

var (source, index) = (".permissionsetdeny=(deadbeef)", 0);

if(IDeclaration<SecurityBlock>.Parse(ref index, source, out SecurityBlock resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}

record PersonTest(string Name, int Age);