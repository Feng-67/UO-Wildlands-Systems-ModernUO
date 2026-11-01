/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using Server;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class HooksShield : HeaterShield
{
    // TODO(migrate): ServUO's `public override bool IsArtifact => true;` was dropped — ModernUO has
    // no `Item.IsArtifact` member (it was a shard-core addition). Re-add once the core provides one.

    // Override resistances here
    public override int BasePhysicalResistance => 0;
    public override int BaseFireResistance => 10;
    public override int BaseColdResistance => 0;
    public override int BasePoisonResistance => 0;
    public override int BaseEnergyResistance => 10;

    [Constructible]
    public HooksShield()
    {
        Name = "Hook's Shield";
        ItemID = 0xA64A; // Sets the graphic to Shield_Pirate
        //Hue = 1175; // Matches the dark pirate theme

        // Set Durability here
        MaxHitPoints = 255;
        HitPoints = 255;

        // Primary Attributes
        Attributes.SpellChanneling = 1;
        Attributes.CastSpeed = 1;         // Faster Casting 1
        Attributes.SpellDamage = 10;      // Spell Damage Increase 10%
        Attributes.DefendChance = 15;     // Defense Chance Increase 15%

        // Requirement adjustment
        ArmorAttributes.LowerStatReq = 100;
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        // This adds the "Artifact" tag to the tooltip if desired
        // list.Add(1061078, $"{"Artifact"}\t{"Hook's Shield"}");
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    // Delete this method if existing saves do not need to load.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
