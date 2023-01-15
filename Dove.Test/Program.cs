var result = Parser.Parse<MethodDecl.Method>("""
.method public hidebysig static bool  TryParse<(class IDeclaration`1<!!T>) T>(string source,
                                                                                [out] !!T& result) cil managed
  {
    .param type T 
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .param [2] = int32(0)
    .custom instance void [System.Runtime]System.Diagnostics.CodeAnalysis.NotNullWhenAttribute::.ctor(bool) = ( 01 00 01 00 00 ) 
    
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  call       class Core/Parser`1<!0> class IDeclaration`1<!!T>::get_AsParser()
    IL_0005:  ldc.i4.0
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldloca.s   V_0
    IL_000a:  ldarg.1
    IL_000b:  callvirt   instance bool class Core/Parser`1<!!T>::Invoke(string,
                                                                        int32&,
                                                                        !0&)
    IL_0010:  brfalse.s  IL_001d

    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
    IL_0019:  bne.un.s   IL_001d

    IL_001b:  ldc.i4.1
    IL_001c:  ret

    IL_001d:  ldc.i4.0
    IL_001e:  ret
  } 
""");

Console.WriteLine(result);