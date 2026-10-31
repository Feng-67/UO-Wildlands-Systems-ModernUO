/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;

namespace Server.Mobiles;

[TypeAlias("Server.Mobiles.Polarbear")]
[SerializationGenerator(0, false)]
public partial class PolarBear : BaseMount, ILegendaryPet
{
    [SerializableField(0, setter: "private")]
    private bool _isLegendary;

    [Constructible]
    public PolarBear() : base(213, 0x3EC5, AIType.AI_Animal, FightMode.Aggressor)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 213; // BaseMount doesn't set Body from the item ID
        BaseSoundID = 0xA3;

        SetStr(116, 140);
        SetDex(81, 105);
        SetInt(26, 50);

        SetHits(70, 84);
        SetMana(0);

        SetDamage(7, 12);

        SetDamageType(ResistanceType.Physical, 100);

        SetResistance(ResistanceType.Physical, 25, 35);
        SetResistance(ResistanceType.Cold, 60, 80);
        SetResistance(ResistanceType.Poison, 15, 25);
        SetResistance(ResistanceType.Energy, 10, 15);

        SetSkill(SkillName.MagicResist, 45.1, 60.0);
        SetSkill(SkillName.Tactics, 60.1, 90.0);
        SetSkill(SkillName.Wrestling, 45.1, 70.0);

        Fame = 1500;
        Karma = 0;

        VirtualArmor = 18;

        Tamable = true;
        ControlSlots = 1;
        MinTameSkill = 35.1;
    }

    public override string CorpseName => "a polar bear corpse";
    public override string DefaultName => "a polar bear";

    public override bool CanAngerOnTame => _isLegendary;
    public override int Meat => 2;
    public override int Hides => 16;
    public override FoodType FavoriteFood => FoodType.Fish | FoodType.FruitsAndVeggies | FoodType.Meat;
    public override PackInstinct PackInstinct => PackInstinct.Bear;

    // ModernUO: replaced SetSpecialAbility with GetMonsterAbilities.
    private static readonly MonsterAbility[] _legendaryAbilities =
    {
        SpecialAbility.ColossalRage
    };

    public override MonsterAbility[] GetMonsterAbilities() =>
        _isLegendary ? _legendaryAbilities : base.GetMonsterAbilities();

    public override void OnBeforeSpawn(Point3D location, Map map)
    {
        base.OnBeforeSpawn(location, map);

        // Legendary polar bears only appear in the Winter Spur on Tokuno
        if (map == Map.Tokuno && IsInWinterSpur(location.X, location.Y) && Utility.RandomDouble() < 0.10)
        {
            ConvertToLegendary();
        }
    }

    private static bool IsInWinterSpur(int x, int y)
    {
        // Define Triangle Vertices: A(846, 139), B(1002, 146), C(905, 34)
        var d1 = GetSide(x, y, 846, 139, 1002, 146);
        var d2 = GetSide(x, y, 1002, 146, 905, 34);
        var d3 = GetSide(x, y, 905, 34, 846, 139);

        var hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
        var hasPos = d1 > 0 || d2 > 0 || d3 > 0;

        return !(hasNeg && hasPos);
    }

    private static float GetSide(int px, int py, int x1, int y1, int x2, int y2) =>
        (px - x2) * (y1 - y2) - (x1 - x2) * (py - y2);

    public void ConvertToLegendary()
    {
        IsLegendary = true;

        Hue = Utility.RandomList(1153, 2953);

        ChangeAIType(AIType.AI_Melee);

        SetDamage(21, 28);
        SetDamageType(ResistanceType.Physical, 0);
        SetDamageType(ResistanceType.Cold, 50);
        SetDamageType(ResistanceType.Energy, 50);

        SetHits(1010, 1275);

        SetResistance(ResistanceType.Physical, 50, 65);
        SetResistance(ResistanceType.Fire, 25, 45);
        SetResistance(ResistanceType.Cold, 70, 85);
        SetResistance(ResistanceType.Poison, 30, 50);
        SetResistance(ResistanceType.Energy, 70, 85);

        SetSkill(SkillName.Wrestling, 90.1, 96.8);
        SetSkill(SkillName.Tactics, 90.3, 99.3);
        SetSkill(SkillName.MagicResist, 75.3, 90.0);
        SetSkill(SkillName.Anatomy, 65.5, 69.4);
        SetSkill(SkillName.Healing, 72.2, 98.9);

        MinTameSkill = 120.0;
        ControlSlots = 2;
        ControlSlotsMax = 5;

        ApplyLegendaryTrainingDefinition();
    }

    private void ApplyLegendaryTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(PolarBear),
            Class.Clawed,
            MagicalAbility.Chivalry | MagicalAbility.Discordance | MagicalAbility.Poisoning |
            MagicalAbility.BattleDefense | MagicalAbility.Bashing | MagicalAbility.Piercing |
            MagicalAbility.Slashing | MagicalAbility.WrestlingMastery,
            new[]
            {
                SpecialAbility.GraspingClaw,
                SpecialAbility.ManaDrain,
                SpecialAbility.Repel,
                SpecialAbility.SearingWounds,
                SpecialAbility.LifeLeech,
                SpecialAbility.StealLife,
                SpecialAbility.FlurryForce
            },
            PetTrainingDefinitions.WepAbility2,
            PetTrainingDefinitions.AreaEffectArea2,
            2,
            5
        );
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        // TrainingDefinition isn't serialized; re-apply for legendary instances on load
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
