using Server.Engines.Imbuing;

namespace Server.Items;

public abstract partial class BaseRunicTool
{
    public override void OnDoubleClick(Mobile from)
    {
        if (ReforgingToolHook.TryBeginReforging(from, this))
        {
            return; // consumed — don't open the craft gump
        }

        base.OnDoubleClick(from); // classic runic crafting (away from forge / no entitlement)
    }
}

/// <summary>
/// Shared gate for the reforging entry points. Mirrors the ServUO rule: re-forging requires a
/// soulforge in range, otherwise the runic tool behaves as a classic runic crafting tool.
/// </summary>
internal static class ReforgingToolHook
{
    private const int SoulForgeRange = 2;

    public static bool TryBeginReforging(Mobile from, BaseRunicTool tool)
    {
        if (from?.Backpack == null || tool?.Deleted != false)
        {
            return false;
        }

        // Tool must be in the player's pack and accessible, same precondition RunicReforgingTarget
        // relies on.
        if (!tool.IsChildOf(from.Backpack) || !BaseTool.CheckAccessible(tool, from))
        {
            return false;
        }

        // message: false — we fall through to the classic craft gump, so don't emit the
        // imbuing-specific "You must be near a soulforge to imbue an item." (1079787).
        if (!Imbuing.CheckSoulForge(from, SoulForgeRange, false))
        {
            return false;
        }

        from.Target = new RunicReforgingTarget(tool);
        return true;
    }
}
