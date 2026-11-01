using ModernUO.Serialization;
using Server.Ethics;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class AnBalXen : BaseTalisman
{
    public override string DefaultName => "An Bal Xen";

    [Constructible]
    public AnBalXen() : base(0x9E28)
    {
        
        Attributes.BonusStr = 1;
        Attributes.RegenHits = 2;
        Attributes.WeaponDamage = 20;
        Attributes.AttackChance = 10;
    }
}
