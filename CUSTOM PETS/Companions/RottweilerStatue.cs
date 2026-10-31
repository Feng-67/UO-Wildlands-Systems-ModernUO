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
public partial class RottweilerStatue : Item
{
    [Constructible]
    public RottweilerStatue() : base(0xA770)
    {
        Name = "Rottweiler [Companion]";
        LootType = LootType.Blessed;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (IsChildOf(from.Backpack))
        {
            from.SendGump(new ConfirmRottweilerGump(this));
        }
        else
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
    }

    private class ConfirmRottweilerGump : BaseConfirmGump
    {
        private readonly RottweilerStatue _statue;

        public ConfirmRottweilerGump(RottweilerStatue statue) => _statue = statue;

        public override void Confirm(Mobile from)
        {
            if (_statue?.Deleted != false || !_statue.IsChildOf(from.Backpack))
            {
                return;
            }

            var rottweiler = new Rottweiler();

            if (from.Followers + rottweiler.ControlSlots > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049607);
                rottweiler.Delete();
                return;
            }

            if (!rottweiler.SetControlMaster(from))
            {
                rottweiler.Delete();
                return;
            }

            rottweiler.MoveToWorld(from.Location, from.Map);
            _statue.Delete();
        }
    }
}
