/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class UberTurkey : BaseCreature
{
    // Not serialized: reset on load
    private DateTime _nextGobble;

    [Constructible]
    public UberTurkey() : this(true)
    {
    }

    [Constructible]
    public UberTurkey(bool tamable) : base(AIType.AI_Animal, FightMode.Aggressor)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 1026;
        BaseSoundID = 0x66A;

        // 50% chance for a special hue, 50% chance for default (0)
        Hue = Utility.RandomBool()
            ? Utility.RandomList(2101, 1150, 1109, 1901, 1143, 2012)
            : 0;

        Tamable = tamable;
        ControlSlots = 3;
        MinTameSkill = 108.0;

        SetStr(350, 450);
        SetDex(120, 160);
        SetInt(75, 125);

        SetHits(300, 400);
        SetStam(120, 160);
        SetMana(75, 125);

        SetDamage(14, 20);
        SetDamageType(ResistanceType.Physical, 100);

        SetResistance(ResistanceType.Physical, 60, 70);
        SetResistance(ResistanceType.Fire, 30, 40);
        SetResistance(ResistanceType.Cold, 25, 35);
        SetResistance(ResistanceType.Poison, 35, 45);
        SetResistance(ResistanceType.Energy, 30, 40);

        SetSkill(SkillName.Wrestling, 90.1, 105.0);
        SetSkill(SkillName.Tactics, 85.0, 100.0);
        SetSkill(SkillName.MagicResist, 65.0, 80.0);
        SetSkill(SkillName.Anatomy, 40.0, 60.0);

        Fame = 6500;
        Karma = 0;

        _nextGobble = Core.Now;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a turkey corpse";
    public override string DefaultName => "an Uber Turkey";

    public override bool CanAngerOnTame => true;
    public override int Meat => 4;
    public override MeatType MeatType => MeatType.Bird;
    public override FoodType FavoriteFood => FoodType.Meat;
    public override int Feathers => 50;

    public override int GetIdleSound() => 0x66A;
    public override int GetAngerSound() => 0x66A;
    public override int GetHurtSound() => 0x66B;
    public override int GetDeathSound() => 0x66B;

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        _nextGobble = Core.Now;

        // TrainingDefinition isn't serialized; rebuild on load.
        ApplyTrainingDefinition();
    }

    // Gobble mechanic
    public override void OnThink()
    {
        base.OnThink();

        if (Tamable && !Controlled && _nextGobble < Core.Now)
        {
            Say(1153511); // *gobble* *gobble*
            PlaySound(GetIdleSound());

            _nextGobble = Core.Now + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 240));
        }
    }

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(UberTurkey),
            Class.None,
            MagicalAbility.Poisoning |
            MagicalAbility.WrestlingMastery,
            new[]
            {
                SpecialAbility.GraspingClaw,
                SpecialAbility.ManaDrain,
                SpecialAbility.Repel,
                SpecialAbility.SearingWounds,
                SpecialAbility.TailSwipe,
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
                // WeaponAbility.Disarm,
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
                AreaEffect.EssenceOfEarth,
                AreaEffect.AuraOfNausea,
                AreaEffect.PoisonBreath
            },
            3,
            5
        );
    }
}
