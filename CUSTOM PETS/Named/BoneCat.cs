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
public partial class BoneCat : BaseMount
{
    [Constructible]
    public BoneCat() : base(1441, 16080, AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0x69;

        Body = 1441; // BaseMount doesn't set Body from the item ID

        SetStr(200, 280);
        SetDex(150, 200);
        SetInt(50, 100);

        SetHits(280, 360);

        SetDamage(10, 16);

        SetDamageType(ResistanceType.Physical, 40);
        SetDamageType(ResistanceType.Fire, 60);

        SetResistance(ResistanceType.Physical, 35, 45);
        SetResistance(ResistanceType.Fire, 70, 80);
        SetResistance(ResistanceType.Cold, 10, 20);
        SetResistance(ResistanceType.Poison, 20, 30);
        SetResistance(ResistanceType.Energy, 25, 35);

        SetSkill(SkillName.MagicResist, 70.0, 90.0);
        SetSkill(SkillName.Tactics, 65.0, 80.0);
        SetSkill(SkillName.Wrestling, 60.0, 75.0);
        SetSkill(SkillName.Necromancy, 30.0, 45.0);
        SetSkill(SkillName.SpiritSpeak, 30.0, 45.0);

        Fame = 5000;
        Karma = -5000;

        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a bone cat corpse";
    public override string DefaultName => "Bone Cat";

    public override bool CanAngerOnTame => true;
    public override int Meat => 3;
    public override int Hides => 10;
    public override FoodType FavoriteFood => FoodType.Meat;

    // ModernUO: SetSpecialAbility replaced by GetMonsterAbilities.
    private static readonly MonsterAbility[] _abilities = { SpecialAbility.DragonBreath };

    public override MonsterAbility[] GetMonsterAbilities() => _abilities;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(BoneCat),
            Class.None,
            MagicalAbility.Necromage |
            MagicalAbility.Necromancy |
            MagicalAbility.Poisoning |
            MagicalAbility.WrestlingMastery,
            new[]
            {
                SpecialAbility.GraspingClaw,
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
                // WeaponAbility.CrushingBlow,
                // WeaponAbility.Disarm,
                WeaponAbility.Dismount,
                WeaponAbility.Feint,
                WeaponAbility.ForceOfNature,
                // WeaponAbility.FrenziedWhirlwind,
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
