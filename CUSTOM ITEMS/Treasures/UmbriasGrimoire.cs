using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class UmbriasGrimoire : NecromancerSpellbook
{
    public override string DefaultName => "Umbria's Grimoire";

    [Constructible]
    public UmbriasGrimoire() : base((ulong)0x1FFFF) // Makes it full
    {
        Hue = 2500; // Same as Juo'nar's Grimoire I think?!

        Slayer = SlayerName.Silver;
        Attributes.DefendChance = 10;
        Attributes.SpellDamage = 30;
        Attributes.CastRecovery = 2;
        Attributes.LowerManaCost = 5;
    }
}
