using ModernUO.Serialization;

namespace Server.Items;

// Combined imbuing ingredients used by the custom Imbuing engine
// (Server.Engines.Imbuing). Grouped into a single file so the
// cliloc/itemId table is easy to audit.
//
// Ordered by cliloc ascending. CrystallineBlackrock keeps its
// pre-SA cliloc (1077568) and therefore sorts first.

/// <summary>
/// Crystalline Blackrock - Used for skill bonuses on jewelry
/// </summary>
[SerializationGenerator(0, false)]
public partial class CrystallineBlackrock : Item
{
    [Constructible]
    public CrystallineBlackrock(int amount = 1) : base(0x5732)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x455;
    }

    public override int LabelNumber => 1077568; // Crystalline Blackrock
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Seed of Renewal - Used for regeneration properties
/// </summary>
[SerializationGenerator(0, false)]
public partial class SeedOfRenewal : Item
{
    [Constructible]
    public SeedOfRenewal(int amount = 1) : base(0x5736)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x48;
    }

    public override int LabelNumber => 1113346; // Seed of Renewal
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Crystal Shards - Used for damage increase properties
/// </summary>
[SerializationGenerator(0, false)]
public partial class CrystalShards : Item
{
    [Constructible]
    public CrystalShards(int amount = 1) : base(0x5738)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x47E;
    }

    public override int LabelNumber => 1113347; // Crystal Shards
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Lodestone - Used for skill belts crafting and imbuing skill bonuses
/// Drops from Cavern of the Discarded
/// </summary>
[SerializationGenerator(0, false)]
public partial class Lodestone : Item
{
    [Constructible]
    public Lodestone(int amount = 1) : base(0x5739)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113348; // Lodestone
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Elven Fletching - Used for lower stat req
/// </summary>
[SerializationGenerator(0, false)]
public partial class ElvenFletching : Item
{
    [Constructible]
    public ElvenFletching(int amount = 1) : base(0x5741)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113349; // Elven Fletching
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Delicate Scales - Used for use best skill
/// </summary>
[SerializationGenerator(0, false)]
public partial class DelicateScales : Item
{
    [Constructible]
    public DelicateScales(int amount = 1) : base(0x5743)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113350; // Delicate Scales
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Crushed Glass - Used for enhance potions
/// </summary>
[SerializationGenerator(0, false)]
public partial class CrushedGlass : Item
{
    [Constructible]
    public CrushedGlass(int amount = 1) : base(0x573F)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113351; // Crushed Glass
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Abyssal Cloth - Used for mage armor
/// </summary>
[SerializationGenerator(0, false)]
public partial class AbyssalCloth : Item
{
    [Constructible]
    public AbyssalCloth(int amount = 1) : base(0x5733)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113352; // Abyssal Cloth
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Powdered Iron - Used for durability
/// </summary>
[SerializationGenerator(0, false)]
public partial class PowderedIron : Item
{
    [Constructible]
    public PowderedIron(int amount = 1) : base(0x5740)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113353; // Powdered Iron
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Void Orb - Used for leech properties
/// </summary>
[SerializationGenerator(0, false)]
public partial class VoidOrb : Item
{
    [Constructible]
    public VoidOrb(int amount = 1) : base(0x573A)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x455;
    }

    public override int LabelNumber => 1113354; // Void Orb
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Boura Pelt - Used for resistance properties
/// </summary>
[SerializationGenerator(0, false)]
public partial class BouraPelt : Item
{
    [Constructible]
    public BouraPelt(int amount = 1) : base(0x5742)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113355; // Boura Pelt
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Arcanic Rune Stone - Used for mage weapon
/// </summary>
[SerializationGenerator(0, false)]
public partial class ArcanicRuneStone : Item
{
    [Constructible]
    public ArcanicRuneStone(int amount = 1) : base(0x573C)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113356; // Arcanic Rune Stone
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Chaga Mushroom - Used for luck
/// </summary>
[SerializationGenerator(0, false)]
public partial class ChagaMushroom : Item
{
    [Constructible]
    public ChagaMushroom(int amount = 1) : base(0x573E)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113357; // Chaga Mushroom
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Faery Dust - Used for lower reagent cost
/// </summary>
[SerializationGenerator(0, false)]
public partial class FaeryDust : Item
{
    [Constructible]
    public FaeryDust(int amount = 1) : base(0x5745)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x9;
    }

