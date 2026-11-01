/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using Server;
using Server.Gumps;
using Server.Network;
using System;

namespace Server.Gumps;

public class DeathTeleportGump : Gump
{
    // Same palette as MyCommandsGump / PlayerStatsGump / CorpseRetrievalStoneGump.
    private const int C_YELLOW = 2213;
    private const int C_WHITE = 2036;

    // Same width as CorpseRetrievalStoneGump so the two read as a matched pair.
    private const int Width = 400;
    private const int Height = 220;

    private readonly Mobile _mobile;

    public DeathTeleportGump(Mobile mobile) : base(150, 150)
    {
        _mobile = mobile;

        AddPage(0);

        // --- Matching background stack from CorpseRetrievalStoneGump ---
        AddBackground(0, 0, Width, Height, 9200);
        AddImageTiled(10, 10, Width - 20, Height - 20, 2624);
        AddAlphaRegion(10, 10, Width - 20, Height - 20);

        // --- HEADER: matching gold, matching divider ---
        AddHtml(0, 20, Width, 25,
            "<BASEFONT COLOR=#FFCC00><CENTER>CORPSE RETRIEVAL</CENTER></BASEFONT>",
            false, false);

        AddImageTiled(20, 50, Width - 40, 2, 9264);

        // --- Skull and crossbones centered under the divider ---
        AddImage(164, 62, 9804);

        // --- Body text: white, centered ---
        AddHtml(30, 125, Width - 60, 40,
            "<BASEFONT COLOR=#FFFFFF><CENTER>Travel to the Corpse Retrieval Stone?</CENTER></BASEFONT>",
            false, false);

        // --- Buttons in the same style as the retrieval gump ---
        AddButton(105, 175, 4005, 4007, 1, GumpButtonType.Reply, 0);
        AddLabel(140, 178, C_WHITE, "CONTINUE");

        AddButton(220, 175, 4005, 4007, 0, GumpButtonType.Reply, 0);
        AddLabel(250, 178, C_WHITE, "CANCEL");
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID != 1)
        {
            return;
        }

        _mobile.MoveToWorld(new Point3D(3471, 2601, 10), Map.Trammel);

        // Give the client a moment to process the teleport, then offer
        // resurrection the same way a BaseHealer would on approach.
        Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
        {
            if (_mobile == null || _mobile.Deleted || _mobile.Alive)
            {
                return;
            }

            var stone = ResurrectHelper.FindStone();

            if (stone != null)
            {
                ResurrectHelper.Resurrect(_mobile, stone);
            }
        });
    }
}
