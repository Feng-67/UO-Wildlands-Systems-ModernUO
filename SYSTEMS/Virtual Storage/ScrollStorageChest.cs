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
public partial class ScrollStorageChest : VirtualStorageChest
{
    // Magery, Necromancy, Mysticism and Spellweaving scrolls all inherit from SpellScroll.
    private static readonly Type[] _allowedTypes = { typeof(SpellScroll) };

    [Constructible]
    public ScrollStorageChest() : base(0xE7C) // Metal Chest Graphic
    {
    }

    public override string DefaultName => "Scroll Storage Chest";

    public override string ChestTitle => "Scroll Stockpile";

    public override Type[] AllowedTypes => _allowedTypes;
}
