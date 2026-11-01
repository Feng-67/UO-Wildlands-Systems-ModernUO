/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Authors Unknown)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using Server;
using System;
using Server.Items;
using Server.Multis;
using Server.Targeting;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(2, false)] // ServUO wrote version 1; generator version is old + 1
public partial class SpellbookDyeTub : DyeTub
{
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private bool _allowPack;

    [Constructible]
    public SpellbookDyeTub()
    {
        Name = "Spellbook Dye Tub";
        Weight = 5.0;

        int hue = Utility.RandomDyedHue();
        Hue = hue;
        DyedHue = hue;

        _allowPack = true; // was the old field initializer
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (this.IsChildOf(from.Backpack))
        {
            DoPack(from);
        }
        else
        {
            DoOut(from);
        }
    }

    public void DoPack(Mobile from)
    {
        if (AllowPack)
        {
            DoOut(from);
        }
        else
        {
            from.SendMessage("The dyetub cannot be in your pack.");
        }
    }

    public void DoOut(Mobile from)
    {
        if (from.InRange(this.GetWorldLocation(), 1))
        {
            from.SendMessage("Select the item to dye");
            from.Target = new SpellbookDyeTubTarget(this);
        }
        else
        {
            from.SendLocalizedMessage(500446); // That is too far away.
        }
    }

    // Legacy hook: loads saves written by the ServUO version. Version 0 also carried four
    // inherited values that the old code discarded; kept so those saves still load cleanly.
    // Delete this method if existing saves do not need to load.
    private void Deserialize(IGenericReader reader, int version)
    {
        switch (version)
        {
            case 1:
                {
                    _allowPack = reader.ReadBool();
                    break;
                }
            case 0:
                {
                    // Safely reads and discards old version data to prevent crashes on existing items
                    reader.ReadInt(); // m_DyedHue
                    reader.ReadInt(); // i_charges
                    reader.ReadBool(); // m_Redyable
                    reader.ReadBool(); // m_Charged
                    break;
                }
        }
    }

    public class SpellbookDyeTubTarget : Target
    {
        private SpellbookDyeTub m_Tub;

        public SpellbookDyeTubTarget(SpellbookDyeTub tub) : base(12, false, TargetFlags.None)
        {
            m_Tub = tub;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item)
            {
                Item item = (Item)targeted;

                if (item is Spellbook)
                {
                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("The item must be in your pack.");
                    }
                    else
                    {
                        item.Hue = m_Tub.DyedHue;
                        from.PlaySound(0x23E);
                    }
                }
                else
                {
                    from.SendMessage("That item cannot be dyed.");
                }
            }
            else
            {
                from.SendMessage("You cannot dye that.");
            }
        }
    }
}
