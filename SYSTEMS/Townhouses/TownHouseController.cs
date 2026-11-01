/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author Voxpire)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 *
 * ModernUO port: TownHouseController.cs
 *   TownHouseState / TownHouseWipePolicy enums
 *   TownHouseUtil / TownHouseHighlightTile
 *   TownHouseRegion
 *   TownHouseBoundsTarget1/2
 *   TownHouseController
 */

using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Collections;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Custom.TownHouses;

// -------------------------------------------------------------------------
// ENUMS
// -------------------------------------------------------------------------

public enum TownHouseState
{
    Purchasable,
    Rentable,
    Transferable
}

public enum TownHouseWipePolicy
{
    LeaveAsIs,
    WipeItemsInBounds
}

// -------------------------------------------------------------------------
// UTILITIES
// -------------------------------------------------------------------------

public static class TownHouseUtil
{
    public static Rectangle2D MakeRect(Point3D a, Point3D b)
    {
        var x1 = Math.Min(a.X, b.X);
        var y1 = Math.Min(a.Y, b.Y);
        var x2 = Math.Max(a.X, b.X);
        var y2 = Math.Max(a.Y, b.Y);

        return new Rectangle2D(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
    }

    public static bool HasBounds(Rectangle2D r) => r.Width > 0 && r.Height > 0;

    public static bool In3DRange(Point3D p, Rectangle2D r, int minZ, int maxZ) =>
        r.Contains(p) && p.Z >= minZ && p.Z <= maxZ;
}

/// <summary>
/// Temporary highlight tile used for GM visualization. Auto-deletes after the duration,
/// and is removed on load so markers never survive a restart.
/// </summary>
[SerializationGenerator(0)]
public partial class TownHouseHighlightTile : Item
{
    private TimerExecutionToken _timerToken;

    public override bool Decays => false;

    public override string DefaultName => "boundary marker";

    [Constructible]
    public TownHouseHighlightTile() : base(0x1766)
    {
        Movable = false;
        Hue = 1266;
    }

    public void Start(TimeSpan duration)
    {
        _timerToken.Cancel();
        Timer.StartTimer(duration, Delete, out _timerToken);
    }

    public override void OnAfterDelete()
    {
        _timerToken.Cancel();
        base.OnAfterDelete();
    }

    [AfterDeserialization(false)]
    private void AfterDeserialization() => Delete();
}

// -------------------------------------------------------------------------
// REGION
// -------------------------------------------------------------------------

/// <summary>
/// Region wrapper for inside checks, ban checks, and secured container access.
/// </summary>
public class TownHouseRegion : Region
{
    public TownHouseController Controller { get; }

    public TownHouseRegion(TownHouseController controller)
        : base(
            controller.RegionName,
            controller.Map,
            controller.RegionPriority,
            new Rectangle3D(
                controller.Rect.X,
                controller.Rect.Y,
                controller.MinZ,
                controller.Rect.Width,
                controller.Rect.Height,
                controller.MaxZ - controller.MinZ + 1
            )
        ) => Controller = controller;

    public override TimeSpan GetLogoutDelay(Mobile m) => TimeSpan.Zero;

    public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
    {
        if (Controller == null || m.AccessLevel > AccessLevel.Player || Controller.Owner == null)
        {
            return base.OnMoveInto(m, d, newLocation, oldLocation);
        }

        if (Controller.IsBanned(m))
        {
            return false;
        }

        if (Controller.IsPublic)
        {
            return base.OnMoveInto(m, d, newLocation, oldLocation);
        }

        if (!Controller.IsFriend(m))
        {
            m.SendMessage("This is a private residence.");
            return false;
        }

        return base.OnMoveInto(m, d, newLocation, oldLocation);
    }

