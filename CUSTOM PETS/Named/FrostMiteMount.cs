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
public partial class FrostMiteMount : BaseMount
{
    [Constructible]
    public FrostMiteMount() : base(0x590, 0x3EDA, AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 0x590; // BaseMount doesn't set Body from the item ID

        SetStr(1017);
        SetDex(164);
        SetInt(283);

        SetHits(800, 1000);

        SetDamage(21, 28);

        SetDamageType(ResistanceType.Physical, 0);
        SetDamageType(ResistanceType.Cold, 100);

        SetResistance(ResistanceType.Physical, 55, 65);
        SetResistance(ResistanceType.Fire, 10, 15);
        SetResistance(ResistanceType.Cold, 90, 100);
        SetResistance(ResistanceType.Poison, 55, 65);
        SetResistance(ResistanceType.Energy, 65, 75);

        SetSkill(SkillName.MagicResist, 110.0, 120.0);
        SetSkill(SkillName.Tactics, 110.0, 120.0);
        SetSkill(SkillName.Wrestling, 110.0, 120.0);

        Fame = 15000;
        Karma = -15000;

        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108.0;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a frost mite corpse";
    public override string DefaultName => "Frost Mite";

    public override bool CanAngerOnTame => true;
    public override bool StatLossAfterTame => true;
    public override int Meat => 3;
    public override FoodType FavoriteFood => FoodType.Meat;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(FrostMiteMount),
            Class.None,
            MagicalAbility.Poisoning |
            MagicalAbility.WrestlingMastery,
            new[]
            {
                SpecialAbility.ManaDrain,
                SpecialAbility.Repel,
                SpecialAbility.RuneCorruption,
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
