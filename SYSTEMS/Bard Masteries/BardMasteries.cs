using System;
using System.Collections.Generic;
using ModernUO.CodeGeneratedEvents;
using Server.Collections;
using Server.Engines.BuffIcons;
using Server.Engines.PartySystem;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.BardMasteries;

/// <summary>
/// Bard-only port of the ServUO Skill Mastery system. Handles the active mastery selection,
/// the shared attribute/damage hooks and the persistence of the selected masteries.
/// </summary>
public static class BardMasterySystem
{
    public const int BookOffset = 700;
    public const int MinSkillRequirement = 90;

    public static readonly SkillName[] MasterySkills =
    {
        SkillName.Provocation,
        SkillName.Peacemaking,
        SkillName.Discordance
    };

    private static readonly Dictionary<Mobile, SkillName> _activeMasteries = new();

    // Retained so the persistence stays registered with the world save system.
    private static BardMasteryPersistence Persistence { get; set; }

    public static void Configure() => Persistence ??= new BardMasteryPersistence();

    public static bool IsBardMasterySkill(SkillName skill) =>
        skill is SkillName.Provocation or SkillName.Peacemaking or SkillName.Discordance;

    public static SkillName GetActiveMastery(Mobile m)
    {
        if (m == null)
        {
            return SkillName.Alchemy;
        }

        if (_activeMasteries.TryGetValue(m, out var active))
        {
            return active;
        }

        return GetDefaultMastery(m);
    }

    private static SkillName GetDefaultMastery(Mobile m)
    {
        var best = SkillName.Alchemy;
        var bestValue = 0.0;

        for (var i = 0; i < MasterySkills.Length; ++i)
        {
            var skill = MasterySkills[i];
            var value = m.Skills[skill].Base;

            if (value < MinSkillRequirement || value <= bestValue)
            {
                continue;
            }

            bestValue = value;
            best = skill;
        }

        return best;
    }

    public static void SetActiveMastery(Mobile m, SkillName skill)
    {
        if (m == null)
        {
            return;
        }

        if (skill != SkillName.Alchemy && (!IsBardMasterySkill(skill) || GetMasteryLevel(m, skill) == 0))
        {
            return;
        }

        _activeMasteries[m] = skill;
    }

    public static int GetMasteryLevel(Mobile m, SkillName skill)
    {
        if (m == null || !IsBardMasterySkill(skill))
        {
            return 0;
        }

        var value = m.Skills[skill].Base;

        if (value >= 110.0)
        {
            return 3;
        }

        if (value >= 100.0)
        {
            return 2;
        }

        return value >= MinSkillRequirement ? 1 : 0;
    }

    public static bool HasLearned(Mobile m, SkillName skill) => GetMasteryLevel(m, skill) > 0;

    public static int GetAttributeBonus(Mobile m, AosAttribute attribute) => BardSpell.GetAttributeBonus(m, attribute);

    public static int GetHitsBonus(Mobile m) =>
        BardSpell.GetSpellForParty(m, typeof(InvigorateSpell)) is InvigorateSpell invigorate ? invigorate.StatBonus() : 0;

    public static void OnDamage(Mobile victim, Mobile damager, ref int damage) =>
        BardSpell.OnDamage(victim, damager, ref damage);

    public static void OnMasteryChanged(Mobile m) => BardSpell.OnMasteryChanged(m);

    [OnEvent(nameof(PlayerMobile.PlayerDeletedEvent))]
    public static void OnPlayerDeleted(Mobile m) => _activeMasteries.Remove(m);

    public static int GetLocalization(SkillName skill) => skill switch
    {
        SkillName.Provocation => 1151946,
        SkillName.Peacemaking => 1151947,
        SkillName.Discordance => 1151945,
        _                     => 0
    };

    private sealed class BardMasteryPersistence : GenericPersistence
    {
        public BardMasteryPersistence() : base("BardMasteries", 10)
        {
        }

        public override void Serialize(IGenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            var count = 0;

            foreach (var (mobile, _) in _activeMasteries)
            {
                if (mobile?.Deleted == false)
                {
                    ++count;
                }
            }

            writer.WriteEncodedInt(count);

            foreach (var (mobile, skill) in _activeMasteries)
            {
                if (mobile?.Deleted != false)
                {
                    continue;
                }

                writer.Write(mobile);
                writer.Write((byte)skill);
            }
        }

        public override void Deserialize(IGenericReader reader)
        {
            reader.ReadEncodedInt(); // version

            var count = reader.ReadEncodedInt();

            for (var i = 0; i < count; ++i)
            {
                var mobile = reader.ReadEntity<Mobile>();
                var skill = (SkillName)reader.ReadByte();

                // SkillName.Alchemy is the explicit "no active mastery" selection; keep it.
                if (mobile != null && (skill == SkillName.Alchemy || IsBardMasterySkill(skill)))
                {
                    _activeMasteries[mobile] = skill;
                }
            }
        }
    }
}

/// <summary>
/// Shared implementation of the ServUO SkillMasterySpell/BardSpell pair, adapted to ModernUO.
/// </summary>
public abstract class BardSpell : Spell
{
    private static readonly Dictionary<Mobile, List<BardSpell>> _table = new();

    private BaseInstrument _instrument;
    private TimerExecutionToken _upkeepToken;

    protected BardSpell(Mobile caster, Item scroll, SpellInfo info) : base(caster, scroll, info)
    {
    }

    public Mobile Target { get; set; }

    public List<Mobile> PartyList { get; private set; }

    public DateTime Expires { get; set; }

    public BaseInstrument Instrument => _instrument;

    public virtual double RequiredSkill => 90.0;

