/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using Server.Targeting;
using Server.Mobiles;
using Server.Network;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(1, false)]
    public partial class PetDyeTub : DyeTub
    {
        public override int FailMessage => 1042083; // You cannot dye that.

        [Constructible]
        public PetDyeTub()
        {
            Name = "a pet dye tub";
            Weight = 10.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 1))
            {
                from.SendMessage("Target the pet you wish to dye.");
                from.Target = new InternalPetTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        private class InternalPetTarget : Target
        {
            private readonly PetDyeTub m_Tub;

            public InternalPetTarget(PetDyeTub tub) : base(1, false, TargetFlags.None)
            {
                m_Tub = tub;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)targeted;

                    // Range check: check the tub's world location and the pet's location
                    if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(pet.Location, 1))
                    {
                        from.SendLocalizedMessage(500446); // That is too far away.
                    }
                    else if (pet.ControlMaster != from)
                    {
                        from.SendMessage("You can only dye your own pets!");
                    }
                    else
                    {
                        pet.Hue = m_Tub.DyedHue;
                        from.PlaySound(0x23E); 
                        from.SendMessage("You dye your pet a new color!");
                    }
                }
                else
                {
                    from.SendMessage("That is not a pet!");
                }
            }
        }

        private void Deserialize(IGenericReader reader, int version)
        {
        }
}
}
