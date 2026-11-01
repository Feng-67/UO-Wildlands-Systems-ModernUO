/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author ExX)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */

using System;
using ModernUO.Serialization;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Scripts.Commands;

public enum EmotePage
{
    P1,
    P2,
    P3,
    P4
}

public static class Emote
{
    public static void Configure()
    {
        CommandSystem.Register("emote", AccessLevel.Player, Emote_OnCommand);
        CommandSystem.Register("e", AccessLevel.Player, Emote_OnCommand);
    }

    [Usage("<sound>")]
    [Description("Emote with sounds, words, and possibly an animation with one command!")]
    public static void Emote_OnCommand(CommandEventArgs e)
    {
        var pm = e.Mobile;
        var em = e.ArgString.Trim();

        var soundInt = em switch
        {
            "ah"             => 1,
            "ahha"           => 2,
            "applaud"        => 3,
            "blownose"       => 4,
            "bow"            => 5,
            "bscough"        => 6,
            "burp"           => 7,
            "clearthroat"    => 8,
            "cough"          => 9,
            "cry"            => 10,
            "faint"          => 11,
            "fart"           => 12,
            "gasp"           => 13,
            "giggle"         => 14,
            "groan"          => 15,
            "growl"          => 16,
            "hey"            => 17,
            "hiccup"         => 18,
            "huh"            => 19,
            "kiss"           => 20,
            "laugh"          => 21,
            "no"             => 22,
            "oh"             => 23,
            "oooh"           => 24,
            "oops"           => 25,
            "puke"           => 26,
            "punch"          => 27,
            "scream"         => 28,
            "shush"          => 29,
            "sigh"           => 30,
            "slap"           => 31,
            "sneeze"         => 32,
            "sniff"          => 33,
            "snore"          => 34,
            "spit"           => 35,
            "stickouttongue" => 36,
            "tapfoot"        => 37,
            "wistle"         => 38,
            "woohoo"         => 39,
            "yawn"           => 40,
            "yea"            => 41,
            "yell"           => 42,
            _                => 0
        };

        if (soundInt > 0)
        {
            ESound.Play(pm, soundInt);
        }
        else
        {
            EmoteGump.DisplayTo(pm, EmotePage.P1);
        }
    }
}

public class EmoteGump : DynamicGump
{
    private const int White = 0xFFFFFF;

    // Button ID layout: 1 + (index * 15) + type
    //   type 0 = page navigation (index = target page, 1-4)
    //   type 1 = emote button    (index = emote number, 1-42)
    private const int ButtonStride = 15;

    // Emote labels per page. Emote numbers run consecutively: P1 = 1-12, P2 = 13-24, P3 = 25-36, P4 = 37-42.
    private static readonly string[][] _pageLabels =
    {
        new[]
        {
            "Ah", "Ah-ha", "Applaud", "Blows Nose", "Bows", "BS Cough",
            "Burp", "Clear Throat", "Cough", "Cry", "Faints", "Fart"
        },
        new[]
        {
            "Gasp", "Giggle", "Groan", "Growl", "Hey", "Hiccup",
            "Huh", "Kiss", "Laughs", "No", "Oh", "Oooh"
        },
        new[]
        {
            "Oops", "Puke", "Punch", "Scream", "Shush", "Sigh",
            "Slap", "Sneeze", "Sniff", "Snore", "Spit", "Sticks Tongue"
        },
        new[]
        {
            "Taps Foot", "Whistle", "Woohoo", "Yawn", "Yea", "Yell"
        }
    };

    private readonly EmotePage _page;

    public override bool Singleton => true;

    private EmoteGump(EmotePage page) : base(600, 50) => _page = page;

    public static void DisplayTo(Mobile from, EmotePage page) => from.SendGump(new EmoteGump(page));

    private static int GetButtonID(int type, int index) => 1 + index * ButtonStride + type;