    public virtual double UpKeep => 0.0;

    public virtual int RequiredMana => 0;

    public virtual bool PartyEffects => false;

    public virtual int DamageThreshold => 45;

    public virtual bool DamageCanDisrupt => true;

    public virtual double TickTime => 2.0;

    public virtual int PartyRange => 12;

    public virtual TimeSpan ExpirationPeriod => TimeSpan.FromMinutes(30.0);

    public virtual bool RevealOnTick => true;

    public virtual int UpkeepCancelMessage => 1115665; // You do not have enough mana to continue infusing your song with magic.

    public virtual int OutOfRangeMessage => 1115771; // Your target is no longer in range of your spellsong.

    public virtual int DisruptMessage => 1115710; // Your spell song has been interrupted.

    public virtual double SlayerBonus => 1.5;

    public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.25);

    public override bool ClearHandsOnCast => false;

    public override bool BlocksMovement => true;

    public override SkillName DamageSkill => SkillName.Musicianship;

    public virtual double BaseSkillBonus
    {
        get
        {
            if (Caster == null)
            {
                return 0.0;
            }

            return Math.Floor(
                2 + (Caster.Skills[CastSkill].Base - 90) / 10 + (Caster.Skills[DamageSkill].Base - 90) / 10
            );
        }
    }

    public virtual double CollectiveBonus
    {
        get
        {
            var bonus = 0.0;

            var prov = Caster.Skills[SkillName.Provocation].Base;
            var peac = Caster.Skills[SkillName.Peacemaking].Base;
            var disc = Caster.Skills[SkillName.Discordance].Base;

            switch (CastSkill)
            {
                default:
                    return 0.0;
                case SkillName.Provocation:
                    {
                        if (peac >= 100)
                        {
                            bonus += 1 + (peac - 100) / 10;
                        }

                        if (disc >= 100)
                        {
                            bonus += 1 + (disc - 100) / 10;
                        }

                        break;
                    }
                case SkillName.Peacemaking:
                    {
                        if (prov >= 100)
                        {
                            bonus += 1 + (peac - 100) / 10;
                        }

                        if (disc >= 100)
                        {
                            bonus += 1 + (disc - 100) / 10;
                        }

                        break;
                    }
                case SkillName.Discordance:
                    {
                        if (prov >= 100)
                        {
                            bonus += 1 + (peac - 100) / 10;
                        }

                        if (peac >= 100)
                        {
                            bonus += 1 + (disc - 100) / 10;
                        }

                        break;
                    }
            }

            return bonus;
        }
    }

    public override bool CheckCast()
    {
        if (!base.CheckCast())
        {
            return false;
        }

        var mana = ScaleMana(RequiredMana);

        if (Caster.Player && Caster.Skills[CastSkill].Value < RequiredSkill)
        {
            Caster.SendLocalizedMessage(1115709); // Your skills are not high enough to invoke this mastery ability.
            return false;
        }

        if (Caster is PlayerMobile && BardMasterySystem.GetActiveMastery(Caster) != CastSkill)
        {
            Caster.SendLocalizedMessage(1115664); // You are not on the correct path for using this mastery ability.
            return false;
        }

        if (Caster is PlayerMobile && !BardMasterySystem.HasLearned(Caster, CastSkill))
        {
            Caster.SendLocalizedMessage(1115664); // You are not on the correct path for using this mastery ability.
            return false;
        }

        if (Caster.Mana < mana)
        {
            Caster.SendLocalizedMessage(1060174, mana.ToString()); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
            return false;
        }

        _instrument = BaseInstrument.GetInstrument(Caster);

        if (_instrument == null)
        {
            BaseInstrument.PickInstrument(Caster, OnPickedInstrument);
            return false;
        }

        return true;
    }

    private void OnPickedInstrument(Mobile from, BaseInstrument instrument) =>
        from.SendMessage("You choose a musical instrument. Try using the bard mastery again.");

    public override bool CheckFizzle()
    {
        var check = base.CheckFizzle();

        if (check)
        {
            if (_instrument != null)
            {
                _instrument.PlayInstrumentWell(Caster);
                _instrument.ConsumeUse(Caster);
            }
        }
        else
        {
            Caster.SendLocalizedMessage(500612); // You play poorly, and there is no effect.

            _instrument?.PlayInstrumentBadly(Caster);
        }

        return check;
    }

    public override void OnBeginCast()
    {
        base.OnBeginCast();
        SendCastEffect();
    }

    public virtual void SendCastEffect() =>
        Caster.FixedEffect(0x37C4, 10, (int)(GetCastDelay().TotalSeconds * 28), 4, 3);

    public override void GetCastSkills(out double min, out double max)
    {
        min = RequiredSkill;
        max = RequiredSkill + 25.0;
    }

    public override int GetMana() => RequiredMana;

    public int GetMasteryLevel() => BardMasterySystem.GetMasteryLevel(Caster, CastSkill);

    public virtual int ScaleUpkeep()
    {
        if (UpKeep == 0.0)
        {
            return 0;
        }

        var mod = CollectiveBonus;
        var upkeep = UpKeep;
        var mana = (int)(upkeep - upkeep * mod / 4.5);

        // ServUO's formula can go negative with a high collective bonus; upkeep must never refund mana.
        if (mana < 0)
        {
            mana = 0;
        }

        return ScaleMana(mana);
    }

    protected virtual void BeginTimer()
    {
        StopTimer();

        var interval = TimeSpan.FromSeconds(TickTime);
        Timer.StartTimer(interval, interval, OnUpkeepTick, out _upkeepToken);

        if (Expires < Core.Now)
        {
            Expires = Core.Now + ExpirationPeriod;
        }

        AddToTable();
        AddStatMods();

        if (RevealOnTick)
        {
            Caster.RevealingAction();
        }

        Caster.Delta(MobileDelta.WeaponDamage);

        Target?.Delta(MobileDelta.WeaponDamage);

        if (PartyList != null)
        {
            for (var i = 0; i < PartyList.Count; ++i)
            {
                PartyList[i].Delta(MobileDelta.WeaponDamage);
            }
        }
    }

    private void OnUpkeepTick()
    {
        if (!OnTick())
        {
            StopTimer();
        }
    }

    private void StopTimer()
    {
        if (_upkeepToken.Running)
        {
            _upkeepToken.Cancel();
        }
    }

    public virtual bool OnTick()
    {
        UpdateParty();

        if (RevealOnTick)
        {
            Caster.RevealingAction();
        }

        var upkeep = ScaleUpkeep();

        if ((Caster is PlayerMobile && Caster.NetState == null) || Expires < Core.Now || !Caster.Alive ||
            Caster.IsDeadBondedPet)
        {
            Expire();
        }
        else if (Target != null && !Target.Alive)
        {
            Expire();
        }
        else if (Target != null && !Caster.InRange(Target.Location, PartyRange))
        {
            Expire();
            Caster.SendLocalizedMessage(OutOfRangeMessage);
        }
        else if (Caster.Mana < upkeep)
        {
            Caster.SendLocalizedMessage(UpkeepCancelMessage);
            Expire();
        }
        else if (Caster.Player && Caster.Skills[CastSkill].Value < RequiredSkill)
        {
            Expire();
        }
        else
        {
            DoEffects();
            Caster.Mana -= upkeep;
            return true;
        }

        return false;
    }

    protected virtual void DoEffects()
    {
    }

    public virtual int PropertyBonus() => 0;

    public virtual int PropertyBonus2() => 0;

    /// <summary>Used for hit point, stamina and mana bonuses.</summary>
    public virtual int StatBonus() => 0;

    public virtual int DamageBonus() => 0;

    public virtual void AddStatMods()
    {
    }

    public virtual void RemoveStatMods()
    {
    }

    public virtual void AddPartyEffects(Mobile m)
    {
    }

    public virtual void RemovePartyEffects(Mobile m)
    {
    }

    public virtual void EndEffects()
    {
        if (PartyList != null)
        {
            for (var i = 0; i < PartyList.Count; ++i)
            {
                RemovePartyEffects(PartyList[i]);
            }
        }

        RemovePartyEffects(Caster);
    }

    public virtual void OnTargetDamaged(Mobile attacker, Mobile victim, ref int damage)
    {
    }

    public virtual void OnExpire()
    {
    }

    public virtual void Expire(bool disrupt = false)
    {
        StopTimer();

        if (disrupt)
        {
            Caster.SendLocalizedMessage(DisruptMessage);
        }

        RemoveFromTable();
        RemoveStatMods();
        EndEffects();

        Caster.Delta(MobileDelta.WeaponDamage);
        Target?.Delta(MobileDelta.WeaponDamage);

        if (PartyList != null)
        {
            for (var i = 0; i < PartyList.Count; ++i)
            {
                PartyList[i].Delta(MobileDelta.WeaponDamage);
            }

            PartyList.Clear();
            PartyList = null;
        }

        OnExpire();
    }

    public virtual double DamageModifier(Mobile victim)
    {
        var dSkill = Caster.Skills[DamageSkill].Value;
        var vSkill = GetResistSkill(victim);

        var reduce = 1.0 - (dSkill - vSkill) / dSkill;

        if (reduce < 0.0)
        {
            reduce = 0.0;
        }

        if (reduce > 1.0)
        {
            reduce = 1.0;
        }

        return reduce;
    }

    public virtual bool CheckResisted(Mobile target)
    {
        var volumeMod = GetMasteryLevel() * 2;
        var n = GetResistPercent(target, volumeMod) / 100.0;

        if (n <= 0.0)
        {
            return false;
        }

        if (n >= 1.0)
        {
            return true;
        }

        var maxSkill = (1 + volumeMod) * 10;
        maxSkill += (1 + volumeMod / 6) * 25;

        if (target.Skills[SkillName.MagicResist].Value < maxSkill)
        {
            target.CheckSkill(SkillName.MagicResist, 0.0, target.Skills[SkillName.MagicResist].Cap);
        }

        return n >= Utility.RandomDouble();
    }

    public virtual double GetResistPercent(Mobile target, int level)
    {
        double value = GetResistSkill(target);
        var firstPercent = value / 5.0;
        var secondPercent = value - ((Caster.Skills[CastSkill].Value - 20.0) / 5.0 + (1 + level) * 5.0);

        return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0;
    }

    public virtual double GetSlayerBonus()
    {
        if (Target == null || Instrument is not ISlayer slayer)
        {
            return 1.0;
        }

        var entry1 = SlayerGroup.GetEntryByName(slayer.Slayer);
        var entry2 = SlayerGroup.GetEntryByName(slayer.Slayer2);

        return (entry1?.Slays(Target) == true || entry2?.Slays(Target) == true) ? SlayerBonus : 1.0;
    }

    public void UpdateParty() => UpdateParty(false);

    public void UpdateParty(bool playersOnly)
    {
        var party = Party.Get(Caster);

        PartyList ??= new List<Mobile>();

        if (party == null)
        {
            // Party disbanded: drop every non-caster entry except the caster's own followers.
            for (var i = PartyList.Count - 1; i >= 0; --i)
            {
                if (i >= PartyList.Count)
                {
                    continue;
                }

                var m = PartyList[i];

                if (m != Caster && !IsFollowerOf(Caster, m))
                {
                    RemovePartyMember(m);
                }
            }

            if (!PartyList.Contains(Caster))
            {
                AddPartyMember(Caster);
            }

            return;
        }

        var members = party.Members;

        for (var i = 0; i < members.Count; ++i)
        {
            var m = members[i].Mobile;

            if (m == null)
            {
                continue;
            }

            if (!PartyList.Contains(m))
            {
                if ((!playersOnly || m is PlayerMobile) && ValidPartyMember(m))
                {
                    AddPartyMember(m);
                }
            }
            else if (!ValidPartyMember(m))
            {
                RemovePartyMember(m);
            }
            else
            {
                AddMissingPets(m);
            }
        }

        // Members who left or were removed from the party are no longer visited by the loop above,
        // so drop any stale entries (and their followers) here.
        for (var i = PartyList.Count - 1; i >= 0; --i)
        {
            if (i >= PartyList.Count)
            {
                continue;
            }

            var m = PartyList[i];

            if (m == Caster || IsCurrentPartyMember(party, m) || IsFollowerOfPartyMember(party, m))
            {
                continue;
            }

            RemovePartyMember(m);
        }
    }

    public virtual bool CheckPartyEffects(Mobile m, bool beneficial = false)
    {
        if (m == Caster)
        {
            return true;
        }

        if (Caster.IsBeneficialCriminal(m))
        {
            var casterNoto = Notoriety.Compute(Caster, m);
            var mNoto = Notoriety.Compute(m, Caster);

            if (casterNoto == Notoriety.Enemy || casterNoto != mNoto)
            {
                return false;
            }
        }

        if (beneficial)
        {
            Caster.DoBeneficial(m);
        }

        return true;
    }

    protected virtual bool ValidPartyMember(Mobile m) =>
        m.Alive && m.Map == Caster.Map && m.InRange(Caster, PartyRange) && CheckPartyEffects(m);

    protected void AddPartyMember(Mobile m)
    {
        PartyList.Add(m);
        AddPartyEffects(m);

        if (m is PlayerMobile pm && pm.AllFollowers != null)
        {
            foreach (var pet in pm.AllFollowers)
            {
                if (!PartyList.Contains(pet) && ValidPartyMember(pet))
                {
                    AddPartyMember(pet);
                }
            }
        }
    }

    protected void RemovePartyMember(Mobile m)
    {
        PartyList.Remove(m);
        RemovePartyEffects(m);

        if (m is PlayerMobile pm && pm.AllFollowers != null)
        {
            foreach (var pet in pm.AllFollowers)
            {
                if (PartyList.Contains(pet))
                {
                    RemovePartyMember(pet);
                }
            }
        }
    }

    private static bool IsCurrentPartyMember(Party party, Mobile m)
    {
        var members = party.Members;

        for (var i = 0; i < members.Count; ++i)
        {
            if (members[i].Mobile == m)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsFollowerOf(Mobile owner, Mobile m) =>
        owner is PlayerMobile pm && pm.AllFollowers?.Contains(m) == true;

    private static bool IsFollowerOfPartyMember(Party party, Mobile m)
    {
        var members = party.Members;

        for (var i = 0; i < members.Count; ++i)
        {
            if (IsFollowerOf(members[i].Mobile, m))
            {
                return true;
            }
        }

        return false;
    }

    private void AddMissingPets(Mobile m)
    {
        if (m is not PlayerMobile pm || pm.AllFollowers == null)
        {
            return;
        }

        foreach (var pet in pm.AllFollowers)
        {
            if (!PartyList.Contains(pet) && ValidPartyMember(pet))
            {
                AddPartyMember(pet);
            }
        }
    }

    private void AddToTable()
    {
        if (!_table.TryGetValue(Caster, out var list))
        {
            _table[Caster] = list = new List<BardSpell>();
        }

        if (!list.Contains(this))
        {
            list.Add(this);
        }
    }

    private void RemoveFromTable()
    {
        if (Caster != null && _table.TryGetValue(Caster, out var list))
        {
            list.Remove(this);

            if (list.Count == 0)
            {
                _table.Remove(Caster);
            }
        }
    }

    public static BardSpell GetSpell(Mobile from, Type type)
    {
        if (from == null || !_table.TryGetValue(from, out var list))
        {
            return null;
        }

        for (var i = 0; i < list.Count; ++i)
        {
            if (list[i]?.GetType() == type)
            {
                return list[i];
            }
        }

        return null;
    }

    public static BardSpell GetSpellForTarget(Mobile target, Type type)
    {
        if (target == null)
        {
            return null;
        }

        foreach (var list in _table.Values)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                if (list[i]?.Target == target && list[i].GetType() == type)
                {
                    return list[i];
                }
            }
        }

        return null;
    }

    public static BardSpell GetSpellForParty(Mobile from, Type type)
    {
        if (from == null)
        {
            return null;
        }

        var check = from;

        // ServUO follows both controlled and summoned creatures back to their master.
        if (from is BaseCreature creature && (creature.Controlled || creature.Summoned) &&
            creature.GetMaster() is { } master)
        {
            check = master;
        }

        var spell = GetSpell(check, type);

        if (spell != null)
        {
            return spell;
        }

        var party = Party.Get(check);

        if (party == null)
        {
            return null;
        }

        var members = party.Members;

        for (var i = 0; i < members.Count; ++i)
        {
            var member = members[i].Mobile;

            if (member == null)
            {
                continue;
            }

            spell = GetSpell(member, type);

            if (spell?.PartyEffects == true && from.InRange(member.Location, spell.PartyRange) &&
                spell.CheckPartyEffects(member))
            {
                return spell;
            }
        }

        return null;
    }

    public static bool HasHarmfulEffects(Mobile target, Type type) => GetSpellForTarget(target, type) != null;

    public static bool HasHarmfulEffects(Mobile target)
    {
        if (target == null)
        {
            return false;
        }

        foreach (var list in _table.Values)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                if (list[i]?.Target == target)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void OnMasteryChanged(Mobile m)
    {
        if (m == null || !_table.TryGetValue(m, out var list))
        {
            return;
        }

        for (var i = list.Count - 1; i >= 0; --i)
        {
            if (i < list.Count)
            {
                list[i].Expire();
            }
        }
    }

    public static void OnDamage(Mobile victim, Mobile damager, ref int damage)
    {
        if (victim?.Deleted != false || damager?.Deleted != false || damage <= 0)
        {
            return;
        }

        // Damage can interrupt the victim's own spellsong.
        if (_table.TryGetValue(victim, out var victimSpells))
        {
            for (var i = victimSpells.Count - 1; i >= 0; --i)
            {
                if (i >= victimSpells.Count)
                {
                    continue;
                }

                var spell = victimSpells[i];

                if (spell.DamageCanDisrupt && damage > spell.DamageThreshold)
                {
                    spell.Expire(true);
                }
            }
        }

        if (GetSpellForParty(victim, typeof(PerseveranceSpell)) is PerseveranceSpell perseverance)
        {
            perseverance.AbsorbDamage(ref damage);
        }

        if (GetSpellForParty(damager, typeof(InspireSpell)) is InspireSpell inspire)
        {
            inspire.DoDamage(ref damage);
        }

        // Collect the victim's target effects first: a Tribulation burst re-enters AOS.Damage and can
        // mutate the registry, so the hooks must not run while _table is being enumerated.
        // PooledRefList keeps the snapshot off the heap.
        using var targetSpells = new PooledRefList<BardSpell>(4);

        foreach (var list in _table.Values)
        {
            for (var i = list.Count - 1; i >= 0; --i)
            {
                var spell = list[i];

                if (spell?.Target == victim)
                {
                    targetSpells.Add(spell);
                }
            }
        }

        for (var i = 0; i < targetSpells.Count; ++i)
        {
            targetSpells[i].OnTargetDamaged(damager, victim, ref damage);
        }

        if (damage < 0)
        {
            damage = 0;
        }
    }

    public static int GetAttributeBonus(Mobile m, AosAttribute attribute)
    {
        if (m?.Deleted != false)
        {
            return 0;
        }

        var value = 0;

        switch (attribute)
        {
            case AosAttribute.AttackChance:
                {
                    if (GetSpellForParty(m, typeof(InspireSpell)) is InspireSpell inspire)
                    {
                        value += inspire.PropertyBonus();
                    }

                    if (GetSpellForTarget(m, typeof(TribulationSpell)) is TribulationSpell tribulation)
                    {
                        value -= tribulation.PropertyBonus();
                    }

                    break;
                }
            case AosAttribute.DefendChance:
                {
                    if (GetSpellForParty(m, typeof(PerseveranceSpell)) is PerseveranceSpell perseverance)
                    {
                        value += perseverance.PropertyBonus();
                    }

                    break;
                }
            case AosAttribute.RegenHits:
            case AosAttribute.RegenStam:
            case AosAttribute.RegenMana:
                {
                    if (GetSpellForParty(m, typeof(ResilienceSpell)) is ResilienceSpell resilience)
                    {
                        value += resilience.PropertyBonus();
                    }

                    break;
                }
            case AosAttribute.WeaponDamage:
                {
                    if (GetSpellForParty(m, typeof(InspireSpell)) is InspireSpell inspire)
                    {
                        value += inspire.DamageBonus();
                    }

                    break;
                }
            case AosAttribute.SpellDamage:
                {
                    if (GetSpellForParty(m, typeof(InspireSpell)) is InspireSpell inspire)
                    {
                        value += inspire.PropertyBonus();
                    }

                    if (GetSpellForTarget(m, typeof(TribulationSpell)) is TribulationSpell tribulation)
                    {
                        value -= tribulation.PropertyBonus();
                    }

                    break;
                }
        }

        return value;
    }
}

