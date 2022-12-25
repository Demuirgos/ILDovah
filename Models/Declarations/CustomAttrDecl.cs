using System.ComponentModel;

public record CustomAttrDecl(OwnerTypeDecl OwnerType, CustomTypeDecl CustomType, BYTE[] Bytes, CompQstring str) : Decl
{
    public override string ToString()
    {
        var ownerStr = OwnerType is null ? String.Empty : $"({OwnerType})";
        var BytesStr = Bytes is null ? String.Empty : $"= ({Bytes.Aggregate(String.Empty, (acc, b) => $"{acc} {b}")})";
        var compStr = str is null ? String.Empty : $"= {str}";
        return $".custom {ownerStr} {CustomType} {Bytes} {compStr}";
    }
    internal static bool Parse(ref int index, string source, out CustomAttrDecl attrDecl)
    {
        OwnerTypeDecl ownerType = null;
        if(source.ConsumeWord(ref index, ".custom")) {
            if(source.ConsumeWord(ref index, "(")) {
                OwnerTypeDecl.Parse(ref index, source, out ownerType);
                source.ConsumeWord(ref index, ")");
            }
            CustomTypeDecl.Parse(ref index, source, out CustomTypeDecl customType);
            if(source.ConsumeWord(ref index, "=")) {
                if(source.ConsumeWord(ref index, "(")) {
                    List<BYTE> bytes = new();
                    while(source.ConsumeWord(ref index, ")")) {
                        BYTE.Parse(ref index, source, out BYTE byteValue);
                        bytes.Add(byteValue);
                    }
                    attrDecl = new CustomAttrDecl(ownerType, customType, bytes.ToArray(), null);
                } else {
                    CompQstring.Parse(ref index, source, out CompQstring compQstring);
                    attrDecl = new CustomAttrDecl(ownerType, customType, null, compQstring);
                }
            } else {
                attrDecl = new CustomAttrDecl(ownerType, customType, null, null);
            }
            return true;
        } else {
            attrDecl = null;
            return false;
        }
    }
}

public record CustomTypeDecl(CallConv Conv, Type Type, TypeSpec Spec, SigArgs SigArgs) : Decl
{
    public override string ToString()
    {
        return $"{Conv} {Type} {(Spec is null ? String.Empty : $"{Spec}::")}.ctor({SigArgs})";
    }
    internal static bool Parse(ref int index, string source, out CustomTypeDecl customType)
    {
        int startIndex = index;
        CallConv.Parse(ref index, source, out CallConv callConv);
        Type.Parse(ref index, source, out Type type);
        if(source.ConsumeWord(ref index, ".ctor")) {
            if(source.ConsumeWord(ref index, "(")) {
                SigArgs.Parse(ref index, source, out SigArgs sigArgs);
                source.ConsumeWord(ref index, ")");
                customType = new CustomTypeDecl(callConv, type, null, sigArgs);
                return true;
            }
        } else {
            TypeSpec.Parse(ref index, source, out TypeSpec typeSpec);
            source.ConsumeWord(ref index, "::");
            source.ConsumeWord(ref index, ".ctor");
            if(source.ConsumeWord(ref index, "(")) {
                SigArgs.Parse(ref index, source, out SigArgs sigArgs);
                source.ConsumeWord(ref index, ")");
                customType = new CustomTypeDecl(callConv, type, typeSpec, sigArgs);
                return true;
            }
        } 
        index = startIndex;
        customType = null;
        return false;
    }
}

public record OwnerTypeDecl() : Decl
{
    internal static bool Parse(ref int index, string source, out OwnerTypeDecl ownerType)
    {
        if(TypeSpec.Parse(ref index, source, out TypeSpec typeSpec)) {
            ownerType = new OwnerTypeSpecDecl(typeSpec);
        } else if(MemberRef.Parse(ref index, source, out MemberRef memberRef)) {
            ownerType = new OwnerTypeMemberDecl(memberRef);
        } else {
            ownerType = null;
            return false;
        }
        return true;
    }
}

public record OwnerTypeSpecDecl(TypeSpec Spec) : OwnerTypeDecl {
    public override string ToString() {
        return Spec.ToString();
    }
}
public record OwnerTypeMemberDecl(MemberRef MemRef) : OwnerTypeDecl {
    public override string ToString() {
        return MemRef.ToString();
    }
}

