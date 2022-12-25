public record MemberRef() : Decl
{
    internal static bool Parse(ref int index, string source, out MemberRef memberRef)
    {
        if(source.ConsumeWord(ref index, "field")) {
            Type.Parse(ref index, source, out Type type);
            if(TypeSpec.Parse(ref index, source, out TypeSpec typeSpec)) {
                source.ConsumeWord(ref index, "::");
                IdDecl.Parse(ref index, source, out IdDecl id);
                memberRef = new MemberRefFieldDecl(type, typeSpec, id);
                return true;
            } else {
                IdDecl.Parse(ref index, source, out IdDecl id);
                memberRef = new MemberRefFieldDecl(type, null, id);
                return true;
            }
        } else {
            if(MethodSpec.Parse(ref index, source, out MethodSpec methodSpec)) {
                CallConv.Parse(ref index, source, out CallConv callConv);
                Type.Parse(ref index, source, out Type type);
                if(TypeSpec.Parse(ref index, source, out TypeSpec typeSpec)) {
                    source.ConsumeWord(ref index, "::");
                    MethodName.Parse(ref index, source, out MethodName methodName);
                    source.ConsumeWord(ref index, "(");
                    SigArgs.Parse(ref index, source, out SigArgs sigArgs);
                    source.ConsumeWord(ref index, ")");
                    memberRef = new MemberRefMethodDecl(methodSpec, callConv, type, typeSpec, methodName, sigArgs);
                    return true;
                } else {
                    MethodName.Parse(ref index, source, out MethodName methodName);
                    source.ConsumeWord(ref index, "(");
                    SigArgs.Parse(ref index, source, out SigArgs sigArgs);
                    source.ConsumeWord(ref index, ")");
                    memberRef = new MemberRefMethodDecl(methodSpec, callConv, type, null, methodName, sigArgs);
                    return true;
                }
            }
        }
        memberRef = null;
        return false;
    }
}

public record MemberRefMethodDecl(MethodSpec MethodSpec, CallConv Conv, Type Type, TypeSpec Spec, MethodName Name, SigArgs SigArgs0) : MemberRef
{
    public override string ToString()
    {
        return $"{MethodSpec} {Conv} {Type} {(Spec is null ? String.Empty : $"{Spec}::")}{Name}({SigArgs0})";
    }
}

public record MemberRefFieldDecl(Type Type, TypeSpec Spec, IdDecl Id) : MemberRef
{
    public override string ToString()
    {
        return $"field {Type} {(Spec is null ? String.Empty : $"{Spec}::")}{Id}";
    }
}
