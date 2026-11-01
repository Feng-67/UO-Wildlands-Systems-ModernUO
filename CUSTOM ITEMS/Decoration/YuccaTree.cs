using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class YuccaTree : Item
{
    [Constructible]
    public YuccaTree() : base(0x0D37)
    {
    }

    public override double DefaultWeight => 10.0;

    public override int LabelNumber => 1023383; // yucca
}
