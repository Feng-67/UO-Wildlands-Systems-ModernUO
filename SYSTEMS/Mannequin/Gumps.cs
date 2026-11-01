// UO Wildlands Custom Script
// Adapted for ModernUO from ServUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)

using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps;

public sealed class MannequinCompareGump : DynamicGump
{
    private readonly Item _item;
    private readonly Item _sameItem;

    public override bool Singleton => true;

    private MannequinCompareGump(Mannequin mann, Item item, Item sameItem) : base(100, 100)
    {
        _item = item;
        _sameItem = sameItem;
    }

    // The layout renders nothing when the mannequin has no item in the matching slot, and the
    // client cannot close a gump that renders nothing, so construction is guarded by DisplayTo.
    public static void DisplayTo(Mobile from, Mannequin mann, Item item)
    {
        if (from?.NetState == null || from.Deleted || mann == null || mann.Deleted || item == null)
        {
            return;
        }

        Item sameItem = null;

        for (var i = 0; i < mann.Items.Count; i++)
        {
            var equipped = mann.Items[i];

            if (mann.LayerValidation(equipped, item))
            {
                sameItem = equipped;
                break;
            }
        }

        if (sameItem == null)
        {
            return;
        }

        from.SendGump(new MannequinCompareGump(mann, item, sameItem));
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage(0);

        builder.AddBackground(0, 0, 560, 720, 0x6DB);
        builder.AddImageTiled(270, 5, 6, 710, 0x6DE);
        builder.AddHtmlLocalized(5, 5, 270, 18, 1114513, "#1159291", 0x42FF, false, false);
        builder.AddAlphaRegion(5, 5, 550, 710);

        builder.AddHtmlLocalized(0, 23, 270, 18, 1114513, $"#{_sameItem.LabelNumber}", 0x7FFF, false, false);
        builder.AddImageTiled(4, 80, 550, 5, 0x6DC);

        var equipmentProperties = Mannequin.FindItemProperty(_sameItem);
        var spriteIndex = 0;

        for (var i = 0; i < equipmentProperties.Count; i++)
        {
            var property = equipmentProperties[i];

            if (property.LabelNumber == 1159280) // Medable Armor - not appear
            {
                continue;
            }

            if (property.IsSpriteGraph)
            {
                builder.AddSpriteImage(5 + 35 * spriteIndex, 41, 0x9D3B, property.SpriteW, property.SpriteH, 30, 30);
                builder.AddTooltip(property.LabelNumber);

                spriteIndex++;
            }
            else
            {
                builder.AddHtmlLocalized(45, 94 + 18 * i, 140, 18, property.LabelNumber, property.Hue, false, false);
                builder.AddTooltip(property.Description);

                if (!property.IsBoolen)
                {
                    builder.AddHtml(190, 94 + 18 * i, 100, 18, Color(property.Value, property.Cap));
                }
            }
        }

        builder.AddHtmlLocalized(270, 23, 270, 18, 1114513, $"#{_item.LabelNumber}", 0x7FFF, false, false);

        var selectedProperties = Mannequin.FindItemProperty(_item);
        spriteIndex = 0;

        for (var i = 0; i < selectedProperties.Count; i++)
        {
            var property = selectedProperties[i];

            if (property.LabelNumber == 1159280) // Medable Armor - not appear
            {
                continue;
            }

            if (property.IsSpriteGraph)
            {
                builder.AddSpriteImage(275 + 35 * spriteIndex, 41, 0x9D3B, property.SpriteW, property.SpriteH, 30, 30);
                builder.AddTooltip(property.LabelNumber);

                spriteIndex++;
            }
            else
            {
                builder.AddHtmlLocalized(315, 94 + 18 * i, 140, 18, property.LabelNumber, property.Hue, false, false);
                builder.AddTooltip(property.Description);

                if (!property.IsBoolen)
                {
                    var equipmentValue = 0.0;

                    for (var j = 0; j < equipmentProperties.Count; j++)
                    {
                        if (equipmentProperties[j].LabelNumber == property.LabelNumber)
                        {
                            equipmentValue = equipmentProperties[j].Value;
                            break;
                        }
                    }

                    builder.AddHtml(460, 94 + 18 * i, 100, 18, Color(property.Value, property.Cap, equipmentValue));
                }
            }
        }
    }

