// UO Wildlands Custom Script — ModernUO
// ServUO SpecialAbility.cs port: the 29 concrete special-ability singletons, in persisted index order.
// Licensed under GPL-3.0.

using System;
using System.Collections.Generic;
using Server.Collections;
using Server.Engines.BuffIcons;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Spells.Bushido;

namespace Server.Mobiles;

/// <summary>
/// Registry for the special abilities. The index into <see cref="All"/> is the persisted identity
/// written by AbilityProfile and PlanningProfile, so the order must never change.
/// </summary>
public static class PetTrainingAbilities
{
    public static readonly SpecialAbility[] All =
    {
        new AngryFire(),       // 0
        new ConductiveBlast(), // 1
        new DragonBreath(),    // 2
        new GraspingClaw(),    // 3
        new Inferno(),         // 4
        new LightningForce(),  // 5
        new ManaDrain(),       // 6
        new RagingBreath(),    // 7
        new Repel(),           // 8
        new SearingWounds(),   // 9
        new StealLife(),       // 10
        new VenomousBite(),    // 11
        new ViciousBite(),     // 12
        new RuneCorruption(),  // 13
        new LifeLeech(),       // 14
        new StickySkin(),      // 15
        new TailSwipe(),       // 16
        new FlurryForce(),     // 17
        new Rage(),            // 18

        // Indices 19..28 are creature-innate; SpecialAbility.IsNonTrainable keeps them out of the gump.
        new Heal(),            // 19
        new HowlOfCacophony(), // 20
        new Webbing(),         // 21
        new Anemia(),          // 22
        new BloodDisease(),    // 23
        new PoisonSpit(),      // 24
        new TrueFear(),        // 25
        new ColossalBlow(),    // 26
        new LifeDrain(),       // 27
        new ColossalRage(),    // 28
    };

    public sealed class GraspingClaw : SpecialAbility
    {
        private const string ModName = "PetGraspingClaw";
    
        private static readonly Dictionary<Mobile, TimerExecutionToken> _table = new();
    
        public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
        public override int ManaCost => 30;
    
        public GraspingClaw()
        {
        }
    
        public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
        {
            if (defender == null || defender.Deleted)
            {
                return;
            }
    
            if (_table.ContainsKey(defender))
            {
                RemoveEffects(defender);
                defender.SendLocalizedMessage(1070837); // The creature lands another blow in your weakened state.
            }
            else
            {
                // The blow from the creature's claws has made you more susceptible to physical attacks.
                defender.SendLocalizedMessage(1070836);
            }
    
            defender.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
            defender.AddResistanceMod(
                new ResistanceMod(ResistanceType.Physical, ModName, -(defender.PhysicalResistance * 15 / 100))
            );
    
            Timer.StartTimer(TimeSpan.FromSeconds(5.0), () => OnExpire(defender), out var token);
            _table[defender] = token;
        }
    
        private static void RemoveEffects(Mobile m)
        {
            if (m == null)
            {
                return;
            }
    
            if (_table.Remove(m, out var token))
            {
                token.Cancel();
            }
    
            m.RemoveResistanceMod(ModName);
        }
    
        private static void OnExpire(Mobile m)
        {
            if (m != null && !m.Deleted)
            {
                m.SendLocalizedMessage(1070838); // Your resistance to physical attacks has returned.
            }
    
            RemoveEffects(m);
        }
    }

    public sealed class RuneCorruption : SpecialAbility
    {
        private static readonly string[] ModNames =
        {
            "PetRuneCorruptionPhysical", "PetRuneCorruptionFire", "PetRuneCorruptionCold",
            "PetRuneCorruptionPoison", "PetRuneCorruptionEnergy"
        };
    
        private static readonly Dictionary<Mobile, TimerExecutionToken> _table = new();
    
        public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
        public override int ManaCost => 30;
    
        public RuneCorruption()
        {
        }
    
        public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
        {
            if (defender == null || defender.Deleted)
            {
                return;
            }
    
            // ServUO looked the entry up by attacker and stored it by defender, so its refresh branch was
            // dead code; the defender is used for both here.
            if (_table.ContainsKey(defender))
            {
                RemoveEffects(defender);
                defender.SendLocalizedMessage(1070845); // The creature continues to corrupt your armor!
            }
            else
            {
                defender.SendLocalizedMessage(1070846); // The creature magically corrupts your armor!
            }
    
            var phys = GetOffset(defender.PhysicalResistance);
            var fire = GetOffset(defender.FireResistance);
            var cold = GetOffset(defender.ColdResistance);
            var poison = GetOffset(defender.PoisonResistance);
            var energy = GetOffset(defender.EnergyResistance);
    
            AddMod(defender, ResistanceType.Physical, ModNames[0], phys);
            AddMod(defender, ResistanceType.Fire, ModNames[1], fire);
            AddMod(defender, ResistanceType.Cold, ModNames[2], cold);
            AddMod(defender, ResistanceType.Poison, ModNames[3], poison);
            AddMod(defender, ResistanceType.Energy, ModNames[4], energy);
    
            defender.FixedEffect(0x37B9, 10, 5);
    
            Timer.StartTimer(TimeSpan.FromSeconds(5.0), () => OnExpire(defender), out var token);
            _table[defender] = token;
    
            (defender as PlayerMobile)?.AddBuff(
                new BuffInfo(
                    BuffIcon.RuneBeetleCorruption,
                    1153796,
                    1153823,
                    TimeSpan.FromSeconds(5.0),
                    $"{phys}\t{cold}\t{poison}\t{energy}\t{fire}"
                )
            );
        }
    
        /// <summary>
        /// ServUO halves the resistance on modern-era shards and caps the drained amount at 70 before
        /// that.
        /// </summary>
        private static int GetOffset(int resistance)
        {
            if (resistance <= 0)
            {
                return 0;
            }
    
            return Core.ML ? resistance / 2 : Math.Min(resistance, 70);
        }
    
        private static void AddMod(Mobile m, ResistanceType type, string name, int offset)
        {
            if (offset > 0)
            {
                m.AddResistanceMod(new ResistanceMod(type, name, -offset));
            }
        }
    
        private static void RemoveEffects(Mobile m)
        {
            if (m == null)
            {
                return;
            }
    
            if (_table.Remove(m, out var token))
            {
                token.Cancel();
            }
    
            for (var i = 0; i < ModNames.Length; i++)
            {
                m.RemoveResistanceMod(ModNames[i]);
            }
    
            (m as PlayerMobile)?.RemoveBuff(BuffIcon.RuneBeetleCorruption);
        }
    
        private static void OnExpire(Mobile m)
        {
            if (m != null && !m.Deleted)
            {
                m.SendMessage("The corruption of your armor has worn off");
            }
    
            RemoveEffects(m);
        }
    }

    public sealed class TailSwipe : SpecialAbility
    {
        public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
        public override int ManaCost => 30;
    
        public TailSwipe()
        {
        }
    
        public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
        {
            if (defender == null || defender.Deleted)
            {
                return;
            }
    
            if (Utility.RandomBool())
            {
                // You're stunned as the creature's tail knocks the wind out of you.
                defender.SendLocalizedMessage(1112554);
    
                defender.PlaySound(0x204);
                defender.FixedEffect(0x376A, 6, 1);
    
                defender.Paralyze(TimeSpan.FromSeconds(3));
            }
            else
            {
                // You're left confused as the creature's tail catches you right in the face!
                defender.SendLocalizedMessage(1112555);
    
                defender.PlaySound(0x204);
                defender.FixedEffect(0x376A, 6, 1);
    
                defender.AddStatMod(new StatMod(StatType.Dex, "PetTailSwipeDex", -20, TimeSpan.FromSeconds(5)));
                defender.AddStatMod(new StatMod(StatType.Int, "PetTailSwipeInt", -20, TimeSpan.FromSeconds(5)));
            }
        }
    }

