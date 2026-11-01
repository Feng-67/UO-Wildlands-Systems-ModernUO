using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace YourGameNamespace.Services
{
    public class GoldDistribution
    {
        // =============================================
        // CONFIGURATION SERVICE
        // =============================================
        public static class Config
        {
            public static bool Enabled { get; set; } = true;
            public static TimeSpan DistributionInterval { get; set; } = TimeSpan.FromHours(1);
            public static int GoldAmountPerInterval { get; set; } = 1500;
            public static AccessLevel MaxAccessLevel { get; set; } = AccessLevel.Player;
        }

        // =============================================
        // INITIALIZATION
        // =============================================
        public static void Initialize()
        {
            if (Config.Enabled)
            {
                Console.WriteLine($"Gold distribution enabled. Distributing {Config.GoldAmountPerInterval} gold every {Config.DistributionInterval.TotalHours} hour(s).");
                Timer.DelayCall(TimeSpan.Zero, Config.DistributionInterval, ExecuteAutoDistribution);
            }
        }

        // =============================================
        // DISTRIBUTION LOGIC
        // =============================================
        private static void ExecuteAutoDistribution()
        {
            if (!Config.Enabled) return;

            foreach (NetState state in NetState.Instances)
            {
                if (state.Mobile is PlayerMobile player && player.AccessLevel <= Config.MaxAccessLevel)
                {
                    GiveGoldToPlayer(player);
                }
            }
        }

        private static void GiveGoldToPlayer(PlayerMobile player)
        {
            BankBox bank = player.BankBox;

            if (bank != null)
            {
                bank.DropItem(new Gold(Config.GoldAmountPerInterval));
                player.SendMessage($"You have received {Config.GoldAmountPerInterval} gold in your bank box.");
                Console.WriteLine($"Deposited {Config.GoldAmountPerInterval} gold to {player.Name}'s bank box.");
            }
        }
    }
}