    /// <summary>
    /// Blocks non-friends from opening secured containers inside the town house.
    /// </summary>
    public override bool OnDoubleClick(Mobile from, object o)
    {
        if (Controller == null || Controller.Deleted || from.AccessLevel >= AccessLevel.GameMaster)
        {
            return base.OnDoubleClick(from, o);
        }

        if (o is Item item && Controller.Secures.Contains(item) && !Controller.IsFriend(from))
        {
            from.SendMessage("That is secured. You do not have access.");
            return false;
        }

        return base.OnDoubleClick(from, o);
    }
}

// -------------------------------------------------------------------------
// GM BOUND-SETTING TARGETS
// -------------------------------------------------------------------------

public class TownHouseBoundsTarget1 : Target
{
    private readonly TownHouseController _controller;

    public TownHouseBoundsTarget1(TownHouseController c) : base(18, true, TargetFlags.None) => _controller = c;

    protected override void OnTarget(Mobile from, object targeted)
    {
        if (_controller == null || _controller.Deleted || targeted is not IPoint3D p)
        {
            return;
        }

        from.SendMessage("Corner 1 set. Target corner 2.");
        from.Target = new TownHouseBoundsTarget2(_controller, new Point3D(p));
    }
}

public class TownHouseBoundsTarget2 : Target
{
    private readonly TownHouseController _controller;
    private readonly Point3D _p1;

    public TownHouseBoundsTarget2(TownHouseController c, Point3D p1) : base(18, true, TargetFlags.None)
    {
        _controller = c;
        _p1 = p1;
    }

    protected override void OnTarget(Mobile from, object targeted)
    {
        if (_controller == null || _controller.Deleted || targeted is not IPoint3D p)
        {
            return;
        }

        var rect = TownHouseUtil.MakeRect(_p1, new Point3D(p));

        var minZ = _controller.MinZ;
        var maxZ = _controller.MaxZ;

        if (!_controller.HasBounds)
        {
            minZ = from.Z - 20;
            maxZ = from.Z + 60;
        }

        _controller.SetBounds(rect, minZ, maxZ);

        from.SendMessage($"Bounds set: {rect}. ZRange: {minZ}..{maxZ}");
    }
}

// -------------------------------------------------------------------------
// CONTROLLER
// -------------------------------------------------------------------------

[SerializationGenerator(0)]
public partial class TownHouseController : Item
{
    // Bounds
    [SerializableField(0, setter: "private")]
    private Rectangle2D _rect;

    [SerializableField(1, setter: "private")]
    private int _minZ;

    [SerializableField(2, setter: "private")]
    private int _maxZ;

    // State / economy
    [SerializableField(3, setter: "private")]
    [InvalidateProperties]
    private TownHouseState _state;

    [SerializableField(4, setter: "private")]
    private TownHouseWipePolicy _wipePolicy;

    [SerializableField(5, setter: "private")]
    private int _purchasePrice;

    // Ownership & access
    [SerializableField(6, setter: "private")]
    [InvalidateProperties]
    private Mobile _owner;

    [SerializableField(7, setter: "private")]
    [Tidy]
    private List<Mobile> _coOwners;

    [SerializableField(8, setter: "private")]
    [Tidy]
    private List<Mobile> _friends;

    [SerializableField(9, setter: "private")]
    [Tidy]
    private List<Mobile> _bans;

    // Owner sale (player-driven)
    [SerializableField(10, setter: "private")]
    [InvalidateProperties]
    private bool _ownerSaleActive;

    [SerializableField(11, setter: "private")]
    [InvalidateProperties]
    private int _ownerSalePrice;

    // Locks & secures
    [SerializableField(12, setter: "private")]
    private int _lockdownLimit;

    [SerializableField(13, setter: "private")]
    [Tidy]
    private List<Item> _lockdowns;

    [SerializableField(14, setter: "private")]
    [Tidy]
    private List<Item> _secures;

    // Public / Private access mode
    [SerializableField(15, setter: "private")]
    [InvalidateProperties]
    private bool _isPublic;

    // Region instance (not serialized)
    private TownHouseRegion _region;

