using System;
using ModernUO.Serialization;
using Server.Targeting;

namespace Server.Items;

public enum CompassionPigmentType
{
    PhoenixRed = 0,
    AuraOfAmber,
    DeepViolet,
    PolishedBronze,
    VibrantCrimson,
    Lavender,
    GleamingFuchsia,
    DeepBlue,
    GlossyFuchsia,
    DarkVoid,
    MurkySeagreen,
    ReflectiveShadow,
    LiquidSunshine,
    ShadowyBlue,
    BlackAndGreen,
    GlossyBlue,
    HunterGreen,
    SlateBlue,
    MotherOfPearl,
    StarBlue,
    MurkyAmber,
    VibranSeagreen,
    VibrantOcher,
    MossyGreen,
    OliveGreen,
    MottledSunsetBlue,
    TyrianPurple,
    IntenseTeal
}

public class CompassionPigmentInfo
{
    private static readonly CompassionPigmentInfo[] _table =
    {
        new(CompassionPigmentType.PhoenixRed, 1964, 1151651),
        new(CompassionPigmentType.AuraOfAmber, 1967, 1152308),
        new(CompassionPigmentType.DeepViolet, 1929, 1151912),
        new(CompassionPigmentType.PolishedBronze, 1944, 1151909),
        new(CompassionPigmentType.VibrantCrimson, 1964, 1153386),
        new(CompassionPigmentType.Lavender, 1951, 1151650),
        new(CompassionPigmentType.GleamingFuchsia, 1930, 1152311),
        new(CompassionPigmentType.DeepBlue, 1939, 1152348),
        new(CompassionPigmentType.GlossyFuchsia, 1919, 1152347),
        new(CompassionPigmentType.DarkVoid, 2068, 1154214),
        new(CompassionPigmentType.MurkySeagreen, 1992, 1152309),
        new(CompassionPigmentType.ReflectiveShadow, 1910, 1153387),
        new(CompassionPigmentType.LiquidSunshine, 1923, 1154213),
        new(CompassionPigmentType.ShadowyBlue, 1960, 1152310),
        new(CompassionPigmentType.BlackAndGreen, 1979, 1151911),
        new(CompassionPigmentType.GlossyBlue, 1916, 1151910),
        new(CompassionPigmentType.HunterGreen, 1936, 1151649),
        new(CompassionPigmentType.SlateBlue, 1983, 1151653),
        new(CompassionPigmentType.MotherOfPearl, 2720, 1154120),
        new(CompassionPigmentType.StarBlue, 2723, 1154121),
        new(CompassionPigmentType.MurkyAmber, 1989, 1152350),
        new(CompassionPigmentType.VibranSeagreen, 1970, 1152349),
        new(CompassionPigmentType.VibrantOcher, 2725, 1154736),
        new(CompassionPigmentType.MossyGreen, 2684, 1154731),
        new(CompassionPigmentType.OliveGreen, 2709, 1154733),
        new(CompassionPigmentType.MottledSunsetBlue, 2714, 1154734),
        new(CompassionPigmentType.TyrianPurple, 2716, 1154735),
        new(CompassionPigmentType.IntenseTeal, 2691, 1154732)
    };

    public CompassionPigmentInfo(CompassionPigmentType type, int hue, int labelNumber)
    {
        Type = type;
        Hue = hue;
        LabelNumber = labelNumber;
    }

    public static int Count => _table.Length;

    public CompassionPigmentType Type { get; }
    public int Hue { get; }
    public int LabelNumber { get; }

    public static CompassionPigmentInfo GetInfo(CompassionPigmentType type)
    {
        var v = (int)type;

        if (v < 0 || v >= _table.Length)
        {
            v = 0;
        }

        return _table[v];
    }
}

[SerializationGenerator(0, false)]
public partial class CompassionPigment : Item, IUsesRemaining
{
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private CompassionPigmentType _type;

    [InvalidateProperties]
    [SerializableField(1)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private int _usesRemaining;

    [Constructible]
    public CompassionPigment() : this(GetRandomType())
    {
    }

    [Constructible]
    public CompassionPigment(CompassionPigmentType type) : this(type, 5)
    {
    }

    [Constructible]
    public CompassionPigment(CompassionPigmentType type, int uses) : base(0x0FAB)
    {
        _type = type;
        _usesRemaining = uses;

        Hue = CompassionPigmentInfo.GetInfo(_type).Hue;
    }

    public override double DefaultWeight => 1.0;

    public override int LabelNumber => CompassionPigmentInfo.GetInfo(_type).LabelNumber;

    bool IUsesRemaining.ShowUsesRemaining
    {
        get => true;
        set { }
    }

    private static CompassionPigmentType GetRandomType() =>
        (CompassionPigmentType)Utility.Random(CompassionPigmentInfo.Count);

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        list.Add(1060584, _usesRemaining); // uses remaining: ~1_val~
    }

    public override void OnDoubleClick(Mobile from)
    {
        from.SendLocalizedMessage(1042015); // Select the item you wish to dye.
        from.BeginTarget(8, false, TargetFlags.None, OnDyeTarget);
    }

    private void OnDyeTarget(Mobile from, object targeted)
    {
        if (Deleted || targeted is not Item item)
        {
            return;
        }

        if (!item.IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }
        else if (item is MetalPigmentsOfTokuno or LesserPigmentsOfTokuno or PigmentsOfTokuno or CompassionPigment)
        {
            from.SendLocalizedMessage(1042083); // You cannot dye that.
        }
        else if (item.IsLockedDown)
        {
            from.SendLocalizedMessage(501479); // That is locked down and you cannot use it.
        }
        else
        {
            item.Hue = Hue;

            if (--UsesRemaining <= 0)
            {
                Delete();
            }
        }
    }
}
