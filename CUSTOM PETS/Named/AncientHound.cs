/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Author Dragonslayer2)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class AncientHound : BaseMount
{
    [Constructible]
    public AncientHound() : base(0x42D, 0x3EC9, AIType.AI_Melee, FightMode.Aggressor)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0xE5; // Grey Wolf sounds

        Body = 0x42D; // BaseMount doesn't set Body from the item ID

        SetStr(1200, 1300);
        SetDex(284, 384);
        SetInt(226, 250);

        SetHits(1200, 1250);

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
        SetSkill(SkillName.MagicResist, 59.3, 69.0);
        SetSkill(SkillName.Anatomy, 55.5, 70.4);
        SetSkill(SkillName.Healing, 72.2, 98.9);

        Fame = 24000;
        Karma = -24000;

        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "an ancient hound corpse";
    public override string DefaultName => "Ancient Hound";       
    public override FoodType FavoriteFood => FoodType.Meat;
    public override bool CanAngerOnTame => true;
    public override bool StatLossAfterTame => true;
    public override int Hides => 20;
    public override int Meat => 16;
    public override HideType HideType => HideType.Horned;

    public override void OnAfterTame(Mobile tamer)
    {
        if (Owners.Count == 0 && PetTrainingHelper.Enabled)
        {
            if (RawStr > 0)
            {
                RawStr = (int)Math.Max(1, RawStr * 0.5);
            }

            if (RawDex > 0)
            {
                RawDex = (int)Math.Max(1, RawDex * 0.5);
            }

            if (HitsMaxSeed > 0)
            {
                HitsMaxSeed = (int)Math.Max(1, HitsMaxSeed * 0.5);
            }

            Hits = Math.Min(HitsMaxSeed, Hits);
            Stam = Math.Min(RawDex, Stam);
        }
        else
        {
            base.OnAfterTame(tamer);
        }
    }

    public override int GetIdleSound() => 0xE5;
    public override int GetAttackSound() => 0xE5;
    public override int GetAngerSound() => 0xE5;
    public override int GetHurtSound() => 0xE5;
    public override int GetDeathSound() => 0xE5;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(AncientHound),
            Class.None,
            MagicalAbility.Chivalry |
            MagicalAbility.MageryMastery |
            MagicalAbility.Mysticism |
            MagicalAbility.Poisoning |
            MagicalAbility.Spellweaving |
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
