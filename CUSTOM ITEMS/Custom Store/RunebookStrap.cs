/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class RunebookStrap : BaseContainer
{
    public override string DefaultName => "Runebook Strap";
    public override double DefaultWeight => 1.0;

    [Constructible]
    public RunebookStrap() : base(0xA721)
    {
        LootType = LootType.Blessed;
    }

    public override void AddNameProperties(IPropertyList list)
    {
        base.AddNameProperties(list);

        list.Add($"{"Weight Reduction: 100%"}");
    }

    public override bool OnDragDrop(Mobile from, Item dropped)
    {
        // RunicAtlas derives from Runebook, so one check covers both
        if (dropped is Runebook)
        {
            return base.OnDragDrop(from, dropped);
        }

        from.SendMessage("This strap can only hold runebooks or runic atlases.");
        return false;
    }

    public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
    {
        if (item is Runebook)
        {
            return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
        }

        if (message)
        {
            m.SendMessage("This strap can only hold runebooks or runic atlases.");
        }

        return false;
    }

    // The strap and its contents weigh nothing
    public override int GetTotal(TotalType type) => type == TotalType.Weight ? 0 : base.GetTotal(type);

    public override void UpdateTotal(Item sender, TotalType type, int delta)
    {
        base.UpdateTotal(sender, type, type == TotalType.Weight ? 0 : delta);
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
