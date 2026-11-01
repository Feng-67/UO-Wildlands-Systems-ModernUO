/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Authors Unknown)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class RunicAtlasExceptional : RunicAtlas
{
    public override string DefaultName => "Runic Atlas";

    [Constructible]
    public RunicAtlasExceptional()
    {
        MaxCharges = 100;
        CurCharges = 100;

        Quality = BookQuality.Exceptional;
        LootType = LootType.Blessed;
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
