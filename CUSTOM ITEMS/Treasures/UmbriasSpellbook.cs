using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class UmbriasSpellbook : Spellbook
{
    public override string DefaultName => "Umbria's Spellbook";

    [Constructible]
    public UmbriasSpellbook()
    {
        Hue = 898; // Not verified!
        Content = ulong.MaxValue;

        Attributes.RegenMana = 3;
        Attributes.SpellDamage = 50;
        Attributes.CastSpeed = 1;
        Attributes.LowerRegCost = 10;
    }
}
