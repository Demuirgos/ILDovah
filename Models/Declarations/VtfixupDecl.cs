public record VtfixupAttrDecl(String[] Attributes) : Decl {
    public override string ToString()
        => $"[ {String.Join(" ", Attributes)} ]";
    /*
    vtfixupAttr : EMPTY 
                | vtfixupAttr 'int32' 
                | vtfixupAttr 'int64' 
                | vtfixupAttr 'fromunmanaged' 
                | vtfixupAttr 'callmostderived'
    */
    public static void Parse(ref int index, string source, out VtfixupAttrDecl vtfixupAttrDecl) {
        string[] possibleValues = new string[] { "int32", "int64", "fromunmanaged", "callmostderived" };
        List<string> attributes = new List<string>();

        while(source.StartsWith(possibleValues, out string value)) {
            attributes.Add(value);
            index += value.Length;
        }
        vtfixupAttrDecl = new VtfixupAttrDecl(attributes.ToArray());
    }
}

public record VtfixupDecl(long number, String[] Attributes, String Id) : Decl {
    public override string ToString()
        => $".vtfixup [ {number} ] {String.Join(" ", Attributes)} at {Id}";
    
    public static void Parse(ref int index, string source, out VtfixupDecl vtfixupDecl) {
        if(source.ConsumeWord(ref index, ".vtfixup")){
            source.ConsumeWord(ref index, "[");
            INT.Parse(ref index, source, out INT number);
            source.ConsumeWord(ref index, "]");
            VtfixupAttrDecl.Parse(ref index, source, out VtfixupAttrDecl vtfixupAttrDecl);
            source.ConsumeWord(ref index, "at");
            ID.Parse(ref index, source, out ID id);
            vtfixupDecl = new VtfixupDecl(number.Value, vtfixupAttrDecl.Attributes, id.Value);
        } 
        else
        { 
            vtfixupDecl = null;
        }
    }
}
