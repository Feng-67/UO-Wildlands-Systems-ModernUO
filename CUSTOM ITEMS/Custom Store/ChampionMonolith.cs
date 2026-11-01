/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Engines.CannedEvil;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class ChampionMonolith : Item
{
    public override string DefaultName => "Champion Monolith";

    [Constructible]
    public ChampionMonolith() : base(41066) // 0xA06A sculpture_wood_skull
    {
    }

    public override void OnDoubleClick(Mobile from)
    {
        // Staff can test it anywhere, players need it locked down
        if (!IsLockedDown && Movable && from.AccessLevel == AccessLevel.Player)
        {
            from.SendMessage("The Monolith's power can only be harnessed while it is locked down.");
            return;
        }

        if (!from.InRange(GetWorldLocation(), 3))
        {
            from.SendLocalizedMessage(500446); // That is too far away.
            return;
        }

        ChampionMonolithGump.DisplayTo(from, this);
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

public class ChampionMonolithGump : DynamicGump
{
    private const int EntryHeight = 25;
    private const int Width = 380;

    // Hand-picked arrival points, keyed by the altar's name
    private static readonly Dictionary<string, Point3D> _teleportMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ABYSSAL"] = new Point3D(7051, 727, 32),
        ["BEDLAM"] = new Point3D(153, 1610, 0),
        ["CITY-OF-THE-DEAD"] = new Point3D(5241, 3640, -1),
        ["DAMWIN-THICKET"] = new Point3D(5324, 3366, 0),
        ["DECEIT"] = new Point3D(5155, 676, 0),
        ["DESERT"] = new Point3D(5636, 2952, 17),
        ["DESPISE"] = new Point3D(5522, 837, 50),
        ["DESTARD"] = new Point3D(5212, 846, 0),
        ["DRAGON-TURTLE-EODON"] = new Point3D(473, 1720, 40),
        ["DRAGON-TURTLE-FEL"] = new Point3D(7067, 1916, 40),
        ["FIRE"] = new Point3D(5820, 1388, 0),
        ["HOPPERS-BOG"] = new Point3D(5976, 3448, 3),
        ["HUMILTY"] = new Point3D(430, 926, -83), // Spelling matches the altar names on your shard
        ["ICE-EAST"] = new Point3D(5993, 2425, 26),
        ["ICE-WEST"] = new Point3D(5548, 2359, 19),
        ["KHALDUN-DUNGEON-FEL"] = new Point3D(6012, 3780, 18),
        ["KHALDUN-DUNGEON-TRAM"] = new Point3D(6012, 3780, 19),
        ["KHALDUN-OVERLAND"] = new Point3D(5949, 3886, 19),
        ["MARBLE"] = new Point3D(5228, 3179, 9),
        ["OAKS"] = new Point3D(5597, 3759, 28),
        ["OAKS-ILSHENAR"] = new Point3D(1687, 1104, 5),
        ["OASIS"] = new Point3D(5559, 2669, -5),
        ["PRIMEVAL-LICH"] = new Point3D(6960, 1030, -15),
        ["SLEEPING-DRAGON"] = new Point3D(915, 409, 12),
        ["TERA-KEEP"] = new Point3D(5135, 1589, 0),
        ["TERRA-SANCTUM"] = new Point3D(6022, 2914, 16),
        ["TORTOISE"] = new Point3D(5747, 4015, 1),
        ["TWISTED-WEALD"] = new Point3D(1450, 1475, -26),
        ["VALOR"] = new Point3D(338, 308, -54),
        ["ORC-DUNGEON"] = new Point3D(5294, 2031, 0),
        ["HYTHLOTH"] = new Point3D(6049, 48, 0)
    };

    private readonly ChampionMonolith _monolith;
    private readonly List<ChampionSpawn> _spawns;

    public override bool Singleton => true;

    private ChampionMonolithGump(ChampionMonolith monolith, List<ChampionSpawn> spawns) : base(100, 100)
    {
        _monolith = monolith;
        _spawns = spawns;
    }

    public static void DisplayTo(Mobile from, ChampionMonolith monolith)
    {
        if (from?.NetState == null || monolith?.Deleted != false)
        {
            return;
        }

        // ModernUO keeps no public list of champion spawns (GenChamps scans the world the same way),
        // so the list is gathered each time the monolith is opened.
        var spawns = new List<ChampionSpawn>();

        foreach (var item in World.Items.Values)
        {
            if (item is ChampionSpawn spawn)
            {
                spawns.Add(spawn);
            }
        }

        spawns.Sort((a, b) => string.Compare(GetSortName(a), GetSortName(b), StringComparison.Ordinal));

        from.SendGump(new ChampionMonolithGump(monolith, spawns));
    }

    private static string GetSortName(ChampionSpawn spawn) => spawn.Name ?? spawn.Champion?.Name ?? "Unknown";

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        var totalHeight = 100 + _spawns.Count * EntryHeight + 20;

        builder.AddPage();
        builder.AddBackground(0, 0, Width, totalHeight, 9270);
        builder.AddAlphaRegion(10, 10, Width - 20, totalHeight - 20);
        builder.AddHtml(10, 20, Width - 20, 25, "CHAMPION MONOLITH", "#FFD700", align: TextAlignment.Center);
        builder.AddImageTiled(20, 45, Width - 40, 2, 96);

        builder.AddLabel(55, 55, 0x35, "REGION");
        builder.AddLabel(240, 55, 0x35, "STATUS");

        var y = 80;

        for (var i = 0; i < _spawns.Count; i++)
        {
            var spawn = _spawns[i];
            var name = spawn.Name ?? "Unknown Altar";

            // Truncate long names for display
            var displayName = name.Length > 20 ? $"{name[..18]}.." : name;

            string status;
            int hue;

            if (spawn.Active)
            {
                status = "ACTIVE";
                hue = 63; // Green
            }
            else if (Core.Now >= spawn.RestartTime)
            {
                status = "READY";
                hue = 0x481; // White/gold
            }
            else
            {
                status = $"COOLDOWN ({(int)(spawn.RestartTime - Core.Now).TotalMinutes}m)";
                hue = 2401; // Grey
            }

            builder.AddButton(25, y + 3, 2224, 2224, i + 1);
            builder.AddLabel(55, y, hue, displayName);
            builder.AddLabel(240, y, hue, status);

            y += EntryHeight;
        }
    }

    public override void OnResponse(NetState state, in RelayInfo info)
    {
        var from = state.Mobile;
        var index = info.ButtonID - 1;

        if (index < 0 || index >= _spawns.Count)
        {
            return;
        }

        // The player has to still be beside the monolith
        if (_monolith.Deleted || !from.InRange(_monolith.GetWorldLocation(), 3))
        {
            from.SendLocalizedMessage(500446); // That is too far away.
            return;
        }

        var target = _spawns[index];
        var map = target.Map;

        if (target.Deleted || map == null || map == Map.Internal)
        {
            return;
        }

        var name = target.Name;
        Point3D destination;

        if (name != null && _teleportMap.TryGetValue(name, out var known))
        {
            destination = known;
        }
        else
        {
            // Fallback: just off the altar
            var loc = target.Location;
            destination = new Point3D(loc.X + 10, loc.Y + 10, map.GetAverageZ(loc.X + 10, loc.Y + 10));
            from.SendMessage(33, "Warning: Precision coordinates not found. Using approximate location.");
        }

        // Pets go first, while the player is still at the old location
        BaseCreature.TeleportPets(from, destination, map);
        from.MoveToWorld(destination, map);

        from.SendMessage(0x22, $"You travel to the {name ?? "Champion"} region.");
        from.PlaySound(0x1FE);
    }
}
