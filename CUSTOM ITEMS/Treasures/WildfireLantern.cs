using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class WildfireLantern : GoldRing
{
    public override string DefaultName => "Wildfire Lantern";

    [Constructible]
    public WildfireLantern()
    {
        Attributes.SpellChanneling = 1;
        Attributes.RegenMana = 3;
        Attributes.SpellDamage = 5;
        Attributes.CastRecovery = 2;
        Light = LightType.Circle300;
        Weight = 1.0;
        ItemID = 0xA76A;
        Layer = Layer.TwoHanded;
    }

    public override void AddNameProperties(IPropertyList list)
    {
        base.AddNameProperties(list);

        if (ItemID == 0xA769)
        {
            list.Add(1049644, $"{"Double-Click to Unequip"}");
        }
        else
        {
            list.Add(1049644, $"{"Double-Click to Equip"}");
        }
    }     

    public override bool OnEquip(Mobile from)
    {
        ItemID = 0xA769;
        return base.OnEquip(from);
    }

    public override void OnRemoved(IEntity parent)
    {
        ItemID = 0xA76A;
        base.OnRemoved(parent);
    }

    public override void OnDoubleClick(Mobile from)
    {
        var lantern = from.FindItemOnLayer(Layer.TwoHanded);

        if (lantern == this)
        {
            from.AddToBackpack(this);
            ItemID = 0xA76A;
            from.PlaySound(0x4BB);
            base.OnRemoved(from);
        }
        else if (!IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
        else
        {
            var equipped = from.FindItemOnLayer(Layer.TwoHanded);

            if (equipped != null)
            {
                from.AddToBackpack(equipped);
            }

            from.SendMessage("You put the lantern in your left hand.");
            from.AddItem(this);
            ItemID = 0xA769;
            from.PlaySound(0x47);
            base.OnEquip(from);
        }
    }
}