    private static void AddButtonLabeled(ref DynamicGumpBuilder builder, int x, int y, int buttonID, string text)
    {
        builder.AddButton(x, y - 1, 4005, 4007, buttonID);
        builder.AddHtml(x + 35, y, 240, 20, $"<BASEFONT COLOR=#{White:X6}>{text}</BASEFONT>");
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();
        builder.AddBackground(0, 65, 150, 360, 5054);
        builder.AddAlphaRegion(10, 70, 130, 350);
        builder.AddImageTiled(10, 70, 130, 20, 9354);
        builder.AddLabel(42, 70, 200, "Emote List");

        var pageIndex = (int)_page;
        var labels = _pageLabels[pageIndex];
        var firstEmote = pageIndex * 12 + 1;

        for (var i = 0; i < labels.Length; i++)
        {
            AddButtonLabeled(ref builder, 15, 95 + i * 25, GetButtonID(1, firstEmote + i), labels[i]);
        }

        // Previous page arrow
        if (pageIndex > 0)
        {
            builder.AddButton(10, 380, 4506, 4508, GetButtonID(0, pageIndex));
        }

        // Next page arrow
        if (pageIndex < _pageLabels.Length - 1)
        {
            builder.AddButton(90, 380, 4502, 504, GetButtonID(0, pageIndex + 2));
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var val = info.ButtonID - 1;

        if (val < 0)
        {
            return;
        }

        var from = sender.Mobile;
        var type = val % ButtonStride;
        var index = val / ButtonStride;

        switch (type)
        {
            case 0: // Page navigation
                {
                    if (index is < 1 or > 4)
                    {
                        return;
                    }

                    DisplayTo(from, (EmotePage)(index - 1));
                    break;
                }
            case 1: // Emote
                {
                    if (index is > 0 and < 43)
                    {
                        DisplayTo(from, (EmotePage)((index - 1) / 12));
                    }

                    ESound.Play(from, index);
                    break;
                }
        }
    }
}

[SerializationGenerator(1, false)]
public partial class Puke : Item
{
    private TimerExecutionToken _timerToken;

    [Constructible]
    public Puke() : base(Utility.RandomList(0xf3b, 0xf3c))
    {
        Hue = 0x557;
        Movable = false;

        Timer.StartTimer(TimeSpan.FromSeconds(10.0), Delete, out _timerToken);
    }

    public override string DefaultName => "A Pile of Puke";

    public override void OnAfterDelete()
    {
        base.OnAfterDelete();

        _timerToken.Cancel();
    }

    // Puke is temporary: none should survive a world load.
    [AfterDeserialization]
    private void AfterDeserialization() => Delete();

