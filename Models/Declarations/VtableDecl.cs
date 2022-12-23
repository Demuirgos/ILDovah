public record VtableDecl(byte[] values) : Decl {
    public override string ToString()
        => $".vtable = ({ String.Join(" ", values) })";
    
    public static void Parse(ref int index, string source, out VtableDecl vtableDecl) {
        if(source.ConsumeWord(ref index, ".vtable")) {
            source.ConsumeWord(ref index, "=");
            source.ConsumeWord(ref index, "(");
            List<byte> bytes = new();
            while(!source.ConsumeWord(ref index, ")")) {
                BYTE.Parse(ref index, source, out BYTE byteValue);
                bytes.Add(byteValue.Value);
            }
            vtableDecl = new VtableDecl(bytes.ToArray());
        } else {
            vtableDecl = null;
        }
    }
}