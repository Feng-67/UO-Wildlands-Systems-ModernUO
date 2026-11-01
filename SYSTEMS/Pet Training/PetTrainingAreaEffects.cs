// UO Wildlands Custom Script — ModernUO
// ServUO AreaEffects.cs port: the seven concrete AreaEffect singletons, in persisted index order.
// Licensed under GPL-3.0.

using System;
using System.Collections.Generic;
using Server.Engines.BuffIcons;
using Server.Items;
using Server.Logging;

namespace Server.Mobiles;

/// <summary>
/// Registry for the area-effect abilities. The index into <see cref="All"/> is the persisted
/// identity written by AbilityProfile and PlanningProfile, so the order must never change.
/// </summary>
public static class PetTrainingAreaEffects
{
    public static readonly AreaEffect[] All =
    {
        new AuraOfEnergy(),     // 0
        new AuraOfNausea(),     // 1
        new EssenceOfDisease(), // 2
        new EssenceOfEarth(),   // 3
        new ExplosiveGoo(),     // 4
        new AuraDamage(),       // 5
        new PoisonBreath(),     // 6
    };

    private static readonly ILogger _logger = LogFactory.GetLogger(typeof(PetTrainingAreaEffects));

    private static readonly List<string> _missing = new();

    /// <summary>
    /// Creature types named by the ported ServUO tables that this core does not have. ServUO named
    /// them with <c>typeof</c>; they are resolved by name here so the file compiles on a stock core
    /// and a shard can add the creature later without touching this file.
    /// </summary>
    public static string[] MissingTypes => _missing.ToArray();

    internal static Type[] ResolveTypes(string[] names)
    {
        var resolved = new List<Type>(names.Length);

        for (var i = 0; i < names.Length; i++)
        {
            var type = AssemblyHandler.FindTypeByName(names[i], ignoreCase: true);

            if (type == null)
            {
                if (!_missing.Contains(names[i]))
                {
                    _missing.Add(names[i]);
                    _logger.Warning(
                        "Pet training area effect references creature type {CreatureType}, which this core does not have; skipping it",
                        names[i]
                    );
                }

                continue;
            }

            resolved.Add(type);
        }

        return resolved.ToArray();
    }
}

/// <summary>
/// The creature's aura of energy damages everything around it. ServUO carries no overrides beyond
/// the base contract, so neither does this.
/// </summary>
public sealed class AuraOfEnergy : AreaEffect
{
    public AuraOfEnergy()
    {
    }

    public override void DoEffect(BaseCreature creature, Mobile defender)
    {
        AOS.Damage(defender, creature, Utility.RandomMinMax(20, 30), 0, 0, 0, 0, 100);

        defender.SendLocalizedMessage(1072073, false, creature.Name); // The creature's aura of energy is damaging you!

        creature.DoHarmful(defender);
        defender.FixedParticles(0x374A, 10, 30, 5052, 1278, 0, EffectLayer.Waist);
        defender.PlaySound(0x51D);
    }
}

/// <summary>
/// The creature's putrid presence causes nausea. Nausea has no native stat hook in ModernUO, so the
/// 30 second state is tracked here and read back through <see cref="UnderNausea"/>.
/// </summary>
public sealed class AuraOfNausea : AreaEffect
{
    private static readonly TimeSpan _nauseaDuration = TimeSpan.FromSeconds(30);

    private static Dictionary<Mobile, TimerExecutionToken> _table;

    // ServUO rolls 40 + RandomDouble() * 30 seconds per trigger. The native ability expresses that
    // jitter as a min/max cooldown range rather than a per-trigger value.
    public override TimeSpan MinTriggerCooldown => TimeSpan.FromSeconds(40);
    public override TimeSpan MaxTriggerCooldown => TimeSpan.FromSeconds(70);

    public override int MaxRange => 4;
    public override int EffectRange => 4;
    public override int ManaCost => 100;

    public AuraOfNausea()
    {
    }

