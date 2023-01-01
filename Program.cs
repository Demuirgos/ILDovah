using static Core;

// Note(Ayman) : Add whitespace skipping to all parsers, and add a parser for whitespace

var (source, index) = (".property int32 Count() { .get instance int32 MyCount::get_Count() .set instance void MyCount::set_Count(int32) .other instance void MyCount::reset_Count() }", 0);

TestConstruct<Property>(ref index, source);

void TestConstruct<T>(ref int index, string source)
    where T : IDeclaration<T>
    {
        if(IDeclaration<T>.Parse(ref index, source, out T resultVal)) {
            Console.WriteLine(resultVal);
        } else {
            Console.WriteLine("Failed to parse");
        }
    }