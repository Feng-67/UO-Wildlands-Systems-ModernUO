using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class SerpentsBite : BaseTalisman
{
    public override int LabelNumber => 1161981; // Serpent's Bite
    
    public override bool ForceShowName => true;

    [Constructible]
    public SerpentsBite() : base(0x2F59)
    {
        Hue = 2764;
        Attributes.NightSight = 1;
        Attributes.Luck = 125;
        Attributes.SpellDamage = 8;
        Attributes.LowerManaCost = 5;
        Attributes.LowerRegCost = 10;
    }
}