    public sealed class ColossalBlow : SpecialAbility
    {
        public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
        public override bool NaturalAbility => true;
        public override int ManaCost => 0;
        public override double TriggerChance => 0.3;
        public override TimeSpan CooldownDuration => TimeSpan.FromSeconds(10.0);
    
        public ColossalBlow()
        {
        }
    
        public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
        {
            if (defender == null || defender.Deleted)
            {
                return;
            }
    
            // ModernUO's only Animate overload takes an animation id, so ServUO's Core.SA death animation
            // cannot be expressed and the numeric call is always used.
            defender.Animate(21, 6, 1, true, false, 0);
    
            creature.PlaySound(0xEE);
            defender.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1070696); // You have been stunned by a colossal blow!
    
            if (creature.Weapon is BaseWeapon weapon)
            {
                weapon.OnHit(creature, defender);
            }
    
            if (defender.Deleted || !defender.Alive)
            {
                return;
            }
    
            defender.Frozen = true;
    
            Timer.DelayCall(TimeSpan.FromSeconds(5.0), RecoverSenses, defender);
        }
    
        private static void RecoverSenses(Mobile victim)
        {
            if (victim == null || victim.Deleted)
            {
                return;
            }
    
            victim.Frozen = false;
            victim.Combatant = null;
            victim.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1070695); // You recover your senses.
        }
    }
}

/// <summary>
/// The creature hits with a burst of fire.
/// </summary>
public sealed class AngryFire : SpecialAbility
{
    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 30;

    public AngryFire()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        AOS.Damage(defender, creature, Utility.RandomMinMax(30, 40), 60, 20, 0, 0, 20);

        defender.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
        defender.PlaySound(0x208);

        defender.SendLocalizedMessage(1070823); // The creature hits you with its Angry Fire.
    }
}

/// <summary>
/// Halves the target's energy resistance for ten seconds. The native resistance-mod list is keyed by
/// name, so refreshing the effect drops the old entry before the new one is added.
/// </summary>
public sealed class ConductiveBlast : SpecialAbility
{
    private const string ModName = "PetConductiveBlast";

    private static readonly Dictionary<Mobile, TimerExecutionToken> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 30;

    public ConductiveBlast()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        if (_table.ContainsKey(defender))
        {
            RemoveEffects(defender);
            defender.SendLocalizedMessage(1070828); // The creature continues to hinder your energy resistance!
        }
        else
        {
            // The creature's attack has made you more susceptible to energy attacks!
            defender.SendLocalizedMessage(1070827);
        }

        defender.FixedEffect(0x37B9, 10, 5);
        defender.AddResistanceMod(
            new ResistanceMod(ResistanceType.Energy, ModName, -(defender.EnergyResistance / 2))
        );

        Timer.StartTimer(TimeSpan.FromSeconds(10.0), () => OnExpire(defender), out var token);
        _table[defender] = token;
    }

    /// <summary>Drops the debuff and its timer without messaging; also used when refreshing.</summary>
    private static void RemoveEffects(Mobile m)
    {
        if (m == null)
        {
            return;
        }

        if (_table.Remove(m, out var token))
        {
            token.Cancel();
        }

        m.RemoveResistanceMod(ModName);
    }

    private static void OnExpire(Mobile m)
    {
        if (m != null && !m.Deleted)
        {
            m.SendLocalizedMessage(1070829); // Your resistance to energy attacks has returned.
        }

        RemoveEffects(m);
    }
}

/// <summary>
/// Full dragon breath: a per-creature-type profile drives the wind-up, the projectile and the damage
/// mix.
/// </summary>
public sealed class DragonBreath : SpecialAbility
{
    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.Think;
    public override int ManaCost => 30;
    public override int MaxRange => 12;

    // ServUO derived the cooldown from the triggering creature's DragonBreathDefinition. The native
    // pipeline carries one min/max pair per ability, so the default profile's 30-45 second jitter is
    // used for every creature and the shorter Leviathan/RedDeath/CrystalHydra windows are lost.
    public override TimeSpan MinTriggerCooldown => TimeSpan.FromSeconds(30.0);
    public override TimeSpan MaxTriggerCooldown => TimeSpan.FromSeconds(45.0);

