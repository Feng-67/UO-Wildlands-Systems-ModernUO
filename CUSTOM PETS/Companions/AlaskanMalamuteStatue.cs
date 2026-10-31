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
public partial class AlaskanMalamuteStatue : Item
{
    [Constructible]
    public AlaskanMalamuteStatue() : base(0xA76C)
    {
        Name = "Alaskan Malamute [Companion]";
        LootType = LootType.Blessed;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (IsChildOf(from.Backpack))
        {
            from.SendGump(new ConfirmMalamuteGump(this));
        }
        else
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
    }

    private class ConfirmMalamuteGump : BaseConfirmGump
    {
        private readonly AlaskanMalamuteStatue _statue;

        public ConfirmMalamuteGump(AlaskanMalamuteStatue statue) => _statue = statue;

        public override void Confirm(Mobile from)
        {
            if (_statue?.Deleted != false || !_statue.IsChildOf(from.Backpack))
            {
                return;
            }

            var malamute = new AlaskanMalamute();

            if (from.Followers + malamute.ControlSlots > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                malamute.Delete();
                return;
            }

            if (!malamute.SetControlMaster(from))
            {
                malamute.Delete();
                return;
            }

            malamute.MoveToWorld(from.Location, from.Map);
            _statue.Delete();
        }
    }
}
