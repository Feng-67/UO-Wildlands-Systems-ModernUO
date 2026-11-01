using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class WealdCodex : Spellbook
{
    public override string DefaultName => "Weald Codex";

    [Constructible]
    public WealdCodex()
    {
        Hue = 2758;
        Content = ulong.MaxValue;

        Attributes.RegenMana = 3;
        Attributes.SpellDamage = 50;
        Attributes.CastSpeed = 1;
        Attributes.LowerRegCost = 10;
    }
}
