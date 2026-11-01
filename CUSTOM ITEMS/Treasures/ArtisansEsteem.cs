using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class ArtisansEsteem : SilverRing
{
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    public override string DefaultName => "Artisan's Esteem";

    [Constructible]
    public ArtisansEsteem()
    {
        Weight = 1.0;
        Hue = 2758;

        SkillBonuses.SetValues(0, SkillName.EvalInt, 20);
        Attributes.EnhancePotions = 25;
        Attributes.DefendChance = 20;
        Attributes.SpellDamage = 15;
        Attributes.CastRecovery = 3;
    }
}
