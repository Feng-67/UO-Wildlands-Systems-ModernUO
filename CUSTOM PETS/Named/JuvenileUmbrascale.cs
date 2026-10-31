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
public partial class JuvenileUmbrascale : BaseMount
{
    [Constructible]
    public JuvenileUmbrascale() : base(1409, 16093, AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0x16A;

        Body = 1409; // BaseMount doesn't set Body from the item ID

        // --- Taming ---
        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108.0;

        // --- Attributes (fixed values — zero spread per bestiary) ---
        SetStr(760);
        SetDex(165);
        SetInt(300);

        SetHits(450);
        SetStam(150);
        SetMana(300);

        // --- Damage Profile ---
        SetDamage(18, 24);
        SetDamageType(ResistanceType.Physical, 0);
        SetDamageType(ResistanceType.Fire, 50);
        SetDamageType(ResistanceType.Energy, 50);

        // --- Resistances (fixed values — zero spread per bestiary) ---
        SetResistance(ResistanceType.Physical, 60);
        SetResistance(ResistanceType.Fire, 60);
        SetResistance(ResistanceType.Cold, 50);
        SetResistance(ResistanceType.Poison, 50);
        SetResistance(ResistanceType.Energy, 50);

        // --- Skills ---
        SetSkill(SkillName.Wrestling, 110.0, 130.0);
        SetSkill(SkillName.Tactics, 100.1, 115.0);
        SetSkill(SkillName.MagicResist, 80.0, 100.0);

        // Healing is listed as an innate ability; Anatomy is required for it to function
        SetSkill(SkillName.Healing, 80.1, 100.0);
        SetSkill(SkillName.Anatomy, 80.1, 100.0);

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a juvenile umbrascale corpse";
    public override string DefaultName => "juvenile umbrascale";

    public override bool CanAngerOnTame => true;
    public override int Meat => 3;
    public override int Hides => 10;
    public override FoodType FavoriteFood => FoodType.Meat;

    public override void OnAfterSpawn()
    {
        base.OnAfterSpawn();

        // Enforce the overcapped Wrestling cap post-spawn so it applies correctly
        Skills[SkillName.Wrestling].Cap = 130.0;
    }

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(JuvenileUmbrascale),
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
        // TrainingDefinition isn't serialized; rebuild on load.
        ApplyTrainingDefinition();
    }
}
