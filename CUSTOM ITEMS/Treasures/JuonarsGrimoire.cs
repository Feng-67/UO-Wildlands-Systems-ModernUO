using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class JuonarsGrimoire : NecromancerSpellbook
{
    // ServUO's SlayerGroup.RandomSuperSlayerTOL picks from the Time of Legends super slayers, which ModernUO does not
    // have. This picks from the seven classic super slayers instead.
    private static readonly SlayerName[] _superSlayers =
    [
        SlayerName.Repond,
        SlayerName.Silver,
        SlayerName.Exorcism,
        SlayerName.ElementalBan,
        SlayerName.ArachnidDoom,
        SlayerName.ReptilianDeath,
        SlayerName.Fey
    ];

    public override int LabelNumber => 1156300; // Juonar's Grimoire

    [Constructible]
    public JuonarsGrimoire()
    {
        Hue = 2500;

        SkillBonuses.SetValues(0, SkillName.Necromancy, 15.0);
        Slayer = _superSlayers.RandomElement();

        Attributes.BonusInt = 8;
        Attributes.SpellDamage = 15;
        Attributes.LowerManaCost = 10;
        Attributes.LowerRegCost = 10;
    }
}
