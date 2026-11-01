/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using Server.Targeting;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class TransmogPotion : Item
{
    [Constructible]
    public TransmogPotion() : base(0x0EFF) // Bottle graphic
    {
        Name = "a Transmogrification Potion";
        Hue = 1150;
        LootType = LootType.Blessed;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            return;
        }

        from.SendMessage("Target the Artifact or item you want to change the appearance of.");
        from.Target = new InternalTarget(this);
    }

    private class InternalTarget : Target
    {
        private Item m_Potion;

        public InternalTarget(Item potion) : base(1, false, TargetFlags.None)
        {
            m_Potion = potion;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item targetItem)
            {
                if (!targetItem.IsChildOf(from.Backpack) && targetItem.Parent != from)
                {
                    from.SendMessage("The item must be in your backpack or equipped.");
                    return;
                }

                from.SendMessage("Target the item you want it to look like.");
                from.Target = new VisualTarget(m_Potion, targetItem);
            }
            else
            {
                from.SendMessage("That is not a valid item.");
            }
        }
    }

    private class VisualTarget : Target
    {
        private Item m_Potion;
        private Item m_StatItem;

        public VisualTarget(Item potion, Item statItem) : base(1, false, TargetFlags.None)
        {
            m_Potion = potion;
            m_StatItem = statItem;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item visualItem)
            {
                if (m_StatItem == visualItem)
                {
                    from.SendMessage("You cannot transmog an item into itself.");
                    return;
                }

                bool statIsTwoHanded = (m_StatItem is BaseWeapon && ((BaseWeapon)m_StatItem).Layer == Layer.TwoHanded);
                bool visualIsTwoHanded = (visualItem is BaseWeapon && ((BaseWeapon)visualItem).Layer == Layer.TwoHanded);

                if (statIsTwoHanded != visualIsTwoHanded)
                {
                    from.SendMessage("You cannot transmog a one-handed item into a two-handed item (or vice versa).");
                    return;
                }

                m_StatItem.ItemID = visualItem.ItemID;

                from.FixedParticles(0x373A, 10, 15, 5012, EffectLayer.Waist);
                from.PlaySound(0x1F2); 
                from.SendMessage("The item's appearance has been transformed!");

                m_Potion.Delete(); 
            }
            else
            {
                from.SendMessage("That is not a valid item.");
            }
        }
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    // Delete this method if existing saves do not need to load.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
