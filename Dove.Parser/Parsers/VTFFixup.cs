using static Core;
using static Extensions;

public record VTFFixup(INT Index, VTFixupAttribute.Collection Attributes, DataLabel Label) : Declaration, IDeclaration<VTFFixup> {
    public override string ToString() => $".vtfixup {Index} {Attributes} at {Label}";
    public static Parser<VTFFixup> AsParser => RunAll(
        converter: parts => new VTFFixup(parts[1].Index, parts[2].Attributes, parts[4].Label),
        Discard<VTFFixup, string>(ConsumeWord(Id, ".vtfixup")),
        TryRun(
            converter: index => Construct<VTFFixup>(3, 0, index), 
            INT.AsParser, Empty<INT>()
        ),
        Map(
            converter: attributes => Construct<VTFFixup>(3, 1, attributes), 
            VTFixupAttribute.Collection.AsParser
        ),
        Discard<VTFFixup, string>(ConsumeWord(Id, "at")),
        Map(
            converter: label => Construct<VTFFixup>(3, 2, label), 
            DataLabel.AsParser
        )
    );
}