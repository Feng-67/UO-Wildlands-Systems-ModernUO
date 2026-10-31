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
public partial class SaintBernardStatue : Item
{
    [Constructible]
    public SaintBernardStatue() : base(0xA76E)
    {
        Name = "Saint Bernard [Companion]";
        LootType = LootType.Blessed;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (IsChildOf(from.Backpack))
        {
            from.SendGump(new ConfirmSaintBernardGump(this));
        }
        else
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
    }

    private class ConfirmSaintBernardGump : BaseConfirmGump
    {
        private readonly SaintBernardStatue _statue;

        public ConfirmSaintBernardGump(SaintBernardStatue statue) => _statue = statue;

        public override void Confirm(Mobile from)
        {
            if (_statue?.Deleted != false || !_statue.IsChildOf(from.Backpack))
            {
                return;
            }

            var saint = new SaintBernard();

            if (from.Followers + saint.ControlSlots > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049607);
                saint.Delete();
                return;
            }

            if (!saint.SetControlMaster(from))
            {
                saint.Delete();
                return;
            }

            saint.MoveToWorld(from.Location, from.Map);
            _statue.Delete();
        }
    }
}
