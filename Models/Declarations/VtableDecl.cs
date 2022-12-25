public record VtableDecl(byte[] values) : Decl {
    public override string ToString()
        => $".vtable = ({ String.Join(" ", values) })";
    
    public static bool Parse(ref int index, string source, out VtableDecl vtableDecl) {
        if(source.ConsumeWord(ref index, ".vtable")) {
            source.ConsumeWord(ref index, "=");
            source.ConsumeWord(ref index, "(");
            List<byte> bytes = new();
            while(!source.ConsumeWord(ref index, ")")) {
                BYTE.Parse(ref index, source, out BYTE byteValue);
                bytes.Add(byteValue.Value);
            }
            vtableDecl = new VtableDecl(bytes.ToArray());
            return true;
        } else {
            vtableDecl = null;
            return false;
        }
    }
}