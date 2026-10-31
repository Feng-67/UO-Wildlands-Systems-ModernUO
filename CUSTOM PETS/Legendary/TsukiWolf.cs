using ModernUO.Serialization;
using Server.Engines.Plants;
using Server.Items;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class TsukiWolf : BaseCreature, ILegendaryPet
{
    [SerializableField(0, setter: "private")]
    private bool _isLegendary;

    [Constructible]
    public TsukiWolf() : base(AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 250;

        if (Utility.Random(3) == 0)
        {
            Hue = Utility.RandomNeutralHue();
        }

        SetStr(401, 450);
        SetDex(151, 200);
        SetInt(66, 76);

        SetHits(376, 450);
        SetMana(40);

        SetDamage(14, 18);

        SetDamageType(ResistanceType.Physical, 90);
        SetDamageType(ResistanceType.Cold, 5);
        SetDamageType(ResistanceType.Energy, 5);

        SetResistance(ResistanceType.Physical, 40, 60);
        SetResistance(ResistanceType.Fire, 50, 70);
        SetResistance(ResistanceType.Cold, 50, 70);
        SetResistance(ResistanceType.Poison, 50, 70);
        SetResistance(ResistanceType.Energy, 50, 70);

        SetSkill(SkillName.Anatomy, 65.1, 72.0);
        SetSkill(SkillName.MagicResist, 65.1, 70.0);
        SetSkill(SkillName.Tactics, 95.1, 110.0);
        SetSkill(SkillName.Wrestling, 97.6, 107.5);
        SetSkill(SkillName.Necromancy, 20.0);
        SetSkill(SkillName.SpiritSpeak, 20.0);
        SetSkill(SkillName.DetectHidden, 100.0);
        SetSkill(SkillName.Parry, 90.0, 100.0);

        Fame = 8500;
        Karma = -8500;

        if (Core.ML && Utility.RandomDouble() < .33)
        {
            PackItem(Seed.RandomPeculiarSeed(1));
        }

        PackBodyPartOrBones();

        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 96.0;
    }

    public override string CorpseName => "a tsuki wolf corpse";
    public override string DefaultName => "a tsuki wolf";

    public override bool CanAngerOnTame => true;
    public override int TreasureMapLevel => 3;
    public override int Meat => 4;
    public override int Hides => 25;
    public override FoodType FavoriteFood => FoodType.Meat;

    // ModernUO: replaced SetSpecialAbility with GetMonsterAbilities
    private static readonly MonsterAbility[] _normalAbilities =
    {
        MonsterAbilities.BloodBathAttack,
        SpecialAbility.Rage
    };

    private static readonly MonsterAbility[] _legendaryAbilities =
    {
        MonsterAbilities.BloodBathAttack,
        SpecialAbility.LifeLeech,
        SpecialAbility.Rage
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

        Hue = Utility.RandomList(1929, 1918, 1910, 1462, 1158, 2716, 2747);

        ControlSlots = 2;
        MinTameSkill = 120.0;

        SetDamageType(ResistanceType.Physical, 50);
        SetDamageType(ResistanceType.Cold, 10);
        SetDamageType(ResistanceType.Energy, 40);

        // Assign the legendary training definition; PetTrainingHelper.GetTrainingDefinition
        // reads TrainingDefinition first before falling back to type lookup.
        ApplyLegendaryTrainingDefinition();
    }

    private void ApplyLegendaryTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(TsukiWolf),
            Class.MagicalClawedTailedNecromanticAndTokuno,
            MagicalAbility.TsukiWolf,
            PetTrainingDefinitions.SpecialAbilityTsukiWolf,
            PetTrainingDefinitions.WepAbility2,
            PetTrainingDefinitions.AreaEffectArea2,
            2,
            5
        );
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        // TrainingDefinition isn't serialized; re-apply on load
        if (_isLegendary)
        {
            ApplyLegendaryTrainingDefinition();
        }
    }

    public override WeaponAbility GetWeaponAbility() =>
        _isLegendary ? WeaponAbility.ArmorIgnore : base.GetWeaponAbility();

    public override int GetAngerSound() => 0x52D;
    public override int GetIdleSound() => 0x52C;
    public override int GetAttackSound() => 0x52B;
    public override int GetHurtSound() => 0x52E;
    public override int GetDeathSound() => 0x52A;

    public override void GenerateLoot()
    {
        AddLoot(LootPack.FilthyRich);
    }

    public override void AddNameProperties(IPropertyList list)
    {
        base.AddNameProperties(list);

        if (_isLegendary)
        {
            list.Add("<BASEFONT COLOR=#FFD700>Legendary</BASEFONT>");
        }
    }

    // ModernUO: PackBodyPartOrBones doesn't exist on BaseCreature; inlined from the working version
    private void PackBodyPartOrBones()
    {
        PackItem(
            Utility.Random(10) switch
            {
                0 => new LeftArm(),
                1 => new RightArm(),
                2 => new Torso(),
                3 => new Bone(),
                4 => new RibCage(),
                5 => new RibCage(),
                _ => new BonePile() // 6-9
            }
        );
    }
}
