/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using Server;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SpellbookStrap : BaseContainer
{
    [Constructible]
    public SpellbookStrap() : base(0xA71F)
    {
        Name = "Spellbook Strap";
        Weight = 1.0;
        LootType = LootType.Blessed;
    }

    public override void AddNameProperties(IPropertyList list)
    {
        base.AddNameProperties(list);
        list.Add($"{"Weight Reduction: 100%"}");
    }

    // --- FIX 1: Allow Opening/Casting without opening the strap ---
    // This tells the server "The contents of this bag are viewable/usable
    // provided the bag itself is accessible to the player."
    public override bool CheckContentDisplay(Mobile from)
    {
        if (IsChildOf(from.Backpack))
        {
            return true;
        }

        return base.CheckContentDisplay(from);
    }

    // --- FIX 2: Ensure the strap itself is touchable ---
    public override bool IsAccessibleTo(Mobile m)
    {
        if (IsChildOf(m.Backpack))
        {
            return true;
        }

        return base.IsAccessibleTo(m);
    }

    // --- FIX 3: Allow the Item Usage (Double Click) to pass through ---
    public override bool CheckItemUse(Mobile from, Item item)
    {
        // If the item is in this strap, and the strap is in the backpack, allow it.
        if (item.IsChildOf(this) && IsChildOf(from.Backpack))
        {
            return true;
        }

        return base.CheckItemUse(from, item);
    }

    public override bool OnDragDrop(Mobile from, Item dropped)
    {
        if (dropped is Spellbook)
        {
            return base.OnDragDrop(from, dropped);
        }

        from.SendMessage("This strap is only designed to hold spellbooks.");
        return false;
    }

    public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
    {
        if (item is Spellbook)
        {
            return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
        }

        if (message)
        {
            m.SendMessage("This strap can only hold spellbooks.");
        }

        return false;
    }

    // FORCE CALCULATED WEIGHT TO 0
    public override int GetTotal(TotalType type)
    {
        if (type == TotalType.Weight)
        {
            return 0;
        }

        return base.GetTotal(type);
    }

    public override void UpdateTotal(Item sender, TotalType type, int delta)
    {
        if (type == TotalType.Weight)
        {
            delta = 0;
        }

        base.UpdateTotal(sender, type, delta);
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    // Delete this method if existing saves do not need to load.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
