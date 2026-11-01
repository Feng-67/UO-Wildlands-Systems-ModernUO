using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class LordMorphiusEpaulettes : BaseOuterTorso
{
    public override string DefaultName => "Lord Morphius' Epaulettes";

    [Constructible]
    public LordMorphiusEpaulettes() : base(0x9985, 0)
    {
        Hue = 2742;
        Attributes.BonusStam = 8;
        Attributes.RegenStam = 2;
        Attributes.LowerManaCost = 3;
        Attributes.WeaponSpeed = 10;
    }
}