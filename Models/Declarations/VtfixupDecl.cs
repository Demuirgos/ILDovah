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
        foreach(var word in possibleValues) {
            if(source[index..].StartsWith(word)) {
                attributes.Add(word);
                index += word.Length;
            }
        }
        vtfixupAttrDecl = new VtfixupAttrDecl(attributes.ToArray());
    }
}

public record VtfixupDecl(long number, String[] Attributes, String Id) : Decl {
    public override string ToString()
        => $".vtfixup [ {number} ] {String.Join(" ", Attributes)} at {Id}";
    
    public static void Parse(ref int index, string source, out VtfixupDecl vtfixupDecl) {
        if(source[index..].StartsWith(".vtfixup")){
            index+= 8 + 1;
            INT.Parse(ref index, source, out INT number);
            index++;
            VtfixupAttrDecl.Parse(ref index, source, out VtfixupAttrDecl vtfixupAttrDecl);
            index+=2;
            ID.Parse(ref index, source, out ID id);
            vtfixupDecl = new VtfixupDecl(number.Value, vtfixupAttrDecl.Attributes, id.Value);
        } 
        else
        { 
            vtfixupDecl = null;
        }
    }
}
