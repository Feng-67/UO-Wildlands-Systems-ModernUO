// UO Wildlands Custom Script
// Adapted for ModernUO from ServUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles.MannequinProperty;

public abstract class AosArmorAttr : ValuedProperty
{
    public abstract AosArmorAttribute Attribute { get; }

    public double GetPropertyValue(Item item)
    {
        if (item is BaseArmor armor)
        {
            return armor.ArmorAttributes[Attribute];
        }

        if (item is BaseClothing clothing)
        {
            return clothing.ClothingAttributes[Attribute];
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

public class ArmorSelfRepairProperty : AosArmorAttr
{
    public override Catalog Catalog => Catalog.None;
    public override int LabelNumber => 1079709;
    public override bool IsSpriteGraph => true;
    public override int SpriteW => 0;
    public override int SpriteH => 300;
    public override AosArmorAttribute Attribute => AosArmorAttribute.SelfRepair;
}
