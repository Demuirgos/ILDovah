public record LanguageDecl() : Decl {
    public QSTRING[] Language { get; init; }

    public override string ToString() {
        return $".language {Language.Aggregate(String.Empty, (acc, s) => $"{acc}, s.Value")}";
    }

    public static void Parse(ref int index, string source, out LanguageDecl languageDecl) {
        if(source[index..].StartsWith(".language")) {
            index += 9;
            QSTRING.Parse(ref index, source, true, out QSTRING langString);
            List<QSTRING> langList = new() { langString };
            while(source[index] == ',') {
                index++;
                QSTRING.Parse(ref index, source, true, out QSTRING langString2);
                langList.Add(langString2);
            }
            languageDecl = new LanguageDecl() { Language = langList.ToArray() };
            return;
        } 
        languageDecl = null;
    }
}