using ModernUO.Serialization;
using Server;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class WeaponStrap : Container
{
    [InvalidateProperties]
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private int _weightReduction;

    public override int DefaultMaxItems => 10;
    public override int DefaultGumpID => 61;

    [Constructible]
    public WeaponStrap() : base(0x2B02)
    {
        Hue = 0x901;
        Layer = Layer.Cloak;
        Weight = 1.0;
        _weightReduction = 50;

        // Explicit name so it shows on the item
        Name = "Weapon Strap";
    }

    public override int GetTotal(TotalType type)
    {
        var total = base.GetTotal(type);

        if (type == TotalType.Weight && _weightReduction > 0)
        {
            total = total * (100 - _weightReduction) / 100;
        }

        return total;
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        if (_weightReduction > 0)
        {
            list.Add(1072210, _weightReduction);
        }
    }

    // Restrict what can be placed inside to melee weapons only
    public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
    {
        if (item is not BaseWeapon)
        {
            if (message)
            {
                m.SendMessage("You can only place melee weapons in a weapon strap.");
            }

            return false;
        }

        if (item is BaseRanged)
        {
            if (message)
            {
                m.SendMessage("Ranged weapons do not belong in a weapon strap.");
            }

            return false;
        }

        return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (from == null || !from.Alive)
        {
            return;
        }

        if (Parent == from || !IsChildOf(from.Backpack))
        {
            base.OnDoubleClick(from);
        }
        else
        {
            from.EquipItem(this);
        }
    }

    // Legacy hook: loads saves written by the ServUO version (version 0).
    // Delete this method if existing saves do not need to load.
    private void Deserialize(IGenericReader reader, int version)
    {
        _weightReduction = reader.ReadInt();
    }
}
