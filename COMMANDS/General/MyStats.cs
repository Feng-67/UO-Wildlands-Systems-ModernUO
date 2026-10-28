// UO Wildlands Custom Script
// Adapted for ModernUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)
// Compiled & Modified by: [Feng / UO Wildlands Team]

using System;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps;

public class MyStatsCommand
{
    public static void Initialize()
    {
        CommandSystem.Register("mystats", AccessLevel.Player, MyStats_OnCommand);
    }

    [Usage("mystats")]
    [Description("Opens the Player Statistics dashboard.")]
    public static void MyStats_OnCommand(CommandEventArgs e)
    {
        PlayerStatsGump.DisplayTo(e.Mobile);
    }
}

public class PlayerStatsGump : DynamicGump
{
    // Color Constants
    private const int C_YELLOW = 2213;
    private const int C_WHITE = 2036;
    private const int C_RED = 1258;
    private const int C_BLUE = 1264;
    private const int C_GREEN = 1270;
    private const int C_PURPLE = 1276;
    private const int C_GREY = 2401;

    private readonly Mobile _from;

    public override bool Singleton => true;

    private PlayerStatsGump(Mobile from) : base(50, 50) => _from = from;

    public static void DisplayTo(Mobile from)
    {
        if (from == null || from.Deleted || from.NetState == null)
        {
            return;
        }

        from.SendGump(new PlayerStatsGump(from));
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        const int width = 640;
        const int height = 850;

        builder.AddPage();
        builder.AddBackground(0, 0, width, height, 9200);
        builder.AddImageTiled(10, 10, width - 20, height - 20, 2624);
        builder.AddAlphaRegion(10, 10, width - 20, height - 20);

        // --- HEADER ---
        builder.AddHtml(0, 25, width, 25, ColorAndCenter("#FFCC00", _from.Name));

        builder.AddButton(width - 75, 21, 4005, 4007, 1, GumpButtonType.Reply, 0); // Sync Button
        builder.AddButton(width - 40, 20, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0); // Close Button

        // --- ATTRIBUTES ---
        int attrY = 55;

        builder.AddLabel(30, attrY, C_YELLOW, "Attributes");

        DrawStat(ref builder, 30, attrY + 25, "Strength", _from.Str.ToString());
        DrawStat(ref builder, 340, attrY + 25, "Hit Points", $"{_from.Hits} / {_from.HitsMax}");

        DrawStat(ref builder, 30, attrY + 50, "Dexterity", _from.Dex.ToString());
        DrawStat(ref builder, 340, attrY + 50, "Stamina", $"{_from.Stam} / {_from.StamMax}");

        DrawStat(ref builder, 30, attrY + 75, "Intelligence", _from.Int.ToString());
        DrawStat(ref builder, 340, attrY + 75, "Mana", $"{_from.Mana} / {_from.ManaMax}");

        // --- RESISTANCES ---
        int resY = 175;
        DrawDivider(ref builder, 20, resY - 5, width - 40);
        builder.AddLabel(30, resY + 5, C_YELLOW, "Resistances");

        DrawResistBox(ref builder, 130, resY + 5, "Phys", _from.PhysicalResistance, C_GREY);
        DrawResistBox(ref builder, 230, resY + 5, "Fire", _from.FireResistance, C_RED);
        DrawResistBox(ref builder, 330, resY + 5, "Cold", _from.ColdResistance, C_BLUE);
        DrawResistBox(ref builder, 430, resY + 5, "Poison", _from.PoisonResistance, C_GREEN);
        DrawResistBox(ref builder, 530, resY + 5, "Energy", _from.EnergyResistance, C_PURPLE);

        // --- RATINGS (Combat & Magic) ---
        int colY = 220;
        DrawDivider(ref builder, 20, colY - 5, width - 40);
        builder.AddLabel(30, colY + 5, C_YELLOW, "Combat Ratings");
        builder.AddLabel(340, colY + 5, C_YELLOW, "Magic Ratings");

        // AOS Fetches
        int hci = AosAttributes.GetValue(_from, AosAttribute.AttackChance);
        int dci = AosAttributes.GetValue(_from, AosAttribute.DefendChance);
        int di = AosAttributes.GetValue(_from, AosAttribute.WeaponDamage);
        int ssi = AosAttributes.GetValue(_from, AosAttribute.WeaponSpeed);
        int sdi = AosAttributes.GetValue(_from, AosAttribute.SpellDamage);
        int lmc = AosAttributes.GetValue(_from, AosAttribute.LowerManaCost);
        int lrc = AosAttributes.GetValue(_from, AosAttribute.LowerRegCost);
        int fc = AosAttributes.GetValue(_from, AosAttribute.CastSpeed);
        int fcr = AosAttributes.GetValue(_from, AosAttribute.CastRecovery);

        // Custom Fetches
        double swingSpeed = 0.0;

        if (_from.Weapon is BaseWeapon weapon)
        {
            swingSpeed = weapon.GetDelay(_from).TotalSeconds;
        }

        double dex = _from.Dex;
        double bandageSeconds = Math.Min(8, Math.Ceiling(11.0 - dex / 10)); // Standard AOS Math
        bandageSeconds = Math.Max(bandageSeconds, 2); // Hard cap at 2.0 seconds

        // New Attribute Fetches
        int hpInc = AosAttributes.GetValue(_from, AosAttribute.BonusHits);
        int stamInc = AosAttributes.GetValue(_from, AosAttribute.BonusStam);
        int manaInc = AosAttributes.GetValue(_from, AosAttribute.BonusMana);

        int strBon = AosAttributes.GetValue(_from, AosAttribute.BonusStr);
        int dexBon = AosAttributes.GetValue(_from, AosAttribute.BonusDex);
        int intBon = AosAttributes.GetValue(_from, AosAttribute.BonusInt);

        int enhPot = AosAttributes.GetValue(_from, AosAttribute.EnhancePotions);

        // ModernUO: ammo-cost reduction lives on the equipped quiver (Layer.Cloak),
        // not as a global AOS attribute.
        int lAmmo = _from.FindItemOnLayer(Layer.Cloak) is BaseQuiver quiver
            ? quiver.LowerAmmoCost
            : 0;

        // TODO: AbsorptionAttributes / SAAbsorptionAttribute is a custom shard system
        // not present in ModernUO. Stubbed at 0 until it's wired back in.
        int dmgEater = 0;

        int tithing = (_from as PlayerMobile)?.TithingPoints ?? 0;

        // Account Gold & Followers
        long totalGold = 0;

        if (_from.Account is Account acct)
        {
            totalGold = acct.GetTotalGold();
        }

        string followers = $"{_from.Followers} / {_from.FollowersMax}";
                
        // TODO: PointsSystem / CleanUpBritannia is a custom shard system
        // not present in ModernUO. Stubbed at 0 until it's wired back in.
        double cubPoints = 0;

        // Left Side: Combat
        DrawStat(ref builder, 30, colY + 30, "HCI", $"{hci} / 45");
        DrawStat(ref builder, 30, colY + 50, "DCI", $"{dci} / 45");
        DrawStat(ref builder, 30, colY + 70, "DI", $"{di} / 100");
        DrawStat(ref builder, 30, colY + 90, "SSI", $"{ssi} / 60");

        DrawStat(ref builder, 30, colY + 130, "Swing Speed", swingSpeed > 0 ? $"{swingSpeed:F2}s" : "---");
        DrawStat(ref builder, 30, colY + 150, "Bandage Speed", $"{bandageSeconds:F1}s");

        DrawStat(ref builder, 30, colY + 185, "HP Regen", AosAttributes.GetValue(_from, AosAttribute.RegenHits).ToString());
        DrawStat(ref builder, 30, colY + 205, "Stam Regen", AosAttributes.GetValue(_from, AosAttribute.RegenStam).ToString());
        DrawStat(ref builder, 30, colY + 225, "Mana Regen", AosAttributes.GetValue(_from, AosAttribute.RegenMana).ToString());

        // Group 1: Increases
        DrawStat(ref builder, 30, colY + 265, "HP Increase", $"{hpInc} / 25");
        DrawStat(ref builder, 30, colY + 285, "Stam Increase", $"{stamInc} / 25");
        DrawStat(ref builder, 30, colY + 305, "Mana Increase", $"{manaInc} / 25");

        // Group 2: Stat Bonus
        DrawStat(ref builder, 30, colY + 345, "Str Bonus", strBon.ToString());
        DrawStat(ref builder, 30, colY + 365, "Dex Bonus", dexBon.ToString());
        DrawStat(ref builder, 30, colY + 385, "Int Bonus", intBon.ToString());

        // Group 3: Utility
        DrawStat(ref builder, 30, colY + 425, "Damage Eater", $"{dmgEater} / 30");
        DrawStat(ref builder, 30, colY + 445, "Enhance Pots", $"{enhPot} / 50");
        DrawStat(ref builder, 30, colY + 465, "Lower Ammo", $"{lAmmo}");

        DrawStat(ref builder, 30, colY + 505, "Followers", followers);
        DrawStat(ref builder, 30, colY + 525, "Weight", $"{_from.TotalWeight} / {_from.MaxWeight}");

        // Right Side: Magic
        DrawStat(ref builder, 340, colY + 30, "SDI", $"{sdi}%");
        DrawStat(ref builder, 340, colY + 50, "LRC", $"{lrc}%");
        DrawStat(ref builder, 340, colY + 70, "LMC", $"{lmc}%");
        DrawStat(ref builder, 340, colY + 90, "FC", $"{fc} / 4");
        DrawStat(ref builder, 340, colY + 110, "FCR", $"{fcr} / 6");

        // Miscellaneous Section
        builder.AddLabel(340, colY + 140, C_YELLOW, "Miscellaneous");
        DrawStat(ref builder, 340, colY + 165, "Luck", AosAttributes.GetValue(_from, AosAttribute.Luck).ToString());
        DrawStat(ref builder, 340, colY + 185, "Fame", _from.Fame.ToString());
        DrawStat(ref builder, 340, colY + 205, "Karma", _from.Karma.ToString());
        DrawStat(ref builder, 340, colY + 225, "Tithing", tithing.ToString());

        DrawStat(ref builder, 340, colY + 265, "Acct Gold", totalGold.ToString("N0"));
        DrawStat(ref builder, 340, colY + 285, "CUB Points", cubPoints.ToString("N0"));

        // --- SKILL LIST ---
        int skillY = colY + 365;
        builder.AddLabel(340, skillY - 20, C_YELLOW, "Skills");

        int count = 0;
        int startX = 340;
        int startY = skillY + 10;
        int rowSpacing = 20;

        // Filters out Human Jack of All Trades (20.0) unless base is invested or gear pushes it higher
        double humanMinimum = _from.Race == Race.Human ? 20.0 : 0.0;

        for (var i = 0; i < _from.Skills.Length; i++)
        {
            Skill sk = _from.Skills[i];

            if (sk.Base > 0 || (sk.Value > sk.Base && sk.Value > humanMinimum))
            {
                int yPos = startY + count * rowSpacing;

                builder.AddLabel(startX, yPos, C_WHITE, sk.Name);
                builder.AddLabel(startX + 130, yPos, C_WHITE, $"{sk.Value:F1} / {sk.Cap:F1}");

                count++;
            }
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID == 1) // Refresh
        {
            DisplayTo(_from);
        }
    }

    // --- HELPERS ---
    // DynamicGumpBuilder is a `ref struct`, so it can't be stored on the instance —
    // these helpers take it as an explicit `ref` parameter instead.

    private static string ColorAndCenter(string color, string text) =>
        $"<BASEFONT COLOR={color}><CENTER>{text}</CENTER></BASEFONT>";

    private static void DrawResistBox(ref DynamicGumpBuilder builder, int x, int y, string name, int val, int color)
    {
        builder.AddLabel(x, y, C_WHITE, name);
        builder.AddLabel(x + 55, y, color, $"{val}%");
    }

    private static void DrawStat(ref DynamicGumpBuilder builder, int x, int y, string name, string val)
    {
        builder.AddLabel(x, y, C_WHITE, name);
        builder.AddLabel(x + 130, y, C_WHITE, val);
    }

    private static void DrawDivider(ref DynamicGumpBuilder builder, int x, int y, int width)
    {
        builder.AddImageTiled(x, y, width, 2, 9264);
    }
}