public class InspireSpell : BardSpell
{
    private static readonly SpellInfo _info = new("Inspire", "Unus Por", -1, 9002);

    private int _propertyBonus;
    private int _damageBonus;
    private int _damageModifier;

    public InspireSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
    {
    }

    public override SkillName CastSkill => SkillName.Provocation;

    public override double RequiredSkill => 90.0;

    public override double UpKeep => 4.0;

    public override int RequiredMana => 16;

    public override bool PartyEffects => true;

    public override void OnCast()
    {
        var existing = GetSpell(Caster, GetType());

        if (existing != null)
        {
            existing.Expire();
            Caster.SendLocalizedMessage(1115774); // You halt your spellsong.
        }
        else if (CheckSequence())
        {
            _propertyBonus = (int)(BaseSkillBonus * 2 + CollectiveBonus);
            _damageBonus = (int)(BaseSkillBonus * 5 + CollectiveBonus * 3);
            _damageModifier = (int)(BaseSkillBonus + 1 + CollectiveBonus);

            UpdateParty();
            BeginTimer();
        }

        FinishSequence();
    }

    public override void AddPartyEffects(Mobile m)
    {
        m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
        m.SendLocalizedMessage(1115736); // You feel inspired by the bard's spellsong.

        if (m is PlayerMobile pm)
        {
            var args = $"{_propertyBonus}\t{_propertyBonus}\t{_damageBonus}\t{_damageModifier}";
            pm.AddBuff(new BuffInfo(BuffIcon.Inspire, 1115683, 1151951, args: args));
        }
    }

