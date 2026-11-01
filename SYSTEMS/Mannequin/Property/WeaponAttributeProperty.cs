// UO Wildlands Custom Script
// Adapted for ModernUO from ServUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles.MannequinProperty;

public abstract class WeaponAttr : ValuedProperty
{
    public abstract AosWeaponAttribute Attribute { get; }

    public double GetPropertyValue(Item item) => item is BaseWeapon weapon ? weapon.WeaponAttributes[Attribute] : 0;

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

public class WeaponSelfRepairProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.None;
    public override int LabelNumber => 1079709;
    public override bool IsSpriteGraph => true;
    public override int SpriteW => 0;
    public override int SpriteH => 300;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.SelfRepair;
}

public class HitLeechHitsProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079698;
    public override int Description => 1152425;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLeechHits;
    public override int Hue => 0x43FF;
    public override int SpriteW => 90;
    public override int SpriteH => 180;
}

public class HitLeechStamProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079707;
    public override int Description => 1152423;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLeechStam;
    public override int Hue => 0x43FF;
    public override int SpriteW => 150;
    public override int SpriteH => 180;
}

public class HitLeechManaProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079701;
    public override int Description => 1152424;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLeechMana;
    public override int Hue => 0x43FF;
    public override int SpriteW => 120;
    public override int SpriteH => 180;
}

public class HitLowerAttackProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079699;
    public override int Description => 1152420;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLowerAttack;
    public override int Hue => 0x43FF;
    public override int SpriteW => 0;
    public override int SpriteH => 120;
}

public class HitLowerDefendProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079700;
    public override int Description => 1152419;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLowerDefend;
    public override int Hue => 0x43FF;
    public override int SpriteW => 30;
    public override int SpriteH => 120;
}

public class HitMagicArrowProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079706;
    public override int Description => 1152427;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitMagicArrow;
    public override int Hue => 0x43FF;
    public override int SpriteW => 270;
    public override int SpriteH => 150;
}

public class HitHarmProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079704;
    public override int Description => 1152413;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitHarm;
    public override int Hue => 0x43FF;
    public override int SpriteW => 0;
    public override int SpriteH => 180;
}

public class HitFireballProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079703;
    public override int Description => 1152412;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitFireball;
    public override int Hue => 0x43FF;
    public override int SpriteW => 210;
    public override int SpriteH => 150;
}

public class HitLightningProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079705;
    public override int Description => 1152426;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLightning;
    public override int Hue => 0x43FF;
    public override int SpriteW => 240;
    public override int SpriteH => 150;
}

public class HitDispelProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079702;
    public override int Description => 1152409;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitDispel;
    public override int Hue => 0x43FF;
    public override int SpriteW => 30;
    public override int SpriteH => 180;
}

public class HitColdAreaProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079693;
    public override int Description => 1152422;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitColdArea;
    public override int Hue => 0x43FF;
    public override int SpriteW => 120;
    public override int SpriteH => 150;
}

public class HitFireAreaProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079695;
    public override int Description => 1152422;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitFireArea;
    public override int Hue => 0x43FF;
    public override int SpriteW => 90;
    public override int SpriteH => 150;
}

public class HitPoisonAreaProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079697;
    public override int Description => 1152422;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitPoisonArea;
    public override int Hue => 0x43FF;
    public override int SpriteW => 150;
    public override int SpriteH => 150;
}

public class HitEnergyAreaProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079694;
    public override int Description => 1152422;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitEnergyArea;
    public override int Hue => 0x43FF;
    public override int SpriteW => 180;
    public override int SpriteH => 150;
}

public class HitPhysicalAreaProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1079696;
    public override int Description => 1152422;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitPhysicalArea;
    public override int Hue => 0x43FF;
    public override int SpriteW => 60;
    public override int SpriteH => 150;
}

public class UseBestWeaponSkillProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.Combat1;
    public override bool IsBoolen => true;
    public override int LabelNumber => 1079592;
    public override int Description => 1152393;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.UseBestSkill;
    public override int Hue => 0x5E5D;
    public override int SpriteW => 30;
    public override int SpriteH => 330;
}

public class MageWeaponProperty : WeaponAttr
{
    public override Catalog Catalog => Catalog.Casting;
    public override int LabelNumber => 1079759;
    public override int Description => 1152428;
    public override AosWeaponAttribute Attribute => AosWeaponAttribute.MageWeapon;
    public override int Hue => 0x1FF;
    public override int SpriteW => 0;
    public override int SpriteH => 150;
}

public abstract class ElementalDamageAttr : ValuedProperty
{
    public override Catalog Catalog => Catalog.Combat2;
    public override bool AlwaysVisible => true;
    public abstract AosElementAttribute Element { get; }
    public override int Hue => 0x5E5D;

    public double GetPropertyValue(Item item) => item is BaseWeapon weapon ? GetElementAttributeValue(weapon, Element) : 0;

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

    private static int GetElementAttributeValue(BaseWeapon weapon, AosElementAttribute element)
    {
        weapon.GetDamageTypes(
            null,
            out var phys,
            out var fire,
            out var cold,
            out var pois,
            out var nrgy,
            out var chaos,
            out var direct
        );

        return element switch
        {
            AosElementAttribute.Physical => phys,
            AosElementAttribute.Fire => fire,
            AosElementAttribute.Cold => cold,
            AosElementAttribute.Poison => pois,
            AosElementAttribute.Energy => nrgy,
            AosElementAttribute.Chaos => chaos,
            AosElementAttribute.Direct => direct,
            _ => -1
        };
    }
}

public class PhysicalDamageProperty : ElementalDamageAttr
{
    public override int LabelNumber => 1151800;
    public override int Description => 1152461;
    public override AosElementAttribute Element => AosElementAttribute.Physical;
    public override int SpriteW => 270;
    public override int SpriteH => 210;
}

public class ColdDamageProperty : ElementalDamageAttr
{
    public override int LabelNumber => 1151802;
    public override int Description => 1152462;
    public override AosElementAttribute Element => AosElementAttribute.Cold;
    public override int SpriteW => 210;
    public override int SpriteH => 210;
}

public class FireDamageProperty : ElementalDamageAttr
{
    public override int LabelNumber => 1151801;
    public override int Description => 1152472;
    public override AosElementAttribute Element => AosElementAttribute.Fire;
    public override int SpriteW => 180;
    public override int SpriteH => 210;
}

public class PoisonDamageProperty : ElementalDamageAttr
{
    public override int LabelNumber => 1151803;
    public override int Description => 1152473;
    public override AosElementAttribute Element => AosElementAttribute.Poison;
    public override int SpriteW => 0;
    public override int SpriteH => 240;
}

public class EnergyDamageProperty : ElementalDamageAttr
{
    public override int LabelNumber => 1151804;
    public override int Description => 1152474;
    public override AosElementAttribute Element => AosElementAttribute.Energy;
    public override int SpriteW => 240;
    public override int SpriteH => 210;
}

public class ChaosDamageProperty : ElementalDamageAttr
{
    public override int LabelNumber => 1151805;
    public override int Description => 1152475;
    public override AosElementAttribute Element => AosElementAttribute.Chaos;
    public override int SpriteW => 30;
    public override int SpriteH => 240;
}

public class VelocityProperty : ValuedProperty
{
    public override Catalog Catalog => Catalog.HitEffects;
    public override int LabelNumber => 1080416;
    public override int Description => 1152392;
    public override int Hue => 0x5E5D;
    public override int SpriteW => 90;
    public override int SpriteH => 120;

    public double GetPropertyValue(Item item) => item is BaseRanged ranged ? ranged.Velocity : 0;

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
