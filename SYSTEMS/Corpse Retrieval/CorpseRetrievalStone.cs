/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author Rutibex)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using System.Collections.Generic;
using Server.Collections;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    public static class ResurrectHelper
    {
        public static int ResurrectRange = 3;

        public static void Initialize()
        {
            EventSink.Connected += OnConnected;
        }

        private static void OnConnected(Mobile m)
        {
            if (m == null || m.Deleted || m.Alive)
            {
                return;
            }

            m.CloseGump<DeathTeleportGump>();
            m.SendGump(new DeathTeleportGump(m));
        }

        public static void Resurrect(Mobile m, Item item)
        {
            if (m.Alive)
            {
                return;
            }

            if (!m.InRange(item.GetWorldLocation(), ResurrectRange))
            {
                m.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (m.Map != null && m.Map.CanFit(m.Location, 16, false, false))
            {
                m.CloseGump<ResurrectGump>();
                m.SendGump(new ResurrectGump(m, ResurrectMessage.Healer));
            }
            else
            {
                m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
            }
        }

        public static Item FindStone()
        {
            foreach (var item in World.Items.Values)
            {
                if (item is CorpseRetrievalStoneWest || item is CorpseRetrievalStoneNorth)
                {
                    return item;
                }
            }
            return null;
        }

        public static void DeleteGoldRecursively(Item item)
        {
            if (item == null || item.Deleted)
            {
                return;
            }

            if (item is Gold)
            {
                item.Delete();
                return;
            }

            if (item is Container c)
            {
                foreach (var child in new List<Item>(c.Items))
                {
                    DeleteGoldRecursively(child);
                }
            }
        }

        public class ResurrectEntry : ContextMenuEntry
        {
            public ResurrectEntry() : base(6195, ResurrectRange)
            {
            }

            public override void OnClick(Mobile from, IEntity target)
            {
                if (target is Item item && !from.Alive)
                {
                    Resurrect(from, item);
                }
            }
        }
    }
}

namespace Server.Items
{
    public class CorpseRetrievalStoneWest : Item
    {
        private double _totalWeight;
        private int _totalCost;
        private List<Item> _items;
        private List<Corpse> _corpses;
        private InternalItem _internal;

        [Constructible]
        public CorpseRetrievalStoneWest() : this(false)
        {
        }

        [Constructible]
        public CorpseRetrievalStoneWest(bool bloodied) : base(bloodied ? 0x1D98 : 0x3)
        {
            Name = "a corpse retrieval stone";
            Movable = false;
            _internal = new InternalItem(bloodied, this);
        }

