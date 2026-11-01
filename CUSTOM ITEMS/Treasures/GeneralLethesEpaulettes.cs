using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class GeneralLethesEpaulettes : BaseOuterTorso
{
    public override string DefaultName => "General Lethe's Epaulettes";

    [Constructible]
    public GeneralLethesEpaulettes() : base(0x9985, 0)
    {
        Hue = 2642;
        Attributes.BonusMana = 8;
        Attributes.RegenMana = 1;
        Attributes.CastRecovery = 1;
        Attributes.LowerRegCost = 10;
    }
}