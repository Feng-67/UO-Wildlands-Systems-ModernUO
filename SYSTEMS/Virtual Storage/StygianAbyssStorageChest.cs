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
public partial class StygianAbyssStorageChest : VirtualStorageChest
{
    private static readonly Type[] _allowedTypes =
    {
        // Imbuing Ingredients (Core)
        typeof(MagicalResidue), typeof(EnchantedEssence), typeof(RelicFragment),
        typeof(ArcanicRuneStone), typeof(CrushedGlass), typeof(CrystalShards),

        // Essences
        typeof(EssencePrecision), typeof(EssenceAchievement), typeof(EssenceBalance),
        typeof(EssenceControl), typeof(EssenceDiligence), typeof(EssenceDirection),
        typeof(EssenceFeeling), typeof(EssenceOrder), typeof(EssencePassion),
        typeof(EssencePersistence), typeof(EssenceSingularity),

        // Monster/Expansion Drops
        typeof(AbyssalCloth), typeof(BottleIchor), typeof(BouraPelt),
        typeof(ChagaMushroom), typeof(CrystallineBlackrock), typeof(DaemonClaw),
        typeof(DelicateScales), typeof(ElvenFletching), typeof(FaeryDust),
        //typeof(Fur),
        typeof(GoblinBlood), typeof(LavaSerpentCrust),
        //typeof(MedusaBlood), typeof(MedusaLightScales), typeof(MedusaDarkScales),
        typeof(PowderedIron),
        //typeof(PrimalLichDust),
        typeof(RaptorTeeth),
        typeof(ReflectiveWolfEye), typeof(SeedOfRenewal),
        //typeof(ScouringToxin),
        //typeof(SilverSerpentVenom),
        typeof(SilverSnakeSkin),
        //typeof(SlithEye),
        typeof(SlithTongue), typeof(SpiderCarapace),
        //typeof(ToxicVenomSac),
        typeof(UndyingFlesh), typeof(VialOfVitriol),
        //typeof(VileTentacles),
        //typeof(VoidCore), typeof(VoidEssence),
        typeof(VoidOrb)
    };

    [Constructible]
    public StygianAbyssStorageChest() : base(0xE7C)
    {
    }

    public override string DefaultName => "Stygian Abyss Storage Chest";

    public override string ChestTitle => "Stygian Stockpile";

    public override Type[] AllowedTypes => _allowedTypes;
}
