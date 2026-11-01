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
public partial class WarClydesdale : BaseMount, IBardedMount
{
    [SerializableField(0, getter: "private", setter: "private")]
    private bool _bardingExceptionalValue;

    [SerializableField(1, getter: "private", setter: "private")]
    private Mobile _bardingCrafterValue;

    [SerializableField(2, getter: "private", setter: "private")]
    private bool _hasBardingValue;

    [SerializableField(3, getter: "private", setter: "private")]
    private int _bardingHPValue;

    [SerializableField(4, getter: "private", setter: "private")]
    private CraftResource _bardingResourceValue;

    [Constructible]
    public WarClydesdale() : base(1651, 16094, AIType.AI_Melee, FightMode.Aggressor)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        BaseSoundID = 0xA8;

        SetStr(201, 300);
        SetDex(66, 85);
        SetInt(61, 100);

        SetHits(121, 180);

        SetDamage(3, 4);

        SetDamageType(ResistanceType.Physical, 75);
        SetDamageType(ResistanceType.Poison, 25);

        SetResistance(ResistanceType.Physical, 35, 40);
        SetResistance(ResistanceType.Fire, 20, 30);
        SetResistance(ResistanceType.Cold, 20, 40);
        SetResistance(ResistanceType.Poison, 20, 30);
        SetResistance(ResistanceType.Energy, 30, 40);

        SetSkill(SkillName.Anatomy, 45.1, 55.0);
        SetSkill(SkillName.MagicResist, 45.1, 55.0);
        SetSkill(SkillName.Tactics, 45.1, 55.0);
        SetSkill(SkillName.Wrestling, 45.1, 55.0);

        Fame = 2000;
        Karma = -2000;

        Tamable = true;
        ControlSlots = 1;
        MinTameSkill = 93.9;
    }

    public override string CorpseName => "a clydesdale corpse";
    public override string DefaultName => "a war clydesdale";

    [CommandProperty(AccessLevel.GameMaster)]
    public Mobile BardingCrafter
    {
        get => _bardingCrafterValue;
        set
        {
            BardingCrafterValue = value;
            InvalidateProperties();
        }
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public bool BardingExceptional
    {
        get => _bardingExceptionalValue;
        set
        {
            BardingExceptionalValue = value;
            InvalidateProperties();
        }
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public int BardingHP
    {
        get => _bardingHPValue;
        set
        {
            BardingHPValue = value;
            InvalidateProperties();
        }
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public bool HasBarding
    {
        get => _hasBardingValue;
        set
        {
            HasBardingValue = value;

            Hue = value ? CraftResources.GetHue(_bardingResourceValue) : 0;

            InvalidateProperties();
        }
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public CraftResource BardingResource
    {
        get => _bardingResourceValue;
        set
        {
            BardingResourceValue = value;

            if (_hasBardingValue)
            {
                Hue = CraftResources.GetHue(value);
            }

            InvalidateProperties();
        }
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public int BardingMaxHP =>
        _bardingResourceValue switch
        {
            CraftResource.DullCopper or CraftResource.Valorite => BardingExceptional ? 14500 : 12500,
            CraftResource.ShadowIron => BardingExceptional ? 17000 : 15000,
            _ => BardingExceptional ? 12000 : 10000
        };

    public override bool ReacquireOnMovement => true;
    public override FoodType FavoriteFood => FoodType.FruitsAndVeggies | FoodType.GrainsAndHay;
    public override int Meat => 9;
    public override int Hides => 10;
    public override bool CanAngerOnTame => true;

    public override bool OverrideBondingReqs() => true;

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        if (BaseSoundID == -1)
        {
            BaseSoundID = 0xA8;
        }
    }

    private int CalculateBardingResistance(ResistanceType type)
    {
        if (_bardingResourceValue == CraftResource.None || !_hasBardingValue)
        {
            return 0;
        }

        var attrs = CraftResources.GetInfo(_bardingResourceValue)?.AttributeInfo;

        if (attrs == null)
        {
            return 0;
        }

        var expBonus = BardingExceptional ? 1 : 0;

        var resBonus = type switch
        {
            ResistanceType.Fire => Math.Max(3, attrs.ArmorFireResist),
            ResistanceType.Cold => Math.Max(2, attrs.ArmorColdResist),
            ResistanceType.Poison => Math.Max(3, attrs.ArmorPoisonResist),
            ResistanceType.Energy => Math.Max(2, attrs.ArmorEnergyResist),
            _ => Math.Max(5, attrs.ArmorPhysicalResist)
        };

        return (resBonus + expBonus) * 5;
    }

    public override int GetResistance(ResistanceType type) => base.GetResistance(type) + CalculateBardingResistance(type);

    public override double GetControlChance(Mobile m, bool useBaseSkill)
    {
        if (PetTrainingHelper.Enabled)
        {
            var profile = PetTrainingHelper.GetAbilityProfile(this);

            // ModernUO AbilityProfile has no HasCustomized(); check whether any
            // abilities were actually assigned instead.
            if (profile != null && profile.AbilityCount() > 0)
            {
                return base.GetControlChance(m, useBaseSkill);
            }
        }

        return 1.0;
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        if (HasBarding && BardingExceptional && BardingCrafter != null)
        {
            list.Add(1060853, BardingCrafter.Name); // armor exceptionally crafted by ~1_val~
        }

        if (HasBarding)
        {
            list.Add(1115719, BardingHP.ToString()); // armor points: ~1_val~
        }
    }

    // ModernUO changed OnRiderDamaged's signature: no `ref int amount`, and it's
    // now a notification-only hook. Damage absorption for barded mounts is handled
    // through AlterMeleeDamageFrom / AlterSpellDamageFrom instead.
    public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
    {
        base.AlterMeleeDamageFrom(from, ref damage);

        if (from != null && Rider != null && from != Rider && Rider.Player && Rider.Mount == this && HasBarding)
        {
            var percent = BardingExceptional ? 20 : 10;
            var absorbed = AOS.Scale(damage, percent);

            damage -= absorbed;
            BardingHP -= absorbed;

            if (BardingHP < 0)
            {
                HasBarding = false;
                BardingHP = 0;

                Rider.SendLocalizedMessage(1053031); // Your dragon's barding has been destroyed!
            }
        }
    }

    public override void AlterSpellDamageFrom(Mobile from, ref int damage)
    {
        base.AlterSpellDamageFrom(from, ref damage);

        if (from != null && Rider != null && from != Rider && Rider.Player && Rider.Mount == this && HasBarding)
        {
            var percent = BardingExceptional ? 20 : 10;
            var absorbed = AOS.Scale(damage, percent);

            damage -= absorbed;
            BardingHP -= absorbed;

            if (BardingHP < 0)
            {
                HasBarding = false;
                BardingHP = 0;

                Rider.SendLocalizedMessage(1053031);
            }
        }
    }
}
