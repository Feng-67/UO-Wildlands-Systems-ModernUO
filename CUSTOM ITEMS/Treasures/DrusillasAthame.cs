using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class DrusillasAthame : Kryss
{
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    public override string DefaultName => "Drusilla's Athame";

    [Constructible]
    public DrusillasAthame()
    {
        Hue = 2753;
                
        Attributes.SpellChanneling = 1;
        Attributes.SpellDamage = 25;
        Attributes.CastSpeed = 1;
        Attributes.WeaponDamage = 20;
    }

    public override void GetDamageTypes(
        Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos,
        out int direct
    )
    {
        fire = pois = nrgy = chaos = direct = 0;

        phys = cold = 50;
    }
}
