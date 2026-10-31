/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class JuvenileManticore : BaseMount
{
    [Constructible]
    public JuvenileManticore() : base(1553, 16091, AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0x27D;

        Body = 1553; // BaseMount doesn't set Body from the item ID

        // --- Random Hue Assignment ---
        var roll = Utility.RandomDouble();

        Hue = roll switch
        {
            < 0.005 => 2767,                           // 0.5% Uber Rare (Blue-Green)
            < 0.02 => 2770,                           // 1.5% Extremely Rare (Lavender)
            < 0.05 => 2766,                           // 3% Very Rare (Pink)
            < 0.15 => Utility.RandomList(2768, 2592), // 10% Rare (Decay, Metallic Blue)
            < 0.30 => Utility.RandomList(2762, 2765), // 15% More Uncommon (Orange, Cocoa)
            < 0.50 => Utility.RandomList(2764, 1330), // 20% Uncommon (Shogun Grey, Light Blue)
            < 0.75 => Utility.RandomList(2763, 2761), // 25% Common (Tan, Brown)
            _ => Utility.RandomBool() ? 0 : 2769 // 25% Very Common (White or Off-White)
        };

        // --- Taming ---
        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108.0;

        // --- Attributes ---
        SetStr(551, 600);
        SetDex(151, 180);
        SetInt(301, 350);

        SetHits(451, 525);
        SetStam(151, 180);
        SetMana(301, 350);

        // --- Damage Profile (100% Physical) ---
        SetDamage(14, 26);
        SetDamageType(ResistanceType.Physical, 100);

        // --- Resistances ---
        SetResistance(ResistanceType.Physical, 60, 75);
        SetResistance(ResistanceType.Fire, 55, 65);
        SetResistance(ResistanceType.Cold, 55, 65);
        SetResistance(ResistanceType.Poison, 60, 70);
        SetResistance(ResistanceType.Energy, 60, 70);

        // --- Skills ---
        SetSkill(SkillName.Wrestling, 100.1, 115.0);
        SetSkill(SkillName.Tactics, 100.1, 115.0);
        SetSkill(SkillName.MagicResist, 80.0, 100.0);
        SetSkill(SkillName.Anatomy, 50.0, 70.0);

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a manticore corpse";
    public override string DefaultName => "Juvenile Manticore";

    public override bool CanAngerOnTame => true;
    public override int Meat => 3;
    public override int Hides => 10;
    public override FoodType FavoriteFood => FoodType.Meat;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(JuvenileManticore),
            Class.None,
            MagicalAbility.Chivalry |
            MagicalAbility.MageryMastery |
            MagicalAbility.Mysticism |
            MagicalAbility.Necromage |
            MagicalAbility.Necromancy |
            MagicalAbility.Poisoning |
            MagicalAbility.Spellweaving |
            MagicalAbility.Bushido |
            MagicalAbility.Ninjitsu |
            MagicalAbility.WrestlingMastery,
            new[]
            {
                SpecialAbility.AngryFire,
                SpecialAbility.ConductiveBlast,
                SpecialAbility.DragonBreath,
                SpecialAbility.GraspingClaw,
                SpecialAbility.Inferno,
                SpecialAbility.LifeLeech,
                SpecialAbility.LightningForce,
                SpecialAbility.ManaDrain,
                SpecialAbility.RagingBreath,
                SpecialAbility.Repel,
                SpecialAbility.SearingWounds,
                SpecialAbility.StealLife,
                SpecialAbility.VenomousBite,
                SpecialAbility.ViciousBite
            },
            new[]
            {
                WeaponAbility.ArmorIgnore,
                WeaponAbility.ArmorPierce,
                WeaponAbility.Bladeweave,
                WeaponAbility.BleedAttack,
                WeaponAbility.Block,
                // WeaponAbility.ColdWind, // not in ModernUO
                WeaponAbility.ConcussionBlow,
                WeaponAbility.CrushingBlow,
                WeaponAbility.Disarm,
                WeaponAbility.Dismount,
                WeaponAbility.Feint,
                WeaponAbility.ForceOfNature,
                WeaponAbility.FrenziedWhirlwind,
                WeaponAbility.MortalStrike,
                WeaponAbility.NerveStrike,
                WeaponAbility.ParalyzingBlow,
                WeaponAbility.PsychicAttack,
                WeaponAbility.TalonStrike
            },
            new[]
            {
                AreaEffect.AuraOfEnergy,
                AreaEffect.ExplosiveGoo,
                AreaEffect.EssenceOfEarth,
                AreaEffect.AuraOfNausea,
                AreaEffect.PoisonBreath,
                AreaEffect.EssenceOfDisease
            },
            3,
            5
        );
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        // TrainingDefinition isn't serialized; rebuild on load.
        ApplyTrainingDefinition();
    }
}