    public override void RemovePartyEffects(Mobile m)
    {
        if (m is PlayerMobile pm)
        {
            pm.RemoveBuff(BuffIcon.Inspire);
        }
    }

    /// <summary>Hit chance and spell damage bonus.</summary>
    public override int PropertyBonus() => _propertyBonus;

    /// <summary>Weapon damage bonus.</summary>
    public override int DamageBonus() => _damageBonus;

    public void DoDamage(ref int damageTaken) => damageTaken += AOS.Scale(damageTaken, _damageModifier);
}

public class InvigorateSpell : BardSpell
{
    private static readonly SpellInfo _info = new("Invigorate", "An Zu", -1, 9002);

    private readonly List<Mobile> _statMods = new();

    private DateTime _nextHeal;
    private int _hitPointBonus;
    private int _statBonus;

    public InvigorateSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info) =>
        _nextHeal = Core.Now + TimeSpan.FromSeconds(4.0);

    public override SkillName CastSkill => SkillName.Provocation;

    public override double RequiredSkill => 90.0;

    public override double UpKeep => 5.0;

    public override int RequiredMana => 22;

    public override bool PartyEffects => true;

    private string StatModPrefix => $"Invigorate{Caster.Serial}";

    public override void OnCast()
    {
        var existing = GetSpell(Caster, GetType());

        if (existing != null)
        {
            existing.Expire();
            Caster.SendLocalizedMessage(1115774); // You halt your spellsong.
        }
        else if (CheckSequence())
        {
            _statBonus = (int)(BaseSkillBonus + CollectiveBonus);
            _hitPointBonus = (int)(2.5 * BaseSkillBonus + CollectiveBonus);

            UpdateParty();
            BeginTimer();
        }

        FinishSequence();
    }

    public override void AddPartyEffects(Mobile m)
    {
        m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
        m.SendLocalizedMessage(1115737); // You feel invigorated by the bard's spellsong.

        if (m is PlayerMobile pm)
        {
            var args = $"{_hitPointBonus}\t{_statBonus}\t{_statBonus}\t{_statBonus}";
            pm.AddBuff(new BuffInfo(BuffIcon.Invigorate, 1115613, 1115730, args: args));
        }

        m.AddStatMod(new StatMod(StatType.Str, StatModPrefix + "Str", _statBonus, TimeSpan.Zero));
        m.AddStatMod(new StatMod(StatType.Dex, StatModPrefix + "Dex", _statBonus, TimeSpan.Zero));
        m.AddStatMod(new StatMod(StatType.Int, StatModPrefix + "Int", _statBonus, TimeSpan.Zero));

        _statMods.Add(m);

        if (!m.Deleted)
        {
            m.Delta(MobileDelta.Hits);
        }
    }

    public override void RemovePartyEffects(Mobile m)
    {
        if (m == null)
        {
            return;
        }

        if (m is PlayerMobile pm)
        {
            pm.RemoveBuff(BuffIcon.Invigorate);
        }

        if (_statMods.Contains(m))
        {
            m.RemoveStatMod(StatModPrefix + "Str");
            m.RemoveStatMod(StatModPrefix + "Dex");
            m.RemoveStatMod(StatModPrefix + "Int");

            _statMods.Remove(m);
        }

        if (!m.Deleted)
        {
            m.Delta(MobileDelta.Hits);
        }
    }

    public override bool OnTick()
    {
        var tick = base.OnTick();

        if (_nextHeal > Core.Now || PartyList == null)
        {
            return tick;
        }

        for (var i = PartyList.Count - 1; i >= 0; --i)
        {
            if (i >= PartyList.Count)
            {
                continue;
            }

            var m = PartyList[i];

            if (CheckPartyEffects(m, true))
            {
                var healRange = (int)(BaseSkillBonus * 2 + CollectiveBonus);

                if (m.Hits < m.HitsMax)
                {
                    m.Heal(Utility.RandomMinMax(healRange - 2, healRange + 2));
                    m.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                    m.PlaySound(0x1F2);
                }
            }
            else
            {
                RemovePartyMember(m);
            }
        }

        _nextHeal = Core.Now + TimeSpan.FromSeconds(4.0);
        return tick;
    }

    /// <summary>Hit point bonus applied through PlayerMobile.HitsMax.</summary>
    public override int StatBonus() => _hitPointBonus;
}

