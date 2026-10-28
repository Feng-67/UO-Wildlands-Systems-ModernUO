// UO Wildlands Custom Script — ModernUO
// Consolidated pet training: data, profiles, helper, ability bases, and gates.
// Licensed under GPL-3.0.

using ModernUO.Serialization;
using Server;
using Server.Commands;
using Server.Engines.BuffIcons;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.SkillHandlers;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Server.Mobiles
{
    // ==========================================================================================
    // ENUMS
    // ==========================================================================================

    [Flags]
    public enum PetStat
    {
        Str, Dex, Int, Hits, Stam, Mana, RegenHits, RegenStam, RegenMana, BaseDamage
    }

    [Flags]
    public enum Class
    {
        None = 0x00000000,
        Untrainable = 0x00000001,
        Magical = 0x00000002,
        Necromantic = 0x00000004,
        Tokuno = 0x00000008,
        StickySkin = 0x00000010,
        Clawed = 0x00000020,
        Tailed = 0x00000040,
        Insectoid = 0x00000080,
        Restricted = 0x00000100,
        MagicalAndNecromantic = Magical | Necromantic,
        MagicalAndClawed = Magical | Clawed,
        MagicalAndTailed = Magical | Tailed,
        ClawedAndTailed = Clawed | Tailed,
        MagicalAndInsectoid = Magical | Insectoid,
        StickySkinAndTailed = StickySkin | Tailed,
        TailedAndNecromantic = Tailed | Necromantic,
        ClawedNecromanticAndTokuno = Clawed | Necromantic | Tokuno,
        MagicalClawedAndTailed = MagicalAndClawed | Tailed,
        MagicalNecromanticAndTokuno = MagicalAndNecromantic | Tokuno,
        ClawedTailedAndTokuno = ClawedAndTailed | Tokuno,
        ClawedTailedAndNecromantic = ClawedAndTailed | Necromantic,
        MagicalClawedTailedAndNecromantic = MagicalClawedAndTailed | Necromantic,
        ClawedTailedNecromanticAndTokuno = ClawedNecromanticAndTokuno | Tailed,
        ClawedTailedMagicalAndTokuno = MagicalClawedAndTailed | Tokuno,
        MagicalClawedTailedNecromanticAndTokuno = MagicalClawedTailedAndNecromantic | Tokuno,
    }

    [Flags]
    public enum MagicalAbility
    {
        None = 0x00000000,

        // Special magical abilities (mutually exclusive schools of mastery)
        Piercing = 0x00000001,
        Bashing = 0x00000002,
        Slashing = 0x00000004,
        BattleDefense = 0x00000008,
        WrestlingMastery = 0x00000010,

        // Magical schools
        Poisoning = 0x00000020,
        Bushido = 0x00000040,
        Ninjitsu = 0x00000080,
        Discordance = 0x00000100,
        MageryMastery = 0x00000200,
        Mysticism = 0x00000400,
        Spellweaving = 0x00000800,
        Chivalry = 0x00001000,
        Necromage = 0x00002000,
        Necromancy = 0x00004000,
        Magery = 0x00008000,

        Tokuno = Bushido | Ninjitsu,
        SabreToothedTiger = Bashing | Piercing | Poisoning,

        Vartiety = Piercing | Slashing | WrestlingMastery,
        Variety1 = Bashing | Vartiety,
        Variety2 = Bashing | Chivalry | Discordance | MageryMastery | Mysticism | Necromage | Necromancy | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery,
        Variety3 = Bashing | Chivalry | Discordance | MageryMastery | Mysticism | Necromage | Necromancy | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery | BattleDefense,

        StandardClawedOrTailed = SabreToothedTiger | Slashing | WrestlingMastery,
        Tokuno1 = Tokuno | Chivalry | Discordance | MageryMastery | Mysticism | Poisoning | Spellweaving,
        Dragon1 = Bashing | BattleDefense | Chivalry | Discordance | MageryMastery | Mysticism | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery,
        Dragon2 = Bashing | Chivalry | Discordance | MageryMastery | Mysticism | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery,
        ColdDrake = Chivalry | Discordance | Poisoning | Mysticism | Spellweaving,
        Cusidhe = ColdDrake | WrestlingMastery,
        Wolf = Bashing | Tokuno | Necromage | Necromancy | Piercing | Poisoning | Slashing | WrestlingMastery,
        DragonWolf = Bashing | BattleDefense | Piercing | Poisoning | Slashing | WrestlingMastery,
        DreadSpider = Bashing | Tokuno | Chivalry | Discordance | MageryMastery | Mysticism | Necromage | Necromancy | Piercing | Slashing | Spellweaving | WrestlingMastery,
        DreadWarhorse = Bashing | BattleDefense | Chivalry | Discordance | MageryMastery | Mysticism | Necromage | Necromancy | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery,
        GreaterDragon = Chivalry | Discordance | MageryMastery | Mysticism | Poisoning | Spellweaving,
        Hellcat = Bashing | Necromage | Necromancy | Piercing | Poisoning | Slashing | WrestlingMastery,
        GrizzledMare = BattleDefense | Bashing | Necromage | Necromancy | Piercing | Poisoning | Slashing | WrestlingMastery,
        Hiryu = Tokuno | Chivalry | Discordance | Poisoning | Spellweaving | WrestlingMastery,
        LavaLizard = Tokuno | Chivalry | Bashing,
        RuneBeetle = Chivalry | Discordance | MageryMastery | Mysticism | Spellweaving,
        StygianDrake = Bashing | Chivalry | Discordance | Mysticism | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery,
        Triceratops = Bashing | Poisoning | Slashing | WrestlingMastery,
        TsukiWolf = Tokuno | Chivalry | Discordance | Mysticism | Necromage | Necromancy | Poisoning | Spellweaving | WrestlingMastery,
        Triton = Chivalry | Discordance | MageryMastery | Mysticism | Poisoning | Spellweaving | Bushido | Ninjitsu | BattleDefense | Bashing | Piercing | Slashing | WrestlingMastery,
        CoconutCrab = GreaterDragon | BattleDefense | Bashing | Piercing | Slashing | WrestlingMastery,
    }

    public enum TrainingMode { Regular, Planning }

    // ==========================================================================================
    // ABILITY BASE CLASSES
    //
    // SpecialAbility and AreaEffect are ModernUO MonsterAbility singletons. Behaviour lives in
    // PetTrainingAbilities.cs / PetTrainingAreaEffects.cs; these bases provide the ServUO data
    // contract (mana cost, range, trigger chance) on top of the native trigger pipeline.
    // ==========================================================================================

    /// <summary>
    /// ServUO SpecialAbility port. Instances are stateless singletons registered in
    /// <see cref="SpecialAbility.Abilities"/>; all per-creature state lives in static tables or in
    /// the base MonsterAbility cooldown map.
    /// </summary>
    public abstract class SpecialAbility : MonsterAbility
    {
        public virtual int ManaCost => 10;
        public virtual int MaxRange => 1;
        public virtual double TriggerChance => 0.1;
        public virtual bool RequiresCombatant => true;
        public virtual TimeSpan CooldownDuration => TimeSpan.FromSeconds(30);

        public virtual MagicalAbility RequiredSchool => MagicalAbility.None;
        public virtual bool NaturalAbility => false;

        // ServUO flags a school of mastery as "special"; only one may be known at a time.
        public static readonly MagicalAbility SpecialMagicalMask =
            MagicalAbility.Piercing | MagicalAbility.Bashing | MagicalAbility.Slashing |
            MagicalAbility.BattleDefense | MagicalAbility.WrestlingMastery;

        // The single trigger flag this ability listens on. Maps ServUO's TriggerOn* booleans onto
        // the native MonsterAbilityTrigger taxonomy (there is no DamageType enum in ModernUO).
        public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.None;

        public override TimeSpan MinTriggerCooldown => CooldownDuration;
        public override TimeSpan MaxTriggerCooldown => CooldownDuration;
        public override double ChanceToTrigger => TriggerChance;
        public override MonsterAbilityType AbilityType => MonsterAbilityType.Generic;

        public bool CheckMana(BaseCreature creature) => creature.HasManaOverride || creature.Mana >= ManaCost;

        public virtual bool Validate(BaseCreature attacker, Mobile defender)
        {
            if (RequiredSchool != MagicalAbility.None)
            {
                var profile = PetTrainingHelper.GetAbilityProfile(attacker);

                if (profile == null || !profile.HasAbility(RequiredSchool))
                {
                    return false;
                }
            }

            if (defender == null || !defender.Alive || defender.Deleted || defender.IsDeadBondedPet)
            {
                return false;
            }

            return attacker.Alive && !attacker.IsDeadBondedPet && !attacker.BardPacified &&
                   defender.Map == attacker.Map && attacker.InLOS(defender) &&
                   defender.InRange(attacker.Location, MaxRange) && attacker.CanBeHarmful(defender);
        }

        public override void Trigger(MonsterAbilityTrigger trigger, BaseCreature source, Mobile target)
        {
            if (!CheckMana(source) || !Validate(source, target))
            {
                return;
            }

            // Spent up front so a failed DoEffects cannot be retried for free.
            if (!source.HasManaOverride)
            {
                source.Mana -= ManaCost;
            }

            DoEffects(source, target, trigger);

            base.Trigger(trigger, source, target);
        }

        public abstract void DoEffects(BaseCreature creature, Mobile defender, MonsterAbilityTrigger trigger);

        // ---- static triggers (mirrors the three ServUO entry points) ----

        // Owned by PetTrainingAbilities.cs so the 29 concrete classes live in one file.
        public static SpecialAbility[] Abilities => PetTrainingAbilities.All;

        private static SpecialAbility _venomousBite;
        private static SpecialAbility _viciousBite;

        public static SpecialAbility AngryFire => Abilities[0];
        public static SpecialAbility ConductiveBlast => Abilities[1];
        public static SpecialAbility DragonBreath => Abilities[2];
        public static SpecialAbility GraspingClaw => Abilities[3];
        public static SpecialAbility Inferno => Abilities[4];
        public static SpecialAbility LightningForce => Abilities[5];
        public static SpecialAbility ManaDrain => Abilities[6];
        public static SpecialAbility RagingBreath => Abilities[7];
        public static SpecialAbility Repel => Abilities[8];
        public static SpecialAbility SearingWounds => Abilities[9];
        public static SpecialAbility StealLife => Abilities[10];
        public static SpecialAbility VenomousBite => _venomousBite ??= Abilities[11];
        public static SpecialAbility ViciousBite => _viciousBite ??= Abilities[12];
        public static SpecialAbility RuneCorruption => Abilities[13];
        public static SpecialAbility LifeLeech => Abilities[14];
        public static SpecialAbility StickySkin => Abilities[15];
        public static SpecialAbility TailSwipe => Abilities[16];
        public static SpecialAbility FlurryForce => Abilities[17];
        public static SpecialAbility Rage => Abilities[18];

        // Indices 19..28 are non-trainable (read-only) abilities. They must stay in the array
        // because the index is the persisted identity, but they are never offered in the gump.
        public static SpecialAbility Heal => Abilities[19];
        public static SpecialAbility HowlOfCacophony => Abilities[20];
        public static SpecialAbility Webbing => Abilities[21];
        public static SpecialAbility Anemia => Abilities[22];
        public static SpecialAbility BloodDisease => Abilities[23];
        public static SpecialAbility PoisonSpit => Abilities[24];
        public static SpecialAbility TrueFear => Abilities[25];
        public static SpecialAbility ColossalBlow => Abilities[26];
        public static SpecialAbility LifeDrain => Abilities[27];
        public static SpecialAbility ColossalRage => Abilities[28];

        public static bool IsNonTrainable(SpecialAbility ability)
        {
            var index = Array.IndexOf(Abilities, ability);
            return index is >= 19 and <= 28;
        }

        /// <summary>
        /// Approach-triggered abilities cannot use the native trigger pipeline: the Movement
        /// trigger carries a Direction, not the Mobile that moved. Called from
        /// BaseCreature.OnMovement instead.
        /// </summary>
        public static void CheckApproachTrigger(BaseCreature bc, Mobile mobile, Point3D oldLocation)
        {
            var abilities = bc.GetAllMonsterAbilities();

            if (abilities == null || abilities.Length == 0)
            {
                return;
            }

            for (var i = 0; i < abilities.Length; i++)
            {
                if (abilities[i] is SpecialAbility ability && ability.AbilityTrigger == MonsterAbilityTrigger.Movement)
                {
                    if (!bc.InRange(mobile.Location, ability.MaxRange) ||
                        bc.InRange(oldLocation, ability.MaxRange))
                    {
                        continue;
                    }

                    ability.Trigger(MonsterAbilityTrigger.Movement, bc, mobile);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// ServUO AreaEffect port. Fans effects out to every valid target in range on a Think trigger.
    /// </summary>
    public abstract class AreaEffect : MonsterAbility
    {
        public virtual int ManaCost => 20;
        public virtual int MaxRange => 3;
        public virtual double TriggerChance => 1.0;
        public virtual TimeSpan CooldownDuration => TimeSpan.FromSeconds(30);
        public virtual bool RequiresCombatant => true;
        public virtual int EffectRange => 5;

        public override MonsterAbilityTrigger AbilityTrigger => MonsterAbilityTrigger.Think;
        public override TimeSpan MinTriggerCooldown => CooldownDuration;
        public override TimeSpan MaxTriggerCooldown => CooldownDuration;
        public override double ChanceToTrigger => TriggerChance;

        /// <summary>Area effects ignore mana when uncontrolled, matching ServUO.</summary>
        public bool CheckMana(BaseCreature bc) => !bc.Controlled || bc.HasManaOverride || bc.Mana >= ManaCost;

        protected virtual bool Validate(BaseCreature attacker, Mobile defender)
        {
            if (!attacker.Alive || attacker.Deleted || attacker.IsDeadBondedPet || attacker.BardPacified)
            {
                return false;
            }

            return !RequiresCombatant || defender != null && defender.Alive && !defender.Deleted &&
                   !defender.IsDeadBondedPet && defender.InRange(attacker.Location, MaxRange) &&
                   defender.Map == attacker.Map && attacker.InLOS(defender);
        }

        public override void Trigger(MonsterAbilityTrigger trigger, BaseCreature source, Mobile target)
        {
            if (!CheckMana(source) || !Validate(source, target))
            {
                return;
            }

            if (source.Controlled && !source.HasManaOverride)
            {
                source.Mana -= ManaCost;
            }

            DoEffects(source, target);

            base.Trigger(trigger, source, target);
        }

        public virtual void DoEffects(BaseCreature creature, Mobile combatant)
        {
            if (creature.Map == null || creature.Map == Map.Internal)
            {
                return;
            }

            var count = 0;

            foreach (var m in creature.GetMobilesInRange(EffectRange))
            {
                if (ValidTarget(creature, m))
                {
                    count++;
                    DoEffect(creature, m);
                }
            }

            if (count > 0)
            {
                OnAfterEffects(creature, combatant);
            }
        }

        public virtual void DoEffect(BaseCreature creature, Mobile defender)
        {
        }

        public virtual void OnAfterEffects(BaseCreature creature, Mobile defender)
        {
        }

        public static bool ValidTarget(Mobile from, Mobile to) =>
            to != from && to.Alive && !to.IsDeadBondedPet && from.CanBeHarmful(to, false) &&
            SpellHelper.ValidIndirectTarget(from, to) && from.InLOS(to);

        // Owned by PetTrainingAreaEffects.cs so the 7 concrete classes live in one file.
        public static AreaEffect[] Effects => PetTrainingAreaEffects.All;

        public static AreaEffect AuraOfEnergy => Effects[0];
        public static AreaEffect AuraOfNausea => Effects[1];
        public static AreaEffect EssenceOfDisease => Effects[2];
        public static AreaEffect EssenceOfEarth => Effects[3];
        public static AreaEffect ExplosiveGoo => Effects[4];
        public static AreaEffect AuraDamage => Effects[5];
        public static AreaEffect PoisonBreath => Effects[6];
    }

    // ==========================================================================================
    // BASECREATURE PARTIAL
    // ==========================================================================================

    public abstract partial class BaseCreature
    {
        public const double MaxTameRequirement = 120.0;

        private static readonly ConditionalWeakTable<BaseCreature, MonsterAbility[]> _learnedAbilities = new();

        private MonsterAbility[] _mergedInnate, _mergedTrained, _merged;

        [CommandProperty(AccessLevel.GameMaster)] public int ControlSlotsMin { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public int ControlSlotsMax { get; set; } = 5;
        [CommandProperty(AccessLevel.GameMaster)] public double CurrentTameSkill { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public SkillName Mastery { get; set; }

        public double BardingDifficulty => BaseInstrument.GetBaseDifficulty(this);

        public TrainingProfile TrainingProfile { get; set; }
        public AbilityProfile AbilityProfile { get; set; }
        public TrainingDefinition TrainingDefinition { get; set; }

        public virtual void OnAfterTame(Mobile tamer)
        {
            CurrentTameSkill = MinTameSkill;
        }

        /// <summary>
        /// The trained ability set for this creature, cached against the creature and invalidated
        /// by <see cref="InvalidateLearnedAbilities"/>. The instances are the shared singletons in
        /// <see cref="SpecialAbility.Abilities"/> / <see cref="AreaEffect.Effects"/>, so their
        /// MonsterAbility cooldown maps survive a cache rebuild.
        /// </summary>
        public MonsterAbility[] GetTrainedAbilities()
        {
            if (_learnedAbilities.TryGetValue(this, out var cached))
            {
                return cached;
            }

            var profile = AbilityProfile;
            MonsterAbility[] learned = null;

            if (profile != null)
            {
                var specials = profile.SpecialAbilities;
                var areas = profile.AreaEffects;
                var count = 0;
                var trainableSpecials = 0;

                if (specials != null)
                {
                    for (var i = 0; i < specials.Length; i++)
                    {
                        // Read-only abilities (Heal, Webbing, ...) are creature-innate, not trained.
                        if (specials[i] != null && !SpecialAbility.IsNonTrainable(specials[i]))
                        {
                            trainableSpecials++;
                        }
                    }
                }

                count = trainableSpecials + (areas?.Length ?? 0);

                if (count > 0)
                {
                    learned = new MonsterAbility[count];
                    var index = 0;

                    if (specials != null)
                    {
                        for (var i = 0; i < specials.Length; i++)
                        {
                            if (specials[i] != null && !SpecialAbility.IsNonTrainable(specials[i]))
                            {
                                learned[index++] = specials[i];
                            }
                        }
                    }

                    if (areas != null)
                    {
                        for (var i = 0; i < areas.Length; i++)
                        {
                            if (areas[i] != null)
                            {
                                learned[index++] = areas[i];
                            }
                        }
                    }
                }
            }

            learned ??= MonsterAbilities.Empty;
            _learnedAbilities.AddOrUpdate(this, learned);
            return learned;
        }

        /// <summary>Innate abilities (from subclass overrides) plus trained ones.</summary>
        public MonsterAbility[] GetAllMonsterAbilities()
        {
            var innate = GetMonsterAbilities();
            var trained = GetTrainedAbilities();

            if (trained == null || trained.Length == 0) return innate;
            if (innate == null || innate.Length == 0) return trained;

            if (ReferenceEquals(innate, _mergedInnate) && ReferenceEquals(trained, _mergedTrained))
                return _merged;

            var result = new MonsterAbility[innate.Length + trained.Length];
            innate.CopyTo(result, 0);
            trained.CopyTo(result, innate.Length);

            _mergedInnate = innate;
            _mergedTrained = trained;
            return _merged = result;
        }

        public void InvalidateLearnedAbilities() => _learnedAbilities.Remove(this);

        /// <summary>Special move chosen by training; consumed by BaseWeapon.OnSwing.</summary>
        public WeaponAbility GetTrainedWeaponAbility()
        {
            var abilities = AbilityProfile?.WeaponAbilities;

            if (abilities == null || abilities.Length == 0)
            {
                return null;
            }

            return abilities.Length == 1 ? abilities[0] : abilities[Utility.Random(abilities.Length)];
        }
    }

    // ==========================================================================================
    // TRAINING POINT / DEFINITION
    // ==========================================================================================

    public class TrainingPoint
    {
        public object TrainPoint { get; set; }
        public double Weight { get; set; }
        public int Start { get; set; }
        public int Max { get; set; }
        public TextDefinition Name { get; set; }
        public TextDefinition Description { get; set; }
        public TrainingPointRequirement[] Requirements { get; set; }

        public TrainingPoint(object trainpoint, double weight, int start, int max,
            TextDefinition name, TextDefinition description, params TrainingPointRequirement[] requirements)
        {
            TrainPoint = trainpoint;
            Weight = weight;
            Start = start;
            Max = max;
            Name = name;
            Description = description;
            Requirements = requirements;
        }

        public int GetMax(BaseCreature bc) =>
            TrainPoint is PetStat s && s == PetStat.BaseDamage
                ? PetTrainingHelper.GetMaxDamagePerSecond(bc)
                : Max;
    }

    public class TrainingPointRequirement
    {
        public object Requirement { get; set; }
        public int Cost { get; set; }
        public TextDefinition Name { get; set; }

        public TrainingPointRequirement(object requirement, int cost, TextDefinition name)
        {
            Requirement = requirement;
            Cost = cost;
            Name = name;
        }
    }

    public class TrainingDefinition
    {
        public Type CreatureType { get; private set; }
        public Class Class { get; private set; }
        public int ControlSlotsMin { get; private set; }
        public int ControlSlotsMax { get; private set; }

        public MagicalAbility MagicalAbilities { get; private set; }
        public SpecialAbility[] SpecialAbilities { get; private set; } = Array.Empty<SpecialAbility>();
        public WeaponAbility[] WeaponAbilities { get; private set; } = Array.Empty<WeaponAbility>();
        public AreaEffect[] AreaEffects { get; private set; } = Array.Empty<AreaEffect>();

        public TrainingDefinition(Type type, Class classification, int controlmin, int controlmax)
        {
            CreatureType = type;
            Class = classification;
            ControlSlotsMin = controlmin;
            ControlSlotsMax = controlmax;
        }

        public TrainingDefinition(
            Type type,
            Class classification,
            MagicalAbility magicalAbility,
            SpecialAbility[] specialAbility,
            WeaponAbility[] weaponAbility,
            AreaEffect[] areaEffect,
            int controlmin,
            int controlmax)
            : this(type, classification, controlmin, controlmax)
        {
            MagicalAbilities = magicalAbility;
            SpecialAbilities = specialAbility ?? Array.Empty<SpecialAbility>();
            WeaponAbilities = weaponAbility ?? Array.Empty<WeaponAbility>();
            AreaEffects = areaEffect ?? Array.Empty<AreaEffect>();
        }

        public bool HasSpecialAbility(SpecialAbility ability) =>
            SpecialAbilities != null && Array.IndexOf(SpecialAbilities, ability) >= 0;

        public bool HasAreaEffect(AreaEffect ability) =>
            AreaEffects != null && Array.IndexOf(AreaEffects, ability) >= 0;
    }

    // ==========================================================================================
    // PROFILES
    // ==========================================================================================

    public class TrainingProfile
    {
        [CommandProperty(AccessLevel.GameMaster)] public TrainingMode TrainingMode { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public bool HasBegunTraining { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public bool HasIncreasedControlSlot { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public int TrainedThisLevel { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public bool HasRecievedControlSlotWarning { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public double TrainingProgress { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public double TrainingProgressMax { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public BaseCreature Creature { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double TrainingProgressPercentile => TrainingProgressMax <= 0 ? 0 : TrainingProgress / TrainingProgressMax;

        [CommandProperty(AccessLevel.GameMaster)] public int ControlSlots => Creature.ControlSlots;
        [CommandProperty(AccessLevel.GameMaster)] public int ControlSlotsMin => Creature.ControlSlotsMin;
        [CommandProperty(AccessLevel.GameMaster)] public int ControlSlotsMax => Creature.ControlSlotsMax;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanApplyOptions => HasBegunTraining && TrainingProgressPercentile >= 1.0;

        private int _trainingPoints;
        private int _startingTrainingPoints;
        private PlanningProfile _plan;

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrainingPoints
        {
            get => _trainingPoints;
            set { if (value <= 0) EndTraining(); else _trainingPoints = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StartingTrainingPoints
        {
            get => _startingTrainingPoints;
            set => _startingTrainingPoints = value;
        }

        public PlanningProfile PlanningProfile => _plan ??= new PlanningProfile(Creature);

        public static TimeSpan PowerHourDuration = TimeSpan.FromHours(1);
        public static TimeSpan PowerHourDelay = TimeSpan.FromHours(24);

        [CommandProperty(AccessLevel.GameMaster)] public DateTime PowerHourBegin { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public bool InPowerHour { get; set; }
        public int PowerHourMultiplier => InPowerHour ? 2 : 1;

        private const int MaxTrainingProgress = 100;
        private readonly Dictionary<BaseCreature, int> _progressTable = new();

        public TrainingProfile(BaseCreature bc) => Creature = bc;

        private int AssignStartingTrainingPoints()
        {
            if (ControlSlots != ControlSlotsMin) return 1501;
            if (ControlSlotsMin == 1 && ControlSlotsMax == 2) return 2556;
            if (ControlSlotsMin == 1 && ControlSlotsMax == 3) return 2381;
            return 1501;
        }

        public void BeginTraining()
        {
            if (ControlSlots >= ControlSlotsMax) return;
            TrainingPoints = AssignStartingTrainingPoints();
            HasBegunTraining = true;
            TrainingProgress = 0;
            TrainingProgressMax = MaxTrainingProgress;
        }

        public void OnTrain(PlayerMobile pm, int points)
        {
            int reqInc;
            if (!HasIncreasedControlSlot)
            {
                reqInc = GetRequirementIncrease(true);
                Creature.RemoveFollowers();
                Creature.ControlSlots++;
                Creature.AddFollowers();
                TrainedThisLevel = 0;
                pm.SendLocalizedMessage(1157537);
                HasIncreasedControlSlot = true;
            }
            else
            {
                TrainedThisLevel++;
                reqInc = GetRequirementIncrease(false);
            }
            Creature.CurrentTameSkill = Math.Min(BaseCreature.MaxTameRequirement, Creature.CurrentTameSkill + reqInc);
            TrainingPoints -= points;
        }

        public int GetRequirementIncrease(bool slotIncreased)
        {
            if (slotIncreased) return ControlSlotsMin + 1 == ControlSlots ? 21 : 22;
            return TrainedThisLevel == 1 ? 3 : 2;
        }

        public void EndTraining()
        {
            HasBegunTraining = false;
            _trainingPoints = 0;
            HasRecievedControlSlotWarning = false;
            HasIncreasedControlSlot = false;
        }

        private bool CheckCanProgress(BaseCreature bc, double toGain)
        {
            if (_progressTable.TryGetValue(bc, out var val))
            {
                var gains = GetGainsPerCreature(toGain);
                if (val >= gains) return false;
                _progressTable[bc] = val + 1;
                return true;
            }
            _progressTable[bc] = 1;
            return true;
        }

        private int GetGainsPerCreature(double toGain)
        {
            int gains = ControlSlots switch
            {
                1 or 2 => int.MaxValue,
                3 => (int)(MaxTrainingProgress / toGain / 2.0),
                _ => (int)(MaxTrainingProgress / toGain / 4.0),
            };
            if (gains < int.MaxValue) gains *= PowerHourMultiplier;
            return gains;
        }

        public void CheckProgress(BaseCreature bc)
        {
            if (ControlSlots >= ControlSlotsMax || !HasBegunTraining ||
                TrainingProgress >= TrainingProgressMax || Creature.ControlMaster == null) return;

            var ourDif = Creature.BardingDifficulty;
            var theirDif = bc.BardingDifficulty;
            var master = Creature.ControlMaster;

            if (Utility.Random(100) < 8 - (1 + (ControlSlots - ControlSlotsMin)))
            {
                var toGain = GetAdvance(theirDif);
                if (ourDif - theirDif <= 50 && CheckCanProgress(bc, toGain))
                {
                    if (PowerHourBegin + PowerHourDelay < Core.Now)
                    {
                        _progressTable.Clear();
                        PowerHourBegin = Core.Now;
                        InPowerHour = true;
                        master.SendLocalizedMessage(1157569);
                    }
                    else if (InPowerHour && PowerHourBegin + PowerHourDuration < Core.Now)
                    {
                        InPowerHour = false;
                        master.SendLocalizedMessage(1157570);
                    }

                    TrainingProgress = Math.Min(TrainingProgressMax, TrainingProgress + toGain);

                    if (!bc.Controlled && !bc.Summoned && master is PlayerMobile)
                    {
                        var cliloc = 1157574;
                        if (toGain < 1.3) cliloc = 1157565;
                        else if (toGain < 2.5) cliloc = 1157573;

                        if (TrainingProgress >= TrainingProgressMax)
                            Creature.PrivateOverheadMessage(MessageType.Regular, 0x59, 1157543, master.NetState);

                        Creature.PrivateOverheadMessage(MessageType.Regular, 0x59, cliloc, master.NetState);
                    }
                    else
                    {
                        Creature.PrivateOverheadMessage(MessageType.Regular, 0x21, 1157564, master.NetState);
                    }
                }
                else
                {
                    Creature.PrivateOverheadMessage(MessageType.Regular, 0x21, 1157564, master.NetState);
                }
            }
        }

        private static double GetAdvance(double difficulty)
        {
            var advance = difficulty / 64;
            return advance >= 2.5 ? 2.5 : advance;
        }

        public void ResendProgressGump(Mobile m) { }
        public override string ToString() => "...";

        public TrainingProfile(BaseCreature bc, IGenericReader reader)
        {
            var version = reader.ReadInt();
            switch (version)
            {
                case 2:
                    TrainedThisLevel = reader.ReadInt();
                    goto case 1;
                case 1:
                    PowerHourBegin = reader.ReadDateTime();
                    InPowerHour = reader.ReadBool();
                    break;
            }

            Creature = bc;
            if (reader.ReadInt() == 1) _plan = new PlanningProfile(bc, reader);

            TrainingMode = (TrainingMode)reader.ReadInt();
            HasBegunTraining = reader.ReadBool();
            HasIncreasedControlSlot = reader.ReadBool();
            HasRecievedControlSlotWarning = reader.ReadBool();
            TrainingProgress = reader.ReadDouble();
            TrainingProgressMax = reader.ReadDouble();
            _startingTrainingPoints = reader.ReadInt();
            _trainingPoints = reader.ReadInt();
        }

        public void Serialize(IGenericWriter writer)
        {
            writer.Write(2);
            writer.Write(TrainedThisLevel);
            writer.Write(PowerHourBegin);
            writer.Write(InPowerHour);
            if (_plan != null) { writer.Write(1); _plan.Serialize(writer); }
            else writer.Write(0);
            writer.Write((int)TrainingMode);
            writer.Write(HasBegunTraining);
            writer.Write(HasIncreasedControlSlot);
            writer.Write(RecievedControlSlotWarningSerialized);
            writer.Write(TrainingProgress);
            writer.Write(TrainingProgressMax);
            writer.Write(_startingTrainingPoints);
            writer.Write(_trainingPoints);
        }

        private bool RecievedControlSlotWarningSerialized => HasRecievedControlSlotWarning;
    }

    public class PlanningProfile
    {
        public BaseCreature Creature { get; private set; }
        public List<PlanningEntry> Entries { get; private set; }

        public PlanningProfile(BaseCreature bc) { Creature = bc; Entries = new List<PlanningEntry>(); }
        public void Clear() => Entries.Clear();

        public void AddToPlan(object tp, int value, int cost)
        {
            var entry = Entries.FirstOrDefault(e => e.TrainPoint == tp);
            if (entry != null) Entries.Remove(entry);
            Entries.Add(new PlanningEntry(tp, value, cost));
        }

        public class PlanningEntry
        {
            public object TrainPoint { get; private set; }
            public int Value { get; set; }
            public int Cost { get; set; }
            public PlanningEntry(object tp, int value, int cost)
            {
                TrainPoint = tp; Value = value; Cost = cost;
            }
        }

        public PlanningProfile(BaseCreature bc, IGenericReader reader)
        {
            reader.ReadInt();
            Entries = new List<PlanningEntry>();
            var count = reader.ReadInt();
            for (var i = 0; i < count; i++)
            {
                switch (reader.ReadInt())
                {
                    case 0: break;
                    case 1: Entries.Add(new PlanningEntry((MagicalAbility)reader.ReadInt(), reader.ReadInt(), reader.ReadInt())); break;
                    case 2: Entries.Add(new PlanningEntry(ReadSpecialAbility(reader), reader.ReadInt(), reader.ReadInt())); break;
                    case 3: Entries.Add(new PlanningEntry(ReadAreaEffect(reader), reader.ReadInt(), reader.ReadInt())); break;
                    case 4: Entries.Add(new PlanningEntry(ReadWeaponAbility(reader), reader.ReadInt(), reader.ReadInt())); break;
                    case 5: Entries.Add(new PlanningEntry((PetStat)reader.ReadInt(), reader.ReadInt(), reader.ReadInt())); break;
                    case 6: Entries.Add(new PlanningEntry((ResistanceType)reader.ReadInt(), reader.ReadInt(), reader.ReadInt())); break;
                    case 7: Entries.Add(new PlanningEntry((SkillName)reader.ReadInt(), reader.ReadInt(), reader.ReadInt())); break;
                }
            }
        }

        private static SpecialAbility ReadSpecialAbility(IGenericReader reader)
        {
            var index = reader.ReadInt();
            var abilities = SpecialAbility.Abilities;
            return index >= 0 && index < abilities.Length ? abilities[index] : null;
        }

        private static AreaEffect ReadAreaEffect(IGenericReader reader)
        {
            var index = reader.ReadInt();
            var effects = AreaEffect.Effects;
            return index >= 0 && index < effects.Length ? effects[index] : null;
        }

        private static WeaponAbility ReadWeaponAbility(IGenericReader reader)
        {
            var index = reader.ReadInt();
            var abilities = PetTrainingHelper.WeaponAbilities;
            return index >= 0 && index < abilities.Length ? abilities[index] : null;
        }

        public void Serialize(IGenericWriter writer)
        {
            writer.Write(0);
            writer.Write(Entries.Count);
            for (var i = 0; i < Entries.Count; i++)
            {
                var entry = Entries[i];
                var o = entry.TrainPoint;
                if (o is MagicalAbility ma) { writer.Write(1); writer.Write((int)ma); }
                else if (o is SpecialAbility sa) { writer.Write(2); writer.Write(Array.IndexOf(SpecialAbility.Abilities, sa)); }
                else if (o is AreaEffect ae) { writer.Write(3); writer.Write(Array.IndexOf(AreaEffect.Effects, ae)); }
                else if (o is WeaponAbility wa) { writer.Write(4); writer.Write(Array.IndexOf(PetTrainingHelper.WeaponAbilities, wa)); }
                else if (o is PetStat ps) { writer.Write(5); writer.Write((int)ps); }
                else if (o is ResistanceType rt) { writer.Write(6); writer.Write((int)rt); }
                else if (o is SkillName sn) { writer.Write(7); writer.Write((int)sn); }
                else { writer.Write(0); continue; }
                writer.Write(entry.Value);
                writer.Write(entry.Cost);
            }
        }
    }

    public class AbilityProfile
    {
        [CommandProperty(AccessLevel.GameMaster)] public bool TokunoTame { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public int RegenHits { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public int RegenStam { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public int RegenMana { get; set; }
        [CommandProperty(AccessLevel.GameMaster)] public int DamageIndex { get; set; } = -1;
        [CommandProperty(AccessLevel.GameMaster)] public BaseCreature Creature { get; private set; }

        public MagicalAbility MagicalAbility { get; set; }
        public SpecialAbility[] SpecialAbilities { get; set; } = Array.Empty<SpecialAbility>();
        public WeaponAbility[] WeaponAbilities { get; set; } = Array.Empty<WeaponAbility>();
        public AreaEffect[] AreaEffects { get; set; } = Array.Empty<AreaEffect>();

        public AbilityProfile(BaseCreature bc) => Creature = bc;

        public void OnTame() { if (Creature.Map == Map.Tokuno) TokunoTame = true; }

        public override string ToString() => "...";

        // ---- persistence ----
        //
        // The wire format of version 1 is unchanged: the three ability arrays were always written
        // with a zero count and their contents discarded on load. Version 1 now writes real
        // indices, so old saves still read (as empty arrays) and new saves carry the ability set.

        public AbilityProfile(BaseCreature bc, IGenericReader reader)
        {
            var version = reader.ReadInt();
            Creature = bc;
            DamageIndex = version >= 1 ? reader.ReadInt() : -1;

            MagicalAbility = (MagicalAbility)reader.ReadInt();
            TokunoTame = reader.ReadBool();
            RegenHits = reader.ReadInt();
            RegenStam = reader.ReadInt();
            RegenMana = reader.ReadInt();

            SpecialAbilities = ReadAbilities(reader, SpecialAbility.Abilities);
            AreaEffects = ReadAbilities(reader, AreaEffect.Effects);
            WeaponAbilities = ReadAbilities(reader, PetTrainingHelper.WeaponAbilities);
        }

        private static T[] ReadAbilities<T>(IGenericReader reader, T[] registry) where T : class
        {
            var count = reader.ReadInt();

            if (count <= 0)
            {
                return Array.Empty<T>();
            }

            // A corrupt or oversized count must not over-read the stream, so drain defensively.
            if (count > registry.Length || count > 64)
            {
                for (var i = 0; i < count; i++)
                {
                    reader.ReadInt();
                }

                return Array.Empty<T>();
            }

            var result = new T[count];
            var valid = 0;

            for (var i = 0; i < count; i++)
            {
                var index = reader.ReadInt();

                if (index >= 0 && index < registry.Length && registry[index] != null)
                {
                    result[valid++] = registry[index];
                }
            }

            if (valid == count)
            {
                return result;
            }

            var trimmed = new T[valid];
            Array.Copy(result, trimmed, valid);
            return trimmed;
        }

        private static void WriteAbilities<T>(IGenericWriter writer, T[] abilities, T[] registry) where T : class
        {
            var count = 0;

            if (abilities != null)
            {
                for (var i = 0; i < abilities.Length; i++)
                {
                    if (abilities[i] != null && Array.IndexOf(registry, abilities[i]) >= 0)
                    {
                        count++;
                    }
                }
            }

            writer.Write(count);

            if (count == 0)
            {
                return;
            }

            for (var i = 0; i < abilities.Length; i++)
            {
                var index = abilities[i] == null ? -1 : Array.IndexOf(registry, abilities[i]);

                if (index >= 0)
                {
                    writer.Write(index);
                }
            }
        }

        public void Serialize(IGenericWriter writer)
        {
            writer.Write(1);
            writer.Write(DamageIndex);
            writer.Write((int)MagicalAbility);
            writer.Write(TokunoTame);
            writer.Write(RegenHits);
            writer.Write(RegenStam);
            writer.Write(RegenMana);
            WriteAbilities(writer, SpecialAbilities, SpecialAbility.Abilities);
            WriteAbilities(writer, AreaEffects, AreaEffect.Effects);
            WriteAbilities(writer, WeaponAbilities, PetTrainingHelper.WeaponAbilities);
        }

        // ---- ability queries ----

        public bool HasAbility(object o) => o switch
        {
            MagicalAbility ma => (MagicalAbility & ma) != 0,
            SpecialAbility sa => SpecialAbilities != null && Array.IndexOf(SpecialAbilities, sa) >= 0,
            AreaEffect ae => AreaEffects != null && Array.IndexOf(AreaEffects, ae) >= 0,
            WeaponAbility wa => WeaponAbilities != null && Array.IndexOf(WeaponAbilities, wa) >= 0,
            _ => false,
        };

        public int AbilityCount()
        {
            var count = 0;

            if (MagicalAbility != MagicalAbility.None) count++;

            if (SpecialAbilities != null)
            {
                for (var i = 0; i < SpecialAbilities.Length; i++)
                {
                    if (SpecialAbilities[i] is { NaturalAbility: false })
                    {
                        count++;
                    }
                }
            }

            count += AreaEffects?.Length ?? 0;
            count += WeaponAbilities?.Length ?? 0;

            return count;
        }

        public bool HasSpecialMagicalAbility() => (MagicalAbility & SpecialAbility.SpecialMagicalMask) != 0;

        public static bool IsSpecialMagicalAbility(MagicalAbility ability) =>
            ability != MagicalAbility.None && ability <= MagicalAbility.WrestlingMastery;

        public bool IsRuleBreaker(SpecialAbility ability)
        {
            var ruleBreakers = PetTrainingHelper.RuleBreakers;

            for (var i = 0; i < ruleBreakers.Length; i++)
            {
                if (ruleBreakers[i] == ability)
                {
                    return true;
                }
            }

            return false;
        }

        private int NonNaturalSpecialCount()
        {
            var count = 0;

            if (SpecialAbilities != null)
            {
                for (var i = 0; i < SpecialAbilities.Length; i++)
                {
                    if (SpecialAbilities[i] is { NaturalAbility: false })
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public bool CanChooseMagicalAbility(MagicalAbility ability)
        {
            if (!Creature.Controlled)
            {
                return true;
            }

            if (HasSpecialMagicalAbility() && IsSpecialMagicalAbility(ability) && NonNaturalSpecialCount() > 0)
            {
                return false;
            }

            return true;
        }

        public bool CanChooseSpecialAbility(SpecialAbility[] list)
        {
            if (!Creature.Controlled)
            {
                return true;
            }

            var hasRuleBreaker = false;

            if (list != null)
            {
                for (var i = 0; i < list.Length; i++)
                {
                    if (IsRuleBreaker(list[i]))
                    {
                        hasRuleBreaker = true;
                        break;
                    }
                }
            }

            if (HasSpecialMagicalAbility() && hasRuleBreaker &&
                (AreaEffects == null || AreaEffects.Length == 0) &&
                (SpecialAbilities == null || SpecialAbilities.Length == 0 || NonNaturalSpecialCount() == 0))
            {
                return true;
            }

            return !HasSpecialMagicalAbility() && NonNaturalSpecialCount() == 0 && AbilityCount() < 3;
        }

        public bool CanChooseAreaEffect()
        {
            if (!Creature.Controlled)
            {
                return true;
            }

            if (HasSpecialMagicalAbility() &&
                (AreaEffects == null || AreaEffects.Length == 0) &&
                (SpecialAbilities == null || SpecialAbilities.Length == 0))
            {
                return true;
            }

            return !HasSpecialMagicalAbility() && (AreaEffects == null || AreaEffects.Length == 0) && AbilityCount() < 3;
        }

        public bool CanChooseWeaponAbility()
        {
            if (!Creature.Controlled)
            {
                return true;
            }

            return !HasSpecialMagicalAbility() && (WeaponAbilities == null || WeaponAbilities.Length < 2) && AbilityCount() < 3;
        }

        // ---- ability mutation ----

        public bool AddAbility(MagicalAbility ability, bool advancement = true)
        {
            if (Creature.Controlled)
            {
                var oldAbility = MagicalAbility;

                if (IsSpecialMagicalAbility(oldAbility))
                {
                    RemoveSpecialMagicalAbility();
                }

                OnRemoveMagicalAbility(oldAbility);

                MagicalAbility = ability;
            }
            else
            {
                MagicalAbility |= ability;
            }

            OnAddAbility(ability, advancement);
            return true;
        }

        public bool AddAbility(SpecialAbility ability, bool advancement = true)
        {
            SpecialAbilities = Append(SpecialAbilities, ability);
            OnAddAbility(ability, advancement);
            return true;
        }

        public bool AddAbility(AreaEffect ability, bool advancement = true)
        {
            AreaEffects = Append(AreaEffects, ability);
            OnAddAbility(ability, advancement);
            return true;
        }

        public bool AddAbility(WeaponAbility ability, bool advancement = true)
        {
            WeaponAbilities = Append(WeaponAbilities, ability);
            OnAddAbility(ability, advancement);
            return true;
        }

        public bool AddAbility(SkillName skill, bool advancement = true)
        {
            OnAddAbility(skill, advancement);
            return true;
        }

        private static T[] Append<T>(T[] array, T value) where T : class
        {
            if (value == null)
            {
                return array ?? Array.Empty<T>();
            }

            if (array == null || array.Length == 0)
            {
                return new[] { value };
            }

            if (Array.IndexOf(array, value) >= 0)
            {
                return array;
            }

            var result = new T[array.Length + 1];
            Array.Copy(array, result, array.Length);
            result[array.Length] = value;
            return result;
        }

        private static T[] Remove<T>(T[] array, T value) where T : class
        {
            if (array == null)
            {
                return Array.Empty<T>();
            }

            var index = Array.IndexOf(array, value);

            if (index < 0)
            {
                return array;
            }

            var result = new T[array.Length - 1];

            if (index > 0)
            {
                Array.Copy(array, 0, result, 0, index);
            }

            if (index < array.Length - 1)
            {
                Array.Copy(array, index + 1, result, index, array.Length - index - 1);
            }

            return result;
        }

        public void RemoveAbility(MagicalAbility ability)
        {
            if ((MagicalAbility & ability) != 0)
            {
                MagicalAbility ^= ability;
                Invalidate();
            }
        }

        public void RemoveAbility(SpecialAbility ability)
        {
            if (SpecialAbilities == null || Array.IndexOf(SpecialAbilities, ability) < 0)
            {
                return;
            }

            SpecialAbilities = Remove(SpecialAbilities, ability);
            Invalidate();
        }

        public void RemoveAbility(WeaponAbility ability)
        {
            if (WeaponAbilities == null || Array.IndexOf(WeaponAbilities, ability) < 0)
            {
                return;
            }

            WeaponAbilities = Remove(WeaponAbilities, ability);
            Invalidate();
        }

        public void RemoveAbility(AreaEffect ability)
        {
            if (AreaEffects == null || Array.IndexOf(AreaEffects, ability) < 0)
            {
                return;
            }

            AreaEffects = Remove(AreaEffects, ability);
            Invalidate();
        }

        public bool CanAddAbility(object o)
        {
            if (!Creature.Controlled)
            {
                return true;
            }

            return o switch
            {
                MagicalAbility _ => true,
                SpecialAbility => SpecialAbilities == null || SpecialAbilities.Length == 0,
                AreaEffect => AreaEffects == null || AreaEffects.Length == 0,
                WeaponAbility => WeaponAbilities == null || WeaponAbilities.Length < 2,
                _ => false,
            };
        }

        private void Invalidate()
        {
            if (Creature != null)
            {
                Creature.InvalidateLearnedAbilities();
            }
        }

        public void OnAddAbility(object newAbility, bool advancement)
        {
            if (newAbility is MagicalAbility magical)
            {
                ApplyMagicalAbility(magical);
            }

            Invalidate();

            var trainPoint = PetTrainingHelper.GetTrainingPoint(newAbility);

            if (trainPoint?.Requirements == null)
            {
                return;
            }

            for (var i = 0; i < trainPoint.Requirements.Length; i++)
            {
                var req = trainPoint.Requirements[i];

                if (req?.Requirement is SkillName requiredSkill)
                {
                    var toAdd = req.Cost == 100 ? 20 : 40;

                    if (requiredSkill == SkillName.Hiding)
                    {
                        toAdd = 100;
                    }

                    if (Creature.Skills[requiredSkill].Base < toAdd)
                    {
                        Creature.Skills[requiredSkill].Base = toAdd;
                    }
                }
                else if (req?.Requirement is WeaponAbility granted)
                {
                    AddAbility(granted, false);
                }
            }
        }

        /// <summary>
        /// Applies the creature-side effects of a magical school: mastery skill, forced AI, and the
        /// companion special moves that Bushido/Ninjitsu grant.
        /// </summary>
        public void ApplyMagicalAbility(MagicalAbility ability)
        {
            if (IsSpecialMagicalAbility(ability))
            {
                WeaponAbilities = Array.Empty<WeaponAbility>();
                Creature.Mastery = ability switch
                {
                    MagicalAbility.Piercing => SkillName.Fencing,
                    MagicalAbility.Bashing => SkillName.Macing,
                    MagicalAbility.Slashing => SkillName.Swords,
                    MagicalAbility.BattleDefense => SkillName.Parry,
                    MagicalAbility.WrestlingMastery => SkillName.Wrestling,
                    _ => Creature.Mastery,
                };

                if (Creature.Controlled)
                {
                    Creature.AI = AIType.AI_Melee.Supported();
                }

                return;
            }

            switch (ability)
            {
                case MagicalAbility.Poisoning:
                case MagicalAbility.Discordance:
                case MagicalAbility.Magery:
                case MagicalAbility.MageryMastery:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Mage && ability != MagicalAbility.Poisoning &&
                        ability != MagicalAbility.Discordance)
                    {
                        Creature.AI = AIType.AI_Mage.Supported();
                    }
                    else if (Creature.Controlled && Creature.AI != AIType.AI_Melee &&
                             (ability == MagicalAbility.Poisoning || ability == MagicalAbility.Discordance))
                    {
                        Creature.AI = AIType.AI_Melee.Supported();
                    }

                    break;
                case MagicalAbility.Bushido:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Samurai)
                    {
                        Creature.AI = AIType.AI_Samurai.Supported();
                    }

                    if (!HasAbility(WeaponAbility.WhirlwindAttack))
                    {
                        AddAbility(WeaponAbility.WhirlwindAttack, false);
                    }

                    break;
                case MagicalAbility.Ninjitsu:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Ninja)
                    {
                        Creature.AI = AIType.AI_Ninja.Supported();
                    }

                    if (!HasAbility(WeaponAbility.FrenziedWhirlwind))
                    {
                        AddAbility(WeaponAbility.FrenziedWhirlwind, false);
                    }

                    break;
                case MagicalAbility.Mysticism:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Mystic)
                    {
                        Creature.AI = AIType.AI_Mystic.Supported();
                    }

                    break;
                case MagicalAbility.Spellweaving:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Spellweaving)
                    {
                        Creature.AI = AIType.AI_Spellweaving.Supported();
                    }

                    break;
                case MagicalAbility.Chivalry:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Paladin)
                    {
                        Creature.AI = AIType.AI_Paladin.Supported();
                    }

                    break;
                case MagicalAbility.Necromage:
                    if (Creature.Controlled && Creature.AI != AIType.AI_NecroMage)
                    {
                        Creature.AI = AIType.AI_NecroMage.Supported();
                    }

                    break;
                case MagicalAbility.Necromancy:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Necro)
                    {
                        Creature.AI = AIType.AI_Necro.Supported();
                    }

                    break;
            }
        }

        private void OnRemoveMagicalAbility(MagicalAbility oldAbility)
        {
            if ((oldAbility & MagicalAbility.Bushido) != 0)
            {
                RemoveAbility(WeaponAbility.WhirlwindAttack);
            }

            if ((oldAbility & MagicalAbility.Ninjitsu) != 0)
            {
                RemoveAbility(WeaponAbility.FrenziedWhirlwind);
            }
        }

        private void RemoveSpecialMagicalAbility()
        {
            WeaponAbilities = Array.Empty<WeaponAbility>();
            Creature.Mastery = SkillName.Alchemy;
        }
    }

    // ==========================================================================================
    // ABILITY TRIGGER BOOTSTRAP
    //
    // Approach-triggered abilities are handled by BaseCreature.OnMovement; the remaining four
    // trigger families (Think, GiveMeleeDamage, TakeMeleeDamage, GiveSpellDamage,
    // TakeSpellDamage) are already forwarded to MonsterAbility by the native damage pipeline
    // (BaseCreature.TriggerAbility / BaseWeapon.OnSwing / SpellHelper.Damage), so no extra
    // bootstrap class is needed.
    // ==========================================================================================

    // ==========================================================================================
    // PET TRAINING HELPER
    // ==========================================================================================

    public static class PetTrainingHelper
    {
        public static bool Enabled => Core.TOL;

        public static List<TrainingPoint> TrainingPoints => _TrainingPoints;
        public static List<TrainingPoint> _TrainingPoints;

        public static TrainingDefinition[] Definitions { get; private set; } = Array.Empty<TrainingDefinition>();

        public static MagicalAbility[] MagicalAbilities { get; } =
        {
            MagicalAbility.Piercing, MagicalAbility.Bashing, MagicalAbility.Slashing,
            MagicalAbility.BattleDefense, MagicalAbility.WrestlingMastery, MagicalAbility.Poisoning,
            MagicalAbility.Bushido, MagicalAbility.Ninjitsu, MagicalAbility.Discordance,
            MagicalAbility.MageryMastery, MagicalAbility.Mysticism, MagicalAbility.Spellweaving,
            MagicalAbility.Chivalry, MagicalAbility.Necromage, MagicalAbility.Necromancy,
            MagicalAbility.Magery,
        };

        /// <summary>
        /// The trainable special moves, in the definition's display order. ServUO's ColdWind has no
        /// ModernUO equivalent and is intentionally absent.
        /// </summary>
        public static WeaponAbility[] WeaponAbilities { get; } =
        {
            WeaponAbility.NerveStrike,
            WeaponAbility.WhirlwindAttack,
            WeaponAbility.LightningArrow,
            WeaponAbility.InfusedThrow,
            WeaponAbility.MysticArc,
            WeaponAbility.TalonStrike,
            WeaponAbility.PsychicAttack,
            WeaponAbility.ArmorIgnore,
            WeaponAbility.ArmorPierce,
            WeaponAbility.Bladeweave,
            WeaponAbility.BleedAttack,
            WeaponAbility.Block,
            WeaponAbility.ConcussionBlow,
            WeaponAbility.CrushingBlow,
            WeaponAbility.Disarm,
            WeaponAbility.Dismount,
            WeaponAbility.DoubleStrike,
            WeaponAbility.DualWield,
            WeaponAbility.Feint,
            WeaponAbility.ForceOfNature,
            WeaponAbility.FrenziedWhirlwind,
            WeaponAbility.MortalStrike,
            WeaponAbility.ParalyzingBlow,
        };

        public static SpecialAbility[] SpecialAbilityNone { get; } = Array.Empty<SpecialAbility>();
        public static AreaEffect[] AreaEffectNone { get; } = Array.Empty<AreaEffect>();
        public static WeaponAbility[] WeaponAbilityNone { get; } = Array.Empty<WeaponAbility>();

        public static SpecialAbility[] RuleBreakers { get; } =
        {
            SpecialAbility.SearingWounds, SpecialAbility.TailSwipe, SpecialAbility.DragonBreath,
            SpecialAbility.LifeLeech, SpecialAbility.ViciousBite,
        };

        public static SkillName[] MagicSkills =
        {
            SkillName.Magery, SkillName.EvalInt, SkillName.Necromancy, SkillName.SpiritSpeak,
            SkillName.Chivalry, SkillName.Focus, SkillName.Bushido, SkillName.Ninjitsu,
            SkillName.Spellweaving, SkillName.Mysticism, SkillName.Meditation,
            SkillName.MagicResist, SkillName.Discordance
        };

        public static SkillName[] CombatSkills =
        {
            SkillName.Wrestling, SkillName.Tactics, SkillName.Anatomy,
            SkillName.Parry, SkillName.Healing
        };

        #region Accessors

        public static TrainingDefinition GetTrainingDefinition(BaseCreature bc)
        {
            if (bc == null) return null;

            var def = bc.TrainingDefinition ?? FindDefinition(bc.GetType());

            if (def == null && bc.Tamable)
            {
                var min = bc.ControlSlotsMin == 0 ? bc.ControlSlots : bc.ControlSlotsMin;
                var max = bc.ControlSlotsMax == 0 ? bc.ControlSlots : bc.ControlSlotsMax;
                def = new TrainingDefinition(bc.GetType(), Class.Untrainable, min, max);
            }

            return def;
        }

        public static TrainingDefinition FindDefinition(Type type)
        {
            var definitions = Definitions;

            if (definitions == null || type == null)
            {
                return null;
            }

            for (var i = 0; i < definitions.Length; i++)
            {
                var def = definitions[i];

                if (def != null && def.CreatureType == type)
                {
                    return def;
                }
            }

            return null;
        }

        public static TrainingPoint GetTrainingPoint(object o)
        {
            if (_TrainingPoints == null || o == null) return null;

            foreach (var tp in _TrainingPoints)
            {
                if (Matches(tp.TrainPoint, o))
                {
                    return tp;
                }
            }

            return null;
        }

        private static bool Matches(object a, object b) => a switch
        {
            PetStat stat => b is PetStat other && stat == other,
            ResistanceType resist => b is ResistanceType otherResist && resist == otherResist,
            SkillName skill => b is SkillName otherSkill && skill == otherSkill,
            MagicalAbility magical => b is MagicalAbility otherMagical && magical == otherMagical,
            SpecialAbility special => b is SpecialAbility otherSpecial && special == otherSpecial,
            WeaponAbility weapon => b is WeaponAbility otherWeapon && weapon == otherWeapon,
            AreaEffect area => b is AreaEffect otherArea && area == otherArea,
            _ => false,
        };

        public static AbilityProfile GetAbilityProfile(BaseCreature bc, bool create = false)
        {
            var profile = bc.AbilityProfile;
            if (profile == null && create) bc.AbilityProfile = profile = new AbilityProfile(bc);
            return profile;
        }

        public static TrainingProfile GetTrainingProfile(BaseCreature bc, bool create = false)
        {
            var profile = bc.TrainingProfile;
            if (profile == null && create) bc.TrainingProfile = profile = new TrainingProfile(bc);
            return profile;
        }

        public static PlanningProfile GetPlanningProfile(BaseCreature bc, bool create = false)
            => GetTrainingProfile(bc, create)?.PlanningProfile;

        #endregion

        #region Configure

        public static void Configure()
        {
            if (_TrainingPoints != null) return;
            _TrainingPoints = new List<TrainingPoint>();

            // Stats
            _TrainingPoints.Add(new TrainingPoint(PetStat.Str, 3.0, 1, 700, 1061146, 1157507));
            _TrainingPoints.Add(new TrainingPoint(PetStat.Dex, 0.1, 1, 150, 1061147, 1157508));
            _TrainingPoints.Add(new TrainingPoint(PetStat.Int, 0.5, 1, 700, 1061148, 1157509));
            _TrainingPoints.Add(new TrainingPoint(PetStat.Hits, 3.0, 1, 1100, 1061149, 1157510));
            _TrainingPoints.Add(new TrainingPoint(PetStat.Stam, 0.5, 1, 150, 1061150, 1157511));
            _TrainingPoints.Add(new TrainingPoint(PetStat.Mana, 0.5, 1, 1500, 1061151, 1157512));
            _TrainingPoints.Add(new TrainingPoint(PetStat.RegenHits, 18.0, 1, 20, 1075627, 1157513));
            _TrainingPoints.Add(new TrainingPoint(PetStat.RegenStam, 12.0, 1, 30, 1079411, 1157514));
            _TrainingPoints.Add(new TrainingPoint(PetStat.RegenMana, 12.0, 1, 30, 1079410, 1157515));
            _TrainingPoints.Add(new TrainingPoint(PetStat.BaseDamage, 5.0, 1, 22, 1157506, 1157516));

            // Resistances
            _TrainingPoints.Add(new TrainingPoint(ResistanceType.Physical, 3.0, 1, 80, 1061158, 1157517));
            _TrainingPoints.Add(new TrainingPoint(ResistanceType.Fire, 3.0, 1, 80, 1061159, 1157518));
            _TrainingPoints.Add(new TrainingPoint(ResistanceType.Cold, 3.0, 1, 80, 1061160, 1157519));
            _TrainingPoints.Add(new TrainingPoint(ResistanceType.Poison, 3.0, 1, 80, 1061161, 1157520));
            _TrainingPoints.Add(new TrainingPoint(ResistanceType.Energy, 3.0, 1, 80, 1061162, 1157521));

            // Magic skill caps
            _TrainingPoints.Add(new TrainingPoint(SkillName.Magery, 0.5, 50, 200, 1002106, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.EvalInt, 1.0, 50, 200, 1044076, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Necromancy, 0.5, 50, 200, 1044109, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.SpiritSpeak, 1.0, 50, 200, 1044092, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Chivalry, 0.5, 50, 200, 1044111, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Focus, 0.1, 50, 200, 1044110, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Bushido, 0.5, 50, 200, 1044112, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Ninjitsu, 0.5, 50, 200, 1044113, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Spellweaving, 0.5, 50, 200, 1044114, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Mysticism, 0.5, 50, 200, 1044115, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Meditation, 0.1, 50, 200, 1044106, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.MagicResist, 0.1, 50, 200, 1044086, 1157522));

            // Combat skill caps
            _TrainingPoints.Add(new TrainingPoint(SkillName.Wrestling, 1.0, 50, 200, 1044103, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Tactics, 1.0, 50, 200, 1044087, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Anatomy, 0.1, 50, 200, 1044061, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Parry, 0.1, 50, 200, 1002118, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Healing, 0.1, 50, 200, 1002082, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Discordance, 0.1, 50, 200, 1044075, 1157522));

            // Magical abilities
            var magicalLoc = MagicalAbilityLocalizations;

            for (var i = 0; i < MagicalAbilities.Length; i++)
            {
                var ability = MagicalAbilities[i];
                var loc = i < magicalLoc.Length ? magicalLoc[i] : new TextDefinition[] { 0, 0 };
                _TrainingPoints.Add(new TrainingPoint(ability, 1.0, 1, 1, loc[0], loc[1], GetMagicalRequirements(ability)));
            }

            // Special abilities (index-aligned with the localisation table)
            var specialLoc = SpecialAbilityLocalizations;

            for (var i = 0; i < SpecialAbility.Abilities.Length; i++)
            {
                if (i >= specialLoc.Length) break;
                _TrainingPoints.Add(new TrainingPoint(SpecialAbility.Abilities[i], 1.0, 100, 100, specialLoc[i][0], specialLoc[i][1]));
            }

            // Area effects
            var areaLoc = AreaEffectLocalizations;

            for (var i = 0; i < AreaEffect.Effects.Length; i++)
            {
                if (i >= areaLoc.Length) break;
                _TrainingPoints.Add(new TrainingPoint(AreaEffect.Effects[i], 1.0, 100, 100, areaLoc[i][0], areaLoc[i][1]));
            }

            // Special moves
            var weaponLoc = WeaponAbilityLocalizations;

            for (var i = 0; i < WeaponAbilities.Length; i++)
            {
                var ability = WeaponAbilities[i];
                var loc = i < weaponLoc.Length ? weaponLoc[i] : new TextDefinition[] { 0, 0 };
                _TrainingPoints.Add(new TrainingPoint(ability, 1.0, 100, 100, loc[0], loc[1], GetWeaponRequirement(ability)));
            }

            // Definitions are built once; missing creature types are skipped with a log line.
            Definitions = PetTrainingDefinitions.Build();
        }

        private static TrainingPointRequirement[] GetMagicalRequirements(MagicalAbility ability) => ability switch
        {
            MagicalAbility.Piercing => new[]
            {
                new TrainingPointRequirement(WeaponAbility.ArmorIgnore, 100, 1028838),
                new TrainingPointRequirement(WeaponAbility.ParalyzingBlow, 100, 1028848),
                new TrainingPointRequirement(WeaponAbility.BleedAttack, 100, 1028839),
            },
            MagicalAbility.Bashing => new[]
            {
                new TrainingPointRequirement(WeaponAbility.MortalStrike, 100, 1028846),
                new TrainingPointRequirement(WeaponAbility.ConcussionBlow, 100, 1028840),
                new TrainingPointRequirement(WeaponAbility.Disarm, 100, 1028842),
            },
            MagicalAbility.Slashing => new[]
            {
                new TrainingPointRequirement(SkillName.Bushido, 500, 1044112),
                new TrainingPointRequirement(WeaponAbility.ArmorIgnore, 100, 1028838),
                new TrainingPointRequirement(WeaponAbility.Disarm, 100, 1028842),
                new TrainingPointRequirement(WeaponAbility.NerveStrike, 100, 1028855),
            },
            MagicalAbility.BattleDefense => new[]
            {
                new TrainingPointRequirement(WeaponAbility.Disarm, 100, 1028842),
                new TrainingPointRequirement(WeaponAbility.ParalyzingBlow, 100, 1028848),
            },
            MagicalAbility.WrestlingMastery => new[]
            {
                new TrainingPointRequirement(WeaponAbility.Disarm, 100, 1028842),
                new TrainingPointRequirement(WeaponAbility.ParalyzingBlow, 100, 1028848),
            },
            MagicalAbility.Poisoning => new[] { new TrainingPointRequirement(SkillName.Poisoning, 100, 1044090) },
            MagicalAbility.Bushido => new[] { new TrainingPointRequirement(SkillName.Bushido, 500, 1044112) },
            MagicalAbility.Ninjitsu => new[]
            {
                new TrainingPointRequirement(SkillName.Ninjitsu, 500, 1044113),
                new TrainingPointRequirement(SkillName.Hiding, 100, 1002088),
            },
            MagicalAbility.Discordance => new[] { new TrainingPointRequirement(SkillName.Discordance, 500, 1044075) },
            MagicalAbility.MageryMastery => new[]
            {
                new TrainingPointRequirement(SkillName.Magery, 100, 1044085),
                new TrainingPointRequirement(SkillName.EvalInt, 1000, 1044076),
            },
            MagicalAbility.Mysticism => new[] { new TrainingPointRequirement(SkillName.Mysticism, 500, 1044115) },
            MagicalAbility.Spellweaving => new[] { new TrainingPointRequirement(SkillName.Spellweaving, 500, 1044114) },
            MagicalAbility.Chivalry => new[] { new TrainingPointRequirement(SkillName.Chivalry, 500, 1044111) },
            MagicalAbility.Necromage => new[]
            {
                new TrainingPointRequirement(SkillName.Magery, 100, 1044085),
                new TrainingPointRequirement(SkillName.Necromancy, 100, 1044109),
                new TrainingPointRequirement(SkillName.SpiritSpeak, 100, 1044092),
                new TrainingPointRequirement(SkillName.EvalInt, 100, 1044076),
            },
            MagicalAbility.Necromancy => new[]
            {
                new TrainingPointRequirement(SkillName.Necromancy, 100, 1044109),
                new TrainingPointRequirement(SkillName.SpiritSpeak, 100, 1044092),
            },
            _ => Array.Empty<TrainingPointRequirement>(),
        };

        private static TrainingPointRequirement[] GetWeaponRequirement(WeaponAbility ability) => ability switch
        {
            _ when ability == WeaponAbility.NerveStrike => new[] { new TrainingPointRequirement(SkillName.Bushido, 500, 1044112) },
            _ when ability == WeaponAbility.TalonStrike => new[] { new TrainingPointRequirement(SkillName.Ninjitsu, 500, 1044113) },
            _ when ability == WeaponAbility.Feint => new[] { new TrainingPointRequirement(SkillName.Bushido, 500, 1044112) },
            _ when ability == WeaponAbility.FrenziedWhirlwind => new[] { new TrainingPointRequirement(SkillName.Ninjitsu, 500, 1044113) },
            _ when ability == WeaponAbility.Bladeweave => new[] { new TrainingPointRequirement(SkillName.Bushido, 500, 1044112) },
            _ when ability == WeaponAbility.Block => new[] { new TrainingPointRequirement(SkillName.Bushido, 500, 1044112) },
            _ => Array.Empty<TrainingPointRequirement>(),
        };

        #endregion

        #region Helpers

        public static bool CanControl(Mobile m, BaseCreature bc, TrainingProfile trainProfile)
        {
            var projected = Math.Min(BaseCreature.MaxTameRequirement,
                bc.CurrentTameSkill + trainProfile.GetRequirementIncrease(!trainProfile.HasIncreasedControlSlot));
            return m.Skills[SkillName.AnimalTaming].Value >= projected;
        }

        public static int GetTrainingCapTotal(PetStat stat) => stat switch
        {
            PetStat.Str or PetStat.Int or PetStat.Dex => 2300,
            PetStat.Hits or PetStat.Stam or PetStat.Mana => 3300,
            _ => 0,
        };

        public static int GetTrainingCapTotal(ResistanceType resist) => 1095;

        public static int GetTotalStatWeight(BaseCreature bc)
        {
            var str = GetTrainingPoint(PetStat.Str);
            var dex = GetTrainingPoint(PetStat.Dex);
            var intel = GetTrainingPoint(PetStat.Int);
            return (int)(Math.Min(bc.RawStr * str.Weight, str.GetMax(bc) * str.Weight) +
                         Math.Min(bc.RawDex * dex.Weight, dex.GetMax(bc) * dex.Weight) +
                         Math.Min(bc.RawInt * intel.Weight, intel.GetMax(bc) * intel.Weight));
        }

        public static int GetTotalAttributeWeight(BaseCreature bc)
        {
            var hits = GetTrainingPoint(PetStat.Hits);
            var stam = GetTrainingPoint(PetStat.Stam);
            var mana = GetTrainingPoint(PetStat.Mana);
            return (int)(Math.Min(bc.HitsMax * hits.Weight, hits.GetMax(bc) * hits.Weight) +
                         Math.Min(bc.StamMax * stam.Weight, stam.GetMax(bc) * stam.Weight) +
                         Math.Min(bc.ManaMax * mana.Weight, mana.GetMax(bc) * mana.Weight));
        }

        public static int GetTotalResistWeight(BaseCreature bc)
        {
            var phys = GetTrainingPoint(ResistanceType.Physical);
            var fire = GetTrainingPoint(ResistanceType.Fire);
            var cold = GetTrainingPoint(ResistanceType.Cold);
            var pois = GetTrainingPoint(ResistanceType.Poison);
            var nrgy = GetTrainingPoint(ResistanceType.Energy);
            return (int)(bc.PhysicalResistanceSeed * phys.Weight +
                         bc.FireResistSeed * fire.Weight +
                         bc.ColdResistSeed * cold.Weight +
                         bc.PoisonResistSeed * pois.Weight +
                         bc.EnergyResistSeed * nrgy.Weight);
        }

        public static int GetMaxDamagePerSecond(BaseCreature bc)
        {
            var slots = bc.ControlSlots;
            var profile = GetTrainingProfile(bc);
            if (profile != null && !profile.HasIncreasedControlSlot) slots++;
            return slots switch { 2 => 8, 3 => 13, 4 => 17, _ => 22 };
        }

        private static readonly int[][] _DamageTable =
        {
            new[] { 0, 1 }, new[] { 2, 3 }, new[] { 3, 4 }, new[] { 4, 6 }, new[] { 5, 7 },
            new[] { 6, 9 }, new[] { 7, 10 }, new[] { 9, 12 }, new[] { 9, 13 }, new[] { 11, 15 },
            new[] { 12, 16 }, new[] { 13, 18 }, new[] { 14, 19 }, new[] { 15, 21 }, new[] { 16, 22 },
            new[] { 18, 24 }, new[] { 18, 25 }, new[] { 19, 26 }, new[] { 20, 28 }, new[] { 21, 29 },
            new[] { 23, 31 }, new[] { 24, 33 },
        };

        public static bool SetDamage(BaseCreature bc, int value)
        {
            if (value < 0 || value >= _DamageTable.Length) return false;
            bc.SetDamage(_DamageTable[value][0], _DamageTable[value][1]);
            GetAbilityProfile(bc, true).DamageIndex = value;
            return true;
        }

        public static void GetStartValue(TrainingPoint tp, BaseCreature bc, ref int value)
        {
            if (tp.Start == tp.Max) return;

            if (tp.TrainPoint is PetStat stat)
            {
                var profile = GetAbilityProfile(bc, true);
                switch (stat)
                {
                    case PetStat.Str: value = Math.Max(bc.RawStr, value); break;
                    case PetStat.Dex: value = Math.Max(bc.RawDex, value); break;
                    case PetStat.Int: value = Math.Max(bc.RawInt, value); break;
                    case PetStat.Hits: value = Math.Max(bc.HitsMax, value); break;
                    case PetStat.Stam: value = Math.Max(bc.StamMax, value); break;
                    case PetStat.Mana: value = Math.Max(bc.ManaMax, value); break;
                    case PetStat.RegenHits: value = Math.Max(profile.RegenHits, value); break;
                    case PetStat.RegenStam: value = Math.Max(profile.RegenStam, value); break;
                    case PetStat.RegenMana: value = Math.Max(profile.RegenMana, value); break;
                    case PetStat.BaseDamage:
                        if (profile.DamageIndex > -1) { value = profile.DamageIndex + 1; break; }
                        if (bc.DamageMin <= 1) { value = 1; break; }
                        for (var i = 0; i < _DamageTable.Length; i++)
                            if (_DamageTable[i][0] >= bc.DamageMin && bc.DamageMin <= _DamageTable[i][1])
                            { value = i + 1; return; }
                        value = _DamageTable.Length - 1;
                        break;
                }
            }
            else if (tp.TrainPoint is ResistanceType resist)
            {
                value = resist switch
                {
                    ResistanceType.Physical => Math.Max(bc.PhysicalResistanceSeed, value),
                    ResistanceType.Fire => Math.Max(bc.FireResistSeed, value),
                    ResistanceType.Cold => Math.Max(bc.ColdResistSeed, value),
                    ResistanceType.Poison => Math.Max(bc.PoisonResistSeed, value),
                    ResistanceType.Energy => Math.Max(bc.EnergyResistSeed, value),
                    _ => value,
                };
            }
            else if (tp.TrainPoint is SkillName)
            {
                var skill = bc.Skills[(SkillName)tp.TrainPoint];
                value = (int)skill.Cap switch
                {
                    < 105 => 0,
                    < 110 => 50,
                    < 115 => 100,
                    < 120 => 150,
                    _ => value,
                };
            }
        }

        public static int GetTotalCost(TrainingPoint tp, BaseCreature bc, int value, int startValue)
        {
            var cost = (int)(value * tp.Weight);
            if (startValue > 0) cost -= (int)(startValue * tp.Weight);
            return cost;
        }

        public static bool ApplyTrainingPoint(BaseCreature bc, TrainingPoint tp, int value)
        {
            var profile = GetAbilityProfile(bc, true);

            if (tp.TrainPoint is PetStat stat)
            {
                switch (stat)
                {
                    case PetStat.Str:
                        if (bc.HitsMaxSeed == -1) bc.HitsMaxSeed = bc.HitsMax;
                        bc.SetStr(value); break;
                    case PetStat.Dex:
                        if (bc.StamMaxSeed == -1) bc.StamMaxSeed = bc.StamMax;
                        bc.SetDex(value); break;
                    case PetStat.Int:
                        if (bc.ManaMaxSeed == -1) bc.ManaMaxSeed = bc.ManaMax;
                        bc.SetInt(value); break;
                    case PetStat.Hits: bc.SetHits(value); break;
                    case PetStat.Stam: bc.SetStam(value); break;
                    case PetStat.Mana: bc.SetMana(value); break;
                    case PetStat.RegenHits: profile.RegenHits = value; break;
                    case PetStat.RegenStam: profile.RegenStam = value; break;
                    case PetStat.RegenMana: profile.RegenMana = value; break;
                    case PetStat.BaseDamage: return SetDamage(bc, value - 1);
                }
                return true;
            }

            if (tp.TrainPoint is SkillName skill)
            {
                bc.Skills[skill].Cap = 100 + value / 10;
                return true;
            }

            if (tp.TrainPoint is ResistanceType resist)
            {
                switch (resist)
                {
                    case ResistanceType.Physical: bc.PhysicalResistanceSeed = value; break;
                    case ResistanceType.Fire: bc.FireResistSeed = value; break;
                    case ResistanceType.Cold: bc.ColdResistSeed = value; break;
                    case ResistanceType.Poison: bc.PoisonResistSeed = value; break;
                    case ResistanceType.Energy: bc.EnergyResistSeed = value; break;
                }
                return true;
            }

            if (tp.TrainPoint is MagicalAbility magical)
            {
                return ValidateTrainingPoint(bc, magical) && profile.AddAbility(magical);
            }

            if (tp.TrainPoint is SpecialAbility special)
            {
                return ValidateTrainingPoint(bc, special) && profile.AddAbility(special);
            }

            if (tp.TrainPoint is AreaEffect area)
            {
                return ValidateTrainingPoint(bc, area) && profile.AddAbility(area);
            }

            if (tp.TrainPoint is WeaponAbility weapon)
            {
                return ValidateTrainingPoint(bc, weapon) && profile.AddAbility(weapon);
            }

            return false;
        }

        public static bool ValidateTrainingPoint(BaseCreature bc, MagicalAbility ability)
        {
            var def = GetTrainingDefinition(bc);
            return def != null && (def.MagicalAbilities & ability) != 0;
        }

        public static bool ValidateTrainingPoint(BaseCreature bc, SpecialAbility ability)
        {
            var def = GetTrainingDefinition(bc);
            if (def == null) return false;
            if (def.HasSpecialAbility(ability)) return true;

            var profile = GetAbilityProfile(bc);
            return profile != null && (ability == SpecialAbility.ViciousBite || ability == SpecialAbility.VenomousBite) &&
                   profile.HasAbility(MagicalAbility.Poisoning);
        }

        public static bool ValidateTrainingPoint(BaseCreature bc, AreaEffect ability)
        {
            var def = GetTrainingDefinition(bc);
            if (def == null) return false;
            if (def.HasAreaEffect(ability)) return true;

            var profile = GetAbilityProfile(bc);
            return profile != null && ability == AreaEffect.PoisonBreath && profile.HasAbility(MagicalAbility.Poisoning);
        }

        public static bool ValidateTrainingPoint(BaseCreature bc, WeaponAbility ability)
        {
            var def = GetTrainingDefinition(bc);
            if (def == null) return false;

            for (var i = 0; i < def.WeaponAbilities.Length; i++)
            {
                if (def.WeaponAbilities[i] == ability) return true;
            }

            return false;
        }

        public static bool ValidateTrainingPoint(BaseCreature bc, SkillName skill)
            => bc.Skills[skill].Base > 0;

        public static bool CheckSecondarySkill(BaseCreature bc, SkillName skill)
        {
            if (Enabled && bc.Controlled)
            {
                var profile = GetAbilityProfile(bc);

                if (profile != null && profile.TokunoTame)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        public static void OnWeaponAbilityUsed(BaseCreature bc, SkillName skill)
        {
            if (Enabled && bc.Controlled)
            {
                var profile = GetAbilityProfile(bc);

                if (profile != null)
                {
                    bc.CheckSkill(skill, 0.0, bc.Skills[skill].Cap);
                }
            }
        }

        public static bool CommonSkill(BaseCreature bc, SkillName skill)
        {
            if (bc.Skills[SkillName.Wrestling].Base >= 100 && skill == SkillName.Parry) return true;
            var ai = bc.AI;
            if (skill == SkillName.Meditation &&
                (ai == AIType.AI_Mage || ai == AIType.AI_NecroMage ||
                 ai == AIType.AI_Spellweaving || ai == AIType.AI_Mystic))
                return true;
            return skill == SkillName.Focus || skill == SkillName.Tactics ||
                   skill == SkillName.Anatomy || skill == SkillName.MagicResist ||
                   skill == SkillName.Wrestling;
        }

        #endregion

        #region Localization

        /// <summary>
        /// Index-aligned with <see cref="MagicalAbilities"/>. Do not reorder: the tables are looked
        /// up by position.
        /// </summary>
        public static TextDefinition[][] MagicalAbilityLocalizations { get; } =
        {
            new TextDefinition[] { 1157559, 1157392 }, // Piercing
            new TextDefinition[] { 1157560, 1157471 }, // Bashing
            new TextDefinition[] { 1157561, 1157396 }, // Slashing
            new TextDefinition[] { 1157562, 1157395 }, // Battle Defense
            new TextDefinition[] { 1155784, 1157397 }, // Wrestling Mastery
            new TextDefinition[] { 1002122, 1157389 }, // Poisoning
            new TextDefinition[] { 1044112, 1157383 }, // Bushido
            new TextDefinition[] { 1044113, 1157388 }, // Ninjitsu
            new TextDefinition[] { 1044075, 1157385 }, // Discordance
            new TextDefinition[] { 1155771, 1157472 }, // Magery Mastery
            new TextDefinition[] { 1044115, 1157386 }, // Mysticism
            new TextDefinition[] { 1044114, 1157390 }, // Spellweaving
            new TextDefinition[] { 1044111, 1157384 }, // Chivalry
            new TextDefinition[] { 1157473, 1157474 }, // Necromage
            new TextDefinition[] { 1044109, 1157387 }, // Necromancy
            new TextDefinition[] { 1002106, 1157391 }, // Magery
        };

        /// <summary>Index-aligned with <see cref="SpecialAbility.Abilities"/>.</summary>
        public static TextDefinition[][] SpecialAbilityLocalizations { get; } =
        {
            new TextDefinition[] { 1157412, 1157413 }, // Angry Fire
            new TextDefinition[] { 1157406, 1157407 }, // Conductive Blast
            new TextDefinition[] { 1157414, 1157415 }, // Dragon Breath
            new TextDefinition[] { 1157400, 1157401 }, // Grasping Claw
            new TextDefinition[] { 1157416, 1157417 }, // Inferno
            new TextDefinition[] { 1157408, 1157409 }, // Lightning Force
            new TextDefinition[] { 1157432, 1157433 }, // Mana Drain
            new TextDefinition[] { 1157404, 1157405 }, // Raging Breath
            new TextDefinition[] { 1157434, 1157435 }, // Repel
            new TextDefinition[] { 1157422, 1157423 }, // Searing Wounds
            new TextDefinition[] { 1157410, 1157411 }, // Steal Life
            new TextDefinition[] { 1157430, 1157431 }, // Venomous Bite
            new TextDefinition[] { 1157420, 1157421 }, // Vicious Bite
            new TextDefinition[] { 1157398, 1157399 }, // Rune Corruption
            new TextDefinition[] { 1157424, 1157425 }, // Life Leech
            new TextDefinition[] { 1157426, 1157427 }, // Sticky Skin
            new TextDefinition[] { 1157428, 1157429 }, // Tail Swipe
            new TextDefinition[] { 1157418, 1157419 }, // Flurry Force
            new TextDefinition[] { 1150005, 0 },       // Rage
            new TextDefinition[] { 1151311, 0 },       // Heal
            new TextDefinition[] { 1153793, 0 },       // Howl of Cacophony
            new TextDefinition[] { 1153789, 0 },       // Webbing
            new TextDefinition[] { 1153797, 0 },       // Anemia
            new TextDefinition[] { 1153798, 0 },       // Blood Disease
            new TextDefinition[] { 0, 0 },             // Poison Spit (non-trainable)
            new TextDefinition[] { 0, 0 },             // True Fear (non-trainable)
            new TextDefinition[] { 0, 0 },             // Colossal Blow (non-trainable)
            new TextDefinition[] { 0, 0 },             // Life Drain (non-trainable)
            new TextDefinition[] { 0, 0 },             // Colossal Rage (non-trainable)
        };

        /// <summary>Index-aligned with <see cref="AreaEffect.Effects"/>.</summary>
        public static TextDefinition[][] AreaEffectLocalizations { get; } =
        {
            new TextDefinition[] { 1157459, 1157460 }, // Aura of Energy
            new TextDefinition[] { 1157467, 1157468 }, // Aura of Nausea
            new TextDefinition[] { 1157469, 1157470 }, // Essence of Disease
            new TextDefinition[] { 1157465, 1157466 }, // Essence of Earth
            new TextDefinition[] { 1157463, 1157464 }, // Explosive Goo
            new TextDefinition[] { 0, 0 },             // Aura Damage (hidden)
            new TextDefinition[] { 1157475, 1157476 }, // Poison Breath
        };

        /// <summary>Index-aligned with <see cref="WeaponAbilities"/>.</summary>
        public static TextDefinition[][] WeaponAbilityLocalizations { get; } =
        {
            new TextDefinition[] { 1028855, 1157436 }, // Nerve Strike
            new TextDefinition[] { 1028850, 1157437 }, // Whirlwind Attack
            new TextDefinition[] { 1028863, 1157438 }, // Lightning Arrow
            new TextDefinition[] { 1113283, 1157439 }, // Infused Throw
            new TextDefinition[] { 1113282, 1157440 }, // Mystic Arc
            new TextDefinition[] { 1028856, 1157441 }, // Talon Strike
            new TextDefinition[] { 1028864, 1157442 }, // Psychic Attack
            new TextDefinition[] { 1028838, 1157443 }, // Armor Ignore
            new TextDefinition[] { 1028860, 1157444 }, // Armor Pierce
            new TextDefinition[] { 1028861, 1157445 }, // Bladeweave
            new TextDefinition[] { 1028839, 1157446 }, // Bleed Attack
            new TextDefinition[] { 1028853, 1157447 }, // Block
            new TextDefinition[] { 1028840, 1157448 }, // Concussion Blow
            new TextDefinition[] { 1028841, 1157449 }, // Crushing Blow
            new TextDefinition[] { 1028842, 1157450 }, // Disarm
            new TextDefinition[] { 1028843, 1157451 }, // Dismount
            new TextDefinition[] { 1028844, 1157452 }, // Double Strike
            new TextDefinition[] { 1028858, 1157453 }, // Dual Wield
            new TextDefinition[] { 1028857, 1157454 }, // Feint
            new TextDefinition[] { 1028866, 1157455 }, // Force of Nature
            new TextDefinition[] { 1028852, 1157456 }, // Frenzied Whirlwind
            new TextDefinition[] { 1028846, 1157457 }, // Mortal Strike
            new TextDefinition[] { 1028848, 1157458 }, // Paralyzing Blow
        };

        public static TextDefinition[] GetLocalization(object o)
        {
            var tp = GetTrainingPoint(o);

            if (tp != null)
            {
                return new[] { tp.Name, tp.Description };
            }

            return o switch
            {
                MagicalAbility magical => GetLocalization(magical),
                SpecialAbility special => GetLocalization(special),
                AreaEffect area => GetLocalization(area),
                WeaponAbility weapon => GetLocalization(weapon),
                SkillName skill => GetLocalization(skill),
                _ => new TextDefinition[] { 0, 0 },
            };
        }

        public static TextDefinition[] GetLocalization(MagicalAbility ability)
        {
            var loc = MagicalAbilityLocalizations;

            for (var i = 0; i < MagicalAbilities.Length && i < loc.Length; i++)
            {
                if ((ability & MagicalAbilities[i]) != 0)
                {
                    return loc[i];
                }
            }

            return new TextDefinition[] { 0, 0 };
        }

        public static TextDefinition[] GetLocalization(SpecialAbility ability)
        {
            var index = Array.IndexOf(SpecialAbility.Abilities, ability);
            var loc = SpecialAbilityLocalizations;
            return index >= 0 && index < loc.Length ? loc[index] : new TextDefinition[] { 0, 0 };
        }

        public static TextDefinition[] GetLocalization(WeaponAbility weapon)
        {
            var index = Array.IndexOf(WeaponAbilities, weapon);
            var loc = WeaponAbilityLocalizations;
            return index >= 0 && index < loc.Length ? loc[index] : new TextDefinition[] { 0, 0 };
        }

        public static TextDefinition[] GetLocalization(AreaEffect area)
        {
            var index = Array.IndexOf(AreaEffect.Effects, area);
            var loc = AreaEffectLocalizations;
            return index >= 0 && index < loc.Length ? loc[index] : new TextDefinition[] { 0, 0 };
        }

        public static TextDefinition[] GetLocalization(SkillName skill)
        {
            var tp = GetTrainingPoint(skill);

            if (tp != null)
            {
                return new[] { tp.Name, tp.Description };
            }

            return new TextDefinition[] { SkillInfo.Table[(int)skill].Name, 1157522 };
        }

        public static int GetCategoryLocalization(object o) => o switch
        {
            MagicalAbility => 1157481,
            SpecialAbility => 1157480,
            AreaEffect => 1157482,
            WeaponAbility => 1157479,
            PetStat => 1114262,
            ResistanceType => 1154539,
            SkillName skill => IsCombatSkill(skill) ? 1157496 : 1157495,
            _ => 0,
        };

        private static bool IsCombatSkill(SkillName skill)
        {
            for (var i = 0; i < CombatSkills.Length; i++)
            {
                if (CombatSkills[i] == skill) return true;
            }

            return false;
        }

        #endregion

        public static void Initialize()
        {
            CommandSystem.Register("PetTrainTest", AccessLevel.GameMaster, e =>
            {
                var m = e.Mobile;
                var arg = e.GetString(0);
                if (string.IsNullOrEmpty(arg)) { m.SendMessage("PetTrainTest <type>"); return; }

                var t = AssemblyHandler.FindTypeByName(arg);
                if (t == null || !t.IsSubclassOf(typeof(BaseCreature))) return;
                if (Activator.CreateInstance(t) is not BaseCreature bc) return;

                if (!bc.Tamable) { m.SendMessage("That cannot be tamed!"); bc.Delete(); return; }
                if (m.Followers + bc.ControlSlots > m.FollowersMax)
                { m.SendLocalizedMessage(1049611); bc.Delete(); return; }

                bc.MoveToWorld(m.Location, m.Map);

                if (bc is GreaterDragon)
                {
                    AnimalTaming.ScaleSkills(bc, 0.72, 0.72);
                    bc.Skills[SkillName.Magery].Base = bc.Skills[SkillName.Magery].Cap;
                }
                else AnimalTaming.ScaleSkills(bc, 0.90, 0.90);

                Timer.DelayCall(TimeSpan.FromSeconds(.25), () =>
                {
                    bc.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502799, m.NetState);
                    bc.Owners.Add(m);
                    bc.SetControlMaster(m);
                    bc.IsBonded = true;
                    bc.OnAfterTame(m);
                    GetAbilityProfile(bc, true).OnTame();
                });
            });
        }
    }
}

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class PetTrainingGate : Item
    {
        public override bool ForceShowProperties => true;

        public override string DefaultName =>
            "Pet Training Gate - You Must Start Pet Training Before Bringing Your Pet Through The Gate!";

        [Constructible]
        public PetTrainingGate() : base(3948) { Hue = 1918; Movable = false; }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is BaseCreature bc)
            {
                var profile = PetTrainingHelper.GetTrainingProfile(bc);
                if (bc.Controlled && bc.ControlMaster?.InRange(bc.Location, 25) == true
                    && profile != null && profile.HasBegunTraining
                    && profile.TrainingProgress < profile.TrainingProgressMax)
                {
                    profile.TrainingProgress = profile.TrainingProgressMax;
                    bc.FixedEffect(0x375A, 10, 30);

                    if (bc.ControlMaster is PlayerMobile pm)
                    {
                        bc.PublicOverheadMessage(MessageType.Regular, 0x59, false,
                            $"*{bc.Name} surges with battle experience and is ready to train!*");
                        pm.FindGump<Server.Mobiles.DashboardAnimalLoreGump>()?.Refresh();
                    }
                }
            }
            return true;
        }
    }

    [SerializationGenerator(0, false)]
    public partial class PetBondRemoveGate : Item
    {
        public override bool ForceShowProperties => true;
        public override string DefaultName => "Pet Bond Timer Remover";

        [Constructible]
        public PetBondRemoveGate() : base(3948) { Hue = 1911; Movable = false; }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is BaseCreature bc && bc.Controlled
                && bc.ControlMaster?.InRange(bc.Location, 25) == true && !bc.IsBonded)
            {
                bc.IsBonded = true;
                bc.FixedEffect(0x375A, 10, 30);
            }
            return true;
        }
    }

    [SerializationGenerator(0, false)]
    public partial class PowerScrollGiver : Item
    {
        public override bool ForceShowProperties => true;
        public override string DefaultName => "+20 Power Scrolls";

        [Constructible]
        public PowerScrollGiver() : base(0x1183) { Hue = 2214; Movable = false; }

        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(Location, 3)) return;
            var bag = new Bag();
            foreach (var sk in PetTrainingHelper.MagicSkills) bag.DropItem(new PowerScroll(sk, 120));
            foreach (var sk in PetTrainingHelper.CombatSkills) bag.DropItem(new PowerScroll(sk, 120));
            m.AddToBackpack(bag);
        }
    }
}