    public DragonBreath()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender != null && !defender.Deleted)
        {
            DoBreath(creature, defender);
        }
    }

    public void DoBreath(BaseCreature creature, Mobile target)
    {
        var def = DragonBreathDefinition.GetDefinition(creature);

        creature.RevealingAction();

        if (creature.AIObject != null)
        {
            creature.AIObject.NextMove = Core.TickCount + (int)(def.StallTime * 1000);
        }

        creature.PlaySound(creature.GetAngerSound());

        // ModernUO's only Animate overload takes an animation id, so ServUO's Core.SA pillage
        // animation cannot be expressed and the numeric anger animation is always used.
        creature.Animate(def.AngerAnimation, 5, 1, true, false, 0);

        creature.Direction = creature.GetDirectionTo(target.Location);

        if (def.AttacksMultipleTargets)
        {
            using var targets = PooledRefList<Mobile>.Create();

            foreach (var m in creature.GetMobilesInRange(5))
            {
                if (m != target && SpellHelper.ValidIndirectTarget(creature, m) &&
                    m.InRange(creature.Location, MaxRange))
                {
                    targets.Add(m);
                }
            }

            for (var i = 0; i < 5; i++)
            {
                Mobile m;

                if (i == 0)
                {
                    m = target;
                }
                else if (targets.Count > 0)
                {
                    var index = Utility.Random(targets.Count);
                    m = targets[index];
                    targets.RemoveAt(index);
                }
                else
                {
                    break;
                }

                Timer.DelayCall(TimeSpan.FromSeconds(def.EffectDelay), BreathEffect_Callback, creature, m, def);
            }

            return;
        }

        Timer.DelayCall(TimeSpan.FromSeconds(def.EffectDelay), BreathEffect_Callback, creature, target, def);
    }

    public void BreathEffect_Callback(BaseCreature creature, Mobile target, DragonBreathDefinition def)
    {
        if (creature.Deleted || target == null || target.Deleted || !target.Alive || !creature.CanBeHarmful(target))
        {
            return;
        }

        creature.RevealingAction();
        creature.PlaySound(def.EffectSound);

        Effects.SendMovingEffect(
            creature,
            target,
            def.EffectItemID,
            def.EffectSpeed,
            def.EffectDuration,
            def.EffectFixedDir,
            def.EffectExplodes,
            def.EffectHue,
            def.EffectRenderMode
        );

        Timer.DelayCall(TimeSpan.FromSeconds(def.DamageDelay), BreathDamage_Callback, creature, target, def);
    }

    public void BreathDamage_Callback(BaseCreature creature, Mobile target, DragonBreathDefinition def)
    {
        if (target == null || target.Deleted)
        {
            return;
        }

        if (target is BaseCreature { BreathImmune: true })
        {
            return;
        }

        if (creature.CanBeHarmful(target))
        {
            creature.DoHarmful(target);
            BreathDealDamage(creature, target, def);
        }
    }

    public void BreathDealDamage(BaseCreature creature, Mobile target, DragonBreathDefinition def)
    {
        if (Evasion.CheckSpellEvasion(target))
        {
            return;
        }

        // ServUO routed every element through IElementalCreature; that interface does not exist in
        // ModernUO, so the profile's flat split is used.
        AOS.Damage(
            target,
            creature,
            BreathComputeDamage(creature, def),
            false,
            def.PhysicalDamage,
            def.FireDamage,
            def.ColdDamage,
            def.PoisonDamage,
            def.EnergyDamage,
            def.ChaosDamage,
            def.DirectDamage
        );
    }

    public int BreathComputeDamage(BaseCreature creature, DragonBreathDefinition def)
    {
        var damage = (int)(creature.Hits * def.DamageScalar);

        if (creature.IsParagon)
        {
            damage = (int)(damage / Paragon.HitsBuff);
        }

        return Math.Min(damage, 200);
    }

    /// <summary>
    /// Per-creature-type breath profile, ported from ServUO. ServUO named its creatures with
    /// <c>typeof</c>; the names are resolved through the core so the file compiles against a stock
    /// core that lacks some of them.
    /// </summary>
    public sealed class DragonBreathDefinition
    {
        private static readonly List<DragonBreathDefinition> _definitions = new();
        private static bool _initialized;

        public Type[] Uses { get; }

        // Base damage given is: CurrentHitPoints * DamageScalar
        public double DamageScalar { get; }

        // Creature stops moving for this long while breathing
        public double StallTime { get; }

        // Effect is sent this long after the anger sound and animation
        public double EffectDelay { get; }

        // Damage is given this long after the effect is sent
        public double DamageDelay { get; }

        public int PhysicalDamage { get; }
        public int FireDamage { get; }
        public int ColdDamage { get; }
        public int PoisonDamage { get; }
        public int EnergyDamage { get; }
        public int ChaosDamage { get; }
        public int DirectDamage { get; }

        public double MinDelay { get; }
        public double MaxDelay { get; }

        public int EffectItemID { get; }
        public int EffectSpeed { get; }
        public int EffectDuration { get; }
        public bool EffectExplodes { get; }
        public bool EffectFixedDir { get; }
        public int EffectHue { get; }
        public int EffectRenderMode { get; }
        public int EffectSound { get; }

        public int AngerAnimation { get; }
        public bool AttacksMultipleTargets { get; }

        public DragonBreathDefinition(
            double scalar, double stallTime, double effectDelay, double damageDelay,
            int physDamage, int fireDamage, int coldDamage, int poisonDamage, int energyDamage,
            int chaosDamage, int directDamage, double minDelay, double maxDelay,
            int effectID, int effectSpeed, int effectDuration, bool explodes, bool fixedDirection,
            int effectHue, int renderMode, int effectSound, int angerAnimation,
            bool attacksMultiples = false, string[] uses = null
        )
        {
            DamageScalar = scalar;
            StallTime = stallTime;
            EffectDelay = effectDelay;
            DamageDelay = damageDelay;

            PhysicalDamage = physDamage;
            FireDamage = fireDamage;
            ColdDamage = coldDamage;
            PoisonDamage = poisonDamage;
            EnergyDamage = energyDamage;
            ChaosDamage = chaosDamage;
            DirectDamage = directDamage;

            MinDelay = minDelay;
            MaxDelay = maxDelay;

            EffectItemID = effectID;
            EffectSpeed = effectSpeed;
            EffectDuration = effectDuration;
            EffectExplodes = explodes;
            EffectFixedDir = fixedDirection;
            EffectHue = effectHue;
            EffectRenderMode = renderMode;
            EffectSound = effectSound;
            AngerAnimation = angerAnimation;
            AttacksMultipleTargets = attacksMultiples;

            Uses = uses == null ? Array.Empty<Type>() : PetTrainingAreaEffects.ResolveTypes(uses);
        }

        public static List<DragonBreathDefinition> Definitions
        {
            get
            {
                if (!_initialized)
                {
                    Initialize();
                }

                return _definitions;
            }
        }

        /// <summary>
        /// Idempotent. Also runs on first use so a bootstrap that never calls it cannot leave
        /// <see cref="Definitions"/> empty.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            _definitions.Clear();

            var scalar = Core.AOS ? 0.16 : 0.05;

            // Default
            _definitions.Add(new DragonBreathDefinition(
                scalar, 1.0, 1.3, 1.0,
                0, 100, 0, 0, 0, 0, 0,
                30.0, 45.0,
                0x36D4, 5, 0, false, false, 0, 0, 0x227, 12
            ));

            // Skeletal Dragon / Renowned
            //_definitions.Add(new DragonBreathDefinition(
            //    scalar, 1.0, 1.3, 1.0,
            //    0, 0, 100, 0, 0, 0, 0,
            //    30.0, 45.0,
            //    0x36D4, 5, 0, false, false, 0x480, 0, 0x227, 12,
            //    false, new[] { "SkeletalDragonRenowned", "SkeletalDragon" }
            //));

            // Leviathan
            _definitions.Add(new DragonBreathDefinition(
                0.05, 1.0, 1.3, 1.0,
                70, 0, 30, 0, 0, 0, 0,
                5.0, 7.5,
                0x36D4, 5, 0, false, false, 0x1ED, 0, 0x227, 12,
                false, new[] { "Leviathan" }
            ));

            // Red Death
            _definitions.Add(new DragonBreathDefinition(
                0.16, 1.0, 1.3, 1.0,
                0, 0, 0, 0, 0, 100, 0,
                5.0, 7.5,
                0x36D4, 5, 0, false, false, 0x1ED, 0, 0x227, 12,
                false, new[] { "RedDeath" }
            ));

            // Frost Dragon / Cold Drake
            //_definitions.Add(new DragonBreathDefinition(
            //    scalar, 1.0, 1.3, 1.0,
            //    0, 0, 100, 0, 0, 0, 0,
            //    30.0, 45.0,
            //    0x36D4, 5, 0, false, false, 1264, 0, 0x227, 12,
            //    false, new[] { "FrostDragon", "ColdDrake" }
            //));

            // Antlion
            _definitions.Add(new DragonBreathDefinition(
                scalar, 1.0, 1.3, 1.0,
                0, 0, 0, 100, 0, 0, 0,
                30.0, 45.0,
                0x36D4, 5, 0, false, false, 0x3F, 0, 0, 12,
                false, new[] { "AntLion" }
            ));

            // Rend
            _definitions.Add(new DragonBreathDefinition(
                0.06, 1.0, 1.3, 1.0,
                0, 100, 0, 0, 0, 0, 0,
                30.0, 45.0,
                0x36D4, 5, 0, false, false, 0, 0, 0x227, 12,
                false, new[] { "Rend" }
            ));

            // Crystal Sea Serpent
            _definitions.Add(new DragonBreathDefinition(
                0.55, 1.0, 1.3, 1.0,
                0, 0, 50, 0, 50, 0, 0,
                30.0, 45.0,
                0x36D4, 5, 0, false, false, 0x1ED, 0, 0x227, 12,
                false, new[] { "CrystalSeaSerpent" }
            ));

            // Crystal Hydra
            //_definitions.Add(new DragonBreathDefinition(
            //    0.13, 1.0, 1.3, 1.0,
            //    0, 0, 100, 0, 0, 0, 0,
            //    5.0, 7.0,
            //    0x36D4, 5, 0, false, false, 0x47E, 0, 0x56D, 12,
            //    true, new[] { "CrystalHydra" }
            //));
        }

        public static DragonBreathDefinition GetDefinition(BaseCreature bc)
        {
            var definitions = Definitions;
            var type = bc.GetType();

            for (var i = 0; i < definitions.Count; i++)
            {
                var uses = definitions[i].Uses;

                for (var j = 0; j < uses.Length; j++)
                {
                    if (uses[j] == type)
                    {
                        return definitions[i];
                    }
                }
            }

            return definitions[0]; // Default
        }
    }
}

/// <summary>
/// The blow from the creature's claws weakens the target against physical damage for five seconds.
/// </summary>

/// <summary>
/// Fans the target with fire: a flat fire hit plus a 25 point fire resistance penalty for six
/// seconds.
/// </summary>
public sealed class Inferno : SpecialAbility
{
    private const string ModName = "PetInferno";

    private static readonly Dictionary<Mobile, TimerExecutionToken> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 30;

    public Inferno()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        if (_table.ContainsKey(defender))
        {
            RemoveEffects(defender);
        }

