using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class DrogenisSpellbook : Spellbook
{
    public override string DefaultName => "Drogeni's Spellbook";

    [Constructible]
    public DrogenisSpellbook()
    {
        Hue = 1195;
        Content = ulong.MaxValue;

        SkillBonuses.SetValues(0, SkillName.Magery, 20.0);
        Attributes.DefendChance = 15;
        Attributes.CastSpeed = 1;
        Attributes.CastRecovery = 3;
        Attributes.SpellDamage = 15;
        Attributes.EnhancePotions = 25;
    }
}
