using System;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class BloodOfTheDarkFather : Item, ICommodity
{
    public override int LabelNumber => 1157343; // Blood of the Dark Father

    [Constructible]
    public BloodOfTheDarkFather() : this(1)
    {
    }

    [Constructible]
    public BloodOfTheDarkFather(int amount) : base(0x9D7F)
    {
        Hue = 2741;
        Stackable = true;
        Amount = amount;
    }

    public BloodOfTheDarkFather(Serial serial) : base(serial)
    {
    }

    int ICommodity.DescriptionNumber => LabelNumber;
    bool ICommodity.IsDeedable => true;
}