    // Region priority 60 beats GuardedRegion/TownRegion (both 50), so ban checks
    // and secured container access in TownHouseRegion are consulted first.
    public int RegionPriority => 60;

    public string RegionName => $"TownHouse_{Serial.Value}";

    public bool HasBounds => TownHouseUtil.HasBounds(Rect);

    public Rectangle2D[] Bounds => HasBounds ? [Rect] : [];

    public bool IsForSale => Owner is { Deleted: false } && OwnerSaleActive && OwnerSalePrice > 0;

    public override string DefaultName => "town house sign";

    [Constructible]
    public TownHouseController() : base(0xBD2)
    {
        Hue = 1141;

        _rect = new Rectangle2D(0, 0, 0, 0);
        _minZ = -128;
        _maxZ = 128;

        _purchasePrice = 250000;

        _coOwners = [];
        _friends = [];
        _bans = [];

        _lockdownLimit = 1500;
        _lockdowns = [];
        _secures = [];

        TownHouseSystem.Register(this);
    }

    [AfterDeserialization(false)]
    private void AfterDeserialization()
    {
        TownHouseSystem.Register(this);

        foreach (var item in _secures)
        {
            item.IsSecure = true;
        }

        Timer.StartTimer(TimeSpan.Zero, EnsureRegion);
    }

    public override void OnMapChange()
    {
        base.OnMapChange();
        RemoveRegion();
        EnsureRegion();
    }

    public override void OnAfterDelete()
    {
        RemoveRegion();
        TownHouseSystem.Unregister(this);
        base.OnAfterDelete();
    }

    // ----------------------------------------------------------------------
    // TOOLTIPS & CLICKING
    // ----------------------------------------------------------------------

    public override void OnSingleClick(Mobile from)
    {
        if (from == null)
        {
            return;
        }

        if (Owner == null)
        {
            if (State == TownHouseState.Purchasable)
            {
                LabelTo(from, $"For Sale: {PurchasePrice:N0} gp");
            }
            else
            {
                LabelTo(from, "Unclaimed Property");
            }
        }
        else if (IsForSale)
        {
            LabelTo(from, $"Owned by {Owner.Name} (For Sale: {OwnerSalePrice:N0} gp)");
        }
        else
        {
            LabelTo(from, $"The Home of {Owner.Name}");
        }
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        if (Owner == null)
        {
            list.Add(1060659, $"{"Status"}\t{"Unclaimed"}");
        }
        else if (IsForSale)
        {
            var price = OwnerSalePrice.ToString("N0") + " GP";

            list.Add(1060659, $"{"Status"}\t{"FOR SALE BY OWNER"}");
            list.Add(1070722, $"{price}");
        }
        else if (IsPublic)
        {
            list.Add(1060659, $"{"Status"}\t{"Public Residence"}");
        }
        else
        {
            list.Add(1060659, $"{"Status"}\t{"Private Residence"}");
        }
    }

    public override void OnDoubleClick(Mobile from)
    {
        var inRegion = from.Region is TownHouseRegion r && r.Controller == this;

        if (inRegion || from.InRange(Location, 5) || from.AccessLevel >= AccessLevel.GameMaster)
        {
            TownHouseSignGump.DisplayTo(from, this);
        }
        else
        {
            from.SendLocalizedMessage(500446); // That is too far away.
        }
    }

    // ----------------------------------------------------------------------
    // ACCESS & HELPERS
    // ----------------------------------------------------------------------

    private static bool SameAccount(Mobile a, Mobile b) =>
        TownHouseConfig.UseAccountOwnership
        && a?.Account != null
        && b?.Account != null
        && a.Account == b.Account;

    public bool IsOwner(Mobile m) => m != null && (Owner == m || SameAccount(Owner, m));

    public bool IsCoOwner(Mobile m) => m != null && (CoOwners.Contains(m) || SameAccount(Owner, m));

