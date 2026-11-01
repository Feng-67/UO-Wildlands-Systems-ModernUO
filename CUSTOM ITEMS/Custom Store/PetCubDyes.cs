using System;
using ModernUO.Serialization;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items;

/* BASE CLASSES */

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public abstract partial class BasePetCubDye : Item
{
    public abstract int TargetHue { get; }
    public abstract string DyeName { get; }
    public virtual bool IsSample => false;

    public override double DefaultWeight => 1.0;

    protected BasePetCubDye(int itemID) : base(itemID)
    {
        LootType = LootType.Blessed;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!from.InRange(GetWorldLocation(), 1))
        {
            from.SendLocalizedMessage(500446); // That is too far away.
            return;
        }

        if (IsSample)
        {
            from.SendMessage($"Target the pet you wish to sample the {DyeName} dye on.");
        }
        else
        {
            from.SendMessage($"Target the pet you wish to apply the {DyeName} dye on.");
        }

        from.Target = new InternalPetTarget(this);
    }

    private class InternalPetTarget : Target
    {
        private readonly BasePetCubDye _dye;

        public InternalPetTarget(BasePetCubDye dye) : base(1, false, TargetFlags.None) => _dye = dye;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is not BaseCreature pet || pet.ControlMaster != from)
            {
                from.SendMessage("You can only dye your own pets!");
                return;
            }

            if (!from.InRange(_dye.GetWorldLocation(), 1) || !from.InRange(pet.Location, 1))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            var oldHue = pet.Hue;
            pet.Hue = _dye.TargetHue;
            from.PlaySound(0x23E);

            if (_dye.IsSample)
            {
                // Capture the name before the dye is deleted
                var dyeName = _dye.DyeName;

                from.SendMessage("The sample is applied! Your pet will revert in 30 seconds.");

                Timer.DelayCall(
                    TimeSpan.FromSeconds(30.0),
                    () =>
                    {
                        if (pet.Deleted)
                        {
                            return;
                        }

                        pet.Hue = oldHue;
                        pet.ControlMaster?.SendMessage($"The {dyeName} sample has worn off.");
                    }
                );
            }
            else
            {
                from.SendMessage($"The dye tub vanishes as your pet turns {_dye.DyeName}!");
            }

            _dye.Delete();
        }
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public abstract partial class BasePetCubPermanent : BasePetCubDye
{
    public override bool IsSample => false;

    protected BasePetCubPermanent(int hue) : base(0xFAB) => Hue = hue;

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        list.Add(1114057, $"{"<BASEFONT COLOR=#FFFF00>Single Use Only</BASEFONT>"}");
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public abstract partial class BasePetCubSample : BasePetCubDye
{
    public override bool IsSample => true;

    protected BasePetCubSample(int hue) : base(0xEFB) => Hue = hue;

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        list.Add(1114057, $"{"<BASEFONT COLOR=#FFFF00>30 sec Sample Pet Dye</BASEFONT>"}");
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

/* DEFINITIONS */

// Phoenix Red (1964)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubPhoenixRed : BasePetCubPermanent
{
    public override int TargetHue => 1964;
    public override string DyeName => "Phoenix Red";
    public override string DefaultName => "Pet Dye Tub: Phoenix Red";

    [Constructible]
    public PetCubPhoenixRed() : base(1964)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubPhoenixRed : BasePetCubSample
{
    public override int TargetHue => 1964;
    public override string DyeName => "Phoenix Red";
    public override string DefaultName => "Pet Dye Sample: Phoenix Red";

    [Constructible]
    public SamplePetCubPhoenixRed() : base(1964)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Aura Of Amber (1967)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubAuraOfAmber : BasePetCubPermanent
{
    public override int TargetHue => 1967;
    public override string DyeName => "Aura Of Amber";
    public override string DefaultName => "Pet Dye Tub: Aura Of Amber";

    [Constructible]
    public PetCubAuraOfAmber() : base(1967)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubAuraOfAmber : BasePetCubSample
{
    public override int TargetHue => 1967;
    public override string DyeName => "Aura Of Amber";
    public override string DefaultName => "Pet Dye Sample: Aura Of Amber";

    [Constructible]
    public SamplePetCubAuraOfAmber() : base(1967)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Deep Violet (1929)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubDeepViolet : BasePetCubPermanent
{
    public override int TargetHue => 1929;
    public override string DyeName => "Deep Violet";
    public override string DefaultName => "Pet Dye Tub: Deep Violet";

    [Constructible]
    public PetCubDeepViolet() : base(1929)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubDeepViolet : BasePetCubSample
{
    public override int TargetHue => 1929;
    public override string DyeName => "Deep Violet";
    public override string DefaultName => "Pet Dye Sample: Deep Violet";

    [Constructible]
    public SamplePetCubDeepViolet() : base(1929)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Polished Bronze (1944)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubPolishedBronze : BasePetCubPermanent
{
    public override int TargetHue => 1944;
    public override string DyeName => "Polished Bronze";
    public override string DefaultName => "Pet Dye Tub: Polished Bronze";

    [Constructible]
    public PetCubPolishedBronze() : base(1944)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubPolishedBronze : BasePetCubSample
{
    public override int TargetHue => 1944;
    public override string DyeName => "Polished Bronze";
    public override string DefaultName => "Pet Dye Sample: Polished Bronze";

    [Constructible]
    public SamplePetCubPolishedBronze() : base(1944)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Vibrant Crimson (1964)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubVibrantCrimson : BasePetCubPermanent
{
    public override int TargetHue => 1964;
    public override string DyeName => "Vibrant Crimson";
    public override string DefaultName => "Pet Dye Tub: Vibrant Crimson";

    [Constructible]
    public PetCubVibrantCrimson() : base(1964)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubVibrantCrimson : BasePetCubSample
{
    public override int TargetHue => 1964;
    public override string DyeName => "Vibrant Crimson";
    public override string DefaultName => "Pet Dye Sample: Vibrant Crimson";

    [Constructible]
    public SamplePetCubVibrantCrimson() : base(1964)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Lavender (1951)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubLavender : BasePetCubPermanent
{
    public override int TargetHue => 1951;
    public override string DyeName => "Lavender";
    public override string DefaultName => "Pet Dye Tub: Lavender";

    [Constructible]
    public PetCubLavender() : base(1951)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubLavender : BasePetCubSample
{
    public override int TargetHue => 1951;
    public override string DyeName => "Lavender";
    public override string DefaultName => "Pet Dye Sample: Lavender";

    [Constructible]
    public SamplePetCubLavender() : base(1951)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Gleaming Fuchsia (1930)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubGleamingFuchsia : BasePetCubPermanent
{
    public override int TargetHue => 1930;
    public override string DyeName => "Gleaming Fuchsia";
    public override string DefaultName => "Pet Dye Tub: Gleaming Fuchsia";

    [Constructible]
    public PetCubGleamingFuchsia() : base(1930)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubGleamingFuchsia : BasePetCubSample
{
    public override int TargetHue => 1930;
    public override string DyeName => "Gleaming Fuchsia";
    public override string DefaultName => "Pet Dye Sample: Gleaming Fuchsia";

    [Constructible]
    public SamplePetCubGleamingFuchsia() : base(1930)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Deep Blue (1939)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubDeepBlue : BasePetCubPermanent
{
    public override int TargetHue => 1939;
    public override string DyeName => "Deep Blue";
    public override string DefaultName => "Pet Dye Tub: Deep Blue";

    [Constructible]
    public PetCubDeepBlue() : base(1939)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubDeepBlue : BasePetCubSample
{
    public override int TargetHue => 1939;
    public override string DyeName => "Deep Blue";
    public override string DefaultName => "Pet Dye Sample: Deep Blue";

    [Constructible]
    public SamplePetCubDeepBlue() : base(1939)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Glossy Fuchsia (1919)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubGlossyFuchsia : BasePetCubPermanent
{
    public override int TargetHue => 1919;
    public override string DyeName => "Glossy Fuchsia";
    public override string DefaultName => "Pet Dye Tub: Glossy Fuchsia";

    [Constructible]
    public PetCubGlossyFuchsia() : base(1919)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubGlossyFuchsia : BasePetCubSample
{
    public override int TargetHue => 1919;
    public override string DyeName => "Glossy Fuchsia";
    public override string DefaultName => "Pet Dye Sample: Glossy Fuchsia";

    [Constructible]
    public SamplePetCubGlossyFuchsia() : base(1919)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Dark Void (2068)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubDarkVoid : BasePetCubPermanent
{
    public override int TargetHue => 2068;
    public override string DyeName => "Dark Void";
    public override string DefaultName => "Pet Dye Tub: Dark Void";

    [Constructible]
    public PetCubDarkVoid() : base(2068)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubDarkVoid : BasePetCubSample
{
    public override int TargetHue => 2068;
    public override string DyeName => "Dark Void";
    public override string DefaultName => "Pet Dye Sample: Dark Void";

    [Constructible]
    public SamplePetCubDarkVoid() : base(2068)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Murky Seagreen (1992)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubMurkySeagreen : BasePetCubPermanent
{
    public override int TargetHue => 1992;
    public override string DyeName => "Murky Seagreen";
    public override string DefaultName => "Pet Dye Tub: Murky Seagreen";

    [Constructible]
    public PetCubMurkySeagreen() : base(1992)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubMurkySeagreen : BasePetCubSample
{
    public override int TargetHue => 1992;
    public override string DyeName => "Murky Seagreen";
    public override string DefaultName => "Pet Dye Sample: Murky Seagreen";

    [Constructible]
    public SamplePetCubMurkySeagreen() : base(1992)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Reflective Shadow (1910)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubReflectiveShadow : BasePetCubPermanent
{
    public override int TargetHue => 1910;
    public override string DyeName => "Reflective Shadow";
    public override string DefaultName => "Pet Dye Tub: Reflective Shadow";

    [Constructible]
    public PetCubReflectiveShadow() : base(1910)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubReflectiveShadow : BasePetCubSample
{
    public override int TargetHue => 1910;
    public override string DyeName => "Reflective Shadow";
    public override string DefaultName => "Pet Dye Sample: Reflective Shadow";

    [Constructible]
    public SamplePetCubReflectiveShadow() : base(1910)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Liquid Sunshine (1923)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubLiquidSunshine : BasePetCubPermanent
{
    public override int TargetHue => 1923;
    public override string DyeName => "Liquid Sunshine";
    public override string DefaultName => "Pet Dye Tub: Liquid Sunshine";

    [Constructible]
    public PetCubLiquidSunshine() : base(1923)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubLiquidSunshine : BasePetCubSample
{
    public override int TargetHue => 1923;
    public override string DyeName => "Liquid Sunshine";
    public override string DefaultName => "Pet Dye Sample: Liquid Sunshine";

    [Constructible]
    public SamplePetCubLiquidSunshine() : base(1923)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Shadowy Blue (1960)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubShadowyBlue : BasePetCubPermanent
{
    public override int TargetHue => 1960;
    public override string DyeName => "Shadowy Blue";
    public override string DefaultName => "Pet Dye Tub: Shadowy Blue";

    [Constructible]
    public PetCubShadowyBlue() : base(1960)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubShadowyBlue : BasePetCubSample
{
    public override int TargetHue => 1960;
    public override string DyeName => "Shadowy Blue";
    public override string DefaultName => "Pet Dye Sample: Shadowy Blue";

    [Constructible]
    public SamplePetCubShadowyBlue() : base(1960)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Black And Green (1979)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubBlackAndGreen : BasePetCubPermanent
{
    public override int TargetHue => 1979;
    public override string DyeName => "Black And Green";
    public override string DefaultName => "Pet Dye Tub: Black And Green";

    [Constructible]
    public PetCubBlackAndGreen() : base(1979)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubBlackAndGreen : BasePetCubSample
{
    public override int TargetHue => 1979;
    public override string DyeName => "Black And Green";
    public override string DefaultName => "Pet Dye Sample: Black And Green";

    [Constructible]
    public SamplePetCubBlackAndGreen() : base(1979)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Glossy Blue (1916)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubGlossyBlue : BasePetCubPermanent
{
    public override int TargetHue => 1916;
    public override string DyeName => "Glossy Blue";
    public override string DefaultName => "Pet Dye Tub: Glossy Blue";

    [Constructible]
    public PetCubGlossyBlue() : base(1916)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubGlossyBlue : BasePetCubSample
{
    public override int TargetHue => 1916;
    public override string DyeName => "Glossy Blue";
    public override string DefaultName => "Pet Dye Sample: Glossy Blue";

    [Constructible]
    public SamplePetCubGlossyBlue() : base(1916)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Hunter Green (1936)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubHunterGreen : BasePetCubPermanent
{
    public override int TargetHue => 1936;
    public override string DyeName => "Hunter Green";
    public override string DefaultName => "Pet Dye Tub: Hunter Green";

    [Constructible]
    public PetCubHunterGreen() : base(1936)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubHunterGreen : BasePetCubSample
{
    public override int TargetHue => 1936;
    public override string DyeName => "Hunter Green";
    public override string DefaultName => "Pet Dye Sample: Hunter Green";

    [Constructible]
    public SamplePetCubHunterGreen() : base(1936)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Slate Blue (1983)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubSlateBlue : BasePetCubPermanent
{
    public override int TargetHue => 1983;
    public override string DyeName => "Slate Blue";
    public override string DefaultName => "Pet Dye Tub: Slate Blue";

    [Constructible]
    public PetCubSlateBlue() : base(1983)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubSlateBlue : BasePetCubSample
{
    public override int TargetHue => 1983;
    public override string DyeName => "Slate Blue";
    public override string DefaultName => "Pet Dye Sample: Slate Blue";

    [Constructible]
    public SamplePetCubSlateBlue() : base(1983)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Mother Of Pearl (2720)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubMotherOfPearl : BasePetCubPermanent
{
    public override int TargetHue => 2720;
    public override string DyeName => "Mother Of Pearl";
    public override string DefaultName => "Pet Dye Tub: Mother Of Pearl";

    [Constructible]
    public PetCubMotherOfPearl() : base(2720)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubMotherOfPearl : BasePetCubSample
{
    public override int TargetHue => 2720;
    public override string DyeName => "Mother Of Pearl";
    public override string DefaultName => "Pet Dye Sample: Mother Of Pearl";

    [Constructible]
    public SamplePetCubMotherOfPearl() : base(2720)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Star Blue (2723)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubStarBlue : BasePetCubPermanent
{
    public override int TargetHue => 2723;
    public override string DyeName => "Star Blue";
    public override string DefaultName => "Pet Dye Tub: Star Blue";

    [Constructible]
    public PetCubStarBlue() : base(2723)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubStarBlue : BasePetCubSample
{
    public override int TargetHue => 2723;
    public override string DyeName => "Star Blue";
    public override string DefaultName => "Pet Dye Sample: Star Blue";

    [Constructible]
    public SamplePetCubStarBlue() : base(2723)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Murky Amber (1989)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubMurkyAmber : BasePetCubPermanent
{
    public override int TargetHue => 1989;
    public override string DyeName => "Murky Amber";
    public override string DefaultName => "Pet Dye Tub: Murky Amber";

    [Constructible]
    public PetCubMurkyAmber() : base(1989)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubMurkyAmber : BasePetCubSample
{
    public override int TargetHue => 1989;
    public override string DyeName => "Murky Amber";
    public override string DefaultName => "Pet Dye Sample: Murky Amber";

    [Constructible]
    public SamplePetCubMurkyAmber() : base(1989)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Vibran Seagreen (1970)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubVibranSeagreen : BasePetCubPermanent
{
    public override int TargetHue => 1970;
    public override string DyeName => "Vibran Seagreen";
    public override string DefaultName => "Pet Dye Tub: Vibran Seagreen";

    [Constructible]
    public PetCubVibranSeagreen() : base(1970)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubVibranSeagreen : BasePetCubSample
{
    public override int TargetHue => 1970;
    public override string DyeName => "Vibran Seagreen";
    public override string DefaultName => "Pet Dye Sample: Vibran Seagreen";

    [Constructible]
    public SamplePetCubVibranSeagreen() : base(1970)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Vibrant Ocher (2725)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubVibrantOcher : BasePetCubPermanent
{
    public override int TargetHue => 2725;
    public override string DyeName => "Vibrant Ocher";
    public override string DefaultName => "Pet Dye Tub: Vibrant Ocher";

    [Constructible]
    public PetCubVibrantOcher() : base(2725)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubVibrantOcher : BasePetCubSample
{
    public override int TargetHue => 2725;
    public override string DyeName => "Vibrant Ocher";
    public override string DefaultName => "Pet Dye Sample: Vibrant Ocher";

    [Constructible]
    public SamplePetCubVibrantOcher() : base(2725)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Mossy Green (2684)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubMossyGreen : BasePetCubPermanent
{
    public override int TargetHue => 2684;
    public override string DyeName => "Mossy Green";
    public override string DefaultName => "Pet Dye Tub: Mossy Green";

    [Constructible]
    public PetCubMossyGreen() : base(2684)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubMossyGreen : BasePetCubSample
{
    public override int TargetHue => 2684;
    public override string DyeName => "Mossy Green";
    public override string DefaultName => "Pet Dye Sample: Mossy Green";

    [Constructible]
    public SamplePetCubMossyGreen() : base(2684)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Olive Green (2709)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubOliveGreen : BasePetCubPermanent
{
    public override int TargetHue => 2709;
    public override string DyeName => "Olive Green";
    public override string DefaultName => "Pet Dye Tub: Olive Green";

    [Constructible]
    public PetCubOliveGreen() : base(2709)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubOliveGreen : BasePetCubSample
{
    public override int TargetHue => 2709;
    public override string DyeName => "Olive Green";
    public override string DefaultName => "Pet Dye Sample: Olive Green";

    [Constructible]
    public SamplePetCubOliveGreen() : base(2709)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Mottled Sunset Blue (2714)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubMottledSunsetBlue : BasePetCubPermanent
{
    public override int TargetHue => 2714;
    public override string DyeName => "Mottled Sunset Blue";
    public override string DefaultName => "Pet Dye Tub: Mottled Sunset Blue";

    [Constructible]
    public PetCubMottledSunsetBlue() : base(2714)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubMottledSunsetBlue : BasePetCubSample
{
    public override int TargetHue => 2714;
    public override string DyeName => "Mottled Sunset Blue";
    public override string DefaultName => "Pet Dye Sample: Mottled Sunset Blue";

    [Constructible]
    public SamplePetCubMottledSunsetBlue() : base(2714)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Tyrian Purple (2716)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubTyrianPurple : BasePetCubPermanent
{
    public override int TargetHue => 2716;
    public override string DyeName => "Tyrian Purple";
    public override string DefaultName => "Pet Dye Tub: Tyrian Purple";

    [Constructible]
    public PetCubTyrianPurple() : base(2716)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubTyrianPurple : BasePetCubSample
{
    public override int TargetHue => 2716;
    public override string DyeName => "Tyrian Purple";
    public override string DefaultName => "Pet Dye Sample: Tyrian Purple";

    [Constructible]
    public SamplePetCubTyrianPurple() : base(2716)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// Intense Teal (2691)
[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class PetCubIntenseTeal : BasePetCubPermanent
{
    public override int TargetHue => 2691;
    public override string DyeName => "Intense Teal";
    public override string DefaultName => "Pet Dye Tub: Intense Teal";

    [Constructible]
    public PetCubIntenseTeal() : base(2691)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class SamplePetCubIntenseTeal : BasePetCubSample
{
    public override int TargetHue => 2691;
    public override string DyeName => "Intense Teal";
    public override string DefaultName => "Pet Dye Sample: Intense Teal";

    [Constructible]
    public SamplePetCubIntenseTeal() : base(2691)
    {
    }

    // Legacy hook: version 0 held no payload beyond the base class.
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
