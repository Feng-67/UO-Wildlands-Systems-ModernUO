/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author Voxpire)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 *
 * ModernUO port: TownHouseSignGump.cs
 */

using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Server.Custom.TownHouses;

public class TownHouseSignGump : DynamicGump
{
    // Pages ("tabs")
    private const int PageMain = 0;
    private const int PageFriends = 1;
    private const int PageCoOwners = 2;
    private const int PageBans = 3;
    private const int PageSaleSetup = 4;

    // TextEntry IDs
    private const int TePurchase = 10;
    private const int TeLockLimit = 13;
    private const int TeMinZ = 14;
    private const int TeMaxZ = 15;
    private const int TeSalePrice = 20;

    // Button IDs
    private const int BtnPurchase = 1;
    private const int BtnTransfer = 2;
    private const int BtnLockdown = 3;
    private const int BtnSecure = 4;
    private const int BtnRelease = 5;
    private const int BtnSellSetup = 6;
    private const int BtnSellConfirm = 7;
    private const int BtnCancelSale = 8;
    private const int BtnBuyFromOwner = 9;
    private const int BtnAbandon = 10;
    private const int BtnBackToMain = 11;
    private const int BtnAddTarget = 12;
    private const int BtnListNext = 13;
    private const int BtnListPrev = 14;
    private const int BtnTogglePublic = 15;

    private const int BtnRemoveBase = 1000;

    // GM IDs (contiguous range, checked as a group in OnResponse)
    private const int BtnGmSetBounds = 50;
    private const int BtnGmHighlight = 51;
    private const int BtnGmCycleState = 52;
    private const int BtnGmToggleWipe = 53;
    private const int BtnGmApplySettings = 54;
    private const int BtnGmEvict = 55;

    // Nav Buttons
    private const int BtnOpenFriends = 101;
    private const int BtnOpenCoOwners = 102;
    private const int BtnOpenBans = 103;

    // Colors
    private const int LabelHue = 0x481;
    private const int HeaderHue = 0x35;
    private const int ValueHue = 2213;

    private const int ListPerPage = 10;

    private readonly Mobile _from;
    private readonly TownHouseController _c;
    private readonly int _page;
    private readonly int _listPage;

    public override bool Singleton => true;

    private TownHouseSignGump(Mobile from, TownHouseController c, int page, int listPage) : base(50, 50)
    {
        _from = from;
        _c = c;
        _page = page;
        _listPage = listPage;
    }

    public static void DisplayTo(Mobile from, TownHouseController c, int page = PageMain, int listPage = 0)
    {
        if (from?.NetState == null || c == null || c.Deleted)
        {
            return;
        }

        var list = GetList(c, page);
        if (list != null)
        {
            var maxPage = Math.Max(0, (list.Count - 1) / ListPerPage);
            listPage = Math.Max(0, Math.Min(maxPage, listPage));
        }
        else
        {
            listPage = 0;
        }

        from.SendGump(new TownHouseSignGump(from, c, page, listPage));
    }

    private static List<Mobile> GetList(TownHouseController c, int page) =>
        page switch
        {
            PageFriends  => c.Friends,
            PageCoOwners => c.CoOwners,
            PageBans     => c.Bans,
            _            => null
        };

    private bool IsGm => _from.AccessLevel >= AccessLevel.GameMaster;

    private bool CanManageLists => _c.IsOwner(_from) || _c.IsCoOwner(_from) || IsGm;

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.SetNoResize();

        builder.AddPage();
        builder.AddBackground(0, 0, 600, 520, 9270);
        builder.AddAlphaRegion(10, 10, 580, 500);

        builder.AddHtml(10, 20, 580, 25, "<CENTER><BASEFONT COLOR=#FFD700>TOWN HOUSE PROPERTY</BASEFONT></CENTER>");
        builder.AddImageTiled(20, 45, 560, 2, 96);

