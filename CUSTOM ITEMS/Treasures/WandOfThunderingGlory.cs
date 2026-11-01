using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class WandOfThunderingGlory : BaseWand
{
    private const WandEffect NoEffect = (WandEffect)(-1);

    public override int LabelNumber => 1116623; // Wand of Thundering Glory
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    [Constructible]
    public WandOfThunderingGlory() : base(NoEffect, 0, 0)
    {
        WeaponAttributes.HitLightning = 40;
        Attributes.AttackChance = 5;
        Attributes.WeaponSpeed = 10;
        Attributes.WeaponDamage = 50;
        Attributes.SpellDamage = 10;
    }

    public override void GetDamageTypes(
        Mobile wielder, out int phys, out int fire, out int cold, out int pois,
        out int nrgy, out int chaos, out int direct
    )
    {
        // 100% chaos damage
        phys = fire = cold = nrgy = pois = direct = 0;
        chaos = 100;
    }
}
