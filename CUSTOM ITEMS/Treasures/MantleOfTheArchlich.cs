using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class MantleOfTheArchlich : Robe
{
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;
    public override bool CanFortify => true;

    public override string DefaultName => "Mantle of the Archlich";

    [Constructible]
    public MantleOfTheArchlich()
    {
        Hue = 2702;
        SkillBonuses.SetValues(0, SkillName.MagicResist, 10);
        Attributes.SpellDamage = 8;
        Attributes.CastSpeed = 1;
    }
}
