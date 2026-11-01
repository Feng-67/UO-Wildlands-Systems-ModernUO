using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class SentinelsMempo : PlateMempo
{
    public override int LabelNumber => 1161992; // Sentinel's Mempo
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    public override int BasePhysicalResistance => 15;
    public override int BaseFireResistance => 15;
    public override int BaseColdResistance => 15;
    public override int BasePoisonResistance => 15;
    public override int BaseEnergyResistance => 15;

    [Constructible]
    public SentinelsMempo()
    {
        Hue = 2764;
        Attributes.BonusStr = 4;
        Attributes.BonusDex = 4;
        Attributes.BonusHits = 8;
        Attributes.BonusStam = 12;
        Attributes.BonusMana = 8;
        Attributes.AttackChance = 5;
        Attributes.DefendChance = 5;
        Attributes.LowerManaCost = 8;
    }
}