    public bool IsFriend(Mobile m) => m != null && (IsOwner(m) || IsCoOwner(m) || Friends.Contains(m));

    public bool IsBanned(Mobile m) => m != null && Bans.Contains(m);

    public bool IsInside(Point3D p, Map map)
    {
        if (map == null || map == Map.Internal || map != Map || !HasBounds)
        {
            return false;
        }

        return TownHouseUtil.In3DRange(p, Rect, MinZ, MaxZ);
    }

    public void EnsureRegion()
    {
        if (Deleted || _region != null || Map == null || Map == Map.Internal || !HasBounds)
        {
            return;
        }

        _region = new TownHouseRegion(this);
        _region.Register();
    }

    public void RemoveRegion()
    {
        _region?.Unregister();
        _region = null;
    }

    // GM setters
    public void SetBounds(Rectangle2D rect, int minZ, int maxZ)
    {
        Rect = rect;
        MinZ = minZ;
        MaxZ = maxZ;

        RemoveRegion();
        EnsureRegion();
    }

    public void ApplySettings(int purchasePrice, int lockdownLimit, int minZ, int maxZ)
    {
        PurchasePrice = Math.Max(0, purchasePrice);
        LockdownLimit = Math.Max(0, lockdownLimit);

        MinZ = minZ;
        MaxZ = maxZ;

        RemoveRegion();
        EnsureRegion();
    }

    public void CycleState() =>
        State = State == TownHouseState.Purchasable ? TownHouseState.Transferable : TownHouseState.Purchasable;

    public void ToggleWipePolicy() =>
        WipePolicy = WipePolicy == TownHouseWipePolicy.LeaveAsIs
            ? TownHouseWipePolicy.WipeItemsInBounds
            : TownHouseWipePolicy.LeaveAsIs;

    public void TogglePublic(Mobile from)
    {
        if (!IsOwner(from) && from.AccessLevel < AccessLevel.GameMaster)
        {
            from.SendMessage("Only the owner can change the house access mode.");
            return;
        }

        IsPublic = !IsPublic;

        if (IsPublic)
        {
            from.SendMessage("This house is now open to the public.");
        }
        else
        {
            from.SendMessage("This house is now private.");
        }
    }

    private void ClearAccessLists()
    {
        CoOwners.Clear();
        Friends.Clear();
        Bans.Clear();
        this.MarkDirty();
    }

    // ----------------------------------------------------------------------
    // TRANSACTIONS
    // ----------------------------------------------------------------------

    public bool TryPurchase(Mobile buyer)
    {
        if (buyer == null)
        {
            return false;
        }

        if (!HasBounds)
        {
            buyer.SendMessage("This town house is not configured yet.");
            return false;
        }

        if (Owner != null)
        {
            buyer.SendMessage("This town house is already owned.");
            return false;
        }

        if (State != TownHouseState.Purchasable)
        {
            buyer.SendMessage("This town house is not for purchase.");
            return false;
        }

        if (TownHouseSystem.GetOwnedCount(buyer) >= TownHouseConfig.MaxOwnedHouses)
        {
            buyer.SendMessage($"You already own the maximum number of town houses ({TownHouseConfig.MaxOwnedHouses}).");
            return false;
        }

        if (!Banker.Withdraw(buyer, PurchasePrice))
        {
            buyer.SendMessage("You do not have enough gold in the bank.");
            return false;
        }

        AssignOwner(buyer);
        buyer.SendMessage("You have purchased the town house.");
        TownHouseSystem.OnPurchased?.Invoke(this, buyer);

        return true;
    }

    private void AssignOwner(Mobile m)
    {
        Owner = m;

        State = TownHouseState.Transferable;
        OwnerSaleActive = false;
        OwnerSalePrice = 0;

        ClearAccessLists();

        if (WipePolicy == TownHouseWipePolicy.WipeItemsInBounds)
        {
            WipeItemsInside();
        }

        EnsureRegion();
    }

