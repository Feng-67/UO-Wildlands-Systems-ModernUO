using System;
using ModernUO.Serialization;

namespace Server.Items;

[TypeAlias("Server.Items.Putrefication")]
[SerializationGenerator(0)]
public partial class Putrefaction : Item, ICommodity
{
    [Constructible]
    public Putrefaction() : this(1)
    {
    }

    [Constructible]
    public Putrefaction(int amountFrom, int amountTo) : this(Utility.RandomMinMax(amountFrom, amountTo))
    {
    }

    [Constructible]
    public Putrefaction(int amount) : base(0x3186)
    {
        Stackable = true;
        Amount = amount;
        Hue = 883;
    }

    public Putrefaction(Serial serial) : base(serial)
    {
    }

    int ICommodity.DescriptionNumber => LabelNumber;
    bool ICommodity.IsDeedable => true;
}