    private static string Color(double v1, double v2, double ev = 0)
    {
        var diff = v1 - ev;

        if (v2 == 0)
        {
            if (v1 == diff || diff == 0)
            {
                return $"<BASEFONT COLOR=#FFFFFF>{v1}</BASEFONT>";
            }

            if (v1 > ev)
            {
                return $"<BASEFONT COLOR=#008000>{v1} (+{diff})</BASEFONT>";
            }

            return $"<BASEFONT COLOR=#800000>{v1} ({diff})</BASEFONT>";
        }

        if (v1 == diff || diff == 0)
        {
            return $"<BASEFONT COLOR=#FFFFFF>{v1}/{v2}</BASEFONT>";
        }

        if (v1 > ev)
        {
            return $"<BASEFONT COLOR=#008000>{v1}/{v2} (+{diff})</BASEFONT>";
        }

        return $"<BASEFONT COLOR=#800000>{v1}/{v2} ({diff})</BASEFONT>";
    }
}

public sealed class MannequinStatsGump : DynamicGump
{
    private static readonly LabelDefinition[] _page1 =
    [
        new LabelDefinition(1049593, Catalog.Attributes, 5),  // Attributes
        new LabelDefinition(1061645, Catalog.Resistances, 5), // Resistances
        new LabelDefinition(1077417, Catalog.Combat1, 8),     // Combat
    ];

    private static readonly LabelDefinition[] _page2 =
    [
        new LabelDefinition(1077417, Catalog.Combat2, 3),     // Combat
        new LabelDefinition(1076209, Catalog.Casting),        // Casting
        new LabelDefinition(1044050, Catalog.Misc),           // Miscellaneous
        new LabelDefinition(1114251, Catalog.HitEffects),     // Hit Effects
        new LabelDefinition(1155065, Catalog.SkillBonusGear), // Skill Bonus Gear
    ];

    private readonly Mannequin _mannequin;
    private readonly Item _item;
    private readonly Item _sameItem;
    private readonly int _page;

    public override bool Singleton => true;

    public MannequinStatsGump(Mannequin mann) : this(mann, null)
    {
    }

    public MannequinStatsGump(Mannequin mann, Item item) : this(mann, item, 0)
    {
    }

