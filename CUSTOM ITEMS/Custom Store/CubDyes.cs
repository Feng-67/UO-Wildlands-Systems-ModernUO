/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;
using Server.Targeting;

namespace Server.Items;

/* BASE DYE SYSTEM */
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public abstract partial class BaseCubStoreDye : Item, IUsesRemaining
{
    [SerializableField(0)]
    [InvalidateProperties]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private int _usesRemaining;

    public override double DefaultWeight => 1.0;

    bool IUsesRemaining.ShowUsesRemaining
    {
        get => true;
        set { }
    }

    protected BaseCubStoreDye(int hue) : base(0xEFF)
    {
        Hue = hue;
        _usesRemaining = 5;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1070929); // Select the item to dye.
            from.Target = new InternalDyeTarget(this);
        }
        else
        {
            from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        list.Add(1060584, _usesRemaining); // uses remaining: ~1_val~
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 wrote the use count).
    private void Deserialize(IGenericReader reader, int version)
    {
        _usesRemaining = reader.ReadInt();
    }

    private class InternalDyeTarget : Target
    {
        private readonly BaseCubStoreDye _dye;

        public InternalDyeTarget(BaseCubStoreDye dye) : base(8, false, TargetFlags.None) => _dye = dye;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is not Item item)
            {
                from.SendLocalizedMessage(1042083); // You cannot dye that.
                return;
            }

            if (!item.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                return;
            }

            if (item is HoodedShroudOfShadows or MonkRobe)
            {
                from.SendLocalizedMessage(1042083); // You cannot dye that.
                return;
            }

            // TODO(migrate): the ServUO check also allowed DecorativePlant, ShoulderParrot, BaseStatuette and
            // Item.IsArtifact. None of those exist in ModernUO, so they are not part of this check.
            var valid = item is IDyable or BaseTalisman or BaseBook or BaseClothing or BaseJewel or BaseWeapon
                or Runebook or Spellbook || BasePigmentsOfTokuno.IsValidItem(item);

            if (!valid && item is BaseArmor armor)
            {
                var resourceType = CraftResources.GetType(armor.Resource);

                if (resourceType is CraftResourceType.Leather or CraftResourceType.Metal &&
                    armor.MaterialType != ArmorMaterialType.Bone)
                {
                    valid = true;
                }
            }

            if (!valid)
            {
                from.SendLocalizedMessage(1042083); // You cannot dye that.
                return;
            }

            item.Hue = _dye.Hue;
            from.PlaySound(0x23E);

            if (--_dye.UsesRemaining <= 0)
            {
                _dye.Delete();
            }
        }
    }
}

/* INDIVIDUAL DYE CLASSES */
// Re-uses Clilocs and Hues from CompassionPigment.cs

[SerializationGenerator(1, false)]
public partial class CubPhoenixRed : BaseCubStoreDye
{
    public override int LabelNumber => 1151651;

    [Constructible]
    public CubPhoenixRed() : base(1964)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubAuraOfAmber : BaseCubStoreDye
{
    public override int LabelNumber => 1152308;

    [Constructible]
    public CubAuraOfAmber() : base(1967)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubDeepViolet : BaseCubStoreDye
{
    public override int LabelNumber => 1151912;

    [Constructible]
    public CubDeepViolet() : base(1929)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubPolishedBronze : BaseCubStoreDye
{
    public override int LabelNumber => 1151909;

    [Constructible]
    public CubPolishedBronze() : base(1944)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubVibrantCrimson : BaseCubStoreDye
{
    public override int LabelNumber => 1153386;

    [Constructible]
    public CubVibrantCrimson() : base(1964)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubLavender : BaseCubStoreDye
{
    public override int LabelNumber => 1151650;

    [Constructible]
    public CubLavender() : base(1951)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubGleamingFuchsia : BaseCubStoreDye
{
    public override int LabelNumber => 1152311;

    [Constructible]
    public CubGleamingFuchsia() : base(1930)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubDeepBlue : BaseCubStoreDye
{
    public override int LabelNumber => 1152348;

    [Constructible]
    public CubDeepBlue() : base(1939)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubGlossyFuchsia : BaseCubStoreDye
{
    public override int LabelNumber => 1152347;

    [Constructible]
    public CubGlossyFuchsia() : base(1919)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubDarkVoid : BaseCubStoreDye
{
    public override int LabelNumber => 1154214;

    [Constructible]
    public CubDarkVoid() : base(2068)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubMurkySeagreen : BaseCubStoreDye
{
    public override int LabelNumber => 1152309;

    [Constructible]
    public CubMurkySeagreen() : base(1992)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubReflectiveShadow : BaseCubStoreDye
{
    public override int LabelNumber => 1153387;

    [Constructible]
    public CubReflectiveShadow() : base(1910)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubLiquidSunshine : BaseCubStoreDye
{
    public override int LabelNumber => 1154213;

    [Constructible]
    public CubLiquidSunshine() : base(1923)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubShadowyBlue : BaseCubStoreDye
{
    public override int LabelNumber => 1152310;

    [Constructible]
    public CubShadowyBlue() : base(1960)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubBlackAndGreen : BaseCubStoreDye
{
    public override int LabelNumber => 1151911;

    [Constructible]
    public CubBlackAndGreen() : base(1979)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubGlossyBlue : BaseCubStoreDye
{
    public override int LabelNumber => 1151910;

    [Constructible]
    public CubGlossyBlue() : base(1916)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubHunterGreen : BaseCubStoreDye
{
    public override int LabelNumber => 1151649;

    [Constructible]
    public CubHunterGreen() : base(1936)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubSlateBlue : BaseCubStoreDye
{
    public override int LabelNumber => 1151653;

    [Constructible]
    public CubSlateBlue() : base(1983)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubMotherOfPearl : BaseCubStoreDye
{
    public override int LabelNumber => 1154120;

    [Constructible]
    public CubMotherOfPearl() : base(2720)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubStarBlue : BaseCubStoreDye
{
    public override int LabelNumber => 1154121;

    [Constructible]
    public CubStarBlue() : base(2723)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubMurkyAmber : BaseCubStoreDye
{
    public override int LabelNumber => 1152350;

    [Constructible]
    public CubMurkyAmber() : base(1989)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubVibranSeagreen : BaseCubStoreDye
{
    public override int LabelNumber => 1152349;

    [Constructible]
    public CubVibranSeagreen() : base(1970)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubVibrantOcher : BaseCubStoreDye
{
    public override int LabelNumber => 1154736;

    [Constructible]
    public CubVibrantOcher() : base(2725)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubMossyGreen : BaseCubStoreDye
{
    public override int LabelNumber => 1154731;

    [Constructible]
    public CubMossyGreen() : base(2684)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubOliveGreen : BaseCubStoreDye
{
    public override int LabelNumber => 1154733;

    [Constructible]
    public CubOliveGreen() : base(2709)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubMottledSunsetBlue : BaseCubStoreDye
{
    public override int LabelNumber => 1154734;

    [Constructible]
    public CubMottledSunsetBlue() : base(2714)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubTyrianPurple : BaseCubStoreDye
{
    public override int LabelNumber => 1154735;

    [Constructible]
    public CubTyrianPurple() : base(2716)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class CubIntenseTeal : BaseCubStoreDye
{
    public override int LabelNumber => 1154732;

    [Constructible]
    public CubIntenseTeal() : base(2691)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
