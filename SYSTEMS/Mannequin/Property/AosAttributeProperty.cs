// UO Wildlands Custom Script
// Adapted for ModernUO from ServUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles.MannequinProperty;

public abstract class MagicalAttr : ValuedProperty
{
    public abstract AosAttribute Attribute { get; }

    public double GetPropertyValue(Item item)
    {
        if (item is BaseWeapon weapon)
        {
            return weapon.Attributes[Attribute];
        }

        if (item is BaseArmor armor)
        {
            return armor.Attributes[Attribute];
        }

        if (item is BaseClothing clothing)
        {
            return clothing.Attributes[Attribute];
        }

        if (item is BaseJewel jewel)
        {
            return jewel.Attributes[Attribute];
        }

        if (item is BaseTalisman talisman)
        {
            return talisman.Attributes[Attribute];
        }

        if (item is BaseQuiver quiver)
        {
            return quiver.Attributes[Attribute];
        }

        if (item is Spellbook spellbook)
        {
            return spellbook.Attributes[Attribute];
        }

        return 0;
    }

    public override bool Matches(Item item)
    {
        var total = GetPropertyValue(item);

        if (!IsBoolen)
        {
            Value = GetPropertyValue(item);
        }

        return total != 0;
    }

    public override bool Matches(List<Item> items)
    {
        double total = 0;

        foreach (var item in items)
        {
            total += GetPropertyValue(item);
        }

        if (!IsBoolen)
        {
            Value = total;
        }

        return total != 0;
    }
}

public class DamageIncreaseProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Combat1;
    public override int Order => 1;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1079760;
    public override int Description => 1115234;
    public override AosAttribute Attribute => AosAttribute.WeaponDamage;
    public override int Cap => 100;
    public override int Hue => 0x5E5D;
    public override int SpriteW => 60;
    public override int SpriteH => 0;
}

public class DefenseChanceIncreaseProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Combat1;
    public override int Order => 2;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075620;
    public override int Description => 1152403;
    public override AosAttribute Attribute => AosAttribute.DefendChance;
    public override int Cap => 45;
    public override int Hue => 0x5E5D;
    public override int SpriteW => 210;
    public override int SpriteH => 30;
}

public class HitChanceIncreaseProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Combat1;
    public override int Order => 3;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075616;
    public override int Description => 1115232;
    public override AosAttribute Attribute => AosAttribute.AttackChance;
    public override int Cap => 45;
    public override int Hue => 0x5E5D;
    public override int SpriteW => 210;
    public override int SpriteH => 0;
}

public class SwingSpeedIncreaseProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Combat1;
    public override int Order => 4;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075629;
    public override int Description => 1115233;
    public override AosAttribute Attribute => AosAttribute.WeaponSpeed;
    public override int Cap => 60;
    public override int Hue => 0x5E5D;
    public override int SpriteW => 150;
    public override int SpriteH => 30;
}

public class LowerManaCostProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Combat1;
    public override int Order => 5;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075621;
    public override int Description => 1115240;
    public override AosAttribute Attribute => AosAttribute.LowerManaCost;
    public override int Cap => 41;
    public override int Hue => 0x5E5D;
    public override int SpriteW => 60;
    public override int SpriteH => 60;
}

public class ReflectPhysicalDamageProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Combat1;
    public override int Order => 7;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075626;
    public override int Description => 1152399;
    public override AosAttribute Attribute => AosAttribute.ReflectPhysical;
    public override int Cap => 250;
    public override int Hue => 0x5E5D;
    public override int SpriteW => 150;
    public override int SpriteH => 60;
}

public class EnhancePotionsProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Combat1;
    public override int Order => 8;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075624;
    public override int Description => 1152405;
    public override AosAttribute Attribute => AosAttribute.EnhancePotions;
    public override int Hue => 0x5E5D;
    public override int Cap => 50;
    public override int SpriteW => 240;
    public override int SpriteH => 30;
}

public class LuckProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Combat1;
    public override int Order => 9;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1061153;
    public override int Description => 1152415;
    public override AosAttribute Attribute => AosAttribute.Luck;
    public override int Hue => 0x5E5D;
    public override int SpriteW => 270;
    public override int SpriteH => 0;
}

public class StrengthBonusProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Attributes;
    public override int Order => 1;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1079767;
    public override int Description => 1152432;
    public override AosAttribute Attribute => AosAttribute.BonusStr;
    public override int Hue => 0x43FF;
    public override int SpriteW => 120;
    public override int SpriteH => 30;
}

public class DexterityBonusProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Attributes;
    public override int Order => 2;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1079732;
    public override int Description => 1152434;
    public override AosAttribute Attribute => AosAttribute.BonusDex;
    public override int Hue => 0x43FF;
    public override int SpriteW => 90;
    public override int SpriteH => 0;
}

public class IntelligenceBonusProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Attributes;
    public override int Order => 3;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1079756;
    public override int Description => 1152433;
    public override AosAttribute Attribute => AosAttribute.BonusInt;
    public override int Hue => 0x43FF;
    public override int SpriteW => 240;
    public override int SpriteH => 0;
}

