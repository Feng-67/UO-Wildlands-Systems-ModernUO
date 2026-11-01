/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using ModernUO.Serialization;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetDyeSampleVividCrimson : Item
{
    // Neither value was ever saved, so they are constants rather than serialized fields.
    private const int TargetHue = 1157; // Vivid Crimson
    private const string DyeName = "Vivid Crimson";

    public override string DefaultName => "Pet Dye Sample: Vivid Crimson";
    public override double DefaultWeight => 1.0;

    [Constructible]
    public PetDyeSampleVividCrimson() : base(0xEFB) // Graphic: Potion/Powder Pot
    {
        Hue = TargetHue;
        LootType = LootType.Blessed;
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        // Yellow font for the sample text
        list.Add(1114057, $"{"<BASEFONT COLOR=#FFFF00>30 sec Sample Pet Dye</BASEFONT>"}");
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!from.InRange(GetWorldLocation(), 1))
        {
            from.SendLocalizedMessage(500446); // That is too far away.
            return;
        }

        from.SendMessage($"Target the pet you wish to sample the {DyeName} dye on.");
        from.Target = new InternalPetTarget(this);
    }

    private class InternalPetTarget : Target
    {
        private readonly PetDyeSampleVividCrimson _sample;

        public InternalPetTarget(PetDyeSampleVividCrimson sample) : base(1, false, TargetFlags.None) => _sample = sample;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is not BaseCreature pet || pet.ControlMaster != from)
            {
                from.SendMessage("You can only dye your own pets!");
                return;
            }

            if (!from.InRange(_sample.GetWorldLocation(), 1) || !from.InRange(pet.Location, 1))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            var oldHue = pet.Hue; // Store the original colour
            pet.Hue = TargetHue;
            from.PlaySound(0x23E);
            from.SendMessage("The sample is applied! Your pet will revert in 30 seconds.");

            // Start the 30-second timer to revert the colour
            Timer.DelayCall(
                TimeSpan.FromSeconds(30.0),
                () =>
                {
                    if (pet.Deleted)
                    {
                        return;
                    }

                    pet.Hue = oldHue;
                    pet.ControlMaster?.SendMessage($"The {DyeName} sample has worn off.");
                }
            );

            _sample.Delete();
        }
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
