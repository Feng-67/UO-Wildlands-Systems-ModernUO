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
public partial class KurakuDread : BaseCreature
{
    [Constructible]
    public KurakuDread() : base(AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 1400;
        Hue = Utility.RandomList(1143, 2012, 1437, 2419);
        BaseSoundID = 362;

        // --- Taming ---
        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108.0;

        // --- Attributes ---
        SetStr(700, 825);
        SetDex(190, 220);
        SetInt(50, 100);

        SetHits(500, 700);

        // --- Damage Profile ---
        SetDamage(25, 35);
        SetDamageType(ResistanceType.Physical, 100);

        // --- Resistances ---
        SetResistance(ResistanceType.Physical, 75, 85);
        SetResistance(ResistanceType.Fire, 50, 60);
        SetResistance(ResistanceType.Cold, 30, 40);
        SetResistance(ResistanceType.Poison, 40, 50);
        SetResistance(ResistanceType.Energy, 40, 50);

        // --- Skills ---
        SetSkill(SkillName.Anatomy, 100.0);
        SetSkill(SkillName.MagicResist, 110.0, 130.0);
        SetSkill(SkillName.Tactics, 110.0, 130.0);
        SetSkill(SkillName.Wrestling, 120.0, 140.0);

        Fame = 22000;
        Karma = -22000;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a kuraku dread corpse";
    public override string DefaultName => "a kuraku dread";

    public override bool AutoDispel => !Controlled;
    public override Poison PoisonImmune => Poison.Lethal;
    public override bool CanAngerOnTame => true;
    public override bool CanFlee => false;
    public override FoodType FavoriteFood => FoodType.Meat;

    // ModernUO: SetWeaponAbility replaced by GetWeaponAbility override.
    public override WeaponAbility GetWeaponAbility() => WeaponAbility.ArmorIgnore;

    // ModernUO: SetSpecialAbility replaced by GetMonsterAbilities.
    private static readonly MonsterAbility[] _abilities = { SpecialAbility.TailSwipe };

    public override MonsterAbility[] GetMonsterAbilities() => _abilities;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(KurakuDread),
            Class.None,
            MagicalAbility.Poisoning |
            MagicalAbility.Bushido |
            MagicalAbility.Ninjitsu |
            MagicalAbility.WrestlingMastery,
            new[]
            {
                SpecialAbility.GraspingClaw,
                SpecialAbility.RagingBreath,
                SpecialAbility.Repel,
                SpecialAbility.RuneCorruption,
                SpecialAbility.SearingWounds,
                SpecialAbility.StickySkin,
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
