/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author Voxpire)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 *
 * ModernUO port: TownHouseSystem.cs
 *   TownHouseConfig, TownHouseSystem (registry + [TownHouses command), TownHouseIndexGump
 */

using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Custom.TownHouses;

public static class TownHouseConfig
{
    public static bool UseAccountOwnership = false;
    public static int HighlightStep = 2;
    public static TimeSpan HighlightDuration = TimeSpan.FromSeconds(30);
    public static int MaxOwnedHouses = 3;
}

/// <summary>
/// Registry + GM tools + events.
/// Controllers register themselves on creation and after deserialization,
/// and unregister in OnDelete, so no world-wide scan is needed on load.
/// </summary>
public static class TownHouseSystem
{
    private static readonly List<TownHouseController> _houses = [];

    public static IReadOnlyList<TownHouseController> Houses => _houses;

    public static Action<TownHouseController, Mobile> OnPurchased;
    public static Action<TownHouseController> OnEvicted;

    public static int GetOwnedCount(Mobile m)
    {
        if (m == null)
        {
            return 0;
        }

        var count = 0;
        var useAccount = TownHouseConfig.UseAccountOwnership;

        foreach (var c in _houses)
        {
            if (c == null || c.Deleted || c.Owner == null)
            {
                continue;
            }

            if (useAccount)
            {
                if (c.Owner.Account != null && m.Account != null && c.Owner.Account == m.Account)
                {
                    count++;
                }
            }
            else if (c.Owner == m)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Returns the controller of the town house the mobile is standing in,
    /// or null if they are not inside one or are not friend-or-above there.
    /// Used by the patched addon/steward/mannequin/locker scripts.
    /// </summary>
    public static TownHouseController GetFriendController(Mobile from)
    {
        var controller = from?.Region?.GetRegion<TownHouseRegion>()?.Controller;

        return controller is { Deleted: false } && controller.IsFriend(from) ? controller : null;
    }

    /// <summary>
    /// Returns the controller of the town house covering a world location, or null.
    /// </summary>
    public static TownHouseController GetControllerAt(Point3D location, Map map)
    {
        if (map == null || map == Map.Internal)
        {
            return null;
        }

        var controller = Region.Find(location, map)?.GetRegion<TownHouseRegion>()?.Controller;

        return controller is { Deleted: false } ? controller : null;
    }

    public static void Configure()
    {
        CommandSystem.Register("TownHouses", AccessLevel.GameMaster, TownHouses_OnCommand);

        OnPurchased += HandlePurchased;
    }

    private static void HandlePurchased(TownHouseController house, Mobile player)
    {
        if (player == null)
        {
            return;
        }

        player.PlaySound(0x5A7);
        player.SendMessage("The property deed has been registered in your name.");
    }

    public static void Register(TownHouseController c)
    {
        if (c == null || c.Deleted)
        {
            return;
        }

        if (!_houses.Contains(c))
        {
            _houses.Add(c);
        }
    }

    public static void Unregister(TownHouseController c)
    {
        if (c == null)
        {
            return;
        }

        _houses.Remove(c);
    }

    [Usage("TownHouses")]
    [Description("Opens the Town House global index.")]
    private static void TownHouses_OnCommand(CommandEventArgs e)
    {
        TownHouseIndexGump.DisplayTo(e.Mobile);
    }
}

public class TownHouseIndexGump : DynamicGump
{
    private const int PerPage = 14;

    private const int BtnRefresh = 900;
    private const int BtnPrev = 901;
    private const int BtnNext = 902;
    private const int RowButtonBase = 1000;

    private const int LabelHue = 0x481;
    private const int HeaderHue = 0x35;

    private readonly int _page;

    public override bool Singleton => true;

    private TownHouseIndexGump(int page) : base(70, 50) => _page = page;

    public static void DisplayTo(Mobile from, int page = 0)
    {
        if (from?.NetState == null)
        {
            return;
        }

        from.SendGump(new TownHouseIndexGump(ClampPage(page, GetHouses().Count)));
    }

    private static int ClampPage(int page, int total)
    {
        var maxPage = total == 0 ? 0 : (total - 1) / PerPage;
        return Math.Max(0, Math.Min(maxPage, page));
    }

    private static List<TownHouseController> GetHouses()
    {
        var list = new List<TownHouseController>(TownHouseSystem.Houses.Count);

        foreach (var c in TownHouseSystem.Houses)
        {
            if (c is { Deleted: false })
            {
                list.Add(c);
            }
        }

        return list;
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.SetNoResize();

        builder.AddPage();

        builder.AddBackground(0, 0, 720, 500, 9270);
        builder.AddAlphaRegion(10, 10, 700, 480);

        builder.AddHtml(
            10,
            20,
            700,
            25,
            "<CENTER><BASEFONT COLOR=#FFD700>TOWN HOUSE GLOBAL INDEX</BASEFONT></CENTER>"
        );
        builder.AddImageTiled(20, 45, 680, 2, 96);

        builder.AddButton(580, 55, 4017, 4019, BtnRefresh);
        builder.AddLabel(615, 55, LabelHue, "Refresh");

        var list = GetHouses();
        var total = list.Count;
        var maxPage = total == 0 ? 0 : (total - 1) / PerPage;

        var start = _page * PerPage;
        var end = Math.Min(start + PerPage, total);

        builder.AddLabel(22, 55, LabelHue, $"Total: {total}    Page: {_page + 1}/{maxPage + 1}");

        var y = 85;
        builder.AddLabel(25, y, HeaderHue, "MAP");
        builder.AddLabel(100, y, HeaderHue, "LOCATION");
        builder.AddLabel(215, y, HeaderHue, "STATE");
        builder.AddLabel(325, y, HeaderHue, "OWNER");
        builder.AddLabel(500, y, HeaderHue, "ACTIONS");
        y += 22;

        var buttonId = RowButtonBase;
        for (var i = start; i < end; i++)
        {
            var c = list[i];

            builder.AddLabel(25, y, LabelHue, c.Map?.Name ?? "Internal");
            builder.AddLabel(100, y, LabelHue, $"{c.X}, {c.Y}, {c.Z}");
            builder.AddLabel(215, y, LabelHue, $"{c.State}");
            builder.AddLabel(325, y, LabelHue, c.Owner?.Name ?? "- Unowned -");

            const int btnX = 480;

            // Go
            builder.AddButton(btnX, y + 3, 2117, 2118, buttonId);
            builder.AddLabel(btnX + 20, y, LabelHue, "Go");

            // Open
            builder.AddButton(btnX + 55, y + 3, 2117, 2118, buttonId + 1);
            builder.AddLabel(btnX + 75, y, LabelHue, "Open");

            // Evict
            builder.AddButton(btnX + 130, y + 3, 2117, 2118, buttonId + 2);
            builder.AddLabel(btnX + 150, y, 33, "Evict");

            y += 22;
            buttonId += 3;
        }

        if (_page > 0)
        {
            builder.AddButton(22, 460, 4014, 4016, BtnPrev);
        }

        if (_page < maxPage)
        {
            builder.AddButton(670, 460, 4005, 4007, BtnNext);
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;

        if (from == null || from.AccessLevel < AccessLevel.GameMaster)
        {
            return;
        }

        var id = info.ButtonID;

        switch (id)
        {
            case 0:
                {
                    return;
                }
            case BtnRefresh:
                {
                    DisplayTo(from, _page);
                    return;
                }
            case BtnPrev:
                {
                    DisplayTo(from, _page - 1);
                    return;
                }
            case BtnNext:
                {
                    DisplayTo(from, _page + 1);
                    return;
                }
        }

        var offset = id - RowButtonBase;
        if (offset < 0)
        {
            return;
        }

        var list = GetHouses();
        var index = _page * PerPage + offset / 3;
        var action = offset % 3;

        if (index >= list.Count)
        {
            DisplayTo(from, _page);
            return;
        }

        var house = list[index];

        switch (action)
        {
            case 0: // Go
                {
                    from.MoveToWorld(house.Location, house.Map);
                    DisplayTo(from, _page);
                    break;
                }
            case 1: // Open
                {
                    TownHouseSignGump.DisplayTo(from, house);
                    break;
                }
            case 2: // Evict
                {
                    house.EvictOwner();
                    from.SendMessage("The property has been reclaimed.");
                    DisplayTo(from, _page);
                    break;
                }
        }
    }
}
