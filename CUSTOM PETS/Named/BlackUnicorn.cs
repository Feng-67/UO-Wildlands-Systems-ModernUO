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
public partial class BlackUnicorn : BaseMount
{
    [Constructible]
    public BlackUnicorn() : base(0x7A, 0x3EB4, AIType.AI_Necro, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0x4BC; // Unicorn sounds
        Hue = 1;             // Pure black

        Body = 0x7A; // BaseMount doesn't set Body from the item ID

        SetStr(1300, 1400);
        SetDex(220, 240);
        SetInt(500, 950);

        SetHits(956, 990);

        SetDamage(21, 28);

        SetDamageType(ResistanceType.Physical, 25);
        SetDamageType(ResistanceType.Poison, 75);

        SetResistance(ResistanceType.Physical, 55, 65);
        SetResistance(ResistanceType.Fire, 50, 60);
        SetResistance(ResistanceType.Cold, 30, 40);
        SetResistance(ResistanceType.Poison, 60, 70);
        SetResistance(ResistanceType.Energy, 35, 45);

        SetSkill(SkillName.SpiritSpeak, 30.1, 40.0);
        SetSkill(SkillName.Necromancy, 50.1, 75.0);
        SetSkill(SkillName.MagicResist, 99.1, 110.0);
        SetSkill(SkillName.Tactics, 97.6, 100.0);
        SetSkill(SkillName.Wrestling, 90.1, 92.5);

        Fame = 15000;
        Karma = -50000;

        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a black unicorn corpse";
    public override string DefaultName => "Black Unicorn";

    // Immune to all poisons except Lethal
    public override Poison PoisonImmune => Poison.Deadly;

    public override bool CanAngerOnTame => true;
    public override bool StatLossAfterTame => true;
    public override int Meat => 3;
    public override FoodType FavoriteFood => FoodType.Meat;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(BlackUnicorn),
            Class.None,
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
                WeaponAbility.Disarm,
                WeaponAbility.Dismount,
                WeaponAbility.Feint,
                WeaponAbility.ForceOfNature,
                WeaponAbility.FrenziedWhirlwind,
                WeaponAbility.MortalStrike,
                WeaponAbility.NerveStrike,
                WeaponAbility.ParalyzingBlow,
                WeaponAbility.PsychicAttack,
                // WeaponAbility.TalonStrike,
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