    public void EvictOwner()
    {
        var oldOwner = Owner;

        Owner = null;
        State = TownHouseState.Purchasable;
        OwnerSaleActive = false;
        OwnerSalePrice = 0;

        ClearAccessLists();

        // Clear ownership flags but leave items immovable in place for the next owner
        foreach (var item in Lockdowns)
        {
            if (item is { Deleted: false })
            {
                item.IsLockedDown = false;
            }
        }

        Lockdowns.Clear();

        foreach (var item in Secures)
        {
            if (item is { Deleted: false })
            {
                item.IsSecure = false;
            }
        }

        Secures.Clear();
        this.MarkDirty();

        if (WipePolicy == TownHouseWipePolicy.WipeItemsInBounds)
        {
            WipeItemsInside();
        }

        oldOwner?.SendMessage("Your town house has been reclaimed.");

        TownHouseSystem.OnEvicted?.Invoke(this);
    }

    private void WipeItemsInside()
    {
        if (Map == null || Map == Map.Internal || !HasBounds)
        {
            return;
        }

        using var toDelete = PooledRefList<Item>.Create();

        foreach (var item in Map.GetItemsInBounds<Item>(Rect))
        {
            if (item.Deleted || item == this || item is TownHouseHighlightTile or TownHouseController)
            {
                continue;
            }

            if (TownHouseUtil.In3DRange(item.Location, Rect, MinZ, MaxZ))
            {
                toDelete.Add(item);
            }
        }

        foreach (var item in toDelete)
        {
            item.Delete();
        }
    }

    // ----------------------------------------------------------------------
    // ITEMS / SECURES
    // ----------------------------------------------------------------------

    public bool CanLockdown(Mobile m) => IsFriend(m) && Lockdowns.Count < LockdownLimit;

    public bool TryLockdown(Mobile m, Item item)
    {
        if (m == null || item == null || item.Deleted)
        {
            return false;
        }

        if (!IsInside(item.Location, item.Map))
        {
            m.SendMessage("That is not inside this town house.");
            return false;
        }

        if (!CanLockdown(m))
        {
            m.SendMessage("You cannot lock down more items here.");
            return false;
        }

        if (Lockdowns.Contains(item))
        {
            m.SendMessage("That is already locked down.");
            return false;
        }

        item.Movable = false;
        Lockdowns.Add(item);
        this.MarkDirty();

        m.SendMessage("Item locked down.");
        return true;
    }

    public bool TrySecure(Mobile m, Item item)
    {
        if (m == null || item == null || item.Deleted)
        {
            return false;
        }

        if (!IsInside(item.Location, item.Map))
        {
            m.SendMessage("That is not inside this town house.");
            return false;
        }

        if (!IsFriend(m))
        {
            m.SendMessage("You do not have access to do that.");
            return false;
        }

        if (Secures.Contains(item))
        {
            m.SendMessage("That is already secured.");
            return false;
        }

        item.Movable = false;
        item.IsSecure = true;
        Secures.Add(item);
        this.MarkDirty();

        m.SendMessage("Item secured. Only friends and above may open it.");
        return true;
    }

    public bool TryRelease(Mobile m, Item item)
    {
        if (m == null || item == null || item.Deleted)
        {
            return false;
        }

        if (!IsCoOwner(m) && !IsOwner(m) && m.AccessLevel < AccessLevel.GameMaster)
        {
            m.SendMessage("You must be at least a Co-Owner to release items.");
            return false;
        }

        if (Lockdowns.Remove(item))
        {
            item.Movable = true;
            item.IsLockedDown = false;
            this.MarkDirty();

            m.SendMessage("Item released.");
            return true;
        }

        if (Secures.Remove(item))
        {
            item.Movable = true;
            item.IsSecure = false;
            this.MarkDirty();

            m.SendMessage("Secure released.");
            return true;
        }

        m.SendMessage("That is not locked down or secured.");
        return false;
    }

