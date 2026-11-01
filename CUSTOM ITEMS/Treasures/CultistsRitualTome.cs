using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class CultistsRitualTome : Spellbook
{
    [Constructible]
    public CultistsRitualTome()
    {
        Hue = 2745;
        LootType = LootType.Blessed;

        Slayer = RandomSuperSlayer();

        // Caddellite Infused
        Attributes.DefendChance = 5;
        Attributes.SpellDamage = 25;
        Attributes.CastRecovery = 2;
        Attributes.LowerManaCost = 4;
    }

    public override int LabelNumber => 1158717; // Cultist's Ritual Tome

    // TODO: ModernUO has no SlayerGroup.RandomSuperSlayerTOL() and none of the Time of Legends slayers.
    // This picks a random super slayer from the slayer groups ModernUO does have. Replace it with a
    // TOL-specific list if you port those slayers.
    private static SlayerName RandomSuperSlayer() =>
        SlayerGroup.Groups[Utility.Random(SlayerGroup.Groups.Length)].Super.Name;
}