        // The creature fans you with fire, reducing your resistance to fire attacks.
        defender.SendLocalizedMessage(1070833);

        Effects.SendLocationParticles(defender, 0x3709, 10, 30, 5052);
        Effects.PlaySound(defender.Location, defender.Map, 0x208);

        defender.AddResistanceMod(new ResistanceMod(ResistanceType.Fire, ModName, -25));

        AOS.Damage(defender, creature, Utility.RandomMinMax(46, 79), 0, 100, 0, 0, 0);

        Timer.StartTimer(TimeSpan.FromSeconds(6.0), () => OnExpire(defender), out var token);
        _table[defender] = token;
    }

    private static void RemoveEffects(Mobile m)
    {
        if (m == null)
        {
            return;
        }

        if (_table.Remove(m, out var token))
        {
            token.Cancel();
        }

        m.RemoveResistanceMod(ModName);
    }

    private static void OnExpire(Mobile m)
    {
        if (m != null && !m.Deleted)
        {
            m.SendLocalizedMessage(1070834); // Your resistance to fire attacks has returned.
        }

        RemoveEffects(m);
    }
}

/// <summary>
/// A short, sharp bolt of energy.
/// </summary>
public sealed class LightningForce : SpecialAbility
{
    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 30;

    public LightningForce()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        Effects.SendBoltEffect(defender, true);
        AOS.Damage(defender, creature, Utility.RandomMinMax(15, 20), 0, 0, 0, 0, 100);
    }
}

/// <summary>
/// Siphons mana from everything within eight tiles into the creature. Triggers both when dealing and
/// when taking melee damage.
/// </summary>
public sealed class ManaDrain : SpecialAbility
{
    public override MonsterAbilityTrigger AbilityTrigger =>
        MonsterAbilityTrigger.GiveMeleeDamage | MonsterAbilityTrigger.TakeMeleeDamage;

    public ManaDrain()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (creature.Map == null || creature.Map == Map.Internal)
        {
            return;
        }

        // Targets are buffered first: draining mutates state the spatial enumerator reads.
        using var targets = PooledRefQueue<Mobile>.Create();

        foreach (var m in creature.GetMobilesInRange(8))
        {
            if (AreaEffect.ValidTarget(creature, m))
            {
                targets.Enqueue(m);
            }
        }

        while (targets.Count > 0)
        {
            var m = targets.Dequeue();

            if (m.Deleted || !m.Alive)
            {
                continue;
            }

            creature.DoHarmful(m, false);

            m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
            m.PlaySound(0x231);

            m.SendMessage("You feel the mana drain out of you!");

            var toDrain = Utility.RandomMinMax(40, 60);

            creature.Mana += toDrain;
            m.Mana -= toDrain;
        }
    }
}

/// <summary>
/// The creature's breath keeps burning for ten one-second ticks. Re-triggering restarts the burn.
/// </summary>
public sealed class RagingBreath : SpecialAbility
{
    private const int MaxTicks = 10;

    private sealed class Effect
    {
        public TimerExecutionToken Token;
        public int Ticks;
    }

    private static readonly Dictionary<Mobile, Effect> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 30;

    public RagingBreath()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        if (_table.ContainsKey(defender))
        {
            EndFire(defender, false);
            defender.SendLocalizedMessage(1070841); // The creature's breath continues to burn you!
        }
        else
        {
            defender.SendLocalizedMessage(1070842); // The creature's breath is burning you!
        }

        SendBurningEffect(defender);
        defender.PlaySound(520);

        var entry = new Effect();
        _table[defender] = entry;

        Timer.StartTimer(
            TimeSpan.FromSeconds(1.0),
            TimeSpan.FromSeconds(1.0),
            () => OnTick(creature, defender, entry),
            out var token
        );

        entry.Token = token;
    }

    /// <summary>
    /// ServUO sent two ParticleEffect packets; the location-particle packet carries the same item id,
    /// speed, duration, hue, render mode, particle id and layer fields.
    /// </summary>
    private static void SendBurningEffect(Mobile defender)
    {
        var layer = defender is BaseCreature ? (EffectLayer)163 : (EffectLayer)45;

        Effects.SendTargetParticles(defender, 0x3709, 1, 15, 2735, 0, 4, layer);
        Effects.SendTargetParticles(defender, 0x3709, 10, 30, 0, 0, 0, layer);
    }

    private static void OnTick(BaseCreature creature, Mobile defender, Effect entry)
    {
        entry.Ticks++;

        if (entry.Ticks > MaxTicks || defender.Deleted || !defender.Alive)
        {
            EndFire(defender, true);
            return;
        }

        SendBurningEffect(defender);
        AOS.Damage(defender, creature, 10, 0, 100, 0, 0, 0);
    }

    public static void EndFire(Mobile m) => EndFire(m, true);

    private static void EndFire(Mobile m, bool message)
    {
        if (m == null)
        {
            return;
        }

        if (_table.Remove(m, out var entry))
        {
            entry.Token.Cancel();
        }

        if (message && !m.Deleted)
        {
            m.SendLocalizedMessage(1070843); // The fiery breath dissipates.
        }
    }
}

/// <summary>
/// Reflects half of an incoming melee or spell hit back at the attacker and cancels the rest.
/// <para>
/// ModernUO's <c>DoEffects</c> has no <c>ref int damage</c> parameter, so the reflect lives in the
/// native damage-alteration hooks, which do. Those run before the trigger pipeline for the same hit,
/// and the cooldown started here stops the pipeline from running the effect a second time.
/// </para>
/// </summary>
public sealed class Repel : SpecialAbility
{
    public override MonsterAbilityTrigger AbilityTrigger =>
        MonsterAbilityTrigger.TakeMeleeDamage | MonsterAbilityTrigger.TakeSpellDamage;

    public override int ManaCost => 30;

    public Repel()
    {
    }

    public override void AlterMeleeDamageFrom(BaseCreature source, Mobile target, ref int damage) =>
        Reflect(source, target, MonsterAbilityTrigger.TakeMeleeDamage, ref damage);

    public override void AlterSpellDamageFrom(BaseCreature source, Mobile target, ref int damage) =>
        Reflect(source, target, MonsterAbilityTrigger.TakeSpellDamage, ref damage);

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        // Handled by Reflect: the damage value only exists in the alteration hooks.
    }

    private void Reflect(BaseCreature source, Mobile target, MonsterAbilityTrigger trigger, ref int damage)
    {
        if (target == null || target.Deleted || damage <= 0 || !CheckMana(source) || !Validate(source, target))
        {
            return;
        }

        target.SendLocalizedMessage(1070844); // The creature repels the attack back at you.
        target.FixedEffect(0x37B9, 10, 5);

        AOS.Damage(target, source, damage / 2, false, 0, 0, 0, 0, 0, 0, 100);

        damage = 0;

        // Reuses the base trigger so the mana cost and the 30 second cooldown are applied once.
        base.Trigger(trigger, source, target);
    }
}

/// <summary>
/// Searing wounds cauterises on impact, leaving the target unable to bleed for ten seconds.
/// </summary>
public sealed class SearingWounds : SpecialAbility
{
    private static readonly Dictionary<Mobile, TimerExecutionToken> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 25;

    public SearingWounds()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted || _table.ContainsKey(defender))
        {
            return;
        }

        defender.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
        defender.SendLocalizedMessage(1151177); // The searing attack cauterizes the wound on impact.

        Timer.StartTimer(TimeSpan.FromSeconds(10.0), () => EndEffects(defender), out var token);
        _table[defender] = token;
    }

    public static bool IsUnderEffects(Mobile m) => _table.ContainsKey(m);

    private static void EndEffects(Mobile m)
    {
        if (m == null)
        {
            return;
        }

        if (_table.Remove(m, out var token))
        {
            token.Cancel();
        }
    }
}