    // ----------------------------------------------------------------------
    // ACCESS LISTS
    // ----------------------------------------------------------------------

    private bool CanManageLists(Mobile from) =>
        IsOwner(from) || IsCoOwner(from) || from.AccessLevel >= AccessLevel.GameMaster;

    public bool TryAddFriend(Mobile from, Mobile target)
    {
        if (!CanManageLists(from))
        {
            return false;
        }

        if (target == null || target.Deleted || !target.Player)
        {
            return false;
        }

        if (Friends.Contains(target) || IsOwner(target) || IsCoOwner(target))
        {
            return false;
        }

        Friends.Add(target);
        Bans.Remove(target);
        this.MarkDirty();
        return true;
    }

    public bool TryAddCoOwner(Mobile from, Mobile target)
    {
        if (!IsOwner(from) && from.AccessLevel < AccessLevel.GameMaster)
        {
            from.SendMessage("Only the owner can add Co-Owners.");
            return false;
        }

        if (target == null || target.Deleted || !target.Player)
        {
            return false;
        }

        if (CoOwners.Contains(target) || IsOwner(target))
        {
            return false;
        }

        CoOwners.Add(target);
        Friends.Remove(target);
        Bans.Remove(target);
        this.MarkDirty();
        return true;
    }

    public bool TryBan(Mobile from, Mobile target)
    {
        if (!CanManageLists(from))
        {
            return false;
        }

        if (target == null || target.Deleted || !target.Player || IsOwner(target))
        {
            return false;
        }

        if (!Bans.Contains(target))
        {
            Bans.Add(target);
        }

        Friends.Remove(target);
        CoOwners.Remove(target);
        this.MarkDirty();
        return true;
    }

    public bool TryRemoveFriend(Mobile from, Mobile target)
    {
        if (!CanManageLists(from))
        {
            return false;
        }

        var removed = Friends.Remove(target);
        if (removed)
        {
            this.MarkDirty();
        }

        return removed;
    }

    public bool TryRemoveCoOwner(Mobile from, Mobile target)
    {
        if (!IsOwner(from) && from.AccessLevel < AccessLevel.GameMaster)
        {
            from.SendMessage("Only the owner can remove Co-Owners.");
            return false;
        }

        var removed = CoOwners.Remove(target);
        if (removed)
        {
            this.MarkDirty();
        }

        return removed;
    }

    public bool TryUnban(Mobile from, Mobile target)
    {
        if (!CanManageLists(from))
        {
            return false;
        }

        var removed = Bans.Remove(target);
        if (removed)
        {
            this.MarkDirty();
        }

        return removed;
    }

    public bool TryTransfer(Mobile from, Mobile to)
    {
        if (from == null || to == null)
        {
            return false;
        }

        if (State != TownHouseState.Transferable)
        {
            from.SendMessage("This town house is not transferable.");
            return false;
        }

        if (!IsOwner(from))
        {
            from.SendMessage("You are not the owner.");
            return false;
        }

        if (to.Deleted || !to.Player)
        {
            from.SendMessage("That is not a valid player.");
            return false;
        }

        if (TownHouseSystem.GetOwnedCount(to) >= TownHouseConfig.MaxOwnedHouses)
        {
            from.SendMessage(
                $"That player already owns the maximum number of town houses ({TownHouseConfig.MaxOwnedHouses})."
            );
            return false;
        }

        Owner = to;
        ClearAccessLists();

        from.SendMessage("You have transferred the town house.");
        to.SendMessage("You are now the owner of this town house.");
        return true;
    }

    // ----------------------------------------------------------------------
    // PLAYER SALES
    // ----------------------------------------------------------------------

    public bool TryBeginSale(Mobile from, int price)
    {
        if (from == null || Owner is not { Deleted: false })
        {
            return false;
        }

        if (!IsOwner(from))
        {
            from.SendMessage("You are not the owner.");
            return false;
        }

        if (price <= 0)
        {
            from.SendMessage("Sale price must be greater than zero.");
            return false;
        }

        OwnerSaleActive = true;
        OwnerSalePrice = price;

        from.SendMessage("This town house is now for sale.");
        return true;
    }

