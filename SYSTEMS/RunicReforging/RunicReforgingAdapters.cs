using System;
using System.Collections.Generic;
using Server.Engines.Imbuing;

namespace Server.Items;

/// <summary>
/// Item categories used by the reforging tables.
/// </summary>
/// <remarks>
/// The numeric values are part of the ported table data and must not be reordered: they index the
/// prefix/suffix collections in <c>RunicReforging.Tables.cs</c>. Note that they do NOT match
/// <see cref="ImbuingItemType"/> (Armor and Shield are swapped), so the two enums are mapped
/// explicitly rather than cast.
/// </remarks>
public enum ReforgingItemType
{
    Invalid = 0,
    Melee = 1,
    Ranged = 2,
    Shield = 3,
    Armor = 4,
    Jewel = 5,
    Hat = 6
}

/// <summary>
/// Reforging-facing facade over the custom imbuing property table.
/// </summary>
/// <remarks>
/// The ServUO reforging system was written against a richer <c>ItemPropertyInfo</c> API (per-item-type
/// attribute lookups, loot tables that include random slayer/skill markers, and validation that knows
/// about reforged loot categories). The custom <see cref="Server.Engines.Imbuing.ItemPropertyInfo"/> only
/// keeps the imbuing data, so the missing members live here.
/// </remarks>
public static class ReforgingPropertyInfo
{
    // Reforging loot tables mix real property ids with markers: 1000 asks the caller for a random
    // slayer and 1001..1005 ask for a random skill. The caller (RunicReforging.ApplyRandomProperty)
    // resolves the markers to real ids before validation.
    private const int RandomSlayerMarker = 1000;
    private const int FirstSkillMarker = 1001;
    private const int LastSkillMarker = 1005;

    /// <summary>
    /// Resolves an attribute value (or the "WeaponVelocity" marker string) to its property id, or -1.
    /// </summary>
    public static int GetID(object attribute)
    {
        if (attribute == null)
        {
            return -1;
        }

        // The table holds a few dozen entries and is only populated once, from Imbuing.Initialize,
        // so a linear scan is cheaper than maintaining a reverse map that could go stale.
        var id = -1;

        foreach (var (candidateId, info) in ItemPropertyInfo.Table)
        {
            var candidate = info.Attribute;

            if (candidate == null || !candidate.Equals(attribute))
            {
                continue;
            }

            // Keep the lowest id so the result is stable if two properties ever share an attribute.
            if (id < 0 || candidateId < id)
            {
                id = candidateId;
            }
        }

        return id;
    }

    public static int GetIDForAttribute(AosAttribute attribute) => GetID(attribute);

    public static int GetIDForAttribute(AosWeaponAttribute attribute) => GetID(attribute);

    public static int GetIDForAttribute(AosArmorAttribute attribute) => GetID(attribute);

    public static int GetIDForAttribute(AosElementAttribute attribute) => GetID(attribute);

    public static int GetIDForAttribute(SkillName attribute) => GetID(attribute);

    public static int GetIDForAttribute(SlayerName attribute) => GetID(attribute);

    /// <summary>
    /// Total budget cost of applying <paramref name="value"/> of the given attribute.
    /// </summary>
    public static int GetTotalWeight(Item item, object attribute, int value)
    {
        var id = GetID(attribute);

        if (id < 0)
        {
            return 0;
        }

        var info = ItemPropertyInfo.GetInfo(id);

        if (info == null)
        {
            return 0;
        }

        // Reforging budgets are measured against the standard (non-imbuing) cap; going over it is
        // what triggers the powerful-loot overcaps handled by the caller.
        var max = ItemPropertyInfo.GetMaxIntensity(item, id);

        if (max <= 0)
        {
            return 0;
        }

        return info.Weight * value / max;
    }

    public static bool ValidateProperty(Item item, object attribute, bool reforged)
    {
        var id = GetID(attribute);

        return id >= 0 && ValidateProperty(item, id, reforged);
    }

