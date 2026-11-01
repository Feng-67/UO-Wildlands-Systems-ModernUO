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
public partial class MinerStorageChest : VirtualStorageChest
{
    private static readonly Type[] _allowedTypes =
    {
        // Ore (also covers the small ore variants if they derive from these types)
        typeof(IronOre), typeof(DullCopperOre), typeof(ShadowIronOre), typeof(CopperOre),
        typeof(BronzeOre), typeof(GoldOre), typeof(AgapiteOre), typeof(VeriteOre),
        typeof(ValoriteOre),

        // Ingots
        typeof(IronIngot), typeof(DullCopperIngot), typeof(ShadowIronIngot), typeof(CopperIngot),
        typeof(BronzeIngot), typeof(GoldIngot), typeof(AgapiteIngot), typeof(VeriteIngot),
        typeof(ValoriteIngot),

        // Granite
        typeof(Granite), typeof(DullCopperGranite), typeof(ShadowIronGranite), typeof(CopperGranite),
        typeof(BronzeGranite), typeof(GoldGranite), typeof(AgapiteGranite), typeof(VeriteGranite),
        typeof(ValoriteGranite),

        // Scales
        typeof(RedScales), typeof(YellowScales), typeof(BlackScales), typeof(GreenScales),
        typeof(WhiteScales), typeof(BlueScales),

        // Sand
        typeof(Sand)
    };

    [Constructible]
    public MinerStorageChest() : base(0xE7C)
    {
    }

    public override string DefaultName => "Miner's Storage Chest";

    public override string ChestTitle => "Miner's Stockpile";

    public override Type[] AllowedTypes => _allowedTypes;
}
