/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Gumps;
using Server.Network;

namespace Server.Items;

[Flippable(0x9A95, 0x9AA7)]
[SerializationGenerator(2, false)] // ServUO wrote version 1; generator version is old + 1
public partial class PowerScrollBook : BaseSpecialScrollBook
{
    // Scrolls are internalized and tracked in this list, not stored as container contents.
    // Change it through AddEntry / RemoveEntry so the book is marked dirty and saved.
    [SerializableField(0, setter: "private")]
    private List<Item> _entries;

    public override Type ScrollType => typeof(PowerScroll);
    public override int LabelNumber => 1155684;
    public override int BadDropMessage => 1155691;
    public override int DropMessage => 1155692;
    public override int RemoveMessage => 1155690;
    public override int GumpTitle => 1155689;

    public override Dictionary<SkillCat, List<SkillName>> SkillInfo => _SkillInfo;
    public override Dictionary<int, double> ValueInfo => _ValueInfo;

    public static Dictionary<SkillCat, List<SkillName>> _SkillInfo;
    public static Dictionary<int, double> _ValueInfo;

    [Constructible]
    public PowerScrollBook() : base(0x9A95)
    {
        Hue = 1153;
        _entries = [];
    }

    public void AddEntry(Item scroll)
    {
        _entries.Add(scroll);
        this.MarkDirty();
    }

    public bool RemoveEntry(Item scroll)
    {
        if (!_entries.Remove(scroll))
        {
            return false;
        }

        this.MarkDirty();
        return true;
    }

    public override void OnAfterDelete()
    {
        base.OnAfterDelete();

        // Scrolls are internalized, not container contents, so they must be deleted with the book
        foreach (var scroll in _entries)
        {
            scroll.Delete();
        }
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!from.InRange(GetWorldLocation(), 2))
        {
            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            return;
        }

        PowerScrollBookGump.DisplayTo(from, this);
    }

    // Accepts Power Scrolls and refreshes the gump
    public override bool OnDragDrop(Mobile from, Item dropped)
    {
        if (dropped is not PowerScroll)
        {
            from.SendLocalizedMessage(BadDropMessage);
            return false;
        }

        AddEntry(dropped);
        dropped.Internalize();
        from.SendLocalizedMessage(DropMessage);

        // Refresh the gump if it's open
        PowerScrollBookGump.DisplayTo(from, this);

        return true;
    }

    // Legacy hook: loads saves written by the ServUO version
    // (version 0 had no entries, version 1 wrote the entries list).
    private void Deserialize(IGenericReader reader, int version)
    {
        _entries = version >= 1 ? reader.ReadEntityList<Item>() : [];
    }

    public static void Initialize()
    {
        _SkillInfo = new Dictionary<SkillCat, List<SkillName>>
        {
            [SkillCat.Combat] =
            [
                SkillName.Anatomy, SkillName.Archery, SkillName.Fencing, SkillName.Focus,
                SkillName.Healing, SkillName.Macing, SkillName.Parry, SkillName.Swords,
                SkillName.Tactics, SkillName.Throwing, SkillName.Wrestling, SkillName.Lumberjacking
            ],
            [SkillCat.TradeSkills] =
            [
                SkillName.Blacksmith, SkillName.Tailoring, SkillName.Alchemy, SkillName.Carpentry,
                SkillName.Cooking, SkillName.Inscribe, SkillName.Tinkering, SkillName.Fletching
            ],
            [SkillCat.Magic] =
            [
                SkillName.Bushido, SkillName.Chivalry, SkillName.EvalInt, SkillName.Imbuing,
                SkillName.Magery, SkillName.Meditation, SkillName.Mysticism, SkillName.Necromancy,
                SkillName.Ninjitsu, SkillName.MagicResist, SkillName.Spellweaving, SkillName.SpiritSpeak
            ],
            [SkillCat.Wilderness] =
            [
                SkillName.AnimalLore, SkillName.AnimalTaming, SkillName.Fishing, SkillName.Veterinary,
                SkillName.Camping, SkillName.Forensics, SkillName.Herding, SkillName.Tracking
            ],
            [SkillCat.Thievery] =
            [
                SkillName.Stealing, SkillName.Stealth, SkillName.DetectHidden, SkillName.Hiding,
                SkillName.Lockpicking, SkillName.Poisoning, SkillName.RemoveTrap, SkillName.Snooping
            ],
            [SkillCat.Bard] =
            [
                SkillName.Discordance, SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation
            ],
            [SkillCat.Miscellaneous] =
            [
                SkillName.ArmsLore, SkillName.Begging, SkillName.Cartography, SkillName.ItemID, SkillName.TasteID
            ]
        };

        _ValueInfo = new Dictionary<int, double>
        {
            [1155685] = 105,
            [1155686] = 110,
            [1155687] = 115,
            [1155688] = 120
        };
    }
}
