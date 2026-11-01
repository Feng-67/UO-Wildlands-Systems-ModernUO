/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using Server;
using Server.Targeting;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(1, false)]
    public partial class ScrollOfDecurse : Item
    {
        [Constructible]
        public ScrollOfDecurse() : base(0xA1E4) // Star Map East Graphic
        {
            Name = "Scroll of Decurse";
            Weight = 1.0;
           // Hue = 1266; 
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your backpack for you to use it.
                return;
            }

            from.SendMessage("Target the cursed item you wish to cleanse with the scroll.");
            from.Target = new DecurseTarget(this);
        }

        private class DecurseTarget : Target
        {
            private ScrollOfDecurse _scroll;

            public DecurseTarget(ScrollOfDecurse scroll) : base(1, false, TargetFlags.None)
            {
                _scroll = scroll;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (_scroll.Deleted || !_scroll.IsChildOf(from.Backpack))
                    return;

                if (targeted is Item item)
                {
                    if (!item.IsChildOf(from.Backpack) && item.Parent != from)
                    {
                        from.SendMessage("The item must be in your backpack or equipped to decurse it.");
                        return;
                    }

                    if (item.LootType == LootType.Cursed)
                    {
                        item.LootType = LootType.Regular;
                        
                        // Chivalry Remove Curse Sound & Visuals
                        from.PlaySound(0x5AA); 
                        from.FixedParticles(0x375A, 1, 15, 5001, 1266, 4, EffectLayer.Waist);
                        
                        from.SendMessage("The scroll's power purges the curse from the item.");
                        _scroll.Delete(); 
                    }
                    else
                    {
                        from.SendMessage("That item is not cursed.");
                    }
                }
                else
                {
                    from.SendMessage("You can only use this on items.");
                }
            }
        }

        private void Deserialize(IGenericReader reader, int version)
        {
        }
}
}
