using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class SilverbranchBow : Yumi
{
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    public override string DefaultName => "Silverbranch Bow";

    [Constructible]
    public SilverbranchBow()
    {
        // Hue = ; TODO: NEED HUE!

        WeaponAttributes.HitLeechHits = 40;
        WeaponAttributes.HitLeechMana = 40;
        WeaponAttributes.HitLightning = 35;
        Attributes.AttackChance = 10;
        Attributes.WeaponDamage = 25;
        AosElementDamages.Fire = 100;
    }
}
