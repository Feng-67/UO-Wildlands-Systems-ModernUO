using ModernUO.Serialization;
using Server.Engines.Plants;
using Server.Items;

namespace Server.Mobiles;

[TypeAlias("Server.Mobiles.Tiger")]
[CorpseName("a tiger corpse")]
[SerializationGenerator(0, false)]
public partial class WildTiger : BaseMount
{
    [SerializableField(0, getter: "private", setter: "private")]
    private bool _canRideValue;
    

    [CommandProperty(AccessLevel.GameMaster)]
    public bool CanRide
    {
        get => _canRideValue;
        set
        {
            CanRideValue = value;
            InvalidateProperties();
        }
    }

    [Constructible]
    public WildTiger() : this("a wild tiger")
    {
        CanRide = false;
    }

    [Constructible]
    public WildTiger(string name) : base(Utility.RandomBool() ? 1254 : 1255, 16071, AIType.AI_Melee, FightMode.Closest)
    {
        Name = name;

        // BaseMount doesn't set Body from itemID
        Body = ItemID;

        if (Body == 1255)
        {
            ItemID = 16072;
        }

        SetStr(496, 554);
        SetDex(88, 124);
        SetInt(94, 163);

        SetHits(352, 450);

        SetDamage(18, 24);

        SetDamageType(ResistanceType.Physical, 100);

        SetResistance(ResistanceType.Physical, 56, 75);
        SetResistance(ResistanceType.Fire, 21, 40);
        SetResistance(ResistanceType.Cold, 55, 64);
        SetResistance(ResistanceType.Poison, 30, 40);
        SetResistance(ResistanceType.Energy, 25, 35);

        SetSkill(SkillName.MagicResist, 90.8, 97.5);
        SetSkill(SkillName.Anatomy, 0);
        SetSkill(SkillName.Tactics, 100.2, 102.5);
        SetSkill(SkillName.Wrestling, 90.1, 94.4);

        Fame = 11000;
        Karma = -11000;

        if (Core.ML && Utility.RandomDouble() < .33)
        {
            PackItem(Seed.RandomPeculiarSeed(Utility.RandomList(1, 1, 1, 1, 2, 2, 2, 3, 3, 4)));
        }

        Tamable = true;
        ControlSlots = 2;
        MinTameSkill = 95.1;
    }

    // ModernUO: SetWeaponAbility → GetWeaponAbility override
    public override WeaponAbility GetWeaponAbility() => WeaponAbility.BleedAttack;

    // ModernUO: SetSpecialAbility → GetMonsterAbilities override
    private static readonly MonsterAbility[] _abilities = { SpecialAbility.GraspingClaw };

    public override MonsterAbility[] GetMonsterAbilities() => _abilities;

    public override void OnDoubleClick(Mobile from)
    {
        if (CanRide)
        {
            base.OnDoubleClick(from);
        }
        else if (from.AccessLevel >= AccessLevel.GameMaster && !Body.IsHuman)
        {
            var pack = Backpack;

            if (pack != null)
            {
                pack.DisplayTo(from);
            }
        }
    }

    public override int GetIdleSound() => 0x673;
    public override int GetAngerSound() => 0x670;
    public override int GetHurtSound() => 0x672;
    public override int GetDeathSound() => 0x671;

    public override double WeaponAbilityChance => 0.5;

    public override int Meat => 2;
    public override FoodType FavoriteFood => FoodType.Meat;
    public override int TreasureMapLevel => 1;

    public override void GenerateLoot()
    {
        AddLoot(LootPack.Rich, 1);
    }        
}

[CorpseName("a tiger corpse")]
[SerializationGenerator(0, false)]
public partial class WildWhiteTiger : WildTiger
{
    

    [Constructible]
    public WildWhiteTiger() : base("a wild white tiger")
    {
        Hue = 2500;
    }
}

[CorpseName("a tiger corpse")]
[SerializationGenerator(0, false)]
public partial class WildBlackTiger : WildTiger
{
    
    [Constructible]
    public WildBlackTiger() : base("a wild black tiger")
    {
        Hue = 1175;
    }
}