public class ResilienceSpell : BardSpell
{
    private static readonly SpellInfo _info = new("Resilience", "Kal Mani Tym", -1, 9002);

    private int _propertyBonus;

    public ResilienceSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
    {
    }

    public override SkillName CastSkill => SkillName.Peacemaking;

    public override double RequiredSkill => 90.0;

    public override double UpKeep => 4.0;

    public override int RequiredMana => 16;

    public override bool PartyEffects => true;

    public override void OnCast()
    {
        var existing = GetSpell(Caster, GetType());

        if (existing != null)
        {
            existing.Expire();
            Caster.SendLocalizedMessage(1115774); // You halt your spellsong.
        }
        else if (CheckSequence())
        {
            _propertyBonus = (int)(BaseSkillBonus * 2 + CollectiveBonus);

            UpdateParty();
            BeginTimer();
        }

        FinishSequence();
    }

    public override void AddPartyEffects(Mobile m)
    {
        m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
        m.SendLocalizedMessage(1115738); // The bard's spellsong fills you with a feeling of resilience.

        if (m is PlayerMobile pm)
        {
            var args = $"{_propertyBonus}\t{_propertyBonus}\t{_propertyBonus}";
            pm.AddBuff(new BuffInfo(BuffIcon.Resilience, 1115614, 1115731, args: args));
        }

        m.CheckStatTimers();
    }

