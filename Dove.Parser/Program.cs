using ResourceDecl;

var (source, index) = ("""
.class private auto ansi beforefieldinit test`1<T>
    extends [System.Runtime]System.Object
{
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
        01 00 01 00 00
    )
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = (
        01 00 00 00 00
    )
    .param type T
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = (
        01 00 02 00 00
    )
    
    .field private !T '<field>k__BackingField'
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

    
    .method public hidebysig specialname rtspecialname 
        instance void .ctor (
            !T val
        ) cil managed 
    {
        
        
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: ldarg.0
        IL_0007: ldarg.1
        IL_0008: call instance void class test`1<!T>::set_field(!0)
        IL_000d: ret
    } 

    .method public hidebysig specialname 
        instance !T get_field () cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
        
        
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldfld !0 class test`1<!T>::'<field>k__BackingField'
        IL_0006: ret
    } 

    .method public hidebysig specialname 
        instance void set_field (
            !T 'value'
        ) cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
        
        
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldarg.1
        IL_0002: stfld !0 class test`1<!T>::'<field>k__BackingField'
        IL_0007: ret
    } 

    
    .property instance !T 'field'()
    {
        .get instance !0 test`1::get_field()
        .set instance void test`1::set_field(!0)
    }

}
""", 0);

TestConstruct<ClassDecl.Class>(ref index, source, out _);

void TestConstruct<T>(ref int index, string source, out T resultVal)
    where T : IDeclaration<T>
{
    if (IDeclaration<T>.Parse(ref index, source, out resultVal))
    {
        Console.WriteLine(resultVal);
    }
    else
    {
        Console.WriteLine("Failed to parse");
    }
}
