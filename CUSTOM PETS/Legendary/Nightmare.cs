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
public partial class Nightmare : BaseMount, ILegendaryPet
{
    [SerializableField(0, setter: "private")]
    private bool _isLegendary;

    [Constructible]
    public Nightmare() : base(0x74, 0x3EA7, AIType.AI_Mage, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = Core.AOS ? 0xA8 : 0x16A;

        SetStr(496, 525);
        SetDex(86, 105);
        SetInt(86, 125);

        SetHits(298, 315);

        SetDamage(16, 22);

        SetDamageType(ResistanceType.Physical, 40);
        SetDamageType(ResistanceType.Fire, 40);
        SetDamageType(ResistanceType.Energy, 20);

        SetResistance(ResistanceType.Physical, 55, 65);
        SetResistance(ResistanceType.Fire, 30, 40);
        SetResistance(ResistanceType.Cold, 30, 40);
        SetResistance(ResistanceType.Poison, 30, 40);
        SetResistance(ResistanceType.Energy, 20, 30);

        SetSkill(SkillName.EvalInt, 10.4, 50.0);
        SetSkill(SkillName.Magery, 10.4, 50.0);
        SetSkill(SkillName.MagicResist, 85.3, 100.0);
        SetSkill(SkillName.Tactics, 97.6, 100.0);
        SetSkill(SkillName.Wrestling, 80.5, 92.5);

        Fame = 14000;
        Karma = -14000;

        VirtualArmor = 60;

        Tamable = true;
        ControlSlots = 2;
        MinTameSkill = 95.1;

        switch (Utility.Random(12))
        {
            case 0: PackItem(new BloodOathScroll()); break;
            case 1: PackItem(new HorrificBeastScroll()); break;
            case 2: PackItem(new StrangleScroll()); break;
            case 3: PackItem(new VengefulSpiritScroll()); break;
        }

        // ModernUO: BodyValue → Body (BodyValue doesn't exist)
        switch (Utility.Random(4))
        {
            case 0: Body = 116; ItemID = 16039; break;
            case 1: Body = 177; ItemID = 16053; break;
            case 2: Body = 178; ItemID = 16041; break;
            case 3: Body = 179; ItemID = 16055; break;
        }

        if (Utility.RandomDouble() < 0.05)
        {
            Hue = 1910;
        }

        PackItem(new SulfurousAsh(Utility.RandomMinMax(3, 5)));
    }

    public override string CorpseName => "a nightmare corpse";
    public override string DefaultName => "a nightmare";

    public override int Meat => 5;
    public override int Hides => 10;
    public override HideType HideType => HideType.Barbed;
    public override FoodType FavoriteFood => FoodType.Meat;
    public override bool CanAngerOnTame => true;

    // ModernUO: SetSpecialAbility replaced by GetMonsterAbilities.
    // Base uses native FireBreath (matches the working Nightmare); Legendary swaps to Heal.
    private static readonly MonsterAbility[] _baseAbilities = { MonsterAbilities.FireBreath };
    private static readonly MonsterAbility[] _legendaryAbilities = { SpecialAbility.Heal };

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
        Hue = 2040;

        ControlSlots = 1;
        MinTameSkill = 120.0;

        SetSkill(SkillName.Healing, 72.2, 98.9);

        // Ability swap (FireBreath → Heal) handled by GetMonsterAbilities via _isLegendary.
        ApplyLegendaryTrainingDefinition();
    }

    private void ApplyLegendaryTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(Nightmare),
            Class.MagicalAndNecromantic,
            MagicalAbility.Variety2,
            PetTrainingDefinitions.SpecialAbilityNecroMagical,
            PetTrainingDefinitions.WepAbility2,
            PetTrainingDefinitions.AreaEffectArea2,
            1,
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

    public override void GenerateLoot()
    {
        AddLoot(LootPack.Rich);
        AddLoot(LootPack.Average);
        AddLoot(LootPack.LowScrolls);
        AddLoot(LootPack.Potions);
    }

    public override int GetAngerSound() => !Controlled ? 0x16A : base.GetAngerSound();
}