        public override void GetContextMenuEntries(Mobile from, ref PooledRefList<ContextMenuEntry> list)
        {
            if (!from.Alive)
            {
                list.Add(new ResurrectHelper.ResurrectEntry());
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (Parent == null && Utility.InRange(Location, m.Location, ResurrectHelper.ResurrectRange)
                && !Utility.InRange(Location, oldLocation, ResurrectHelper.ResurrectRange))
            {
                ResurrectHelper.Resurrect(m, this);
            }
        }

        public override void OnSingleClick(Mobile m)
        {
            ResurrectHelper.Resurrect(m, this);
        }

        public override void OnDoubleClickDead(Mobile m)
        {
            ResurrectHelper.Resurrect(m, this);
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.CloseGump<CorpseRetrievalStoneGump>();

            if (Utility.InRange(Location, from.Location, ResurrectHelper.ResurrectRange))
            {
                if (GetItems(from))
                {
                    from.SendGump(new CorpseRetrievalStoneGump(from, _items, _corpses, _totalCost));
                }
                else
                {
                    from.SendLocalizedMessage(1080107); // I'm sorry, I have nothing for you at this time.
                }
            }
            else
            {
                from.SendLocalizedMessage(502138); // That is too far away for you to use
            }
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (_internal != null)
            {
                _internal.Location = new Point3D(X, Y + 1, Z);
            }
        }

        public override void OnMapChange()
        {
            if (_internal != null)
            {
                _internal.Map = Map;
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            _internal?.Delete();
        }

        private bool GetItems(Mobile from)
        {
            var worldItems = new List<Item>(World.Items.Values);
            var foundCorpses = new List<Corpse>();

            foreach (var i in worldItems)
            {
                if (i is Corpse corpse && corpse.Owner == from && !i.Deleted)
                {
                    foundCorpses.Add(corpse);
                }
            }

            _corpses = foundCorpses;
            _items = new List<Item>();
            _totalWeight = 0.0;

            if (_corpses.Count > 0)
            {
                foreach (var corpse in _corpses)
                {
                    // Build the preview list shown in the gump. The corpse
                    // itself (not these individual items) is what actually
                    // gets moved to the player on confirmation, so equipped
                    // items and backpack positions are restored correctly.
                    //
                    // Gold is skipped here as a preview only. The actual
                    // deletion happens in CorpseRetrievalStoneGump.OnResponse
                    // on the CONTINUE button, so cancelling is side-effect-free.
                    foreach (var item in corpse.Items)
                    {
                        if (item != null && !item.Deleted && item is not Gold)
                        {
                            _items.Add(item);
                            _totalWeight += item.Weight;
                        }
                    }
                }
            }

            _totalCost = 15000;

            return _items.Count > 0;
        }

        public CorpseRetrievalStoneWest(Serial serial) : base(serial)
        {
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(_internal);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();

            _internal = reader.ReadEntity<Item>() as InternalItem;
        }

        private class InternalItem : Item
        {
            private CorpseRetrievalStoneWest _stone;

            public InternalItem(bool bloodied, CorpseRetrievalStoneWest item) : base(bloodied ? 0x1D97 : 0x2)
            {
                Name = "a corpse retrieval stone";
                Movable = false;
                _stone = item;
            }

            public override void GetContextMenuEntries(Mobile from, ref PooledRefList<ContextMenuEntry> list)
            {
                _stone?.GetContextMenuEntries(from, ref list);
            }

            public override bool HandlesOnMovement => true;

            public override void OnMovement(Mobile m, Point3D oldLocation)
            {
                _stone?.OnMovement(m, oldLocation);
            }

            public override void OnSingleClick(Mobile m)
            {
                _stone?.OnSingleClick(m);
            }

            public override void OnDoubleClickDead(Mobile m)
            {
                _stone?.OnDoubleClickDead(m);
            }

            public override void OnDoubleClick(Mobile from)
            {
                _stone?.OnDoubleClick(from);
            }

            public override void OnLocationChange(Point3D oldLocation)
            {
                if (_stone != null)
                {
                    _stone.Location = new Point3D(X, Y - 1, Z);
                }
            }

            public override void OnMapChange()
            {
                if (_stone != null)
                {
                    _stone.Map = Map;
                }
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                _stone?.Delete();
            }

            public InternalItem(Serial serial) : base(serial)
            {
            }

            public override void Serialize(IGenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write(0); // version

                writer.Write(_stone);
            }

            public override void Deserialize(IGenericReader reader)
            {
                base.Deserialize(reader);

                var version = reader.ReadInt();

                _stone = reader.ReadEntity<Item>() as CorpseRetrievalStoneWest;
            }
        }
    }

    [TypeAlias("Server.Items.CorpseRetrievalStoneEast")]
    public class CorpseRetrievalStoneNorth : Item
    {
        private double _totalWeight;
        private int _totalCost;
        private List<Item> _items;
        private List<Corpse> _corpses;
        private InternalItem _internal;

        [Constructible]
        public CorpseRetrievalStoneNorth() : this(false)
        {
        }

        [Constructible]
        public CorpseRetrievalStoneNorth(bool bloodied) : base(bloodied ? 0x1E5D : 0x4)
        {
            Name = "a corpse retrieval stone";
            Movable = false;
            _internal = new InternalItem(bloodied, this);
        }

        public override void GetContextMenuEntries(Mobile from, ref PooledRefList<ContextMenuEntry> list)
        {
            if (!from.Alive)
            {
                list.Add(new ResurrectHelper.ResurrectEntry());
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (Parent == null && Utility.InRange(Location, m.Location, ResurrectHelper.ResurrectRange)
                && !Utility.InRange(Location, oldLocation, ResurrectHelper.ResurrectRange))
            {
                ResurrectHelper.Resurrect(m, this);
            }
        }

        public override void OnSingleClick(Mobile m)
        {
            ResurrectHelper.Resurrect(m, this);
        }

        public override void OnDoubleClickDead(Mobile m)
        {
            ResurrectHelper.Resurrect(m, this);
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.CloseGump<CorpseRetrievalStoneGump>();

            if (Utility.InRange(Location, from.Location, ResurrectHelper.ResurrectRange))
            {
                if (GetItems(from))
                {
                    from.SendGump(new CorpseRetrievalStoneGump(from, _items, _corpses, _totalCost));
                }
                else
                {
                    from.SendLocalizedMessage(1080107); // I'm sorry, I have nothing for you at this time.
                }
            }
            else
            {
                from.SendLocalizedMessage(502138); // That is too far away for you to use
            }
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (_internal != null)
            {
                _internal.Location = new Point3D(X + 1, Y, Z);
            }
        }

        public override void OnMapChange()
        {
            if (_internal != null)
            {
                _internal.Map = Map;
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            _internal?.Delete();
        }

        private bool GetItems(Mobile from)
        {
            var worldItems = new List<Item>(World.Items.Values);
            var foundCorpses = new List<Corpse>();

            foreach (var i in worldItems)
            {
                if (i is Corpse corpse && corpse.Owner == from && !i.Deleted)
                {
                    foundCorpses.Add(corpse);
                }
            }

            _corpses = foundCorpses;
            _items = new List<Item>();
            _totalWeight = 0.0;

            if (_corpses.Count > 0)
            {
                foreach (var corpse in _corpses)
                {
                    // Preview-only list. Gold is skipped here so the gump
                    // matches what CONTINUE will actually do. The real
                    // deletion is deferred to CorpseRetrievalStoneGump.OnResponse.
                    foreach (var item in corpse.Items)
                    {
                        if (item != null && !item.Deleted && item is not Gold)
                        {
                            _items.Add(item);
                            _totalWeight += item.Weight;
                        }
                    }
                }
            }

            _totalCost = 15000;

            return _items.Count > 0;
        }

        public CorpseRetrievalStoneNorth(Serial serial) : base(serial)
        {
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(_internal);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();

            _internal = reader.ReadEntity<Item>() as InternalItem;
        }

        [TypeAlias("Server.Items.CorpseRetrievalStoneNorth+InternalItem")]
        private class InternalItem : Item
        {
            private CorpseRetrievalStoneNorth _stone;

            public InternalItem(bool bloodied, CorpseRetrievalStoneNorth item) : base(bloodied ? 0x1E5C : 0x5)
            {
                Name = "a corpse retrieval stone";
                Movable = false;
                _stone = item;
            }

            public override void GetContextMenuEntries(Mobile from, ref PooledRefList<ContextMenuEntry> list)
            {
                _stone?.GetContextMenuEntries(from, ref list);
            }

            public override bool HandlesOnMovement => true;

            public override void OnMovement(Mobile m, Point3D oldLocation)
            {
                _stone?.OnMovement(m, oldLocation);
            }

            public override void OnSingleClick(Mobile m)
            {
                _stone?.OnSingleClick(m);
            }

            public override void OnDoubleClickDead(Mobile m)
            {
                _stone?.OnDoubleClickDead(m);
            }

            public override void OnDoubleClick(Mobile from)
            {
                _stone?.OnDoubleClick(from);
            }

            public override void OnLocationChange(Point3D oldLocation)
            {
                if (_stone != null)
                {
                    _stone.Location = new Point3D(X - 1, Y, Z);
                }
            }

            public override void OnMapChange()
            {
                if (_stone != null)
                {
                    _stone.Map = Map;
                }
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                _stone?.Delete();
            }

            public InternalItem(Serial serial) : base(serial)
            {
            }

            public override void Serialize(IGenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write(0); // version

                writer.Write(_stone);
            }

            public override void Deserialize(IGenericReader reader)
            {
                base.Deserialize(reader);

                var version = reader.ReadInt();

                _stone = reader.ReadEntity<Item>() as CorpseRetrievalStoneNorth;
            }
        }
    }
}

namespace Server.Gumps
{
    public class CorpseRetrievalStoneGump : Gump
    {
        private readonly Mobile _from;
        private readonly List<Item> _items;
        private readonly List<Corpse> _corpses;
        private readonly int _totalCost;

        public CorpseRetrievalStoneGump(Mobile from, List<Item> items, List<Corpse> corpses, int totalCost) : base(0, 0)
        {
            _from = from;
            _items = items;
            _corpses = corpses;
            _totalCost = totalCost;

            const int width = 400;
            const int height = 300;

            AddPage(0);

            // --- Same background stack as DeathTeleportGump / MyCommandsGump ---
            AddBackground(0, 0, width, height, 9200);
            AddImageTiled(10, 10, width - 20, height - 20, 2624);
            AddAlphaRegion(10, 10, width - 20, height - 20);

            // --- HEADER: matching gold, matching divider ---
            AddHtml(0, 20, width, 25,
                "<BASEFONT COLOR=#FFCC00><CENTER>CORPSE RETRIEVAL STONE</CENTER></BASEFONT>",
                false, false);

            AddImageTiled(20, 50, width - 40, 2, 9264);

            // --- Skull and crossbones centered under the divider ---
            AddImage(164, 62, 9804);

            // --- Body text: white, spaced, no scrollbar ---
            AddHtml(30, 125, width - 60, 120,
                "<BASEFONT COLOR=#FFFFFF><CENTER>" + string.Format(
                    "It is possible for your corpse items to be retrieved.<br><br>" +
                    "It will cost you {0} Gold.<br><br>" +
                    "CONTINUE - Bring my items now.<br>" +
                    "The GOLD I carried will be DESTROYED.<br><br>" +
                    "CANCEL - I will collect my own stuff!!", _totalCost) + "</CENTER></BASEFONT>",
                false, false);

            // --- Buttons in the same style as the rez gump ---
            AddButton(105, 265, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(140, 268, 2036, "CONTINUE");

            AddButton(220, 265, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddLabel(250, 268, 2036, "CANCEL");
        }

        private static void RestoreCorpseItems(Mobile from, Corpse corpse)
        {
            if (corpse == null || corpse.Deleted || from == null || from.Deleted)
            {
                return;
            }

            corpse.Open(from, true);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            // CANCEL (0) is now fully side-effect-free: no payment attempted,
            // no gold deleted, no corpse moved.
            if (info.ButtonID == 0)
            {
                return;
            }

            if (info.ButtonID == 1)
            {
                if (!MakePayment(_from, _totalCost))
                {
                    return;
                }

                // Payment succeeded. NOW destroy the corpse gold, including
                // gold nested inside bags on the corpse. This used to happen
                // in CorpseRetrievalStone*.GetItems() when the gump opened,
                // which meant CANCEL couldn't undo it.
                foreach (var corpse in _corpses)
                {
                    if (corpse == null || corpse.Deleted)
                    {
                        continue;
                    }

                    foreach (var item in new List<Item>(corpse.Items))
                    {
                        ResurrectHelper.DeleteGoldRecursively(item);
                    }
                }

                // Bring each corpse to the player rather than copying its
                // contents into the backpack. Corpse.Open() is the same
                // self-loot routine the client triggers when a player
                // double clicks their own corpse - it re-equips worn
                // items to the paperdoll and restores backpack items to
                // their original positions, instead of dumping
                // everything loose into the top of the backpack.
                foreach (var corpse in _corpses)
                {
                    if (corpse != null && !corpse.Deleted)
                    {
                        corpse.MoveToWorld(_from.Location, _from.Map);
                        RestoreCorpseItems(_from, corpse);
                    }
                }

                _from.SendLocalizedMessage(1062471); // You quickly gather all of your belongings.
            }
        }

        public static bool MakePayment(Mobile buyer, int totalCost, BaseVendor from = null)
        {
            Container cont;

            var bought = false;
            var fromBank = false;

            cont = buyer.Backpack;
            bought = buyer.AccessLevel >= AccessLevel.GameMaster;

            if (!bought && cont != null && cont.ConsumeTotal(typeof(Gold), totalCost))
            {
                bought = true;
            }

            if (!bought)
            {
                if (totalCost <= int.MaxValue)
                {
                    if (Banker.Withdraw(buyer, totalCost))
                    {
                        bought = true;
                        fromBank = true;
                    }
                }
                // TODO: ModernUO does not expose a ServUO-style AccountGold currency
                // withdrawal path. Banker.Withdraw covers the account-wide case
                // because your shard banks are already account-linked. If you
                // later add a parallel platinum-style account currency, hook it
                // in here before the bank-fallback block below.
            }

            if (!bought)
            {
                cont = buyer.FindBankNoCreate();

                if (cont != null && cont.ConsumeTotal(typeof(Gold), totalCost))
                {
                    bought = true;
                    fromBank = true;
                }
            }

            if (from != null)
            {
                if (bought)
                {
                    buyer.PlaySound(0x32); // coins

                    if (buyer.AccessLevel >= AccessLevel.GameMaster)
                    {
                        from.SayTo(buyer, true, "I would not presume to charge thee anything.  Here is the repair you requested.");
                    }
                    else if (fromBank)
                    {
                        from.SayTo(buyer, 1151638, totalCost.ToString());
                    }
                    else
                    {
                        from.SayTo(buyer, 1151639, totalCost.ToString());
                    }

                    return true;
                }

                buyer.SayTo(buyer, totalCost >= 2000 ? 500191 : 500192, 0x3B2);
            }
            else
            {
                if (bought)
                {
                    buyer.PlaySound(0x32); // coins

                    if (buyer.AccessLevel >= AccessLevel.GameMaster)
                    {
                        buyer.SendMessage("You have not been charged for this service.");
                    }
                    else if (fromBank)
                    {
                        buyer.SendMessage($"{totalCost} gold has been drawn from your bank account.");
                    }
                    else
                    {
                        buyer.SendMessage($"{totalCost} gold has been taken for the service.");
                    }

                    return true;
                }

                buyer.SendMessage("You lack sufficient funds for this service.");
            }

            return false;
        }
    }
}
