/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using ModernUO.Serialization;
using Server.Engines.BuffIcons;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class AlacrityScrollPicker : Item
{
    public override string DefaultName => "Alacrity Scroll Picker";

    [Constructible]
    public AlacrityScrollPicker() : base(0x9981) // Scroll of Alacrity Book Graphic
    {
        Hue = 2601; // Unique Hue to distinguish from a normal book
        LootType = LootType.Blessed;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            return;
        }

        AlacrityPickerGump.DisplayTo(from, this);
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

public class AlacrityPickerGump : DynamicGump
{
    private const int SkillsPerPage = 10;

    private readonly AlacrityScrollPicker _picker;

    public override bool Singleton => true;

    private AlacrityPickerGump(AlacrityScrollPicker picker) : base(50, 50) => _picker = picker;

    public static void DisplayTo(Mobile from, AlacrityScrollPicker picker)
    {
        if (from?.NetState == null || picker == null || picker.Deleted)
        {
            return;
        }

        from.SendGump(new AlacrityPickerGump(picker));
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();
        builder.AddBackground(0, 0, 300, 450, 9270);
        builder.AddAlphaRegion(10, 10, 280, 430);
        builder.AddHtml(10, 20, 280, 20, "SELECT ALACRITY SKILL", "#FFA500", align: TextAlignment.Center);

        var page = 1;
        var count = 0;
        var y = 50;

        builder.AddPage(page);

        for (var i = 0; i < SkillInfo.Table.Length; i++)
        {
            if (count >= SkillsPerPage)
            {
                builder.AddButton(240, 410, 0xFA5, 0xFA7, 0, GumpButtonType.Page, page + 1);
                page++;
                builder.AddPage(page);
                builder.AddButton(20, 410, 0xFAE, 0xFAF, 0, GumpButtonType.Page, page - 1);
                count = 0;
                y = 50;
            }

            builder.AddButton(20, y, 0x4B9, 0x4BA, i + 1);
            builder.AddLabel(45, y - 2, 0x481, SkillInfo.Table[i].Name);

            y += 35;
            count++;
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (sender.Mobile is not PlayerMobile pm || _picker.Deleted || info.ButtonID == 0)
        {
            return;
        }

        var skillIndex = info.ButtonID - 1;

        if (skillIndex < 0 || skillIndex >= SkillInfo.Table.Length)
        {
            return;
        }

        if (!_picker.IsChildOf(pm.Backpack))
        {
            pm.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            return;
        }

        // Same rules as the stock Scroll of Alacrity: one accelerated skillgain at a time
        if (pm.AcceleratedStart > Core.Now)
        {
            pm.SendLocalizedMessage(1077951); // You are already under the effect of an accelerated skillgain scroll.
            return;
        }

        var selectedSkill = (SkillName)skillIndex;
        var skill = pm.Skills[selectedSkill];

        if (skill.Base >= skill.Cap || skill.Lock != SkillLock.Up)
        {
            // You cannot increase this skill at this time. The skill may be locked or set to lower in your skill menu.
            pm.SendLocalizedMessage(1094935);
            return;
        }

        // ModernUO's skill gain code reads AcceleratedStart and AcceleratedSkill directly.
        // The ServUO file called ScrollOfAlacrity.AlacrityEnd, which does not exist in ModernUO.
        pm.AcceleratedStart = Core.Now + TimeSpan.FromMinutes(15);
        pm.AcceleratedSkill = selectedSkill;

        // 1078511 = Scroll of Alacrity, 1078512 = accelerated gain description
        pm.AddBuff(new BuffInfo(BuffIcon.ArcaneEmpowerment, 1078511, 1078512, TimeSpan.FromMinutes(15), selectedSkill.ToString()));

        Timer.StartTimer(TimeSpan.FromMinutes(15), () => OnExpire(pm));

        pm.PlaySound(0xF5);
        pm.SendMessage(0x35, $"You have initiated accelerated skillgain for {selectedSkill}!");

        _picker.Delete();
    }

    private static void OnExpire(Mobile m)
    {
        m.PlaySound(0x1F8);
        // The intense energy dissipates. You are no longer under the effects of an accelerated skillgain scroll.
        m.SendLocalizedMessage(1077957);
    }
}