    public override void RemovePartyEffects(Mobile m)
    {
        if (m is PlayerMobile pm)
        {
            pm.RemoveBuff(BuffIcon.Resilience);
        }

        m.CheckStatTimers();
    }

    /// <summary>Hit point, stamina and mana regeneration bonus.</summary>
    public override int PropertyBonus() => _propertyBonus;

    public static bool UnderEffects(Mobile m) => GetSpellForParty(m, typeof(ResilienceSpell)) != null;
}

public class PerseveranceSpell : BardSpell
{
    private static readonly SpellInfo _info = new("Perseverance", "Unus Jux Sanct", -1, 9002);

    private int _propertyBonus;
    private int _castingFocusBonus;
    private int _damageModifier;

    public PerseveranceSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
    {
    }

    public override SkillName CastSkill => SkillName.Peacemaking;

    public override double RequiredSkill => 90.0;

    public override double UpKeep => 5.0;

    public override int RequiredMana => 18;

    public override bool PartyEffects => true;

    public override void OnCast()
    {
        var existing = GetSpell(Caster, GetType());

        if (existing != null)
        {
            existing.Expire();
            Caster.SendLocalizedMessage(1115774); // You halt your spellsong.
        }
        else if (CheckSequence())
        {
            _propertyBonus = (int)(BaseSkillBonus * 3 + CollectiveBonus);
            // ModernUO has no SAAbsorptionAttributes/Casting Focus; the value is only used for the buff text.
            _castingFocusBonus = (int)(BaseSkillBonus / 2 + CollectiveBonus / 3);
            _damageModifier = (int)(BaseSkillBonus * 3 + CollectiveBonus);

            UpdateParty();
            BeginTimer();
        }

        FinishSequence();
    }

