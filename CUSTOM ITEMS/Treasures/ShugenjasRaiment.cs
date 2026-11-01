using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class ShugenjasRaiment : PlateDo
{
    public override int LabelNumber => 1161973; // Shugenja's Raiment
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    public override int BasePhysicalResistance => 15;
    public override int BaseFireResistance => 15;
    public override int BaseColdResistance => 15;
    public override int BasePoisonResistance => 15;
    public override int BaseEnergyResistance => 15;

    [Constructible]
    public ShugenjasRaiment()
    {
        Hue = 2764;
        Attributes.BonusInt = 5;
        Attributes.BonusHits = 5;
        Attributes.RegenHits = 3;
        Attributes.RegenMana = 3;
        Attributes.SpellDamage = 5;
        Attributes.CastRecovery = 1;
        Attributes.LowerManaCost = 8;
        Attributes.LowerRegCost = 20;
        ArmorAttributes.MageArmor = 1;
    }
}

[SerializationGenerator(0, false)]
public partial class ShugenjasRaimentStudded : StuddedDo
{
    public override int LabelNumber => 1161973; // Shugenja's Raiment
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    public override int BasePhysicalResistance => 15;
    public override int BaseFireResistance => 15;
    public override int BaseColdResistance => 15;
    public override int BasePoisonResistance => 15;
    public override int BaseEnergyResistance => 15;

    [Constructible]
    public ShugenjasRaimentStudded()
    {
        Hue = 2764;
        Attributes.BonusInt = 5;
        Attributes.BonusHits = 5;
        Attributes.RegenHits = 3;
        Attributes.RegenMana = 3;
        Attributes.SpellDamage = 5;
        Attributes.CastRecovery = 1;
        Attributes.LowerManaCost = 8;
        Attributes.LowerRegCost = 20;
        ArmorAttributes.MageArmor = 1;
    }
}