    /// <summary>
    /// Checks that a property can be applied to the item given its item type and loot category.
    /// </summary>
    public static bool ValidateProperty(Item item, int id, bool reforged)
    {
        if (item == null)
        {
            return false;
        }

        var info = ItemPropertyInfo.GetInfo(id);

        if (info == null)
        {
            return false;
        }

        var itemType = ItemPropertyInfo.GetItemType(item);

        if (itemType == ImbuingItemType.Invalid)
        {
            return false;
        }

        var category = GetCategory(info, itemType);

        if (category == null)
        {
            return false;
        }

        // Reforged loot only draws from categories that have loot values (or an overcap range, or a
        // hard maximum); ordinary reforging is limited to categories that have a standard cap.
        var valid = reforged
            ? category.LootMax > 0 || category.PowerfulLootRange != null || info.MaxIntensity > 0
            : category.StandardMax > 0 || info.MaxIntensity > 0;

        if (!valid)
        {
            return false;
        }

        // BalancedWeapon only exists on two-handed weapons.
        if (info.Attribute is AosAttribute attribute && attribute == AosAttribute.BalancedWeapon)
        {
            return item is BaseWeapon weapon && weapon.Layer == Layer.TwoHanded;
        }

        return true;
    }

    /// <summary>
    /// Loot property pool for the item, including the random slayer and random skill markers.
    /// </summary>
    public static List<int> LookupLootTable(Item item)
    {
        var props = new List<int>();

        var itemType = ItemPropertyInfo.GetItemType(item);

        if (itemType != ImbuingItemType.Invalid && ItemPropertyInfo.LootTable.TryGetValue(itemType, out var loot))
        {
            foreach (var id in loot)
            {
                props.Add(id);
            }
        }

        props.Add(RandomSlayerMarker);

        for (var marker = FirstSkillMarker; marker <= LastSkillMarker; marker++)
        {
            props.Add(marker);
        }

        return props;
    }

    public static int GetScale(Item item, object attribute)
    {
        var id = GetID(attribute);

        return id < 0 ? 1 : ItemPropertyInfo.GetScale(item, id);
    }

    /// <summary>
    /// Scale for an already-resolved property id.
    /// </summary>
    /// <remarks>
    /// The named-table rows in the source pass an id, not an attribute, so this overload must not be
    /// collapsed into the object one: an id would not match any attribute and would yield scale 1.
    /// </remarks>
    public static int GetScale(Item item, int id) => ItemPropertyInfo.GetScale(item, id);

    public static int GetMinIntensity(Item item, object attribute)
    {
        var id = GetID(attribute);

        return id < 0 ? 1 : ItemPropertyInfo.GetMinIntensity(item, id);
    }

    /// <summary>
    /// Minimum intensity for an already-resolved property id.
    /// </summary>
    public static int GetMinIntensity(Item item, int id) => ItemPropertyInfo.GetMinIntensity(item, id);

    public static object GetAttribute(int id) => ItemPropertyInfo.GetAttribute(id);

    /// <summary>
    /// Powerful-loot overcap steps for the property on this item type, or null when it has none.
    /// </summary>
    public static int[] GetMaxOvercappedRange(Item item, int id)
    {
        var info = ItemPropertyInfo.GetInfo(id);

        if (info == null)
        {
            return null;
        }

        var itemType = ItemPropertyInfo.GetItemType(item);

        if (itemType == ImbuingItemType.Invalid)
        {
            return null;
        }

        return GetCategory(info, itemType)?.PowerfulLootRange;
    }

    /// <summary>
    /// Whether the item carries any SA absorption ("eater") property.
    /// </summary>
    /// <remarks>
    /// ModernUO does not implement the SA <c>SAAbsorptionAttributes</c> collection, so no item can
    /// carry eaters. The source only used this to skip re-rolling eater properties on artifacts.
    /// </remarks>
    public static bool HasEater(Item item) => false;

