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
public partial class MondainStorageChest : VirtualStorageChest
{
    private static readonly Type[] _allowedTypes =
    {
        // Standard and ML Gems
        typeof(Citrine), typeof(EcruCitrine), typeof(Emerald), typeof(PerfectEmerald),
        typeof(Tourmaline), typeof(Turquoise), typeof(Diamond), typeof(BlueDiamond),
        typeof(Sapphire), typeof(StarSapphire), typeof(DarkSapphire), typeof(Ruby),
        typeof(FireRuby), typeof(Amber), typeof(BrilliantAmber), typeof(Amethyst),
        typeof(ArcaneGem), typeof(WhitePearl),

        // ML Harvesting Specials
        typeof(BarkFragment), typeof(LuminescentFungi), typeof(ParasiticPlant), typeof(SwitchItem),

        // Peerless Reagents
        typeof(Corruption), typeof(Taint), typeof(Blight), typeof(Putrefaction),
        typeof(Muculent), typeof(Scourge), typeof(DaemonBone), typeof(DiseasedBark),
        typeof(BloodOfTheDarkFather)
    };

    [Constructible]
    public MondainStorageChest() : base(0xE7C)
    {
    }

    public override string DefaultName => "Mondain's Legacy Storage Chest";

    public override string ChestTitle => "Mondain's Stockpile";

    public override Type[] AllowedTypes => _allowedTypes;
}
