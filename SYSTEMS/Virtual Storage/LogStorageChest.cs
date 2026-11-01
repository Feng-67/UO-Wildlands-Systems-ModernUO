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
public partial class LogStorageChest : VirtualStorageChest
{
    private static readonly Type[] _allowedTypes =
    {
        // Standard and Special Logs
        typeof(Log), typeof(OakLog), typeof(AshLog),
        typeof(YewLog), typeof(HeartwoodLog), typeof(BloodwoodLog),
        typeof(FrostwoodLog),

        // Standard and Special Boards
        typeof(Board), typeof(OakBoard), typeof(AshBoard),
        typeof(YewBoard), typeof(HeartwoodBoard), typeof(BloodwoodBoard),
        typeof(FrostwoodBoard),

        // Fletching and Crafting Items
        typeof(Bolt), typeof(Arrow), typeof(Shaft),
        typeof(Feather), typeof(BlankScroll)
    };

    [Constructible]
    public LogStorageChest() : base(0xE7C) // Metal Chest Graphic
    {
    }

    public override string DefaultName => "Log & Board Storage Chest";

    public override string ChestTitle => "Wood Stockpile";

    public override Type[] AllowedTypes => _allowedTypes;
}
