/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Logging;
using Server.Network;
using Server.Targeting;

namespace Server.Gumps;

public class VirtualStorageGump : DynamicGump
{
    private static readonly ILogger logger = LogFactory.GetLogger(typeof(VirtualStorageGump));

    private const int ItemsPerPage = 16;
    private const int Width = 350;
    private const int Height = 450;
    private const int MaxInteractionRange = 25;
    private const int MaxWithdrawAmount = 100;

    private const int ButtonPrevPage = 1;
    private const int ButtonNextPage = 2;
    private const int ButtonAdd = 9999;
    private const int ButtonWithdrawBase = 1000;

    private static readonly HashSet<string> _gemNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Amber", "Amethyst", "Citrine", "Diamond", "Emerald", "Ruby", "Sapphire", "Tourmaline", "StarSapphire"
    };

    private static readonly Comparison<Type> _sortComparison = (a, b) =>
    {
        var pA = GetSortPriority(a);
        var pB = GetSortPriority(b);
        return pA != pB ? pA.CompareTo(pB) : string.Compare(a.Name, b.Name, StringComparison.Ordinal);
    };

    private readonly VirtualStorageChest _chest;
    private readonly List<Type> _keys;
    private readonly int _page;

    public override bool Singleton => true;

    private VirtualStorageGump(VirtualStorageChest chest, int page) : base(50, 50)
    {
        _chest = chest;

        // Snapshot the sorted order once so button indexes always match what the player is looking at.
        _keys = new List<Type>(chest.Content.Keys);
        _keys.Sort(_sortComparison);

        var lastPage = Math.Max(0, (_keys.Count - 1) / ItemsPerPage);
        _page = Math.Clamp(page, 0, lastPage);
    }

    public static void DisplayTo(Mobile from, VirtualStorageChest chest, int page = 0)
    {
        if (from?.NetState == null || chest?.Deleted != false)
        {
            return;
        }

        from.SendGump(new VirtualStorageGump(chest, page));
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();
        builder.AddBackground(0, 0, Width, Height, 9270);
        builder.AddHtml(
    0,
    20,
    Width,
    20,
    $"<center><BASEFONT COLOR=#FFFF00 size=7>{_chest.ChestTitle}</BASEFONT></center>",
    background: false,
    scrollbar: false
);
        builder.AddButton(25, 20, 4005, 4007, ButtonAdd);
        builder.AddLabel(60, 20, 1152, "ADD");

        var y = 55;
        builder.AddLabel(70, y, 1152, "Item Name");
        builder.AddLabel(240, y, 1152, "Count");
        builder.AddImageTiled(20, y + 22, Width - 40, 2, 96);

        var start = _page * ItemsPerPage;
        var end = start + ItemsPerPage;
        var count = _keys.Count;
        y += 30;

        for (var i = start; i < end && i < count; i++)
        {
            var type = _keys[i];
            _chest.Content.TryGetValue(type, out var held);

            builder.AddButton(30, y + 3, 0x837, 0x838, ButtonWithdrawBase + i);
            builder.AddLabel(50, y, 0x481, type.Name);
            builder.AddLabel(240, y, 0x481, $"{held}");
            y += 20;
        }

        if (end < count)
        {
            builder.AddButton(Width - 40, 20, 0x15E1, 0x15E5, ButtonNextPage);
        }

        if (_page > 0)
        {
            builder.AddButton(Width - 75, 20, 0x15E3, 0x15E7, ButtonPrevPage);
        }
    }

    private static int GetSortPriority(Type t)
    {
        var name = t.Name;

        if (name.Contains("Board")) { return 1; }
        if (name.Contains("Log")) { return 2; }
        if (name.Contains("Ingot")) { return 10; }
        if (name.Contains("Granite")) { return 11; }
        if (name.Contains("Scale")) { return 12; }
        if (name.Contains("Leather") || name.Contains("Hide")) { return 20; }
        if (name.Contains("Cloth")) { return 21; }
        if (_gemNames.Contains(name)) { return 30; }

        return name switch
        {
            // Magery Circle 1
            "ClumsyScroll"                => 101,
            "CreateFoodScroll"            => 102,
            "FeeblemindScroll"            => 103,
            "HealScroll"                  => 104,
            "MagicArrowScroll"            => 105,
            "NightSightScroll"            => 106,
            "ReactiveArmorScroll"         => 107,
            "WeakenScroll"                => 108,
            // Magery Circle 2
            "AgilityScroll"               => 109,
            "CunningScroll"               => 110,
            "CureScroll"                  => 111,
            "HarmScroll"                  => 112,
            "MagicTrapScroll"             => 113,
            "MagicUntrapScroll"           => 114,
            "ProtectionScroll"            => 115,
            "StrengthScroll"              => 116,
            // Magery Circle 3
            "BlessScroll"                 => 117,
            "FireballScroll"              => 118,
            "MagicLockScroll"             => 119,
            "PoisonScroll"                => 120,
            "TelekinesisScroll"           => 121,
            "TeleportScroll"              => 122,
            "UnlockScroll"                => 123,
            "WallOfStoneScroll"           => 124,
            // Magery Circle 4
            "ArchCureScroll"              => 125,
            "ArchProtectionScroll"        => 126,
            "CurseScroll"                 => 127,
            "FireFieldScroll"             => 128,
            "GreaterHealScroll"           => 129,
            "LightningScroll"             => 130,
            "ManaDrainScroll"             => 131,
            "RecallScroll"                => 132,
            // Magery Circle 5
            "BladeSpiritsScroll"          => 133,
            "DispelFieldScroll"           => 134,
            "IncognitoScroll"             => 135,
            "MagicReflectionScroll"       => 136,
            "MindBlastScroll"             => 137,
            "ParalyzeScroll"              => 138,
            "PoisonFieldScroll"           => 139,
            "SummonCreatureScroll"        => 140,
            // Magery Circle 6
            "DispelScroll"                => 141,
            "EnergyBoltScroll"            => 142,
            "ExplosionScroll"             => 143,
            "InvisibilityScroll"          => 144,
            "MarkScroll"                  => 145,
            "MassCurseScroll"             => 146,
            "ParalyzeFieldScroll"         => 147,
            "RevealScroll"                => 148,
            // Magery Circle 7
            "ChainLightningScroll"        => 149,
            "EnergyFieldScroll"           => 150,
            "FlamestrikeScroll"           => 151,
            "GateTravelScroll"            => 152,
            "ManaVampireScroll"           => 153,
            "MassDispelScroll"            => 154,
            "MeteorSwarmScroll"           => 155,
            "PolymorphScroll"             => 156,
            // Magery Circle 8
            "EarthquakeScroll"            => 157,
            "EnergyVortexScroll"          => 158,
            "ResurrectionScroll"          => 159,
            "SummonAirElementalScroll"    => 160,
            "SummonDaemonScroll"          => 161,
            "SummonEarthElementalScroll"  => 162,
            "SummonFireElementalScroll"   => 163,
            "SummonWaterElementalScroll"  => 164,

            // Necromancy
            "CurseWeaponScroll"           => 201,
            "BloodOathScroll"             => 202,
            "CorpseSkinScroll"            => 203,
            "EvilOmenScroll"              => 204,
            "PainSpikeScroll"             => 205,
            "WraithFormScroll"            => 206,
            "MindRotScroll"               => 207,
            "SummonFamiliarScroll"        => 208,
            "AnimateDeadScroll"           => 209,
            "HorrificBeastScroll"         => 210,
            "PoisonStrikeScroll"          => 211,
            "WitherScroll"                => 212,
            "StrangleScroll"              => 213,
            "LichFormScroll"              => 214,
            "ExorcismScroll"              => 215,
            "VengefulSpiritScroll"        => 216,
            "VampiricEmbraceScroll"       => 217,

            // Mysticism
            "HealingStoneScroll"          => 301,
            "NetherBoltScroll"            => 302,
            "EnchantScroll"               => 303,
            "PurgeMagicScroll"            => 304,
            "EagleStrikeScroll"           => 305,
            "SleepScroll"                 => 306,
            "AnimatedWeaponScroll"        => 307,
            "StoneFormScroll"             => 308,
            "MassSleepScroll"             => 309,
            "SpellTriggerScroll"          => 310,
            "BombardScroll"               => 311,
            "CleansingWindsScroll"        => 312,
            "HailStormScroll"             => 313,
            "SpellPlagueScroll"           => 314,
            "NetherCycloneScroll"         => 315,
            "RisingColossusScroll"        => 316,

            // Spellweaving
            "ArcaneCircleScroll"          => 401,
            "AttuneWeaponScroll"          => 402,
            "GiftOfRenewalScroll"         => 403,
            "NaturesFuryScroll"           => 404,
            "ImmolatingWeaponScroll"      => 405,
            "ThunderstormScroll"          => 406,
            "ArcaneEmpowermentScroll"     => 407,
            "EtherealVoyageScroll"        => 408,
            "ReaperFormScroll"            => 409,
            "GiftOfLifeScroll"            => 410,
            "SummonFeyScroll"             => 411,
            "SummonFiendScroll"           => 412,
            "DryadAllureScroll"           => 413,
            "EssenceOfWindScroll"         => 414,
            "WildfireScroll"              => 415,
            "WordOfDeathScroll"           => 416,

            _ when t == typeof(Bottle)    => 999,
            _                             => 500
        };
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;
        var buttonId = info.ButtonID;

        if (buttonId == 0 || _chest.Deleted || !from.InRange(_chest.GetWorldLocation(), MaxInteractionRange))
        {
            return;
        }

        switch (buttonId)
        {
            case ButtonAdd:
                {
                    from.SendMessage("Target an item to add.");
                    from.Target = new AddTarget(_chest);
                    return;
                }
            case ButtonPrevPage:
                {
                    DisplayTo(from, _chest, _page - 1);
                    return;
                }
            case ButtonNextPage:
                {
                    DisplayTo(from, _chest, _page + 1);
                    return;
                }
        }

        if (buttonId >= ButtonWithdrawBase)
        {
            var index = buttonId - ButtonWithdrawBase;

            if (index < _keys.Count)
            {
                Withdraw(from, _keys[index]);
            }

            DisplayTo(from, _chest, _page);
        }
    }

    private void Withdraw(Mobile from, Type type)
    {
        if (!_chest.Content.TryGetValue(type, out var held))
        {
            return;
        }

        var amount = Math.Min(held, MaxWithdrawAmount);

        Item item;
        try
        {
            item = Activator.CreateInstance(type) as Item;
        }
        catch (Exception e)
        {
            logger.Warning(e, "Failed to create {Type} for withdrawal from {Chest}", type, _chest.Serial);
            item = null;
        }

        if (item == null)
        {
            from.SendMessage("Error.");
            return;
        }

        item.Amount = amount;

        if (from.Backpack?.CheckHold(from, item, true) != true)
        {
            item.Delete();
            from.SendMessage("Backpack full.");
            return;
        }

        from.AddToBackpack(item);
        _chest.Withdraw(type, amount);
    }

    private class AddTarget : Target
    {
        private readonly VirtualStorageChest _chest;

        public AddTarget(VirtualStorageChest chest) : base(12, false, TargetFlags.None) => _chest = chest;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_chest.Deleted || !from.InRange(_chest.GetWorldLocation(), MaxInteractionRange))
            {
                return;
            }

            var added = 0;

            if (targeted is Container container)
            {
                // Collect first: TryAdd deletes items, which would otherwise mutate the list being walked.
                var found = new List<Item>();
                CollectItems(container, found);

                foreach (var item in found)
                {
                    if (_chest.TryAdd(item) > 0)
                    {
                        added++;
                    }
                }
            }
            else if (targeted is Item single && _chest.TryAdd(single) > 0)
            {
                added++;
            }

            if (added > 0)
            {
                from.PlaySound(0x42);
                from.SendMessage($"Added {added} items.");
            }

            DisplayTo(from, _chest);
        }

        private static void CollectItems(Container container, List<Item> found)
        {
            foreach (var item in container.Items)
            {
                if (item is Container sub)
                {
                    CollectItems(sub, found);
                }
                else
                {
                    found.Add(item);
                }
            }
        }
    }
}
