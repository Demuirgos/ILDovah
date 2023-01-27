.class public auto ansi beforefieldinit Student
    extends [System.Runtime]System.Object
    implements 
    class [System.Runtime]System.IEquatable`1<class Student>
{
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
        01 00 01 00 00
    )
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = (
        01 00 00 00 00
    )
    .field private initonly string '<Name>k__BackingField'
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )
    .field private initonly int32 '<Age>k__BackingField'
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

        .method public hidebysig specialname rtspecialname 
        instance void .ctor (
            string Name,
            int32 Age
        ) cil managed 
    {
                        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldarg.1
        IL_0002: stfld string Student::'<Name>k__BackingField'
        IL_0007: ldarg.0
        IL_0008: ldarg.2
        IL_0009: stfld int32 Student::'<Age>k__BackingField'
        IL_000e: ldarg.0
        IL_000f: call instance void [System.Runtime]System.Object::.ctor()
        IL_0014: ret
    } 
    .method family hidebysig specialname newslot virtual 
        instance class [System.Runtime]System.Type get_EqualityContract () cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldtoken Student
        IL_0005: call class [System.Runtime]System.Type [System.Runtime]System.Type::GetTypeFromHandle(valuetype [System.Runtime]System.RuntimeTypeHandle)
        IL_000a: ret
    } 
    .method public hidebysig specialname 
        instance string get_Name () cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldfld string Student::'<Name>k__BackingField'
        IL_0006: ret
    } 
    .method public hidebysig specialname 
        instance void modreq([System.Runtime]System.Runtime.CompilerServices.IsExternalInit) set_Name (
            string 'value'
        ) cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldarg.1
        IL_0002: stfld string Student::'<Name>k__BackingField'
        IL_0007: ret
    } 
    .method public hidebysig specialname 
        instance int32 get_Age () cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldfld int32 Student::'<Age>k__BackingField'
        IL_0006: ret
    } 
    .method public hidebysig specialname 
        instance void modreq([System.Runtime]System.Runtime.CompilerServices.IsExternalInit) set_Age (
            int32 'value'
        ) cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldarg.1
        IL_0002: stfld int32 Student::'<Age>k__BackingField'
        IL_0007: ret
    } 
    .method public hidebysig virtual 
        instance string ToString () cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 2
        .locals init (
            class [System.Runtime]System.Text.StringBuilder
        )

        IL_0000: newobj instance void [System.Runtime]System.Text.StringBuilder::.ctor()
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: ldstr "Student"
        IL_000c: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
        IL_0011: pop
        IL_0012: ldloc.0
        IL_0013: ldstr " { "
        IL_0018: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
        IL_001d: pop
        IL_001e: ldarg.0
        IL_001f: ldloc.0
        IL_0020: callvirt instance bool Student::PrintMembers(class [System.Runtime]System.Text.StringBuilder)
        IL_0025: brfalse.s IL_0030

        IL_0027: ldloc.0
        IL_0028: ldc.i4.s 32
        IL_002a: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(char)
        IL_002f: pop

        IL_0030: ldloc.0
        IL_0031: ldc.i4.s 125
        IL_0033: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(char)
        IL_0038: pop
        IL_0039: ldloc.0
        IL_003a: callvirt instance string [System.Runtime]System.Object::ToString()
        IL_003f: ret
    } 
    .method family hidebysig newslot virtual 
        instance bool PrintMembers (
            class [System.Runtime]System.Text.StringBuilder builder
        ) cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 2
        .locals init (
            [0..] int32
        )

        IL_0000: call void [System.Runtime]System.Runtime.CompilerServices.RuntimeHelpers::EnsureSufficientExecutionStack()
        IL_0005: ldarg.1
        IL_0006: ldstr "Name = "
        IL_000b: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
        IL_0010: pop
        IL_0011: ldarg.1
        IL_0012: ldarg.0
        IL_0013: call instance string Student::get_Name()
        IL_0018: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(object)
        IL_001d: pop
        IL_001e: ldarg.1
        IL_001f: ldstr ", Age = "
        IL_0024: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
        IL_0029: pop
        IL_002a: ldarg.1
        IL_002b: ldarg.0
        IL_002c: call instance int32 Student::get_Age()
        IL_0031: stloc.0
        IL_0032: ldloca.s 0
        IL_0034: constrained. [System.Runtime]System.Int32
        IL_003a: callvirt instance string [System.Runtime]System.Object::ToString()
        IL_003f: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
        IL_0044: pop
        IL_0045: ldc.i4.1
        IL_0046: ret
    } 
    .method public hidebysig specialname static 
        bool op_Inequality (
            class Student left,
            class Student right
        ) cil managed 
    {
        .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
            01 00 02 00 00
        )
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldarg.1
        IL_0002: call bool Student::op_Equality(class Student, class Student)
        IL_0007: ldc.i4.0
        IL_0008: ceq
        IL_000a: ret
    } 
    .method public hidebysig specialname static 
        bool op_Equality (
            class Student left,
            class Student right
        ) cil managed 
    {
        .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
            01 00 02 00 00
        )
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldarg.1
        IL_0002: beq.s IL_0011

        IL_0004: ldarg.0
        IL_0005: brfalse.s IL_000f

        IL_0007: ldarg.0
        IL_0008: ldarg.1
        IL_0009: callvirt instance bool Student::Equals(class Student)
        IL_000e: ret

        IL_000f: ldc.i4.0
        IL_0010: ret

        IL_0011: ldc.i4.1
        IL_0012: ret
    } 
    .method public hidebysig virtual 
        instance int32 GetHashCode () cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<class [System.Runtime]System.Type>::get_Default()
        IL_0005: ldarg.0
        IL_0006: callvirt instance class [System.Runtime]System.Type Student::get_EqualityContract()
        IL_000b: callvirt instance int32 class [System.Collections]System.Collections.Generic.EqualityComparer`1<class [System.Runtime]System.Type>::GetHashCode(!0)
        IL_0010: ldc.i4 -1521134295
        IL_0015: mul
        IL_0016: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<string>::get_Default()
        IL_001b: ldarg.0
        IL_001c: ldfld string Student::'<Name>k__BackingField'
        IL_0021: callvirt instance int32 class [System.Collections]System.Collections.Generic.EqualityComparer`1<string>::GetHashCode(!0)
        IL_0026: add
        IL_0027: ldc.i4 -1521134295
        IL_002c: mul
        IL_002d: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::get_Default()
        IL_0032: ldarg.0
        IL_0033: ldfld int32 Student::'<Age>k__BackingField'
        IL_0038: callvirt instance int32 class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::GetHashCode(!0)
        IL_003d: add
        IL_003e: ret
    } 
    .method public hidebysig virtual 
        instance bool Equals (
            object obj
        ) cil managed 
    {
        .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
            01 00 02 00 00
        )
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldarg.1
        IL_0002: isinst Student
        IL_0007: callvirt instance bool Student::Equals(class Student)
        IL_000c: ret
    } 
    .method public hidebysig newslot virtual 
        instance bool Equals (
            class Student others
        ) cil managed 
    {
        .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
            01 00 02 00 00
        )
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 3

        IL_0000: ldarg.0
        IL_0001: ldarg.1
        IL_0002: beq.s IL_004b

        IL_0004: ldarg.1
        IL_0005: brfalse.s IL_0049

        IL_0007: ldarg.0
        IL_0008: callvirt instance class [System.Runtime]System.Type Student::get_EqualityContract()
        IL_000d: ldarg.1
        IL_000e: callvirt instance class [System.Runtime]System.Type Student::get_EqualityContract()
        IL_0013: call bool [System.Runtime]System.Type::op_Equality(class [System.Runtime]System.Type, class [System.Runtime]System.Type)
        IL_0018: brfalse.s IL_0049

        IL_001a: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<string>::get_Default()
        IL_001f: ldarg.0
        IL_0020: ldfld string Student::'<Name>k__BackingField'
        IL_0025: ldarg.1
        IL_0026: ldfld string Student::'<Name>k__BackingField'
        IL_002b: callvirt instance bool class [System.Collections]System.Collections.Generic.EqualityComparer`1<string>::Equals(!0, !0)
        IL_0030: brfalse.s IL_0049

        IL_0032: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::get_Default()
        IL_0037: ldarg.0
        IL_0038: ldfld int32 Student::'<Age>k__BackingField'
        IL_003d: ldarg.1
        IL_003e: ldfld int32 Student::'<Age>k__BackingField'
        IL_0043: callvirt instance bool class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::Equals(!0, !0)
        IL_0048: ret

        IL_0049: ldc.i4.0
        IL_004a: ret

        IL_004b: ldc.i4.1
        IL_004c: ret
    } 
    .method public hidebysig newslot virtual 
        instance class Student '<Clone>$' () cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: newobj instance void Student::.ctor(class Student)
        IL_0006: ret
    } 
    .method family hidebysig specialname rtspecialname 
        instance void .ctor (
            class Student original
        ) cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: ldarg.0
        IL_0007: ldarg.1
        IL_0008: ldfld string Student::'<Name>k__BackingField'
        IL_000d: stfld string Student::'<Name>k__BackingField'
        IL_0012: ldarg.0
        IL_0013: ldarg.1
        IL_0014: ldfld int32 Student::'<Age>k__BackingField'
        IL_0019: stfld int32 Student::'<Age>k__BackingField'
        IL_001e: ret
    } 
    .method public hidebysig 
        instance void Deconstruct (
            [out] string& Name,
            [out] int32& Age
        ) cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
                        .maxstack 8

        IL_0000: ldarg.1
        IL_0001: ldarg.0
        IL_0002: call instance string Student::get_Name()
        IL_0007: stind.ref
        IL_0008: ldarg.2
        IL_0009: ldarg.0
        IL_000a: call instance int32 Student::get_Age()
        IL_000f: stind.i4
        IL_0010: ret
    } 
        .property instance class [System.Runtime]System.Type EqualityContract()
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
        .get instance class [System.Runtime]System.Type Student::get_EqualityContract()
    }
    .property instance string Name()
    {
        .get instance string Student::get_Name()
        .set instance void modreq([System.Runtime]System.Runtime.CompilerServices.IsExternalInit) Student::set_Name(string)
    }
    .property instance int32 Age()
    {
        .get instance int32 Student::get_Age()
        .set instance void modreq([System.Runtime]System.Runtime.CompilerServices.IsExternalInit) Student::set_Age(int32)
    }

} 