    public bool TryCancelSale(Mobile from)
    {
        if (from == null || !IsOwner(from) && from.AccessLevel < AccessLevel.GameMaster)
        {
            return false;
        }

        OwnerSaleActive = false;
        OwnerSalePrice = 0;

        from.SendMessage("Sale cancelled.");
        return true;
    }

    public bool TryAbandon(Mobile from)
    {
        if (from == null || !IsOwner(from) && from.AccessLevel < AccessLevel.GameMaster)
        {
            return false;
        }

        EvictOwner();
        from.SendMessage("You have abandoned the town house.");
        return true;
    }

    public bool TryPurchaseFromOwner(Mobile buyer)
    {
        if (buyer == null)
        {
            return false;
        }

        if (!IsForSale)
        {
            buyer.SendMessage("This town house is not for sale.");
            return false;
        }

        if (Owner == null || Owner.Deleted)
        {
            buyer.SendMessage("This town house has no owner.");
            return false;
        }

        if (IsOwner(buyer))
        {
            buyer.SendMessage("You already own this town house.");
            return false;
        }

        if (TownHouseSystem.GetOwnedCount(buyer) >= TownHouseConfig.MaxOwnedHouses)
        {
            buyer.SendMessage($"You already own the maximum number of town houses ({TownHouseConfig.MaxOwnedHouses}).");
            return false;
        }

        var price = OwnerSalePrice;

        if (!Banker.Withdraw(buyer, price))
        {
            buyer.SendMessage("You do not have enough gold in your bank.");
            return false;
        }

        var oldOwner = Owner;

        if (!GiveSaleProceeds(oldOwner, price))
        {
            GiveSaleProceeds(buyer, price);
            buyer.SendMessage("Sale failed: the seller could not receive payment.");
            return false;
        }

        Owner = buyer;
        ClearAccessLists();
        OwnerSaleActive = false;
        OwnerSalePrice = 0;

        buyer.SendMessage($"You have purchased the town house for {price:N0} gp.");

        if (oldOwner is { Deleted: false })
        {
            oldOwner.SendMessage($"Your town house has been sold for {price:N0} gp.");
        }

        return true;
    }

    private static bool GiveSaleProceeds(Mobile m, int amount)
    {
        if (m == null || m.Deleted || amount <= 0)
        {
            return false;
        }

        if (Banker.Deposit(m, amount))
        {
            return true;
        }

        var check = new BankCheck(amount);

        var bank = m.BankBox;
        if (bank is { Deleted: false } && bank.TryDropItem(m, check, false))
        {
            return true;
        }

        var pack = m.Backpack;
        if (pack is { Deleted: false } && pack.TryDropItem(m, check, false))
        {
            return true;
        }

        check.MoveToWorld(m.Location, m.Map);
        return true;
    }

    // ----------------------------------------------------------------------
    // MISC
    // ----------------------------------------------------------------------

    public void ShowHighlight(Mobile gm, TimeSpan duration)
    {
        if (gm == null || Map == null || Map == Map.Internal || !HasBounds)
        {
            return;
        }

        var step = Math.Max(1, TownHouseConfig.HighlightStep);

        for (var x = Rect.X; x < Rect.X + Rect.Width; x += step)
        {
            for (var y = Rect.Y; y < Rect.Y + Rect.Height; y += step)
            {
                var z = Map.GetAverageZ(x, y);

                if (z < MinZ || z > MaxZ)
                {
                    continue;
                }

                var tile = new TownHouseHighlightTile();
                tile.MoveToWorld(new Point3D(x, y, z), Map);
                tile.Start(duration);
            }
        }

        gm.SendMessage("Highlight placed.");
    }
}
