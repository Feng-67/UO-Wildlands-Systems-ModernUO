/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class ReagentStorageChest : VirtualStorageChest
{
    private static readonly Type[] _allowedTypes =
    {
        // Standard Reagents
        typeof(BlackPearl), typeof(Bloodmoss), typeof(Garlic),
        typeof(Ginseng), typeof(MandrakeRoot), typeof(Nightshade),
        typeof(SulfurousAsh), typeof(SpidersSilk),
        // Necro / Rare Reagents
        typeof(BatWing), typeof(GraveDust), typeof(DaemonBlood),
        typeof(NoxCrystal), typeof(PigIron), typeof(Bone),
        //typeof(DragonBlood),
        typeof(FertileDirt), typeof(DaemonBone),
        // Other
        typeof(Bottle)
    };

    [Constructible]
    public ReagentStorageChest() : base(0xE7C) // Metal Chest Graphic
    {
    }

    public override string DefaultName => "Reagent Storage Chest";

    public override string ChestTitle => "Reagent Stockpile";

    public override Type[] AllowedTypes => _allowedTypes;
}
