/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;
using Server.Gumps;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class NewfoundlandStatue : Item
{
    [Constructible]
    public NewfoundlandStatue() : base(0xA76B)
    {
        Name = "Newfoundland [Companion]";
        LootType = LootType.Blessed;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (IsChildOf(from.Backpack))
        {
            from.SendGump(new ConfirmNewfoundlandGump(this));
        }
        else
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
    }

    private class ConfirmNewfoundlandGump : BaseConfirmGump
    {
        private readonly NewfoundlandStatue _statue;

        public ConfirmNewfoundlandGump(NewfoundlandStatue statue) => _statue = statue;

        public override void Confirm(Mobile from)
        {
            if (_statue?.Deleted != false || !_statue.IsChildOf(from.Backpack))
            {
                return;
            }

            var newfoundland = new Newfoundland();

            if (from.Followers + newfoundland.ControlSlots > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                newfoundland.Delete();
                return;
            }

            if (!newfoundland.SetControlMaster(from))
            {
                newfoundland.Delete();
                return;
            }

            newfoundland.MoveToWorld(from.Location, from.Map);
            _statue.Delete();
        }
    }
}
