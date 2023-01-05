# ILDovah
A basic MSIL parser (follows 2012 ECMA-CIL) (WIP)

```csharp
var (source, index) = (".field public static int32 count = int32(23)", 0);

TestConstruct<Field>(ref index, source);

void TestConstruct<T>(ref int index, string source)
    where T : IDeclaration<T>
    {
        if(IDeclaration<T>.Parse(ref index, source, out T resultVal)) {
            Console.WriteLine(resultVal);
        } else {
            Console.WriteLine("Failed to parse");
        }
    }
```
