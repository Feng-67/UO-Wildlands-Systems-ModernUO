using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Engines.Imbuing;
using Server.Gumps;
using Server.Logging;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items;

public enum ReforgedPrefix
{
    None,
    Might,
    Mystic,
    Animated,
    Arcane,
    Exquisite,
    Vampiric,
    Invigorating,
    Fortified,
    Auspicious,
    Charmed,
    Vicious,
    Towering
}

public enum ReforgedSuffix
{
    None,
    Vitality,
    Sorcery,
    Haste,
    Wizadry,
    Quality,
    Vampire,
    Restoration,
    Defense,
    Fortune,
    Alchemy,
    Slaughter,
    Aegis,
    Blackthorn,
    Minax,
    Kotl,
    Khaldun,
    Doom,
    EnchantedOrigin,
    Fellowship
}

public enum ItemPower
{
    None,
    Minor,
    Lesser,
    Greater,
    Major,
    LesserArtifact,
    GreaterArtifact,
    MajorArtifact,
    LegendaryArtifact,
    ReforgedMinor,
    ReforgedLesser,
    ReforgedGreater,
    ReforgedMajor,
    ReforgedLegendary
}

public static partial class RunicReforging
{
    private static readonly ILogger logger = LogFactory.GetLogger(typeof(RunicReforging));

    public static bool CanReforge(Mobile from, Item item, CraftSystem crsystem)
    {
        CraftItem crItem = null;
        bool allowableSpecial = _allowableTable.ContainsKey(item.GetType());

        if (!allowableSpecial)
        {
            crItem = crsystem?.CraftItems?.SearchFor(item.GetType());
        }

        if (crItem == null && !allowableSpecial)
        {
            from.SendLocalizedMessage(1152279); // You cannot re-forge that item with this tool.
            return false;
        }

        bool goodtogo = true;
        int mods = GetTotalMods(item);

        // ServUO's BaseWeapon.DImodded / BaseArmor.IsMageArmorType checks have no ModernUO
        // equivalent. Approximate the original allowed existing-mod count: plain weapons and
        // mage armor are permitted to already carry one property, everything else must be clean.
        var maxmods = item is JukaBow || item is BaseWeapon ||
                      (item is BaseArmor armor && armor.ArmorAttributes.MageArmor > 0)
            ? 1
            : 0;

        if (item is BaseWeapon weapon &&
            (weapon.AosElementDamages[AosElementAttribute.Fire] > 0 ||
                weapon.AosElementDamages[AosElementAttribute.Cold] > 0 ||
                weapon.AosElementDamages[AosElementAttribute.Poison] > 0 ||
                weapon.AosElementDamages[AosElementAttribute.Energy] > 0))
        {
            mods++;
        }

        if (mods > maxmods)
        {
            goodtogo = false;
        }
        else if (_allowableTable.ContainsKey(item.GetType()) && _allowableTable[item.GetType()] != crsystem)
        {
            goodtogo = false;
        }
        else if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied)
        {
            goodtogo = false;
        }
        else if (!allowableSpecial && item is BaseClothing && !(item is BaseHat))
        {
            goodtogo = false;
        }
        else if (ReforgingImbuing.IsInNonImbueList(item.GetType()))
        {
            goodtogo = false;
        }

        if (!goodtogo)
        {
            from.SendLocalizedMessage(1152113); // You cannot reforge that item.
        }

