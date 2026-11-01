using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class DivinumLuminous : BaseTalisman
{
    public override int LabelNumber => 1160471;

    public override string DefaultName => "Divinum Luminous";

    [Constructible]
    public DivinumLuminous() : base(0x2F5A)
    {
        Hue = 1158;
        Attributes.Luck = 100;
        Attributes.ReflectPhysical = 15;
        Attributes.AttackChance = 10;
        Attributes.DefendChance = 10;
        Attributes.WeaponDamage = 20;
    }
}