/// <summary>
/// Steals life with every melee blow and feeds a fifth of it back over the next twenty-five seconds.
/// </summary>
public sealed class StealLife : SpecialAbility
{
    private const int TickCount = 5;

    private sealed class Effect
    {
        public TimerExecutionToken Token;
        public int Ticks;
        public int ToHeal;
    }

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 30;

    public StealLife()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        defender.FixedParticles(0x374A, 1, 15, 5054, 23, 7, EffectLayer.Head);
        defender.PlaySound(0x1F9);

        // ServUO forwarded the triggering melee hit's damage here. DoEffects has no damage
        // parameter, so the creature's own melee roll stands in for it.
        var damage = creature.DamageMax > 0
            ? Utility.RandomMinMax(creature.DamageMin, creature.DamageMax)
            : 10;

        AOS.Damage(defender, creature, damage, 0, 0, 0, 0, 100);

        defender.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
        defender.PlaySound(0x231);

        var entry = new Effect { ToHeal = Math.Max(1, damage / TickCount) };

        Timer.StartTimer(
            TimeSpan.FromSeconds(5.0),
            TimeSpan.FromSeconds(5.0),
            () => OnTick(creature, defender, entry),
            out var token
        );

        entry.Token = token;
    }

    private static void OnTick(BaseCreature creature, Mobile defender, Effect entry)
    {
        entry.Ticks++;

        if (defender.Deleted || creature.Deleted || !defender.Alive)
        {
            entry.Token.Cancel();
            return;
        }

        defender.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
        creature.Hits = Math.Min(creature.HitsMax, creature.Hits + entry.ToHeal);

        if (entry.Ticks >= TickCount)
        {
            entry.Token.Cancel();
        }
    }
}

/// <summary>
/// A venomous bite poisons everything within three tiles.
/// </summary>
public sealed class VenomousBite : SpecialAbility
{
    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 30;
    public override MagicalAbility RequiredSchool => MagicalAbility.Poisoning;

    public VenomousBite()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        // ServUO's GetHitPoison() is BaseCreature.HitPoison in ModernUO, and may be null.
        var poison = creature.HitPoison;

        if (poison == null)
        {
            return;
        }

        using var targets = PooledRefQueue<Mobile>.Create();

        targets.Enqueue(defender);

        foreach (var m in creature.GetMobilesInRange(3))
        {
            if (m != defender && AreaEffect.ValidTarget(creature, m))
            {
                targets.Enqueue(m);
            }
        }

        var poisoned = 0;

        while (targets.Count > 0)
        {
            var m = targets.Dequeue();

            if (m.Deleted || !m.Alive)
            {
                continue;
            }

            m.PlaySound(0xDD);
            m.FixedParticles(0x3728, 244, 25, 9941, 1266, 0, EffectLayer.Waist);

            m.SendLocalizedMessage(1008097, false, creature.Name); //  : poisoned you!

            m.ApplyPoison(creature, poison);
            poisoned++;
        }

        if (!creature.Controlled || poisoned == 0)
        {
            return;
        }

        var profile = PetTrainingHelper.GetAbilityProfile(creature);

        if (profile != null && profile.HasAbility(MagicalAbility.Poisoning) || 0.2 > Utility.RandomDouble())
        {
            creature.CheckSkill(SkillName.Poisoning, 0, creature.Skills[SkillName.Poisoning].Cap);
        }
    }
}

/// <summary>
/// A vicious bite leaves a festering wound that grows worse every twenty seconds.
/// </summary>
public sealed class ViciousBite : SpecialAbility
{
    private const int MaxTicks = 20;

    private sealed class Effect
    {
        public TimerExecutionToken Token;
        public int Ticks;
    }

    private static readonly Dictionary<Mobile, Effect> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 20;

    public ViciousBite()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted || _table.ContainsKey(defender))
        {
            return;
        }

        defender.PlaySound(0x1324);
        defender.SendLocalizedMessage(1234567); // The creature gives you a particular vicious bite.
        Effects.SendLocationParticles(defender, 0x37CC, 1, 40, 97, 3, 9917, 0);

        var entry = new Effect();
        _table[defender] = entry;

        Timer.StartTimer(
            TimeSpan.FromSeconds(20.0),
            TimeSpan.FromSeconds(20.0),
            () => OnTick(creature, defender, entry),
            out var token
        );

        entry.Token = token;
    }

    private static void OnTick(BaseCreature creature, Mobile defender, Effect entry)
    {
        entry.Ticks++;

        AOS.Damage(defender, creature, entry.Ticks * 5, false, 0, 0, 0, 0, 0, 0, 100);

        if (!defender.Deleted)
        {
            defender.SendLocalizedMessage(1112473); // Your vicious wound is festering!
        }

        if (entry.Ticks < MaxTicks && !defender.Deleted && defender.Alive && !defender.IsDeadBondedPet)
        {
            return;
        }

        entry.Token.Cancel();
        _table.Remove(defender);
    }
}

/// <summary>
/// Corrupts the target's armour, halving every positive resistance for five seconds.
/// </summary>

/// <summary>
/// Steals five points of life per second for five seconds and hands them to the creature.
/// </summary>
public sealed class LifeLeech : SpecialAbility
{
    private const int TickCount = 5;

    private sealed class Effect
    {
        public TimerExecutionToken Token;
        public int Ticks;
    }

    private static readonly Dictionary<Mobile, Effect> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 5;

    // ServUO rolled a fresh 10-20 second cooldown on every trigger; the native pipeline expresses
    // that as a min/max range instead of a per-trigger value.
    public override TimeSpan MinTriggerCooldown => TimeSpan.FromSeconds(10.0);
    public override TimeSpan MaxTriggerCooldown => TimeSpan.FromSeconds(20.0);

    public LifeLeech()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        EndLifeDrain(defender, false);

        var entry = new Effect();
        _table[defender] = entry;

        defender.SendLocalizedMessage(1070848); // You feel your life force being stolen away!

        Timer.StartTimer(
            TimeSpan.FromSeconds(1.0),
            TimeSpan.FromSeconds(1.0),
            () => OnTick(creature, defender, entry),
            out var token
        );

        entry.Token = token;
    }

    private static void OnTick(BaseCreature creature, Mobile defender, Effect entry)
    {
        entry.Ticks++;

        if (defender.Deleted || !defender.Alive)
        {
            EndLifeDrain(defender, true);
            return;
        }

        DrainLife(defender, creature);

        if (entry.Ticks >= TickCount)
        {
            EndLifeDrain(defender, true);
        }
    }

    public static void DrainLife(Mobile m, Mobile from)
    {
        var damageGiven = AOS.Damage(m, from, 5, 0, 0, 0, 0, 100);

        if (from != null && !from.Deleted)
        {
            from.Hits += damageGiven;
        }

        m.SendLocalizedMessage(1070847); // The creature continues to steal your life force!
    }

    public static void EndLifeDrain(Mobile m) => EndLifeDrain(m, true);

    private static void EndLifeDrain(Mobile m, bool message)
    {
        if (m == null)
        {
            return;
        }

        if (_table.Remove(m, out var entry))
        {
            entry.Token.Cancel();

            if (message && !m.Deleted)
            {
                m.SendLocalizedMessage(1070849); // The drain on your life force is gone.
            }
        }
    }
}

/// <summary>
/// Sticky skin slows the target's attack speed for ten seconds. ModernUO has no attack-speed hook on
/// <see cref="Mobile"/>, so the slow is exposed through <see cref="IsUnderEffects"/> for callers that
/// can apply it.
/// </summary>
public sealed class StickySkin : SpecialAbility
{
    private static readonly TimeSpan _duration = TimeSpan.FromSeconds(10.0);

