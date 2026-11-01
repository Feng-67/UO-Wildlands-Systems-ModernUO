using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class TheLexiconOfJuonar : BookOfChivalry
{
    public override string DefaultName => "The Lexicon Of Juo'nar";

    [Constructible]
    public TheLexiconOfJuonar()
    {
        Hue = 2702;

        Attributes.RegenMana = 3;
        Attributes.SpellDamage = 50;
        Attributes.CastSpeed = 1;
        Attributes.LowerRegCost = 10;
    }
}
