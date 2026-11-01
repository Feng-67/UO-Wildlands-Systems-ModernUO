using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class DivineSanctifier : HammerPick
{
    public override int LabelNumber => 1161982; // Divine Sanctifier
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    [Constructible]
    public DivineSanctifier()
    {
        Hue = 2764;
        WeaponAttributes.HitLeechMana = 100;
        WeaponAttributes.HitLeechHits = 100;
        WeaponAttributes.HitLeechStam = 50;
        WeaponAttributes.HitLowerDefend = 50;
        WeaponAttributes.HitLightning = 70;
        Attributes.WeaponDamage = 50;
    }
}
