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
public partial class ClydesdaleHorse : BaseMount
{
    [Constructible]
    public ClydesdaleHorse() : base(1651, 16094, AIType.AI_Animal, FightMode.Aggressor)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0xA8;

        Body = 1651; // BaseMount doesn't set Body from the item ID

        var roll = Utility.RandomDouble();

        Hue = roll switch
        {
            < 0.01 => Utility.RandomList(2775, 2781),       // 1% Uber Rare
            < 0.05 => Utility.RandomList(2075, 2753),       // 4% Very Rare
            < 0.15 => Utility.RandomList(2659, 2605),       // 10% Rare
            < 0.40 => Utility.RandomList(2500, 2678, 2120), // 25% Uncommon
            < 0.70 => Utility.RandomList(2761, 2763),       // 30% Common
            _ => 0                                     // 30% Very Common
        };

        SetStr(302, 420);
        SetDex(131, 185);
        SetInt(101, 150);

        SetHits(151, 200);
        SetStam(131, 150);
        SetMana(101, 150);

        SetDamage(14, 20);
        SetDamageType(ResistanceType.Physical, 100);

        SetResistance(ResistanceType.Physical, 40, 50);
        SetResistance(ResistanceType.Fire, 35, 45);
        SetResistance(ResistanceType.Cold, 30, 40);
        SetResistance(ResistanceType.Poison, 30, 40);
        SetResistance(ResistanceType.Energy, 30, 40);

        SetSkill(SkillName.Wrestling, 50.0, 70.0);
        SetSkill(SkillName.Tactics, 50.0, 70.0);
        SetSkill(SkillName.MagicResist, 77.5, 100.0);
        SetSkill(SkillName.Anatomy, 50.0, 70.0);

        Tamable = true;
        ControlSlots = 1;
        MinTameSkill = 94.0;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a clydesdale corpse";
    public override string DefaultName => "a clydesdale horse";

    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(ClydesdaleHorse),
            Class.None,
            MagicalAbility.Chivalry |
            MagicalAbility.MageryMastery |
            MagicalAbility.Mysticism |
            MagicalAbility.Necromage |
            MagicalAbility.Necromancy |
            MagicalAbility.Poisoning |
            MagicalAbility.Spellweaving |
            MagicalAbility.Bashing |
            MagicalAbility.BattleDefense |
            MagicalAbility.Piercing |
            MagicalAbility.Slashing |
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
        ApplyTrainingDefinition();
    }
}
