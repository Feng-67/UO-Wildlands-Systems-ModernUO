/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using Server;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(1, false)]
    public partial class BreakParalysisPotion : Item
    {
        [Constructible]
        public BreakParalysisPotion() : base(0xF09) // Standard potion bottle graphic
        {
            Name = "Break-Paralysis Potion";
           // Hue = 2543; // Matching the original VvV color
            Weight = 1.0;
            Stackable = true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(502138); // That is too far away.
                return;
            }

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            // The main logic: Break Paralysis/Frozen states
            if (from.Paralyzed || from.Frozen)
            {
                from.Paralyzed = false;
                from.Frozen = false;
                from.SendMessage("The potion's chemical reaction shatters the paralysis!");
            }
            else
            {
                from.SendMessage("You drink the potion, but you weren't paralyzed.");
            }

            // Visual Effect: Small explosion puff at the waist
            from.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.Waist);

            // Sound Effect: Mostly harmless explosion (sound 0x207)
            from.PlaySound(0x207);

            // --- FIXED CONSUMPTION LOGIC ---
            if (this.Amount > 1)
            {
                this.Amount--; // Just take one off the stack
            }
            else
            {
                this.Delete(); // Only delete if it was the last one
            }
        }

        private void Deserialize(IGenericReader reader, int version)
        {
        }
}
}