        return goodtogo;
    }

    public static void ApplyReforgedProperties(
        Item item, List<int> props, ReforgedPrefix prefix, ReforgedSuffix suffix, int budget, int perclow, int perchigh,
        int maxmods, int luckChance
    )
    {
        ApplyReforgedProperties(
            item, props, prefix, suffix, budget, perclow, perchigh, maxmods, luckChance, null, ReforgingOption.None
        );
    }

    public static void ApplyReforgedProperties(
        Item item, ReforgedPrefix prefix, ReforgedSuffix suffix, int budget, int perclow, int perchigh, int maxmods,
        int luckchance, BaseRunicTool tool, ReforgingOption option
    )
    {
        var props = new List<int>(ReforgingPropertyInfo.LookupLootTable(item));
        ApplyReforgedProperties(item, props, prefix, suffix, budget, perclow, perchigh, maxmods, luckchance, tool, option);
    }

    public static void ApplyReforgedProperties(
        Item item, List<int> props, ReforgedPrefix prefix, ReforgedSuffix suffix, int budget, int perclow, int perchigh,
        int maxmods, int luckchance, BaseRunicTool tool, ReforgingOption option
    )
    {
        bool reforged = tool != null;
        bool powerful = reforged ? (option & ReforgingOption.Powerful) != 0 : IsPowerful(budget);

        if (prefix == ReforgedPrefix.None && (suffix == ReforgedSuffix.None || suffix > ReforgedSuffix.Aegis))
        {
            for (int i = 0; i < maxmods; i++)
            {
                ApplyRandomProperty(item, props, perclow, perchigh, ref budget, luckchance, reforged, powerful);
            }

            if (suffix != ReforgedSuffix.None)
            {
                ApplySuffixName(item, suffix);
            }
        }
        else
        {
            int prefixID = (int)prefix;
            int suffixID = (int)suffix;

            int index = GetCollectionIndex(item);
            int resIndex = -1;
            int preIndex = -1;
            // resIndex & preIndex = -1 indicates is not reforged

            if (reforged)
            {
                resIndex = GetResourceIndex(tool.Resource);
                preIndex = GetPrerequisiteIndex(option);
            }

            if (index == -1)
            {
                return;
            }

            List<NamedInfoCol> prefixCol = null;
            List<NamedInfoCol> suffixCol = null;

            if (prefix != ReforgedPrefix.None)
            {
                try
                {
                    prefixCol = new List<NamedInfoCol>();
                    prefixCol.AddRange(_prefixSuffixInfo[prefixID][index]);
                }
                catch
                {
                    logger.Error("Error: Prefix not in collection: {PrefixId}", prefixID);
                }
            }

            if (suffix != ReforgedSuffix.None)
            {
                if (suffixCol == null)
                {
                    suffixCol = new List<NamedInfoCol>();
                }

                try
                {
                    suffixCol.AddRange(_prefixSuffixInfo[suffixID][index]);
                }
                catch
                {
                    logger.Error("Error: Suffix not in collection: {SuffixId}", suffixID);
                }
            }

            // Removes things like blood drinking/balanced/splintering
            CheckAttributes(item, prefixCol, reforged);
            CheckAttributes(item, suffixCol, reforged);

            int i = 0;
            int mods = 0;

            if (prefix != ReforgedPrefix.None && suffix == ReforgedSuffix.None && prefixCol != null)
            {
                int specialAdd = 0;
                int nothing = 0;
                GetNamedModCount(index, prefixID, 0, maxmods, prefixCol.Count, 0, ref specialAdd, ref nothing);

                while (budget > 25 && mods < maxmods && i < 25)
                {
                    if (prefixCol.Count > 0 && specialAdd > 0)
                    {
                        int random = Utility.Random(prefixCol.Count);

                        if (ApplyPrefixSuffixAttribute(
                                item, prefixCol[random], resIndex, preIndex, perclow, perchigh, ref budget, luckchance,
                                reforged, powerful
                            ))
                        {
                            specialAdd--;
                            mods++;
                        }

                        prefixCol.RemoveAt(random);
                    }
                    else if (ApplyRandomProperty(
                                 item, props, perclow, perchigh, ref budget, luckchance, reforged, powerful
                             ))
                    {
                        mods++;
                    }

                    i++;
                }

                if (prefix != ReforgedPrefix.None)
                {
                    ApplyPrefixName(item, prefix);
                }
            }
            else if (prefix == ReforgedPrefix.None && suffix != ReforgedSuffix.None && suffixCol != null)
            {
                int specialAdd = 0;
                int nothing = 0;
                GetNamedModCount(index, 0, suffixID, maxmods, 0, suffixCol.Count, ref nothing, ref specialAdd);

                while (budget > 25 && mods < maxmods && i < 25)
                {
                    if (suffixCol.Count > 0 && specialAdd > 0)
                    {
                        int random = Utility.Random(suffixCol.Count);

                        if (ApplyPrefixSuffixAttribute(
                                item, suffixCol[random], resIndex, preIndex, perclow, perchigh, ref budget, luckchance,
                                reforged, powerful
                            ))
                        {
                            specialAdd--;
                            mods++;
                        }

                        suffixCol.RemoveAt(random);
                    }
                    else if (ApplyRandomProperty(
                                 item, props, perclow, perchigh, ref budget, luckchance, reforged, powerful
                             ))
                    {
                        mods++;
                    }

                    i++;
                }

                if (suffix != ReforgedSuffix.None)
                {
                    ApplySuffixName(item, suffix);
                }
            }
            else if (prefix != ReforgedPrefix.None && suffix != ReforgedSuffix.None && prefixCol != null &&
                     suffixCol != null)
            {
                int specialAddPrefix = 0;
                int specialAddSuffix = 0;

                GetNamedModCount(
                    index, prefixID, suffixID, maxmods, prefixCol.Count, suffixCol.Count, ref specialAddPrefix,
                    ref specialAddSuffix
                );

                while (budget > 25 && mods < maxmods && i < 25)
                {
                    if (prefixCol.Count > 0 && specialAddPrefix > 0)
                    {
                        int random = Utility.Random(prefixCol.Count);

                        if (ApplyPrefixSuffixAttribute(
                                item, prefixCol[random], resIndex, preIndex, perclow, perchigh, ref budget, luckchance,
                                reforged, powerful
                            ))
                        {
                            specialAddPrefix--;
                            mods++;
                        }

                        prefixCol.RemoveAt(random);
                    }
                    else if (suffixCol.Count > 0 && specialAddSuffix > 0)
                    {
                        int random = Utility.Random(suffixCol.Count);

                        if (ApplyPrefixSuffixAttribute(
                                item, suffixCol[random], resIndex, preIndex, perclow, perchigh, ref budget, luckchance,
                                reforged, powerful
                            ))
                        {
                            specialAddSuffix--;
                            mods++;
                        }

                        suffixCol.RemoveAt(random);
                    }
                    else if (ApplyRandomProperty(
                                 item, props, perclow, perchigh, ref budget, luckchance, reforged, powerful
                             ))
                    {
                        mods++;
                    }

                    i++;
                }

                if (prefix != ReforgedPrefix.None)
                {
                    ApplyPrefixName(item, prefix);
                }

                if (suffix != ReforgedSuffix.None)
                {
                    ApplySuffixName(item, suffix);
                }
            }

            _elements.Remove(item);
        }
    }

    public static bool HasSelection(
        int index, Item toreforge, BaseRunicTool tool, ReforgingOption options, int prefix, int suffix
    )
    {
        // No Vampire prefix/suffix for non-weapons
        if (index == 6 && !(toreforge is BaseWeapon))
        {
            return false;
        }

        // Cannot choose same suffix/prefix
        //if (index != 0 && (index == prefix || index == suffix))
        //    return false;HasOption(options, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulAndFundamental)

        var type = ReforgingPropertyInfo.GetItemType(toreforge);

        if (type == ReforgingItemType.Melee)
        {
            switch (tool.Resource)
            {
                case CraftResource.DullCopper:
                    if (index == 8 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.ShadowIron:
                    if (index == 8 && HasOption(
                            options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental
                        ))
                    {
                        return false;
                    }
                    else if (index >= 8 && index <= 10 &&
                             HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Copper:
                    if (index == 8 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental
                        ))
                    {
                        return false;
                    }

                    if (index >= 8 && index <= 10 && HasOption(
                            options, ReforgingOption.StructuralAndFundamental,
                            ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Bronze:
                    if (index == 8)
                    {
                        return false;
                    }

                    if (index == 9 && HasOption(options, ReforgingOption.Powerful))
                    {
                        return false;
                    }

                    if (index >= 8 && index <= 10 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Gold:
                    if (index == 8)
                    {
                        return false;
                    }

                    if (index >= 8 && index <= 10 && HasOption(
                            options, ReforgingOption.Powerful, ReforgingOption.PowerfulAndStructural,
                            ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental,
                            ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Agapite:
                case CraftResource.Verite:
                    if (index >= 8 && index <= 10)
                    {
                        return false;
                    }

                    if (index == 12 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Valorite:
                    if (index >= 8 && index <= 10)
                    {
                        return false;
                    }

                    if (index == 12 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;

                case CraftResource.OakWood:
                    if (index == 8 && HasOption(options, ReforgingOption.StructuralAndFundamental))
                    {
                        return false;
                    }

                    if (index >= 8 && index <= 10 &&
                        HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.AshWood:
                    if (index == 8)
                    {
                        return false;
                    }

                    if (index >= 8 && index <= 10 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.YewWood:
                    if (index >= 8 && index <= 10)
                    {
                        return false;
                    }

                    if (index == 12 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Heartwood:
                    if (index >= 8 && index <= 10)
                    {
                        return false;
                    }

                    if (index == 12 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
            }
        }
        else if (type == ReforgingItemType.Ranged)
        {
            switch (tool.Resource)
            {
                case CraftResource.OakWood:
                    if (index == 10 && HasOption(options, ReforgingOption.PowerfulAndStructural))
                    {
                        return false;
                    }

                    if ((index == 8 || index == 10) && HasOption(options, ReforgingOption.PowerfulAndFundamental))
                    {
                        return false;
                    }

                    if (index >= 8 && index <= 10 && (HasOption(
                            options, ReforgingOption.PowerfulStructuralAndFundamental
                        ) || HasOption(options, ReforgingOption.StructuralAndFundamental)))
                    {
                        return false;
                    }

                    break;
                case CraftResource.AshWood:
                    if (index == 8 || index == 10)
                    {
                        return false;
                    }

                    if (index >= 8 && index <= 10 && HasOption(options, ReforgingOption.PowerfulAndStructural))
                    {
                        return false;
                    }

                    if (index >= 8 && index <= 11 && HasOption(
                            options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental,
                            ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.YewWood:
                    if (index >= 8 && index <= 11)
                    {
                        return false;
                    }

                    break;
                case CraftResource.Heartwood:
                    if (index >= 8 && index <= 11)
                    {
                        return false;
                    }

                    if (index == 12 && HasOption(
                            options, ReforgingOption.StructuralAndFundamental,
                            ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
            }
        }
        else if (type == ReforgingItemType.Shield)
        {
            if (index == 10)
            {
                return false;
            }

            switch (tool.Resource)
            {
                case CraftResource.DullCopper:
                    if (index == 8 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.ShadowIron:
                    if (index == 8 && HasOption(
                            options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    if ((index == 8 || index == 9) &&
                        HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Copper:
                    if (index == 8 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental
                        ))
                    {
                        return false;
                    }
                    else if ((index == 8 || index == 9) &&
                             HasOption(options, ReforgingOption.StructuralAndFundamental))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 8 || index == 9) &&
                             HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Bronze:
                    if (index == 8)
                    {
                        return false;
                    }
                    else if (index == 9 && HasOption(options, ReforgingOption.PowerfulAndStructural))
                    {
                        return false;
                    }
                    else if ((index == 9 || index == 5) &&
                             HasOption(options, ReforgingOption.PowerfulAndFundamental))
                    {
                        return false;
                    }
                    else if ((index == 9 || index == 5 || index == 11) && HasOption(
                            options, ReforgingOption.StructuralAndFundamental,
                            ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Gold:
                    if (index == 8)
                    {
                        return false;
                    }
                    else if (index == 9 && HasOption(options, ReforgingOption.Powerful))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 9) && HasOption(options, ReforgingOption.PowerfulAndStructural))
                    {
                        return false;
                    }
                    else if ((index == 9 || index == 5 || index == 11) && HasOption(
                            options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental,
                            ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Agapite:
                    if (index == 8 || index == 9)
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 11) && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Verite:
                case CraftResource.Valorite:
                    if (index == 8 || index == 9 || index == 11)
                    {
                        return false;
                    }
                    else if (index == 5 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;

                case CraftResource.OakWood:
                    if (index == 8 && HasOption(
                            options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    if ((index == 8 || index == 9) &&
                        HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.AshWood:
                    if (index == 8)
                    {
                        return false;
                    }
                    else if (index == 9 && HasOption(options, ReforgingOption.PowerfulAndStructural))
                    {
                        return false;
                    }
                    else if ((index == 9 || index == 5) &&
                             HasOption(options, ReforgingOption.PowerfulAndFundamental))
                    {
                        return false;
                    }
                    else if ((index == 9 || index == 5 || index == 11) && HasOption(
                            options, ReforgingOption.StructuralAndFundamental,
                            ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.YewWood:
                    if (index == 8 || index == 9)
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 11) && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Heartwood:
                    if (index == 8 || index == 9 || index == 11)
                    {
                        return false;
                    }
                    else if (index == 5 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
            }
        }
        else if (type == ReforgingItemType.Armor)
        {
            switch (tool.Resource)
            {
                case CraftResource.DullCopper:
                    if ((index == 10 || index == 11) &&
                        HasOption(options, ReforgingOption.PowerfulAndFundamental))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 10 || index == 11) && HasOption(
                            options, ReforgingOption.StructuralAndFundamental,
                            ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.ShadowIron:
                    if ((index == 10 || index == 11) &&
                        HasOption(options, ReforgingOption.PowerfulAndStructural))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 10 || index == 11) && HasOption(
                            options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental
                        ))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 9 || index == 10 || index == 11) &&
                             HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Copper:
                    if (index == 10 || index == 11)
                    {
                        return false;
                    }
                    else if (index == 5 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental
                        ))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 9) &&
                             HasOption(options, ReforgingOption.StructuralAndFundamental))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 9 || index == 5) &&
                             HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Bronze:
                    if (index == 10 || index == 11)
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 9) && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental
                        ))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 9 || index == 12) && HasOption(
                            options, ReforgingOption.StructuralAndFundamental,
                            ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Gold:
                    if (index == 10 || index == 11)
                    {
                        return false;
                    }
                    else if (index == 9 && HasOption(options, ReforgingOption.Powerful))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 9 || index == 12) && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Agapite:
                    if (index >= 9 && index <= 11)
                    {
                        return false;
                    }
                    else if (index == 12 && HasOption(options, ReforgingOption.Powerful))
                    {
                        return false;
                    }
                    else if ((index == 12 || index == 5) && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Verite:
                case CraftResource.Valorite:
                    if (index >= 9 && index <= 12)
                    {
                        return false;
                    }
                    else if (index == 5 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;

                case CraftResource.SpinedLeather:
                    if ((index == 10 || index == 11) &&
                        HasOption(options, ReforgingOption.PowerfulAndStructural))
                    {
                        return false;
                    }
                    else if ((index == 10 || index == 11 || index == 5) && HasOption(
                            options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental
                        ))
                    {
                        return false;
                    }
                    else if ((index == 9 || index == 10 || index == 11 || index == 5) &&
                             HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.HornedLeather:
                    if (index == 10 || index == 11)
                    {
                        return false;
                    }
                    else if (index == 9 && HasOption(options, ReforgingOption.Powerful))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 9 || index == 12) && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.BarbedLeather:
                    if (index >= 9 && index <= 12)
                    {
                        return false;
                    }
                    else if (index == 5 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;

                case CraftResource.OakWood:
                    if ((index == 10 || index == 11) && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental
                        ))
                    {
                        return false;
                    }
                    else if (index == 9 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                    {
                        return false;
                    }

                    break;
                case CraftResource.AshWood:
                    if (index == 10 || index == 11)
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 9) && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental
                        ))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 9 || index == 12) && HasOption(
                            options, ReforgingOption.StructuralAndFundamental,
                            ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.YewWood:
                    if (index == 9 || index == 10 || index == 11)
                    {
                        return false;
                    }
                    else if (index == 12 && HasOption(options, ReforgingOption.Powerful))
                    {
                        return false;
                    }
                    else if ((index == 5 || index == 12) && HasOption(
                            options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
                case CraftResource.Heartwood:
                    if (index >= 9 && index <= 12)
                    {
                        return false;
                    }
                    else if (index == 5 && HasOption(
                            options, ReforgingOption.PowerfulAndStructural,
                            ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental,
                            ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental
                        ))
                    {
                        return false;
                    }

                    break;
            }
        }

        return true;
    }

    public static bool HasOption(ReforgingOption options, params ReforgingOption[] optionArray)
    {
        foreach (var option in optionArray)
        {
            if ((options & option) == option)
            {
                return true;
            }
        }

        return false;
    }

    private static void CheckAttributes(Item item, List<NamedInfoCol> list, bool reforged)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            var col = list[i];

            if (col != null && !ReforgingPropertyInfo.ValidateProperty(item, col.Attribute, reforged))
            {
                list.RemoveAt(i);
            }
        }
    }

    private static void GetNamedModCount(
        int itemIndex, int prefixID, int suffixID, int maxmods, int precolcount, int suffixcolcount,
        ref int prefixCount, ref int suffixCount
    )
    {
        if (prefixID > 0 && suffixID > 0)
        {
            if (0.5 > Utility.RandomDouble())
            {
                // Even Split
                if (0.5 > Utility.RandomDouble())
                {
                    prefixCount = maxmods / 2;
                    suffixCount = maxmods - prefixCount;
                }
                else
                {
                    suffixCount = maxmods / 2;
                    prefixCount = maxmods - suffixCount;
                }
            }
            else if (0.5 > Utility.RandomDouble())
            {
                prefixCount = (maxmods / 2) - 1;
                suffixCount = maxmods - prefixCount;
            }
            else
            {
                suffixCount = (maxmods / 2) - 1;
                prefixCount = maxmods - suffixCount;
            }
        }
        else
        {
            int mods = 0;

            switch (maxmods)
            {
                default:
                case 8:
                case 7:
                    mods = maxmods / 2;
                    break;
                case 6:
                case 5:
                case 4:
                    mods = Utility.RandomBool() ? 2 : 3;
                    break;
                case 3:
                    mods = Utility.RandomBool() ? 1 : 2;
                    break;
                case 2:
                case 1:
                    mods = 1;
                    break;
            }

            if (prefixID > 0)
            {
                prefixCount = mods;
            }
            else
            {
                suffixCount = mods;
            }
        }

        if (prefixCount > precolcount)
        {
            prefixCount = precolcount;
        }

        if (suffixCount > suffixcolcount)
        {
            suffixCount = suffixcolcount;
        }
    }

    public static int GetPropertyCount(BaseRunicTool tool)
    {
        switch (tool.Resource)
        {
            case CraftResource.DullCopper:
            case CraftResource.ShadowIron: return Utility.RandomMinMax(1, 2);
            case CraftResource.Copper: return Utility.RandomMinMax(2, 3);
            case CraftResource.Bronze:
            case CraftResource.Gold: return 3;
            case CraftResource.Agapite:
            case CraftResource.Verite: return Utility.RandomMinMax(3, 4);
            case CraftResource.Valorite: return 5;

            case CraftResource.SpinedLeather: return Utility.RandomMinMax(1, 2);
            case CraftResource.HornedLeather: return 3;
            case CraftResource.BarbedLeather: return 5;

            case CraftResource.OakWood: return Utility.RandomMinMax(1, 2);
            case CraftResource.AshWood: return 2;
            case CraftResource.YewWood: return 3;
            case CraftResource.Heartwood: return 5;
        }

        return 1;
    }

    private static bool ApplyPrefixSuffixAttribute(
        Item item, NamedInfoCol col, int resIndex, int preIndex, int percLow, int percHigh, ref int budget,
        int luckchance, bool reforged, bool powerful
    )
    {
        int start = budget;
        object attribute = col.Attribute;

        // Converts Collection entry into actual attribute
        if (attribute is string)
        {
            switch ((string)attribute)
            {
                case "HitSpell":
                    attribute = GetRandomHitSpell();
                    break;
                case "HitArea":
                    attribute = GetRandomHitArea();
                    break;
                case "Slayer":
                    attribute = BaseRunicTool.GetRandomSlayer();
                    break;
                case "WeaponVelocity":
                    break;
                case "ElementalDamage":
                    attribute = GetRandomElemental();
                    break;
            }
        }

        int id = ReforgingPropertyInfo.GetID(attribute);

        // prop is invalid, or the item already has a value for this prop
        if (id == -1 || ReforgingImbuing.GetValueForID(item, id) > 0 ||
            !ReforgingPropertyInfo.ValidateProperty(item, id, reforged))
        {
            return false;
        }

        if (reforged)
        {
            ApplyReforgedNameProperty(
                item, id, col, resIndex, preIndex, 0, 100, ref budget, luckchance, reforged, powerful
            );
        }
        else
        {
            ApplyProperty(item, id, percLow, percHigh, ref budget, luckchance, reforged, powerful); // TODO: powerful
        }

        return start != budget;
    }

    private static readonly Dictionary<Item, int[]> _elements = new();

    public static bool ApplyResistance(Item item, int value, AosElementAttribute attribute)
    {
        var resists = GetAosElementAttributes(item);

        if (!_elements.ContainsKey(item))
        {
            if (item is BaseArmor armor)
            {
                _elements[item] = new[]
                {
                    armor.PhysicalBonus, armor.FireBonus, armor.ColdBonus, armor.PoisonBonus, armor.EnergyBonus
                };
            }
            else if (item is BaseWeapon weapon)
            {
                _elements[item] = new[]
                {
                    weapon.WeaponAttributes.ResistPhysicalBonus, weapon.WeaponAttributes.ResistFireBonus,
                    weapon.WeaponAttributes.ResistColdBonus, weapon.WeaponAttributes.ResistPoisonBonus,
                    weapon.WeaponAttributes.ResistEnergyBonus
                };
            }
            else if (resists != null)
            {
                _elements[item] = new[]
                {
                    resists[AosElementAttribute.Physical], resists[AosElementAttribute.Fire],
                    resists[AosElementAttribute.Cold], resists[AosElementAttribute.Poison],
                    resists[AosElementAttribute.Energy]
                };
            }
            else
            {
                return false;
            }
        }

        switch (attribute)
        {
            default:
            case AosElementAttribute.Physical:
                if (item is BaseArmor physicalArmor &&
                    (!_elements.ContainsKey(item) || physicalArmor.PhysicalBonus == _elements[item][0]))
                {
                    physicalArmor.PhysicalBonus = value;
                    return true;
                }
                else if (item is BaseWeapon physicalWeapon &&
                         (!_elements.ContainsKey(item) ||
                             physicalWeapon.WeaponAttributes.ResistPhysicalBonus == _elements[item][0]))
                {
                    physicalWeapon.WeaponAttributes.ResistPhysicalBonus = value;
                    return true;
                }
                else if (resists != null && (!_elements.ContainsKey(item) || resists[attribute] == _elements[item][0]))
                {
                    resists[attribute] = value;
                    return true;
                }

                break;
            case AosElementAttribute.Fire:
                if (item is BaseArmor fireArmor &&
                    (!_elements.ContainsKey(item) || fireArmor.FireBonus == _elements[item][1]))
                {
                    fireArmor.FireBonus = value;
                    return true;
                }
                else if (item is BaseWeapon fireWeapon &&
                         (!_elements.ContainsKey(item) ||
                             fireWeapon.WeaponAttributes.ResistFireBonus == _elements[item][1]))
                {
                    fireWeapon.WeaponAttributes.ResistFireBonus = value;
                    return true;
                }
                else if (resists != null && (!_elements.ContainsKey(item) || resists[attribute] == _elements[item][1]))
                {
                    resists[attribute] = value;
                    return true;
                }

                break;
            case AosElementAttribute.Cold:
                if (item is BaseArmor coldArmor &&
                    (!_elements.ContainsKey(item) || coldArmor.ColdBonus == _elements[item][2]))
                {
                    coldArmor.ColdBonus = value;
                    return true;
                }
                else if (item is BaseWeapon coldWeapon &&
                         (!_elements.ContainsKey(item) ||
                             coldWeapon.WeaponAttributes.ResistColdBonus == _elements[item][2]))
                {
                    coldWeapon.WeaponAttributes.ResistColdBonus = value;
                    return true;
                }
                else if (resists != null && (!_elements.ContainsKey(item) || resists[attribute] == _elements[item][2]))
                {
                    resists[attribute] = value;
                    return true;
                }

                break;
            case AosElementAttribute.Poison:
                if (item is BaseArmor poisonArmor &&
                    (!_elements.ContainsKey(item) || poisonArmor.PoisonBonus == _elements[item][3]))
                {
                    poisonArmor.PoisonBonus = value;
                    return true;
                }
                else if (item is BaseWeapon poisonWeapon &&
                         (!_elements.ContainsKey(item) ||
                             poisonWeapon.WeaponAttributes.ResistPoisonBonus == _elements[item][3]))
                {
                    poisonWeapon.WeaponAttributes.ResistPoisonBonus = value;
                    return true;
                }
                else if (resists != null && (!_elements.ContainsKey(item) || resists[attribute] == _elements[item][3]))
                {
                    resists[attribute] = value;
                    return true;
                }

                break;
            case AosElementAttribute.Energy:
                if (item is BaseArmor energyArmor &&
                    (!_elements.ContainsKey(item) || energyArmor.EnergyBonus == _elements[item][4]))
                {
                    energyArmor.EnergyBonus = value;
                    return true;
                }
                else if (item is BaseWeapon energyWeapon &&
                         (!_elements.ContainsKey(item) ||
                             energyWeapon.WeaponAttributes.ResistEnergyBonus == _elements[item][4]))
                {
                    energyWeapon.WeaponAttributes.ResistEnergyBonus = value;
                    return true;
                }
                else if (resists != null && (!_elements.ContainsKey(item) || resists[attribute] == _elements[item][4]))
                {
                    resists[attribute] = value;
                    return true;
                }

                break;
        }

        return false;
    }

    public static int Scale(int min, int max, int perclow, int perchigh, int luckchance, bool reforged)
    {
        int percent;

        if (reforged)
        {
            percent = Utility.RandomMinMax(perclow, perchigh);
        }
        else
        {
            percent = Utility.RandomMinMax(0, perchigh);
        }

        if (LootPack.CheckLuck(luckchance))
        {
            percent += 10;
        }

        if (percent < perclow)
        {
            percent = perclow;
        }

        if (percent > perchigh)
        {
            percent = perchigh;
        }

        int scaledBy = Math.Abs(min - max) + 1;

        if (scaledBy != 0)
        {
            scaledBy = 10000 / scaledBy;
        }

        percent *= (10000 + scaledBy);

        return min + (((max - min) * percent) / 1000001);
    }

    private static int CalculateValue(
        Item item, object attribute, int min, int max, int perclow, int perchigh, ref int budget, int luckchance
    )
    {
        return CalculateValue(item, attribute, min, max, perclow, perchigh, ref budget, luckchance, false);
    }

    private static int CalculateValue(
        Item item, object attribute, int min, int max, int perclow, int perchigh, ref int budget, int luckchance,
        bool reforged
    )
    {
        int scale = Math.Max(1, ReforgingPropertyInfo.GetScale(item, attribute));

        if (scale > 0 && min < scale)
        {
            min = scale;
        }

        int value = Scale(min, max, perclow, perchigh, luckchance, reforged);

        if (scale > 1 && value > scale)
        {
            value = (value / scale) * scale;
        }

        int totalweight = ReforgingPropertyInfo.GetTotalWeight(item, attribute, value);

        while (budget <= totalweight)
        {
            value -= scale;

            if (value <= 0)
            {
                if (ReforgingPropertyInfo.GetTotalWeight(item, attribute, 3) > budget)
                {
                    budget = 0;
                }

                return 0;
            }

            totalweight = ReforgingPropertyInfo.GetTotalWeight(item, attribute, value);
        }

        return value;
    }

    private static int GetTotalMods(Item item)
    {
        return ReforgingImbuing.GetTotalMods(item);
    }

    private static ItemPropertyInfo GetItemProps(object attr)
    {
        int id = -1;

        if (attr is AosAttribute aosAttribute)
        {
            id = ReforgingPropertyInfo.GetIDForAttribute(aosAttribute);
        }
        else if (attr is AosWeaponAttribute aosWeaponAttribute)
        {
            id = ReforgingPropertyInfo.GetIDForAttribute(aosWeaponAttribute);
        }
        else if (attr is SkillName skillName)
        {
            id = ReforgingPropertyInfo.GetIDForAttribute(skillName);
        }
        else if (attr is SlayerName slayerName)
        {
            id = ReforgingPropertyInfo.GetIDForAttribute(slayerName);
        }
        else if (attr is AosArmorAttribute aosArmorAttribute)
        {
            id = ReforgingPropertyInfo.GetIDForAttribute(aosArmorAttribute);
        }
        else if (attr is AosElementAttribute aosElementAttribute)
        {
            id = ReforgingPropertyInfo.GetIDForAttribute(aosElementAttribute);
        }

        if (ItemPropertyInfo.Table.ContainsKey(id))
        {
            return ItemPropertyInfo.Table[id];
        }

        return null;
    }

    private static int GetCollectionIndex(Item item)
    {
        if (item is BaseWeapon)
        {
            return 0;
        }

        if (item is BaseShield)
        {
            return 2;
        }

        if (item is BaseArmor || item is BaseClothing)
        {
            return 1;
        }

        if (item is BaseJewel)
        {
            return 3;
        }

        return -1;
    }

    private static int GetResourceIndex(CraftResource resource)
    {
        // RunicIndex 0 - dullcopper; 1 - shadow; 2 - copper; 3 - spined; 4 - Oak; 5 - ash
        switch (resource)
        {
            default:
            case CraftResource.DullCopper: return 0;
            case CraftResource.ShadowIron: return 1;
            case CraftResource.Bronze:
            case CraftResource.Gold:
            case CraftResource.Agapite:
            case CraftResource.Verite:
            case CraftResource.Valorite:
            case CraftResource.Copper: return 2;
            case CraftResource.SpinedLeather: return 3;
            case CraftResource.OakWood: return 4;
            case CraftResource.YewWood:
            case CraftResource.Heartwood:
            case CraftResource.Bloodwood:
            case CraftResource.Frostwood:
            case CraftResource.HornedLeather:
            case CraftResource.BarbedLeather:
            case CraftResource.AshWood: return 5;
        }
    }

    private static int GetPrerequisiteIndex(ReforgingOption option)
    {
        if ((option & ReforgingOption.Powerful) != 0 &&
            (option & ReforgingOption.Structural) != 0 &&
            (option & ReforgingOption.Fundamental) != 0)
        {
            return 6;
        }

        if ((option & ReforgingOption.Structural) != 0 &&
            (option & ReforgingOption.Fundamental) != 0)
        {
            return 5;
        }

        if ((option & ReforgingOption.Powerful) != 0 &&
            (option & ReforgingOption.Structural) != 0)
        {
            return 4;
        }

        if ((option & ReforgingOption.Fundamental) != 0)
        {
            return 3;
        }

        if ((option & ReforgingOption.Structural) != 0)
        {
            return 2;
        }

        if ((option & ReforgingOption.Powerful) != 0)
        {
            return 1;
        }

        return 0;
    }

    private static int CalculateMinIntensity(int perclow, int perchi, ReforgingOption option)
    {
        if (option == ReforgingOption.None)
        {
            return perclow;
        }

        return perclow + (int)((double)(perchi - perclow) * ((double)(GetPrerequisiteIndex(option) * 5.0) / 100.0));
    }

    public static void ApplyPrefixName(Item item, ReforgedPrefix prefix)
    {
        if (item is BaseWeapon weapon)
        {
            weapon.ReforgedPrefix = prefix;
        }
        else if (item is BaseShield shield)
        {
            shield.ReforgedPrefix = prefix;
        }
        else if (item is BaseArmor armor)
        {
            armor.ReforgedPrefix = prefix;
        }
        else if (item is BaseJewel jewel)
        {
            jewel.ReforgedPrefix = prefix;
        }
        else if (item is BaseClothing clothing)
        {
            clothing.ReforgedPrefix = prefix;
        }
    }

    public static void ApplySuffixName(Item item, ReforgedSuffix suffix)
    {
        if (item is BaseWeapon weapon)
        {
            weapon.ReforgedSuffix = suffix;
        }
        else if (item is BaseShield shield)
        {
            shield.ReforgedSuffix = suffix;
        }
        else if (item is BaseArmor armor)
        {
            armor.ReforgedSuffix = suffix;
        }
        else if (item is BaseJewel jewel)
        {
            jewel.ReforgedSuffix = suffix;
        }
        else if (item is BaseClothing clothing)
        {
            clothing.ReforgedSuffix = suffix;
        }
    }

    public static Item GenerateRandomItem(IEntity e)
    {
        Item item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();

        if (item != null)
        {
            GenerateRandomItem(item, null, Utility.RandomMinMax(100, 700), 0, ReforgedPrefix.None, ReforgedSuffix.None);
        }

        return item;
    }

    /// <summary>
    /// This can be called from lootpack once loot pack conversions are implemented (if need be)
    /// </summary>
    /// <param name="item">item to mutate</param>
    /// <param name="luck">adjust luck chance</param>
    /// <param name="minBudget"></param>
    /// <param name="maxBudget"></param>
    /// <returns></returns>
    public static bool GenerateRandomItem(Item item, int luck, int minBudget, int maxBudget)
    {
        if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
        {
            int budget = Utility.RandomMinMax(minBudget, maxBudget);
            GenerateRandomItem(item, null, budget, luck, ReforgedPrefix.None, ReforgedSuffix.None);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Called in DemonKnight.cs for forcing rad items
    /// </summary>
    /// <param name="item"></param>
    /// <param name="luck">raw luck</param>
    /// <param name="artifact"></param>
    /// <returns></returns>
    public static bool GenerateRandomArtifactItem(
        Item item, int luck, int budget, ReforgedPrefix prefix = ReforgedPrefix.None,
        ReforgedSuffix suffix = ReforgedSuffix.None
    )
    {
        if (prefix == ReforgedPrefix.None)
        {
            prefix = ChooseRandomPrefix(item, budget);
        }

        if (suffix == ReforgedSuffix.None)
        {
            suffix = ChooseRandomSuffix(item, budget);
        }

        if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
        {
            GenerateRandomItem(item, null, budget, GetLuckChance(luck), prefix, suffix, artifact: true);
            return true;
        }

        return false;
    }

    public static Item GenerateRandomItem(Mobile killer, BaseCreature creature)
    {
        Item item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();

        if (item != null)
        {
            GenerateRandomItem(
                item, killer, Math.Max(100, GetDifficultyFor(creature)), LootPack.GetLuckChanceForKiller(creature),
                ReforgedPrefix.None, ReforgedSuffix.None
            );
        }

        return item;
    }

    /// <summary>
    /// Called in LootPack.cs
    /// </summary>
    public static bool GenerateRandomItem(Item item, Mobile killer, BaseCreature creature)
    {
        if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
        {
            GenerateRandomItem(
                item, killer, Math.Max(100, GetDifficultyFor(creature)), LootPack.GetLuckChanceForKiller(creature),
                ReforgedPrefix.None, ReforgedSuffix.None
            );
            return true;
        }

        return false;
    }

    public static bool GenerateRandomItem(
        Item item, Mobile killer, BaseCreature creature, ReforgedPrefix prefix, ReforgedSuffix suffix
    )
    {
        if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
        {
            GenerateRandomItem(
                item, killer, Math.Max(100, GetDifficultyFor(creature)), LootPack.GetLuckChanceForKiller(creature),
                prefix, suffix
            );
            return true;
        }

        return false;
    }

    /// <summary>
    /// Called from TreasureMapChest.cs
    /// </summary>
    /// <param name="item">item to mutate</param>
    /// <param name="luck">raw luck</param>
    /// <param name="minBudget"></param>
    /// <param name="maxBudget"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    public static bool GenerateRandomItem(Item item, int luck, int minBudget, int maxBudget, Map map)
    {
        if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
        {
            int budget = Utility.RandomMinMax(minBudget, maxBudget);
            GenerateRandomItem(
                item, null, budget, GetLuckChance(luck), ReforgedPrefix.None, ReforgedSuffix.None, map
            );
            return true;
        }

        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="item">item to mutate</param>
    /// <param name="killer">who killed the monster, if applicable</param>
    /// <param name="basebudget">where to we start, regarding the difficulty of the monster we killed</param>
    /// <param name="luckchance">adjusted luck</param>
    /// <param name="forcedprefix"></param>
    /// <param name="forcedsuffix"></param>
    /// <param name="map"></param>
    public static void GenerateRandomItem(
        Item item, Mobile killer, int basebudget, int luckchance, ReforgedPrefix forcedprefix,
        ReforgedSuffix forcedsuffix, Map map = null, bool artifact = false
    )
    {
        if (map == null && killer != null)
        {
            map = killer.Map;
        }

        if (item != null)
        {
            int budget = basebudget;
            int rawLuck = killer?.Luck ?? 0;

            int mods = 0;
            int perclow = 0;
            int perchigh = 0;

            ReforgedPrefix prefix = forcedprefix;
            ReforgedSuffix suffix = forcedsuffix;

            if (artifact)
            {
                ChooseArtifactMods(item, budget, out mods, out perclow, out perchigh);
            }
            else
            {
                int budgetBonus = 0;

                if (killer != null)
                {
                    if (map != null && map.Rules == MapRules.FeluccaRules)
                    {
                        budgetBonus = RandomItemGenerator.FeluccaBudgetBonus;
                    }
                }

                int divisor = GetDivisor(basebudget);

                double perc = 0.0;
                double highest = 0.0;

                for (int i = 0; i < 1 + (rawLuck / 600); i++)
                {
                    perc = (100.0 - Math.Sqrt(Utility.RandomMinMax(0, 10000))) / 100.0;

                    if (perc > highest)
                    {
                        highest = perc;
                    }
                }

                perc = highest;

                if (perc > 1.0)
                {
                    perc = 1.0;
                }

                int toAdd = Math.Min(500, RandomItemGenerator.MaxAdjustedBudget - basebudget);

                budget = Utility.RandomMinMax(basebudget - (basebudget / divisor), (int)(basebudget + (toAdd * perc))) +
                         budgetBonus;

                // Gives a rare chance for a high end item to drop on a low budgeted monster
                if (rawLuck > 0 && !IsPowerful(budget) && LootPack.CheckLuck(luckchance / 6))
                {
                    budget = Utility.RandomMinMax(600, 1150);
                }

                budget = Math.Min(RandomItemGenerator.MaxAdjustedBudget, budget);
                budget = Math.Max(RandomItemGenerator.MinAdjustedBudget, budget);

                if (!(item is BaseWeapon) && prefix == ReforgedPrefix.Vampiric)
                {
                    prefix = ReforgedPrefix.None;
                }

                if (!(item is BaseWeapon) && suffix == ReforgedSuffix.Vampire)
                {
                    suffix = ReforgedSuffix.None;
                }

                if (forcedprefix == ReforgedPrefix.None && budget >= Utility.Random(2700) &&
                    suffix < ReforgedSuffix.Minax)
                {
                    prefix = ChooseRandomPrefix(item, budget);
                }

                if (forcedsuffix == ReforgedSuffix.None && budget >= Utility.Random(2700))
                {
                    suffix = ChooseRandomSuffix(item, budget, prefix);
                }

                if (!IsPowerful(budget))
                {
                    mods = Math.Max(1, GetProperties(5));

                    perchigh = Math.Max(50, Math.Min(500, budget) / mods);
                    perclow = Math.Max(20, perchigh / 3);
                }
                else
                {
                    int maxmods = Math.Max(
                        5,
                        Math.Min(
                            RandomItemGenerator.MaxProps - 1,
                            (int)Math.Ceiling((double)budget / (double)Utility.RandomMinMax(100, 140))
                        )
                    );
                    int minmods = Math.Max(4, maxmods - 4);

                    mods = Math.Max(minmods, GetProperties(maxmods));

                    perchigh = 100;
                    perclow = Utility.RandomMinMax(50, 70);
                }

                if (perchigh > 100)
                {
                    perchigh = 100;
                }

                if (perclow < 10)
                {
                    perclow = 10;
                }

                if (perclow > 80)
                {
                    perclow = 80;
                }
            }

            if (mods < RandomItemGenerator.MaxProps - 1 && LootPack.CheckLuck(luckchance))
            {
                mods++;
            }

            var props = new List<int>(ReforgingPropertyInfo.LookupLootTable(item));
            bool powerful = IsPowerful(budget);

            ApplyReforgedProperties(item, props, prefix, suffix, budget, perclow, perchigh, mods, luckchance);

            var power = ApplyItemPower(item, false);

            if (artifact && power < ItemPower.LesserArtifact)
            {
                int extra = 5000;

                do
                {
                    ApplyRandomProperty(item, props, perclow, perchigh, ref extra, luckchance, false, powerful);
                }
                while (ApplyItemPower(item, false) < ItemPower.LesserArtifact);
            }

            // hues
            if (power == ItemPower.LegendaryArtifact && (item is BaseArmor || item is BaseClothing))
            {
                item.Hue = 2500;
            }

            switch (suffix)
            {
                case ReforgedSuffix.Minax:
                    item.Hue = 1157;
                    break;
                case ReforgedSuffix.Khaldun:
                    item.Hue = 2745;
                    break;
                case ReforgedSuffix.Kotl:
                    item.Hue = 2591;
                    break;
                case ReforgedSuffix.EnchantedOrigin:
                    item.Hue = 1171;
                    break;
                case ReforgedSuffix.Doom:
                    item.Hue = 2301;
                    break;
            }
        }
    }

    private static bool IsPowerful(int budget)
    {
        return budget >= 550;
    }

    public static int GetProperties(int max)
    {
        if (max > RandomItemGenerator.MaxProps - 1)
        {
            max = RandomItemGenerator.MaxProps - 1;
        }

        int p0 = 0, p1 = 0, p2 = 0, p3 = 0, p4 = 0, p5 = 0, p6 = 0, p7 = 0, p8 = 0, p9 = 0, p10 = 0, p11 = 0;

        switch (max)
        {
            case 1:
                p0 = 3;
                p1 = 1;
                break;
            case 2:
                p0 = 6;
                p1 = 3;
                p2 = 1;
                break;
            case 3:
                p0 = 10;
                p1 = 6;
                p2 = 3;
                p3 = 1;
                break;
            case 4:
                p0 = 16;
                p1 = 12;
                p2 = 6;
                p3 = 5;
                p4 = 1;
                break;
            case 5:
                p0 = 30;
                p1 = 25;
                p2 = 20;
                p3 = 15;
                p4 = 9;
                p5 = 1;
                break;
            case 6:
                p0 = 35;
                p1 = 30;
                p2 = 25;
                p3 = 20;
                p4 = 15;
                p5 = 9;
                p6 = 1;
                break;
            case 7:
                p0 = 40;
                p1 = 35;
                p2 = 30;
                p3 = 25;
                p4 = 20;
                p5 = 15;
                p6 = 9;
                p7 = 1;
                break;
            case 8:
                p0 = 50;
                p1 = 40;
                p2 = 35;
                p3 = 30;
                p4 = 25;
                p5 = 20;
                p6 = 15;
                p7 = 9;
                p8 = 1;
                break;
            case 9:
                p0 = 70;
                p1 = 55;
                p2 = 45;
                p3 = 35;
                p4 = 30;
                p5 = 25;
                p6 = 20;
                p7 = 15;
                p8 = 9;
                p9 = 1;
                break;
            case 10:
                p0 = 90;
                p1 = 70;
                p2 = 55;
                p3 = 45;
                p4 = 40;
                p5 = 35;
                p6 = 25;
                p7 = 15;
                p8 = 10;
                p9 = 5;
                p10 = 1;
                break;
        }

        int pc = p0 + p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11;

        int rnd = Utility.Random(pc);

        if (rnd < p10)
        {
            return 10;
        }

        rnd -= p10;

        if (rnd < p9)
        {
            return 9;
        }

        rnd -= p9;

        if (rnd < p8)
        {
            return 8;
        }

        rnd -= p8;

        if (rnd < p7)
        {
            return 7;
        }

        rnd -= p7;

        if (rnd < p6)
        {
            return 6;
        }

        rnd -= p6;

        if (rnd < p5)
        {
            return 5;
        }

        rnd -= p5;

        if (rnd < p4)
        {
            return 4;
        }

        rnd -= p4;

        if (rnd < p3)
        {
            return 3;
        }

        rnd -= p3;

        if (rnd < p2)
        {
            return 2;
        }

        rnd -= p2;

        if (rnd < p1)
        {
            return 1;
        }

        return 0;
    }

    private static void ChooseArtifactMods(Item item, int budget, out int mods, out int perclow, out int perchigh)
    {
        int maxmods = Math.Min(10, budget / 120);
        mods = Utility.RandomMinMax(6, maxmods);

        perchigh = 100;
        perclow = item is BaseShield ? 100 : 80;
    }

    private static int GetDivisor(int basebudget)
    {
        if (basebudget < 400)
        {
            return 5;
        }

        if (basebudget < 550)
        {
            return 4;
        }

        if (basebudget < 650)
        {
            return 3;
        }

        return 2;
    }

    public static int GetLuckForKiller(BaseCreature dead)
    {
        return dead.LastKiller?.Luck ?? 0;
    }

    private static int GetLuckChance(int luck)
    {
        return (int)(Math.Pow(luck, 1 / 1.8) * 100);
    }

    private static readonly Dictionary<int, int> _standard = new()
    {
        { 1, 10 },
        { 2, 10 },
        { 3, 10 },
        { 4, 10 },
        { 5, 10 },
        { 7, 10 },
        { 8, 10 },
        { 9, 2 },
        { 10, 2 },
        { 11, 5 },
        { 12, 5 }
    };

    private static readonly Dictionary<int, int> _standardPowerful = new()
    {
        { 1, 10 },
        { 2, 10 },
        { 3, 10 },
        { 4, 10 },
        { 5, 10 },
        { 7, 10 },
        { 8, 10 },
        { 9, 0 },
        { 10, 0 },
        { 11, 2 },
        { 12, 2 }
    };

    private static readonly Dictionary<int, int> _weapon = new()
    {
        { 1, 10 },
        { 2, 10 },
        { 3, 10 },
        { 4, 10 },
        { 5, 10 },
        { 6, 10 },
        { 7, 10 },
        { 8, 10 },
        { 9, 2 },
        { 10, 2 },
        { 11, 5 },
        { 12, 5 }
    };

    private static readonly Dictionary<int, int> _weaponPowerful = new()
    {
        { 1, 10 },
        { 2, 10 },
        { 3, 10 },
        { 4, 10 },
        { 5, 10 },
        { 6, 10 },
        { 7, 10 },
        { 8, 10 },
        { 9, 0 },
        { 10, 0 },
        { 11, 2 },
        { 12, 2 }
    };

    public static ReforgedPrefix ChooseRandomPrefix(Item item, int budget)
    {
        return ChooseRandomPrefix(item, budget, ReforgedSuffix.None);
    }

    public static ReforgedPrefix ChooseRandomPrefix(Item item, int budget, ReforgedSuffix suffix)
    {
        Dictionary<int, int> table;
        bool powerful = budget > 600;

        if (item is BaseWeapon)
        {
            table = powerful ? _weaponPowerful : _weapon;
        }
        else
        {
            table = powerful ? _standardPowerful : _standard;
        }

        int random = GetRandomName(table);

        while ((int)suffix != 0 && random == (int)suffix)
        {
            random = GetRandomName(table);
        }

        return (ReforgedPrefix)random;
    }

    public static ReforgedSuffix ChooseRandomSuffix(Item item, int budget)
    {
        return ChooseRandomSuffix(item, budget, ReforgedPrefix.None);
    }

    public static ReforgedSuffix ChooseRandomSuffix(Item item, int budget, ReforgedPrefix prefix)
    {
        Dictionary<int, int> table;
        bool powerful = budget > 600;

        if (item is BaseWeapon)
        {
            table = powerful ? _weaponPowerful : _weapon;
        }
        else
        {
            table = powerful ? _standardPowerful : _standard;
        }

        int random = GetRandomName(table);

        while ((int)prefix != 0 && random == (int)prefix)
        {
            random = GetRandomName(table);
        }

        return (ReforgedSuffix)random;
    }

    private static int GetRandomName(Dictionary<int, int> table)
    {
        int total = 0;

        foreach (var kvp in table)
        {
            total += kvp.Value;
        }

        var random = Utility.RandomMinMax(1, total);
        total = 0;

        foreach (var kvp in table)
        {
            total += kvp.Value;

            if (total >= random)
            {
                return kvp.Key;
            }
        }

        return 0;
    }

    public static int GetDifficultyFor(BaseCreature bc)
    {
        return RandomItemGenerator.GetDifficultyFor(bc);
    }

    public static ItemPower ApplyItemPower(Item item, bool reforged)
    {
        ItemPower ip = GetItemPower(
            item, ReforgingImbuing.GetTotalWeight(item, -1, false, false), ReforgingImbuing.GetTotalMods(item), reforged
        );

        if (item is ICombatEquipment equipment)
        {
            equipment.ItemPower = ip;
        }

        return ip;
    }

    public static ItemPower GetItemPower(Item item, int weight, int totalMods, bool reforged)
    {
        // pre-arty uses max imbuing weight + 100
        // arty ranges from pre-arty to a flat 1200
        double preArty = ReforgingImbuing.GetMaxWeight(item) + 100;
        double arty = 1200 - preArty;

        if (totalMods == 0)
        {
            return ItemPower.None;
        }

        if (weight < preArty * .4)
        {
            return reforged ? ItemPower.ReforgedMinor : ItemPower.Minor;
        }

        if (weight < preArty * .6)
        {
            return reforged ? ItemPower.ReforgedLesser : ItemPower.Lesser;
        }

        if (weight < preArty * .8)
        {
            return reforged ? ItemPower.ReforgedGreater : ItemPower.Greater;
        }

        if (weight <= preArty)
        {
            return reforged ? ItemPower.ReforgedGreater : ItemPower.Major;
        }

        if (weight < preArty + (arty * .2))
        {
            return reforged ? ItemPower.ReforgedMajor : ItemPower.LesserArtifact;
        }

        if (weight < preArty + (arty * .4))
        {
            return reforged ? ItemPower.ReforgedMajor : ItemPower.GreaterArtifact;
        }

        if (weight < preArty + (arty * .7) || totalMods <= 5)
        {
            return ItemPower.MajorArtifact;
        }

        return reforged ? ItemPower.ReforgedLegendary : ItemPower.LegendaryArtifact;
    }

    private static bool ApplyRandomProperty(
        Item item, List<int> props, int perclow, int perchigh, ref int budget, int luckchance, bool reforged,
        bool powerful
    )
    {
        if (props == null || props.Count == 0)
        {
            return false;
        }

        int id = -1;

        while (true)
        {
            var random = props[Utility.Random(props.Count)];

            if (random == 1000)
            {
                random = ReforgingPropertyInfo.GetID(BaseRunicTool.GetRandomSlayer());
            }
            else if (random >= 1001 && id <= 1005)
            {
                random = ReforgingPropertyInfo.GetID(GetRandomSkill(item));
            }

            if (ReforgingImbuing.GetValueForID(item, random) == 0 &&
                ReforgingPropertyInfo.ValidateProperty(item, random, reforged))
            {
                id = random;
                break;
            }

            props.Remove(random);

            if (props.Count == 0)
            {
                break;
            }
        }

        if (id == -1)
        {
            return false;
        }

        return ApplyProperty(item, id, perclow, perchigh, ref budget, luckchance, reforged, powerful);
    }

    /// <summary>
    /// unsafe applies property. Checks need to be made prior to calling this, see Imbuing.GetValueForID and ItemPropertyInfo.ValidateProperty
    /// </summary>
    /// <param name="item"></param>
    /// <param name="id"></param>
    /// <param name="perloc"></param>
    /// <param name="perchigh"></param>
    /// <param name="budget"></param>
    /// <param name="luckchance"></param>
    /// <param name="reforged"></param>
    /// <param name="powerful"></param>
    /// <returns></returns>
    public static bool ApplyProperty(
        Item item, int id, int perclow, int perchigh, ref int budget, int luckchance, bool reforged, bool powerful
    )
    {
        int min = ItemPropertyInfo.GetMinIntensity(item, id);
        int naturalMax = ItemPropertyInfo.GetMaxIntensity(item, id, false);
        int max = naturalMax;
        int[] overcap = null;

        if (powerful)
        {
            overcap = ReforgingPropertyInfo.GetMaxOvercappedRange(item, id);

            if (overcap != null)
            {
                max = overcap[overcap.Length - 1];
            }
        }

        int value = CalculateValue(
            item, ReforgingPropertyInfo.GetAttribute(id), min, max, perclow, perchigh, ref budget, luckchance, reforged
        );

        // We're using overcap, so the value must have gone over the natural max, but under the overrcap max
        if (overcap != null && overcap.Length > 0 && value > naturalMax && value < max)
        {
            if (overcap.Length > 1)
            {
                value = AdjustOvercap(overcap, value);
            }
            else
            {
                value = naturalMax;
            }
        }

        ReforgingImbuing.SetProperty(item, id, value);

        budget -= ReforgingImbuing.GetIntensityForID(item, id, -1, value);

        return true;
    }

    public static bool ApplyReforgedNameProperty(
        Item item, int id, NamedInfoCol info, int resIndex, int preIndex, int perclow, int perchigh, ref int budget,
        int luckchance, bool reforged, bool powerful
    )
    {
        int value = info.RandomRangedIntensity(item, id, resIndex, preIndex);

        ReforgingImbuing.SetProperty(item, id, value);

        budget -= ReforgingImbuing.GetIntensityForID(item, id, -1, value);

        return true;
    }

    private static int AdjustOvercap(int[] overcap, int value)
    {
        for (int i = overcap.Length - 1; i >= 0; i--)
        {
            if (value >= overcap[i])
            {
                return overcap[i];
            }
        }

        return overcap[0];
    }

    public static AosAttributes GetAosAttributes(Item item)
    {
        if (item is BaseWeapon weapon)
        {
            return weapon.Attributes;
        }

        if (item is BaseArmor armor)
        {
            return armor.Attributes;
        }

        if (item is BaseClothing clothing)
        {
            return clothing.Attributes;
        }

        if (item is BaseJewel jewel)
        {
            return jewel.Attributes;
        }

        if (item is BaseTalisman talisman)
        {
            return talisman.Attributes;
        }

        if (item is BaseQuiver quiver)
        {
            return quiver.Attributes;
        }

        if (item is Spellbook spellbook)
        {
            return spellbook.Attributes;
        }

        return null;
    }

    public static AosArmorAttributes GetAosArmorAttributes(Item item)
    {
        if (item is BaseArmor armor)
        {
            return armor.ArmorAttributes;
        }

        if (item is BaseClothing clothing)
        {
            return clothing.ClothingAttributes;
        }

        return null;
    }

    public static AosWeaponAttributes GetAosWeaponAttributes(Item item)
    {
        if (item is BaseWeapon weapon)
        {
            return weapon.WeaponAttributes;
        }

        if (item is ElvenGlasses glasses)
        {
            return glasses._weaponAttributes;
        }

        return null;
    }

    public static AosElementAttributes GetAosElementAttributes(Item item)
    {
        if (item is BaseClothing clothing)
        {
            return clothing.Resistances;
        }

        if (item is BaseJewel jewel)
        {
            return jewel.Resistances;
        }

        if (item is BaseWeapon weapon)
        {
            return weapon.AosElementDamages;
        }

        return null;
    }

    public static AosSkillBonuses GetAosSkillBonuses(Item item)
    {
        if (item is BaseJewel jewel)
        {
            return jewel.SkillBonuses;
        }

        if (item is BaseWeapon weapon)
        {
            return weapon.SkillBonuses;
        }

        if (item is BaseArmor armor)
        {
            return armor.SkillBonuses;
        }

        if (item is BaseTalisman talisman)
        {
            return talisman.SkillBonuses;
        }

        if (item is Spellbook spellbook)
        {
            return spellbook.SkillBonuses;
        }

        if (item is BaseClothing clothing)
        {
            return clothing.SkillBonuses;
        }

        return null;
    }

    public static int GetArtifactRarity(Item item) => item switch
    {
        BaseWeapon weapon => weapon.ArtifactRarity,
        BaseArmor armor => armor.ArtifactRarity,
        BaseClothing clothing => clothing.ArtifactRarity,
        BaseJewel jewel => jewel.ArtifactRarity,
        _ => 0
    };

    private static void Reforge_OnCommand(CommandEventArgs e)
    {
        var from = e.Mobile;
        BaseRunicTool tool = null;

        if (from.Backpack != null)
        {
            foreach (var item in from.Backpack.Items)
            {
                if (item is BaseRunicTool runicTool)
                {
                    tool = runicTool;
                    break;
                }
            }
        }

        if (tool == null)
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            return;
        }

        if (!ReforgingToolHook.TryBeginReforging(from, tool))
        {
            from.SendLocalizedMessage(1079787); // You must be near a soulforge to imbue an item.
            return;
        }
    }
}

public class RunicReforgingTarget : Target
{
    private readonly BaseRunicTool _tool;

    public RunicReforgingTarget(BaseRunicTool tool) : base(-1, false, TargetFlags.None)
    {
        _tool = tool;
    }

    protected override void OnTarget(Mobile from, object targeted)
    {
        if (targeted is Item item && BaseTool.CheckAccessible(_tool, from))
        {
            if (item is BaseRunicTool runicTool)
            {
                if (item.IsChildOf(from.Backpack))
                {
                    if (item == _tool)
                    {
                        from.SendLocalizedMessage(1010087); // You cannot use that!
                    }
                    else if (item.GetType() != _tool.GetType())
                    {
                        from.SendLocalizedMessage(1152274); // You may only combine runic tools of the same type.
                    }
                    else if (runicTool.Resource != _tool.Resource)
                    {
                        from.SendLocalizedMessage(1152275); // You may only combine runic tools of the same material.
                    }
                    else if (_tool.UsesRemaining + runicTool.UsesRemaining > 100)
                    {
                        from.SendLocalizedMessage(1152276); // The combined charges of the two tools cannot exceed 100.
                    }
                    else
                    {
                        _tool.UsesRemaining += runicTool.UsesRemaining;
                        item.Delete();

                        from.SendLocalizedMessage(1152278); // You combine the runic tools, consolidating their Uses Remaining.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1152277); // Both tools must be in your backpack in order to combine them.
                }
            }
            else
            {
                if (item.IsChildOf(from.Backpack))
                {
                    if (RunicReforging.CanReforge(from, item, _tool.CraftSystem))
                    {
                        from.SendGump(new RunicReforgingGump(from, item, _tool));
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1152271); // The item must be in your backpack to re-forge it.
                }
            }
        }
    }
}
