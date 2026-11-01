/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using Server.Items;

namespace Server.Mobiles;

public interface IBardedMount
{
    bool BardingExceptional { get; set; }
    Mobile BardingCrafter { get; set; }
    int BardingHP { get; set; }
    bool HasBarding { get; set; }
    CraftResource BardingResource { get; set; }
    int BardingMaxHP { get; }
}
