/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author Methril)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;
using Server.Gumps;
using Server.Network;

namespace Server.Items;

// Fixed 120 deed. ServUO saved nothing for it (version 0, empty payload).
[SerializationGenerator(1, false)]
public partial class ChampionPowerScroll : Item
{
    public override string DefaultName => "Champion Power Scroll 120";
    public override double DefaultWeight => 1.0;

    [Constructible]
    public ChampionPowerScroll() : base(0x2258) // 0x2258 is the Power Scroll ItemID
    {
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            return;
        }

        PowerScrollPickerGump.DisplayTo(from, this, 120.0);
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Deed whose level (105/110/115/120) is set by staff or by whatever creates it.
[SerializationGenerator(1, false)]
public partial class PowerScrollChoice : Item
{
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    [InvalidateProperties]
    private double _deedLevel;

    public override string DefaultName => "Champion Power Scroll";
    public override double DefaultWeight => 1.0;

    [Constructible]
    public PowerScrollChoice() : this(120.0)
    {
    }

    [Constructible]
    public PowerScrollChoice(double skillLevel) : base(0x14F0)
    {
        _deedLevel = skillLevel;
        Hue = 88;
        ItemID = 8707;
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        list.Add($"Scroll: +{_deedLevel}");
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            return;
        }

        PowerScrollPickerGump.DisplayTo(from, this, _deedLevel);
    }

    // Legacy hook: loads saves written by the ServUO version.
    // ServUO wrote the level with Write(double) but read it back with ReadInt, which only worked because
    // each item is read from its own length-prefixed block. Reading the double here is the correct form.
    private void Deserialize(IGenericReader reader, int version)
    {
        _deedLevel = reader.ReadDouble();
    }
}

// Shared picker for both deeds (the ServUO files had two near-identical nested gumps).
public class PowerScrollPickerGump : DynamicGump
{
    private static readonly SkillName[] _skills =
    [
        SkillName.Alchemy,
        SkillName.Anatomy,
        SkillName.AnimalLore,
        SkillName.AnimalTaming,
        SkillName.Archery,
        SkillName.ArmsLore,
        SkillName.Begging,
        SkillName.Blacksmith,
        SkillName.Bushido,
        SkillName.Camping,
        SkillName.Carpentry,
        SkillName.Cartography,
        SkillName.Chivalry,
        SkillName.Cooking,
        SkillName.DetectHidden,
        SkillName.Discordance,
        SkillName.EvalInt,
        SkillName.Fencing,
        SkillName.Fletching,
        SkillName.Focus,
        SkillName.Forensics,
        SkillName.Healing,
        SkillName.Herding,
        SkillName.Hiding,
        SkillName.Inscribe,
        SkillName.ItemID,
        SkillName.Lockpicking,
        SkillName.Lumberjacking,
        SkillName.Macing,
        SkillName.Magery,
        SkillName.MagicResist,
        SkillName.Meditation,
        SkillName.Mining,
        SkillName.Musicianship,
        SkillName.Fishing,
        SkillName.Necromancy,
        SkillName.Ninjitsu,
        SkillName.Parry,
        SkillName.Peacemaking,
        SkillName.Poisoning,
        SkillName.Provocation,
        SkillName.RemoveTrap,
        SkillName.Snooping,
        SkillName.Spellweaving,
        SkillName.SpiritSpeak,
        SkillName.Stealing,
        SkillName.Stealth,
        SkillName.Swords,
        SkillName.Tactics,
        SkillName.Tailoring,
        SkillName.TasteID,
        SkillName.Tinkering,
        SkillName.Tracking,
        SkillName.Veterinary,
        SkillName.Wrestling
    ];

    private static readonly int[] _columnX = [27, 185, 344];

    // Derived from the data so adding/removing a skill can never walk off the end of _columnX.
    // 55 skills / 3 columns → 19 rows per column. (Was hard-coded to 18, which only fit 54 and
    // threw IndexOutOfRangeException on the 55th entry — i.e. every time the gump was built.)
    private static readonly int ColumnRows = (_skills.Length + _columnX.Length - 1) / _columnX.Length;

    private readonly Item _deed;
    private readonly double _level;

    public override bool Singleton => true;

    private PowerScrollPickerGump(Item deed, double level) : base(50, 50)
    {
        _deed = deed;
        _level = level;
    }

    public static void DisplayTo(Mobile from, Item deed, double level)
    {
        if (from?.NetState == null || deed == null || deed.Deleted)
        {
            return;
        }

        from.SendGump(new PowerScrollPickerGump(deed, level));
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();
        builder.AddBackground(0, 0, 475, 620, 9270);
        builder.AddLabel(128, 17, 52, $"Choose  a {_level}  Power  Scroll");

        for (var i = 0; i < _skills.Length; i++)
        {
            var col = i / ColumnRows;

            // Defensive guard: should never trigger with the current data, but prevents a
            // future skill-list / column change from crashing the whole gump build.
            if (col >= _columnX.Length)
            {
                break;
            }

            var x = _columnX[col];
            var y = 58 + i % ColumnRows * 27;

            // Button IDs are 1-based, matching the original gump
            builder.AddButton(x, y, 11410, 11400, i + 1);
            builder.AddLabel(x + 24, y - 1, 52, _skills[i].ToString());
        }

        builder.AddButton(15, 583, 4017, 4018, _skills.Length + 1);
        builder.AddLabel(51, 585, 10, "CLOSE  GUMP");
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;
        var index = info.ButtonID - 1;

        if (_deed.Deleted || index < 0 || index >= _skills.Length)
        {
            return; // Closed, or the CLOSE GUMP button
        }

        // The deed must still be in the player's pack (the original only checked this on double click)
        if (!_deed.IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            return;
        }

        var scroll = new PowerScroll(_skills[index], _level);

        if (from.Backpack == null || !from.Backpack.TryDropItem(from, scroll, false))
        {
            scroll.Delete();
            from.SendLocalizedMessage(502868); // Your backpack is too full.
            return;
        }

        from.SendMessage($"A {_level} Powerscroll has been added to your backpack!");
        _deed.Delete();
    }
}
