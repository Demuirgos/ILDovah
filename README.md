# ILDovah
A basic MSIL parser (follows 2012 ECMA-CIL)

# Sample : 
```csharp 
using ResourceDecl;

var (source, index) = (""".class private auto ansi beforefieldinit Program
    extends [System.Runtime]System.Object
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )
    
    .method private hidebysig static 
        int32 '<Main>$' (
            string[] args
        ) cil managed 
    {
        
        
        .maxstack 2
        .entrypoint
        .locals init (
            [0] valuetype [System.Runtime]System.Nullable`1<int32> HeadBlockHash,
            [1] int32
        )

        .try
        {
            IL_0000: ldloca.s 0
            IL_0002: initobj valuetype [System.Runtime]System.Nullable`1<int32>
            IL_0008: ldloca.s 1
            IL_000a: ldc.i4.s 42
        } 
        finally
        {
            IL_00ca: ldstr "done"
            IL_00cf: call void [System.Console]System.Console::WriteLine(string)
            IL_00d4: endfinally
        } 

        
        IL_00d5: ldloc.s 1
        IL_00d7: ret
    } 

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        
        
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: ret
    } 

}""", 0);

TestConstruct<RootDecl.Declaration.Collection>(ref index, source);
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
.class private auto ansi beforefieldinit Program
    extends [System.Runtime]System.Object
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )
    
    .method private hidebysig static 
        int32 '<Main>$' (
            string[] args
        ) cil managed 
    {
        .maxstack 2
        .entrypoint
        .locals init (
            [0] valuetype [System.Runtime]System.Nullable`1<int32> HeadBlockHash,
            [1] int32
        )

        .try
        {
            IL_0000: ldloca.s 0
            IL_0002: initobj valuetype [System.Runtime]System.Nullable`1<int32>
            IL_0008: ldloca.s 1
            IL_000a: ldc.i4.s 42
        } 
        finally
        {
            IL_00ca: ldstr "done"
            IL_00cf: call void [System.Console]System.Console::WriteLine(string)
            IL_00d4: endfinally
        } 
        IL_00d5: ldloc.s 1
        IL_00d7: ret
    } 

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        .maxstack 8
        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: ret
    } 

}
```