    private static readonly Dictionary<Mobile, TimerExecutionToken> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger =>
        MonsterAbilityTrigger.GiveMeleeDamage | MonsterAbilityTrigger.TakeMeleeDamage;

    public override int ManaCost => 5;

    public StickySkin()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        defender.SendLocalizedMessage(1153752); // Your attack speed has been slowed.

        // Re-applying restarts the ten second window rather than stacking.
        if (_table.Remove(defender, out var existing))
        {
            existing.Cancel();
        }

        Timer.StartTimer(_duration, () => RemoveEffects(defender), out var token);
        _table[defender] = token;
    }

    public static bool IsUnderEffects(Mobile m) => _table.ContainsKey(m);

    public static void RemoveEffects(Mobile m)
    {
        if (m == null)
        {
            return;
        }

        if (_table.Remove(m, out var token))
        {
            token.Cancel();
        }
    }
}

/// <summary>
/// The tail either knocks the wind out of the target or leaves it confused.
/// </summary>

/// <summary>
/// A flurry of twigs weakens the target against physical damage for ten seconds.
/// </summary>
public sealed class FlurryForce : SpecialAbility
{
    private const string ModName = "PetFlurryForce";

    private static readonly Dictionary<Mobile, TimerExecutionToken> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override bool NaturalAbility => true;
    public override int ManaCost => 0;

    public FlurryForce()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        if (_table.ContainsKey(defender))
        {
            RemoveEffects(defender);
            defender.SendLocalizedMessage(1070851); // The creature lands another blow in your weakened state.
        }
        else
        {
            // The creature's flurry of twigs has made you more susceptible to physical attacks!
            defender.SendLocalizedMessage(1070850);
        }

        defender.FixedEffect(0x37B9, 10, 5);
        defender.AddResistanceMod(
            new ResistanceMod(ResistanceType.Physical, ModName, -(defender.PhysicalResistance * 15 / 100))
        );

        Timer.StartTimer(TimeSpan.FromSeconds(10.0), () => OnExpire(defender), out var token);
        _table[defender] = token;
    }

    private static void RemoveEffects(Mobile m)
    {
        if (m == null)
        {
            return;
        }

        if (_table.Remove(m, out var token))
        {
            token.Cancel();
        }

        m.RemoveResistanceMod(ModName);
    }

    private static void OnExpire(Mobile m)
    {
        if (m != null && !m.Deleted)
        {
            // ServUO sends the energy-resistance cliloc here too; kept for parity.
            m.SendLocalizedMessage(1070829);
        }

        RemoveEffects(m);
    }
}

/// <summary>
/// A rage that drains two hit points a second for five seconds.
/// </summary>
public sealed class Rage : SpecialAbility
{
    private const int TickCount = 5;

    private sealed class Effect
    {
        public TimerExecutionToken Token;
        public int Ticks;
    }

    private static readonly Dictionary<Mobile, Effect> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override bool NaturalAbility => true;
    public override int ManaCost => 0;

    public Rage()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        if (_table.ContainsKey(defender))
        {
            DoExpire(defender, false);
            defender.SendLocalizedMessage(1070825); // The creature continues to rage!
        }
        else
        {
            defender.SendLocalizedMessage(1070826); // The creature goes into a rage, inflicting heavy damage!
        }

        var entry = new Effect();
        _table[defender] = entry;

        Timer.StartTimer(
            TimeSpan.FromSeconds(1.0),
            TimeSpan.FromSeconds(1.0),
            () => OnTick(creature, defender, entry),
            out var token
        );

        entry.Token = token;
    }

    private static void OnTick(Mobile from, Mobile victim, Effect entry)
    {
        entry.Ticks++;

        if (victim.Deleted)
        {
            DoExpire(victim, false);
            return;
        }

        if (victim.Alive)
        {
            victim.Damage(2, from);
        }
        else
        {
            DoExpire(victim, false);
            return;
        }

        if (entry.Ticks >= TickCount)
        {
            DoExpire(victim, true);
        }
    }

    private static void DoExpire(Mobile m, bool message)
    {
        if (m == null)
        {
            return;
        }

        if (_table.Remove(m, out var entry))
        {
            entry.Token.Cancel();
        }

        if (message && !m.Deleted)
        {
            m.SendLocalizedMessage(1070824); // The creature's rage subsides.
        }
    }
}

/// <summary>
/// A creature-innate self heal. ServUO gated it on <c>BaseCreature.CheckHeal()</c>, which ModernUO
/// does not have; the stand-in heals 10-20 hit points while below three quarters health.
/// </summary>
public sealed class Heal : SpecialAbility
{
    private const double HealThreshold = 0.75;

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.Think;
    public override double TriggerChance => 1.0;
    public override bool RequiresCombatant => false;
    public override int ManaCost => 15;
    public override TimeSpan CooldownDuration => TimeSpan.MinValue;
    public override bool NaturalAbility => true;

    public Heal()
    {
    }

    /// <summary>
    /// ServUO passed the creature itself as the defender when it had no combatant; the native Think
    /// trigger passes <c>Combatant</c>, which is null out of combat.
    /// </summary>
    public override bool Validate(BaseCreature attacker, Mobile defender)
    {
        if (defender != null)
        {
            return base.Validate(attacker, defender);
        }

        return attacker.Alive && !attacker.IsDeadBondedPet;
    }

    /// <summary>
    /// ServUO only spent mana when the heal actually landed. Its cooldown was
    /// <see cref="TimeSpan.MinValue"/>, which the native pipeline already reads as "no cooldown", so
    /// no cooldown is armed here either.
    /// </summary>
    public override void Trigger(MonsterAbilityTrigger trigger, BaseCreature source, Mobile target)
    {
        if (!CheckMana(source) || !Validate(source, target) || source.Hits >= source.HitsMax * HealThreshold)
        {
            return;
        }

        if (!source.HasManaOverride)
        {
            source.Mana -= ManaCost;
        }

        DoEffects(source, target, trigger);
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (creature.Deleted || creature.Hits >= creature.HitsMax * HealThreshold)
        {
            return;
        }

        creature.Hits = Math.Min(creature.HitsMax, creature.Hits + Utility.RandomMinMax(10, 20));
    }
}

/// <summary>
/// A cacophonic howl that pins the target in place for thirty seconds.
/// </summary>
public sealed class HowlOfCacophony : SpecialAbility
{
    private static readonly TimeSpan _duration = TimeSpan.FromSeconds(30.0);

    private static readonly Dictionary<Mobile, TimerExecutionToken> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.GiveMeleeDamage;
    public override int ManaCost => 25;

    public HowlOfCacophony()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted || _table.ContainsKey(defender))
        {
            return;
        }

        defender.NetState.SendSpeedControl(SpeedControlSetting.Walk);

        // A cacophonic sound lambastes you, suppressing your ability to move.
        defender.SendLocalizedMessage(1072069);
        defender.PlaySound(0x584);

        (defender as PlayerMobile)?.AddBuff(
            new BuffInfo(BuffIcon.HowlOfCacophony, 1153793, 1153820, _duration, "60\t5\t5")
        );

        Timer.StartTimer(_duration, () => EndHowl(defender), out var token);
        _table[defender] = token;
    }

    public static bool IsUnderEffects(Mobile m) => _table.ContainsKey(m);

    private static void EndHowl(Mobile m)
    {
        if (m == null)
        {
            return;
        }

        if (_table.Remove(m, out var token))
        {
            token.Cancel();
        }

        (m as PlayerMobile)?.RemoveBuff(BuffIcon.HowlOfCacophony);
        m.NetState.SendSpeedControl(SpeedControlSetting.Disable);
    }
}

/// <summary>
/// Throws webbing at a random target within twelve tiles. ModernUO has no <c>SpiderWebbing</c> item,
/// so the web is drawn as a moving particle plus a location particle on the target and no item is
/// placed.
/// </summary>
public sealed class Webbing : SpecialAbility
{
    private const int WebItemID = 0x10D3;

