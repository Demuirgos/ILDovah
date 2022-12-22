public record FieldDecl(RepeatOpt RepeatOpt, FieldAttr attributes, Type type, IdDecl Id, AtOpt atOpt, InitOpt Init) : Decl {
    public override string ToString()
        => $"{RepeatOpt} {attributes} {type} {Id} {atOpt} {Init}";

}

internal record AtOpt(IdDecl? Id) : Decl
{
    public override string ToString()
        => Id is null ? String.Empty : $"at {Id}";

    public static void Parse(ref int index, string source, out AtOpt atOpt)
    {   
        if(source[index..].StartsWith("at")) {
            index += 10 + 1;
            IdDecl.Parse(ref index, source, out IdDecl idDecl);
            atOpt = new AtOpt(idDecl);
        } else {
            atOpt = new AtOpt(null);
        }
    }
}

internal record RepeatOpt(long? index) : Decl
{
    public override string ToString()
        => index is null ? String.Empty : $"[{index}]";

    public static void Parse(ref int index, string source, out RepeatOpt repeatOpt)
    {   
        if(source[index] == '[') {
            index += 10 + 1;
            INT.Parse(ref index, source, out INT indexDecl);
            repeatOpt = new RepeatOpt(indexDecl.Value);
            index++;
        } else {
            repeatOpt = new RepeatOpt(null);
        }
    }
}

internal class InitOpt
{
}

internal record FieldAttr(string[] Attributes, String? MarshalType)
{
    public override string ToString()
        => String.Join(" ", Attributes) + (MarshalType is null ? String.Empty : $"marshal ({MarshalType})");
        
    public static void Parse(ref int index, string source, out FieldAttr fieldAttr) {
        string[] possibleValues = new string[] { "static", "public", "private", "family", "initonly", "rtspecialname", "specialname", "pinvokeimpl", "marshal", "assembly", "famandassem", "famorassem", "privatescope", "literal", "notserialized"};
        
        bool StartsWithAttribute(string code, out String attribute) {
            var result = possibleValues.Any(x => code.StartsWith(x));
            if(result) {
                attribute = possibleValues.First(x => code.StartsWith(x));
            } else {
                attribute = null;
            }
            return result;
        } 

        String? marshalType = null;        
        List<string> attributes = new List<string>();
        while(StartsWithAttribute(source, out string word)) {
            if(word == "pinvokeimpl") {
                // commented out because PInvoke for fields is not supported by EE
            } else {
                if(word == "marshal") {
                    NativeType.Parse(ref index, source, out marshalType);
                }
                attributes.Add(word);
                index += word.Length + 1;
            }
        }
        
        fieldAttr = new FieldAttr(attributes.ToArray(), marshalType);
    }
}