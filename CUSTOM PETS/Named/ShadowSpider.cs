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
public partial class ShadowSpider : BaseCreature
{
    [Constructible]
    public ShadowSpider() : base(AIType.AI_Melee, FightMode.Closest)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 737;

        SetStr(100, 104);
        SetDex(200, 220);
        SetInt(29, 50);

        SetHits(125, 144);

        SetDamage(15, 18);

        SetDamageType(ResistanceType.Physical, 20);
        SetDamageType(ResistanceType.Poison, 80);

        SetResistance(ResistanceType.Physical, 0);
        SetResistance(ResistanceType.Fire, 30, 35);
        SetResistance(ResistanceType.Cold, 30, 35);
        SetResistance(ResistanceType.Poison, 40, 45);
        SetResistance(ResistanceType.Energy, 95, 100);

        SetSkill(SkillName.Anatomy, 2.0, 3.8);
        SetSkill(SkillName.MagicResist, 47.5, 57.9);
        SetSkill(SkillName.Poisoning, 70.5, 73.5);
        SetSkill(SkillName.Tactics, 73.3, 78.9);
        SetSkill(SkillName.Wrestling, 92.5, 94.6);
        SetSkill(SkillName.Hiding, 110.3, 119.9);
        SetSkill(SkillName.Stealth, 110.5, 119.6);

        Tamable = true;
        ControlSlots = 3;
        MinTameSkill = 108;

        ApplyTrainingDefinition();
    }

    public override string CorpseName => "a shadow spider corpse";
    public override string DefaultName => "Shadow Spider";        
    public override bool CanAngerOnTame => true;
    public override int Meat => 3;
    public override int Hides => 10;
    public override FoodType FavoriteFood => FoodType.Meat;
    public override int TreasureMapLevel => 2;

    public override int GetIdleSound() => 1605;
    public override int GetAngerSound() => 1602;
    public override int GetHurtSound() => 1604;
    public override int GetDeathSound() => 1603;

    public override void GenerateLoot()
    {
        AddLoot(LootPack.Rich);
    }

    public override void OnDamage(int amount, Mobile from, bool willKill)
    {
        RevealingAction();
        base.OnDamage(amount, from, willKill);
    }

    public override void OnDamagedBySpell(Mobile from, int damage)
    {
        RevealingAction();
        base.OnDamagedBySpell(from, damage);
    }

    public override void OnThink()
    {
        if (!Alive || Deleted)
        {
            return;
        }

        // Note: as in the original, base.OnThink() only runs while the spider is visible
        if (!Hidden)
        {
            var chance = 0.05;

            if (Hits < 20)
            {
                chance = 0.1;
            }

            if (Poisoned)
            {
                chance = 0.01;
            }

            if (Utility.RandomDouble() < chance)
            {
                HideSelf();
            }

            base.OnThink();
        }
    }

    private void HideSelf()
    {
        if (Core.TickCount >= NextSkillTime)
        {
            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x3728,
                10,
                10,
                2023
            );

            PlaySound(0x22F);
            Hidden = true;

            UseSkill(SkillName.Stealth);
        }
    }

    // ModernUO: TrainingDefinition is not virtual and not serialized. Assigned in
    // constructor and re-applied on load via [AfterDeserialization].
    private void ApplyTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(ShadowSpider),
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
                SpecialAbility.AngryFire,
                SpecialAbility.ConductiveBlast,
                SpecialAbility.DragonBreath,
                SpecialAbility.Inferno,
                SpecialAbility.LifeLeech,
                SpecialAbility.LightningForce,
                SpecialAbility.ManaDrain,
                SpecialAbility.RagingBreath,
                SpecialAbility.Repel,
                SpecialAbility.SearingWounds,
                SpecialAbility.VenomousBite,
                SpecialAbility.ViciousBite
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
