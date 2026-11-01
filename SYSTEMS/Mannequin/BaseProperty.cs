// UO Wildlands Custom Script
// Adapted for ModernUO from ServUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles;

public enum Catalog
{
    None = 0,
    Attributes = 1,
    Resistances = 2,
    Combat1 = 3,
    Combat2 = 4,
    Casting = 5,
    Misc = 6,
    HitEffects = 7,
    SkillBonusGear = 8
}

public abstract class Property
{
    public abstract int LabelNumber { get; }
    public abstract Catalog Catalog { get; }
    public virtual int Order => 1000;
    public virtual int Cap { get; set; }
    public virtual int Description { get; set; }

    public virtual bool IsBoolen => false;
    public virtual bool BoolenValue => false;
    public virtual bool AlwaysVisible => false;

    public virtual bool IsSpriteGraph => false;
    public virtual int SpriteH { get; set; }
    public virtual int SpriteW { get; set; }

    public abstract bool Matches(Item item);
    public abstract bool Matches(List<Item> items);
}

public abstract class ValuedProperty : Property
{
    public virtual int Hue => 0;
    public double Value { get; set; }

    public override bool Matches(List<Item> items) => false;
}

public class LabelDefinition
{
    public int TitleLabel { get; set; }
    public Catalog Catalog { get; set; }
    public int ColumnLeftCount { get; set; }

    public LabelDefinition(int titleLabel, Catalog catalog, int columnLeftCount = 0)
    {
        TitleLabel = titleLabel;
        Catalog = catalog;
        ColumnLeftCount = columnLeftCount;
    }
}