    public override int LabelNumber => 1113358; // Faery Dust
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Silver Snake Skin - Used for spell channeling
/// </summary>
[SerializationGenerator(0, false)]
public partial class SilverSnakeSkin : Item
{
    [Constructible]
    public SilverSnakeSkin(int amount = 1) : base(0x5744)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113359; // Silver Snake Skin
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Reflective Wolf Eye - Used for reflect physical
/// </summary>
[SerializationGenerator(0, false)]
public partial class ReflectiveWolfEye : Item
{
    [Constructible]
    public ReflectiveWolfEye(int amount = 1) : base(0x5749)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113360; // Reflective Wolf Eye
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Bottle of Ichor - Used for night sight
/// </summary>
[SerializationGenerator(0, false)]
public partial class BottleIchor : Item
{
    [Constructible]
    public BottleIchor(int amount = 1) : base(0x573D)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113361; // Bottle of Ichor
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Daemon Claw - Used for Exorcism slayer
/// </summary>
[SerializationGenerator(0, false)]
public partial class DaemonClaw : Item
{
    [Constructible]
    public DaemonClaw(int amount = 1) : base(0x5721)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113362; // Daemon Claw
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Goblin Blood - Used for Repond slayer
/// </summary>
[SerializationGenerator(0, false)]
public partial class GoblinBlood : Item
{
    [Constructible]
    public GoblinBlood(int amount = 1) : base(0x572C)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113363; // Goblin Blood
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Vial of Vitriol - Used for Elemental Ban slayer
/// </summary>
[SerializationGenerator(0, false)]
public partial class VialOfVitriol : Item
{
    [Constructible]
    public VialOfVitriol(int amount = 1) : base(0x5722)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113364; // Vial of Vitriol
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Lava Serpent Crust - Used for Reptilian Death slayer
/// </summary>
[SerializationGenerator(0, false)]
public partial class LavaSerpentCrust : Item
{
    [Constructible]
    public LavaSerpentCrust(int amount = 1) : base(0x572D)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113365; // Lava Serpent Crust
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Raptor Teeth - Used for hit area effects
/// </summary>
[SerializationGenerator(0, false)]
public partial class RaptorTeeth : Item
{
    [Constructible]
    public RaptorTeeth(int amount = 1) : base(0x5746)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113366; // Raptor Teeth
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Slith Tongue - Used for Hit Dispel
/// </summary>
[SerializationGenerator(0, false)]
public partial class SlithTongue : Item
{
    [Constructible]
    public SlithTongue(int amount = 1) : base(0x5748)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113367; // Slith Tongue
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Spider Carapace - Used for Arachnid Doom slayer
/// </summary>
[SerializationGenerator(0, false)]
public partial class SpiderCarapace : Item
{
    [Constructible]
    public SpiderCarapace(int amount = 1) : base(0x5720)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113368; // Spider Carapace
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Undying Flesh - Used for Silver/Undead slayer
/// </summary>
[SerializationGenerator(0, false)]
public partial class UndyingFlesh : Item
{
    [Constructible]
    public UndyingFlesh(int amount = 1) : base(0x5728)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113369; // Undying Flesh
    public override double DefaultWeight => 0.1;
}

/// <summary>
/// Fey Wings - Used for Fey slayer
/// </summary>
[SerializationGenerator(0, false)]
public partial class FeyWings : Item
{
    [Constructible]
    public FeyWings(int amount = 1) : base(0x5734)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0;
    }

    public override int LabelNumber => 1113371; // Fey Wings
    public override double DefaultWeight => 0.1;
}