    public override MonsterAbilityTrigger AbilityTrigger =>
        MonsterAbilityTrigger.GiveMeleeDamage | MonsterAbilityTrigger.TakeMeleeDamage;

    public override bool NaturalAbility => true;
    public override int ManaCost => 0;

    public Webbing()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (creature.Map == null || creature.Map == Map.Internal)
        {
            return;
        }

        using var targets = PooledRefQueue<Mobile>.Create();

        foreach (var m in creature.GetMobilesInRange(12))
        {
            if (AreaEffect.ValidTarget(creature, m))
            {
                targets.Enqueue(m);
            }
        }

        if (targets.Count == 0)
        {
            return;
        }

        var skip = Utility.Random(targets.Count);
        var target = targets.Dequeue();

        for (var i = 0; i < skip; i++)
        {
            target = targets.Dequeue();
        }

        creature.DoHarmful(target, false);
        creature.Direction = creature.GetDirectionTo(target.Location);

        Effects.SendMovingParticles(
            creature,
            target,
            WebItemID,
            12,
            0,
            false,
            false,
            0,
            0,
            9502,
            1,
            0,
            (EffectLayer)255,
            0x100
        );

        Timer.DelayCall(TimeSpan.FromSeconds(0.5), WebLands, target);
    }

    private static void WebLands(Mobile m)
    {
        if (m == null || m.Deleted || m.Map == null)
        {
            return;
        }

        Effects.SendLocationParticles(m, WebItemID, 1, 60, 0);
    }
}

/// <summary>
/// Anemia: the creature feeds on the target's blood and leaves it weakened. The disease itself has no
/// native stat hook, so the sixty second state is tracked here.
/// </summary>
public sealed class Anemia : SpecialAbility
{
    private const int MaxTicks = 31; // 60 seconds at two second intervals

    private sealed class Effect
    {
        public TimerExecutionToken Token;
        public int Ticks;
    }

    private static readonly Dictionary<Mobile, Effect> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger =>
        MonsterAbilityTrigger.GiveMeleeDamage | MonsterAbilityTrigger.TakeMeleeDamage;

    public override bool NaturalAbility => true;
    public override int ManaCost => 0;

    public Anemia()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted)
        {
            return;
        }

        if (_table.ContainsKey(defender))
        {
            // * The creature is repulsed by your diseased blood. *
            defender.PublicOverheadMessage(MessageType.Regular, 0x25, 1111668);
        }
        else
        {
            // *The creature drains some of your blood to replenish its health.*
            defender.PublicOverheadMessage(MessageType.Regular, 0x25, 1111698);

            creature.Hits = Math.Min(creature.HitsMax, creature.Hits + Utility.RandomMinMax(50, 70));
        }

        TryInfect(creature, defender);
    }

    /// <summary>
    /// ServUO also required <c>FountainOfFortune.UnderProtection</c>; that type does not exist in
    /// ModernUO, so the check is omitted.
    /// </summary>
    private static void TryInfect(BaseCreature creature, Mobile victim)
    {
        if (_table.ContainsKey(victim) || !creature.InRange(victim, 1) || Utility.RandomDouble() >= 0.25)
        {
            return;
        }

        victim.SendLocalizedMessage(1111669); // The creature's attack weakens you. You have become anemic.

        Effects.SendTargetEffect(victim, 0x375A, 9, 20);
        Effects.SendTargetParticles(victim, 0x373A, 1, 15, 0x26B9, EffectLayer.Head);
        Effects.SendLocationParticles(victim, 0x11A6, 9, 32, 0x253A);

        victim.PlaySound(0x1ED);

        var str = victim.RawStr / 3;
        var dex = victim.RawDex / 3;
        var intel = victim.RawInt / 3;

        victim.AddStatMod(new StatMod(StatType.Str, "BloodWorm_Str", -str, TimeSpan.FromSeconds(60)));
        victim.AddStatMod(new StatMod(StatType.Dex, "BloodWorm_Dex", -dex, TimeSpan.FromSeconds(60)));
        victim.AddStatMod(new StatMod(StatType.Int, "BloodWorm_Int", -intel, TimeSpan.FromSeconds(60)));

        // -~1_STR~ strength.<br>-~2_INT~ intelligence.<br>-~3_DEX~ dexterity.<br> Drains all stamina.
        (victim as PlayerMobile)?.AddBuff(
            new BuffInfo(BuffIcon.BloodwormAnemia, 1153797, 1153824, args: $"{str}\t{dex}\t{intel}")
        );

        var entry = new Effect();
        _table[victim] = entry;

        Timer.StartTimer(
            TimeSpan.FromSeconds(2.0),
            TimeSpan.FromSeconds(2.0),
            () => OnTick(victim, entry),
            out var token
        );

        entry.Token = token;
    }

    private static void OnTick(Mobile victim, Effect entry)
    {
        entry.Ticks++;

        if (entry.Ticks >= MaxTicks || victim.Deleted || !victim.Alive || victim.IsDeadBondedPet)
        {
            if (!victim.Deleted)
            {
                victim.SendLocalizedMessage(1111670); // You recover from your anemia.
            }

            Expire(victim, entry);
            return;
        }

        victim.Stam -= victim.Stam < 2 ? 0 : Utility.RandomMinMax(2, 5);
    }

    private static void Expire(Mobile m, Effect entry)
    {
        entry.Token.Cancel();
        _table.Remove(m);

        (m as PlayerMobile)?.RemoveBuff(BuffIcon.BloodwormAnemia);

        m.RemoveStatMod("BloodWorm_Str");
        m.RemoveStatMod("BloodWorm_Dex");
        m.RemoveStatMod("BloodWorm_Int");
    }
}

/// <summary>
/// Blood disease: the target is infected and bleeds poison damage every two seconds.
/// </summary>
public sealed class BloodDisease : SpecialAbility
{
    private const int MaxTicks = 8;

    private sealed class Effect
    {
        public TimerExecutionToken Token;
        public int Ticks;
    }

    private static readonly Dictionary<Mobile, Effect> _table = new();

    public override MonsterAbilityTrigger AbilityTrigger =>
        MonsterAbilityTrigger.GiveMeleeDamage | MonsterAbilityTrigger.TakeMeleeDamage;

    public override bool NaturalAbility => true;
    public override int ManaCost => 0;

    public BloodDisease()
    {
    }

    /// <summary>
    /// ServUO also required <c>FountainOfFortune.UnderProtection</c>; that type does not exist in
    /// ModernUO, so the check is omitted.
    /// </summary>
    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (defender == null || defender.Deleted || _table.ContainsKey(defender))
        {
            return;
        }

        if (!creature.InRange(defender, 1) || Utility.RandomDouble() >= 0.25)
        {
            return;
        }

        // The rotworm has infected you with blood disease!!
        defender.SendLocalizedMessage(1111672, "", 0x25);

        defender.PlaySound(0x213);
        Effects.SendTargetParticles(defender, 0x373A, 1, 15, 0x26B9, EffectLayer.Head);

        (defender as PlayerMobile)?.AddBuff(new BuffInfo(BuffIcon.RotwormBloodDisease, 1153798, 1153798));

        var entry = new Effect();
        _table[defender] = entry;

        Timer.StartTimer(
            TimeSpan.FromSeconds(2.0),
            TimeSpan.FromSeconds(2.0),
            () => OnTick(creature, defender, entry),
            out var token
        );

