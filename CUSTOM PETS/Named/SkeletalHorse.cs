/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team] (Original Author Rutibex)
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class SkeletalHorse : BaseMount
{
    [Constructible]
    public SkeletalHorse() : base(793, 0x3EBB, AIType.AI_Animal, FightMode.Aggressor)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 793; // BaseMount doesn't set Body from the item ID

        // --- Taming ---
        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108.0;

        // --- Attributes ---
        SetStr(400, 500);
        SetDex(100, 125);
        SetInt(150, 250);

        SetHits(450, 600);
        SetStam(100, 125);
        SetMana(150, 250);

        // --- Damage Profile ---
        SetDamage(18, 24);

        SetDamageType(ResistanceType.Physical, 20);
        SetDamageType(ResistanceType.Fire, 20);
        SetDamageType(ResistanceType.Cold, 20);
        SetDamageType(ResistanceType.Poison, 20);
        SetDamageType(ResistanceType.Energy, 20);

        // --- Resistances ---
        SetResistance(ResistanceType.Physical, 55, 65);
        SetResistance(ResistanceType.Fire, 40, 50);
        SetResistance(ResistanceType.Cold, 50, 60);
        SetResistance(ResistanceType.Poison, 40, 50);
        SetResistance(ResistanceType.Energy, 40, 50);

        // --- Skills ---
        SetSkill(SkillName.Wrestling, 90.1, 105.0);
        SetSkill(SkillName.Tactics, 90.0, 100.0);
        SetSkill(SkillName.MagicResist, 80.0, 95.0);
        SetSkill(SkillName.Anatomy, 60.0, 80.0);

        // Innate: Necromancy (Enhanced)
        SetSkill(SkillName.Necromancy, 80.0, 100.0);
        SetSkill(SkillName.SpiritSpeak, 80.0, 100.0);

        Fame = 12000;
        Karma = -12000;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a skeletal horse corpse";
    public override string DefaultName => "Skeletal Horse";

    public override bool CanAngerOnTame => true;
    public override int Meat => 3;
    public override int Hides => 10;
    public override FoodType FavoriteFood => FoodType.Meat;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(SkeletalHorse),
            Class.None,
            MagicalAbility.Mysticism |
            MagicalAbility.Necromage |
            MagicalAbility.Necromancy |
            MagicalAbility.Poisoning |
            MagicalAbility.WrestlingMastery,
            new[]
            {
                SpecialAbility.AngryFire,
                SpecialAbility.ConductiveBlast,
                SpecialAbility.DragonBreath,
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
