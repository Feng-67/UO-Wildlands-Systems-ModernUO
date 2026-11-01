using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class GrimoireOfNature : Spellbook
{
    public override string DefaultName => "Grimoire Of Nature";

    [Constructible]
    public GrimoireOfNature()
    {
        Hue = 2755;
        Content = ulong.MaxValue;

        Slayer = SlayerName.Fey;
        Attributes.DefendChance = 10;
        Attributes.SpellDamage = 30;
        Attributes.CastRecovery = 2;
        Attributes.LowerManaCost = 5;
    }
}
