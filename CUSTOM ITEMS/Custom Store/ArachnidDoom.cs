using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class ArachnidDoom : BaseInstrument
{
    // TODO: the original file had no item ID or sounds (its base() call has no matching constructor).
    // Harp values (0xEB1, 0x43, 0x44) are used as a placeholder, matching NaturesTears. Change them to the real ones.
    [Constructible]
    public ArachnidDoom() : base(0xEB1, 0x43, 0x44)
    {
        Hue = 1944;
        Slayer = SlayerName.ArachnidDoom;
        Slayer2 = SlayerName.Silver;
        Quality = InstrumentQuality.Exceptional;
        UsesRemaining = 450;
    }

    public override double DefaultWeight => 4.0;

    public override int LabelNumber => 1154724; // Arachnid Doom

    public override int InitMinUses => 450;
    public override int InitMaxUses => 450;
}
