/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items;

// Derives from Item (not a weapon) so no weapon stats are shown
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class DavyJonesPoker : Item
{
    public override string DefaultName => "Davy Jones' Iron Poker";
    public override double DefaultWeight => 7.0;

    [Constructible]
    public DavyJonesPoker() : base(0xF62) // Standard spear graphic
    {
        Hue = 1315;
        LootType = LootType.Blessed;
        Layer = Layer.OneHanded; // Lets it be held without being a weapon
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!from.InRange(GetWorldLocation(), 2))
        {
            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            return;
        }

        from.SendMessage("Target a corpse in the water to sink it.");
        from.Target = new InternalTarget(this);
    }

    private class InternalTarget : Target
    {
        private readonly DavyJonesPoker _poker;

        public InternalTarget(DavyJonesPoker poker) : base(10, false, TargetFlags.None) => _poker = poker;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_poker.Deleted || !from.InRange(_poker.GetWorldLocation(), 2))
            {
                return;
            }

            if (targeted is not Corpse c)
            {
                from.SendMessage("That is not a corpse.");
                return;
            }

            if (BaseBoat.FindBoatAt(from.Location, from.Map) == null)
            {
                from.SendMessage("You must be on a boat to use this.");
                return;
            }

            if (!IsWater(c.Map, c.X, c.Y))
            {
                from.SendMessage("That corpse is not in the water.");
                return;
            }

            Effects.SendLocationEffect(c.Location, c.Map, 0x352E, 50); // Splash
            from.PlaySound(0x026);                                      // Splash sound

            c.Delete();
            from.SendMessage("You poke the remains into the depths.");
        }

        private static bool IsWater(Map map, int x, int y)
        {
            if (map == null)
            {
                return false;
            }

            var id = map.Tiles.GetLandTile(x, y).ID;

            if (id is >= 168 and <= 171 || id is 310 or 311) // Ocean, shallow water
            {
                return true;
            }

            foreach (var tile in map.Tiles.GetStaticTiles(x, y))
            {
                if (tile.ID is >= 6038 and <= 6066)
                {
                    return true;
                }
            }

            return false;
        }
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
