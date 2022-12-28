using static Core;

public record DOTTEDNAME(String Value) : IDeclaration<DOTTEDNAME> {
    public override string ToString() => Value;
    public static Parser<DOTTEDNAME> AsParser => RunAll(
        converter: (vals) => new DOTTEDNAME(String.Join('.', vals)),

        Map(Identifier.AsParser, (Identifier id) => id.ToString()),
        RunMany(0, Int32.MaxValue, RunAll(
                converter: (vals) => vals[1],

                ConsumeChar('.', (_) => String.Empty),
                Map(Identifier.AsParser, (Identifier id) => id.ToString())
            ), 
            converter: (vals) => String.Join('.', vals))
    );
    public static bool Parse(ref int index, string source, out DOTTEDNAME idVal) {
        if(DOTTEDNAME.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
}

public record SLASHEDNAME(String Value) : IDeclaration<SLASHEDNAME> {
    public override string ToString() => Value;
    public static Parser<SLASHEDNAME> AsParser => RunAll(
        converter: (vals) => new SLASHEDNAME(String.Join('/', vals)),

        Map(Identifier.AsParser, (Identifier id) => id.ToString()),
        RunMany(0, Int32.MaxValue, RunAll(
                converter: (vals) => vals[1],

                ConsumeChar('/', (_) => String.Empty),
                Map(Identifier.AsParser, (Identifier id) => id.ToString())
            ), 
            converter: (vals) => String.Join('/', vals))
    );
    public static bool Parse(ref int index, string source, out SLASHEDNAME idVal) {
        if(SLASHEDNAME.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
}

public record Identifier(string Value) : IDeclaration<Identifier> {
    public override string ToString() => Value;
    public static Parser<Identifier> AsParser => TryRun(
        converter: (vals) => new Identifier(vals),
        Map(ID.AsParser, (ID id) => id.ToString()), 
        Map(QSTRING.AsParser, (QSTRING qstring) => qstring.ToString())
    );

    public static bool Parse(ref int index, string source, out Identifier idVal) {
        if(Identifier.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
}


public record ID(String Value) : IDeclaration<ID> {
    public override string ToString() => Value;
    public static Parser<ID> AsParser => RunAll(
        converter: (vals) => new ID(vals[0]),
        RunMany(1, Int32.MaxValue, ConsumeIf(c => Char.IsLetterOrDigit(c) || c == '_', Id), chars => new string(chars.ToArray()))
    );
    public static bool Parse(ref int index, string source, out ID idVal) {
        if(ID.AsParser(source, ref index, out idVal)) {
            return true;
        }
        idVal = null;
        return false;
    }
}