// UO Wildlands Custom Script
// Adapted for ModernUO from ServUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)

using System;
using System.Collections.Generic;
using System.Linq;
using ModernUO.Serialization;
using Server.Collections;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles.MannequinProperty;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class Mannequin : BaseCreature
{
    private static readonly List<Layer> _sameLayers = new()
    {
        Layer.FirstValid,
        Layer.OneHanded,
        Layer.TwoHanded
    };

    private static readonly Type[] _propertyTypes = BuildPropertyTypeCache();

    [SerializableProperty(0)]
    [CommandProperty(AccessLevel.GameMaster)]
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            InvalidateProperties();
            this.MarkDirty();
        }
    }

    [SerializableProperty(1)]
    public Mobile Owner
    {
        get => _owner;
        set
        {
            _owner = value;
            this.MarkDirty();
        }
    }

    public override bool NoHouseRestrictions => true;
    public override bool ClickTitle => false;
    public override bool IsInvulnerable => true;
    public override string DefaultName => "a Mannequin";

    [Constructible]
    public Mannequin(Mobile owner) : base(AIType.AI_Use_Default, FightMode.None, 1, 1)
    {
        InitStats(100, 100, 25);

        SetDamageType(ResistanceType.Physical, 0);
        SetDamageType(ResistanceType.Fire, 0);
        SetDamageType(ResistanceType.Cold, 0);
        SetDamageType(ResistanceType.Poison, 0);
        SetDamageType(ResistanceType.Energy, 0);

        Hits = HitsMax;
        Blessed = true;
        Frozen = true;

        Owner = owner;
        Body = 0x190;
        Race = Race.Human;
        Hue = 1828;
        Direction = Direction.South;
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        // Frozen is a runtime flag that the core does not persist for mobiles.
        Frozen = true;
    }

    public bool IsOwner(Mobile m)
    {
        if (m == Owner || m.AccessLevel >= AccessLevel.GameMaster)
        {
            return true;
        }

        return AccountHandler.CheckAccount(m, Owner);
    }

    public override bool CanBeDamaged() => false;

    public override bool CanBeRenamedBy(Mobile from) => false;

    public override bool AllowEquipFrom(Mobile from)
    {
        if (IsOwner(from))
        {
            return true;
        }

        return base.AllowEquipFrom(from);
    }

    public override bool CheckNonlocalLift(Mobile from, Item item)
    {
        if (IsOwner(from))
        {
            return true;
        }

        return base.CheckNonlocalLift(from, item);
    }

    public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
    {
        if (IsOwner(from))
        {
            return true;
        }

        return false;
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        if (!string.IsNullOrEmpty(Description))
        {
            list.Add(1159410, Description); // Description: ~1_MESSAGE~
        }
    }

    public override void OnDoubleClick(Mobile from)
    {
        DisplayPaperdollTo(from);
    }

    public override void OnStatsQuery(Mobile from)
    {
        if (from.NetState == null || from.Map != Map ||
            !Utility.InUpdateRange(Location, from.Location) || !from.CanSee(this))
        {
            return;
        }

        from.NetState.SendMobileStatusCompact(this, CanBeRenamedBy(from));
    }

    public bool CheckSameLayer(Item i1, Item i2) =>
        i1 is BaseWeapon && i2 is BaseWeapon && _sameLayers.Contains(i1.Layer) && _sameLayers.Contains(i2.Layer);

    public bool LayerValidation(Item i1, Item i2) => i1.Layer == i2.Layer || CheckSameLayer(i1, i2);

    public static List<ValuedProperty> GetProperty(Item item) =>
        FindItemProperty(item, true).Where(x => x.Catalog != Catalog.None).ToList();

    public static List<ValuedProperty> GetProperty(List<Item> items) =>
        FindItemsProperty(items).Where(x => x.Catalog != Catalog.None).ToList();

    public static List<ValuedProperty> FindItemsProperty(List<Item> items)
    {
        var properties = new List<ValuedProperty>();

        foreach (var type in _propertyTypes)
        {
            if (Activator.CreateInstance(type) is ValuedProperty property &&
                (property.Matches(items) || property.AlwaysVisible))
            {
                properties.Add(property);
            }
        }

        return properties;
    }

    public static List<ValuedProperty> FindItemProperty(Item item, bool visible = false)
    {
        var properties = new List<ValuedProperty>();

        foreach (var type in _propertyTypes)
        {
            if (Activator.CreateInstance(type) is ValuedProperty property &&
                (property.Matches(item) || visible && property.AlwaysVisible))
            {
                properties.Add(property);
            }
        }

        return properties.OrderByDescending(x => x.Hue).ToList();
    }

    private static Type[] BuildPropertyTypeCache()
    {
        var propertyTypes = new List<Type>();

        foreach (var type in typeof(ValuedProperty).Assembly.GetTypes())
        {
            if (type.IsClass && !type.IsAbstract && type.Namespace == "Server.Mobiles.MannequinProperty" &&
                typeof(ValuedProperty).IsAssignableFrom(type))
            {
                propertyTypes.Add(type);
            }
        }

        return propertyTypes.ToArray();
    }

    public override void GetContextMenuEntries(Mobile from, ref PooledRefList<ContextMenuEntry> list)
    {
        base.GetContextMenuEntries(from, ref list);

        if (!IsOwner(from))
        {
            return;
        }

        if (from.Alive && from.InRange(this, 2))
        {
            list.Add(new ViewSuitsEntry(this));
            list.Add(new CompareWithItemInSlotEntry(this));
            list.Add(new ViewSuitsSelectItemEntry(this));
            list.Add(new AddDescriptionEntry(this));

            if (from.Race == Race ||
                from.Race == Race.Elf && Race == Race.Human ||
                from.Race == Race.Human && Race == Race.Elf)
            {
                list.Add(new SwitchClothesEntry(this));
            }

            list.Add(new RotateEntry(this));
            list.Add(new RedeedEntry(this));
        }
    }

    public static bool IsEquipped(Item item) =>
        item?.Parent is Mobile mobile && mobile.FindItemOnLayer(item.Layer) == item &&
        item.Layer != Layer.Mount && item.Layer != Layer.Bank &&
        item.Layer != Layer.Invalid && item.Layer != Layer.Backpack && item is not Server.Items.Backpack;

    public void SwitchClothes(Mobile from, Mobile m)
    {
        var mobileItems = new List<Item>();
        var mannequinItems = new List<Item>();

        foreach (var item in from.Items)
        {
            if (IsEquipped(item))
            {
                mobileItems.Add(item);
            }
        }

        foreach (var item in m.Items)
        {
            if (IsEquipped(item))
            {
                mannequinItems.Add(item);
            }
        }

        foreach (var item in mannequinItems)
        {
            m.RemoveItem(item);
        }

        foreach (var item in mobileItems)
        {
            from.RemoveItem(item);
        }

        var exceptItems = new List<Item>();

        foreach (var item in mannequinItems)
        {
            if (item.CanEquip(from))
            {
                from.EquipItem(item);
            }
            else
            {
                exceptItems.Add(item);
            }
        }

        foreach (var item in mobileItems)
        {
            if (item.CanEquip(m))
            {
                m.EquipItem(item);
            }
            else
            {
                exceptItems.Add(item);
            }
        }

        if (exceptItems.Count > 0)
        {
            foreach (var item in exceptItems)
            {
                from.AddToBackpack(item);
            }

            from.SendLocalizedMessage(1151641, exceptItems.Count.ToString(), 0x22);
        }

        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1151607);
    }

    private class ViewSuitsEntry : ContextMenuEntry
    {
        private readonly Mannequin _mannequin;

        public ViewSuitsEntry(Mannequin mannequin) : base(1159296, 3) // View Suit Stats
        {
            _mannequin = mannequin;
        }

        public override void OnClick(Mobile from, IEntity target)
        {
            from.SendGump(new MannequinStatsGump(_mannequin));
        }
    }

    private class CompareWithItemInSlotEntry : ContextMenuEntry
    {
        private readonly Mannequin _mannequin;

        public CompareWithItemInSlotEntry(Mannequin mannequin) : base(1159295, 3) // Compare With Selected Item
        {
            _mannequin = mannequin;
        }

        public override void OnClick(Mobile from, IEntity target)
        {
            from.SendLocalizedMessage(1159294); // Target the item you wish to compare.
            from.Target = new InternalTarget(_mannequin);
        }

        private class InternalTarget : Target
        {
            private readonly Mannequin _mannequin;

            public InternalTarget(Mannequin mannequin) : base(-1, false, TargetFlags.None)
            {
                _mannequin = mannequin;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (_mannequin == null || _mannequin.Deleted)
                {
                    return;
                }

                if (targeted is Item item)
                {
                    MannequinCompareGump.DisplayTo(from, _mannequin, item);
                }
                else
                {
                    from.SendLocalizedMessage(1149667); // Invalid target.
                }
            }
        }
    }

    private class ViewSuitsSelectItemEntry : ContextMenuEntry
    {
        private readonly Mannequin _mannequin;

        public ViewSuitsSelectItemEntry(Mannequin mannequin) : base(1159297, 3) // View Suit Stats With Selected Item
        {
            _mannequin = mannequin;
        }

        public override void OnClick(Mobile from, IEntity target)
        {
            from.SendLocalizedMessage(1159294); // Target the item you wish to compare.
            from.Target = new InternalTarget(_mannequin);
        }

        private class InternalTarget : Target
        {
            private readonly Mannequin _mannequin;

            public InternalTarget(Mannequin mannequin) : base(-1, false, TargetFlags.None)
            {
                _mannequin = mannequin;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (_mannequin == null || _mannequin.Deleted)
                {
                    return;
                }

                if (targeted is Item item)
                {
                    from.SendGump(new MannequinStatsGump(_mannequin, item));
                }
                else
                {
                    from.SendLocalizedMessage(1149667); // Invalid target.
                }
            }
        }
    }

    private class AddDescriptionEntry : ContextMenuEntry
    {
        private readonly Mannequin _mannequin;

        public AddDescriptionEntry(Mannequin mannequin) : base(1159411, 3) // Add Description
        {
            _mannequin = mannequin;
        }

        public override void OnClick(Mobile from, IEntity target)
        {
            from.SendGump(new MannequinDescriptionGump(_mannequin));
        }
    }

    private class MannequinDescriptionGump : DynamicGump
    {
        // Client text entry limit for the mannequin description.
        private const int MaxDescriptionLength = 44;

        private readonly Mannequin _mannequin;

        public MannequinDescriptionGump(Mannequin mannequin) : base(0, 0)
        {
            _mannequin = mannequin;
        }

        protected override void BuildLayout(ref DynamicGumpBuilder builder)
        {
            builder.AddPage();
            builder.AddBackground(50, 50, 400, 300, 0xA28);

            builder.AddHtmlLocalized(50, 70, 400, 20, 1159409, 0x0, false, false); // <CENTER>Mannequin</CENTER>
            // Enter the description to add to the mannequin. Leave the text area blank to remove any existing text.
            builder.AddHtmlLocalized(75, 95, 350, 145, 1159408, 0x0, true, true);
            builder.AddButton(125, 300, 0x81A, 0x81B, 1, GumpButtonType.Reply, 0);
            builder.AddButton(320, 300, 0x819, 0x818, 0, GumpButtonType.Reply, 0);

            builder.AddImageTiled(75, 245, 350, 40, 0xDB0);
            builder.AddImageTiled(76, 245, 350, 2, 0x23C5);
            builder.AddImageTiled(75, 245, 2, 40, 0x23C3);
            builder.AddImageTiled(75, 285, 350, 2, 0x23C5);
            builder.AddImageTiled(425, 245, 2, 42, 0x23C3);
            builder.AddTextEntryLimited(78, 246, 343, 37, 0x4FF, 0, "", MaxDescriptionLength);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (_mannequin == null || _mannequin.Deleted || info.ButtonID != 1)
            {
                return;
            }

            var description = info.GetTextEntry(0) ?? string.Empty;

            if (description.Length > MaxDescriptionLength)
            {
                description = description[..MaxDescriptionLength];
            }

            _mannequin.Description = description;
            _mannequin.InvalidateProperties();

            sender.Mobile.SendLocalizedMessage(1159412); // Updated
        }
    }

    private class SwitchClothesEntry : ContextMenuEntry
    {
        private readonly Mannequin _mannequin;

        public SwitchClothesEntry(Mannequin mannequin) : base(1151606, 2)
        {
            _mannequin = mannequin;
        }

        public override void OnClick(Mobile from, IEntity target)
        {
            if (from.HasTrade)
            {
                from.SendLocalizedMessage(1004041); // You can't do that while you have a trade pending.
                return;
            }

            _mannequin.SwitchClothes(from, _mannequin);
        }
    }

    private class RotateEntry : ContextMenuEntry
    {
        private readonly Mannequin _mannequin;

        public RotateEntry(Mannequin mannequin) : base(1151586, 2)
        {
            _mannequin = mannequin;
        }

        public override void OnClick(Mobile from, IEntity target)
        {
            var direction = (int)_mannequin.Direction + 1;

            if (direction > 0x7)
            {
                direction = 0x0;
            }

            _mannequin.Direction = (Direction)direction;

            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1151587); // You rotate the mannequin a little bit.
        }
    }

    private class RedeedEntry : ContextMenuEntry
    {
        private readonly Mannequin _mannequin;

        public RedeedEntry(Mannequin mannequin) : base(1151601, 2)
        {
            _mannequin = mannequin;
        }

        public override void OnClick(Mobile from, IEntity target)
        {
            var mannequinItems = new List<Item>();

            foreach (var item in _mannequin.Items)
            {
                if (IsEquipped(item))
                {
                    mannequinItems.Add(item);
                }
            }

            foreach (var item in mannequinItems)
            {
                from.AddToBackpack(item);
            }

            _mannequin.Delete();

            from.AddToBackpack(new MannequinDeed());
        }
    }
}

