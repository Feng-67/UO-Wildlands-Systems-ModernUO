/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author JesteR)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class SaintBernard : BaseMount
{
    [Constructible]
    public SaintBernard() : base(0xE2, 0x3EA0, AIType.AI_Animal, FightMode.Aggressor)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 0x60D;
        ItemID = 0x3ED7;
        Hue = 0;

        SetStr(650);
        SetDex(190);
        SetInt(125);

        SetHits(600);
        SetMana(475);

        SetDamage(20, 25);

        SetDamageType(ResistanceType.Physical, 50);
        SetDamageType(ResistanceType.Fire, 50);

        SetResistance(ResistanceType.Physical, 70, 85);
        SetResistance(ResistanceType.Fire, 70, 85);
        SetResistance(ResistanceType.Cold, 25, 45);
        SetResistance(ResistanceType.Poison, 50, 60);
        SetResistance(ResistanceType.Energy, 40, 50);

        SetSkill(SkillName.Wrestling, 90.1, 105.8);
        SetSkill(SkillName.Tactics, 89.3, 98.3);
        SetSkill(SkillName.Parry, 55.5, 70.4);
        SetSkill(SkillName.MagicResist, 59.3, 69.0);
        SetSkill(SkillName.Anatomy, 55.5, 70.4);
        SetSkill(SkillName.Healing, 72.2, 98.9);
        SetSkill(SkillName.Discordance, 72.2, 98.9);

        Fame = 24000;
        Karma = -24000;

        Tamable = true;
        ControlSlots = 3;
        ControlSlotsMax = 5;
        MinTameSkill = 100.0;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a corpse";
    public override string DefaultName => "Saint Bernard";

    public override FoodType FavoriteFood => FoodType.Meat;

    public override int GetAngerSound() => 0x85;
    public override int GetIdleSound() => 0x85;
    public override int GetAttackSound() => 0x85;
    public override int GetHurtSound() => 0x84;
    public override int GetDeathSound() => 0x84;

    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(SaintBernard),
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
        ApplyTrainingDefinition();
    }
}