        switch (_page)
        {
            case PageMain:
                {
                    BuildMain(ref builder);
                    break;
                }
            case PageFriends:
                {
                    BuildList(ref builder, _c.Friends, "FRIENDS LIST", "Add Friend");
                    break;
                }
            case PageCoOwners:
                {
                    BuildList(ref builder, _c.CoOwners, "CO-OWNER LIST", "Add Co-Owner");
                    break;
                }
            case PageBans:
                {
                    BuildList(ref builder, _c.Bans, "BAN LIST", "Ban Player");
                    break;
                }
            case PageSaleSetup:
                {
                    BuildSaleSetup(ref builder);
                    break;
                }
        }
    }

    private void BuildMain(ref DynamicGumpBuilder builder)
    {
        var y = 60;

        builder.AddLabel(30, y, HeaderHue, "PROPERTY STATUS");
        y += 25;
        builder.AddLabel(30, y, LabelHue, "State:");
        builder.AddLabel(100, y, LabelHue, $"{_c.State}");
        y += 20;
        builder.AddLabel(30, y, LabelHue, "Owner:");
        builder.AddLabel(100, y, LabelHue, _c.Owner?.Name ?? "None");
        y += 40;

        if (_c.Owner == null)
        {
            if (_c.State == TownHouseState.Purchasable)
            {
                builder.AddLabel(30, y, HeaderHue, "PURCHASE OPTION");
                y += 25;
                builder.AddButton(30, y, 4005, 4007, BtnPurchase);
                builder.AddLabel(65, y, LabelHue, "Purchase Property");
                builder.AddLabel(65, y + 20, 0x35, $"{_c.PurchasePrice:N0} GP");
                y += 45;
            }
        }
        else
        {
            if (_c.IsForSale && !_c.IsOwner(_from))
            {
                builder.AddLabel(30, y, HeaderHue, "FOR SALE BY OWNER");
                y += 25;
                builder.AddButton(30, y, 4005, 4007, BtnBuyFromOwner);
                builder.AddLabel(65, y, LabelHue, "Buy House");
                builder.AddLabel(65, y + 20, 0x35, $"{_c.OwnerSalePrice:N0} GP");
                y += 45;
            }

            if (_c.IsOwner(_from) || IsGm)
            {
                builder.AddLabel(30, y, HeaderHue, "OWNER CONTROLS");
                y += 25;

                if (!_c.IsForSale)
                {
                    builder.AddButton(30, y, 4005, 4007, BtnSellSetup);
                    builder.AddLabel(65, y, LabelHue, "Sell House (Set Price)");
                }
                else
                {
                    builder.AddButton(30, y, 4005, 4007, BtnCancelSale);
                    builder.AddLabel(65, y, LabelHue, "Cancel Sale");
                }

                y += 25;

                if (_c.State == TownHouseState.Transferable)
                {
                    builder.AddButton(30, y, 4005, 4007, BtnTransfer);
                    builder.AddLabel(65, y, LabelHue, "Transfer Ownership");
                    y += 25;
                }

                builder.AddButton(30, y, 4005, 4007, BtnAbandon);
                builder.AddLabel(65, y, LabelHue, "Abandon House");
                y += 25;

                builder.AddButton(30, y, 4005, 4007, BtnTogglePublic);
                builder.AddLabel(
                    65,
                    y,
                    LabelHue,
                    _c.IsPublic ? "Access: Public (click to make Private)" : "Access: Private (click to make Public)"
                );
                y += 35;
            }
        }

        if (_c.IsFriend(_from))
        {
            builder.AddLabel(30, y, HeaderHue, "ITEM SECURITY");
            y += 25;
            builder.AddButton(30, y, 4005, 4007, BtnLockdown);
            builder.AddLabel(65, y, LabelHue, "Lockdown Item");
            y += 25;
            builder.AddButton(30, y, 4005, 4007, BtnSecure);
            builder.AddLabel(65, y, LabelHue, "Secure Container");
            y += 25;
            builder.AddButton(30, y, 4005, 4007, BtnRelease);
            builder.AddLabel(65, y, LabelHue, "Release Item");
            y += 35;
        }

        // Visible to Co-Owners (includes Owner)
        if (CanManageLists)
        {
            builder.AddLabel(30, y, HeaderHue, "ACCESS LISTS");
            y += 25;
            builder.AddButton(30, y, 4011, 4013, BtnOpenFriends);
            builder.AddLabel(65, y, LabelHue, $"Friends ({_c.Friends.Count})");
            y += 25;
            builder.AddButton(30, y, 4011, 4013, BtnOpenCoOwners);
            builder.AddLabel(65, y, LabelHue, $"Co-Owners ({_c.CoOwners.Count})");
            y += 25;
            builder.AddButton(30, y, 4011, 4013, BtnOpenBans);
            builder.AddLabel(65, y, LabelHue, $"Bans ({_c.Bans.Count})");
        }

        const int col2 = 300;
        var y2 = 60;

        builder.AddLabel(col2, y2, HeaderHue, "PROPERTY DETAILS");
        y2 += 25;
        builder.AddLabel(col2, y2, LabelHue, "Lockdowns:");
        builder.AddLabel(col2 + 100, y2, LabelHue, $"{_c.Lockdowns.Count} / {_c.LockdownLimit}");
        y2 += 20;
        builder.AddLabel(col2, y2, LabelHue, "Secures:");
        builder.AddLabel(col2 + 100, y2, LabelHue, $"{_c.Secures.Count}");
        y2 += 20;
        builder.AddLabel(col2, y2, LabelHue, "Total Area:");
        builder.AddLabel(col2 + 100, y2, LabelHue, $"{_c.Rect.Width * _c.Rect.Height} Tiles");
        y2 += 30;

        if (!IsGm)
        {
            return;
        }

        builder.AddImageTiled(col2 - 10, y2, 260, 2, 96);
        y2 += 10;
        builder.AddLabel(col2, y2, HeaderHue, "GM CONFIGURATION");
        y2 += 25;
        builder.AddButton(col2, y2, 4011, 4013, BtnGmSetBounds);
        builder.AddLabel(col2 + 35, y2, LabelHue, "Set Bounds");
        y2 += 25;
        builder.AddButton(col2, y2, 4011, 4013, BtnGmHighlight);
        builder.AddLabel(col2 + 35, y2, LabelHue, "Highlight Bounds");
        y2 += 25;
        builder.AddButton(col2, y2, 4011, 4013, BtnGmCycleState);
        builder.AddLabel(col2 + 35, y2, LabelHue, "Cycle State");
        y2 += 25;
        builder.AddButton(col2, y2, 4011, 4013, BtnGmToggleWipe);
        builder.AddLabel(col2 + 35, y2, LabelHue, $"Wipe Policy: {_c.WipePolicy}");
        y2 += 25;
        builder.AddButton(col2, y2, 4011, 4013, BtnGmEvict);
        builder.AddLabel(col2 + 35, y2, 33, "EVICT & RECLAIM");
        y2 += 35;

        builder.AddLabel(col2, y2, HeaderHue, "SETTINGS");
        y2 += 20;
        builder.AddLabel(col2, y2, LabelHue, "Price:");
        builder.AddImageTiled(col2 + 70, y2, 80, 20, 2624);
        builder.AddTextEntry(col2 + 72, y2, 76, 20, ValueHue, TePurchase, $"{_c.PurchasePrice}");
        y2 += 22;
        builder.AddLabel(col2, y2, LabelHue, "Locks:");
        builder.AddImageTiled(col2 + 70, y2, 80, 20, 2624);
        builder.AddTextEntry(col2 + 72, y2, 76, 20, ValueHue, TeLockLimit, $"{_c.LockdownLimit}");
        y2 += 22;
        builder.AddLabel(col2, y2, LabelHue, "Z-Range:");
        builder.AddImageTiled(col2 + 70, y2, 35, 20, 2624);
        builder.AddTextEntry(col2 + 72, y2, 31, 20, ValueHue, TeMinZ, $"{_c.MinZ}");
        builder.AddLabel(col2 + 110, y2, LabelHue, "to");
        builder.AddImageTiled(col2 + 130, y2, 35, 20, 2624);
        builder.AddTextEntry(col2 + 132, y2, 31, 20, ValueHue, TeMaxZ, $"{_c.MaxZ}");
        y2 += 30;
        builder.AddButton(col2, y2, 4005, 4007, BtnGmApplySettings);
        builder.AddLabel(col2 + 35, y2, LabelHue, "APPLY SETTINGS");
    }

    private void BuildList(ref DynamicGumpBuilder builder, List<Mobile> list, string title, string addText)
    {
        builder.AddButton(30, 60, 4014, 4016, BtnBackToMain);
        builder.AddLabel(65, 60, LabelHue, "Return to Main Menu");

        builder.AddLabel(30, 90, HeaderHue, title);
        builder.AddImageTiled(30, 110, 250, 2, 96);

        var canEditList = _page == PageCoOwners ? _c.IsOwner(_from) : _c.IsOwner(_from) || _c.IsCoOwner(_from);
        if (canEditList || IsGm)
        {
            builder.AddButton(30, 120, 4005, 4007, BtnAddTarget);
            builder.AddLabel(65, 120, LabelHue, addText);
        }

        var count = list.Count;
        var maxPage = Math.Max(0, (count - 1) / ListPerPage);

        if (_listPage > 0)
        {
            builder.AddButton(200, 90, 4014, 4016, BtnListPrev);
        }

        builder.AddLabel(240, 90, LabelHue, $"{_listPage + 1}/{maxPage + 1}");

        if (_listPage < maxPage)
        {
            builder.AddButton(280, 90, 4005, 4007, BtnListNext);
        }

        var y = 150;

        if (count == 0)
        {
            builder.AddLabel(30, y, LabelHue, "(List is empty)");
            return;
        }

        var start = _listPage * ListPerPage;
        var end = Math.Min(start + ListPerPage, count);

        for (var i = start; i < end; i++)
        {
            builder.AddButton(30, y + 3, 4017, 4019, BtnRemoveBase + i);
            builder.AddLabel(65, y, LabelHue, list[i]?.Name ?? "Unknown");
            y += 25;
        }
    }

    private void BuildSaleSetup(ref DynamicGumpBuilder builder)
    {
        builder.AddButton(30, 60, 4014, 4016, BtnBackToMain);
        builder.AddLabel(65, 60, LabelHue, "Return to Main Menu");

        builder.AddLabel(30, 100, HeaderHue, "SELL HOUSE");
        builder.AddImageTiled(30, 120, 250, 2, 96);

        builder.AddLabel(30, 140, LabelHue, "Enter the price you wish to sell for:");

        builder.AddImageTiled(30, 165, 150, 20, 2624);
        builder.AddTextEntry(32, 165, 146, 20, ValueHue, TeSalePrice, $"{_c.OwnerSalePrice}");
        builder.AddLabel(185, 165, LabelHue, "GP");

        builder.AddButton(30, 200, 4005, 4007, BtnSellConfirm);
        builder.AddLabel(65, 200, 0x35, "CONFIRM SALE");

        builder.AddHtml(
            30,
            240,
            250,
            100,
            "<BASEFONT COLOR=#FFFFFF>Note: Once set, anyone can purchase the house from the sign. The gold will be deposited into your bank.</BASEFONT>"
        );
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;

        if (from == null || from.Deleted || _c.Deleted)
        {
            return;
        }

        var id = info.ButtonID;
        if (id == 0)
        {
            return;
        }

        // Every GM button is rejected up front so a forged response can't reach them.
        if (id is >= BtnGmSetBounds and <= BtnGmEvict && from.AccessLevel < AccessLevel.GameMaster)
        {
            return;
        }

        switch (id)
        {
            case BtnBackToMain:
                {
                    DisplayTo(from, _c);
                    return;
                }
            case BtnOpenFriends:
            case BtnOpenCoOwners:
            case BtnOpenBans:
                {
                    if (CanManageLists)
                    {
                        var page = id == BtnOpenFriends ? PageFriends
                            : id == BtnOpenCoOwners ? PageCoOwners
                            : PageBans;

                        DisplayTo(from, _c, page);
                    }

                    return;
                }
            case BtnSellSetup:
                {
                    DisplayTo(from, _c, PageSaleSetup);
                    return;
                }
            case BtnListNext:
                {
                    DisplayTo(from, _c, _page, _listPage + 1);
                    return;
                }
            case BtnListPrev:
                {
                    DisplayTo(from, _c, _page, Math.Max(0, _listPage - 1));
                    return;
                }
            case BtnPurchase:
                {
                    _c.TryPurchase(from);
                    break;
                }
            case BtnBuyFromOwner:
                {
                    _c.TryPurchaseFromOwner(from);
                    break;
                }
            case BtnCancelSale:
                {
                    _c.TryCancelSale(from);
                    break;
                }
            case BtnAbandon:
                {
                    _c.TryAbandon(from);
                    break;
                }
            case BtnTransfer:
                {
                    if (_c.IsOwner(from))
                    {
                        from.SendMessage("Target the player to transfer ownership to.");
                        from.Target = new TransferTarget(_c);
                        return;
                    }

                    break;
                }
            case BtnLockdown:
                {
                    if (_c.IsFriend(from))
                    {
                        from.SendMessage("Target an item to lock down.");
                        from.Target = new LockdownTarget(_c);
                        return;
                    }

                    break;
                }
            case BtnSecure:
                {
                    if (_c.IsFriend(from))
                    {
                        from.SendMessage("Target a container to secure.");
                        from.Target = new SecureTarget(_c);
                        return;
                    }

                    break;
                }
            case BtnRelease:
                {
                    if (_c.IsFriend(from))
                    {
                        from.SendMessage("Target an item to release.");
                        from.Target = new ReleaseTarget(_c);
                        return;
                    }

                    break;
                }
            case BtnSellConfirm:
                {
                    var price = ParseInt(info, TeSalePrice, 0);

                    if (_c.TryBeginSale(from, price))
                    {
                        from.SendMessage("House is now for sale.");
                        DisplayTo(from, _c);
                        return;
                    }

                    break;
                }
            case BtnAddTarget:
                {
                    var canEdit = _page == PageCoOwners
                        ? _c.IsOwner(from)
                        : _c.IsOwner(from) || _c.IsCoOwner(from);

                    if (canEdit || from.AccessLevel >= AccessLevel.GameMaster)
                    {
                        from.SendMessage("Target the player.");
                        from.Target = new AddListTarget(_c, _page);
                        return;
                    }

                    break;
                }
            case BtnGmSetBounds:
                {
                    from.Target = new TownHouseBoundsTarget1(_c);
                    return;
                }
            case BtnGmHighlight:
                {
                    _c.ShowHighlight(from, TownHouseConfig.HighlightDuration);
                    break;
                }
            case BtnGmCycleState:
                {
                    _c.CycleState();
                    break;
                }
            case BtnGmToggleWipe:
                {
                    _c.ToggleWipePolicy();
                    break;
                }
            case BtnGmEvict:
                {
                    _c.EvictOwner();
                    break;
                }
            case BtnGmApplySettings:
                {
                    ApplyGmSettings(from, info);
                    break;
                }
            case BtnTogglePublic:
                {
                    _c.TogglePublic(from);
                    break;
                }
        }

        if (id >= BtnRemoveBase)
        {
            var index = id - BtnRemoveBase;
            var list = GetList(_c, _page);

            if (list != null && index < list.Count)
            {
                var toRemove = list[index];

                switch (_page)
                {
                    case PageFriends:
                        {
                            _c.TryRemoveFriend(from, toRemove);
                            break;
                        }
                    case PageCoOwners:
                        {
                            _c.TryRemoveCoOwner(from, toRemove);
                            break;
                        }
                    case PageBans:
                        {
                            _c.TryUnban(from, toRemove);
                            break;
                        }
                }
            }
        }

        DisplayTo(from, _c, _page, _listPage);
    }

    private void ApplyGmSettings(Mobile from, in RelayInfo info)
    {
        var purchase = ParseInt(info, TePurchase, _c.PurchasePrice);
        var lockLimit = ParseInt(info, TeLockLimit, _c.LockdownLimit);
        var minZ = ParseInt(info, TeMinZ, _c.MinZ);
        var maxZ = ParseInt(info, TeMaxZ, _c.MaxZ);

        _c.ApplySettings(purchase, lockLimit, minZ, maxZ);
        from.SendMessage("Settings applied.");
    }

    private static int ParseInt(in RelayInfo info, int entryId, int fallback) =>
        int.TryParse(info.GetTextEntry(entryId)?.Trim(), out var value) ? value : fallback;

    private class LockdownTarget : Target
    {
        private readonly TownHouseController _c;

        public LockdownTarget(TownHouseController c) : base(30, false, TargetFlags.None) => _c = c;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_c != null && targeted is Item item)
            {
                _c.TryLockdown(from, item);
            }

            DisplayTo(from, _c);
        }
    }

    private class SecureTarget : Target
    {
        private readonly TownHouseController _c;

        public SecureTarget(TownHouseController c) : base(30, false, TargetFlags.None) => _c = c;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_c != null && targeted is Item item)
            {
                _c.TrySecure(from, item);
            }
            else
            {
                from.SendMessage("That cannot be secured.");
            }

            DisplayTo(from, _c);
        }
    }

    private class ReleaseTarget : Target
    {
        private readonly TownHouseController _c;

        public ReleaseTarget(TownHouseController c) : base(30, false, TargetFlags.None) => _c = c;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_c != null && targeted is Item item)
            {
                _c.TryRelease(from, item);
            }

            DisplayTo(from, _c);
        }
    }

    private class TransferTarget : Target
    {
        private readonly TownHouseController _c;

        public TransferTarget(TownHouseController c) : base(30, false, TargetFlags.None) => _c = c;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_c != null && targeted is Mobile { Player: true } to)
            {
                _c.TryTransfer(from, to);
            }

            DisplayTo(from, _c);
        }
    }

    private class AddListTarget : Target
    {
        private readonly TownHouseController _c;
        private readonly int _page;

        public AddListTarget(TownHouseController c, int page) : base(30, false, TargetFlags.None)
        {
            _c = c;
            _page = page;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_c != null && targeted is Mobile { Player: true } target)
            {
                switch (_page)
                {
                    case PageFriends:
                        {
                            _c.TryAddFriend(from, target);
                            break;
                        }
                    case PageCoOwners:
                        {
                            _c.TryAddCoOwner(from, target);
                            break;
                        }
                    case PageBans:
                        {
                            _c.TryBan(from, target);
                            break;
                        }
                }
            }

            DisplayTo(from, _c, _page);
        }
    }
}
