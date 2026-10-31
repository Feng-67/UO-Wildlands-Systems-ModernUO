/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class Kirin : BaseMount, ILegendaryPet
{
    [SerializableField(0, setter: "private")]
    private bool _isLegendary;

    [SerializableField(1, setter: "private")]
    private bool _isFireVariant;

    [Constructible]
    public Kirin() : base(132, 0x3EAD, AIType.AI_Mage, FightMode.Evil)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0x3C5;

        SetStr(296, 325);
        SetDex(86, 105);
        SetInt(186, 225);

        SetHits(191, 210);

        SetDamage(16, 22);

        SetDamageType(ResistanceType.Physical, 70);
        SetDamageType(ResistanceType.Fire, 10);
        SetDamageType(ResistanceType.Cold, 10);
        SetDamageType(ResistanceType.Energy, 10);

        SetResistance(ResistanceType.Physical, 55, 65);
        SetResistance(ResistanceType.Fire, 35, 45);
        SetResistance(ResistanceType.Cold, 25, 35);
        SetResistance(ResistanceType.Poison, 25, 35);
        SetResistance(ResistanceType.Energy, 25, 35);

        SetSkill(SkillName.EvalInt, 80.1, 90.0);
        SetSkill(SkillName.Magery, 60.4, 100.0);
        SetSkill(SkillName.Meditation, 90.1, 100.0);
        SetSkill(SkillName.MagicResist, 85.3, 100.0);
        SetSkill(SkillName.Tactics, 20.1, 22.5);
        SetSkill(SkillName.Wrestling, 80.5, 92.5);

        Fame = 9000;
        Karma = 9000;

        Tamable = true;
        ControlSlots = 2;
        MinTameSkill = 95.1;
    }

    public override int StepsMax => 4480;
    public override string CorpseName => "a ki-rin corpse";
    public override string DefaultName => "a ki-rin";

    // Vanilla Ki-Rin is male-only; legendary variants allow female riders/tamers.
    public override bool AllowFemaleRider => _isLegendary;
    public override bool AllowFemaleTamer => _isLegendary;

    public override bool InitialInnocent => true;
    public override TimeSpan MountAbilityDelay => TimeSpan.FromHours(1.0);
    public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;
    public override int Meat => 3;
    public override int Hides => 10;
    public override HideType HideType => HideType.Horned;
    public override FoodType FavoriteFood => FoodType.FruitsAndVeggies | FoodType.GrainsAndHay;

    // ModernUO: SetSpecialAbility replaced by GetMonsterAbilities.
    // Base Ki-Rin has none; Fire legendary gets AngryFire; Cold legendary gets FlurryForce + Heal.
    private static readonly MonsterAbility[] _fireAbilities = { SpecialAbility.AngryFire };
    private static readonly MonsterAbility[] _coldAbilities = { SpecialAbility.FlurryForce, SpecialAbility.Heal };

    public override MonsterAbility[] GetMonsterAbilities()
    {
        if (!_isLegendary)
        {
            return Array.Empty<MonsterAbility>();
        }

        return _isFireVariant ? _fireAbilities : _coldAbilities;
    }

    // ModernUO: SetWeaponAbility replaced by GetWeaponAbility override.
    // Only the Fire legendary has a weapon ability (DoubleStrike).
    public override WeaponAbility GetWeaponAbility() =>
        _isFireVariant ? WeaponAbility.DoubleStrike : base.GetWeaponAbility();

    public override void OnBeforeSpawn(Point3D location, Map map)
    {
        base.OnBeforeSpawn(location, map);

        if (Utility.RandomDouble() < 0.10)
        {
            if (Utility.RandomBool())
            {
                ConvertToLegendaryFire();
            }
            else
            {
                ConvertToLegendaryCold();
            }
        }
    }

    public void ConvertToLegendaryFire()
    {
        IsLegendary = true;
        _isFireVariant = true;
        Name = "a fire ki-rin";
        Hue = 1161;

        ChangeAIType(AIType.AI_Melee);
        ControlSlots = 2;
        MinTameSkill = 120.0;

        SetStr(375, 450);
        SetDex(87, 100);
        SetInt(175, 250);
        SetHits(450, 550);

        SetDamage(21, 28);
        SetDamageType(ResistanceType.Physical, 25);
        SetDamageType(ResistanceType.Fire, 75);
        SetDamageType(ResistanceType.Cold, 0);
        SetDamageType(ResistanceType.Energy, 0);

        SetResistance(ResistanceType.Physical, 30, 40);
        SetResistance(ResistanceType.Fire, 55, 75);
        SetResistance(ResistanceType.Cold, 30, 40);
        SetResistance(ResistanceType.Poison, 30, 40);
        SetResistance(ResistanceType.Energy, 20, 30);

        SetSkill(SkillName.Wrestling, 90.1, 96.8);
        SetSkill(SkillName.Tactics, 90.3, 99.3);
        SetSkill(SkillName.MagicResist, 75.3, 90.0);
        SetSkill(SkillName.Anatomy, 65.5, 69.4);

        SetSkill(SkillName.EvalInt, 0.0);
        SetSkill(SkillName.Magery, 0.0);
        SetSkill(SkillName.Meditation, 0.0);

        ApplyLegendaryTrainingDefinition();
    }

    public void ConvertToLegendaryCold()
    {
        IsLegendary = true;
        _isFireVariant = false;
        Name = "a cold ki-rin";
        Hue = 1153;

        ChangeAIType(AIType.AI_Melee);
        ControlSlots = 2;
        MinTameSkill = 120.0;

        SetStr(600, 612);
        SetDex(75, 85);
        SetInt(75, 142);
        SetHits(505, 638);

        SetDamage(21, 28);
        SetDamageType(ResistanceType.Physical, 25);
        SetDamageType(ResistanceType.Fire, 0);
        SetDamageType(ResistanceType.Cold, 75);
        SetDamageType(ResistanceType.Energy, 0);

        SetResistance(ResistanceType.Physical, 55, 75);
        SetResistance(ResistanceType.Fire, 30, 40);
        SetResistance(ResistanceType.Cold, 30, 40);
        SetResistance(ResistanceType.Poison, 30, 40);
        SetResistance(ResistanceType.Energy, 20, 30);

        SetSkill(SkillName.Wrestling, 90.1, 96.8);
        SetSkill(SkillName.Tactics, 90.3, 99.3);
        SetSkill(SkillName.MagicResist, 75.3, 90.0);
        SetSkill(SkillName.Anatomy, 65.5, 69.4);

        SetSkill(SkillName.EvalInt, 0.0);
        SetSkill(SkillName.Magery, 0.0);
        SetSkill(SkillName.Meditation, 0.0);

        ApplyLegendaryTrainingDefinition();
    }

    // Both fire and cold variants share the same training definition.
    private void ApplyLegendaryTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(Kirin),
            Class.MagicalClawedAndTailed,
            MagicalAbility.Chivalry | MagicalAbility.Discordance | MagicalAbility.Poisoning |
            MagicalAbility.BattleDefense | MagicalAbility.Bashing | MagicalAbility.Piercing |
            MagicalAbility.Slashing | MagicalAbility.WrestlingMastery,
            PetTrainingDefinitions.SpecialAbilityClawedTailedAndMagical2,
            PetTrainingDefinitions.WepAbility2,
            PetTrainingDefinitions.AreaEffectArea2,
            2,
            5
        );
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
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

    public override void OnDisallowedRider(Mobile m)
    {
        m.SendLocalizedMessage(1042319); // The Ki-Rin refuses your attempts to mount it.
    }

    public override bool DoMountAbility(int damage, Mobile attacker)
    {
        if (Rider == null || attacker == null)
        {
            return false;
        }

        if (Rider.Hits - damage < 30 && Rider.Map == attacker.Map && Rider.InRange(attacker, 18))
        {
            attacker.BoltEffect(0);
            attacker.Damage(Utility.RandomMinMax(35, 100), this, false);

            Rider.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1042534);
            Rider.FixedParticles(0, 0, 0, 0x13A7, EffectLayer.Waist);
            Rider.PlaySound(0xA9);
            return true;
        }

        return false;
    }

    public override void GenerateLoot()
    {
        AddLoot(LootPack.Rich);
        AddLoot(LootPack.LowScrolls);
        AddLoot(LootPack.Potions);
    }

    public override void OnDeath(Container c)
    {
        base.OnDeath(c);

        if (!Controlled && Utility.RandomDouble() < 0.3)
        {
            c.DropItem(new KirinBrains());
        }
    }
}
