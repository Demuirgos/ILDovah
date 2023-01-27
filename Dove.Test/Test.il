.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                           .ver 7:0:0:0
}
.assembly extern Inoculator.Injector
{
  .ver 1:0:0:0
}
.assembly extern System.Console
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                           .ver 7:0:0:0
}
.assembly Inoculated.Test
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78                                                                                                                      63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       
    
  .custom instance void [System.Runtime]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 18 2E 4E 45 54 43 6F 72 65 41 70 70 2C 56                                                                                                                 65 72 73 69 6F 6E 3D 76 37 2E 30 01 00 54 0E 14                                                                                                                 46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61 79                                                                                                                 4E 61 6D 65 08 2E 4E 45 54 20 37 2E 30 )            .custom instance void [System.Runtime]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 0F 49 6E 6F 63 75 6C 61 74 65 64 2E 54 65                                                                                                         73 74 00 00 )                                       .custom instance void [System.Runtime]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 05 44 65 62 75 67 00 00 )                     .custom instance void [System.Runtime]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )               .custom instance void [System.Runtime]System.Reflection.AssemblyInformationalVersionAttribute::.ctor(string) = ( 01 00 05 31 2E 30 2E 30 00 00 )                     .custom instance void [System.Runtime]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 0F 49 6E 6F 63 75 6C 61 74 65 64 2E 54 65                                                                                                         73 74 00 00 )                                       .custom instance void [System.Runtime]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 0F 49 6E 6F 63 75 6C 61 74 65 64 2E 54 65                                                                                                       73 74 00 00 )                                       .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module Inoculated.Test.dll
.custom instance void System.Runtime.CompilerServices.RefSafetyRulesAttribute::.ctor(int32) = ( 01 00 0B 00 00 00 00 00 ) 
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       .corflags 0x00000001    


