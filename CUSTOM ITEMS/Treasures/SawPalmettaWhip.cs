using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class SawPalmettaWhip : BladedWhip
{
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    public override string DefaultName => "Saw Palmetta Whip";

    [Constructible]
    public SawPalmettaWhip()
    {
        Weight = 5.0;
        Hue = 2755;

        WeaponAttributes.HitHarm = 35;
        WeaponAttributes.HitLowerDefend = 30;
        WeaponAttributes.HitFireArea = 50;
        WeaponAttributes.HitLeechStam = 40;
        WeaponAttributes.HitLeechHits = 100;
        WeaponAttributes.HitLeechMana = 81;
        Attributes.WeaponDamage = 25;
    }
}