        entry.Token = token;
    }

    private static void OnTick(Mobile attacker, Mobile victim, Effect entry)
    {
        if (entry.Ticks == MaxTicks || victim.Deleted || !victim.Alive || victim.IsDeadBondedPet)
        {
            if (!victim.Deleted)
            {
                victim.SendLocalizedMessage(1111673); // You no longer feel sick.
            }

            Expire(victim, entry);
            return;
        }

        if (entry.Ticks > 0)
        {
            AOS.Damage(victim, attacker, Utility.RandomMinMax(10, 20), 0, 0, 0, 100, 0);
            victim.Combatant = null;
        }

        entry.Ticks++;
    }

    private static void Expire(Mobile m, Effect entry)
    {
        entry.Token.Cancel();
        _table.Remove(m);

        (m as PlayerMobile)?.RemoveBuff(BuffIcon.RotwormBloodDisease);
    }
}

/// <summary>
/// Spits a glob of poison at the attacker a moment after the trigger.
/// </summary>
public sealed class PoisonSpit : SpecialAbility
{
    public override MonsterAbilityTrigger AbilityTrigger =>
        MonsterAbilityTrigger.TakeMeleeDamage | MonsterAbilityTrigger.TakeSpellDamage;

    public override int MaxRange => 10;
    public override bool NaturalAbility => true;
    public override int ManaCost => 0;

    public PoisonSpit()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile target, MonsterAbilityTrigger trigger)
    {
        if (target == null || target.Deleted)
        {
            return;
        }

        Effects.SendMovingEffect(creature, target, 0x36D4, 5, 0, false, true, 63, 0);

        Timer.DelayCall(TimeSpan.FromSeconds(1.0), PoisonLands, creature, target);
    }

    private static void PoisonLands(BaseCreature creature, Mobile target)
    {
        if (creature.Deleted || target == null || target.Deleted || !target.Alive)
        {
            return;
        }

        creature.DoHarmful(target);

        // ServUO's GetHitPoison() is BaseCreature.HitPoison in ModernUO.
        target.ApplyPoison(creature, creature.HitPoison ?? Poison.Regular);

        target.SendLocalizedMessage(1070821, creature.Name); // ~1_CREATURE~ spits a poisonous substance at you!
    }
}

/// <summary>
/// Fear freezes the target for a duration that shrinks as its Magic Resist grows.
/// </summary>
public sealed class TrueFear : SpecialAbility
{
    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.Movement;
    public override int MaxRange => 8;
    public override bool NaturalAbility => true;
    public override int ManaCost => 0;

    // ServUO rolled a fresh 1-3 minute cooldown per trigger; the native pipeline expresses that as a
    // min/max range instead of a per-trigger value.
    public override TimeSpan MinTriggerCooldown => TimeSpan.FromMinutes(1.0);
    public override TimeSpan MaxTriggerCooldown => TimeSpan.FromMinutes(3.0);

    public TrueFear()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile target, MonsterAbilityTrigger trigger)
    {
        if (target == null || target.Deleted || !target.Alive)
        {
            return;
        }

        var seconds = (int)Math.Max(1, 13.0 - target.Skills[SkillName.MagicResist].Value / 10.0);

        int number;

        if (seconds <= 2)
        {
            number = 1080339; // A sense of discomfort passes through you, but it fades quickly
        }
        else if (seconds <= 4)
        {
            number = 1080340; // An unfamiliar fear washes over you, and for a moment you're unable to move
        }
        else if (seconds <= 7)
        {
            number = 1080341; // Panic grips you! You're unable to move, to think, to feel anything but fear!
        }
        else if (seconds <= 10)
        {
            // Terror slices into your very being, destroying any chance of resisting ~1_name~ you might have had
            number = 1080342;
        }
        else
        {
            // Everything around you dissolves into darkness as ~1_name~'s burning eyes fill your vision
            number = 1080343;
        }

        target.SendLocalizedMessage(number, creature.Name, 0x21);
        target.Frozen = true;

        (target as PlayerMobile)?.AddBuff(
            new BuffInfo(BuffIcon.TrueFear, 1153791, 1153827, TimeSpan.FromSeconds(seconds))
        );

        Timer.DelayCall(TimeSpan.FromSeconds(seconds), FearWearsOff, target);
    }

    private static void FearWearsOff(Mobile target)
    {
        if (target == null || target.Deleted)
        {
            return;
        }

        target.Frozen = false;
        target.SendLocalizedMessage(1005603); // You can move again!

        (target as PlayerMobile)?.RemoveBuff(BuffIcon.TrueFear);
    }
}

/// <summary>
/// A colossal blow stuns the target and lands a free weapon hit.
/// </summary>

/// <summary>
/// Drains life from everything within two tiles.
/// </summary>
public sealed class LifeDrain : SpecialAbility
{
    public override MonsterAbilityTrigger AbilityTrigger =>
        MonsterAbilityTrigger.GiveMeleeDamage | MonsterAbilityTrigger.TakeMeleeDamage;

    public override bool NaturalAbility => true;
    public override TimeSpan CooldownDuration => TimeSpan.FromSeconds(1.0);

    public LifeDrain()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        if (creature.Map == null || creature.Map == Map.Internal)
        {
            return;
        }

        using var targets = PooledRefQueue<Mobile>.Create();

        foreach (var m in creature.GetMobilesInRange(2))
        {
            if (AreaEffect.ValidTarget(creature, m))
            {
                targets.Enqueue(m);
            }
        }

        while (targets.Count > 0)
        {
            var m = targets.Dequeue();

            if (m.Deleted || !m.Alive)
            {
                continue;
            }

            // ServUO's Succubus/Semidar cases and OnDrainLife hook are shard-specific and are not
            // ported; every drained target yields the same 10-40.
            var toDrain = Utility.RandomMinMax(10, 40);

            creature.DoHarmful(m);

            m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
            m.PlaySound(0x231);

            m.SendMessage("You feel the life drain out of you!");

            creature.Hits = Math.Min(creature.HitsMax, creature.Hits + toDrain);

            AOS.Damage(m, creature, toDrain, false, 0, 0, 0, 0, 0, 0, 100);
        }
    }
}

/// <summary>
/// A frenzied rage that only healthy targets provoke: the creature's hue changes and it is restored
/// to full stamina for ten seconds.
/// </summary>
public sealed class ColossalRage : SpecialAbility
{
    private static readonly TimeSpan _duration = TimeSpan.FromSeconds(10.0);

    private static readonly List<BaseCreature> _inRage = new();

    public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.TakeMeleeDamage;
    public override bool NaturalAbility => true;
    public override int ManaCost => 0;
    public override double TriggerChance => 0.5;
    public override TimeSpan CooldownDuration => TimeSpan.FromSeconds(15.0);

    public ColossalRage()
    {
    }

    /// <summary>The creature only rages against targets above a third of their hit points.</summary>
    public override bool Validate(BaseCreature attacker, Mobile defender)
    {
        if (defender == null || defender.Hits < defender.HitsMax * 0.33)
        {
            return false;
        }

        return base.Validate(attacker, defender);
    }

    public override void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger)
    {
        AddRage(creature);

        Timer.DelayCall(TimeSpan.FromSeconds(0.25), AnnounceRage, creature);
        Timer.DelayCall(_duration, RemoveRage, creature);
    }

    private static void AnnounceRage(BaseCreature bc)
    {
        if (bc.Deleted)
        {
            return;
        }

        bc.PublicOverheadMessage(MessageType.Regular, 0x20, 1113587); // The creature goes into a frenzied rage!
    }

    public static List<BaseCreature> InRage => _inRage;

    public static void AddRage(BaseCreature bc)
    {
        if (!_inRage.Contains(bc))
        {
            _inRage.Add(bc);
        }

        bc.HueMod = 1157;
        bc.Stam = bc.StamMax;
    }

    public static void RemoveRage(BaseCreature bc)
    {
        if (bc == null || !_inRage.Remove(bc))
        {
            return;
        }

        if (!bc.Deleted)
        {
            bc.HueMod = -1;
        }
    }

    public static bool HasRage(BaseCreature bc) => _inRage.Contains(bc);
}
