// UO Wildlands Custom Script
// Adapted for ModernUO from ServUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles.MannequinProperty;

public abstract class SlayerProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.HitEffects;
    public abstract SlayerName Slayer { get; }
    public override int Hue => 0x43FF;
    public override bool IsBoolen => true;
    public override int Description => 1152467;
    public override int SpriteW => 270;
    public override int SpriteH => 180;

    public override bool Matches(Item item) =>
        item is ISlayer slayer && (slayer.Slayer == Slayer || slayer.Slayer2 == Slayer);

    public override bool Matches(List<Item> items)
    {
        foreach (var item in items)
        {
            if (item is ISlayer slayer)
            {
                return slayer.Slayer == Slayer || slayer.Slayer2 == Slayer;
            }
        }

        return false;
    }
}

public abstract class TalismanSlayerProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.HitEffects;
    public abstract TalismanSlayerName Slayer { get; }
    public override int Hue => 0x43FF;
    public override bool IsBoolen => true;
    public override int SpriteW => 270;
    public override int SpriteH => 180;

    public override bool Matches(Item item) => item is BaseTalisman talisman && talisman.Slayer == Slayer;

    public override bool Matches(List<Item> items)
    {
        foreach (var item in items)
        {
            if (item is BaseTalisman talisman)
            {
                return talisman.Slayer == Slayer;
            }
        }

        return false;
    }
}

public class ReptileSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079751;
    public override SlayerName Slayer => SlayerName.ReptilianDeath;
}

public class DragonSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1061284;
    public override SlayerName Slayer => SlayerName.DragonSlaying;
}

public class LizardmanSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079738;
    public override SlayerName Slayer => SlayerName.LizardmanSlaughter;
}

public class OphidianSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079740;
    public override SlayerName Slayer => SlayerName.Ophidian;
}

public class SnakeSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079744;
    public override SlayerName Slayer => SlayerName.SnakesBane;
}

public class ArachnidSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079747;
    public override SlayerName Slayer => SlayerName.ArachnidDoom;
}

public class ScorpionSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079743;
    public override SlayerName Slayer => SlayerName.ScorpionsBane;
}

public class SpiderSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079746;
    public override SlayerName Slayer => SlayerName.SpidersDeath;
}

public class TerathanSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079753;
    public override SlayerName Slayer => SlayerName.Terathan;
}

public class RepondSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079750;
    public override SlayerName Slayer => SlayerName.Repond;
}

public class BatSlayerProperty : TalismanSlayerProperty
{
    public override int LabelNumber => 1072506;
    public override TalismanSlayerName Slayer => TalismanSlayerName.Bat;
}

public class BearSlayerProperty : TalismanSlayerProperty
{
    public override int LabelNumber => 1072504;
    public override TalismanSlayerName Slayer => TalismanSlayerName.Bear;
}

public class BeetleSlayerProperty : TalismanSlayerProperty
{
    public override int LabelNumber => 1072508;
    public override TalismanSlayerName Slayer => TalismanSlayerName.Beetle;
}

public class BirdSlayerProperty : TalismanSlayerProperty
{
    public override int LabelNumber => 1072509;
    public override TalismanSlayerName Slayer => TalismanSlayerName.Bird;
}

public class BovineSlayerProperty : TalismanSlayerProperty
{
    public override int LabelNumber => 1072512;
    public override TalismanSlayerName Slayer => TalismanSlayerName.Bovine;
}

public class FlameSlayerProperty : TalismanSlayerProperty
{
    public override int LabelNumber => 1072511;
    public override TalismanSlayerName Slayer => TalismanSlayerName.Flame;
}

public class IceSlayerProperty : TalismanSlayerProperty
{
    public override int LabelNumber => 1072510;
    public override TalismanSlayerName Slayer => TalismanSlayerName.Ice;
}

public class MageSlayerProperty : TalismanSlayerProperty
{
    public override int LabelNumber => 1072507;
    public override TalismanSlayerName Slayer => TalismanSlayerName.Mage;
}

public class OgreSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079739;
    public override SlayerName Slayer => SlayerName.OgreTrashing;
}

public class OrcSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079741;
    public override SlayerName Slayer => SlayerName.OrcSlaying;
}

public class TrollSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079754;
    public override SlayerName Slayer => SlayerName.TrollSlaughter;
}

public class VerminSlayerProperty : TalismanSlayerProperty
{
    public override int LabelNumber => 1072505;
    public override TalismanSlayerName Slayer => TalismanSlayerName.Vermin;
}

public class UndeadSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079752;
    public override int Description => 1152467;
    public override SlayerName Slayer => SlayerName.Silver;
}

public class DemonSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079748;
    public override SlayerName Slayer => SlayerName.Exorcism;
}

public class GargoyleSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079737;
    public override SlayerName Slayer => SlayerName.GargoylesFoe;
}

public class FeySlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1154652;
    public override SlayerName Slayer => SlayerName.Fey;
}

public class ElementalSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079749;
    public override SlayerName Slayer => SlayerName.ElementalBan;
}

public class AirElementalSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079733;
    public override SlayerName Slayer => SlayerName.Vacuum;
}

public class BloodElementalSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079734;
    public override SlayerName Slayer => SlayerName.BloodDrinking;
}

public class EarthElementalSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079735;
    public override SlayerName Slayer => SlayerName.EarthShatter;
}

public class FireElementalSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079736;
    public override SlayerName Slayer => SlayerName.FlameDousing;
}

public class PoisonElementalSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079742;
    public override SlayerName Slayer => SlayerName.ElementalHealth;
}

public class SnowElementalSlayerProperty : SlayerProperty
{
    public override int LabelNumber => 1079745;
    public override SlayerName Slayer => SlayerName.SummerWind;
}

public class WaterElementalSlayerProperty : SlayerProperty
{
    public override SlayerName Slayer => SlayerName.WaterDissipation;
    public override int LabelNumber => 1079755;
}
