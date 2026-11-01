/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using ModernUO.Serialization;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class VaseOfVirtue : Item
{
    [Constructible]
    public VaseOfVirtue() : base(0xB189)
    {
        Name = "Vase of Virtue";
        Weight = 1.0;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (from.InRange(GetWorldLocation(), 3))
        {
            from.CloseGump<VaseOfVirtueGump>();
            from.SendGump(new VaseOfVirtueGump(this));
        }
        else
        {
            from.SendMessage("You are too far away to use that.");
        }
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    // Delete this method if existing saves do not need to load.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

public class VaseOfVirtueGump : Gump
{
    private Item _ankh;

    // Layout constants
    private const int Width        = 430;
    private const int ColLeft      = 20;   // Left column button X
    private const int LabelLeft    = 50;   // Left column label X
    private const int ColRight     = 225;  // Right column button X
    private const int LabelRight   = 255;  // Right column label X
    private const int EntryHeight  = 24;

    public VaseOfVirtueGump(Item ankh) : base(100, 100)
    {
        _ankh = ankh;

        // --- Section layout measurements ---
        // Title row: 30px, divider: 8px, section headers: 25px, divider: 8px
        // 9 shrine rows: 9 * EntryHeight, gap: 15px
        // Tokuno header: 25px, divider: 8px, 3 rows: 3 * EntryHeight, bottom pad: 20px
        int topPad       = 15;
        int titleH       = 30;
        int divH         = 8;
        int sectionHdrH  = 25;
        int shrineRows   = 9;
        int tokunoRows   = 3;

        int totalHeight  = topPad + titleH + divH + sectionHdrH + divH
                         + (shrineRows * EntryHeight) + 15
                         + sectionHdrH + divH
                         + (tokunoRows * EntryHeight) + 20;

        AddPage(0);
        AddBackground(0, 0, Width, totalHeight, 9270);
        AddAlphaRegion(10, 10, Width - 20, totalHeight - 20);

        // --- Title ---
        AddHtml(10, topPad, Width - 20, titleH,
            "<CENTER><BASEFONT COLOR=#FFD700>VASE OF VIRTUE</BASEFONT></CENTER>",
            false, false);

        int y = topPad + titleH + 5;
        AddImageTiled(20, y, Width - 40, 2, 96);
        y += divH;

        // --- Column headers ---
        AddLabel(LabelLeft,  y, 310,  "Trammel");
        AddLabel(LabelRight, y, 462,  "Felucca");
        y += sectionHdrH;
        AddImageTiled(20, y, Width - 40, 2, 96);
        y += divH;

        // --- Trammel + Felucca shrine rows (side by side) ---
        string[] sharedShrines = new string[]
        {
            "Compassion",
            "Honesty",
            "Honor",
            "Humility",
            "Justice",
            "Sacrifice",
            "Spirituality",
            "Valor",
            "Chaos"
        };

        for (int i = 0; i < sharedShrines.Length; i++)
        {
            // Trammel button (IDs 1–9)
            AddButton(ColLeft,  y + 3, 2224, 2224, i + 1,   GumpButtonType.Reply, 0);
            AddLabel(LabelLeft, y,     310,  sharedShrines[i]);

            // Felucca button (IDs 101–109)
            AddButton(ColRight,  y + 3, 2224, 2224, i + 101, GumpButtonType.Reply, 0);
            AddLabel(LabelRight, y,     462,  sharedShrines[i]);

            y += EntryHeight;
        }

        y += 10;
        AddImageTiled(20, y, Width - 40, 2, 96);
        y += divH;

        // --- Tokuno header ---
        AddLabel(LabelLeft, y, 2212, "Tokuno Islands");
        y += sectionHdrH;
        AddImageTiled(20, y, Width - 40, 2, 96);
        y += divH;

        // --- Tokuno shrine rows (IDs 201–203) ---
        string[] tokunoShrines = new string[]
        {
            "Homare",
            "Isamu",
            "Makoto"
        };

        for (int i = 0; i < tokunoShrines.Length; i++)
        {
            AddButton(ColLeft,  y + 3, 2224, 2224, i + 201, GumpButtonType.Reply, 0);
            AddLabel(LabelLeft, y,     2212, tokunoShrines[i]);
            y += EntryHeight;
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        Mobile from = sender.Mobile;

        if (from == null)
            return;

        if (_ankh == null || !from.InRange(_ankh.GetWorldLocation(), 3))
        {
            from.SendMessage("You are too far away from the Ankh of Devotion to use it.");
            return;
        }

        int buttonID = info.ButtonID;
        Map targetMap = null;
        Point3D targetLocation = Point3D.Zero;

        switch (buttonID)
        {
            // Trammel
            case 1:   targetMap = Map.Trammel; targetLocation = new Point3D(1857, 872,  0);   break; // Compassion
            case 2:   targetMap = Map.Trammel; targetLocation = new Point3D(4216, 564,  36);  break; // Honesty
            case 3:   targetMap = Map.Trammel; targetLocation = new Point3D(1730, 3528, 3);   break; // Honor
            case 4:   targetMap = Map.Trammel; targetLocation = new Point3D(4276, 3697, 0);   break; // Humility
            case 5:   targetMap = Map.Trammel; targetLocation = new Point3D(1301, 641,  15);  break; // Justice
            case 6:   targetMap = Map.Trammel; targetLocation = new Point3D(3355, 298,  9);   break; // Sacrifice
            case 7:   targetMap = Map.Trammel; targetLocation = new Point3D(1603, 2489, 4);   break; // Spirituality
            case 8:   targetMap = Map.Trammel; targetLocation = new Point3D(2495, 3931, 0);   break; // Valor
            case 9:   targetMap = Map.Trammel; targetLocation = new Point3D(1461, 844,  0);   break; // Chaos

            // Felucca
            case 101: targetMap = Map.Felucca; targetLocation = new Point3D(1857, 872,  0);   break; // Compassion
            case 102: targetMap = Map.Felucca; targetLocation = new Point3D(4216, 564,  36);  break; // Honesty
            case 103: targetMap = Map.Felucca; targetLocation = new Point3D(1730, 3528, 3);   break; // Honor
            case 104: targetMap = Map.Felucca; targetLocation = new Point3D(4276, 3697, 0);   break; // Humility
            case 105: targetMap = Map.Felucca; targetLocation = new Point3D(1301, 641,  15);  break; // Justice
            case 106: targetMap = Map.Felucca; targetLocation = new Point3D(3355, 298,  9);   break; // Sacrifice
            case 107: targetMap = Map.Felucca; targetLocation = new Point3D(1603, 2489, 4);   break; // Spirituality
            case 108: targetMap = Map.Felucca; targetLocation = new Point3D(2495, 3931, 0);   break; // Valor
            case 109: targetMap = Map.Felucca; targetLocation = new Point3D(1461, 844,  0);   break; // Chaos

            // Tokuno
            case 201: targetMap = Map.Tokuno;  targetLocation = new Point3D(288,  711,  55);  break; // Homare
            case 202: targetMap = Map.Tokuno;  targetLocation = new Point3D(1044, 517,  15);  break; // Isamu
            case 203: targetMap = Map.Tokuno;  targetLocation = new Point3D(718,  1162, 25);  break; // Makoto

            default:
                return;
        }

        if (targetMap != null && targetLocation != Point3D.Zero)
        {
            BaseCreature.TeleportPets(from, targetLocation, targetMap);
            from.MoveToWorld(targetLocation, targetMap);
        }
    }
}
