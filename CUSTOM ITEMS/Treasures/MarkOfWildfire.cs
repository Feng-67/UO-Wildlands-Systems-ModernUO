using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class MarkOfWildfire : SavageMask
{
    public override int BasePhysicalResistance => 15;
    public override int BaseFireResistance => 15;
    public override int BaseColdResistance => 15;
    public override int BasePoisonResistance => 15;
    public override int BaseEnergyResistance => 15;
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    public override string DefaultName => "Mark Of Wildfire";

    [Constructible]
    public MarkOfWildfire()
    {
        Hue = 2758;
        Attributes.BonusMana = 10;
        Attributes.Luck = 250;
        Attributes.LowerManaCost = 10;
        Attributes.LowerRegCost = 25;
    }
}