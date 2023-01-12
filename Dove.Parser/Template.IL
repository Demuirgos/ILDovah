.assembly _
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = (
        01 00 08 00 00 00 00 00
    )
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = (
        01 00 01 00 54 02 16 57 72 61 70 4e 6f 6e 45 78
        63 65 70 74 69 6f 6e 54 68 72 6f 77 73 01
    )
    .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = (
        01 00 02 00 00 00 00 00
    )
    .permissionset reqmin = (
        2e 01 80 8a 53 79 73 74 65 6d 2e 53 65 63 75 72
        69 74 79 2e 50 65 72 6d 69 73 73 69 6f 6e 73 2e
        53 65 63 75 72 69 74 79 50 65 72 6d 69 73 73 69
        6f 6e 41 74 74 72 69 62 75 74 65 2c 20 53 79 73
        74 65 6d 2e 52 75 6e 74 69 6d 65 2c 20 56 65 72
        73 69 6f 6e 3d 37 2e 30 2e 30 2e 30 2c 20 43 75
        6c 74 75 72 65 3d 6e 65 75 74 72 61 6c 2c 20 50
        75 62 6c 69 63 4b 65 79 54 6f 6b 65 6e 3d 62 30
        33 66 35 66 37 66 31 31 64 35 30 61 33 61 15 01
        54 02 10 53 6b 69 70 56 65 72 69 66 69 63 61 74
        69 6f 6e 01
    )
    .hash algorithm 0x00008004 
    .ver 0:0:0:0
}

.class private auto ansi '<Module>'
{
} 

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
            [1] valuetype [System.Runtime]System.Nullable`1<int32> SafeBlockHash,
            [2] valuetype [System.Runtime]System.Nullable`1<int32> FinalizedBlockHash,
            [3] valuetype [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler,
            [4] int32
        )

        .try
        {
            .try
            {
                IL_0000: ldloca.s 0
                IL_0002: initobj valuetype [System.Runtime]System.Nullable`1<int32>
                IL_0008: ldloca.s 1
                IL_000a: ldc.i4.s 42
                IL_000c: call instance void valuetype [System.Runtime]System.Nullable`1<int32>::.ctor(!0)
                IL_0011: ldloca.s 2
                IL_0013: ldc.i4.s 23
                IL_0015: call instance void valuetype [System.Runtime]System.Nullable`1<int32>::.ctor(!0)
                IL_001a: ldc.i4.s 29
                IL_001c: ldc.i4.6
                IL_001d: newobj instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::.ctor(int32, int32)
                IL_0022: stloc.3
                IL_0023: ldloca.s 3
                IL_0025: ldstr "ForkchoiceState: {"
                IL_002a: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
                IL_002f: ldloca.s 3
                IL_0031: ldstr "HeadBlockHash"
                IL_0036: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted(string)
                IL_003b: ldloca.s 3
                IL_003d: ldstr ": "
                IL_0042: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
                IL_0047: ldloca.s 3
                IL_0049: ldloc.0
                IL_004a: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted<valuetype [System.Runtime]System.Nullable`1<int32>>(!!0)
                IL_004f: ldloca.s 3
                IL_0051: ldstr ", "
                IL_0056: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
                IL_005b: ldloca.s 3
                IL_005d: ldstr "SafeBlockHash"
                IL_0062: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted(string)
                IL_0067: ldloca.s 3
                IL_0069: ldstr ": "
                IL_006e: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
                IL_0073: ldloca.s 3
                IL_0075: ldloc.1
                IL_0076: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted<valuetype [System.Runtime]System.Nullable`1<int32>>(!!0)
                IL_007b: ldloca.s 3
                IL_007d: ldstr ", "
                IL_0082: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
                IL_0087: ldloca.s 3
                IL_0089: ldstr "FinalizedBlockHash"
                IL_008e: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted(string)
                IL_0093: ldloca.s 3
                IL_0095: ldstr ": "
                IL_009a: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
                IL_009f: ldloca.s 3
                IL_00a1: ldloc.2
                IL_00a2: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted<valuetype [System.Runtime]System.Nullable`1<int32>>(!!0)
                IL_00a7: ldloca.s 3
                IL_00a9: ldstr "}"
                IL_00ae: call instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
                IL_00b3: ldloca.s 3
                IL_00b5: call instance string [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::ToStringAndClear()
                IL_00ba: call void [System.Console]System.Console::WriteLine(string)
                IL_00bf: ldc.i4.0
                IL_00c0: stloc.s 4
                IL_00c2: leave.s IL_00d5
            } 
            catch [System.Runtime]System.Object
            {
                IL_00c4: pop
                IL_00c5: ldc.i4.m1
                IL_00c6: stloc.s 4
                IL_00c8: leave.s IL_00d5
            } 
        } 
        finally
        {
            IL_00ca: ldstr "done"
            IL_00cf: call void [System.Console]System.Console::WriteLine(string)
            IL_00d4: endfinally
        } 

        
        IL_00d5: ldloc.s 4
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

.class private auto ansi sealed beforefieldinit Microsoft.CodeAnalysis.EmbeddedAttribute
    extends [System.Runtime]System.Attribute
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )
    .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = (
        01 00 00 00
    )
    
    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        
        
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Attribute::.ctor()
        IL_0006: ret
    } 

} 

.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.RefSafetyRulesAttribute
    extends [System.Runtime]System.Attribute
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )
    .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = (
        01 00 00 00
    )
    .custom instance void [System.Runtime]System.AttributeUsageAttribute::.ctor(valuetype [System.Runtime]System.AttributeTargets) = (
        01 00 02 00 00 00 02 00 54 02 0d 41 6c 6c 6f 77
        4d 75 6c 74 69 70 6c 65 00 54 02 09 49 6e 68 65
        72 69 74 65 64 00
    )
    
    .field public initonly int32 Version

    
    .method public hidebysig specialname rtspecialname 
        instance void .ctor (
            int32 ''
        ) cil managed 
    {
        
        
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Attribute::.ctor()
        IL_0006: ldarg.0
        IL_0007: ldarg.1
        IL_0008: stfld int32 System.Runtime.CompilerServices.RefSafetyRulesAttribute::Version
        IL_000d: ret
    } 

} 