    public override void AddPartyEffects(Mobile m)
    {
        m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
        m.SendLocalizedMessage(1115739); // The bard's spellsong fills you with a feeling of invincibility.

        if (m is PlayerMobile pm)
        {
            var args = $"{_propertyBonus}\t-{_damageModifier}\t{_castingFocusBonus}";
            pm.AddBuff(new BuffInfo(BuffIcon.Perseverance, 1115615, 1115732, args: args));
        }
    }

    public override void RemovePartyEffects(Mobile m)
    {
        if (m is PlayerMobile pm)
        {
            pm.RemoveBuff(BuffIcon.Perseverance);
        }
    }

    /// <summary>Defense chance bonus.</summary>
    public override int PropertyBonus() => _propertyBonus;

    /// <summary>Casting focus bonus (buff text only in ModernUO).</summary>
    public override int PropertyBonus2() => _castingFocusBonus;

    /// <summary>Reduces the incoming damage dealt to the party.</summary>
    public void AbsorbDamage(ref int damage) => damage -= AOS.Scale(damage, _damageModifier);
}

public class TribulationSpell : BardSpell, ITargetingSpell<Mobile>
{
    private static readonly SpellInfo _info = new("Tribulation", "In Jux Hur Rel", -1, 9002);

    private DateTime _nextDamage;
    private int _propertyBonus;
    private double _damageChance;
    private int _damageFactor;
    private int _rounds;

    public TribulationSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info) =>
        _nextDamage = Core.Now;

    public override SkillName CastSkill => SkillName.Discordance;

    public override double RequiredSkill => 90.0;

    public override double UpKeep => 10.0;

    public override int RequiredMana => 24;

    public override bool PartyEffects => false;

    public override void OnCast()
    {
        var existing = GetSpell(Caster, GetType());

        if (existing != null)
        {
            existing.Expire();
            Caster.SendLocalizedMessage(1115774); // You halt your spellsong.
        }
        else
        {
            Caster.Target = new SpellTarget<Mobile>(this, TargetFlags.Harmful);
        }
    }

    void ITargetingSpell<Mobile>.Target(Mobile target) => OnTarget(target);

    private void OnTarget(Mobile m)
    {
        if (m == null)
        {
            return;
        }

        if (!Caster.CanSee(m))
        {
            Caster.SendLocalizedMessage(500237); // Target can not be seen.
        }
        else if (Caster == m)
        {
            Caster.SendMessage("You cannot target yourself!");
        }
        else if (HasHarmfulEffects(m, GetType()))
        {
            Caster.SendLocalizedMessage(1115772); // Your target is already under the effect of this spellsong.
        }
        else if (CheckHSequence(m))
        {
            SpellHelper.Turn(Caster, m);

            Target = m;

            HarmfulSpell(m);

            m.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);

            _propertyBonus = (int)(BaseSkillBonus * 2.75 + CollectiveBonus * 1.667);
            _damageChance = (int)(BaseSkillBonus * 7.5 + CollectiveBonus * 3);
            _damageFactor = (int)(BaseSkillBonus * 4 + CollectiveBonus * 3);
            _rounds = 5 + (int)(BaseSkillBonus * 0.75 + CollectiveBonus / 2);

            if (m is PlayerMobile targetPm)
            {
                var targetArgs = $"{_propertyBonus}\t{_propertyBonus}\t{(int)_damageChance}";
                targetPm.AddBuff(new BuffInfo(BuffIcon.TribulationTarget, 1115740, 1115742, args: targetArgs));
            }

            if (Caster is PlayerMobile casterPm)
            {
                var casterArgs = $"{m.Name}\t{_damageFactor}\t{(int)_damageChance}";
                casterPm.AddBuff(new BuffInfo(BuffIcon.TribulationCaster, 1115740, 1151388, args: casterArgs));
            }

            BeginTimer();
        }
    }

    public override bool OnTick()
    {
        if (Target?.Alive == true && Target.Map != null)
        {
            Target.FixedEffect(0x376A, 1, 32);
        }

        // ServUO post-decrements here, so the song lasts the full round count.
        if (_rounds-- <= 0)
        {
            Expire();
            return false;
        }

        return base.OnTick();
    }

    public override void EndEffects()
    {
        if (Target is PlayerMobile targetPm)
        {
            targetPm.RemoveBuff(BuffIcon.TribulationTarget);
        }

        if (Caster is PlayerMobile casterPm)
        {
            casterPm.RemoveBuff(BuffIcon.TribulationCaster);
        }

        base.EndEffects();
    }

    public override void OnTargetDamaged(Mobile attacker, Mobile victim, ref int damage)
    {
        if (_nextDamage > Core.Now)
        {
            return;
        }

        if (_damageChance / 100.0 <= Utility.RandomDouble())
        {
            return;
        }

        _nextDamage = Core.Now + TimeSpan.FromSeconds(1.0);

        var burst = AOS.Scale(damage, _damageFactor);
        burst = (int)(burst * GetSlayerBonus());
        burst -= (int)(burst * DamageModifier(Target));

        AOS.Damage(victim, Caster, burst, false, 0, 0, 0, 0, 0, 0, 100);
        victim.FixedParticles(0x374A, 10, 15, 5038, 1181, 0, EffectLayer.Head);
    }

    /// <summary>Hit chance and spell damage penalty applied to the target.</summary>
    public override int PropertyBonus() => _propertyBonus;
}

