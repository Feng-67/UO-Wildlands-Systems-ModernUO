/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Engines.BulkOrders;

namespace Server.Items;

[SerializationGenerator(2, false)] // ServUO wrote version 1; generator version is old + 1
public partial class LargeBODBox : Container
{
    // Large BODs that have already been split. The ServUO version saved their serials as plain ints;
    // saving the items themselves lets the list drop deeds that no longer exist.
    [SerializableField(0, setter: "private")]
    private List<Item> _processedDeeds;

    // Serials from old saves, resolved to items once the world has finished loading
    private List<uint> _legacyDeedSerials;

    public override string DefaultName => "Large BOD Deed Box";
    public override double DefaultWeight => 2.0;

    public override int DefaultGumpID => 0x43;
    public override int DefaultDropSound => 0x42;

    public override Rectangle2D Bounds => new(16, 51, 168, 73);

    [Constructible]
    public LargeBODBox() : base(0x9AA)
    {
        _processedDeeds = [];
    }

    public override void OnDoubleClick(Mobile from)
    {
        // Always open the container first
        base.OnDoubleClick(from);

        if (!IsChildOf(from.Backpack) && !IsChildOf(from.BankBox) && !IsSecure)
        {
            from.SendMessage("This must be in your backpack, bank box, or secured in your house to use.");
            return;
        }

        LargeBOD largeBOD = null;

        foreach (var item in Items)
        {
            if (item is LargeBOD bod)
            {
                largeBOD = bod;
                break;
            }
        }

        // Nothing to split; the box is already open
        if (largeBOD == null)
        {
            return;
        }

        // Anti-exploit: each Large BOD can only be split once
        if (_processedDeeds.Contains(largeBOD))
        {
            from.SendMessage("This specific Large BOD has already been split.");
            return;
        }

        if (GenerateSmallBODs(from, largeBOD))
        {
            _processedDeeds.Add(largeBOD);
            this.MarkDirty();

            from.SendMessage("The Small BOD components have been generated.");
        }
    }

    private bool GenerateSmallBODs(Mobile from, LargeBOD largeBOD)
    {
        // TODO(migrate): stock ModernUO only has Smith and Tailor bulk orders. The ServUO version also handled
        // Alchemy, Inscription, Tinkering, Cooking, Fletching and Carpentry; add them here once those BODs exist.
        if (largeBOD is not (LargeSmithBOD or LargeTailorBOD))
        {
            from.SendMessage("Unsupported BOD type.");
            return false;
        }

        foreach (var entry in largeBOD.Entries)
        {
            SmallBOD small = largeBOD is LargeSmithBOD ? new SmallSmithBOD() : new SmallTailorBOD();

            small.AmountMax = largeBOD.AmountMax;
            small.RequireExceptional = largeBOD.RequireExceptional;
            small.Material = largeBOD.Material;

            if (entry.Details != null)
            {
                small.Type = entry.Details.Type;
                small.Number = entry.Details.Number;
                small.Graphic = entry.Details.Graphic; // the original left the random graphic from the constructor
            }

            DropItem(small);
        }

        return true;
    }

    // Legacy hook: loads saves written by the ServUO version
    // (version 0 held no payload, version 1 wrote the processed-deed serials).
    private void Deserialize(IGenericReader reader, int version)
    {
        _processedDeeds = [];

        if (version >= 1)
        {
            var count = reader.ReadInt();
            _legacyDeedSerials = new List<uint>(count);

            for (var i = 0; i < count; i++)
            {
                _legacyDeedSerials.Add(reader.ReadUInt());
            }
        }
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        if (_legacyDeedSerials == null)
        {
            return;
        }

        foreach (var serial in _legacyDeedSerials)
        {
            var deed = World.FindItem((Serial)serial);

            if (deed != null)
            {
                _processedDeeds.Add(deed);
            }
        }

        _legacyDeedSerials = null;
        this.MarkDirty();
    }
}