    public override void DoEffect(BaseCreature creature, Mobile defender)
    {
        _table ??= new Dictionary<Mobile, TimerExecutionToken>();

        // Re-applying nausea restarts the 30 second window rather than stacking.
        if (_table.Remove(defender, out var existing))
        {
            existing.Cancel();
        }

        Timer.StartTimer(_nauseaDuration, () => EndNausea(defender), out var token);
        _table[defender] = token;

        defender.Animate(32, 5, 1, true, false, 0); // bow animation
        defender.SendLocalizedMessage(1072068);     // Your enemy's putrid presence envelops you, overwhelming you with nausea.

        (defender as PlayerMobile)?.AddBuff(
            new BuffInfo(BuffIcon.AuraOfNausea, 1153792, 1153819, _nauseaDuration, "60\t60\t60\t5")
        );
    }

    public static void EndNausea(Mobile m)
    {
        if (_table?.Remove(m) != true)
        {
            return;
        }

        (m as PlayerMobile)?.RemoveBuff(BuffIcon.AuraOfNausea);
        m.Delta(MobileDelta.WeaponDamage);

        if (_table.Count == 0)
        {
            _table = null;
        }
    }

    public static bool UnderNausea(Mobile m) => _table?.ContainsKey(m) == true;
}

/// <summary>
/// The creature's aura of disease damages everything around it.
/// </summary>
public sealed class EssenceOfDisease : AreaEffect
{
    public EssenceOfDisease()
    {
    }

    public override void DoEffect(BaseCreature creature, Mobile defender)
    {
        AOS.Damage(defender, creature, Utility.RandomMinMax(20, 30), 0, 0, 0, 100, 0);

        defender.SendLocalizedMessage(1072074, false, creature.Name); // The creature's aura of disease is damaging you!

        creature.DoHarmful(defender);
        defender.FixedParticles(0x374A, 10, 30, 5052, 1272, 0, EffectLayer.Waist);
        defender.PlaySound(0x476);
    }
}

/// <summary>
/// The creature's earthen essence damages everything around it.
/// </summary>
public sealed class EssenceOfEarth : AreaEffect
{
    public EssenceOfEarth()
    {
    }

    public override void DoEffect(BaseCreature creature, Mobile defender)
    {
        AOS.Damage(defender, creature, Utility.RandomMinMax(20, 30), 100, 0, 0, 0, 0);

        defender.SendLocalizedMessage(1072075, false, creature.Name); // The creature's earthen essence is damaging you!

        creature.DoHarmful(defender);
        defender.FixedParticles(0x374A, 10, 30, 5052, 1836, 0, EffectLayer.Waist);
        defender.PlaySound(0x22C);
    }
}

/// <summary>
/// Flammable goo bursts into flame a moment after it lands. The burst is a separate, deferred pass
/// so the particle fan-out reads as a wind-up; <c>_doingEffect</c> keeps a second trigger out
/// during that one second window.
/// </summary>
public sealed class ExplosiveGoo : AreaEffect
{
    public override int ManaCost => 30;

    private bool _doingEffect;

    public ExplosiveGoo()
    {
    }

    public override void DoEffects(BaseCreature creature, Mobile combatant)
    {
        if (_doingEffect)
        {
            return;
        }

        Server.Effects.SendTargetParticles(creature, 0x3709, 10, 15, 2724, 0, 9907, EffectLayer.LeftFoot, 0);
        creature.PlaySound(0x348);

        _doingEffect = true;

        Timer.DelayCall(TimeSpan.FromSeconds(1.0), ApplyDelayedEffects, creature, combatant);
    }

    private void ApplyDelayedEffects(BaseCreature creature, Mobile combatant)
    {
        base.DoEffects(creature, combatant);
        _doingEffect = false;
    }

    public override void DoEffect(BaseCreature creature, Mobile defender)
    {
        // Staggered per target so the burst is not a single frame.
        Timer.DelayCall(
            TimeSpan.FromMilliseconds(Utility.RandomMinMax(10, 1000)), ApplyGoo, creature, defender
        );
    }