.class private auto ansi sealed beforefieldinit Microsoft.CodeAnalysis.EmbeddedAttribute
       extends [System.Runtime]System.Attribute
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
        .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } 
} 
.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.NullableAttribute
       extends [System.Runtime]System.Attribute
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.AttributeUsageAttribute::.ctor(valuetype [System.Runtime]System.AttributeTargets) = ( 01 00 84 6B 00 00 02 00 54 02 0D 41 6C 6C 6F 77                                                                                                                                        4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65                                                                                                                                        72 69 74 65 64 00 )                                 .field public initonly uint8[] NullableFlags
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(uint8 A_1) cil managed
  {
        .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.1
    IL_0009:  newarr     [System.Runtime]System.Byte
    IL_000e:  dup
    IL_000f:  ldc.i4.0
    IL_0010:  ldarg.1
    IL_0011:  stelem.i1
    IL_0012:  stfld      uint8[] System.Runtime.CompilerServices.NullableAttribute::NullableFlags
    IL_0017:  ret
  } 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(uint8[] A_1) cil managed
  {
        .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  stfld      uint8[] System.Runtime.CompilerServices.NullableAttribute::NullableFlags
    IL_000e:  ret
  } 
} 
.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.NullableContextAttribute
       extends [System.Runtime]System.Attribute
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.AttributeUsageAttribute::.ctor(valuetype [System.Runtime]System.AttributeTargets) = ( 01 00 4C 14 00 00 02 00 54 02 0D 41 6C 6C 6F 77                                                                                                                                        4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65                                                                                                                                        72 69 74 65 64 00 )                                 .field public initonly uint8 Flag
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(uint8 A_1) cil managed
  {
        .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  stfld      uint8 System.Runtime.CompilerServices.NullableContextAttribute::Flag
    IL_000e:  ret
  } 
} 
.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.RefSafetyRulesAttribute
       extends [System.Runtime]System.Attribute
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.AttributeUsageAttribute::.ctor(valuetype [System.Runtime]System.AttributeTargets) = ( 01 00 02 00 00 00 02 00 54 02 0D 41 6C 6C 6F 77                                                                                                                                        4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65                                                                                                                                        72 69 74 65 64 00 )                                 .field public initonly int32 Version
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_1) cil managed
  {
        .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  stfld      int32 System.Runtime.CompilerServices.RefSafetyRulesAttribute::Version
    IL_000e:  ret
  } 
} 
.class public auto ansi beforefieldinit LogEntrencyAttribute
       extends [Inoculator.Injector]Inoculator.Attributes.InterceptorAttribute
{
  .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
  .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
  .method public hidebysig virtual instance void 
          OnEntry(class [Inoculator.Injector]Inoculator.Builder.MethodData 'method') cil managed
  {
        .maxstack  3
    .locals init (valuetype [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  ldc.i4.8
    IL_0004:  ldc.i4.1
    IL_0005:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::.ctor(int32,
                                                                                                                               int32)
    IL_000a:  ldloca.s   V_0
    IL_000c:  ldstr      "Before: "
    IL_0011:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
    IL_0016:  nop
    IL_0017:  ldloca.s   V_0
    IL_0019:  ldarg.1
    IL_001a:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted<class [Inoculator.Injector]Inoculator.Builder.MethodData>(!!0)
    IL_001f:  nop
    IL_0020:  ldloca.s   V_0
    IL_0022:  call       instance string [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::ToStringAndClear()
    IL_0027:  call       void [System.Console]System.Console::WriteLine(string)
    IL_002c:  nop
    IL_002d:  ret
  } 
  .method public hidebysig virtual instance void 
          OnExit(class [Inoculator.Injector]Inoculator.Builder.MethodData 'method') cil managed
  {
        .maxstack  3
    .locals init (valuetype [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  ldc.i4.7
    IL_0004:  ldc.i4.1
    IL_0005:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::.ctor(int32,
                                                                                                                               int32)
    IL_000a:  ldloca.s   V_0
    IL_000c:  ldstr      "After: "
    IL_0011:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
    IL_0016:  nop
    IL_0017:  ldloca.s   V_0
    IL_0019:  ldarg.1
    IL_001a:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted<class [Inoculator.Injector]Inoculator.Builder.MethodData>(!!0)
    IL_001f:  nop
    IL_0020:  ldloca.s   V_0
    IL_0022:  call       instance string [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::ToStringAndClear()
    IL_0027:  call       void [System.Console]System.Console::WriteLine(string)
    IL_002c:  nop
    IL_002d:  ret
  } 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
        .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [Inoculator.Injector]Inoculator.Attributes.InterceptorAttribute::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } 
} 
.class public auto ansi beforefieldinit ElapsedTimeAttribute
       extends [Inoculator.Injector]Inoculator.Attributes.InterceptorAttribute
{
  .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
  .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
  .method public hidebysig virtual instance void 
          OnEntry(class [Inoculator.Injector]Inoculator.Builder.MethodData 'method') cil managed
  {
        .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       class [System.Runtime]System.Diagnostics.Stopwatch [System.Runtime]System.Diagnostics.Stopwatch::StartNew()
    IL_0007:  callvirt   instance void [Inoculator.Injector]Inoculator.Builder.MethodData::set_EmbededResource(object)
    IL_000c:  nop
    IL_000d:  ret
  } 
  .method public hidebysig virtual instance void 
          OnExit(class [Inoculator.Injector]Inoculator.Builder.MethodData 'method') cil managed
  {
        .maxstack  3
    .locals init (class [System.Runtime]System.Diagnostics.Stopwatch V_0,
             valuetype [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance object [Inoculator.Injector]Inoculator.Builder.MethodData::get_EmbededResource()
    IL_0007:  castclass  [System.Runtime]System.Diagnostics.Stopwatch
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  callvirt   instance void [System.Runtime]System.Diagnostics.Stopwatch::Stop()
    IL_0013:  nop
    IL_0014:  ldloca.s   V_1
    IL_0016:  ldc.i4.s   15
    IL_0018:  ldc.i4.2
    IL_0019:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::.ctor(int32,
                                                                                                                               int32)
    IL_001e:  ldloca.s   V_1
    IL_0020:  ldstr      "Method "
    IL_0025:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
    IL_002a:  nop
    IL_002b:  ldloca.s   V_1
    IL_002d:  ldarg.1
    IL_002e:  ldftn      instance string [Inoculator.Injector]Inoculator.Builder.MethodData::Name(bool)
    IL_0034:  newobj     instance void class [System.Runtime]System.Func`2<bool,string>::.ctor(object,
                                                                                               native int)
    IL_0039:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted<class [System.Runtime]System.Func`2<bool,string>>(!!0)
    IL_003e:  nop
    IL_003f:  ldloca.s   V_1
    IL_0041:  ldstr      " took "
    IL_0046:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
    IL_004b:  nop
    IL_004c:  ldloca.s   V_1
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance int64 [System.Runtime]System.Diagnostics.Stopwatch::get_ElapsedMilliseconds()
    IL_0054:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted<int64>(!!0)
    IL_0059:  nop
    IL_005a:  ldloca.s   V_1
    IL_005c:  ldstr      "ms"
    IL_0061:  call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
    IL_0066:  nop
    IL_0067:  ldloca.s   V_1
    IL_0069:  call       instance string [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::ToStringAndClear()
    IL_006e:  call       void [System.Console]System.Console::WriteLine(string)
    IL_0073:  nop
    IL_0074:  ret
  } 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
        .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [Inoculator.Injector]Inoculator.Attributes.InterceptorAttribute::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } 
} 
.class private sequential ansi sealed beforefieldinit Program.Program
       extends [System.Runtime]System.ValueType
{
  .pack 0
  .size 1
  .class auto ansi sealed nested private beforefieldinit '<Main>d__0'
         extends [System.Runtime]System.Object
         implements [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field public string[] args
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 02 00 00 00 00 01 00 00 ) 
    .field private int32 '<r>5__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
            .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } 
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
            .maxstack  3
      .locals init (int32 V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Program.Program/'<Main>d__0'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  nop
        IL_0008:  ldarg.0
        IL_0009:  ldc.i4.s   23
        IL_000b:  ldc.i4.0
        IL_000c:  call       int32 class Program.Gen`1<int32>::TestS(int32,
                                                                     int32)
        IL_0011:  stfld      int32 Program.Program/'<Main>d__0'::'<r>5__1'
        IL_0016:  ldarg.0
        IL_0017:  ldfld      int32 Program.Program/'<Main>d__0'::'<r>5__1'
        IL_001c:  call       void [System.Console]System.Console::WriteLine(int32)
        IL_0021:  nop
        IL_0022:  leave.s    IL_003c

      }        catch [System.Runtime]System.Exception 
      {
        IL_0024:  stloc.1
        IL_0025:  ldarg.0
        IL_0026:  ldc.i4.s   -2
        IL_0028:  stfld      int32 Program.Program/'<Main>d__0'::'<>1__state'
        IL_002d:  ldarg.0
        IL_002e:  ldflda     valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program.Program/'<Main>d__0'::'<>t__builder'
        IL_0033:  ldloc.1
        IL_0034:  call       instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [System.Runtime]System.Exception)
        IL_0039:  nop
        IL_003a:  leave.s    IL_0050

      }        IL_003c:  ldarg.0
      IL_003d:  ldc.i4.s   -2
      IL_003f:  stfld      int32 Program.Program/'<Main>d__0'::'<>1__state'
      IL_0044:  ldarg.0
      IL_0045:  ldflda     valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program.Program/'<Main>d__0'::'<>t__builder'
      IL_004a:  call       instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_004f:  nop
      IL_0050:  ret
    } 
    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .param [1]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
      .override [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
            .maxstack  8
      IL_0000:  ret
    } 
  } 
  .method private hidebysig static class [System.Runtime]System.Threading.Tasks.Task 
          Main(string[] args) cil managed
  {
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [System.Runtime]System.Type) = ( 01 00 1A 50 72 6F 67 72 61 6D 2E 50 72 6F 67 72                                                                                                                                                      61 6D 2B 3C 4D 61 69 6E 3E 64 5F 5F 30 00 00 )        .custom instance void [System.Runtime]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
        .maxstack  2
    .locals init (class Program.Program/'<Main>d__0' V_0)
    IL_0000:  newobj     instance void Program.Program/'<Main>d__0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  call       valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_000c:  stfld      valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program.Program/'<Main>d__0'::'<>t__builder'
    IL_0011:  ldloc.0
    IL_0012:  ldarg.0
    IL_0013:  stfld      string[] Program.Program/'<Main>d__0'::args
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 Program.Program/'<Main>d__0'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldflda     valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program.Program/'<Main>d__0'::'<>t__builder'
    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<class Program.Program/'<Main>d__0'>(!!0&)
    IL_002c:  ldloc.0
    IL_002d:  ldflda     valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program.Program/'<Main>d__0'::'<>t__builder'
    IL_0032:  call       instance class [System.Runtime]System.Threading.Tasks.Task [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_0037:  ret
  } 
  .method private hidebysig specialname static 
          void  '<Main>'(string[] args) cil managed
  {
    .entrypoint
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    .param [1]
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
        .maxstack  1
    .locals init (valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       class [System.Runtime]System.Threading.Tasks.Task Program.Program::Main(string[])
    IL_0006:  callvirt   instance valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter [System.Runtime]System.Threading.Tasks.Task::GetAwaiter()
    IL_000b:  stloc.0
    IL_000c:  ldloca.s   V_0
    IL_000e:  call       instance void [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
    IL_0013:  ret
  } 
} 
.class private auto ansi beforefieldinit Program.Gen`1<T>
       extends [System.Runtime]System.Object
{
  .param type T 
  .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
  .method public hidebysig static int32  TestS(int32 k,
                                               int32 m) cil managed
  {
    .custom instance void ElapsedTimeAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void LogEntrencyAttribute::.ctor() = ( 01 00 00 00 ) 
        .maxstack  3
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0013

    IL_0007:  nop
    IL_0008:  ldloc.0
    IL_0009:  ldarg.1
    IL_000a:  ldloc.1
    IL_000b:  add
    IL_000c:  add
    IL_000d:  stloc.0
    IL_000e:  nop
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.1
    IL_0011:  add
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  ldarg.0
    IL_0015:  clt
    IL_0017:  stloc.2
    IL_0018:  ldloc.2
    IL_0019:  brtrue.s   IL_0007

    IL_001b:  ldloc.0
    IL_001c:  stloc.3
    IL_001d:  br.s       IL_001f

    IL_001f:  ldloc.3
    IL_0020:  ret
  } 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
        .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } 
} 


