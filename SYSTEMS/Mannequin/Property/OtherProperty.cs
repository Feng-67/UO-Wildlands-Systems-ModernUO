// UO Wildlands Custom Script
// Adapted for ModernUO from ServUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles.MannequinProperty;

public class DurabilityProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.None;
    public override int LabelNumber => 1017323;
    public override int Description => 1152404;
    public override int Hue => 0x1F0;

    public override bool Matches(Item item)
    {
        int prop, prop2;

        if (item is BaseArmor armor)
        {
            if ((prop = armor.HitPoints) >= 0 && (prop2 = armor.MaxHitPoints) > 0)
            {
                Value = prop;
                Cap = prop2;
                return true;
            }
        }

        if (item is BaseJewel jewel)
        {
            if ((prop = jewel.HitPoints) >= 0 && (prop2 = jewel.MaxHitPoints) > 0)
            {
                Value = prop;
                Cap = prop2;
                return true;
            }
        }

        if (item is BaseWeapon weapon)
        {
            if ((prop = weapon.HitPoints) >= 0 && (prop2 = weapon.MaxHitPoints) > 0)
            {
                Value = prop;
                Cap = prop2;
                return true;
            }
        }

        if (item is BaseClothing clothing)
        {
            if ((prop = clothing.HitPoints) >= 0 && (prop2 = clothing.MaxHitPoints) > 0)
            {
                Value = prop;
                Cap = prop2;
                return true;
            }
        }

        return false;
    }
}

public class BlessedProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.None;
    public override int LabelNumber => 1038021;
    public override bool IsSpriteGraph => true;
    public override int SpriteW => 150;
    public override int SpriteH => 210;

    public override bool Matches(Item item) => item.LootType == LootType.Blessed;
}

public class CursedProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.None;
    public override int LabelNumber => 1049643;
    public override bool IsSpriteGraph => true;
    public override int SpriteW => 150;
    public override int SpriteH => 240;

    public override bool Matches(Item item) => item.LootType == LootType.Cursed;
}

public class MedableArmorProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.Combat1;
    public override int Order => 6;
    public override bool AlwaysVisible => true;
    public override bool IsBoolen => true;
    public override bool BoolenValue => true;
    public override int LabelNumber => 1159280;
    public override int SpriteW => 0;
    public override int SpriteH => 150;

    public double GetPropertyValue(Item item) => item is BaseArmor armor ? GetArmorMeditationValue(armor) : 0;

    public override bool Matches(Item item)
    {
        var total = GetPropertyValue(item);

        return total != 0;
    }

    public override bool Matches(List<Item> items)
    {
        double total = 0;

        foreach (var item in items)
        {
            total += GetPropertyValue(item);
        }

        return total != 0;
    }

    private static double GetArmorMeditationValue(BaseArmor armor)
    {
        if (armor == null)
        {
            return 0;
        }

        if (armor.ArmorAttributes.MageArmor != 0 || armor.Attributes.SpellChanneling != 0)
        {
            return 0;
        }

        return armor.MeditationAllowance switch
        {
            ArmorMeditationAllowance.None => armor.BaseArmorRatingScaled,
            ArmorMeditationAllowance.Half => armor.BaseArmorRatingScaled / 2.0,
            ArmorMeditationAllowance.All => 0.0,
            _ => armor.BaseArmorRatingScaled
        };
    }
}

public class GargoyleProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.None;
    public override int LabelNumber => 1111709;
    public override bool IsSpriteGraph => true;
    public override int SpriteW => 30;
    public override int SpriteH => 270;

    public override bool Matches(Item item) => item switch
    {
        BaseArmor armor       => armor.RequiredRaces == Race.AllowGargoylesOnly,
        BaseJewel jewel       => jewel.RequiredRaces == Race.AllowGargoylesOnly,
        BaseWeapon weapon     => weapon.RequiredRaces == Race.AllowGargoylesOnly,
        BaseClothing clothing => clothing.RequiredRaces == Race.AllowGargoylesOnly,
        _                     => false
    };
}

public class ElfProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.None;
    public override int LabelNumber => 1075086;
    public override bool IsSpriteGraph => true;
    public override int SpriteW => 210;
    public override int SpriteH => 240;

    public override bool Matches(Item item) => item switch
    {
        BaseArmor armor       => armor.RequiredRaces == Race.AllowElvesOnly,
        BaseJewel jewel       => jewel.RequiredRaces == Race.AllowElvesOnly,
        BaseWeapon weapon     => weapon.RequiredRaces == Race.AllowElvesOnly,
        BaseClothing clothing => clothing.RequiredRaces == Race.AllowElvesOnly,
        _                     => false
    };
}

public class DamageModifierProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.Misc;
    public override int LabelNumber => 1159401;
    public override int Description => 1152402;
    public override int Hue => 0x43FF;
    public override int SpriteW => 180;
    public override int SpriteH => 270;

    public double GetPropertyValue(Item item) => item is BaseQuiver quiver ? quiver.DamageIncrease : 0;

    public override bool Matches(Item item)
    {
        Value = GetPropertyValue(item);

        return Value != 0;
    }

    public override bool Matches(List<Item> items)
    {
        double total = 0;

        foreach (var item in items)
        {
            total += GetPropertyValue(item);
        }

        Value = total;

        return Value != 0;
    }
}
