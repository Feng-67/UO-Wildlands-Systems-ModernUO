/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author Felladrin)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Converted to ModernUO
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace Felladrin.Engines
{
    public static class GlobalChat
    {
        public static class Config
        {
            public static bool Enabled = true;
            public static bool OpenHistoryOnLogin = true;
            public static bool AutoColoredNames = true;
            public static int HistorySize = 50;
            public static int MessageHue = 0x481;
        }

        public static unsafe void Initialize()
        {
            if (Config.Enabled)
            {
                CommandSystem.Register("ChatToggle", AccessLevel.Player, OnCommandToggle);
                CommandSystem.Register("ChatHistory", AccessLevel.Player, OnCommandHistory);
                CommandSystem.Register("C", AccessLevel.Player, OnCommandChat);
                
            }
        }
                
        static readonly List<string> History = new List<string>();
        static readonly HashSet<uint> DisabledPlayers = new HashSet<uint>();

        [Usage("ChatToggle")]
        [Description("Enables or Disables the Chat.")]
        static void OnCommandToggle(CommandEventArgs e)
        {
            if (e.Mobile is not PlayerMobile pm)
                return;

            var acc = pm.Account as Account;

            if (acc == null)
                return;

            if (acc.GetTag("Chat") == null || acc.GetTag("Chat") == "Enabled")
            {
                DisabledPlayers.Add(pm.Serial.Value);
                acc.SetTag("Chat", "Disabled");
                pm.SendMessage(38, "You have disabled the chat for your account.");

                pm.CloseGump<ChatHistoryGump>();
            }
            else
            {
                DisabledPlayers.Remove(pm.Serial.Value);
                acc.SetTag("Chat", "Enabled");
                pm.SendMessage(68, "You have enabled the chat for your account.");
                pm.SendGump(new ChatHistoryGump());
            }
        }

        [Usage("ChatHistory")]
        [Description("Opens the Chat History.")]
        static void OnCommandHistory(CommandEventArgs e)
        {
            if (e.Mobile is not PlayerMobile pm)
                return;

            if (DisabledPlayers.Contains(pm.Serial.Value))
            {
                pm.SendMessage(38, "Chat is currently disabled for your account. Type [ChatToggle to enable it.");
                return;
            }

            pm.CloseGump<ChatHistoryGump>();
            pm.SendGump(new ChatHistoryGump());
        }

        [Usage("C <message>")]
        [Description("Broadcasts a message to all players online. If no message is provided, it opens the Chat History.")]
        static void OnCommandChat(CommandEventArgs e)
        {
            if (e.Mobile is not PlayerMobile pm)
                return;

            if (DisabledPlayers.Contains(pm.Serial.Value))
            {
                pm.SendMessage(38, "Chat is currently disabled for your account. Type [ChatToggle to enable it.");
                return;
            }

            if (e.ArgString.Length == 0)
            {
                pm.CloseGump<ChatHistoryGump>();
                pm.SendGump(new ChatHistoryGump());
            }
            else
            {
                Broadcast(e.Mobile, e.ArgString);
            }
        }

        public static void HandleLogin(PlayerMobile pm)
        {
            if (pm?.Account is not Account acc)
                return;

            if (acc.GetTag("Chat") == "Disabled")
            {
                DisabledPlayers.Add(pm.Serial.Value);
                pm.SendMessage("Chat is Disabled for your account.");
            }
            else
            {
                pm.SendMessage("Chat is Enabled for your account.");

                if (Config.OpenHistoryOnLogin)
                    pm.SendGump(new ChatHistoryGump());
            }
        }

        static void Broadcast(Mobile sender, string message)
        {
            if (History.Count > Config.HistorySize)
                History.RemoveAt(0);

            string nameColor = (sender.AccessLevel > AccessLevel.Player) ? "FFD700" : "FFFFFF";
            string nameLabel = (sender.AccessLevel > AccessLevel.Player) ? "[Admin] " + sender.Name : sender.Name;

            History.Add(string.Format("[{0}] <basefont color=#{1}>{2}: <basefont color=#FFFFFF>{3}",
                DateTime.UtcNow.ToString("HH:mm"),
                nameColor,
                nameLabel,
                Utility.FixHtml(message)));

            foreach (NetState ns in NetState.Instances)
            {
                if (ns.Mobile is not PlayerMobile player)
                    continue;

                if (DisabledPlayers.Contains(player.Serial.Value))
                    continue;

                string fullMessage = $"{sender.RawName}: {message}";
                player.SendMessage(Config.MessageHue, fullMessage);

                if (player.HasGump<ChatHistoryGump>())
                {
                    player.CloseGump<ChatHistoryGump>();
                    player.SendGump(new ChatHistoryGump());
                }
            }
        }

        static string GenerateHistoryHTML()
        {
            if (History.Count == 0)
                return "No messages were sent since the last restart.";

            string HTML = "";

            foreach (string msg in History)
                HTML = msg + " <br/>" + HTML;

            return HTML;
        }

        public class ChatHistoryGump : Gump
        {
            public ChatHistoryGump() : base(110, 100)
            {
                Closable = true;
                Draggable = true;
                Disposable = true;
                Resizable = false;

                AddPage(0);
                AddBackground(0, 0, 420, 250, 5054);
                AddImageTiled(10, 10, 400, 20, 2624);
                AddAlphaRegion(10, 10, 400, 20);
                AddLabel(15, 10, 0x481, "TYPE [c then your message TO CHAT");
                AddImageTiled(10, 40, 400, 200, 2624);
                AddAlphaRegion(10, 40, 400, 200);
                AddHtml(15, 40, 395, 200, GenerateHistoryHTML(), false, true);
            }
        }
    }
}
