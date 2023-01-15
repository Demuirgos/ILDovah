.class public auto ansi beforefieldinit AssemblyDecl.HashClauseMember
       extends AssemblyDecl.Member
       implements class IDeclaration`1<class AssemblyDecl.HashClauseMember>,
                  class [System.Runtime]System.IEquatable`1<class AssemblyDecl.HashClauseMember>
{
  .custom instance void class WrapParserAttribute`1<class SecurityDecl.HashClause>::.ctor() = ( 01 00 00 00 ) 
  .interfaceimpl type class IDeclaration`1<class AssemblyDecl.HashClauseMember>
  .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 02 00 00 00 00 01 00 00 ) 
  .class auto ansi serializable sealed nested private beforefieldinit '<>c'
         extends [System.Runtime]System.Object
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static initonly class AssemblyDecl.HashClauseMember/'<>c' '<>9'
    .field public static class [System.Runtime]System.Func`2<class SecurityDecl.HashClause,class AssemblyDecl.HashClauseMember> '<>9__9_0'
    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  newobj     instance void AssemblyDecl.HashClauseMember/'<>c'::.ctor()
      IL_0005:  stsfld     class AssemblyDecl.HashClauseMember/'<>c' AssemblyDecl.HashClauseMember/'<>c'::'<>9'
      IL_000a:  ret
    } 

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
      IL_0006:  ret
    } 

    .method assembly hidebysig instance class AssemblyDecl.HashClauseMember 
            '<get_AsParser>b__9_0'(class SecurityDecl.HashClause 'value') cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  newobj     instance void AssemblyDecl.HashClauseMember::.ctor(class SecurityDecl.HashClause)
      IL_0006:  ret
    } 

  } 

  .field private initonly class SecurityDecl.HashClause '<Value>k__BackingField'
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method family hidebysig specialname virtual 
          instance class [System.Runtime]System.Type 
          get_EqualityContract() cil managed
  {
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldtoken    AssemblyDecl.HashClauseMember
    IL_0005:  call       class [System.Runtime]System.Type [System.Runtime]System.Type::GetTypeFromHandle(valuetype [System.Runtime]System.RuntimeTypeHandle)
    IL_000a:  ret
  } 

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class SecurityDecl.HashClause Value) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::'<Value>k__BackingField'
    IL_0007:  ldarg.0
    IL_0008:  call       instance void AssemblyDecl.Member::.ctor()
    IL_000d:  ret
  } 

  .method public hidebysig specialname instance class SecurityDecl.HashClause 
          get_Value() cil managed
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::'<Value>k__BackingField'
    IL_0006:  ret
  } 

  .method public hidebysig specialname instance void modreq([System.Runtime]System.Runtime.CompilerServices.IsExternalInit) 
          set_Value(class SecurityDecl.HashClause 'value') cil managed
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::'<Value>k__BackingField'
    IL_0007:  ret
  } 

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::get_Value()
    IL_0006:  callvirt   instance string [System.Runtime]System.Object::ToString()
    IL_000b:  ret
  } 

  .method public hidebysig specialname static 
          class Core/Parser`1<class AssemblyDecl.HashClauseMember> 
          get_AsParser() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Func`2<class SecurityDecl.HashClause,class AssemblyDecl.HashClauseMember> AssemblyDecl.HashClauseMember/'<>c'::'<>9__9_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class AssemblyDecl.HashClauseMember/'<>c' AssemblyDecl.HashClauseMember/'<>c'::'<>9'
    IL_000e:  ldftn      instance class AssemblyDecl.HashClauseMember AssemblyDecl.HashClauseMember/'<>c'::'<get_AsParser>b__9_0'(class SecurityDecl.HashClause)
    IL_0014:  newobj     instance void class [System.Runtime]System.Func`2<class SecurityDecl.HashClause,class AssemblyDecl.HashClauseMember>::.ctor(object,
                                                                                                                                                     native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [System.Runtime]System.Func`2<class SecurityDecl.HashClause,class AssemblyDecl.HashClauseMember> AssemblyDecl.HashClauseMember/'<>c'::'<>9__9_0'
    IL_001f:  call       class Core/Parser`1<!0> class IDeclaration`1<class SecurityDecl.HashClause>::get_AsParser()
    IL_0024:  call       class Core/Parser`1<!!0> Core::Map<class AssemblyDecl.HashClauseMember,class SecurityDecl.HashClause>(class [System.Runtime]System.Func`2<!!1,!!0>,
                                                                                                                               class Core/Parser`1<!!1>)
    IL_0029:  ret
  } 

  .method family hidebysig virtual instance bool 
          PrintMembers(class [System.Runtime]System.Text.StringBuilder builder) cil managed
  {
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool AssemblyDecl.Member::PrintMembers(class [System.Runtime]System.Text.StringBuilder)
    IL_0007:  brfalse.s  IL_0015

    IL_0009:  ldarg.1
    IL_000a:  ldstr      ", "
    IL_000f:  callvirt   instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
    IL_0014:  pop
    IL_0015:  ldarg.1
    IL_0016:  ldstr      "Value = "
    IL_001b:  callvirt   instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
    IL_0020:  pop
    IL_0021:  ldarg.1
    IL_0022:  ldarg.0
    IL_0023:  call       instance class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::get_Value()
    IL_0028:  callvirt   instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(object)
    IL_002d:  pop
    IL_002e:  ldc.i4.1
    IL_002f:  ret
  } 

  .method public hidebysig specialname static 
          bool  op_Inequality(class AssemblyDecl.HashClauseMember left,
                              class AssemblyDecl.HashClauseMember right) cil managed
  {
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       bool AssemblyDecl.HashClauseMember::op_Equality(class AssemblyDecl.HashClauseMember,
                                                                         class AssemblyDecl.HashClauseMember)
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  ret
  } 

  .method public hidebysig specialname static 
          bool  op_Equality(class AssemblyDecl.HashClauseMember left,
                            class AssemblyDecl.HashClauseMember right) cil managed
  {
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  beq.s      IL_0011

    IL_0004:  ldarg.0
    IL_0005:  brfalse.s  IL_000f

    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  callvirt   instance bool AssemblyDecl.HashClauseMember::Equals(class AssemblyDecl.HashClauseMember)
    IL_000e:  ret

    IL_000f:  ldc.i4.0
    IL_0010:  ret

    IL_0011:  ldc.i4.1
    IL_0012:  ret
  } 

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 AssemblyDecl.Member::GetHashCode()
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  mul
    IL_000c:  call       class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<class SecurityDecl.HashClause>::get_Default()
    IL_0011:  ldarg.0
    IL_0012:  ldfld      class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::'<Value>k__BackingField'
    IL_0017:  callvirt   instance int32 class [System.Collections]System.Collections.Generic.EqualityComparer`1<class SecurityDecl.HashClause>::GetHashCode(!0)
    IL_001c:  add
    IL_001d:  ret
  } 

  .method public hidebysig virtual instance bool 
          Equals(object obj) cil managed
  {
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  isinst     AssemblyDecl.HashClauseMember
    IL_0007:  callvirt   instance bool AssemblyDecl.HashClauseMember::Equals(class AssemblyDecl.HashClauseMember)
    IL_000c:  ret
  } 

  .method public hidebysig virtual final 
          instance bool  Equals(class AssemblyDecl.Member other) cil managed
  {
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance bool [System.Runtime]System.Object::Equals(object)
    IL_0007:  ret
  } 

  .method public hidebysig newslot virtual 
          instance bool  Equals(class AssemblyDecl.HashClauseMember other) cil managed
  {
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  beq.s      IL_0026

    IL_0004:  ldarg.0
    IL_0005:  ldarg.1
    IL_0006:  call       instance bool AssemblyDecl.Member::Equals(class AssemblyDecl.Member)
    IL_000b:  brfalse.s  IL_0024

    IL_000d:  call       class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<class SecurityDecl.HashClause>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::'<Value>k__BackingField'
    IL_0018:  ldarg.1
    IL_0019:  ldfld      class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::'<Value>k__BackingField'
    IL_001e:  callvirt   instance bool class [System.Collections]System.Collections.Generic.EqualityComparer`1<class SecurityDecl.HashClause>::Equals(!0,
                                                                                                                                                      !0)
    IL_0023:  ret

    IL_0024:  ldc.i4.0
    IL_0025:  ret

    IL_0026:  ldc.i4.1
    IL_0027:  ret
  } 

  .method public hidebysig newslot virtual 
          instance class AssemblyDecl.HashClauseMember 
          '<Clone>$'() cil managed
  {
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.PreserveBaseOverridesAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .override  method instance class AssemblyDecl.Member AssemblyDecl.Member::'<Clone>$'()
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void AssemblyDecl.HashClauseMember::.ctor(class AssemblyDecl.HashClauseMember)
    IL_0006:  ret
  } 

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor(class AssemblyDecl.HashClauseMember original) cil managed
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .param [1]
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void AssemblyDecl.Member::.ctor(class AssemblyDecl.Member)
    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  ldfld      class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::'<Value>k__BackingField'
    IL_000e:  stfld      class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::'<Value>k__BackingField'
    IL_0013:  ret
  } 

  .method public hidebysig instance void 
          Deconstruct([out] class SecurityDecl.HashClause& Value) cil managed
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  call       instance class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::get_Value()
    IL_0007:  stind.ref
    IL_0008:  ret
  } 

  .property instance callconv(8) class [System.Runtime]System.Type
          EqualityContract()
  {
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .get instance class [System.Runtime]System.Type AssemblyDecl.HashClauseMember::get_EqualityContract()
  } 
  .property instance callconv(8) class SecurityDecl.HashClause
          Value()
  {
    .get instance class SecurityDecl.HashClause AssemblyDecl.HashClauseMember::get_Value()
    .set instance void modreq([System.Runtime]System.Runtime.CompilerServices.IsExternalInit) AssemblyDecl.HashClauseMember::set_Value(class SecurityDecl.HashClause)
  } 
  .property callconv(8) class Core/Parser`1<class AssemblyDecl.HashClauseMember>
          AsParser()
  {
    .get class Core/Parser`1<class AssemblyDecl.HashClauseMember> AssemblyDecl.HashClauseMember::get_AsParser()
  } 
} 

.class private auto ansi sealed '<PrivateImplementationDetails>'
       extends [System.Runtime]System.Object
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=44'
         extends [System.Runtime]System.ValueType
  {
    .pack 1
    .size 44
  } 

  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=44' '8344D8DF7A47914137222D7A8A13B94A17DA43986EEB8F4A8F158A189A4C060F' at I_0007B518
  .method assembly hidebysig static uint32 
          ComputeStringHash(string s) cil managed
  {
    
    .maxstack  2
    .locals init (uint32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_002a

    IL_0003:  ldc.i4     0x811c9dc5
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_0021

    IL_000d:  ldarg.0
    IL_000e:  ldloc.1
    IL_000f:  callvirt   instance char [System.Runtime]System.String::get_Chars(int32)
    IL_0014:  ldloc.0
    IL_0015:  xor
    IL_0016:  ldc.i4     0x1000193
    IL_001b:  mul
    IL_001c:  stloc.0
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  ldarg.0
    IL_0023:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
    IL_0028:  blt.s      IL_000d

    IL_002a:  ldloc.0
    IL_002b:  ret
  } 

} 