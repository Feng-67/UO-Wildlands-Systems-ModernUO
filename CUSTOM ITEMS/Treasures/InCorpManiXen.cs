using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class InCorpManiXen : BaseTalisman
{
    public override bool ForceShowName => true;

    public override string DefaultName => "In Corp Mani Xen";

    [Constructible]
    public InCorpManiXen() : base(0x9E29)
    {
        Weight = 1.0;
        Hue = 2755;
        Attributes.BonusStr = 1;
        Attributes.RegenHits = 2;
        Attributes.AttackChance = 10;
        Attributes.WeaponDamage = 20;
    }
}
