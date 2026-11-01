using ModernUO.Serialization;
using Server.Collections;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Spells.BardMasteries;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class BookOfBardMasteries : Spellbook
{
    [Constructible]
    public BookOfBardMasteries() : base(0x3F, 0x225A)
    {
    }

    public override SpellbookType SpellbookType => SpellbookType.BardMasteries;

    public override int BookOffset => BardMasterySystem.BookOffset;

    public override int BookCount => 6;

    public override string DefaultName => "Book of Bard Masteries";

    public override void GetContextMenuEntries(Mobile from, ref PooledRefList<ContextMenuEntry> list)
    {
        base.GetContextMenuEntries(from, ref list);

        if (from == null)
        {
            return;
        }

        // "Switch Mastery" is only usable while the book is in the player's backpack.
        list.Add(new SwitchMasteryEntry(this) { Enabled = IsChildOf(from.Backpack) });
    }

    private sealed class SwitchMasteryEntry : ContextMenuEntry
    {
        private readonly BookOfBardMasteries _book;

        public SwitchMasteryEntry(BookOfBardMasteries book) : base(1151948, -1) // Switch Mastery
        {
            _book = book;
        }

        public override void OnClick(Mobile from, IEntity target)
        {
            if (from is PlayerMobile pm && !_book.Deleted && _book.IsChildOf(pm.Backpack))
            {
                pm.SendGump(new MasterySelectionGump(pm, _book));
            }
        }
    }

    private sealed class MasterySelectionGump : DynamicGump
    {
        private const int HueSelected = 0x8E2525;
        private const int HueNormal = 0x000066;

        private readonly PlayerMobile _user;
        private readonly BookOfBardMasteries _book;

        public MasterySelectionGump(PlayerMobile user, BookOfBardMasteries book) : base(75, 25)
        {
            _user = user;
            _book = book;
        }

        public override bool Singleton => true;

        protected override void BuildLayout(ref DynamicGumpBuilder builder)
        {
            builder.AddPage();
            builder.AddBackground(0, 0, 320, 150, 9380);
            builder.AddHtmlLocalized(0, 14, 320, 18, 1151948, 0x7FFF, false, false); // Switch Mastery

            var current = BardMasterySystem.GetActiveMastery(_user);

            for (var i = 0; i < BardMasterySystem.MasterySkills.Length; ++i)
            {
                var skill = BardMasterySystem.MasterySkills[i];
                var level = BardMasterySystem.GetMasteryLevel(_user, skill);
                var y = 44 + i * 26;

                if (level > 0)
                {
                    builder.AddButton(24, y, 4005, 4007, i + 1);
                }

                var hue = (skill == current ? HueSelected : HueNormal).C32216();
                builder.AddHtmlLocalized(64, y, 150, 18, BardMasterySystem.GetLocalization(skill), hue, false, false);
                builder.AddHtmlLocalized(210, y, 90, 18, 1156052, level.ToString(), hue, false, false); // ~1_val~
            }

            var currentName = current == SkillName.Alchemy ? "None" : _user.Skills[current].Name;
            builder.AddLabel(24, 124, 0x7FFF, $"Current: {currentName}");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            var index = info.ButtonID - 1;

            if (index < 0 || index >= BardMasterySystem.MasterySkills.Length)
            {
                return;
            }

            if (_book.Deleted || _user.Deleted || !_book.IsChildOf(_user.Backpack))
            {
                return;
            }

            var skill = BardMasterySystem.MasterySkills[index];

            if (BardMasterySystem.GetMasteryLevel(_user, skill) <= 0)
            {
                _user.SendLocalizedMessage(
                    1156236,
                    $"{BardMasterySystem.MinSkillRequirement}\t{_user.Skills[skill].Name}"
                ); // You need at least ~1_SKILL_REQUIREMENT~ ~2_SKILL_NAME~ skill to use that mastery.
                return;
            }

            if (BardMasterySystem.GetActiveMastery(_user) == skill)
            {
                // Selecting the active mastery again turns it off.
                BardMasterySystem.SetActiveMastery(_user, SkillName.Alchemy);
            }
            else
            {
                BardMasterySystem.SetActiveMastery(_user, skill);
                _user.SendLocalizedMessage(1155886, _user.Skills[skill].Name); // Your active skill mastery is now set to ~1_MasterySkill~!
            }

            BardMasterySystem.OnMasteryChanged(_user);

            _user.SendGump(new MasterySelectionGump(_user, _book));
        }
    }
}
