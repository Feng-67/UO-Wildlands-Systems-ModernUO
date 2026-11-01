using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class CorruptedPaladinVambraces : PlateArms
{
    public override int BasePhysicalResistance => 15;
    public override int BaseFireResistance => 15;
    public override int BaseColdResistance => 15;
    public override int BasePoisonResistance => 15;
    public override int BaseEnergyResistance => 15;

    public override string DefaultName => "Corrupted Paladin Vambraces";

    [Constructible]
    public CorruptedPaladinVambraces()
    {
        Hue = 2702;

        Attributes.BonusStr = 5;
        Attributes.BonusDex = 5;
        Attributes.BonusStam = 10;
        Attributes.BonusMana = 10;
        Attributes.RegenHits = 4;
        Attributes.RegenStam = 4;
        Attributes.RegenMana = 4;
        Attributes.LowerManaCost = 8;
    }
}