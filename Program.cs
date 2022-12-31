using static Core;

var (source, index) = ("boolTest2,bool", 0);

if(IDeclaration<Local.Collection>.Parse(ref index, source, out Local.Collection resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}

record PersonTest(string Name, int Age);