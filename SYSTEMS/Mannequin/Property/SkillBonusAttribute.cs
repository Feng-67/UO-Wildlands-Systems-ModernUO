// UO Wildlands Custom Script
// Adapted for ModernUO from ServUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles.MannequinProperty;

public abstract class SkillBonusAttr : ValuedProperty
{
    public override Catalog Catalog => Catalog.SkillBonusGear;
    public override int Description => 1159316;
    public abstract SkillName Skill { get; }
    public override int Hue => 0x42FF;
    public override int SpriteW => 270;
    public override int SpriteH => 300;

    public double GetPropertyValue(Item item)
    {
        double value = 0;

        var skillBonuses = GetSkillBonuses(item);

        if (skillBonuses != null)
        {
            for (var i = 0; i < 5; i++)
            {
                if (skillBonuses.GetValues(i, out var check, out var bonus) && check == Skill && bonus > 0)
                {
                    value += bonus;
                }
            }
        }

        return value;
    }

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

    private static AosSkillBonuses GetSkillBonuses(Item item) => item switch
    {
        BaseWeapon weapon      => weapon.SkillBonuses,
        BaseArmor armor        => armor.SkillBonuses,
        BaseClothing clothing  => clothing.SkillBonuses,
        BaseJewel jewel        => jewel.SkillBonuses,
        BaseTalisman talisman  => talisman.SkillBonuses,
        Spellbook spellbook    => spellbook.SkillBonuses,
        _                      => null
    };
}

public class SwordsBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002151;
    public override SkillName Skill => SkillName.Swords;
}

public class FencingBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044102;
    public override SkillName Skill => SkillName.Fencing;
}

public class MacingBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044101;
    public override SkillName Skill => SkillName.Macing;
}

public class MageryBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002106;
    public override SkillName Skill => SkillName.Magery;
}

public class MusicianshipBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002116;
    public override SkillName Skill => SkillName.Musicianship;
}

public class WrestlingBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002169;
    public override SkillName Skill => SkillName.Wrestling;
}

public class TacticsBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1017321;
    public override SkillName Skill => SkillName.Tactics;
}

public class AnimalTamingBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044095;
    public override SkillName Skill => SkillName.AnimalTaming;
}

public class ProvocationBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002125;
    public override SkillName Skill => SkillName.Provocation;
}

public class SpiritSpeakBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002140;
    public override SkillName Skill => SkillName.SpiritSpeak;
}

public class StealthBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044107;
    public override SkillName Skill => SkillName.Stealth;
}

public class ParryBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002118;
    public override SkillName Skill => SkillName.Parry;
}

public class MeditationBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044106;
    public override SkillName Skill => SkillName.Meditation;
}

public class AnimalLoreBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002007;
    public override SkillName Skill => SkillName.AnimalLore;
}

public class DiscordanceBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044075;
    public override SkillName Skill => SkillName.Discordance;
}

public class FocusBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044110;
    public override SkillName Skill => SkillName.Focus;
}

public class StealingBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002142;
    public override SkillName Skill => SkillName.Stealing;
}

public class AnatomyBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002004;
    public override SkillName Skill => SkillName.Anatomy;
}

public class EvalIntBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044076;
    public override SkillName Skill => SkillName.EvalInt;
}

public class VeterinaryBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044099;
    public override SkillName Skill => SkillName.Veterinary;
}

public class NecromancyBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044109;
    public override SkillName Skill => SkillName.Necromancy;
}

public class BushidoBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044112;
    public override SkillName Skill => SkillName.Bushido;
}

public class MysticismBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044115;
    public override SkillName Skill => SkillName.Mysticism;
}

public class HealingBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002082;
    public override SkillName Skill => SkillName.Healing;
}

public class MagicResistBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1049662;
    public override SkillName Skill => SkillName.MagicResist;
}

public class PeacemakingBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002120;
    public override SkillName Skill => SkillName.Peacemaking;
}

public class ArcheryBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1002029;
    public override SkillName Skill => SkillName.Archery;
}

public class ChivalryBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044111;
    public override SkillName Skill => SkillName.Chivalry;
}

public class NinjitsuBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044113;
    public override SkillName Skill => SkillName.Ninjitsu;
}

public class ThrowingBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044117;
    public override SkillName Skill => SkillName.Throwing;
}

public class LumberjackingBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044104;
    public override SkillName Skill => SkillName.Lumberjacking;
}

public class SnoopingBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044088;
    public override SkillName Skill => SkillName.Snooping;
}

public class MiningBonusProperty : SkillBonusAttr
{
    public override int LabelNumber => 1044105;
    public override SkillName Skill => SkillName.Mining;
}
