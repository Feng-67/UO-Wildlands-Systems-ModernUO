// UO Wildlands Custom Script
// Adapted for ModernUO from ServUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles.MannequinProperty;

public abstract class ResistAttr : ValuedProperty
{
    public override Catalog Catalog => Catalog.Resistances;
    public override bool AlwaysVisible => true;
    public abstract ResistanceType Resist { get; }
    public override int Hue => 0x42FF;
    public override int Cap => 70;

    public double GetPropertyValue(Item item) => GetBaseResistBonus(item, Resist);

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

    public int GetBaseResistBonus(Item item, ResistanceType resist)
    {
        switch (resist)
        {
            case ResistanceType.Physical:
                {
                    if (item is BaseWeapon weapon)
                    {
                        return weapon.PhysicalResistance;
                    }

                    if (item is BaseArmor armor)
                    {
                        return armor.PhysicalResistance;
                    }

                    if (item is BaseClothing clothing)
                    {
                        return clothing.PhysicalResistance;
                    }

                    break;
                }
            case ResistanceType.Fire:
                {
                    if (item is BaseWeapon weapon)
                    {
                        return weapon.FireResistance;
                    }

                    if (item is BaseArmor armor)
                    {
                        return armor.FireResistance;
                    }

                    if (item is BaseClothing clothing)
                    {
                        return clothing.FireResistance;
                    }

                    break;
                }
            case ResistanceType.Cold:
                {
                    if (item is BaseWeapon weapon)
                    {
                        return weapon.ColdResistance;
                    }

                    if (item is BaseArmor armor)
                    {
                        return armor.ColdResistance;
                    }

                    if (item is BaseClothing clothing)
                    {
                        return clothing.ColdResistance;
                    }

                    break;
                }
            case ResistanceType.Poison:
                {
                    if (item is BaseWeapon weapon)
                    {
                        return weapon.PoisonResistance;
                    }

                    if (item is BaseArmor armor)
                    {
                        return armor.PoisonResistance;
                    }

                    if (item is BaseClothing clothing)
                    {
                        return clothing.PoisonResistance;
                    }

                    break;
                }
            case ResistanceType.Energy:
                {
                    if (item is BaseWeapon weapon)
                    {
                        return weapon.EnergyResistance;
                    }

                    if (item is BaseArmor armor)
                    {
                        return armor.EnergyResistance;
                    }

                    if (item is BaseClothing clothing)
                    {
                        return clothing.EnergyResistance;
                    }

                    break;
                }
        }

        return 0;
    }
}

public class PhysicalResistProperty : ResistAttr
{
    public override int Order => 1;
    public override int LabelNumber => 1079764;
    public override ResistanceType Resist => ResistanceType.Physical;
    public override int Description => 1115226;
    public override int SpriteW => 30;
    public override int SpriteH => 30;
}

public class FireResistProperty : ResistAttr
{
    public override int Order => 2;
    public override int LabelNumber => 1079763;
    public override ResistanceType Resist => ResistanceType.Fire;
    public override int Description => 1115227;
    public override int SpriteW => 150;
    public override int SpriteH => 0;
}

public class ColdResistProperty : ResistAttr
{
    public override int Order => 3;
    public override int LabelNumber => 1079761;
    public override ResistanceType Resist => ResistanceType.Cold;
    public override int Description => 1115228;
    public override int SpriteW => 0;
    public override int SpriteH => 0;
}

public class PoisonResistProperty : ResistAttr
{
    public override int Order => 4;
    public override int LabelNumber => 1079765;
    public override ResistanceType Resist => ResistanceType.Poison;
    public override int Description => 1115229;
    public override int SpriteW => 60;
    public override int SpriteH => 30;
}

public class EnergyResistProperty : ResistAttr
{
    public override int Order => 5;
    public override int LabelNumber => 1079762;
    public override ResistanceType Resist => ResistanceType.Energy;
    public override int Description => 1115230;
    public override int SpriteW => 120;
    public override int SpriteH => 0;
}
