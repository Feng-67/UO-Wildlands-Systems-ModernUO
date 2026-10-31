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
public partial class RainbowUnicorn : BaseMount
{
    [Constructible]
    public RainbowUnicorn() : base(1407, 16075, AIType.AI_Mage, FightMode.Aggressor)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0x4BC; // Unicorn sounds

        Body = 1407; // BaseMount doesn't set Body from the item ID

        SetStr(650, 700);
        SetDex(110, 120);
        SetInt(250, 475);

        SetHits(478, 495);

        SetDamage(16, 22);

        SetDamageType(ResistanceType.Physical, 100);

        SetResistance(ResistanceType.Physical, 55, 65);
        SetResistance(ResistanceType.Fire, 60, 70);
        SetResistance(ResistanceType.Cold, 30, 40);
        SetResistance(ResistanceType.Poison, 25, 35);
        SetResistance(ResistanceType.Energy, 35, 45);

        SetSkill(SkillName.EvalInt, 30.1, 40.0);
        SetSkill(SkillName.Magery, 30.1, 40.0);
        SetSkill(SkillName.MagicResist, 99.1, 110.0);
        SetSkill(SkillName.Tactics, 97.6, 100.0);
        SetSkill(SkillName.Wrestling, 90.1, 92.5);

        Fame = 9000;
        Karma = 9000;

        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a rainbow unicorn corpse";
    public override string DefaultName => "Rainbow Unicorn";

    public override Poison PoisonImmune => Poison.Lethal;
    public override bool CanAngerOnTame => true;
    public override FoodType FavoriteFood => FoodType.Meat;
    public override int Meat => 3;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(RainbowUnicorn),
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
                SpecialAbility.Inferno,
                SpecialAbility.LightningForce,
                SpecialAbility.ManaDrain,
                SpecialAbility.RagingBreath,
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
