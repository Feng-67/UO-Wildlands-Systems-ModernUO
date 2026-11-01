/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2026 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: AIType.cs                                                       *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program. If not, see <http://www.gnu.org/licenses/>.  *
 ************************************************************************/

namespace Server.Mobiles;

public enum AIType
{
    AI_Use_Default,
    AI_Melee,
    AI_Animal,
    AI_Archer,
    AI_Healer,
    AI_Vendor,
    AI_Mage,
    AI_Berserk,
    AI_Predator,
    AI_Thief,
    AI_Samurai,
    AI_Ninja,
    AI_Mystic,
    AI_Spellweaving,
    AI_Necro,
    AI_NecroMage,
    AI_Paladin
}

public static class AITypeExtensions
{
    /// <summary>
    /// Collapses the pet-training AI types onto the AI implementations this core actually builds.
    /// <c>BaseCreature.ChangeAIType</c> returns null for anything outside the stock ten and stops
    /// the AI timer first, so assigning one of the extended values directly would leave the
    /// creature with no AI at all. Always assign through this.
    /// </summary>
    public static AIType Supported(this AIType type) => type switch
    {
        AIType.AI_Samurai or AIType.AI_Ninja => AIType.AI_Melee,
        AIType.AI_Mystic or AIType.AI_Spellweaving or AIType.AI_Necro or
            AIType.AI_NecroMage or AIType.AI_Paladin => AIType.AI_Mage,
        _ => type,
    };
}
