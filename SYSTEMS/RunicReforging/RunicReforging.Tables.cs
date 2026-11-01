using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Engines.Imbuing;
using Server.Mobiles;

namespace Server.Items;

public static partial class RunicReforging
{
    private static readonly Dictionary<Type, CraftSystem> _allowableTable = new();
    private static readonly Dictionary<int, NamedInfoCol[][]> _prefixSuffixInfo = new();

    public static Dictionary<int, NamedInfoCol[][]> PrefixSuffixInfo => _prefixSuffixInfo;

    // The craft systems are assigned by their own Initialize methods, so this has to run after them
    // or every entry below would capture a null system.
    [CallPriority(1)]
    public static void Initialize()
    {
        _allowableTable[typeof(LeatherGlovesOfMining)] = DefTailoring.CraftSystem;
        _allowableTable[typeof(RingmailGlovesOfMining)] = DefBlacksmithy.CraftSystem;
        _allowableTable[typeof(StuddedGlovesOfMining)] = DefTailoring.CraftSystem;
        _allowableTable[typeof(JukaBow)] = DefBowFletching.CraftSystem;
        _allowableTable[typeof(TribalSpear)] = DefBlacksmithy.CraftSystem;
        _allowableTable[typeof(Pickaxe)] = DefBlacksmithy.CraftSystem;
        _allowableTable[typeof(Cleaver)] = DefBlacksmithy.CraftSystem;
        _allowableTable[typeof(SkinningKnife)] = DefBlacksmithy.CraftSystem;
        _allowableTable[typeof(ButcherKnife)] = DefBlacksmithy.CraftSystem;
        // TODO(migrate): GargishNecklace, GargishEarrings, GargishAmulet, GargishStoneAmulet,
        // BarbedWhip and SpikedWhip do not exist in ModernUO, so they are not registered.
        _allowableTable[typeof(BladedWhip)] = DefTailoring.CraftSystem;

        BuildPrefixSuffixInfo();
    }

    public static void Configure()
    {
        CommandSystem.Register("GetCreatureScore", AccessLevel.GameMaster, GetCreatureScore_OnCommand);
        CommandSystem.Register("Reforge", AccessLevel.GameMaster, Reforge_OnCommand);
    }

    private static void GetCreatureScore_OnCommand(CommandEventArgs e)
    {
        e.Mobile.BeginTarget(12, false, Server.Targeting.TargetFlags.None, GetCreatureScore_OnTarget);
    }

    private static void GetCreatureScore_OnTarget(Mobile from, object targeted)
    {
        if (targeted is BaseCreature creature)
        {
            creature.PrivateOverheadMessage(
                MessageType.Regular,
                0x25,
                false,
                RandomItemGenerator.GetDifficultyFor(creature).ToString(),
                from.NetState
            );
        }
    }

