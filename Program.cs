using static Core;

// Note(Ayman) : Fix whitespace in ToString methods
// Note(Ayman) : Add Extra checks to Nested Self referential Parsers to avoid 
//               infinite loops or stack overflows 

var (source, index) = (".class C implements I { .method virtual public void M2() { } .override I::M with instance void C::M2() }", 0);



TestConstruct<Class>(ref index, source);

void TestConstruct<T>(ref int index, string source)
    where T : IDeclaration<T>
    {
        if(IDeclaration<T>.Parse(ref index, source, out T resultVal)) {
            Console.WriteLine(resultVal);
        } else {
            Console.WriteLine("Failed to parse");
        }
    }