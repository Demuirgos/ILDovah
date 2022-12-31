using static Core;

var (source, index) = ("class+(bool,int16)typename", 0);

var part1 = Extensions.Construct<PersonTest>(2, 0, "Ayman");
var part2 = Extensions.Construct<PersonTest>(2, 1, 3);
var complete = new PersonTest(part1.Name, part2.Age);
Console.WriteLine(complete);

if(IDeclaration<GenericParameter>.Parse(ref index, source, out GenericParameter resultVal)) {
    Console.WriteLine(resultVal);
} else {
    Console.WriteLine("Failed to parse");
}

record PersonTest(string Name, int Age);