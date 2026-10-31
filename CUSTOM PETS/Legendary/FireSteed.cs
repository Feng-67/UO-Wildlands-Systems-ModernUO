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
public partial class FireSteed : BaseMount, ILegendaryPet
{
    [SerializableField(0, setter: "private")]
    private bool _isLegendary;

    [Constructible]
    public FireSteed() : base(0xBE, 0x3E9E, AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0xA8;
        Hue = 1161;

        SetStr(376, 400);
        SetDex(91, 120);
        SetInt(291, 300);

        SetHits(226, 240);

        SetDamage(11, 30);

        SetDamageType(ResistanceType.Physical, 20);
        SetDamageType(ResistanceType.Fire, 80);

        SetResistance(ResistanceType.Physical, 30, 40);
        SetResistance(ResistanceType.Fire, 70, 80);
        SetResistance(ResistanceType.Cold, 20, 30);
        SetResistance(ResistanceType.Poison, 30, 40);
        SetResistance(ResistanceType.Energy, 30, 40);

        SetSkill(SkillName.MagicResist, 100.0, 120.0);
        SetSkill(SkillName.Tactics, 100.0);
        SetSkill(SkillName.Wrestling, 100.0);

        Fame = 20000;
        Karma = -20000;

        Tamable = true;
        ControlSlots = 2;
        MinTameSkill = 106.0;

        PackItem(new SulfurousAsh(Utility.RandomMinMax(151, 300)));
        PackItem(new Ruby(Utility.RandomMinMax(16, 30)));
    }

    public override string CorpseName => "a fire steed corpse";
    public override string DefaultName => "a fire steed";

    public override FoodType FavoriteFood => FoodType.Meat;
    public override PackInstinct PackInstinct => PackInstinct.Daemon | PackInstinct.Equine;

    // ModernUO: SetSpecialAbility replaced by GetMonsterAbilities.
    // Base uses the full ServUO-port DragonBreath (with projectiles); Legendary swaps to Inferno.
    private static readonly MonsterAbility[] _baseAbilities = { SpecialAbility.DragonBreath };
    private static readonly MonsterAbility[] _legendaryAbilities = { SpecialAbility.Inferno };

    public override MonsterAbility[] GetMonsterAbilities() =>
        _isLegendary ? _legendaryAbilities : _baseAbilities;

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
        Hue = 1174;
        ControlSlots = 1;
        MinTameSkill = 120.0;
        SetStr(410, 440);
        SetHits(300, 375);
        SetSkill(SkillName.Magery, 80.0, 100.0);

        // Ability swap (DragonBreath → Inferno) handled by GetMonsterAbilities via _isLegendary.
        ApplyLegendaryTrainingDefinition();
    }

    private void ApplyLegendaryTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(FireSteed),
            Class.Magical,
            MagicalAbility.Dragon2,
            PetTrainingDefinitions.SpecialAbilityMagical2,
            PetTrainingDefinitions.WepAbility2,
            PetTrainingDefinitions.AreaEffectArea2,
            1,
            5
        );
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        // TrainingDefinition isn't serialized; re-apply for legendary instances on load.
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

    public override void GenerateLoot()
    {
        // Vanilla FireSteed overrides this empty to suppress base loot generation;
        // preserved for parity. Add legendary-only drops here if desired.
    }
}
