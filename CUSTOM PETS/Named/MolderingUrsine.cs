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
public partial class MolderingUrsine : BaseMount
{
    [Constructible]
    public MolderingUrsine() : base(1638, 16092, AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0xA3; // Bear sound

        Body = 1638; // BaseMount doesn't set Body from the item ID

        // --- Taming ---
        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108.0;

        // --- Attributes (fixed values — zero spread per bestiary) ---
        SetStr(500);
        SetDex(165);
        SetInt(200);

        SetHits(500);
        SetStam(150);
        SetMana(200);

        // --- Damage Profile ---
        SetDamage(16, 22);
        SetDamageType(ResistanceType.Physical, 30);
        SetDamageType(ResistanceType.Cold, 35);
        SetDamageType(ResistanceType.Energy, 35);

        // --- Resistances (fixed values — zero spread per bestiary) ---
        SetResistance(ResistanceType.Physical, 65);
        SetResistance(ResistanceType.Fire, 40);
        SetResistance(ResistanceType.Cold, 55);
        SetResistance(ResistanceType.Poison, 40);
        SetResistance(ResistanceType.Energy, 55);

        // --- Skills ---
        SetSkill(SkillName.Wrestling, 100.1, 115.0);
        SetSkill(SkillName.Tactics, 100.1, 115.0);
        SetSkill(SkillName.MagicResist, 80.0, 100.0);
        SetSkill(SkillName.Anatomy, 50.0, 70.0);

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a moldering ursine corpse";
    public override string DefaultName => "Moldering Ursine";

    public override bool CanAngerOnTame => true;
    public override int Meat => 3;
    public override int Hides => 10;
    public override FoodType FavoriteFood => FoodType.Meat;

    // ModernUO: SetSpecialAbility replaced by GetMonsterAbilities.
    private static readonly MonsterAbility[] _abilities = { SpecialAbility.LifeLeech };

    public override MonsterAbility[] GetMonsterAbilities() => _abilities;

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(MolderingUrsine),
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
                SpecialAbility.LifeLeech
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