    // Handles pre-codegen saves (old version 0 wrote no data).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

public static class ESound
{
    public static void Play(Mobile pm, int soundMade)
    {
        switch (soundMade)
        {
            case 1:
                pm.PlaySound(pm.Female ? 778 : 1049);
                pm.Say("*ah!*");
                break;
            case 2:
                pm.PlaySound(pm.Female ? 779 : 1050);
                pm.Say("*ah ha!*");
                break;
            case 3:
                pm.PlaySound(pm.Female ? 780 : 1051);
                pm.Say("*applauds*");
                break;
            case 4:
                pm.PlaySound(pm.Female ? 781 : 1052);
                pm.Say("*blows nose*");
                if (!pm.Mounted)
                {
                    pm.Animate(34, 5, 1, true, false, 0);
                }

                break;
            case 5:
                pm.Say("*bows*");
                if (!pm.Mounted)
                {
                    pm.Animate(32, 5, 1, true, false, 0);
                }

                break;
            case 6:
                pm.PlaySound(pm.Female ? 786 : 1057);
                pm.Say("*bs cough*");
                break;
            case 7:
                pm.PlaySound(pm.Female ? 782 : 1053);
                pm.Say("*burp!*");
                if (!pm.Mounted)
                {
                    pm.Animate(33, 5, 1, true, false, 0);
                }

                break;
            case 8:
                pm.PlaySound(pm.Female ? 748 : 1055);
                pm.Say("*clears throat*");
                if (!pm.Mounted)
                {
                    pm.Animate(33, 5, 1, true, false, 0);
                }

                break;
            case 9:
                pm.PlaySound(pm.Female ? 785 : 1056);
                pm.Say("*cough!*");
                if (!pm.Mounted)
                {
                    pm.Animate(33, 5, 1, true, false, 0);
                }

                break;
            case 10:
                pm.PlaySound(pm.Female ? 787 : 1058);
                pm.Say("*cries*");
                break;
            case 11:
                pm.PlaySound(pm.Female ? 791 : 1063);
                pm.Say("*faints*");
                if (!pm.Mounted)
                {
                    pm.Animate(22, 5, 1, true, false, 0);
                }

                break;
            case 12:
                pm.PlaySound(pm.Female ? 792 : 1064);
                pm.Say("*farts*");
                break;
            case 13:
                pm.PlaySound(pm.Female ? 793 : 1065);
                pm.Say("*gasp!*");
                break;
            case 14:
                pm.PlaySound(pm.Female ? 794 : 1066);
                pm.Say("*giggles*");
                break;
            case 15:
                pm.PlaySound(pm.Female ? 795 : 1067);
                pm.Say("*groans*");
                break;
            case 16:
                pm.PlaySound(pm.Female ? 796 : 1068);
                pm.Say("*growls*");
                break;
            case 17:
                pm.PlaySound(pm.Female ? 797 : 1069);
                pm.Say("*hey!*");
                break;
            case 18:
                pm.PlaySound(pm.Female ? 798 : 1070);
                pm.Say("*hiccup!*");
                break;
            case 19:
                pm.PlaySound(pm.Female ? 799 : 1071);
                pm.Say("*huh?*");
                break;
            case 20:
                pm.PlaySound(pm.Female ? 800 : 1072);
                pm.Say("*kisses*");
                break;
            case 21:
                pm.PlaySound(pm.Female ? 801 : 1073);
                pm.Say("*laughs*");
                break;
            case 22:
                pm.PlaySound(pm.Female ? 802 : 1074);
                pm.Say("*no!*");
                break;
            case 23:
                pm.PlaySound(pm.Female ? 803 : 1075);
                pm.Say("*oh!*");
                break;
            case 24:
                pm.PlaySound(pm.Female ? 811 : 1085);
                pm.Say("*oooh*");
                break;
            case 25:
                pm.PlaySound(pm.Female ? 812 : 1086);
                pm.Say("*oops*");
                break;
            case 26:
                {
                    pm.PlaySound(pm.Female ? 813 : 1087);
                    pm.Say("*pukes*");
                    if (!pm.Mounted)
                    {
                        pm.Animate(32, 5, 1, true, false, 0);
                    }

                    new Puke().MoveToWorld(pm.Location, pm.Map);
                    break;
                }
            case 27:
                pm.PlaySound(315);
                pm.Say("*punches*");
                if (!pm.Mounted)
                {
                    pm.Animate(31, 5, 1, true, false, 0);
                }

                break;
            case 28:
                pm.PlaySound(pm.Female ? 814 : 1088);
                pm.Say("*ahhhh!*");
                break;
            case 29:
                pm.PlaySound(pm.Female ? 815 : 1089);
                pm.Say("*shhh!*");
                break;
            case 30:
                pm.PlaySound(pm.Female ? 816 : 1090);
                pm.Say("*sigh*");
                break;
            case 31:
                pm.PlaySound(948);
                pm.Say("*slaps*");
                if (!pm.Mounted)
                {
                    pm.Animate(11, 5, 1, true, false, 0);
                }

                break;
            case 32:
                pm.PlaySound(pm.Female ? 817 : 1091);
                pm.Say("*ahh-choo!*");
                if (!pm.Mounted)
                {
                    pm.Animate(32, 5, 1, true, false, 0);
                }

                break;
            case 33:
                pm.PlaySound(pm.Female ? 818 : 1092);
                pm.Say("*sniff*");
                if (!pm.Mounted)
                {
                    pm.Animate(34, 5, 1, true, false, 0);
                }

                break;
            case 34:
                pm.PlaySound(pm.Female ? 819 : 1093);
                pm.Say("*snore*");
                break;
            case 35:
                pm.PlaySound(pm.Female ? 820 : 1094);
                pm.Say("*spits*");
                if (!pm.Mounted)
                {
                    pm.Animate(6, 5, 1, true, false, 0);
                }

                break;
            case 36:
                pm.PlaySound(792);
                pm.Say("*sticks out tongue*");
                break;
            case 37:
                pm.PlaySound(874);
                pm.Say("*taps foot*");
                if (!pm.Mounted)
                {
                    pm.Animate(38, 5, 1, true, false, 0);
                }

                break;
            case 38:
                pm.PlaySound(pm.Female ? 821 : 1095);
                pm.Say("*wistles*");
                if (!pm.Mounted)
                {
                    pm.Animate(5, 5, 1, true, false, 0);
                }

                break;
            case 39:
                pm.PlaySound(pm.Female ? 783 : 1054);
                pm.Say("*woohoo!*");
                break;
            case 40:
                pm.PlaySound(pm.Female ? 822 : 1096);
                pm.Say("*yawns*");
                if (!pm.Mounted)
                {
                    pm.Animate(17, 5, 1, true, false, 0);
                }

                break;
            case 41:
                pm.PlaySound(pm.Female ? 823 : 1097);
                pm.Say("*yea!*");
                break;
            case 42:
                pm.PlaySound(pm.Female ? 823 : 1098);
                pm.Say("*yells*");
                break;
        }
    }
}
