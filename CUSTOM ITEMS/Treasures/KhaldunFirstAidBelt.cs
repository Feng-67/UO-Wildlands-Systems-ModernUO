using System;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class KhaldunFirstAidBelt : Container
{
    private static readonly Type[] _bandageTypes = { typeof(Bandage), typeof(EnhancedBandage) };

    [InvalidateProperties]
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private int _healingBonus;

    private bool ShouldSerializeHealingBonus() => _healingBonus != 0;

    [InvalidateProperties]
    [SerializableField(1)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private int _weightReduction;

    private bool ShouldSerializeWeightReduction() => _weightReduction != 0;

    [Constructible]
    public KhaldunFirstAidBelt() : base(0xA1F6)
    {
        LootType = LootType.Blessed;
        Weight = 2.0;
        Layer = Layer.Waist;
        _weightReduction = 50;
        _healingBonus = 10;
    }

    public override int LabelNumber => 1158681; // First Aid Belt
    public override int DefaultGumpID => 0x3C;
    public override int DefaultMaxItems => 1;
    public override int DefaultMaxWeight => 100;
    public override double DefaultWeight => 2.0;
    public override bool DisplaysContent => false;

    public Item Bandage => Items.Count > 0 ? Items[0] : null;
    public int MaxBandage => DefaultMaxWeight * 10;

    public override int GetTotal(TotalType type)
    {
        var total = base.GetTotal(type);
        if (type == TotalType.Weight)
        {
            total -= total * _weightReduction / 100;
        }
        return total;
    }

    public bool CheckType(Item item)
    {
        var type = item.GetType();
        var bandage = Bandage;
        if (bandage != null)
        {
            return bandage.GetType() == type;
        }
        for (var i = 0; i < _bandageTypes.Length; i++)
        {
            if (type == _bandageTypes[i])
            {
                return true;
            }
        }
        return false;
    }

    public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
    {
        if (!Movable) return false;

        if (!CheckType(item))
        {
            if (message) m.SendLocalizedMessage(1074836);
            return false;
        }

        if (!checkItems || Items.Count < DefaultMaxItems)
        {
            var currentAmount = 0;
            for (var i = 0; i < Items.Count; i++) currentAmount += Items[i].Amount;

            if (item.Amount + currentAmount <= MaxBandage)
            {
                return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
            }
            if (message) m.SendLocalizedMessage(1080017);
        }
        return false;
    }

    public override void AddItem(Item dropped) { base.AddItem(dropped); InvalidateWeight(); }
    public override void RemoveItem(Item dropped) { base.RemoveItem(dropped); InvalidateWeight(); }

    public override void UpdateTotal(Item sender, TotalType type, int delta)
    {
        InvalidateProperties();
        base.UpdateTotal(sender, type, delta);
    }

    public void InvalidateWeight()
    {
        if (RootParent is Mobile m) m.UpdateTotals();
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        if (_healingBonus > 0)
        {
            list.Add(1158679, _healingBonus); // ~1_VALUE~% Bandage Healing Bonus
        }

        var bandage = Bandage;
        var weight = bandage != null ? bandage.Weight * bandage.Amount : 0;

        list.Add(1072241, $"{Items.Count}\t{DefaultMaxItems}\t{(int)weight}\t{DefaultMaxWeight}");

        if (_weightReduction != 0)
        {
            list.Add(1072210, _weightReduction); // Weight reduction: ~1_PERCENTAGE~%
        }
    }
}
