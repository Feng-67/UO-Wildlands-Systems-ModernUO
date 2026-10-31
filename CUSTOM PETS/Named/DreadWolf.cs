/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class DreadWolf : BaseMount
{
    [Constructible]
    public DreadWolf() : base(1410, 16076, AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0xA8;

        SetStr(500, 555);
        SetDex(85, 125);
        SetInt(50, 60);

        SetHits(625, 700);

        SetDamage(15, 20);

        SetDamageType(ResistanceType.Physical, 20);
        SetDamageType(ResistanceType.Poison, 80);

        SetResistance(ResistanceType.Physical, 60, 70);
        SetResistance(ResistanceType.Fire, 20, 40);
        SetResistance(ResistanceType.Cold, 55, 65);
        SetResistance(ResistanceType.Poison, 100);
        SetResistance(ResistanceType.Energy, 25, 45);

        SetSkill(SkillName.MagicResist, 125.0, 150.0);
        SetSkill(SkillName.Tactics, 80.0, 95.0);
        SetSkill(SkillName.Wrestling, 80.0, 100.0);
        SetSkill(SkillName.Poisoning, 90.0, 100.0);
        SetSkill(SkillName.DetectHidden, 45.0, 55.0);

        Fame = 15000;
        Karma = -15000;

        // ModernUO: SetMagicalAbility replaced by direct profile assignment
        PetTrainingHelper.GetAbilityProfile(this, create: true).MagicalAbility = MagicalAbility.Poisoning;

        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a dread wolf corpse";
    public override string DefaultName => "Dread Wolf";

    public override Poison PoisonImmune => Poison.Lethal;
    public override bool CanAngerOnTame => true;
    public override int Meat => 3;
    public override int Hides => 10;
    public override FoodType FavoriteFood => FoodType.Meat;

    public override int GetIdleSound() => 0x577;
    public override int GetAttackSound() => 0x576;
    public override int GetAngerSound() => 0x578;
    public override int GetHurtSound() => 0x576;
    public override int GetDeathSound() => 0x579;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(DreadWolf),
            Class.None,
            MagicalAbility.Necromage |
            MagicalAbility.Necromancy |
            MagicalAbility.Poisoning |
            MagicalAbility.WrestlingMastery,
            new[]
            {
                SpecialAbility.LifeLeech,
                SpecialAbility.ManaDrain,
                SpecialAbility.Repel,
                SpecialAbility.SearingWounds,
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
