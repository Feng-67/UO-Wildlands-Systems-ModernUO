using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Collections;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items;

// TODO(migrate): ModernUO has no SkillCat enum. This one is rebuilt from how the ServUO code uses it
// (PowerScrollBook and GetCategoryLocalization). Delete it if your repo already defines SkillCat elsewhere.
public enum SkillCat
{
    None,
    Miscellaneous,
    Combat,
    TradeSkills,
    Magic,
    Wilderness,
    Thievery,
    Bard
}

[Flippable(0x9A95, 0x9AA7)]
[SerializationGenerator(4, false)] // ServUO wrote version 3; generator version is old + 1
public abstract partial class BaseSpecialScrollBook : Container
{
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private SecureLevel _level;

    public override bool DisplaysContent => false;
    public override double DefaultWeight => 1.0;

    public abstract Type ScrollType { get; }

    public abstract int BadDropMessage { get; }
    public abstract int DropMessage { get; }
    public abstract int RemoveMessage { get; }
    public abstract int GumpTitle { get; }

    public virtual Dictionary<SkillCat, List<SkillName>> SkillInfo => null;
    public virtual Dictionary<int, double> ValueInfo => null;

    protected BaseSpecialScrollBook(int id) : base(id)
    {
        LootType = LootType.Blessed;
    }

    public override int GetTotal(TotalType type) => 0;

    public override void OnDoubleClick(Mobile m)
    {
        if (m is PlayerMobile pm && m.InRange(GetWorldLocation(), 2))
        {
            DisplayBookGump(pm);
        }
        else if (m.AccessLevel > AccessLevel.Player)
        {
            base.OnDoubleClick(m);
        }
    }

    // TODO(migrate): ServUO opened its stock SpecialScrollBookGump here (BaseGump.SendGump). That gump is not in
    // ModernUO and was not part of the uploaded files, so subclasses supply their own. PowerScrollBook already
    // overrides OnDoubleClick and opens PowerScrollBookGump, so it never reaches this method.
    protected virtual void DisplayBookGump(PlayerMobile from)
    {
    }

    public override void GetContextMenuEntries(Mobile from, ref PooledRefList<ContextMenuEntry> list)
    {
        base.GetContextMenuEntries(from, ref list);

        SetSecureLevelEntry.AddTo(from, this, ref list);
    }

    public override bool OnDragDrop(Mobile m, Item dropped)
    {
        if (!m.InRange(GetWorldLocation(), 2))
        {
            return false;
        }

        var house = BaseHouse.FindHouseAt(this);

        if (dropped.GetType() != ScrollType)
        {
            m.SendLocalizedMessage(BadDropMessage);
        }
        else if (house == null || !IsLockedDown)
        {
            m.SendLocalizedMessage(1151765); // You must lock this book down in a house to add scrolls to it.
        }
        else if (!house.CheckAccessibility(this, m))
        {
            m.SendLocalizedMessage(1155693); // This item is impermissible and can not be added to the book.
        }
        else
        {
            DropItem(dropped);

            m.SendLocalizedMessage(DropMessage);

            dropped.Movable = false;

            // TODO(migrate): ServUO closed SpecialScrollBookGump here so it would refresh; see DisplayBookGump.

            return true;
        }

        return false;
    }

    public virtual void Construct(Mobile m, SkillName sk, double value)
    {
        SpecialScroll scroll = null;

        foreach (var item in Items)
        {
            if (item is SpecialScroll s && s.Skill == sk && s.Value == value)
            {
                scroll = s;
                break;
            }
        }

        if (scroll == null)
        {
            return;
        }

        if (m.Backpack == null || !m.Backpack.TryDropItem(m, scroll, false))
        {
            m.SendLocalizedMessage(502868); // Your backpack is too full.
            return;
        }

        // ModernUO keeps house lockdowns in a List<Item> (ServUO used a Dictionary), so Remove is enough.
        BaseHouse.FindHouseAt(this)?.LockDowns.Remove(scroll);

        if (!scroll.Movable)
        {
            scroll.Movable = true;
        }

        if (scroll.IsLockedDown)
        {
            scroll.IsLockedDown = false;
        }

        m.SendLocalizedMessage(RemoveMessage);
    }

    // Legacy hook: loads saves written by the ServUO version (version 3 wrote Level then Capacity;
    // capacity has been removed, so the old value is read and discarded).
    private void Deserialize(IGenericReader reader, int version)
    {
        if (version > 2)
        {
            _level = (SecureLevel)reader.ReadInt();
            reader.ReadInt(); // old Capacity, no longer used
        }
    }

    // Scrolls stored in the book must never be movable. ServUO did this from a delayed timer because the
    // contents were not linked yet during Deserialize; AfterDeserialization runs once everything is loaded.
    [AfterDeserialization]
    private void AfterDeserialization()
    {
        foreach (var item in Items)
        {
            if (item.Movable)
            {
                item.Movable = false;
            }
        }
    }

    public static int GetCategoryLocalization(SkillCat category) =>
        category switch
        {
            SkillCat.Miscellaneous => 1078596,
            SkillCat.Combat        => 1078592,
            SkillCat.TradeSkills   => 1078591,
            SkillCat.Magic         => 1078593,
            SkillCat.Wilderness    => 1078595,
            SkillCat.Thievery      => 1078594,
            SkillCat.Bard          => 1078590,
            _                      => 0
        };
}
