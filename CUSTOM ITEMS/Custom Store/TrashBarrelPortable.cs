/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using Server;
using Server.Multis;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(1, false)]
    public partial class TrashBarrelPortable : TrashBarrel
    {
        [Constructible]
        public TrashBarrelPortable() : base()
        {
            Name = "Portable Trash Barrel";
            Movable = true; // Allows it to be sold on vendors and carried
           // Hue = 0x386;    // Distinctive color so players know it's the special one
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            // Check if the barrel is locked down or secured
            if (!this.IsLockedDown && !this.IsSecure)
            {
                from.SendMessage("The trash barrel must be locked down or secured in a house to be used.");
                return false;
            }

            return base.OnDragDrop(from, dropped);
        }

        public override void OnDoubleClick(Mobile from)
        {
            // Prevents players from opening the container to use it as storage while in backpack
            if (!this.IsLockedDown && !this.IsSecure)
            {
                from.SendMessage("You must lock this down in your home to use it.");
                return;
            }

            base.OnDoubleClick(from);
        }

        private void Deserialize(IGenericReader reader, int version)
        {
        }
}
}
