/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using System.Collections.Generic;
using Server.Gumps;

namespace Server.Items;

public abstract class VirtualStorageChest : Item
{
    // Item type -> quantity held
    public Dictionary<Type, int> Content { get; } = new();

    public abstract Type[] AllowedTypes { get; }
    public abstract string ChestTitle { get; }

    protected VirtualStorageChest(int itemID) : base(itemID)
    {
        Weight = 10.0;
    }

    public VirtualStorageChest(Serial serial) : base(serial)
    {
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!from.InRange(GetWorldLocation(), 2))
        {
            from.SendLocalizedMessage(500446); // That is too far away.
            return;
        }

        if (!IsLockedDown && !IsSecure)
        {
            from.SendMessage("This storage chest must be secured in a house to function.");
            return;
        }

        VirtualStorageGump.DisplayTo(from, this);
    }

    public override bool OnDragDrop(Mobile from, Item dropped)
    {
        if (!IsLockedDown && !IsSecure)
        {
            from.SendMessage("This storage chest must be secured in a house to function.");
            return false;
        }

        if (TryAdd(dropped) > 0)
        {
            from.PlaySound(0x42);
            VirtualStorageGump.DisplayTo(from, this);
            return true;
        }

        from.SendMessage("That item does not belong in this stockpile.");
        return false;
    }

    // Returns the amount added, or 0 if the item is not allowed.
    public int TryAdd(Item item)
    {
        var t = item.GetType();
        var isAllowed = false;

        foreach (var allowed in AllowedTypes)
        {
            if (t == allowed || t.IsSubclassOf(allowed))
            {
                isAllowed = true;
                break;
            }
        }

        if (!isAllowed)
        {
            return 0;
        }

        var amountToAdd = item.Amount;
        Content.TryGetValue(t, out var current);
        Content[t] = current + amountToAdd;

        item.Delete();
        this.MarkDirty();
        return amountToAdd;
    }

    // Removes up to `amount` from the stockpile and returns how much was actually removed.
    public int Withdraw(Type type, int amount)
    {
        if (!Content.TryGetValue(type, out var held) || amount <= 0)
        {
            return 0;
        }

        var removed = Math.Min(held, amount);

        if (held - removed <= 0)
        {
            Content.Remove(type);
        }
        else
        {
            Content[type] = held - removed;
        }

        this.MarkDirty();
        return removed;
    }

    // Serialized by hand: the source generator has no support for a Type-keyed dictionary.
    // Serialize runs on background save threads, so it must stay a pure read.
    public override void Serialize(IGenericWriter writer)
    {
        base.Serialize(writer);

        writer.WriteEncodedInt(0); // version

        writer.WriteEncodedInt(Content.Count);
        foreach (var (type, amount) in Content)
        {
            writer.Write(type.FullName);
            writer.Write(amount);
        }
    }

    public override void Deserialize(IGenericReader reader)
    {
        base.Deserialize(reader);

        reader.ReadEncodedInt(); // version

        var count = reader.ReadEncodedInt();
        for (var i = 0; i < count; i++)
        {
            var typeName = reader.ReadString();
            var amount = reader.ReadInt();

            var type = AssemblyHandler.FindTypeByFullName(typeName);
            if (type != null && amount > 0)
            {
                Content[type] = amount;
            }
        }
    }
}
