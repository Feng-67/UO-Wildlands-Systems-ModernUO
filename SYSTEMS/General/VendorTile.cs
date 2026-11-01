/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author Bittiez)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */

using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class VendorTile : Item
{
    [Constructible]
    public VendorTile() : base(0x181D) => Movable = false;

    public override string DefaultName => "a vendor tile";

    public override bool Decays => false;
}