public class HitPointsIncreaseProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Attributes;
    public override int Order => 4;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1079404;
    public override int Description => 1152421;
    public override AosAttribute Attribute => AosAttribute.BonusHits;
    public override int Hue => 0x43FF;
    public override int Cap => 25;
    public override int SpriteW => 180;
    public override int SpriteH => 0;
}

public class StaminaIncreaseProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Attributes;
    public override int Order => 5;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1079405;
    public override int Description => 1152395;
    public override AosAttribute Attribute => AosAttribute.BonusStam;
    public override int Hue => 0x43FF;
    public override int Cap => 25;
    public override int SpriteW => 90;
    public override int SpriteH => 30;
}

public class ManaIncreaseProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Attributes;
    public override int Order => 6;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1079406;
    public override int Description => 1152431;
    public override AosAttribute Attribute => AosAttribute.BonusMana;
    public override int Hue => 0x43FF;
    public override int Cap => 25;
    public override int SpriteW => 0;
    public override int SpriteH => 30;
}

public class HitPointRegenerationProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Attributes;
    public override int Order => 7;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075627;
    public override int Description => 1115241;
    public override AosAttribute Attribute => AosAttribute.RegenHits;
    public override int Cap => 18;
    public override int Hue => 0x43FF;
    public override int SpriteW => 30;
    public override int SpriteH => 60;
}

public class StaminaRegerationProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Attributes;
    public override int Order => 8;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1079411;
    public override int Description => 1115242;
    public override AosAttribute Attribute => AosAttribute.RegenStam;
    public override int Cap => 24;
    public override int Hue => 0x43FF;
    public override int SpriteW => 210;
    public override int SpriteH => 60;
}

public class ManaRegenerationProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Attributes;
    public override int Order => 9;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1079410;
    public override int Description => 1115243;
    public override AosAttribute Attribute => AosAttribute.RegenMana;
    public override int Cap => 30;
    public override int Hue => 0x43FF;
    public override int SpriteW => 120;
    public override int SpriteH => 60;
}

public class SpellDamageIncreaseProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Casting;
    public override int Order => 4;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075628;
    public override int Description => 1152397;
    public override AosAttribute Attribute => AosAttribute.SpellDamage;
    public override int Hue => 0x1FF;
    public override int SpriteW => 180;
    public override int SpriteH => 60;
}

public class FasterCastRecoveryProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Casting;
    public override int Order => 2;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075618;
    public override int Description => 1152406;
    public override AosAttribute Attribute => AosAttribute.CastRecovery;
    public override int Hue => 0x1FF;
    public override int Cap => 6;
    public override int SpriteW => 0;
    public override int SpriteH => 60;
}

public class FasterCastingProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Casting;
    public override int Order => 1;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075617;
    public override int Description => 1152407;
    public override AosAttribute Attribute => AosAttribute.CastSpeed;
    public override int Hue => 0x1FF;
    public override int Cap => 4;
    public override int SpriteW => 270;
    public override int SpriteH => 30;
}

public class LowerReagentCostProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Casting;
    public override int Order => 3;
    public override bool AlwaysVisible => true;
    public override int LabelNumber => 1075625;
    public override int Description => 1115235;
    public override AosAttribute Attribute => AosAttribute.LowerRegCost;
    public override int Cap => 100;
    public override int Hue => 0x1FF;
    public override int SpriteW => 90;
    public override int SpriteH => 60;
}

public class SpellChannelingCostProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Casting;
    public override bool IsBoolen => true;
    public override int LabelNumber => 1079766;
    public override int Description => 1152398;
    public override AosAttribute Attribute => AosAttribute.SpellChanneling;
    public override int Hue => 0x1FF;
    public override int SpriteW => 210;
    public override int SpriteH => 120;
}

public class NightSightProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Misc;
    public override bool IsBoolen => true;
    public override int LabelNumber => 1075643;
    public override int Description => 1152416;
    public override AosAttribute Attribute => AosAttribute.NightSight;
    public override int Hue => 0x43FF;
    public override int SpriteW => 30;
    public override int SpriteH => 150;
}

// ModernUO models lower ammo cost directly on BaseQuiver instead of as an AosAttribute.
public class LowerAmmoCostProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.Misc;
    public override int LabelNumber => 1159315;
    public override int Description => 1152459;
    public override int Hue => 0x43FF;
    public override int SpriteW => 150;
    public override int SpriteH => 270;

    public override bool Matches(Item item)
    {
        Value = item is BaseQuiver quiver ? quiver.LowerAmmoCost : 0;

        return Value != 0;
    }

    public override bool Matches(List<Item> items)
    {
        double total = 0;

        foreach (var item in items)
        {
            if (item is BaseQuiver quiver)
            {
                total += quiver.LowerAmmoCost;
            }
        }

        Value = total;

        return Value != 0;
    }
}

public class BalancedWeaponProperty : MagicalAttr
{
    public override Catalog Catalog => Catalog.Combat1;
    public override bool IsBoolen => true;
    public override int LabelNumber => 1072792;
    public override int Description => 1152384;
    public override AosAttribute Attribute => AosAttribute.BalancedWeapon;
    public override int Hue => 0x5E5D;
    public override int SpriteW => 180;
    public override int SpriteH => 120;
}