    // TypeIndex 0 - Weapon; 1 - Armor; 2 - Shield; 3 - Jewels
    // RunicIndex 0 - dullcopper; 1 - shadow; 2 - copper; 3 - spined; 4 - Oak; 5 - ash
    private static void BuildPrefixSuffixInfo()
    {
        _prefixSuffixInfo[0] = null;

        _prefixSuffixInfo[1] = new NamedInfoCol[][] // Might
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosWeaponAttribute.HitLeechHits, HitsAndManaLeechTable),
                new NamedInfoCol(AosAttribute.BonusHits, WeaponHitsTable),
                new NamedInfoCol(AosAttribute.BonusStr, WeaponStrTable),
                new NamedInfoCol(AosAttribute.RegenHits, WeaponRegenTable),
            },

            new NamedInfoCol[] // armor
            {
                // TODO(migrate): the "RandomEater" row is dropped; ModernUO has no SAAbsorptionAttribute.
                new NamedInfoCol(AosAttribute.BonusHits, ArmorHitsTable),
                new NamedInfoCol(AosAttribute.BonusStr, ArmorStrTable),
                new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
            },

            new NamedInfoCol[] // shield
            {
                // TODO(migrate): the "RandomEater" row is dropped; ModernUO has no SAAbsorptionAttribute.
                new NamedInfoCol(AosAttribute.BonusHits, ArmorHitsTable),
                new NamedInfoCol(AosAttribute.BonusStr, ArmorStrTable),
                new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
            },

            new NamedInfoCol[] // jewels
            {
                new NamedInfoCol(AosAttribute.BonusHits, ArmorHitsTable),
                new NamedInfoCol(AosAttribute.BonusStr, ArmorStrTable),
            }
        };

        _prefixSuffixInfo[2] = new NamedInfoCol[][] // Mystic
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosWeaponAttribute.HitLeechMana, HitsAndManaLeechTable),
                new NamedInfoCol(AosAttribute.BonusMana, WeaponStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                new NamedInfoCol(AosAttribute.LowerManaCost, WeaponStamManaLMCTable),
                /*new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable), */
            },
            new NamedInfoCol[] // armor
            {
                new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable),
                new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable),
            },
        };

        _prefixSuffixInfo[3] = new NamedInfoCol[][] // Animated
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosWeaponAttribute.HitLeechStam, HitStamLeechTable),
                new NamedInfoCol(AosAttribute.BonusStam, WeaponStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusDex, DexIntTable),
                new NamedInfoCol(AosAttribute.RegenStam, WeaponRegenTable),
                new NamedInfoCol(AosAttribute.WeaponSpeed, WeaponWeaponSpeedTable),
            },
            new NamedInfoCol[] // armor
            {
                new NamedInfoCol(AosAttribute.BonusStam, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusDex, DexIntTable),
                new NamedInfoCol(AosAttribute.RegenStam, ArmorRegenTable),
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.BonusStam, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusDex, DexIntTable),
                new NamedInfoCol(AosAttribute.RegenStam, ArmorRegenTable),
                new NamedInfoCol(AosAttribute.WeaponSpeed, ShieldWeaponSpeedTable),
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.BonusStam, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusDex, DexIntTable),
                new NamedInfoCol(AosAttribute.WeaponSpeed, ShieldWeaponSpeedTable),
            },
        };

        _prefixSuffixInfo[4] = new NamedInfoCol[][] // Arcane
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosWeaponAttribute.HitLeechMana, HitsAndManaLeechTable),
                // TODO(migrate): the HitManaDrain row is dropped; AosWeaponAttribute has no HitManaDrain.
                new NamedInfoCol(AosAttribute.BonusMana, WeaponStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                new NamedInfoCol(AosAttribute.LowerManaCost, WeaponStamManaLMCTable),
                new NamedInfoCol(AosAttribute.CastSpeed, 1),
                new NamedInfoCol(AosAttribute.SpellChanneling, 1),
                new NamedInfoCol(AosWeaponAttribute.MageWeapon, MageWeaponTable),
                new NamedInfoCol(AosAttribute.RegenMana, WeaponRegenTable),
            },
            new NamedInfoCol[] // armor
            {
                new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
                new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable),
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
                new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable),
                new NamedInfoCol(AosAttribute.CastSpeed, 1),
                new NamedInfoCol(AosAttribute.CastRecovery, 4),
                new NamedInfoCol(AosAttribute.SpellDamage, 18),
            },
        };

        _prefixSuffixInfo[5] = new NamedInfoCol[][] // Exquisite
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosWeaponAttribute.SelfRepair, SelfRepairTable), //
                new NamedInfoCol(AosWeaponAttribute.DurabilityBonus, DurabilityTable), //
                new NamedInfoCol(AosWeaponAttribute.LowerStatReq, LowerStatReqTable), //
                new NamedInfoCol("Slayer", 1), //
                new NamedInfoCol(AosWeaponAttribute.MageWeapon, MageWeaponTable), //
                new NamedInfoCol(AosAttribute.SpellChanneling, 1), //
                new NamedInfoCol(AosAttribute.BalancedWeapon, 1), //
                new NamedInfoCol("WeaponVelocity", WeaponVelocityTable), //
                new NamedInfoCol("ElementalDamage", ElementalDamageTable), //
            },
            new NamedInfoCol[] // armor
            {
                new NamedInfoCol(AosArmorAttribute.SelfRepair, SelfRepairTable),
                new NamedInfoCol(AosArmorAttribute.DurabilityBonus, DurabilityTable),
                new NamedInfoCol(AosArmorAttribute.LowerStatReq, LowerStatReqTable),
            },
            new NamedInfoCol[] // shield
            {
                new NamedInfoCol(AosArmorAttribute.SelfRepair, SelfRepairTable),
                new NamedInfoCol(AosArmorAttribute.DurabilityBonus, DurabilityTable),
                new NamedInfoCol(AosArmorAttribute.LowerStatReq, LowerStatReqTable),
            },
            new NamedInfoCol[]
            {
            },
        };

        _prefixSuffixInfo[6] = new NamedInfoCol[][] // Vampiric
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosWeaponAttribute.HitLeechHits, HitsAndManaLeechTable),
                new NamedInfoCol(AosWeaponAttribute.HitLeechStam, HitStamLeechTable),
                new NamedInfoCol(AosWeaponAttribute.HitLeechMana, HitsAndManaLeechTable),
                // TODO(migrate): the HitManaDrain, HitFatigue and BloodDrinker rows are dropped;
                // AosWeaponAttribute has none of them.
            },
            new NamedInfoCol[] // armor
            {
            },
            new NamedInfoCol[]
            {
            },
            new NamedInfoCol[]
            {
            },
        };

        _prefixSuffixInfo[7] = new NamedInfoCol[][] // Invigorating
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosAttribute.RegenHits, WeaponRegenTable),
                new NamedInfoCol(AosAttribute.RegenStam, WeaponRegenTable),
                new NamedInfoCol(AosAttribute.RegenMana, WeaponRegenTable),
            },
            new NamedInfoCol[] // armor
            {
                new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
                new NamedInfoCol(AosAttribute.RegenStam, ArmorRegenTable),
                new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),

                // TODO(migrate): the "RandomEater" row is dropped; ModernUO has no SAAbsorptionAttribute.
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
                new NamedInfoCol(AosAttribute.RegenStam, ArmorRegenTable),
                new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
                // TODO(migrate): the AosArmorAttribute.SoulCharge and "RandomEater" rows are dropped;
                // ModernUO has neither SoulCharge nor SAAbsorptionAttribute.
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
                new NamedInfoCol(AosAttribute.RegenStam, ArmorRegenTable),
                new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
            },
        };

        _prefixSuffixInfo[8] = new NamedInfoCol[][] // Fortified
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosWeaponAttribute.ResistPhysicalBonus, ResistTable),
                new NamedInfoCol(AosWeaponAttribute.ResistFireBonus, ResistTable),
                new NamedInfoCol(AosWeaponAttribute.ResistColdBonus, ResistTable),
                new NamedInfoCol(AosWeaponAttribute.ResistPoisonBonus, ResistTable),
                new NamedInfoCol(AosWeaponAttribute.ResistEnergyBonus, ResistTable),
            },
            new NamedInfoCol[] // armor
            {
                new NamedInfoCol(AosElementAttribute.Physical, ResistTable),
                new NamedInfoCol(AosElementAttribute.Fire, ResistTable),
                new NamedInfoCol(AosElementAttribute.Cold, ResistTable),
                new NamedInfoCol(AosElementAttribute.Poison, ResistTable),
                new NamedInfoCol(AosElementAttribute.Energy, ResistTable),
                // TODO(migrate): the "RandomEater" row is dropped; ModernUO has no SAAbsorptionAttribute.
            },
            new NamedInfoCol[] // shield
            {
                new NamedInfoCol(AosElementAttribute.Physical, ResistTable),
                new NamedInfoCol(AosElementAttribute.Fire, ResistTable),
                new NamedInfoCol(AosElementAttribute.Cold, ResistTable),
                new NamedInfoCol(AosElementAttribute.Poison, ResistTable),
                new NamedInfoCol(AosElementAttribute.Energy, ResistTable),
                // TODO(migrate): the "RandomEater" row is dropped; ModernUO has no SAAbsorptionAttribute.
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosElementAttribute.Physical, ResistTable),
                new NamedInfoCol(AosElementAttribute.Fire, ResistTable),
                new NamedInfoCol(AosElementAttribute.Cold, ResistTable),
                new NamedInfoCol(AosElementAttribute.Poison, ResistTable),
                new NamedInfoCol(AosElementAttribute.Energy, ResistTable),
            },
        };

        _prefixSuffixInfo[9] = new NamedInfoCol[][] // Auspicious
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosAttribute.Luck, LuckTable, RangedLuckTable),
            },
            new NamedInfoCol[] // armor
            {
                new NamedInfoCol(AosAttribute.Luck, LuckTable),
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.Luck, LuckTable),
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.Luck, LuckTable),
            },
        };

        _prefixSuffixInfo[10] = new NamedInfoCol[][] // Charmed
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosAttribute.EnhancePotions, WeaponEnhancePots),
                new NamedInfoCol(AosAttribute.BalancedWeapon, 1),
            },
            new NamedInfoCol[] // armor
            {
                new NamedInfoCol(AosAttribute.EnhancePotions, ArmorEnhancePotsTable),
            },
            new NamedInfoCol[]
            {
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.EnhancePotions, ArmorEnhancePotsTable),
            },
        };

        _prefixSuffixInfo[11] = new NamedInfoCol[][] // Vicious
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol("HitSpell", HitWeaponTable1),
                new NamedInfoCol("HitArea", HitWeaponTable1),
                new NamedInfoCol(AosAttribute.AttackChance, WeaponHCITable, RangedHCITable),
                new NamedInfoCol(AosAttribute.WeaponDamage, WeaponDamageTable),
                // TODO(migrate): the BattleLust and SplinteringWeapon rows are dropped;
                // AosWeaponAttribute has neither of them.
                new NamedInfoCol("Slayer", 1),
            },
            new NamedInfoCol[] // armor
            {
                new NamedInfoCol(AosAttribute.AttackChance, ArmorHCIDCITable),
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.AttackChance, WeaponHCITable),
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.AttackChance, WeaponHCITable),
                new NamedInfoCol(AosAttribute.SpellDamage, 18),
            },
        };

        _prefixSuffixInfo[12] = new NamedInfoCol[][] // Towering
        {
            new NamedInfoCol[] // Weapon
            {
                new NamedInfoCol(AosWeaponAttribute.HitLowerAttack, HitWeaponTable1),
                // TODO(migrate): the ReactiveParalyze row is dropped; AosWeaponAttribute has no ReactiveParalyze.
                new NamedInfoCol(AosAttribute.DefendChance, WeaponDCITable, RangedDCITable),
            },
            new NamedInfoCol[] // armor
            {
                new NamedInfoCol(AosAttribute.DefendChance, ArmorHCIDCITable),
                // TODO(migrate): the SAAbsorptionAttribute.CastingFocus row is dropped; ModernUO has no
                // SAAbsorptionAttribute.
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.DefendChance, WeaponDCITable),
                // TODO(migrate): the ReactiveParalyze and SoulCharge rows are dropped;
                // AosArmorAttribute has neither of them.
            },
            new NamedInfoCol[]
            {
                new NamedInfoCol(AosAttribute.DefendChance, ArmorHCIDCITable),
                //new NamedInfoCol(SAAbsorptionAttribute.CastingFocus, ArmorCastingFocusTable),
            },
        };
    }

    public class NamedInfoCol
    {
        public object Attribute { get; private set; }
        public int[][] Info { get; private set; }
        public int[][] SecondaryInfo { get; private set; }

        public int HardCap { get; set; }

        public NamedInfoCol(object attr, int[][] info, int[][] secondary = null)
        {
            Attribute = attr;
            Info = info;
            SecondaryInfo = secondary;
        }

        public NamedInfoCol(object attr, int hardcap)
        {
            Attribute = attr;
            HardCap = hardcap;
        }

        public int RandomRangedIntensity(Item item, int id, int resIndex, int preIndex)
        {
            if (Info == null || HardCap == 1)
            {
                return HardCap;
            }

            int[] range = item is BaseRanged && SecondaryInfo != null ? SecondaryInfo[resIndex] : Info[resIndex];

            var max = range[preIndex];
            var min = Math.Max(ItemPropertyInfo.GetMinIntensity(item, id), (int)((double)range[0] * .75));
            int value;

            if (Utility.RandomBool())
            {
                value = Utility.RandomBool() ? min : max;
            }
            else
            {
                value = Utility.RandomMinMax(min, max);
            }

            var scale = ItemPropertyInfo.GetScale(item, id);

            if (scale > 1 && value > scale)
            {
                value = (value / scale) * scale;
            }

            return value;
        }
    }

    public static object GetRandomHitSpell()
    {
        switch (Utility.Random(4))
        {
            default:
            case 0: return AosWeaponAttribute.HitMagicArrow;
            case 1: return AosWeaponAttribute.HitFireball;
            case 2: return AosWeaponAttribute.HitHarm;
            case 3: return AosWeaponAttribute.HitLightning;
                //case 4: return AosWeaponAttribute.HitCurse;
        }
    }

    private static object GetRandomHitArea()
    {
        switch (Utility.Random(5))
        {
            default:
            case 0: return AosWeaponAttribute.HitPhysicalArea;
            case 1: return AosWeaponAttribute.HitFireArea;
            case 2: return AosWeaponAttribute.HitColdArea;
            case 3: return AosWeaponAttribute.HitPoisonArea;
            case 4: return AosWeaponAttribute.HitEnergyArea;
        }
    }

    // TODO(migrate): ModernUO has no SAAbsorptionAttribute, so there are no eater attributes to pick
    // from. Every "RandomEater" row was removed from the prefix/suffix info; this returns nothing and
    // callers must treat a null attribute as "no eater property".
    private static object GetRandomEater() => null;

    private static AosElementAttribute GetRandomElemental()
    {
        switch (Utility.Random(4))
        {
            default:
            case 0: return AosElementAttribute.Fire;
            case 1: return AosElementAttribute.Cold;
            case 2: return AosElementAttribute.Poison;
            case 3: return AosElementAttribute.Energy;
        }
    }

    private static SkillName GetRandomSkill(Item item)
    {
        var skillbonuses = GetAosSkillBonuses(item);

        if (skillbonuses == null)
        {
            return SkillName.Alchemy;
        }

        SkillName[] possibleSkills = _skills;
        SkillName sk, check;

        double bonus;
        bool found;

        do
        {
            found = false;
            sk = possibleSkills[Utility.Random(possibleSkills.Length)];

            // TODO(migrate): GargishRing and GargishBracelet do not exist in ModernUO, so the
            // Archery -> Throwing substitution has no eligible item and is omitted.

            for (int i = 0; !found && i < 5; ++i)
            {
                found = skillbonuses.GetValues(i, out check, out bonus) && check == sk;
            }
        } while (found);

        return sk;
    }

    public static int GetName(int value)
    {
        switch (value)
        {
            default:
            case 0: return 1062648;
            case 1: return 1151717;
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12: return 1151706 + (value - 2);
        }
    }

    public static int GetPrefixName(ReforgedPrefix prefix) => NameTable[(int)prefix - 1][0];

    public static int GetSuffixName(ReforgedSuffix suffix) => NameTable[(int)suffix - 1][1];

    public static int[][] NameTable => _nameTable;

    private static readonly int[][] _nameTable =
    {
        new int[] { 1151682, 1151683 }, // Might
        new int[] { 1151684, 1151685 }, // Mystic
        new int[] { 1151686, 1151687 }, // Animated
        new int[] { 1151688, 1151689 }, // Arcane
        new int[] { 1151690, 1151691 }, // Exquisite
        new int[] { 1151692, 1151693 }, // Vampiric
        new int[] { 1151694, 1151695 }, // Invigorating
        new int[] { 1151696, 1151697 }, // Fortified
        new int[] { 1151698, 1151699 }, // Auspicious
        new int[] { 1151700, 1151701 }, // Charmed
        new int[] { 1151702, 1151703 }, // Vicious
        new int[] { 1151704, 1151705 }, // Towering
        new int[] { 0, 1154548 }, // Blackthorn
        new int[] { 0, 1154507 }, // Minax
        new int[] { 0, 1156900 }, // Kotl
        new int[] { 0, 1158672 }, // Khaldun
        new int[] { 0, 1155589 }, // Doom
        new int[] { 0, 1157614 }, // Sorcerers Dungeon
    };

    public static void AddSuffixName(IPropertyList list, ReforgedSuffix suffix, string name)
    {
        if (suffix >= ReforgedSuffix.Blackthorn)
        {
            list.Add(GetSuffixName(suffix), name);
        }
        else
        {
            // ~1_ITEM~ of ~2_SUFFIX~
            list.Add(1151758, $"{name}\t{GetSuffixName(suffix):#}");
        }
    }

    private static readonly SkillName[] _skills =
    {
        SkillName.Swords,
        SkillName.Fencing,
        SkillName.Macing,
        SkillName.Archery,
        SkillName.Wrestling,
        SkillName.Parry,
        SkillName.Tactics,
        SkillName.Anatomy,
        SkillName.Healing,
        SkillName.Magery,
        SkillName.Meditation,
        SkillName.EvalInt,
        SkillName.MagicResist,
        SkillName.AnimalTaming,
        SkillName.AnimalLore,
        SkillName.Veterinary,
        SkillName.Musicianship,
        SkillName.Provocation,
        SkillName.Discordance,
        SkillName.Peacemaking,
        SkillName.Chivalry,
        SkillName.Focus,
        SkillName.Necromancy,
        SkillName.Stealing,
        SkillName.Stealth,
        SkillName.SpiritSpeak,
        SkillName.Bushido,
        SkillName.Ninjitsu
    };

    #region Tables
    #region All
    public static int[][] DexIntTable =
    {
        new int[] { 3, 4, 4, 4, 5, 5, 5 },
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
    };

    public static int[][] LowerStatReqTable =
    {
        new int[] { 60, 70, 80, 100, 100, 100, 100 },
        new int[] { 80, 100, 100, 100, 100, 100, 100 },
        new int[] { 100, 100, 100, 100, 100, 100, 100 },
        new int[] { 70, 100, 100, 100, 100, 100, 100 },
        new int[] { 80, 100, 100, 100, 100, 100, 100 },
        new int[] { 100, 100, 100, 100, 100, 100, 100 },
    };

    public static int[][] SelfRepairTable =
    {
        new int[] { 2, 4, 0, 0, 0, 0, 0 },
        new int[] { 5, 5, 0, 0, 0, 0, 0 },
        new int[] { 6, 7, 0, 0, 0, 0, 0 },
        new int[] { 5, 5, 0, 0, 0, 0, 0 },
        new int[] { 5, 5, 0, 0, 0, 0, 0 },
        new int[] { 7, 7, 0, 0, 0, 0, 0 },
    };

    public static int[][] DurabilityTable =
    {
        new int[] { 90, 100, 0, 0, 0, 0, 0 },
        new int[] { 110, 140, 0, 0, 0, 0, 0 },
        new int[] { 150, 150, 0, 0, 0, 0, 0 },
        new int[] { 100, 140, 0, 0, 0, 0, 0 },
        new int[] { 110, 140, 0, 0, 0, 0, 0 },
        new int[] { 150, 150, 0, 0, 0, 0, 0 },
    };

    public static int[][] ResistTable =
    {
        new int[] { 10, 15, 15, 15, 20, 20, 20 },
        new int[] { 15, 15, 15, 20, 20, 20, 20 },
        new int[] { 20, 20, 20, 20, 20, 20, 20 },
        new int[] { 20, 20, 20, 20, 20, 20, 20 },
        new int[] { 15, 15, 20, 20, 20, 20, 20 },
        new int[] { 20, 20, 20, 20, 20, 20, 20 },
    };
    #endregion

    #region Weapon Tables
    public static int[][] ElementalDamageTable =
    {
        new int[] { 40, 60, 60, 70, 80, 90, 100 },
        new int[] { 50, 60, 70, 80, 90, 100, 100 },
        new int[] { 60, 70, 80, 90, 100, 100, 100 },
        new int[] { },
        new int[] { 100, 100, 100, 100, 100, 100, 100 },
        new int[] { 100, 100, 100, 100, 100, 100, 100 },
    };

    // Hit magic, area, HLA
    public static int[][] HitWeaponTable1 =
    {
        new int[] { 30, 50, 50, 60, 70, 70, 70 },
        new int[] { 50, 60, 70, 70, 70, 70, 70 },
        new int[] { 70, 70, 70, 70, 70, 70, 70 },
        new int[] { },
        new int[] { 50, 60, 70, 70, 70, 70, 70 },
        new int[] { 70, 70, 70, 70, 70, 70, 70 },
    };

    // hit fatigue, mana drain, HLD
    public static int[][] HitWeaponTable2 =
    {
        new int[] { 30, 40, 50, 50, 60, 70, 70 },
        new int[] { 50, 50, 50, 60, 70, 70, 70 },
        new int[] { 50, 60, 70, 70, 70, 70, 70 },
        new int[] { },
        new int[] { 50, 50, 50, 60, 70, 70, 70 },
        new int[] { 70, 70, 70, 70, 70, 70, 70 },
    };

    public static int[][] WeaponVelocityTable =
    {
        new int[] { 25, 35, 40, 40, 40, 45, 50 },
        new int[] { 40, 40, 40, 45, 50, 50, 50 },
        new int[] { 40, 45, 50, 50, 50, 50, 50 },
        new int[] { },
        new int[] { 40, 40, 40, 45, 50, 50, 50 },
        new int[] { 45, 50, 50, 50, 50, 50, 50 },
    };

    public static int[][] HitsAndManaLeechTable =
    {
        new int[] { 15, 25, 25, 30, 35, 35, 35 },
        new int[] { 25, 25, 30, 35, 35, 35, 35 },
        new int[] { 30, 35, 35, 35, 35, 35, 35 },
        new int[] { },
        new int[] { 25, 25, 30, 35, 35, 35, 35 },
        new int[] { 35, 35, 35, 35, 35, 35, 35 },
    };

    public static int[][] HitStamLeechTable =
    {
        new int[] { 30, 50, 50, 60, 70, 70, 70 },
        new int[] { 50, 60, 70, 70, 70, 70, 70 },
        new int[] { 70, 70, 70, 70, 70, 70, 70 },
        new int[] { },
        new int[] { 50, 60, 70, 70, 70, 70, 70 },
        new int[] { 70, 70, 70, 70, 70, 70, 70 },
    };

    public static int[][] LuckTable =
    {
        new int[] { 80, 100, 100, 120, 140, 150, 150 },
        new int[] { 100, 120, 140, 150, 150, 150, 150 },
        new int[] { 130, 150, 150, 150, 150, 150, 150 },
        new int[] { 100, 120, 140, 150, 150, 150, 150 },
        new int[] { 100, 120, 140, 150, 150, 150, 150 },
        new int[] { 150, 150, 150, 150, 150, 150, 150 },
    };

    //30% LOW IN MIN
    /* switch (resource)
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
    }*/

    public static int[][] MageWeaponTable =
    {
        new int[] { 25, 20, 20, 20, 20, 15, 15 },
        new int[] { 20, 20, 20, 15, 15, 15, 15 },
        new int[] { 20, 15, 15, 15, 15, 15, 15 },
        new int[] { },
        new int[] { 20, 20, 20, 15, 15, 15, 15 },
        new int[] { 15, 15, 15, 15, 15, 15, 15 },
    };

    public static int[][] WeaponRegenTable =
    {
        new int[] { 2, 3, 6, 6, 6, 6, 6 },
        new int[] { 3, 6, 6, 6, 6, 6, 6 },
        new int[] { 6, 6, 6, 6, 6, 9, 9 },
        new int[] { },
        new int[] { 3, 6, 6, 6, 6, 6, 9 },
        new int[] { 6, 9, 9, 9, 9, 9, 9 },
    };

    public static int[][] WeaponHitsTable =
    {
        new int[] { 2, 3, 3, 3, 4, 4, 4 },
        new int[] { 3, 3, 4, 4, 4, 4, 4 },
        new int[] { 4, 4, 4, 4, 4, 4, 4 },
        new int[] { },
        new int[] { 3, 3, 4, 4, 4, 4, 4 },
        new int[] { 4, 4, 4, 4, 4, 4, 4 },
    };

    public static int[][] WeaponStamManaLMCTable =
    {
        new int[] { 2, 4, 4, 4, 5, 5, 5 },
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
        new int[] { },
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
    };

    public static int[][] WeaponStrTable =
    {
        new int[] { 2, 4, 4, 4, 5, 5, 5 },
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
        new int[] { },
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
    };

    public static int[][] WeaponHCITable =
    {
        new int[] { 5, 10, 15, 15, 15, 20, 20 },
        new int[] { 15, 15, 15, 20, 20, 20, 20 },
        new int[] { 15, 20, 20, 20, 20, 20, 20 },
        new int[] { },
        new int[] { 15, 15, 20, 20, 20, 20, 20 },
        new int[] { 20, 20, 20, 20, 20, 20, 20 },
    };

    public static int[][] WeaponDCITable =
    {
        new int[] { 10, 15, 15, 15, 20, 20, 20 },
        new int[] { 15, 15, 20, 20, 20, 20, 20 },
        new int[] { 20, 20, 20, 20, 20, 20, 20 },
        new int[] { },
        new int[] { 15, 15, 20, 20, 20, 20, 20 },
        new int[] { 20, 20, 20, 20, 20, 20, 20 },
    };

    public static int[][] WeaponDamageTable =
    {
        new int[] { 30, 50, 50, 60, 70, 70, 70 },
        new int[] { 50, 60, 70, 70, 70, 70, 70 },
        new int[] { 70, 70, 70, 70, 70, 70, 70 },
        new int[] { },
        new int[] { 50, 60, 70, 70, 70, 70, 70 },
        new int[] { 70, 70, 70, 70, 70, 70, 70 },
    };

    public static int[][] WeaponEnhancePots =
    {
        new int[] { 5, 10, 10, 10, 10, 15, 15 },
        new int[] { 10, 10, 10, 15, 15, 15, 15 },
        new int[] { 10, 15, 15, 15, 15, 15, 15 },
        new int[] { },
        new int[] { 10, 10, 10, 15, 15, 15, 15 },
        new int[] { 15, 15, 15, 15, 15, 15, 15 },
    };

    public static int[][] WeaponWeaponSpeedTable =
    {
        new int[] { 20, 30, 30, 35, 40, 40, 40 },
        new int[] { 30, 35, 40, 40, 40, 40, 40 },
        new int[] { 35, 40, 40, 40, 40, 40, 40 },
        new int[] { },
        new int[] { 30, 35, 40, 40, 40, 40, 40 },
        new int[] { 40, 40, 40, 40, 40, 40, 40 },
    };
    #endregion

    #region Ranged Weapons
    public static int[][] RangedLuckTable =
    {
        new int[] { 90, 120, 120, 140, 170, 170, 170 },
        new int[] { 120, 140, 160, 170, 170, 170, 170 },
        new int[] { 160, 170, 170, 170, 170, 170, 170 },
        new int[] { },
        new int[] { 120, 140, 160, 170, 170, 170, 170 },
        new int[] { 170, 170, 170, 170, 170, 170, 170 },
    };

    public static int[][] RangedHCITable =
    {
        new int[] { 15, 25, 25, 30, 35, 35, 35 },
        new int[] { 25, 30, 35, 35, 35, 35, 35 },
        new int[] { 35, 35, 35, 35, 35, 35, 35 },
        new int[] { },
        new int[] { 25, 25, 30, 35, 35, 35, 35 },
        new int[] { 35, 35, 35, 35, 35, 35, 35 },
    };

    public static int[][] RangedDCITable =
    {
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { 25, 25, 30, 35, 35, 35, 35 },
        new int[] { 35, 35, 35, 35, 35, 35, 35 },
    };
    #endregion

    #region Armor Tables
    public static int[][] LowerRegTable =
    {
        new int[] { 10, 20, 20, 20, 25, 25, 25 },
        new int[] { 20, 20, 25, 25, 25, 25, 25 },
        new int[] { 25, 25, 25, 25, 25, 25, 25 },
        new int[] { 20, 20, 25, 25, 25, 25, 25 },
        new int[] { 20, 20, 25, 25, 25, 25, 25 },
        new int[] { 25, 25, 25, 25, 25, 25, 25 },
    };

    public static int[][] ArmorHitsTable =
    {
        new int[] { 3, 5, 5, 6, 7, 7, 7 },
        new int[] { 5, 6, 7, 7, 7, 7, 7 },
        new int[] { 7, 7, 7, 7, 7, 7, 7 },
        new int[] { 5, 5, 6, 7, 7, 7, 7 },
        new int[] { 5, 6, 7, 7, 7, 7, 7 },
        new int[] { 7, 7, 7, 7, 7, 7, 7 },
    };

    public static int[][] ArmorStrTable =
    {
        new int[] { 3, 4, 4, 4, 5, 5, 5 },
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
    };

    public static int[][] ArmorRegenTable =
    {
        new int[] { 2, 3, 3, 3, 4, 4, 4 },
        new int[] { 3, 3, 4, 4, 4, 4, 4 },
        new int[] { 4, 4, 4, 4, 4, 4, 4 },
        new int[] { 3, 3, 4, 4, 4, 4, 4 },
        new int[] { 3, 3, 4, 4, 4, 4, 4 },
        new int[] { 4, 4, 4, 4, 4, 4, 4 },
    };

    public static int[][] ArmorStamManaLMCTable =
    {
        new int[] { 4, 8, 8, 8, 10, 10, 10 },
        new int[] { 8, 8, 10, 10, 10, 10, 10 },
        new int[] { 10, 10, 10, 10, 10, 10, 10 },
        new int[] { 8, 8, 10, 10, 10, 10, 10 },
        new int[] { 8, 8, 10, 10, 10, 10, 10 },
        new int[] { 10, 10, 10, 10, 10, 10, 10 },
    };

    public static int[][] ArmorEnhancePotsTable =
    {
        new int[] { 2, 2, 3, 3, 3, 3, 3 },
        new int[] { 3, 3, 3, 3, 3, 3, 3 },
        new int[] { 3, 3, 3, 3, 3, 3, 3 },
        new int[] { 3, 3, 3, 3, 3, 3, 3 },
        new int[] { 3, 3, 3, 3, 3, 3, 3 },
        new int[] { 3, 3, 3, 3, 3, 3, 3 },
    };

    public static int[][] ArmorHCIDCITable =
    {
        new int[] { 4, 4, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
        new int[] { 5, 5, 5, 5, 5, 5, 5 },
    };

    public static int[][] ShieldWeaponSpeedTable =
    {
        new int[] { 5, 5, 5, 5, 10, 10, 10 },
        new int[] { 5, 5, 10, 10, 10, 10, 10 },
        new int[] { 10, 10, 10, 10, 10, 10, 10 },
        new int[] { },
        new int[] { 5, 5, 10, 10, 10, 10, 10 },
        new int[] { 10, 10, 10, 10, 10, 10, 10 },
    };
    #endregion
    #endregion
}
