using System;
using ModernUO.Serialization;
using Server.Gumps;
using Server.Multis;

namespace Server.Items;

public enum AddonFacing
{
    South = 0,
    East = 1
}

[SerializationGenerator(0, false)]
public partial class DragonHeadAddon : BaseAddon
{
    [Constructible]
    public DragonHeadAddon() : this(AddonFacing.South)
    {
    }

    [Constructible]
    public DragonHeadAddon(AddonFacing facing)
    {
        switch (facing)
        {
            case AddonFacing.South:
                {
                    AddComponent(new DragonHeadComponent(0x2234), 0, 0, 10);
                    break;
                }
            case AddonFacing.East:
                {
                    AddComponent(new DragonHeadComponent(0x2235), 0, 0, 10);
                    break;
                }
        }
    }
       
    [SerializationGenerator(0, false)]
    private partial class DragonHeadComponent : AddonComponent
    {
        public DragonHeadComponent(int id) : base(id)
        {
        }

        public override bool NeedsWall => true;

        public override Point3D WallPosition =>
            ItemID switch
            {
                0x2235 => new Point3D(-1, 0, 0),
                _      => new Point3D(0, -1, 0)
            };
    }
}

[SerializationGenerator(0, false)]
public partial class DragonHeadAddonDeed : BaseAddonDeed, IRewardOption
{
    private AddonFacing _facing;

    [Constructible]
    public DragonHeadAddonDeed() => LootType = LootType.Blessed;

    public override BaseAddon Addon => new DragonHeadAddon(_facing);

    public override int LabelNumber => 1080209; // Dragon Head

    public void GetOptions(RewardOptionList list)
    {
        list.Add((int)AddonFacing.South, 1080208);
        list.Add((int)AddonFacing.East, 1080207);
    }

    public void OnOptionSelected(Mobile from, int choice)
    {
        _facing = (AddonFacing)choice;

        if (!Deleted)
        {
            base.OnDoubleClick(from);
        }
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (IsChildOf(from.Backpack))
        {
            RewardOptionGump.DisplayTo(from, this);
        }
        else
        {
            from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }
    }
}

// old classes
[TypeAlias("Server.Items.DragonHead")]
[SerializationGenerator(0)]
public partial class DragonHeadOld : Item, IAddon, IChoppable
{
    [Constructible]
    public DragonHeadOld() : this(0x2234)
    {
    }

    [Constructible]
    public DragonHeadOld(int itemID) : base(itemID) => Movable = false;
        
    public override bool ForceShowProperties => ObjectPropertyList.Enabled;

    public Item Deed => new DragonHeadAddonDeed();

    public bool FacingEast => ItemID == 0x2235;

    public bool CouldFit(IPoint3D p, Map map)
    {
        if (map?.CanFit(p.X, p.Y, p.Z, ItemData.Height) != true)
        {
            return false;
        }

        return FacingEast
            ? BaseAddon.IsWall(p.X - 1, p.Y, p.Z, map)  // west wall
            : BaseAddon.IsWall(p.X, p.Y - 1, p.Z, map); // north wall
    }

    void IChoppable.OnChop(Mobile user) => OnDoubleClick(user);

    public override void OnDoubleClick(Mobile from)
    {
        if (!from.InRange(Location, 2))
        {
            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            return;
        }

        var house = BaseHouse.FindHouseAt(this);

        if (house?.IsOwner(from) == true)
        {
            RewardDemolitionGump.DisplayTo(from, this, 1049783); // Do you wish to re-deed this decoration?
        }
        else
        {
            // You can only re-deed this decoration if you are the house owner or originally placed the decoration.
            from.SendLocalizedMessage(1049784);
        }
    }
}

[TypeAlias("Server.Items.DragonHeadDeed")]
[SerializationGenerator(0)]
public partial class DragonHeadDeedOld : Item
{
    [Constructible]
    public DragonHeadDeedOld() : base(0x14F0) => LootType = LootType.Blessed;

    public override double DefaultWeight => 1.0;

    public override int LabelNumber => 1028756; // dragon head

    [AfterDeserialization]
    private void AfterDeserialization() => Timer.StartTimer(TimeSpan.FromSeconds(10), Replace);

    private void Replace()
    {
        if (Deleted)
        {
            return;
        }

        if (Parent is Container c)
        {
            c.DropItem(new DragonHeadAddonDeed());
        }
        else if (Parent == null)
        {
            var house = BaseHouse.FindHouseAt(this);

            var deed = new DragonHeadAddonDeed();
            deed.MoveToWorld(Location, Map);

            deed.IsLockedDown = IsLockedDown;
            deed.Movable = Movable;

            // ModernUO tracks lockdowns as a plain list of items
            if (house != null && house.LockDowns.Remove(this))
            {
                house.LockDowns.Add(deed);
            }
        }

        Delete();
    }
}
