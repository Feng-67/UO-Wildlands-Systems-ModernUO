/*
 * UO Wildlands Custom Store — UltimaStore-Style UI
 * Built for ModernUO. Uses gold. Self-contained.
 * Licensed under GPL-3.0
 */
using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace Server.Custom.Store
{
    // ============================================================
    //  CONFIG
    // ============================================================
    public static class StoreConfig
    {
        public static bool Enabled = true;
        public static string CurrencyName = "gp";
        public static int CartCapacity = 10;
        public static int PageSize = 6;
    }

    // ============================================================
    //  CATALOG ENTRY
    // ============================================================
    public class StoreItem
    {
        public Type ItemType { get; }
        public string Name { get; }
        public string Category { get; }
        public int ItemID { get; }
        public int Hue { get; }
        public int Price { get; }
        private readonly Func<Mobile, StoreItem, Item> m_Ctor;

        public StoreItem(Type type, string name, string category, int itemID, int hue, int price, Func<Mobile, StoreItem, Item> ctor = null)
        {
            ItemType = type;
            Name = name;
            Category = category;
            ItemID = itemID;
            Hue = hue;
            Price = price;
            m_Ctor = ctor;
        }

        public Item Construct(Mobile m)
        {
            if (m_Ctor != null) return m_Ctor(m, this);
            try { return Activator.CreateInstance(ItemType) as Item; }
            catch { return null; }
        }
    }

    // ============================================================
    //  PER-PLAYER PROFILE
    // ============================================================
    public class StoreProfile
    {
        public Mobile Player { get; }
        public string Category { get; set; } = "Featured";
        public Dictionary<StoreItem, int> Cart { get; } = new Dictionary<StoreItem, int>();

        public StoreProfile(Mobile m) { Player = m; }

        public int SubTotal()
        {
            int total = 0;
            foreach (var kvp in Cart) total += kvp.Key.Price * kvp.Value;
            return total;
        }
    }

    // ============================================================
    //  MAIN SYSTEM
    // ============================================================
    public static class StoreSystem
    {
        public static List<StoreItem> Catalog { get; } = new List<StoreItem>();
        public static Dictionary<Mobile, StoreProfile> Profiles { get; } = new Dictionary<Mobile, StoreProfile>();
        public static List<Item> Pending { get; } = new List<Item>();

        public static void Configure()
        {
            // Persistence disabled by default. Uncomment to enable:
            // GenericPersistence.Register("CustomStore", Serialize, Deserialize);
        }

        public static unsafe void Initialize()
        {
            if (!StoreConfig.Enabled) return;

            CommandSystem.Register("Store", AccessLevel.Player, e =>
            {
                if (e.Mobile is PlayerMobile pm) Open(pm);
            });

            RegisterCatalog();

            // Hook the client's "UO Store" menu bar button.
            // Packet 0xFA, length 1, in-game only, handler is a function pointer.
            IncomingPackets.Register(0xFA, 1, true, &OnUOStoreRequest);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Custom Store: Loading...");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"done ({Catalog.Count} items)");
            Console.ResetColor();
        }

        // Packet handler — must be static, non-generic, and match
        // the delegate* signature (NetState, SpanReader) -> void.
        private static void OnUOStoreRequest(NetState state, SpanReader reader)
        {
            if (state.Mobile is PlayerMobile pm)
            {
                Open(pm);
            }
        }

        private static void RegisterCatalog()
        {
            // ============================================================
            //  FEATURED — items requiring the 7th parameter (multiples,
            //  bundles, or variations) plus the 1,000,000 gp showcase item.
            //  Featured spans 3 pages (14 items @ 6 per page).
            // ============================================================

            // -------- Multiples (7th param required) --------
            Register(typeof(Bandage), "100 Bandages", "Featured", 0xE21, 0, 500,
                (m, e) => new Bandage(100));

            Register(typeof(BlackPearl), "100 Black Pearl", "Featured", 0xF7A, 0, 300,
                (m, e) => new BlackPearl(100));
            Register(typeof(Bloodmoss), "100 Bloodmoss", "Featured", 0xF7B, 0, 300,
                (m, e) => new Bloodmoss(100));
            Register(typeof(Garlic), "100 Garlic", "Featured", 0xF84, 0, 300,
                (m, e) => new Garlic(100));
            Register(typeof(Ginseng), "100 Ginseng", "Featured", 0xF85, 0, 300,
                (m, e) => new Ginseng(100));
            Register(typeof(MandrakeRoot), "100 Mandrake Root", "Featured", 0xF86, 0, 300,
                (m, e) => new MandrakeRoot(100));
            Register(typeof(Nightshade), "100 Nightshade", "Featured", 0xF88, 0, 300,
                (m, e) => new Nightshade(100));

            Register(typeof(Lockpick), "50 Lockpicks", "Featured", 0x14FC, 0, 350,
                (m, e) => new Lockpick(50));

            // -------- Bundles (7th param + Bag) --------
            Register(typeof(GreaterHealPotion), "10 Greater Heal", "Featured", 0xF0B, 0, 300,
                (m, e) => { var bag = new Bag(); for (int i = 0; i < 10; i++) bag.DropItem(new GreaterHealPotion()); return bag; });

            Register(typeof(GreaterCurePotion), "10 Greater Cure", "Featured", 0xF07, 0, 300,
                (m, e) => { var bag = new Bag(); for (int i = 0; i < 10; i++) bag.DropItem(new GreaterCurePotion()); return bag; });

            Register(typeof(RecallRune), "5 Recall Runes", "Featured", 0x1F14, 0, 250,
                (m, e) => { var bag = new Bag(); for (int i = 0; i < 5; i++) bag.DropItem(new RecallRune()); return bag; });

            // -------- Variations (7th param required) --------
            Register(typeof(Pitcher), "Pitcher of Water", "Featured", 0x1F9D, 0, 10,
                (m, e) => new Pitcher(BeverageType.Water));

            Register(typeof(DyeTub), "Black Dye Tub", "Featured", 0x0FAB, 0, 750,
                (m, e) => new DyeTub { DyedHue = 0x0001 });

            // -------- Showcase — 1,000,000 gp --------
            Register(typeof(Runebook), "Runebook", "Featured", 0x22C5, 0, 1000000);

            // ============================================================
            //  OTHER CATEGORIES — 2 items each, no 7th parameter needed
            // ============================================================
                       

            // -------- Tools --------
            Register(typeof(Scissors), "Scissors", "Tools", 0xF9F, 0, 50);
            Register(typeof(SewingKit), "Sewing Kit", "Tools", 0xF9D, 0, 50);

            // -------- Weapons --------
            Register(typeof(Katana), "Katana", "Weapons", 0x13FF, 0, 100);
            Register(typeof(Longsword), "Longsword", "Weapons", 0x0F61, 0, 150);

            // -------- Armor --------
            Register(typeof(PlateChest), "Plate Chest", "Armor", 0x1415, 0, 500);
            Register(typeof(PlateLegs), "Plate Legs", "Armor", 0x1411, 0, 400);                        

            // -------- Scrolls --------
            Register(typeof(RecallScroll), "Recall Scroll", "Scrolls", 0x1F4C, 0, 50);
            Register(typeof(MarkScroll), "Mark Scroll", "Scrolls", 0x1F5A, 0, 75);

            // -------- Jewelry --------
            Register(typeof(GoldRing), "Gold Ring", "Jewelry", 0x108A, 0, 200);
            Register(typeof(GoldNecklace), "Gold Necklace", "Jewelry", 0x1085, 0, 400);
                        
            // -------- Misc --------
            Register(typeof(Bag), "Bag", "Misc", 0x0E76, 0, 25);
            Register(typeof(Backpack), "Backpack", "Misc", 0x0E75, 0, 50);

            // -------- Dyes --------
            Register(typeof(Dyes), "Dyes", "Dyes", 0x0FA9, 0, 25);
            Register(typeof(DyeTub), "Dye Tub", "Dyes", 0x0FAB, 0, 500);

            // -------- Decorations --------
            Register(typeof(StoneAnkhDeed), "Stone Ankh Deed", "Decorations", 0x14F0, 0, 500);
            Register(typeof(FountainDeed), "Fountain Deed", "Decorations", 0x14F0, 0, 1000);
                        

            // -------- Storage --------
            Register(typeof(WoodenChest), "Wooden Chest", "Storage", 0x0E43, 0, 200);
            Register(typeof(MetalGoldenChest), "Golden Chest", "Storage", 0x0E41, 0, 800);

            
        }

        public static void Register(Type type, string name, string category, int itemID, int hue, int price,
                                    Func<Mobile, StoreItem, Item> ctor = null)
        {
            Catalog.Add(new StoreItem(type, name, category, itemID, hue, price, ctor));
        }

        public static List<string> GetCategories()
        {
            return Catalog.Select(i => i.Category).Distinct().ToList();
        }

        public static void Open(PlayerMobile pm)
        {
            if (pm == null || pm.NetState == null) return;
            pm.CloseGump<StoreGump>();
            pm.SendGump(new StoreGump(pm));
        }

        public static StoreProfile GetProfile(Mobile m, bool create = true)
        {
            if (!Profiles.TryGetValue(m, out var p) && create)
                Profiles[m] = p = new StoreProfile(m);
            return p;
        }

        public static int GetBalance(Mobile m)
        {
            if (m.Account is Account acc) return acc.TotalGold;
            return 0;
        }

        public static bool Withdraw(Mobile m, int amount)
        {
            if (m.Account is not Account acc) return false;
            if (acc.TotalGold < amount) return false;
            return acc.WithdrawGold(amount);
        }

        public static void Grant(Mobile m, StoreItem entry)
        {
            var item = entry.Construct(m);
            if (item == null)
            {
                m.SendMessage($"Failed to construct {entry.Name}. Contact staff.");
                return;
            }

            if (m.Backpack != null && m.Backpack.TryDropItem(m, item, false))
                m.SendMessage($"Your purchase of {entry.Name} has been placed in your backpack.");
            else if (m.BankBox != null && m.BankBox.TryDropItem(m, item, false))
                m.SendMessage($"Your purchase of {entry.Name} has been placed in your bank.");
            else
            {
                Pending.Add(item);
                m.SendMessage($"Your purchase of {entry.Name} will be delivered shortly.");
            }
        }

        public static void DeliverPending(Mobile m)
        {
            if (m.Backpack == null) return;
            for (int i = Pending.Count - 1; i >= 0; i--)
            {
                var item = Pending[i];
                if (item == null || item.Deleted) { Pending.RemoveAt(i); continue; }
                if (m.Backpack.TryDropItem(m, item, false))
                {
                    m.SendMessage("A pending store purchase has been delivered.");
                    Pending.RemoveAt(i);
                }
            }
        }

        public static void Serialize(IGenericWriter writer)
        {
            writer.Write(0); // version

            var live = Profiles.Where(kvp => kvp.Key != null && !kvp.Key.Deleted).ToList();
            writer.Write(live.Count);
            foreach (var kvp in live)
            {
                writer.Write(kvp.Key.Serial);
                writer.Write(kvp.Value.Category ?? "");
                writer.Write(kvp.Value.Cart.Count);
                foreach (var cartEntry in kvp.Value.Cart)
                {
                    writer.Write(Catalog.IndexOf(cartEntry.Key));
                    writer.Write(cartEntry.Value);
                }
            }

            var items = Pending.Where(i => i != null && !i.Deleted).ToList();
            writer.Write(items.Count);
            foreach (var i in items) writer.Write(i.Serial);
        }

        public static void Deserialize(IGenericReader reader)
        {
            reader.ReadInt(); // version

            int profileCount = reader.ReadInt();
            for (int i = 0; i < profileCount; i++)
            {
                Serial mobileSerial = reader.ReadSerial();
                string cat = reader.ReadString();
                int cartCount = reader.ReadInt();

                Mobile m = mobileSerial.IsMobile ? World.FindMobile(mobileSerial) : null;

                if (m != null)
                {
                    var p = GetProfile(m);
                    p.Category = string.IsNullOrEmpty(cat) ? "Featured" : cat;
                    for (int j = 0; j < cartCount; j++)
                    {
                        int idx = reader.ReadInt();
                        int amt = reader.ReadInt();
                        if (idx >= 0 && idx < Catalog.Count) p.Cart[Catalog[idx]] = amt;
                    }
                }
                else
                {
                    for (int j = 0; j < cartCount; j++) { reader.ReadInt(); reader.ReadInt(); }
                }
            }

            int pendingCount = reader.ReadInt();
            for (int i = 0; i < pendingCount; i++)
            {
                Serial itemSerial = reader.ReadSerial();
                if (!itemSerial.IsItem) continue;
                var item = World.FindItem(itemSerial);
                if (item != null) Pending.Add(item);
            }
        }
    }

    // ============================================================
    //  MAIN STORE GUMP
    // ============================================================
    public class StoreGump : Gump
    {
        private readonly PlayerMobile m_User;
        private readonly StoreProfile m_Profile;
        private readonly int m_Page;
        private readonly string m_SearchText;
        private readonly bool m_Searching;

        private const int LabelWhite = 0x481;

        private static readonly int[][] Offsets =
        {
            new[] { 167, 74 },
            new[] { 354, 74 },
            new[] { 541, 74 },
            new[] { 167, 294 },
            new[] { 354, 294 },
            new[] { 541, 294 }
        };

        private const int CartButtonId = 900;
        private const int SearchEntryId = 16;

        public StoreGump(PlayerMobile pm, int page = 0, string searchText = null) : base(100, 60)
        {
            m_User = pm;
            m_Profile = StoreSystem.GetProfile(pm);
            m_Page = page;
            m_SearchText = searchText;
            m_Searching = !string.IsNullOrEmpty(searchText);

            Closable = true;
            Draggable = true;
            Disposable = true;

            Build();
        }

        private void Build()
        {
            AddPage(0);
            AddImage(0, 0, 0x9C49);

            // ---------- Category buttons (left column) ----------
            var cats = StoreSystem.GetCategories();
            int[] yPositions = { 97, 126, 155, 184, 213, 242, 271, 300, 329, 358, 387, 416, 445, 474, 503, 532, 561, 590 };

            for (int i = 0; i < cats.Count && i < yPositions.Length; i++)
            {
                string cat = cats[i];
                bool active = !m_Searching && cat == m_Profile.Category;

                AddButton(36, yPositions[i], active ? 0x9C5F : 0x9C55, 0x9C5F, 100 + i, GumpButtonType.Reply, 0);
                AddHtml(36, yPositions[i] + 3, 125, 25,
                    $"<CENTER><BASEFONT COLOR=#FFFFFF>{cat}</BASEFONT></CENTER>", false, false);
            }

            // ---------- Currency display ----------
            AddLabel(36, 74, LabelWhite, $"   {StoreSystem.GetBalance(m_User):N0} gp");

            // ---------- Cart button ----------
            bool cartActive = !m_Searching && m_Profile.Category == "Cart";
            AddButton(598, 36, cartActive ? 0x9C5E : 0x9C54, 0x9C5E, CartButtonId, GumpButtonType.Reply, 0);
            AddHtml(598, 39, 100, 25,
                $"<CENTER><BASEFONT COLOR=#FFFFFF>    Cart ({m_Profile.Cart.Count})</BASEFONT></CENTER>", false, false);

            // ---------- Search bar ----------
            AddBackground(167, 516, 114, 22, 0x2486);
            AddTextEntry(169, 518, 110, 18, 0, SearchEntryId, m_SearchText ?? "", 169);
            AddButton(286, 516, 0x9C52, 0x9C5C, 114, GumpButtonType.Reply, 0);
            AddHtml(286, 519, 64, 22, "<CENTER><BASEFONT COLOR=#FFFFFF>Search</BASEFONT></CENTER>", false, false);

            // ---------- Body ----------
            if (!m_Searching && m_Profile.Category == "Cart")
                BuildCartView();
            else
                BuildItemView();
        }

        private void BuildItemView()
        {
            var items = GetVisibleItems();

            if (items.Count == 0)
            {
                AddHtml(167, 250, 400, 25,
                    "<CENTER><BASEFONT COLOR=#FFFFFF>No items found.</BASEFONT></CENTER>", false, false);
                return;
            }

            int startIdx = m_Page * 6;
            int pages = Math.Max(1, (int)Math.Ceiling(items.Count / 6.0));

            for (int i = 0; i < 6; i++)
            {
                int idx = startIdx + i;
                if (idx >= items.Count) break;

                var entry = items[idx];
                int x = Offsets[i][0];
                int y = Offsets[i][1];

                AddButton(x, y, 0x9C4B, 0x9C4B, 1000 + i, GumpButtonType.Reply, 0);
                AddHtml(x, y + 4, 183, 25,
                    $"<CENTER><BASEFONT COLOR=#FFFFFF>{entry.Name}</BASEFONT></CENTER>", false, false);

                if (entry.ItemID > 0)
                {
                    var b = ItemBounds.Bounds[entry.ItemID];
                    AddItem((x + 91) - b.Width / 2 - b.X, (y + 108) - b.Height / 2 - b.Y, entry.ItemID, entry.Hue);
                }

                //AddImage(x + 60, y + 192, 0x9C56);
                AddLabelCropped(x + 60, y + 190, 143, 25, LabelWhite, $"{entry.Price:N0} {StoreConfig.CurrencyName}");
            }

            if (m_Page + 1 < pages)
                AddButton(692, 516, 0x9C51, 0x9C5B, 116, GumpButtonType.Reply, 0);
            if (m_Page > 0)
                AddButton(648, 516, 0x9C50, 0x9C5A, 117, GumpButtonType.Reply, 0);
        }

        private void BuildCartView()
        {
            AddImage(167, 74, 0x9C4C);

            var cartKeys = m_Profile.Cart.Keys.ToList();
            for (int i = 0; i < cartKeys.Count; i++)
            {
                var entry = cartKeys[i];
                int amount = m_Profile.Cart[entry];

                AddHtml(175, 84 + (35 * i), 256, 25,
                    $"<BASEFONT COLOR=#FFFFFF>{entry.Name}</BASEFONT>", false, false);

                AddButton(431, 81 + (35 * i), 0x9C52, 0x9C5C, 2000 + i, GumpButtonType.Reply, 0);
                AddLabelCropped(457, 82 + (35 * i), 38, 22, LabelWhite, amount.ToString());
                AddLabelCropped(531, 82 + (35 * i), 100, 14, LabelWhite, $"{entry.Price * amount:N0} {StoreConfig.CurrencyName}");

                AddButton(653, 81 + (35 * i), 0x9C52, 0x9C5C, 3000 + i, GumpButtonType.Reply, 0);
                AddHtml(653, 84 + (35 * i), 64, 22,
                    "<CENTER><BASEFONT COLOR=#FFFFFF>Remove</BASEFONT></CENTER>", false, false);

                AddImage(175, 109 + (35 * i), 0x9C4D);
            }

            AddHtml(508, 482, 125, 25,
                "<CENTER><BASEFONT COLOR=#FFFFFF>Subtotal:</BASEFONT></CENTER>", false, false);
            //AddImage(588, 482, 0x9C56);
            AddLabelCropped(611, 480, 100, 14, LabelWhite, $"{m_Profile.SubTotal():N0} {StoreConfig.CurrencyName}");

            AddButton(653, 516, 0x9C52, 0x9C52, 115, GumpButtonType.Reply, 0);
            AddHtml(653, 519, 64, 22,
                "<CENTER><BASEFONT COLOR=#FFFFFF>Buy</BASEFONT></CENTER>", false, false);
        }

        private List<StoreItem> GetVisibleItems()
        {
            if (m_Searching)
            {
                var results = StoreSystem.Catalog
                    .Where(i => i.Name.IndexOf(m_SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                m_User.SendMessage($"'{m_SearchText}' matched {results.Count} items.");
                return results;
            }

            return StoreSystem.Catalog.Where(i => i.Category == m_Profile.Category).ToList();
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            int id = info.ButtonID;
            if (id == 0) return;

            var cats = StoreSystem.GetCategories();

            if (id >= 100 && id < 100 + cats.Count)
            {
                m_Profile.Category = cats[id - 100];
                m_User.SendGump(new StoreGump(m_User, 0));
                return;
            }

            if (id == CartButtonId)
            {
                m_Profile.Category = "Cart";
                m_User.SendGump(new StoreGump(m_User, 0));
                return;
            }

            if (id == 114)
            {
                string text = info.GetTextEntry(SearchEntryId);
                if (!string.IsNullOrWhiteSpace(text))
                    m_User.SendGump(new StoreGump(m_User, 0, text));
                else
                    m_User.SendMessage(0x35, "Please enter a search term.");
                return;
            }

            if (id == 115) { TryBuyCart(); return; }

            if (id == 116) { m_User.SendGump(new StoreGump(m_User, m_Page + 1, m_SearchText)); return; }
            if (id == 117) { m_User.SendGump(new StoreGump(m_User, m_Page - 1, m_SearchText)); return; }

            if (id >= 1000 && id < 1006)
            {
                var items = GetVisibleItems();
                int idx = m_Page * 6 + (id - 1000);
                if (idx < items.Count)
                    m_User.SendGump(new StoreConfirmGump(m_User, items[idx]));
                return;
            }

            if (id >= 2000 && id < 2006)
            {
                var cartKeys = m_Profile.Cart.Keys.ToList();
                int idx = id - 2000;
                if (idx < cartKeys.Count)
                    m_User.SendGump(new StoreConfirmGump(m_User, cartKeys[idx]));
                return;
            }

            if (id >= 3000 && id < 3006)
            {
                var cartKeys = m_Profile.Cart.Keys.ToList();
                int idx = id - 3000;
                if (idx < cartKeys.Count)
                {
                    m_Profile.Cart.Remove(cartKeys[idx]);
                    m_User.SendGump(new StoreGump(m_User, 0));
                }
                return;
            }
        }

        private void TryBuyCart()
        {
            int total = m_Profile.SubTotal();
            if (total <= 0) { m_User.SendMessage(0x35, "Your cart is empty."); return; }

            if (StoreSystem.GetBalance(m_User) < total)
            {
                m_User.SendMessage(0x35, $"You don't have enough {StoreConfig.CurrencyName}.");
                return;
            }

            if (!StoreSystem.Withdraw(m_User, total))
            {
                m_User.SendMessage(0x35, "Transaction failed.");
                return;
            }

            foreach (var kvp in m_Profile.Cart.ToList())
            {
                for (int i = 0; i < kvp.Value; i++)
                    StoreSystem.Grant(m_User, kvp.Key);
            }

            m_Profile.Cart.Clear();
            m_User.SendGump(new StoreGump(m_User, 0));
        }
    }

    // ============================================================
    //  CONFIRM PURCHASE
    // ============================================================
    public class StoreConfirmGump : Gump
    {
        private readonly PlayerMobile m_User;
        private readonly StoreItem m_Item;

        public StoreConfirmGump(PlayerMobile pm, StoreItem item) : base(200, 200)
        {
            m_User = pm;
            m_Item = item;

            Closable = true;
            Draggable = true;

            Build();
        }

        private void Build()
        {
            AddPage(0);
            AddBackground(0, 0, 410, 200, 0x9C40);

            AddHtml(10, 10, 400, 20,
                "<CENTER><BASEFONT COLOR=#FFFFFF>Purchase Confirmation</BASEFONT></CENTER>", false, false);

            AddItem(200, 90, m_Item.ItemID, m_Item.Hue);

            AddHtml(20, 55, 370, 25,
                $"<CENTER><BASEFONT COLOR=#FFFFFF>{m_Item.Name}</BASEFONT></CENTER>", false, false);

            AddHtml(20, 110, 370, 20,
                $"<CENTER><BASEFONT COLOR=#FFFFFF>Price: {m_Item.Price:N0} {StoreConfig.CurrencyName}</BASEFONT></CENTER>", false, false);

            // Pointer for high-value items
            if (m_Item.Price >= 1000000)
            {
                AddHtml(20, 130, 370, 20,
                    "<CENTER><BASEFONT COLOR=#FFCC00>DISCORD  #store-info  FULL DETAILS</BASEFONT></CENTER>",
                    false, false);
            }

            AddButton(45, 150, 0x9C53, 0x9C5D, 195, GumpButtonType.Reply, 0);
            AddHtml(45, 153, 126, 25,
                "<CENTER><BASEFONT COLOR=#FFFFFF>Add to Cart</BASEFONT></CENTER>", false, false);

            AddButton(240, 150, 0x9C53, 0x9C5D, 196, GumpButtonType.Reply, 0);
            AddHtml(240, 153, 126, 25,
                "<CENTER><BASEFONT COLOR=#FFFFFF>Cancel</BASEFONT></CENTER>", false, false);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 196)
            {
                m_User.SendGump(new StoreGump(m_User));
                return;
            }

            if (info.ButtonID != 195)
                return;

            var profile = StoreSystem.GetProfile(m_User);

            if (profile.Cart.TryGetValue(m_Item, out int cur))
                profile.Cart[m_Item] = cur + 1;
            else
                profile.Cart[m_Item] = 1;

            m_User.SendGump(new StoreGump(m_User));
        }
    }
}
