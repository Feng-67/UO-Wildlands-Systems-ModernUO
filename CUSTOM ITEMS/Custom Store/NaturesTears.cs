using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class NaturesTears : BaseInstrument
{
    // TODO: the original file had no item ID or sounds (its base() call has no matching constructor).
    // Harp values (0xEB1, 0x43, 0x44) are used as a placeholder at your request. Change them to the real ones.
    [Constructible]
    public NaturesTears() : base(0xEB1, 0x43, 0x44)
    {
        Hue = 2075;
        Slayer = SlayerName.Fey;
        Slayer2 = SlayerName.Repond;
        Quality = InstrumentQuality.Exceptional;
        UsesRemaining = 450;
    }

    public override double DefaultWeight => 5.0;

    public override int LabelNumber => 1154373; // Nature's Tears

    public override int InitMinUses => 450;
    public override int InitMaxUses => 450;
}
