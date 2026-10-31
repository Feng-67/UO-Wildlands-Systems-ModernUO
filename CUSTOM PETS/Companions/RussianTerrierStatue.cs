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
public partial class RussianTerrierStatue : Item
{
    [Constructible]
    public RussianTerrierStatue() : base(0xA76F)
    {
        Name = "Russian Terrier [Companion]";
        LootType = LootType.Blessed;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (IsChildOf(from.Backpack))
        {
            from.SendGump(new ConfirmRussianTerrierGump(this));
        }
        else
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
    }

    private class ConfirmRussianTerrierGump : BaseConfirmGump
    {
        private readonly RussianTerrierStatue _statue;

        public ConfirmRussianTerrierGump(RussianTerrierStatue statue) => _statue = statue;

        public override void Confirm(Mobile from)
        {
            if (_statue?.Deleted != false || !_statue.IsChildOf(from.Backpack))
            {
                return;
            }

            var terrier = new RussianTerrier();

            if (from.Followers + terrier.ControlSlots > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049607);
                terrier.Delete();
                return;
            }

            if (!terrier.SetControlMaster(from))
            {
                terrier.Delete();
                return;
            }

            terrier.MoveToWorld(from.Location, from.Map);
            _statue.Delete();
        }
    }
}
