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
public partial class GreatDaneStatue : Item
{
    [Constructible]
    public GreatDaneStatue() : base(0xA76D)
    {
        Name = "Great Dane [Companion]";
        Hue = 1117;
        LootType = LootType.Blessed;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (IsChildOf(from.Backpack))
        {
            from.SendGump(new ConfirmGreatDaneGump(this));
        }
        else
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
    }

    private class ConfirmGreatDaneGump : BaseConfirmGump
    {
        private readonly GreatDaneStatue _statue;

        public ConfirmGreatDaneGump(GreatDaneStatue statue) => _statue = statue;

        public override void Confirm(Mobile from)
        {
            if (_statue?.Deleted != false || !_statue.IsChildOf(from.Backpack))
            {
                return;
            }

            var dane = new GreatDane();

            if (from.Followers + dane.ControlSlots > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                dane.Delete();
                return;
            }

            if (!dane.SetControlMaster(from))
            {
                dane.Delete();
                return;
            }

            dane.MoveToWorld(from.Location, from.Map);
            _statue.Delete();
        }
    }
}