[SerializationGenerator(0, false)]
public partial class MannequinDeed : Item
{
    public override int LabelNumber => 1151602; // Mannequin Deed

    [Constructible]
    public MannequinDeed() : base(0x14F0)
    {
        LootType = LootType.Blessed;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            return;
        }

        var house = BaseHouse.FindHouseAt(from);

        if (house == null)
        {
            from.SendLocalizedMessage(502092); // You must be in your house to do this.
            return;
        }

        if (house.Owner != from && !house.IsCoOwner(from))
        {
            from.SendLocalizedMessage(502096); // You must own the house to do this.
            return;
        }

        from.SendLocalizedMessage(1151657); // Where do you wish to place this?
        from.Target = new PlaceTarget(this);
    }

    private class PlaceTarget : Target
    {
        private readonly MannequinDeed _deed;

        public PlaceTarget(MannequinDeed deed) : base(2, true, TargetFlags.None)
        {
            _deed = deed;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_deed.Deleted || !_deed.IsChildOf(from.Backpack))
            {
                return;
            }

            if (targeted is not IPoint3D point)
            {
                from.SendLocalizedMessage(501942); // That location is blocked.
                return;
            }

            var map = from.Map;

            if (map == null || map == Map.Internal)
            {
                return;
            }

            var location = new Point3D(point.X, point.Y, point.Z);
            var house = BaseHouse.FindHouseAt(location, map, 16);

            if (house == null)
            {
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
                return;
            }

            if (house.Owner != from && !house.IsCoOwner(from))
            {
                from.SendLocalizedMessage(502096); // You must own the house to do this.
                return;
            }

            if (!map.CanSpawnMobile(location))
            {
                from.SendLocalizedMessage(501942); // That location is blocked.
                return;
            }

            var mannequin = new Mannequin(from);
            mannequin.MoveToWorld(location, map);

            _deed.Delete();
        }
    }
}