    private static void ApplyGoo(BaseCreature creature, Mobile defender)
    {
        if (creature.Deleted || defender.Deleted || !defender.Alive || defender.Map == null)
        {
            return;
        }

        var p = defender.Location;

        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                // ModernUO orders these (itemID, duration, speed, hue, renderMode); ServUO used
                // (itemID, speed, duration, hue, renderMode).
                Server.Effects.SendLocationEffect(new Point3D(p.X + x, p.Y + y, p.Z), defender.Map, 0x3728, 1921, 13, 3);
            }
        }

        creature.DoHarmful(defender);
        AOS.Damage(defender, creature, Utility.RandomMinMax(30, 40), 0, 100, 0, 0, 0);
        defender.SendLocalizedMessage(1112366); // The flammable goo covering you bursts into flame!
    }
}

/// <summary>
/// A permanent damaging aura whose strength and cooldown come from the creature's
/// <see cref="AuraDamage.AuraDefinition"/>.
/// </summary>
public sealed class AuraDamage : AreaEffect
{
    // The cooldown is a property of the creature's AuraDefinition, which a fixed
    // Min/MaxTriggerCooldown cannot express. The deadline is tracked per creature instead and the
    // native arming disabled so the two cannot disagree.
    public override TimeSpan MinTriggerCooldown => TimeSpan.Zero;
    public override TimeSpan MaxTriggerCooldown => TimeSpan.Zero;

    public override double TriggerChance => 0.4;
    public override int EffectRange => 10;
    public override int ManaCost => 0;
    public override bool RequiresCombatant => false;

    private static readonly Dictionary<BaseCreature, TimerExecutionToken> _cooldowns = new();

    public AuraDamage()
    {
    }

    /// <summary>ServUO's <c>GetCooldown</c>; the equivalent is enforced by <see cref="CanTrigger"/>.</summary>
    public TimeSpan GetCooldown(BaseCreature bc) => AuraDefinition.GetDefinition(bc).Cooldown;

    public override bool CanTrigger(BaseCreature source, MonsterAbilityTrigger trigger) =>
        !_cooldowns.ContainsKey(source) && base.CanTrigger(source, trigger);

    public override void Trigger(MonsterAbilityTrigger trigger, BaseCreature source, Mobile target)
    {
        // Mirror AreaEffect.Trigger's gate so a failed validation does not burn the cooldown.
        var effectWillRun = CheckMana(source) && Validate(source, target);

        base.Trigger(trigger, source, target);

        if (!effectWillRun || source.Deleted)
        {
            return;
        }

        if (_cooldowns.Remove(source, out var existing))
        {
            existing.Cancel();
        }

        Timer.StartTimer(GetCooldown(source), () => EndCooldown(source), out var token);
        _cooldowns[source] = token;
    }

    private static void EndCooldown(BaseCreature bc) => _cooldowns.Remove(bc);

    public override void DoEffect(BaseCreature creature, Mobile defender)
    {
        var def = AuraDefinition.GetDefinition(creature);

        if (def.Damage > 0)
        {
            // ModernUO has no DamageType argument; the master overload carries the chaos/direct
            // split instead. ServUO's DamageType.SpellAOE has no equivalent here.
            AOS.Damage(
                defender, creature, def.Damage, false, def.Physical, def.Fire, def.Cold, def.Poison,
                def.Energy, def.Chaos, def.Direct
            );

            defender.RevealingAction();
        }

        // ServUO also called IAuraCreature.AuraEffect here; that interface does not exist in ModernUO.
    }

    /// <summary>Per-creature-type aura profile, ported from ServUO.</summary>
    public class AuraDefinition
    {
        private static readonly List<AuraDefinition> _definitions = new();
        private static bool _initialized;

        public TimeSpan Cooldown { get; set; }
        public int Range { get; set; }

        public int Damage { get; set; }
        public int Physical { get; set; }
        public int Fire { get; set; }
        public int Cold { get; set; }
        public int Poison { get; set; }
        public int Energy { get; set; }
        public int Chaos { get; set; }
        public int Direct { get; set; }

        public Type[] Uses { get; private set; }

        public AuraDefinition()
            : this(TimeSpan.FromSeconds(5), 4, 5, 0, 0, 0, 0, 0, 0, 100, Array.Empty<Type>())
        {
        }

        public AuraDefinition(params Type[] uses)
            : this(TimeSpan.FromSeconds(5), 2, 5, 0, 100, 0, 0, 0, 0, 0, uses)
        {
        }

