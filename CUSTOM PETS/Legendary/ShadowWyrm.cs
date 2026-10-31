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
public partial class ShadowWyrm : BaseCreature, ILegendaryPet
{
    [SerializableField(0, setter: "private")]
    private bool _isLegendary;

    [Constructible]
    public ShadowWyrm() : base(AIType.AI_NecroMage, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 106;
        BaseSoundID = 362;

        SetStr(898, 1030);
        SetDex(68, 200);
        SetInt(488, 620);

        SetHits(558, 599);

        SetDamage(29, 35);

        SetDamageType(ResistanceType.Physical, 75);
        SetDamageType(ResistanceType.Cold, 25);

        SetResistance(ResistanceType.Physical, 65, 75);
        SetResistance(ResistanceType.Fire, 50, 60);
        SetResistance(ResistanceType.Cold, 45, 55);
        SetResistance(ResistanceType.Poison, 20, 30);
        SetResistance(ResistanceType.Energy, 50, 60);

        SetSkill(SkillName.EvalInt, 80.1, 100.0);
        SetSkill(SkillName.Magery, 80.1, 100.0);
        SetSkill(SkillName.Meditation, 52.5, 75.0);
        SetSkill(SkillName.MagicResist, 100.3, 130.0);
        SetSkill(SkillName.Tactics, 97.6, 100.0);
        SetSkill(SkillName.Wrestling, 97.6, 100.0);
        SetSkill(SkillName.DetectHidden, 90.0, 100.0);
        SetSkill(SkillName.Necromancy, 80.0, 90.0);
        SetSkill(SkillName.SpiritSpeak, 100.0, 105.0);

        Fame = 22500;
        Karma = -22500;

        VirtualArmor = 70;

        Tamable = true;
        ControlSlots = 5;
        MinTameSkill = 105.0;
    }

    public override string CorpseName => "a shadow wyrm corpse";
    public override string DefaultName => "a shadow wyrm";

    public override bool CanAngerOnTame => true;
    public override bool ReacquireOnMovement => !Controlled;
    public override bool AutoDispel => !Controlled;
    public override Poison PoisonImmune => Poison.Deadly;
    public override Poison HitPoison => Poison.Deadly;
    public override int TreasureMapLevel => 5;
    public override int Meat => 19;
    public override int Hides => 20;
    public override int Scales => 10;
    public override ScaleType ScaleType => ScaleType.Black;
    public override HideType HideType => HideType.Barbed;
    public override bool CanFly => true;

    // ModernUO: replaced SetSpecialAbility with GetMonsterAbilities.
    // Base uses the native FireBreath (matching the working ShadowWyrm); Legendary
    // adds LifeLeech on top as an upgrade, same pattern as the working TsukiWolf fix.
    private static readonly MonsterAbility[] _normalAbilities =
    {
        MonsterAbilities.FireBreath
    };

    private static readonly MonsterAbility[] _legendaryAbilities =
    {
        MonsterAbilities.FireBreath,
        SpecialAbility.LifeLeech
    };

    public override MonsterAbility[] GetMonsterAbilities() =>
        _isLegendary ? _legendaryAbilities : _normalAbilities;

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

        Hue = 1910;
        ControlSlots = 2;
        MinTameSkill = 120.0;

        // Attributes and Stats
        SetStr(500, 600);
        SetDex(150, 200);
        SetInt(600, 700);
        SetHits(450, 500);

        // Damage Types (Cold 20%, Poison 80%)
        SetDamageType(ResistanceType.Physical, 0);
        SetDamageType(ResistanceType.Fire, 0);
        SetDamageType(ResistanceType.Cold, 20);
        SetDamageType(ResistanceType.Poison, 80);
        SetDamageType(ResistanceType.Energy, 0);

        // Resistances
        SetResistance(ResistanceType.Poison, 60, 70);

        // Added Skills
        SetSkill(SkillName.Necromancy, 130.0, 150.0);
        SetSkill(SkillName.SpiritSpeak, 130.0, 150.0);

        // Ability swap: base FireBreath becomes FireBreath + LifeLeech (handled by
        // GetMonsterAbilities above), and the magical school is switched to Necromancy.
        var profile = PetTrainingHelper.GetAbilityProfile(this, create: true);
        profile.MagicalAbility = MagicalAbility.Necromancy;

        ApplyLegendaryTrainingDefinition();
    }

    private void ApplyLegendaryTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(ShadowWyrm),
            Class.MagicalAndNecromantic,
            MagicalAbility.Necromage | MagicalAbility.Necromancy | MagicalAbility.Poisoning |
            MagicalAbility.Discordance | MagicalAbility.Bashing | MagicalAbility.Piercing |
            MagicalAbility.Slashing | MagicalAbility.WrestlingMastery,
            PetTrainingDefinitions.SpecialAbilityNecroMagical,
            PetTrainingDefinitions.WepAbility2,
            PetTrainingDefinitions.AreaEffectArea2,
            2,
            5
        );
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        // TrainingDefinition isn't virtual and isn't auto-persisted on the profile path,
        // so re-apply for legendary instances after a server restart.
        if (_isLegendary)
        {
            ApplyLegendaryTrainingDefinition();

            var profile = PetTrainingHelper.GetAbilityProfile(this, create: true);
            profile.MagicalAbility = MagicalAbility.Necromancy;
        }
    }

    public override void GenerateLoot()
    {
        AddLoot(LootPack.FilthyRich, 3);
        AddLoot(LootPack.Gems, 5);
    }

    public override int GetIdleSound() => 0x2D5;
    public override int GetHurtSound() => 0x2D1;

    public override void AddNameProperties(IPropertyList list)
    {
        base.AddNameProperties(list);

        if (_isLegendary)
        {
            list.Add("<BASEFONT COLOR=#FFD700>Legendary</BASEFONT>");
        }
    }
}
