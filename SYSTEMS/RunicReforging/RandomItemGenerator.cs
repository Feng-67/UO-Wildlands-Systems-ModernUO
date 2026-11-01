using System;
using Server.Mobiles;

namespace Server.Items;

/// <summary>
/// Supplies the budget/difficulty numbers the reforging property generator uses.
/// </summary>
public static class RandomItemGenerator
{
    public const int FeluccaBudgetBonus = 100;
    public const int MinAdjustedBudget = 100;
    public const int MaxAdjustedBudget = 1150;

    /// <summary>
    /// The reforging property cap is 10, so this is the cap plus one; callers test
    /// <c>count &lt; MaxProps - 1</c> and <c>count &gt;= MaxProps</c>.
    /// </summary>
    public const int MaxProps = 11;

    /// <summary>
    /// Approximate creature score used as the reforging budget input. The ServUO implementation of
    /// this calculation was not available, so fame is used as the difficulty proxy.
    /// </summary>
    public static int GetDifficultyFor(BaseCreature bc) =>
        Math.Clamp(100 + bc.Fame / 25, MinAdjustedBudget, MaxAdjustedBudget);
}
