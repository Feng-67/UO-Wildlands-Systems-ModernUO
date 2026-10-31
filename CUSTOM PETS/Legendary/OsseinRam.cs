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
public partial class OsseinRam : BaseCreature, ILegendaryPet
{
    [SerializableField(0, setter: "private")]
    private bool _isLegendary;

    [Constructible]
    public OsseinRam() : base(AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 0x591;
        BaseSoundID = 0x99;

        SetStr(300, 400);
        SetDex(80, 100);
        SetInt(100, 120);

        SetHits(450, 550);

        SetDamage(18, 23);

        SetDamageType(ResistanceType.Physical, 50);
        SetDamageType(ResistanceType.Cold, 25);
        SetDamageType(ResistanceType.Energy, 25);

        SetResistance(ResistanceType.Physical, 50, 60);
        SetResistance(ResistanceType.Fire, 10, 20);
        SetResistance(ResistanceType.Cold, 40, 50);
        SetResistance(ResistanceType.Poison, 30, 40);
        SetResistance(ResistanceType.Energy, 40, 50);

        SetSkill(SkillName.MagicResist, 70.0, 80.0);
        SetSkill(SkillName.Tactics, 80.0, 90.0);
        SetSkill(SkillName.Wrestling, 90.0, 100.0);
        SetSkill(SkillName.DetectHidden, 40.0, 50.0);
        SetSkill(SkillName.Anatomy, 75.0, 85.0);
        SetSkill(SkillName.Necromancy, 20.0);
        SetSkill(SkillName.SpiritSpeak, 20.0);

        Tamable = true;
        ControlSlots = 2;
        MinTameSkill = 72.0;

        // ModernUO: SetMagicalAbility replaced by direct profile assignment
        PetTrainingHelper.GetAbilityProfile(this, create: true).MagicalAbility = MagicalAbility.BattleDefense;
    }

    public override string CorpseName => "an ossein ram corpse";
    public override string DefaultName => "an ossein ram";

    public override int Meat => 3;
    public override FoodType FavoriteFood => FoodType.Meat;
    public override bool CanAngerOnTame => true;
    public override bool StatLossAfterTame => true;

    // ModernUO: SetWeaponAbility replaced by GetWeaponAbility override.
    // Base = MortalStrike, Legendary = DoubleStrike (matching original intent).
    public override WeaponAbility GetWeaponAbility() =>
        _isLegendary ? WeaponAbility.DoubleStrike : WeaponAbility.MortalStrike;

    // ModernUO: SetSpecialAbility replaced by GetMonsterAbilities.
    // Both base and legendary have LifeLeech, so unconditional.
    private static readonly MonsterAbility[] _abilities = { SpecialAbility.LifeLeech };

    public override MonsterAbility[] GetMonsterAbilities() => _abilities;

    public override void OnBeforeSpawn(Point3D location, Map map)
    {
        base.OnBeforeSpawn(location, map);

        if (Utility.RandomDouble() < 0.10)
        {
            ConvertToLegendary();
        }
    }

    public void ConvertToLegendary()
    {
        IsLegendary = true;

        Hue = Utility.RandomList(2076, 2706, 2955, 2075);

        ControlSlots = 1;
        MinTameSkill = 120.0;

        // ModernUO: direct profile assignment replaces Set/RemoveMagicalAbility.
        // Necromage replaces BattleDefense; the single-field assignment drops the old value.
        PetTrainingHelper.GetAbilityProfile(this, create: true).MagicalAbility = MagicalAbility.Necromage;

        SetSkill(SkillName.Necromancy, 50.6, 75.0);

        ApplyLegendaryTrainingDefinition();
    }

    private void ApplyLegendaryTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(OsseinRam),
            Class.None,
            MagicalAbility.Necromage | MagicalAbility.Necromancy | MagicalAbility.Piercing |
            MagicalAbility.Slashing | MagicalAbility.Bashing | MagicalAbility.WrestlingMastery,
            new[]
            {
                SpecialAbility.LifeLeech,
                SpecialAbility.ManaDrain,
                SpecialAbility.Repel,
                SpecialAbility.SearingWounds
            },
            PetTrainingDefinitions.WepAbility12,
            PetTrainingDefinitions.AreaEffectArea2,
            1,
            5
        );
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        // TrainingDefinition isn't serialized; re-apply for legendary instances on load.
        // MagicalAbility IS serialized on the AbilityProfile, so no reapply needed.
        if (_isLegendary)
        {
            ApplyLegendaryTrainingDefinition();
        }
    }

    public override void AddNameProperties(IPropertyList list)
    {
        base.AddNameProperties(list);

        if (_isLegendary)
        {
            list.Add("<BASEFONT COLOR=#FFD700>Legendary</BASEFONT>");
        }
    }
}
