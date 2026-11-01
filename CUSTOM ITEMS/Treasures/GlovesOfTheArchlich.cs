using ModernUO.Serialization;

namespace Server.Items;

// The old code wrote version 1 (version 0 -> 1 only set ArmorAttributes.MageArmor, which the
// constructor already applies), so the generator version is 2 and no migration method is kept.
[SerializationGenerator(0, false)]
public partial class GlovesOfTheArchlich : BoneGloves
{
    public override int BasePhysicalResistance => 15;
    public override int BaseFireResistance => 15;
    public override int BaseColdResistance => 15;
    public override int BasePoisonResistance => 15;
    public override int BaseEnergyResistance => 15;

    public override string DefaultName => "Gloves of the Archlich";

    [Constructible]
    public GlovesOfTheArchlich()
    {
        Hue = 2702;
        Attributes.BonusStr = 5;
        Attributes.BonusInt = 5;
        Attributes.BonusHits = 5;
        Attributes.BonusMana = 8;
        Attributes.RegenHits = 3;
        Attributes.RegenMana = 3;
        Attributes.LowerManaCost = 10;
        Attributes.LowerRegCost = 20;
        ArmorAttributes.MageArmor = 1;
    }
}
