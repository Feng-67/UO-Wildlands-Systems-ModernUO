using ModernUO.Serialization;

namespace Server.Items;

[Flippable(0x2684, 0x2683)]
[SerializationGenerator(0, false)]
public partial class RobeOfTheDarkMonk : BaseOuterTorso
{
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;
    
    public override string DefaultName => "Robe of the Dark Monk";

    [Constructible]
    public RobeOfTheDarkMonk() : this(1109)
    {
    }

    [Constructible]
    public RobeOfTheDarkMonk(int hue) : base(0x2684, hue)
    {
        Weight = 1.0;
        Hue = 0x455; // Note: the original also overrides the hue argument with this value
        Attributes.SpellDamage = 6;
        Attributes.IncreasedKarmaLoss = 6;
    }

    public override bool Dye(Mobile from, DyeTub sender)
    {
        from.SendLocalizedMessage(sender.FailMessage);
        return false;
    }
}