    /// <summary>
    /// Reforging category for the item, mapped from the imbuing category.
    /// </summary>
    public static ReforgingItemType GetItemType(Item item) =>
        ItemPropertyInfo.GetItemType(item) switch
        {
            ImbuingItemType.Melee  => ReforgingItemType.Melee,
            ImbuingItemType.Ranged => ReforgingItemType.Ranged,
            ImbuingItemType.Shield => ReforgingItemType.Shield,
            ImbuingItemType.Armor  => ReforgingItemType.Armor,
            ImbuingItemType.Jewel  => ReforgingItemType.Jewel,
            ImbuingItemType.Hat    => ReforgingItemType.Hat,
            _                      => ReforgingItemType.Invalid
        };

    // PropCategories is sized for ImbuingItemType; guard the index so a future category cannot
    // throw here while the table is being built at server start.
    private static PropCategoryInfo GetCategory(ItemPropertyInfo info, ImbuingItemType itemType)
    {
        var categories = info.PropCategories;
        var index = (int)itemType;

        if (categories == null || index < 0 || index >= categories.Length)
        {
            return null;
        }

        return categories[index];
    }
}

/// <summary>
/// Reforging-facing facade over the custom imbuing engine.
/// </summary>
/// <remarks>
/// The ServUO reforging system called <c>Imbuing.GetTotalMods</c>, <c>Imbuing.GetTotalWeight</c>,
/// <c>Imbuing.SetProperty</c> and <c>Imbuing.GetIntensityForID</c>; the custom engine stubs the first
/// two out and has no <c>SetProperty</c>/<c>GetIntensityForID</c> at all, so they are reimplemented
/// over the property table here.
/// </remarks>
public static class ReforgingImbuing
{
    public static int GetValueForID(Item item, int id) => Imbuing.GetValueForID(item, id);

    public static int GetMaxWeight(Item item) => Imbuing.GetMaxWeight(item);

    /// <summary>
    /// Number of properties currently present on the item.
    /// </summary>
    public static int GetTotalMods(Item item)
    {
        var mods = 0;

        foreach (var (id, _) in ItemPropertyInfo.Table)
        {
            if (Imbuing.GetValueForID(item, id) > 0)
            {
                mods++;
            }
        }

        return mods;
    }

    /// <summary>
    /// Total intensity of every property on the item, optionally skipping one id.
    /// </summary>
    public static int GetTotalWeight(Item item, int excludeId, bool trueWeight, bool imbuing)
    {
        // trueWeight has no equivalent in the custom engine: it exposes a single value per property,
        // and the caller already selects the imbuing vs. standard cap through the imbuing flag.
        var total = 0;

        foreach (var (id, info) in ItemPropertyInfo.Table)
        {
            if (id == excludeId)
            {
                continue;
            }

            var value = Imbuing.GetValueForID(item, id);

            if (value <= 0)
            {
                continue;
            }

            var max = ItemPropertyInfo.GetMaxIntensity(item, id, imbuing);

            if (max <= 0)
            {
                continue;
            }

            total += info.Weight * value / max;
        }

        return total;
    }

    /// <summary>
    /// Applies a property id/value to the item, exactly as the imbuing gump would.
    /// </summary>
    /// <remarks>
    /// The source also cleared <c>SAAbsorptionAttributes</c> for eater properties; ModernUO has no
    /// such collection, so there is nothing to clear.
    /// </remarks>
    public static void SetProperty(Item item, int id, int value)
    {
        Imbuing.ImbueItem(null, item, id, value);
        item.InvalidateProperties();
    }

    /// <summary>
    /// Intensity (budget cost) of <paramref name="value"/> points of the given property.
    /// </summary>
    public static int GetIntensityForID(Item item, int id, int excludeId, int value)
    {
        var info = ItemPropertyInfo.GetInfo(id);

        if (info == null)
        {
            return 0;
        }

        var max = Math.Max(1, ItemPropertyInfo.GetMaxIntensity(item, id, true));

        return info.Weight * value / max;
    }

    /// <summary>
    /// Whether the type is excluded from imbuing.
    /// </summary>
    /// <remarks>
    /// The custom imbuing engine has no non-imbuable type list, so nothing is excluded.
    /// </remarks>
    public static bool IsInNonImbueList(Type type) => false;
}
