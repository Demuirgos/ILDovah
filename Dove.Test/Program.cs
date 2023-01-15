using Dove.Core;
var result = Parser.Parse<MethodDecl.Method>("""
.method assembly hidebysig instance !T 
          '<Trim>b__2_1'(!T[] ps) cil managed
  {
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .param [1]
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 02 00 00 00 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  ldelem     !T
    IL_0007:  ret
  } 
""");

Console.WriteLine(result);