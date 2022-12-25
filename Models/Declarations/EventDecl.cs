public record EventDecl(EventAttr eventAttrs, TypeSpec TypeSpec, IdDecl Id, EventSubDecl[] EventDecls) : Decl {
    public override string ToString()
        => $".event {eventAttrs} {TypeSpec} {Id} {EventDecls.Aggregate(String.Empty, (acc, decl) => acc + decl)}";

    public static bool Parse(ref int index, string source, out EventDecl eventDecl) {
        source.ConsumeWord(ref index, ".event");
        EventAttr.Parse(ref index, source, out EventAttr eventAttr);
        TypeSpec.Parse(ref index, source, out TypeSpec typeSpec);
        IdDecl.Parse(ref index, source, out IdDecl id);
        List<EventSubDecl> eventDecls = new List<EventSubDecl>();
        
        while(EventSubDecl.Parse(ref index, source, out EventSubDecl eventSubDecl)) {
            eventDecls.Add(eventSubDecl);
        }
        eventDecl = new EventDecl(eventAttr, typeSpec, id, eventDecls.ToArray());
        return true;
    }

}

public record EventSubDecl() : Decl {

    internal static bool Parse(ref int index, string source, out EventSubDecl eventSubDecl)
    {
        string[] PossibleValues = { ".addon", ".removeon", ".fire", ".other" };
        if(source[index..].StartsWith(PossibleValues, out string word)) {
            index += word.Length;
            CallConv.Parse(ref index, source, out CallConv callConv);
            Type.Parse(ref index, source, out Type type);
            if(TypeSpec.Parse(ref index, source, out TypeSpec typeSpec)) {
                source.ConsumeWord(ref index, "::");
            }
            MethodName.Parse(ref index, source, out MethodName methodName);
            source.ConsumeWord(ref index, "(");
            SigArgs.Parse(ref index, source, out SigArgs sigArgs);
            source.ConsumeWord(ref index, ")");
            switch(word) {
                case ".addon":
                    eventSubDecl = new EventSubAddonDecl(callConv, type, typeSpec, methodName, sigArgs);
                    return true;
                case ".removeon":
                    eventSubDecl = new EventSubRemoveonDecl(callConv, type, typeSpec, methodName, sigArgs);
                    return true;
                case ".fire":
                    eventSubDecl = new EventSubFireDecl(callConv, type, typeSpec, methodName, sigArgs);
                    return true;
                case ".other":
                    eventSubDecl = new EventSubOtherDecl(callConv, type, typeSpec, methodName, sigArgs);
                    return true;
                default:
                    eventSubDecl = null;
                    return false;
            }
        } else {
            if(CustomAttrDecl.Parse(ref index, source, out CustomAttrDecl attrDecl)) {
                eventSubDecl = new EventSubAttributeDecl(attrDecl);
                return true;
            } else if(LanguageDecl.Parse(ref index, source, out LanguageDecl languageDecl)) {
                eventSubDecl = new EventSubLanguageDecl(languageDecl);
                return true;
            } else if(ExtSourceSpecDecl.Parse(ref index, source, out ExtSourceSpecDecl extSourceSpec)) {
                eventSubDecl = new EventSubSourceDecl(extSourceSpec);
                return true;
            } else {
                eventSubDecl = null;
                return false;
            }

        }
    }
}

public record EventSubAddonDecl(CallConv callConv, Type type, TypeSpec TypeSpec, MethodName methodName, SigArgs sigArgs) : EventSubDecl;
public record EventSubRemoveonDecl(CallConv callConv, Type type, TypeSpec TypeSpec, MethodName methodName, SigArgs sigArgs) : EventSubDecl;
public record EventSubFireDecl(CallConv callConv, Type type, TypeSpec TypeSpec, MethodName methodName, SigArgs sigArgs) : EventSubDecl;
public record EventSubOtherDecl(CallConv callConv, Type type, TypeSpec TypeSpec, MethodName methodName, SigArgs sigArgs) : EventSubDecl;
public record EventSubAttributeDecl(CustomAttrDecl AttrDecl) : EventSubDecl;
public record EventSubLanguageDecl(LanguageDecl LanguageDecl) : EventSubDecl;
public record EventSubSourceDecl(ExtSourceSpecDecl ExtSourceSpec) : EventSubDecl;


public record EventAttr(string[] Attributes)
{
    public override string ToString()
        => String.Join(" ", Attributes);
        
    public static void Parse(ref int index, string source, out EventAttr eventAttr) {
        string[] possibleValues = { "rtspecialname", "specialname"};
        List<string> attributes = new List<string>();
        while(source[index..].StartsWith(possibleValues, out string word)) {
            attributes.Add(word);
            index += word.Length;
        }
        
        eventAttr = new EventAttr(attributes.ToArray());
    }
}