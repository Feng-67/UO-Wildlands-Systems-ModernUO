using ModernUO.Serialization;

namespace Server.Items;

[Flippable(0x1541, 0x1542)]
[SerializationGenerator(0, false)]
public partial class WarlordSash : BodySash
{
    public override int LabelNumber => 1161980; // Warlord Sash
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    [Constructible]
    public WarlordSash()
    {
        Hue = 2500;

        // Random primary stat
        switch (Utility.Random(3))
        {
            case 0:
                Attributes.BonusStr = 4;
                break;
            case 1:
                Attributes.BonusDex = 4;
                break;
            case 2:
                Attributes.BonusInt = 4;
                break;
        }
                
    }
}
