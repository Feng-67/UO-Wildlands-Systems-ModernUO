using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class RelviniansSpellbook : Spellbook
{
    public override string DefaultName => "Relvinian's Spellbook";

    [Constructible]
    public RelviniansSpellbook()
    {
        Hue = 1909; // Need to verify!
        Content = ulong.MaxValue;

        Attributes.RegenMana = 3;
        Attributes.SpellDamage = 50;
        Attributes.CastSpeed = 1;
        Attributes.LowerRegCost = 10;
    }
}
