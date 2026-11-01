/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author Unknown)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */

using Server.Accounting;
using Server.Commands;
using Server.Network;

namespace Server.Gumps;

public class PasswordChanger : StaticGump<PasswordChanger>
{
    public override bool Singleton => true;

    private PasswordChanger() : base(0, 0)
    {
    }

    public static void Configure()
    {
        CommandSystem.Register("changepassword", AccessLevel.Player, Password_OnCommand);
    }

    public static void Password_OnCommand(CommandEventArgs e)
    {
        DisplayTo(e.Mobile);
    }

    public static void DisplayTo(Mobile from)
    {
        from.SendGump(new PasswordChanger());
    }

    protected override void BuildLayout(ref StaticGumpBuilder builder)
    {
        builder.SetNoMove();
        builder.SetNoResize();

        builder.AddPage();

        builder.AddBackground(133, 77, 414, 206, 3600);

        builder.AddLabel(279, 97, 1153, "Password Changer");
        builder.AddLabel(160, 130, 1153, "Current Password:");
        builder.AddLabel(160, 160, 1153, "New Password:");
        builder.AddLabel(160, 190, 1153, "Confirm New Password:");

        builder.AddButton(306, 230, 247, 248, 1);

        builder.AddAlphaRegion(321, 190, 200, 20);
        builder.AddAlphaRegion(321, 160, 200, 20);
        builder.AddAlphaRegion(321, 130, 200, 20);

        builder.AddTextEntry(321, 130, 200, 20, 1153, 1, "");
        builder.AddTextEntry(321, 160, 200, 20, 1153, 2, "");
        builder.AddTextEntry(321, 190, 200, 20, 1153, 3, "");
    }

    private static string GetString(in RelayInfo info, int id) => info.GetTextEntry(id)?.Trim() ?? "";

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var m = sender.Mobile;

        switch (info.ButtonID)
        {
            case 0:
                {
                    m.SendMessage(1300, "Your password has not changed.");
                    return;
                }
            case 1:
                {
                    var origPass = GetString(info, 1);
                    var newPass = GetString(info, 2);
                    var confirmNewPass = GetString(info, 3);

                    if (newPass != confirmNewPass) // Two "New password" fields do not match
                    {
                        m.SendMessage(37, "The 'Confirm New Password' value does not match the 'New Password'. Remember it is cAsE sEnSaTiVe. ");
                        DisplayTo(m);
                        return;
                    }

                    foreach (var c in newPass)
                    {
                        if (!char.IsLetterOrDigit(c)) // Char is NOT a letter or digit
                        {
                            m.SendMessage(37, "Passwords may only consist of letters (A - Z) and Digits (0 - 9).");
                            return;
                        }
                    }

                    if (m.Account is not Account a)
                    {
                        return;
                    }

                    if (!a.CheckPassword(origPass)) // "Current Password" value is incorrect
                    {
                        m.SendMessage(37, $"The 'Current Password' value is incorrect. [ {origPass} ].");
                        DisplayTo(m);
                        return;
                    }

                    // Current password is correct, and 2 "New Password" fields match.
                    a.SetPassword(newPass);
                    m.SendMessage(68, $"Your account ( {a.Username} ) password has been changed to '{newPass}'. Remember this!");
                    break;
                }
        }
    }
}