public class DespairSpell : BardSpell, ITargetingSpell<Mobile>
{
    private static readonly SpellInfo _info = new("Despair", "Kal Des Mani Tym", -1, 9002);

    private int _statMod;
    private int _damage;
    private int _rounds;

    public DespairSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
    {
    }

    public override SkillName CastSkill => SkillName.Discordance;

    public override double RequiredSkill => 90.0;

    public override double UpKeep => 12.0;

    public override int RequiredMana => 26;

    public override bool PartyEffects => false;

    public override double SlayerBonus => 3.0;

    private string StatModName => $"Despair{Caster.Serial}";

    public override void OnCast()
    {
        var existing = GetSpell(Caster, GetType());

        if (existing != null)
        {
            existing.Expire();
            Caster.SendLocalizedMessage(1115774); // You halt your spellsong.
        }
        else
        {
            Caster.Target = new SpellTarget<Mobile>(this, TargetFlags.Harmful);
        }
    }

    void ITargetingSpell<Mobile>.Target(Mobile target) => OnTarget(target);

    private void OnTarget(Mobile m)
    {
        if (m == null)
        {
            return;
        }

        if (!Caster.CanSee(m))
        {
            Caster.SendLocalizedMessage(500237); // Target can not be seen.
        }
        else if (!m.Alive)
        {
            Caster.SendLocalizedMessage(1115773); // Your target is dead.
        }
        else if (Caster == m)
        {
            Caster.SendMessage("You cannot target yourself!");
        }
        else if (HasHarmfulEffects(m, GetType()))
        {
            Caster.SendLocalizedMessage(1115772); // Your target is already under the effect of this spellsong.
        }
        else if (CheckHSequence(m))
        {
            SpellHelper.Turn(Caster, m);

            Target = m;

            HarmfulSpell(m);

            m.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);

            _statMod = (int)(BaseSkillBonus * 2 + CollectiveBonus);
            _damage = (int)(BaseSkillBonus * 4.5 + CollectiveBonus * 2);
            _rounds = 5 + (int)(BaseSkillBonus * 0.75 + CollectiveBonus / 2);

            if (m is PlayerMobile targetPm)
            {
                var args = $"{_statMod}\t{_damage}";
                targetPm.AddBuff(new BuffInfo(BuffIcon.DespairTarget, 1115741, 1115743, args: args));
            }

            if (Caster is PlayerMobile casterPm)
            {
                var args = $"{_statMod}\t{_damage}";
                casterPm.AddBuff(new BuffInfo(BuffIcon.DespairCaster, 1115741, 1115743, args: args));
            }

            BeginTimer();
        }
    }

    public override void EndEffects()
    {
        if (Target is PlayerMobile targetPm)
        {
            targetPm.RemoveBuff(BuffIcon.DespairTarget);
        }

        if (Caster is PlayerMobile casterPm)
        {
            casterPm.RemoveBuff(BuffIcon.DespairCaster);
        }

        base.EndEffects();
    }

    public override void AddStatMods()
    {
        if (Target != null)
        {
            Target.AddStatMod(new StatMod(StatType.Str, StatModName, _statMod, TimeSpan.Zero));
        }
    }

    public override void RemoveStatMods()
    {
        if (Target != null)
        {
            Target.RemoveStatMod(StatModName);
        }
    }

    public override bool OnTick()
    {
        var tick = base.OnTick();

        if (!tick || Target == null || !Caster.InRange(Target.Location, PartyRange))
        {
            return false;
        }

        var damage = _damage;

        if (!Target.Player)
        {
            damage += AOS.Scale(damage, 50); // PvM: 1.5x
        }

        damage = (int)(damage * GetSlayerBonus());
        damage -= (int)(damage * DamageModifier(Target));

        AOS.Damage(Target, Caster, damage, 100, 0, 0, 0, 0);

        if (Target.Alive && Target.Map != null)
        {
            Target.FixedEffect(0x376A, 1, 32);
        }

        if (_rounds-- <= 0)
        {
            Expire();
        }

        return tick;
    }
}
