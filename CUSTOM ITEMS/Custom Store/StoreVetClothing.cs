/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using ModernUO.Serialization;
using Server;
using Server.Items;

namespace Server.Items;

// --- HUMAN / ELF ITEMS ---

[SerializationGenerator(1, false)]
public partial class StoreValoriteCloak : Cloak
{
    [Constructible]
    public StoreValoriteCloak()
    {
        Name = "A Cloak hued in Valorite";
        Hue = 2210; // Valorite Hue
        LootType = LootType.Blessed;
    } private void Deserialize(IGenericReader reader, int version) { }
}

[SerializationGenerator(1, false)]
public partial class StoreValoriteRobe : Robe
{
    [Constructible]
    public StoreValoriteRobe()
    {
        Name = "A Robe hued in Valorite";
        Hue = 2210;
        LootType = LootType.Blessed;
    } private void Deserialize(IGenericReader reader, int version) { }
}

[SerializationGenerator(1, false)]
public partial class StoreValoriteDress : PlainDress
{
    [Constructible]
    public StoreValoriteDress()
    {
        Name = "A Dress hued in Valorite";
        Hue = 2210;
        LootType = LootType.Blessed;
    } private void Deserialize(IGenericReader reader, int version) { }
}

// --- GARGOYLE ITEMS ---


