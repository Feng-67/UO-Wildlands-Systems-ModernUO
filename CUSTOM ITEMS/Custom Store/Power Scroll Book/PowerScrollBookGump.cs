/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Server.Gumps;
using Server.Network;

namespace Server.Items;

public class PowerScrollBookGump : DynamicGump
{
    private const int Width = 560;
    private const int Height = 590;
    private const int RowsPerPage = 17;

    private static readonly double[] _tiers = [105.0, 110.0, 115.0, 120.0];
    private static readonly int[] _tierCols = [190, 280, 370, 460];

    // Built on first use (PowerScrollBook._SkillInfo is filled in by its Initialize method at startup)
    private static List<SkillName> _allSkills;

    private readonly PowerScrollBook _book;
    private readonly int _page;
    private readonly Dictionary<(SkillName, double), int> _counts = new();

    public override bool Singleton => true;

    private PowerScrollBookGump(PowerScrollBook book, int page) : base(50, 50)
    {
        _book = book;
        _page = page;

        BuildIndex();
    }

    public static void DisplayTo(Mobile from, PowerScrollBook book, int page = 0)
    {
        if (from?.NetState == null || book == null || book.Deleted)
        {
            return;
        }

        // Singleton: sending a new instance replaces any open one, so no explicit CloseGump is needed
        from.SendGump(new PowerScrollBookGump(book, page));
    }

    private static List<SkillName> AllSkills => _allSkills ??= BuildSkillList();

    private static List<SkillName> BuildSkillList()
    {
        var skills = new List<SkillName>();

        if (PowerScrollBook._SkillInfo != null)
        {
            foreach (var list in PowerScrollBook._SkillInfo.Values)
            {
                foreach (var sn in list)
                {
                    if (!skills.Contains(sn))
                    {
                        skills.Add(sn);
                    }
                }
            }
        }

        // Sort A-Z by the label shown in the gump
        skills.Sort((x, y) => string.Compare(SkillLabel(x), SkillLabel(y), StringComparison.Ordinal));
        return skills;
    }

    private void BuildIndex()
    {
        if (_book.Entries == null)
        {
            return;
        }

        foreach (var item in _book.Entries)
        {
            if (item is PowerScroll ps)
            {
                var key = (ps.Skill, ps.Value);
                _counts[key] = _counts.GetValueOrDefault(key) + 1;
            }
        }
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        var allSkills = AllSkills;
        var totalPages = Math.Max(1, (int)Math.Ceiling(allSkills.Count / (double)RowsPerPage));

        builder.AddPage();
        builder.AddBackground(0, 0, Width, Height, 9270);

        builder.AddHtml(0, 25, Width, 25, "POWER SCROLL STORAGE", "#FFD700", align: TextAlignment.Center);
        builder.AddImageTiled(20, 50, Width - 40, 2, 96);

        // Column headers
        for (var i = 0; i < _tiers.Length; i++)
        {
            builder.AddHtml(_tierCols[i], 60, 80, 20, $"{(int)_tiers[i]}", "#FFFFFF", align: TextAlignment.Center);
        }

        var start = _page * RowsPerPage;
        var end = Math.Min(start + RowsPerPage, allSkills.Count);
        var y = 85;

        for (var si = start; si < end; si++)
        {
            var skill = allSkills[si];

            builder.AddImageTiled(20, y + 22, Width - 40, 1, 96);

            builder.AddHtml(30, y, 200, 22, SkillLabel(skill), "#FFD700");

            for (var ti = 0; ti < _tiers.Length; ti++)
            {
                _counts.TryGetValue((skill, _tiers[ti]), out var count);

                if (count > 0)
                {
                    // Withdraw button (small marble) and the stored count
                    builder.AddButton(_tierCols[ti] + 5, y + 5, 0x837, 0x838, 100 + si * 4 + ti);
                    builder.AddHtml(_tierCols[ti], y, 80, 22, $"{count}", "#00FF00", align: TextAlignment.Center);
                }
                else
                {
                    builder.AddHtml(_tierCols[ti], y, 80, 22, "-", "#444444", align: TextAlignment.Center);
                }
            }

            y += 26;
        }

        // Navigation
        if (_page > 0)
        {
            builder.AddButton(20, Height - 35, 4014, 4016, 2);
        }

        if (_page + 1 < totalPages)
        {
            builder.AddButton(Width - 50, Height - 35, 4005, 4007, 3);
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;
        var buttonId = info.ButtonID;

        if (_book == null || _book.Deleted || buttonId == 0)
        {
            return;
        }

        if (buttonId == 2)
        {
            DisplayTo(from, _book, _page - 1);
        }
        else if (buttonId == 3)
        {
            DisplayTo(from, _book, _page + 1);
        }
        else if (buttonId >= 100)
        {
            var id = buttonId - 100;
            var sIdx = id / 4;
            var tIdx = id % 4;
            var allSkills = AllSkills;

            if (sIdx >= allSkills.Count)
            {
                return;
            }

            // The player must still be able to reach the book
            if (!from.InRange(_book.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            var sk = allSkills[sIdx];
            var val = _tiers[tIdx];

            var toRemove = _book.Entries.Find(x => x is PowerScroll ps && ps.Skill == sk && ps.Value == val);

            if (toRemove != null)
            {
                if (from.Backpack != null && from.Backpack.TryDropItem(from, toRemove, false))
                {
                    _book.RemoveEntry(toRemove);
                    from.SendMessage("You withdraw the scroll.");
                }
                else
                {
                    from.SendMessage("Your backpack is full.");
                }
            }

            // Refresh with updated counts
            DisplayTo(from, _book, _page);
        }
    }

    private static string SkillLabel(SkillName skill) =>
        Regex.Replace(skill.ToString(), "(?<!^)([A-Z])", " $1");
}
