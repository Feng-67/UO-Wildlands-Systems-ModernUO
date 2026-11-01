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
public partial class TailorStorageChest : VirtualStorageChest
{
    private static readonly Type[] _allowedTypes =
    {
        // Leather Types
        typeof(Leather), typeof(SpinedLeather), typeof(HornedLeather), typeof(BarbedLeather),

        // Raw Hide Types
        typeof(Hides), typeof(SpinedHides), typeof(HornedHides), typeof(BarbedHides),

        // Gems & Pelts
        typeof(ArcaneGem),
        //typeof(TigerPelt), typeof(WhiteTigerPelt), typeof(BlackTigerPelt),

        // Raw Materials
        typeof(Cotton), typeof(Flax), typeof(Wool), typeof(SpoolOfThread), typeof(LightYarn), typeof(DarkYarn),

        // Finished Materials
        typeof(Cloth), typeof(BoltOfCloth),
        //typeof(DragonTurtleScute),

        // Supplies
        typeof(Bandage), typeof(EnhancedBandage), typeof(Bone)
    };

    [Constructible]
    public TailorStorageChest() : base(0xE7C)
    {
    }

    public override string DefaultName => "Tailoring Storage Chest";

    public override string ChestTitle => "Tailoring Stockpile";

    public override Type[] AllowedTypes => _allowedTypes;
}
