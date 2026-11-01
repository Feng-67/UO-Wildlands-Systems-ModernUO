using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class ExporMalasFlamus : BladedStaff
{
    public override int LabelNumber => 1160477; // Expor Malas Flamus
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    [Constructible]
    public ExporMalasFlamus()
    {
        Hue = 2702;
                
        WeaponAttributes.HitFireArea = 70;
        WeaponAttributes.HitLeechStam = 50;
        WeaponAttributes.HitLeechMana = 100;
        WeaponAttributes.HitLeechHits = 100;
        WeaponAttributes.HitLowerAttack = 50;
        Attributes.WeaponDamage = 30;
    }
}