    public MannequinStatsGump(Mannequin mann, Item item, int page) : base(100, 100)
    {
        _mannequin = mann;
        _item = item;
        _page = page;

        if (item == null)
        {
            return;
        }

        for (var i = 0; i < mann.Items.Count; i++)
        {
            var equipped = mann.Items[i];

            if (mann.LayerValidation(equipped, item))
            {
                _sameItem = equipped;
                break;
            }
        }
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage(0);

        builder.AddBackground(0, 0, 604, 820, 0x6DB);
        builder.AddAlphaRegion(5, 5, 594, 810);
        builder.AddHtmlLocalized(10, 10, 584, 18, 1114513, "#1159279", 0x43F7, false, false);

        var properties = Mannequin.GetProperty(_mannequin.Items);
        List<ItemPropDefinition> compareItems = null;

        if (_item != null && _sameItem != null)
        {
            compareItems = [new ItemPropDefinition(_item, _sameItem)];

            var comparedItems = new List<Item>(_mannequin.Items.Count);

            for (var i = 0; i < _mannequin.Items.Count; i++)
            {
                var equipped = _mannequin.Items[i];

                if (equipped != _sameItem)
                {
                    comparedItems.Add(equipped);
                }
            }

            comparedItems.Add(_item);

            properties = Mannequin.GetProperty(comparedItems);
        }

        builder.AddHtmlLocalized(479, 10, 75, 18, 1114514, "#1158215", 0x42FF, false, false);

        if (_page > 0)
        {
            builder.AddButton(554, 10, 0x15E3, 0x15E7, 1000, GumpButtonType.Reply, 0); // Next Page Button
        }
        else
        {
            builder.AddButton(554, 10, 0x15E1, 0x15E5, 1001, GumpButtonType.Reply, 0); // Previous Page Button
        }

        var y = 0;
        var pages = _page > 0 ? _page2 : _page1;

        foreach (var pageDefinition in pages)
        {
            y += 28;

            var catalogProperties = GetCatalogProperties(properties, pageDefinition.Catalog);

            if (catalogProperties.Count == 0)
            {
                continue;
            }

            builder.AddHtmlLocalized(10, y, 150, 18, pageDefinition.TitleLabel, 0x6A05, false, false);
            builder.AddImageTiled(10, y + 18, 584, 5, 0x6DC);

            y += 32;

            var columnCount = pageDefinition.ColumnLeftCount == 0
                ? (int)Math.Ceiling(catalogProperties.Count / (decimal)2)
                : pageDefinition.ColumnLeftCount;

            for (var i = 0; i < catalogProperties.Count; i++)
            {
                var property = catalogProperties[i];
                var label = property.LabelNumber;

                if (i < columnCount)
                {
                    builder.AddHtmlLocalized(45, y, 200, 18, label, 0x560A, false, false);
                    builder.AddSpriteImage(10, y - 5, 0x9D3B, property.SpriteW, property.SpriteH, 30, 30);
                    builder.AddTooltip(property.Description);

                    if (property.IsBoolen)
                    {
                        if (property.BoolenValue)
                        {
                            if (property.Value > 0)
                            {
                                builder.AddHtmlLocalized(195, y, 100, 18, 1046362, 0x42FF, false, false); // Yes
                            }
                            else
                            {
                                builder.AddHtmlLocalized(195, y, 100, 18, 1046363, 0x42FF, false, false); // No
                            }
                        }
                    }
                    else
                    {
                        var value = Color(property.Value, GetItemValue(compareItems, label), property.Cap);
                        builder.AddHtml(195, y, 100, 18, value);
                    }
                }
                else
                {
                    if (i == columnCount)
                    {
                        y -= i * 30;
                    }

                    builder.AddHtmlLocalized(342, y, 200, 18, label, 0x560A, false, false);
                    builder.AddSpriteImage(307, y - 5, 0x9D3B, property.SpriteW, property.SpriteH, 30, 30);
                    builder.AddTooltip(property.Description);

                    if (!property.IsBoolen)
                    {
                        var value = Color(property.Value, GetItemValue(compareItems, label), property.Cap);
                        builder.AddHtml(492, y, 100, 18, value);
                    }
                }

                y += 30;
            }

            y += 3;
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (_mannequin == null || _mannequin.Deleted)
        {
            return;
        }

        switch (info.ButtonID)
        {
            case 1000:
                sender.Mobile.SendGump(new MannequinStatsGump(_mannequin, _item, 0));
                break;
            case 1001:
                sender.Mobile.SendGump(new MannequinStatsGump(_mannequin, _item, 1));
                break;
        }
    }

    private static double GetItemValue(List<ItemPropDefinition> list, int label)
    {
        if (list == null)
        {
            return 0;
        }

        var comparison = list[0];

        double equipmentValue = 0;
        double selectValue = 0;

        for (var i = 0; i < comparison.EquipmentItem.Count; i++)
        {
            if (comparison.EquipmentItem[i].LabelNumber == label)
            {
                equipmentValue = comparison.EquipmentItem[i].Value;
                break;
            }
        }

        for (var i = 0; i < comparison.SelectItem.Count; i++)
        {
            if (comparison.SelectItem[i].LabelNumber == label)
            {
                selectValue = comparison.SelectItem[i].Value;
                break;
            }
        }

        return selectValue - equipmentValue;
    }

    private string Color(double ev, double diff, int parmv)
    {
        if (_sameItem == null || diff == 0)
        {
            if (parmv == 0)
            {
                return $"<BASEFONT COLOR=#80BFFF>{ev}</BASEFONT>";
            }

            return $"<BASEFONT COLOR=#80BFFF>{ev}/{parmv}</BASEFONT>";
        }

        if (diff < 0)
        {
            if (parmv == 0)
            {
                return $"<BASEFONT COLOR=#800000>{ev} (-{Math.Abs(diff)})</BASEFONT>";
            }

            return $"<BASEFONT COLOR=#800000>{ev}/{parmv} (-{Math.Abs(diff)})</BASEFONT>";
        }

        if (parmv == 0)
        {
            return $"<BASEFONT COLOR=#008000>{ev} (+{diff})</BASEFONT>";
        }

        return $"<BASEFONT COLOR=#008000>{ev}/{parmv} (+{diff})</BASEFONT>";
    }

    // Properties that share an Order value keep their declaration order, matching the source's
    // stable OrderBy(Order); the list is small, so a stable insertion sort is used.
    private static List<ValuedProperty> GetCatalogProperties(List<ValuedProperty> properties, Catalog catalog)
    {
        var result = new List<ValuedProperty>();

        for (var i = 0; i < properties.Count; i++)
        {
            if (properties[i].Catalog == catalog)
            {
                result.Add(properties[i]);
            }
        }

        for (var i = 1; i < result.Count; i++)
        {
            var current = result[i];
            var j = i - 1;

            while (j >= 0 && result[j].Order > current.Order)
            {
                result[j + 1] = result[j];
                j--;
            }

            result[j + 1] = current;
        }

        return result;
    }

    private sealed class ItemPropDefinition
    {
        public List<ValuedProperty> SelectItem { get; }
        public List<ValuedProperty> EquipmentItem { get; }

        public ItemPropDefinition(Item selectItem, Item equipmentItem)
        {
            SelectItem = Mannequin.GetProperty(selectItem);
            EquipmentItem = Mannequin.GetProperty(equipmentItem);
        }
    }
}
