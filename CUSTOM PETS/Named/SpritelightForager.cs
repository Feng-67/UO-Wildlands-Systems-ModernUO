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
public partial class SpritelightForager : BaseMount
{
    [Constructible]
    public SpritelightForager() : base(1652, 16095, AIType.AI_Animal, FightMode.Aggressor)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 1652; // BaseMount doesn't set Body from the item ID
        BaseSoundID = 0xA8;

        SetStr(460);
        SetDex(180);
        SetInt(300);

        SetHits(410);
        SetStam(150);
        SetMana(300);

        // --- Damage Profile ---
        SetDamage(18, 26);
        SetDamageType(ResistanceType.Poison, 100);

        // --- Resistances ---
        SetResistance(ResistanceType.Physical, 60);
        SetResistance(ResistanceType.Fire, 40);
        SetResistance(ResistanceType.Cold, 30);
        SetResistance(ResistanceType.Poison, 90);
        SetResistance(ResistanceType.Energy, 50);

        // --- Skills ---
        SetSkill(SkillName.Wrestling, 50.0, 70.0);
        SetSkill(SkillName.Tactics, 50.0, 70.0);
        SetSkill(SkillName.MagicResist, 77.5, 100.0);
        SetSkill(SkillName.Anatomy, 50.0, 70.0);

        // --- Taming ---
        Tamable = true;
        ControlSlots = 2;
        MinTameSkill = 88.0;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a spritelight forager corpse";
    public override string DefaultName => "a spritelight forager";

    public override FoodType FavoriteFood => FoodType.Meat;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(SpritelightForager),
            Class.None,
            MagicalAbility.Chivalry |
            MagicalAbility.MageryMastery |
            MagicalAbility.Mysticism |
            MagicalAbility.Poisoning |
            MagicalAbility.Spellweaving |
            MagicalAbility.Bushido |
            MagicalAbility.Ninjitsu |
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
                SpecialAbility.StickySkin,
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