        public AuraDefinition(
            TimeSpan cooldown, int range, int baseDamage, int phys, int fire, int cold, int poison,
            int energy, int chaos, int direct, Type[] uses
        )
        {
            Cooldown = cooldown;
            Range = range;
            Damage = baseDamage;
            Physical = phys;
            Fire = fire;
            Cold = cold;
            Poison = poison;
            Energy = energy;
            Chaos = chaos;
            Direct = direct;

            Uses = uses ?? Array.Empty<Type>();
        }

        public static List<AuraDefinition> Definitions
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

            _definitions.Add(new AuraDefinition());

            //var cora = new AuraDefinition(PetTrainingAreaEffects.ResolveTypes(new[] { "CoraTheSorceress" }));
            //cora.Range = 3;
            //cora.Damage = 10;
            //cora.Fire = 0;
            //_definitions.Add(cora);

            var fireAura = new AuraDefinition(
                PetTrainingAreaEffects.ResolveTypes(new[] { "FireElemental" })
            );
            fireAura.Range = 5;
            fireAura.Damage = 7;
            _definitions.Add(fireAura);

            var coldAura = new AuraDefinition(
                PetTrainingAreaEffects.ResolveTypes(
                    new[]
                    {
                        "SnowElemental", "IceFiend",
                        "IceElemental", "CorporealBrume"
                    }
                )
            );
            coldAura.Damage = 15;
            coldAura.Fire = 0;
            coldAura.Cold = 100;
            _definitions.Add(coldAura);
        }

        public static AuraDefinition GetDefinition(BaseCreature bc)
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
/// Poison breath: poison from the creature's Poisoning skill, plus a flat burst against the few
/// creature types that are vulnerable to it.
/// </summary>
public sealed class PoisonBreath : AreaEffect
{
    public override double TriggerChance => 0.4;
    public override int EffectRange => 10;
    public override int ManaCost => 50;

    private static readonly string[] _damageCreatureNames =
    {
        "ValoriteElemental", "BronzeElemental", "ChiefParoxysmus"
    };

    private static Type[] _damageCreatures;
    private static bool _damageCreaturesResolved;

    public PoisonBreath()
    {
    }

    private static Type[] DamageCreatures
    {
        get
        {
            if (!_damageCreaturesResolved)
            {
                _damageCreaturesResolved = true;
                _damageCreatures = PetTrainingAreaEffects.ResolveTypes(_damageCreatureNames);
            }

            return _damageCreatures;
        }
    }

    public override void DoEffect(BaseCreature creature, Mobile defender)
    {
        defender.ApplyPoison(creature, GetPoison(creature));

        Server.Effects.SendLocationParticles(
            EffectItem.Create(defender.Location, defender.Map, EffectItem.DefaultDuration), 0x36B0, 1, 14, 63, 7, 9915, 0
        );

        Server.Effects.PlaySound(defender.Location, defender.Map, 0x229);

        var damage = GetDamage(creature);

        if (damage > 0)
        {
            creature.DoHarmful(defender);
            AOS.Damage(defender, creature, damage, 0, 0, 0, 100, 0);
        }
    }

    public override void OnAfterEffects(BaseCreature creature, Mobile defender)
    {
        if (!creature.Controlled)
        {
            return;
        }

        var profile = PetTrainingHelper.GetAbilityProfile(creature);

        if ((profile != null && profile.HasAbility(MagicalAbility.Poisoning)) || 0.2 > Utility.RandomDouble())
        {
            creature.CheckSkill(SkillName.Poisoning, 0, creature.Skills[SkillName.Poisoning].Cap);
        }
    }

    public Poison GetPoison(BaseCreature bc)
    {
        var level = 0;
        var total = bc.Skills[SkillName.Poisoning].Value;

        if (total >= 100)
        {
            level = 4;
        }
        else if (total > 60)
        {
            level = 3;
        }
        else if (total > 40)
        {
            level = 2;
        }
        else if (total > 20)
        {
            level = 1;
        }

        return Poison.GetPoison(level);
    }

    public int GetDamage(BaseCreature bc)
    {
        var types = DamageCreatures;
        var type = bc.GetType();

        for (var i = 0; i < types.Length; i++)
        {
            if (types[i] == type)
            {
                return 50;
            }
        }

        return 0;
    }
}
