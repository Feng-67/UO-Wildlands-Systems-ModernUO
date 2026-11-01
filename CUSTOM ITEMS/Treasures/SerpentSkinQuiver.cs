using ModernUO.Serialization;
using Server.Engines.Craft;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class SerpentSkinQuiver : BaseQuiver
{
    public override string DefaultName => "Serpent Skin Quiver";

    [Constructible]
    public SerpentSkinQuiver() : base(0x2B02)
    {
        Hue = 2755;

        DamageIncrease = 10;
        Attributes.Luck = 125;
        Attributes.WeaponSpeed = 5;
        Attributes.WeaponDamage = 10;
        LowerAmmoCost = 30;
        WeightReduction = 30;
    }
}
