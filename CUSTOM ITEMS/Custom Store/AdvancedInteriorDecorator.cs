/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using ModernUO.Serialization;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items;

// Named differently from ModernUO's DecorateCommand (None/Turn/Up/Down), which already exists in
// InteriorDecorator.cs and would clash with this larger set.
public enum AdvancedDecorateCommand
{
    None,
    Turn,
    Up,
    Down,
    North,
    East,
    South,
    West,
    GetHue,
    LockDown,
    Secure,
    Release
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class AdvancedInteriorDecorator : Item
{
    public override int LabelNumber => 1041280; // an advanced interior decorator
    public override double DefaultWeight => 1.0;

    [Constructible]
    public AdvancedInteriorDecorator() : base(0xFC1)
    {
        LootType = LootType.Blessed;
    }

    // The selected command is runtime state only; it is not saved
    [CommandProperty(AccessLevel.GameMaster)]
    public AdvancedDecorateCommand Command { get; set; }

    public static bool InHouse(Mobile from) => BaseHouse.FindHouseAt(from)?.IsCoOwner(from) == true;

    public static bool CheckUse(Mobile from)
    {
        if (InHouse(from))
        {
            return true;
        }

        from.SendLocalizedMessage(502092); // You must be in your house to do this.
        return false;
    }

    public override void OnDoubleClick(Mobile from)
    {
        // Outside a house the only thing the tool can do is read hues
        if (!InHouse(from))
        {
            Command = AdvancedDecorateCommand.GetHue;
        }

        if (!from.HasGump<AdvancedDecoratorGump>())
        {
            AdvancedDecoratorGump.DisplayTo(from, this);
        }

        if (Command != AdvancedDecorateCommand.None)
        {
            from.Target = new InternalTarget(this);
        }
    }

    private class AdvancedDecoratorGump : DynamicGump
    {
        private readonly AdvancedInteriorDecorator _decorator;
        private readonly bool _inHouse;

        public override bool Singleton => true;

        private AdvancedDecoratorGump(Mobile from, AdvancedInteriorDecorator decorator) : base(150, 50)
        {
            _decorator = decorator;
            _inHouse = InHouse(from);
        }

        public static void DisplayTo(Mobile from, AdvancedInteriorDecorator decorator)
        {
            if (from?.NetState == null || decorator?.Deleted != false)
            {
                return;
            }

            from.SendGump(new AdvancedDecoratorGump(from, decorator));
        }

        // The selected command's button is drawn pressed
        private int ButtonGraphic(AdvancedDecorateCommand command) => _decorator.Command == command ? 2154 : 2152;

        protected override void BuildLayout(ref DynamicGumpBuilder builder)
        {
            builder.AddBackground(0, 0, 170, 510, 2600);
            builder.AddPage();

            if (!_inHouse)
            {
                builder.AddButton(40, 36, ButtonGraphic(AdvancedDecorateCommand.GetHue), 2154, 8);
                builder.AddHtmlLocalized(80, 41, 100, 20, 1158863); // Get Hue
                return;
            }

            builder.AddButton(40, 45, ButtonGraphic(AdvancedDecorateCommand.Turn), 2154, 1);
            builder.AddHtmlLocalized(80, 50, 70, 40, 1018323); // Turn

            builder.AddButton(40, 85, ButtonGraphic(AdvancedDecorateCommand.Up), 2154, 2);
            builder.AddHtmlLocalized(80, 90, 70, 40, 1018324); // Up

            builder.AddButton(40, 125, ButtonGraphic(AdvancedDecorateCommand.Down), 2154, 3);
            builder.AddHtmlLocalized(80, 130, 70, 40, 1018325); // Down

            builder.AddButton(40, 165, ButtonGraphic(AdvancedDecorateCommand.North), 2154, 4);
            builder.AddHtmlLocalized(80, 170, 70, 40, 1075389); // North

            builder.AddButton(40, 205, ButtonGraphic(AdvancedDecorateCommand.East), 2154, 5);
            builder.AddHtmlLocalized(80, 210, 70, 40, 1075387); // East

            builder.AddButton(40, 245, ButtonGraphic(AdvancedDecorateCommand.South), 2154, 6);
            builder.AddHtmlLocalized(80, 250, 70, 40, 1075386); // South

            builder.AddButton(40, 285, ButtonGraphic(AdvancedDecorateCommand.West), 2154, 7);
            builder.AddHtmlLocalized(80, 290, 70, 40, 1075390); // West

            builder.AddButton(40, 325, ButtonGraphic(AdvancedDecorateCommand.GetHue), 2154, 8);
            builder.AddHtmlLocalized(80, 330, 100, 20, 1158863); // Get Hue

            builder.AddButton(40, 365, ButtonGraphic(AdvancedDecorateCommand.LockDown), 2154, 9);
            builder.AddHtml(80, 370, 100, 20, "Lock Down");

            builder.AddButton(40, 405, ButtonGraphic(AdvancedDecorateCommand.Secure), 2154, 10);
            builder.AddHtml(80, 410, 100, 20, "Secure");

            builder.AddButton(40, 445, ButtonGraphic(AdvancedDecorateCommand.Release), 2154, 11);
            builder.AddHtml(80, 450, 100, 20, "Release");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            var m = sender.Mobile;

            // The prompt is either a localized message number or plain text
            (AdvancedDecorateCommand command, int cliloc, string text) = info.ButtonID switch
            {
                1  => (AdvancedDecorateCommand.Turn, 1073404, null),                      // Select an object to turn.
                2  => (AdvancedDecorateCommand.Up, 1073405, null),                        // Select an object to increase its height.
                3  => (AdvancedDecorateCommand.Down, 1073406, null),                      // Select an object to lower its height.
                4  => (AdvancedDecorateCommand.North, 0, "Select an object to move North."),
                5  => (AdvancedDecorateCommand.East, 0, "Select an object to move East."),
                6  => (AdvancedDecorateCommand.South, 0, "Select an object to move South."),
                7  => (AdvancedDecorateCommand.West, 0, "Select an object to move West."),
                8  => (AdvancedDecorateCommand.GetHue, 1158864, null),                    // Select an object to get the hue.
                9  => (AdvancedDecorateCommand.LockDown, 0, "Select an object to lock down."),
                10 => (AdvancedDecorateCommand.Secure, 0, "Select an object to secure."),
                11 => (AdvancedDecorateCommand.Release, 0, "Select an object to release."),
                _  => (AdvancedDecorateCommand.None, 0, null)
            };

            if (command == AdvancedDecorateCommand.None)
            {
                Target.Cancel(m);
                return;
            }

            _decorator.Command = command;
            DisplayTo(m, _decorator);

            if (cliloc != 0)
            {
                m.SendLocalizedMessage(cliloc);
            }
            else
            {
                m.SendMessage(text);
            }

            m.Target = new InternalTarget(_decorator);
        }
    }

    private class InternalTarget : Target
    {
        private readonly AdvancedInteriorDecorator _decorator;

        public InternalTarget(AdvancedInteriorDecorator decorator) : base(-1, false, TargetFlags.None)
        {
            CheckLOS = false;

            _decorator = decorator;
        }

        protected override void OnTargetNotAccessible(Mobile from, object targeted)
        {
            OnTarget(from, targeted);
        }

        // King's Collection lamps can be turned without being locked down.
        // TODO(migrate): these types are not in stock ModernUO, so they are matched by name.
        private static bool IsKingsCollection(Item item) =>
            item.GetType().Name is "BirdLamp" or "DragonLantern" or "KoiLamp" or "TallLamp";

        private static bool IsDecorableComponent(Item item, AdvancedDecorateCommand command)
        {
            if (command == AdvancedDecorateCommand.Turn && IsKingsCollection(item))
            {
                return true;
            }

            object addon = null;
            var count = 0;

            switch (item)
            {
                case AddonComponent component:
                    {
                        count = component.Addon.Components.Count;
                        addon = component.Addon;
                        break;
                    }
                case AddonContainerComponent containerComponent:
                    {
                        count = containerComponent.Addon.Components.Count;
                        addon = containerComponent.Addon;
                        break;
                    }
                case BaseAddonContainer container:
                    {
                        count = container.Components.Count;
                        addon = container;
                        break;
                    }
                case Banner:
                    {
                        return command != AdvancedDecorateCommand.Turn;
                    }
                default:
                    {
                        return false;
                    }
            }

            // TODO(migrate): EnormousVenusFlytrapAddon is not in stock ModernUO, so it is matched by name.
            if (count == 1 && Core.SE || item.GetType().Name == "EnormousVenusFlytrapAddon")
            {
                return true;
            }

            return command == AdvancedDecorateCommand.Turn &&
                   addon.GetType().GetCustomAttributes(typeof(FlippableAddonAttribute), false).Length > 0;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            var command = _decorator.Command;

            if (command == AdvancedDecorateCommand.GetHue)
            {
                int hue;

                if (targeted is Item item)
                {
                    hue = item.Hue;
                }
                else if (targeted is Mobile mobile)
                {
                    hue = mobile.Hue;
                }
                else
                {
                    from.Target = new InternalTarget(_decorator);
                    return;
                }

                from.SendLocalizedMessage(1158862, $"{hue}"); // That object is hue ~1_HUE~
            }
            else if (targeted is Item decorItem && CheckUse(from))
            {
                Decorate(from, decorItem, command);
            }

            from.Target = new InternalTarget(_decorator);
        }

        private static void Decorate(Mobile from, Item item, AdvancedDecorateCommand command)
        {
            var house = BaseHouse.FindHouseAt(from);
            var isDecorableComponent = IsDecorableComponent(item, command);

            if (house?.IsCoOwner(from) != true)
            {
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            }
            else if (item.Parent != null || !house.IsInside(item))
            {
                from.SendLocalizedMessage(1042270); // That is not in your house.
            }
            else if (!house.HasLockedDownItem(item) && !house.HasSecureItem(item) && !isDecorableComponent &&
                     command != AdvancedDecorateCommand.LockDown && command != AdvancedDecorateCommand.Secure)
            {
                if (item is AddonComponent && command == AdvancedDecorateCommand.Turn)
                {
                    from.SendLocalizedMessage(1042273); // You cannot turn that.
                }
                else if (item is AddonComponent && command == AdvancedDecorateCommand.Up)
                {
                    from.SendLocalizedMessage(1042274); // You cannot raise it up any higher.
                }
                else if (item is AddonComponent && command == AdvancedDecorateCommand.Down)
                {
                    from.SendLocalizedMessage(1042275); // You cannot lower it down any further.
                }
                else
                {
                    from.SendLocalizedMessage(1042271); // That is not locked down.
                }
            }
            else if (item is VendorRentalContract)
            {
                from.SendLocalizedMessage(1062491); // You cannot use the house decorator on that object.
            }
            else
            {
                switch (command)
                {
                    case AdvancedDecorateCommand.Up:
                        {
                            Up(item, from);
                            break;
                        }
                    case AdvancedDecorateCommand.Down:
                        {
                            Down(item, from);
                            break;
                        }
                    case AdvancedDecorateCommand.Turn:
                        {
                            Turn(item, from);
                            break;
                        }
                    case AdvancedDecorateCommand.North:
                        {
                            Move(item, from, house, 0, -1, "North");
                            break;
                        }
                    case AdvancedDecorateCommand.East:
                        {
                            Move(item, from, house, 1, 0, "East");
                            break;
                        }
                    case AdvancedDecorateCommand.South:
                        {
                            Move(item, from, house, 0, 1, "South");
                            break;
                        }
                    case AdvancedDecorateCommand.West:
                        {
                            Move(item, from, house, -1, 0, "West");
                            break;
                        }
                    case AdvancedDecorateCommand.LockDown:
                        {
                            house.LockDown(from, item);
                            break;
                        }
                    case AdvancedDecorateCommand.Secure:
                        {
                            house.AddSecure(from, item);
                            break;
                        }
                    case AdvancedDecorateCommand.Release:
                        {
                            house.Release(from, item);
                            break;
                        }
                }
            }
        }

        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            if (cancelType == TargetCancelType.Canceled)
            {
                from.CloseGump<AdvancedDecoratorGump>();
            }
        }

        private static void Turn(Item item, Mobile from)
        {
            // TODO(migrate): ServUO first called IFlipable.OnFlip(from) on items that implement it. That interface is
            // not in stock ModernUO; add the call here if your server has it.
            object addon = null;

            if (item is AddonComponent component)
            {
                addon = component.Addon;
            }
            else if (item is AddonContainerComponent containerComponent)
            {
                addon = containerComponent.Addon;
            }
            else if (item is BaseAddonContainer container)
            {
                addon = container;
            }

            if (addon != null)
            {
                var addonAttributes =
                    (FlippableAddonAttribute[])addon.GetType().GetCustomAttributes(typeof(FlippableAddonAttribute), false);

                if (addonAttributes.Length > 0)
                {
                    addonAttributes[0].Flip(from, (Item)addon);
                    return;
                }
            }

            var attributes = (FlippableAttribute[])item.GetType().GetCustomAttributes(typeof(FlippableAttribute), false);

            if (attributes.Length > 0)
            {
                attributes[0].Flip(item);
            }
            else
            {
                from.SendLocalizedMessage(1042273); // You cannot turn that.
            }
        }

        private static void Up(Item item, Mobile from)
        {
            var floorZ = GetFloorZ(item);

            if (floorZ > int.MinValue && item.Z < floorZ + 40)
            {
                item.Location = new Point3D(item.X, item.Y, item.Z + 1);
            }
            else
            {
                from.SendLocalizedMessage(1042274); // You cannot raise it up any higher.
            }
        }

        private static void Down(Item item, Mobile from)
        {
            var floorZ = GetFloorZ(item);

            if (floorZ > int.MinValue && item.Z > floorZ)
            {
                item.Location = new Point3D(item.X, item.Y, item.Z - 1);
            }
            else
            {
                from.SendLocalizedMessage(1042275); // You cannot lower it down any further.
            }
        }

        // Moves the item one tile, as long as the new spot is still inside the house
        private static void Move(Item item, Mobile from, BaseHouse house, int dx, int dy, string direction)
        {
            if (!house.IsInside(new Point3D(item.X + dx, item.Y + dy, item.Z), item.Z))
            {
                from.SendMessage($"You cannot move it any further {direction}.");
                return;
            }

            item.Location = new Point3D(item.X + dx, item.Y + dy, item.Z);
        }

        private static int GetFloorZ(Item item)
        {
            var map = item.Map;

            if (map == null)
            {
                return int.MinValue;
            }

            var z = int.MinValue;

            foreach (var tile in map.Tiles.GetStaticAndMultiTiles(item.X, item.Y))
            {
                var id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                var top = tile.Z;

                if (id.Surface && !id.Impassable && top > z && top <= item.Z)
                {
                    z = top;
                }
            }

            // No static floor: fall back to the land tile
            if (z == int.MinValue)
            {
                z = map.Tiles.GetLandTile(item.X, item.Y).Z;
            }

            return z;
        }
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
