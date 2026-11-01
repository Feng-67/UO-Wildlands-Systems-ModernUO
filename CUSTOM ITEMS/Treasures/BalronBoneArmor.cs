using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class BalronBoneArmor : BoneChest
{
    public override int BasePhysicalResistance => 15;
    public override int BaseFireResistance => 15;
    public override int BaseColdResistance => 15;
    public override int BasePoisonResistance => 15;
    public override int BaseEnergyResistance => 15;
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    [Constructible]
    public BalronBoneArmor()
    {
        Hue = 2753;
        Attributes.BonusStr = 5;
        Attributes.BonusDex = 5;
        Attributes.BonusInt = 5;
        Attributes.BonusHits = 5;
        Attributes.BonusStam = 8;
        Attributes.BonusMana = 8;
        Attributes.LowerManaCost = 8;
    }
}