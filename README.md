# ILDovah
A basic MSIL parser (follows 2012 ECMA-CIL)

# Sample : 
```csharp 
using ResourceDecl;

var (source, index) = (".file File.dll", 0);

TestConstruct<FileReference>(ref index, source);
void TestConstruct<T>(ref int index, string source)
    where T : IDeclaration<T>
{
    if (IDeclaration<T>.Parse(ref index, source, out T resultVal))
    {
        Console.WriteLine(resultVal);
    }
    else
    {
        Console.WriteLine("Failed to parse");
    }
}
```
# Output :
```
.file File.dll
```
