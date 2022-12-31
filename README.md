# ILDovah
A basic MSIL parser (follows 2012 ECMA-CIL) (WIP)

```csharp
var (source, index) = ("RandomIdentifier", 0);

TestConstruct<Identifier>(ref index, source);

publib TestConstruct<T> where T:IDeclaration<T>(ref index, string source) {
  if(IDeclaration<T>.Parse(ref index, source, out T resultVal)) {
    Console.WriteLine(resultVal);
  } else {
      Console.WriteLine("Failed to parse");
  }
}
```
