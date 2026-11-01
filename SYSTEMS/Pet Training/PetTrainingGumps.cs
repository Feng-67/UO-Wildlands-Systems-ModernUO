// UO Wildlands Custom Script
// Adapted for ModernUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)

using System;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using Server.Network;

using static Server.Mobiles.DashboardAnimalLoreGump.GumpFormattingHelpers;

namespace Server.Gumps;

// ==========================================================================================
// PET TRAINING CONFIRM GUMP  (yes/no modal)
// ==========================================================================================
public class PetTrainingConfirmGump : DynamicGump
{
    private readonly int _title;
    private readonly int _body;
    private readonly Action _confirmCallback;
    private readonly Action _cancelCallback;

    public override bool Singleton => true;

    private PetTrainingConfirmGump(int title, int body, Action confirmCallback, Action cancelCallback)
        : base(250, 50)
    {
        _title = title;
        _body = body;
        _confirmCallback = confirmCallback;
        _cancelCallback = cancelCallback;
    }

    public static void DisplayTo(PlayerMobile pm, int title, int body, Action confirmCallback, Action cancelCallback = null)
    {
        if (pm?.NetState == null)
        {
            return;
        }

        pm.CloseGump<PetTrainingConfirmGump>();
        pm.SendGump(new PetTrainingConfirmGump(title, body, confirmCallback, cancelCallback));
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();

        builder.AddBackground(0, 0, 454, 240, 0x24A4);

        builder.AddHtmlLocalized(0, 12, 454, 16, CenterLoc, $"#{_title}", 0xF424E5);
        builder.AddHtmlLocalized(55, 65, 344, 80, _body, C32216(0x8B0000));

        // Original called AddECHandleInput() around each button (Enter/Esc support);
        // dropped — no DynamicGumpBuilder equivalent. See GumpFormattingHelpers.

        builder.AddButton(70, 150, 0x9CC8, 0x9CC7, 1, GumpButtonType.Reply, 0);
        builder.AddHtml(70, 153, 126, 16, Center("Yes"));

        builder.AddButton(235, 150, 0x9CC8, 0x9CC7, 2, GumpButtonType.Reply, 0);
        builder.AddHtml(235, 153, 126, 16, Center("Cancel"));
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID == 1)
        {
            _confirmCallback?.Invoke();
        }
        else if (info.ButtonID == 2)
        {
            _cancelCallback?.Invoke();
        }
    }
}

// ==========================================================================================
// PET TRAINING OPTIONS GUMP  (category / selection browser)
// ==========================================================================================
public class PetTrainingOptionsGump : DynamicGump
{
    private readonly PlayerMobile _from;
    private readonly BaseCreature _creature;
    private readonly int _category;

    private readonly TrainingDefinition _definition;
    private readonly AbilityProfile _abilityProfile;

    public override bool Singleton => true;

    private PetTrainingOptionsGump(PlayerMobile pm, BaseCreature bc, int category) : base(40, 200)
    {
        _from = pm;
        _creature = bc;
        _category = category;

        _definition = PetTrainingHelper.GetTrainingDefinition(bc);
        _abilityProfile = PetTrainingHelper.GetAbilityProfile(bc, true);
    }

    public static void DisplayTo(PlayerMobile pm, BaseCreature bc, int category = 0)
    {
        if (pm?.NetState == null || bc == null || bc.Deleted)
        {
            return;
        }

        pm.SendGump(new PetTrainingOptionsGump(pm, bc, category));
    }

    public void Refresh() => DisplayTo(_from, _creature, _category);

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();

        builder.AddBackground(0, 0, 574, 560, 0x24A4);

        builder.AddHtmlLocalized(0, 11, 574, 18, 1157485); // <CENTER>ANIMAL TRAINING MENU</CENTER>

        builder.AddImageTiled(35, 40, 220, 440, 2624);
        builder.AddImageTiled(37, 42, 216, 436, 3004);

        builder.AddImageTiled(265, 40, 270, 440, 2624);
        builder.AddImageTiled(267, 42, 266, 436, 3004);

        builder.AddHtmlLocalized(35, 63, 220, 20, 1044010, 0); // <CENTER>CATEGORIES</CENTER>
        builder.AddHtmlLocalized(265, 63, 270, 20, 1044011, 0); // <CENTER>SELECTIONS</CENTER>

        builder.AddButton(35, 490, 0x9CC8, 0x9CC7, 0, GumpButtonType.Reply, 0);
        builder.AddHtml(35, 493, 126, 20, Center("CANCEL"));

        builder.AddButton(220, 490, 0x9CC8, 0x9CC7, 1, GumpButtonType.Reply, 0);
        builder.AddHtml(220, 493, 126, 20, Center("PLAN"));

        builder.AddButton(410, 490, 0x9CC8, 0x9CC7, 2, GumpButtonType.Reply, 0);
        builder.AddHtml(410, 493, 126, 20, Center("INFO"));

        var y = 90;

        builder.AddButton(40, y, 4005, 4007, 3, GumpButtonType.Reply, 0);
        builder.AddHtmlLocalized(75, y + 2, 150, 16, 3010049); // Stats
        y += 24;

        builder.AddButton(40, y, 4005, 4007, 4, GumpButtonType.Reply, 0);
        builder.AddHtmlLocalized(75, y + 2, 180, 16, 1114254); // Resists
        y += 24;

        if (HasAvailable(PetTrainingHelper.MagicSkills))
        {
            builder.AddButton(40, y, 4005, 4007, 5, GumpButtonType.Reply, 0);
            builder.AddHtmlLocalized(75, y + 2, 180, 16, 1157495); // Increase Magic Skill Caps
            y += 24;
        }

        if (HasAvailable(PetTrainingHelper.CombatSkills))
        {
            builder.AddButton(40, y, 4005, 4007, 6, GumpButtonType.Reply, 0);
            builder.AddHtmlLocalized(75, y + 2, 180, 16, 1157496); // Increase Combat Skill Caps
            y += 24;
        }

        if (HasAvailable(_definition.MagicalAbilities))
        {
            builder.AddButton(40, y, 4005, 4007, 7, GumpButtonType.Reply, 0);
            builder.AddHtmlLocalized(75, y + 2, 180, 16, 1157481); // Magical Abilities
            y += 24;
        }

        if (HasAvailable(_definition.SpecialAbilities))
        {
            builder.AddButton(40, y, 4005, 4007, 8, GumpButtonType.Reply, 0);
            builder.AddHtmlLocalized(75, y + 2, 180, 16, 1157480); // Special Abilities
            y += 24;
        }

        if (HasAvailable(_definition.WeaponAbilities))
        {
            builder.AddButton(40, y, 4005, 4007, 9, GumpButtonType.Reply, 0);
            builder.AddHtmlLocalized(75, y + 2, 180, 16, 1157479); // Special Moves
            y += 24;
        }

        if (HasAvailable(_definition.AreaEffects))
        {
            builder.AddButton(40, y, 4005, 4007, 10, GumpButtonType.Reply, 0);
            builder.AddHtmlLocalized(75, y + 2, 180, 16, 1157482); // Area Effect Abilities
        }

        switch (_category)
        {
            case 0: BuildStatsPage(ref builder); break;
            case 1: BuildResistsPage(ref builder); break;
            case 2: BuildMagicSkillCapsPage(ref builder); break;
            case 3: BuildCombatSkillCapsPage(ref builder); break;
            case 4: BuildMagicalAbilitiesPage(ref builder); break;
            case 5: BuildSpecialAbilitiesPage(ref builder); break;
            case 6: BuildSpecialMovesPage(ref builder); break;
            case 7: BuildAreaEffectsPage(ref builder); break;
        }
    }

    private void BuildStatsPage(ref DynamicGumpBuilder builder)
    {
        var y = 90;

        foreach (int i in Enum.GetValues(typeof(PetStat)))
        {
            var tp = PetTrainingHelper.GetTrainingPoint((PetStat)i);

            builder.AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
            Record(ref builder, tp, 320, y + 2);

            y += 22;
        }
    }

    private void BuildResistsPage(ref DynamicGumpBuilder builder)
    {
        var y = 90;

        foreach (int i in Enum.GetValues(typeof(ResistanceType)))
        {
            var tp = PetTrainingHelper.GetTrainingPoint((ResistanceType)i);

            builder.AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
            Record(ref builder, tp, 320, y + 2);

            y += 22;
        }
    }

    private void BuildMagicSkillCapsPage(ref DynamicGumpBuilder builder)
    {
        var y = 90;

        foreach (var skill in PetTrainingHelper.MagicSkills)
        {
            if ((!PetTrainingHelper.CommonSkill(_creature, skill) && _creature.Skills[skill].Base <= 0) ||
                _creature.Skills[skill].Cap >= 120)
            {
                continue;
            }

            var tp = PetTrainingHelper.GetTrainingPoint(skill);

            builder.AddButton(275, y, 4005, 4007, 100 + (int)skill, GumpButtonType.Reply, 0);
            Record(ref builder, tp, 320, y + 2);

            y += 22;
        }
    }

    private void BuildCombatSkillCapsPage(ref DynamicGumpBuilder builder)
    {
        var y = 90;

        foreach (var skill in PetTrainingHelper.CombatSkills)
        {
            if ((!PetTrainingHelper.CommonSkill(_creature, skill) && _creature.Skills[skill].Base <= 0) ||
                _creature.Skills[skill].Cap >= 120)
            {
                continue;
            }

            var tp = PetTrainingHelper.GetTrainingPoint(skill);

            builder.AddButton(275, y, 4005, 4007, 100 + (int)skill, GumpButtonType.Reply, 0);
            Record(ref builder, tp, 320, y + 2);

            y += 22;
        }
    }

    private void BuildMagicalAbilitiesPage(ref DynamicGumpBuilder builder)
    {
        var y = 90;

        for (var i = 0; i < PetTrainingHelper.MagicalAbilities.Length; i++)
        {
            var abil = PetTrainingHelper.MagicalAbilities[i];
            var master = _creature.ControlMaster;

            if ((abil == MagicalAbility.Chivalry && master != null && master.Karma < 0) ||
                (abil == MagicalAbility.Necromancy && master != null && master.Karma > 0) ||
                (abil == MagicalAbility.Necromage && master != null && master.Karma > 0))
            {
                continue;
            }

            if ((_definition.MagicalAbilities & abil) == 0 || _abilityProfile.HasAbility(abil) ||
                !_abilityProfile.CanChooseMagicalAbility(abil) ||
                ((abil & MagicalAbility.Tokuno) != 0 && !_abilityProfile.TokunoTame))
            {
                continue;
            }

            var tp = PetTrainingHelper.GetTrainingPoint(abil);

            builder.AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
            Record(ref builder, tp, 320, y + 2);

            y += 22;
        }
    }

    private void BuildSpecialAbilitiesPage(ref DynamicGumpBuilder builder)
    {
        var y = 90;

        for (var i = 0; i < SpecialAbility.Abilities.Length; i++)
        {
            var abil = SpecialAbility.Abilities[i];

            if (_abilityProfile.HasAbility(abil) ||
                (_abilityProfile.HasSpecialMagicalAbility() && !_abilityProfile.IsRuleBreaker(abil)))
            {
                continue;
            }

            if (_definition.HasSpecialAbility(abil) ||
                (_abilityProfile.HasAbility(MagicalAbility.Poisoning) && abil is VenomousBite))
            {
                var tp = PetTrainingHelper.GetTrainingPoint(abil);

                builder.AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                Record(ref builder, tp, 320, y + 2);

                y += 22;
            }
        }
    }

    private void BuildSpecialMovesPage(ref DynamicGumpBuilder builder)
    {
        var y = 90;

        for (var i = 0; i < _definition.WeaponAbilities.Length; i++)
        {
            var abil = _definition.WeaponAbilities[i];

            if (_abilityProfile.HasAbility(abil))
            {
                continue;
            }

            var tp = PetTrainingHelper.GetTrainingPoint(abil);

            builder.AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
            Record(ref builder, tp, 320, y + 2);

            y += 22;
        }
    }

    private void BuildAreaEffectsPage(ref DynamicGumpBuilder builder)
    {
        var y = 90;

        for (var i = 0; i < AreaEffect.Effects.Length; i++)
        {
            var abil = AreaEffect.Effects[i];

            if (_abilityProfile.HasAbility(abil))
            {
                continue;
            }

            if (_definition.HasAreaEffect(abil) ||
                (_abilityProfile.HasAbility(MagicalAbility.Poisoning) && abil is PoisonBreath))
            {
                var tp = PetTrainingHelper.GetTrainingPoint(abil);

                builder.AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                Record(ref builder, tp, 320, y + 2);

                y += 22;
            }
        }
    }

    private static void Record(ref DynamicGumpBuilder builder, TrainingPoint tp, int x, int y)
    {
        if (tp == null)
        {
            builder.AddLabel(x, y, 0, "Null Training Point");
            return;
        }

        if (tp.Name.Number > 0)
        {
            builder.AddHtmlLocalized(x, y, 215, 20, tp.Name.Number, 0);
        }
        else if (tp.Name.String != null)
        {
            builder.AddLabel(x, y, 0, tp.Name.String);
        }
    }

    private bool HasAvailable(object o)
    {
        if (o == null)
        {
            return false;
        }

        if (o is MagicalAbility && _definition.MagicalAbilities != 0)
        {
            foreach (MagicalAbility ability in Enum.GetValues(typeof(MagicalAbility)))
            {
                if ((_definition.MagicalAbilities & ability) != 0 && !_abilityProfile.HasAbility(ability))
                {
                    return true;
                }
            }

            return false;
        }

        if (o is SpecialAbility[] specialAbilities)
        {
            if (!_abilityProfile.CanChooseSpecialAbility(specialAbilities))
            {
                return false;
            }

            if (_abilityProfile.HasAbility(MagicalAbility.Poisoning) && !_abilityProfile.HasAbility(SpecialAbility.VenomousBite))
            {
                return true;
            }

            foreach (var ability in specialAbilities)
            {
                if (!_abilityProfile.HasAbility(ability))
                {
                    return true;
                }
            }
        }
        else if (o is WeaponAbility[] weaponAbilities)
        {
            if (!_abilityProfile.CanChooseWeaponAbility())
            {
                return false;
            }

            foreach (var ability in weaponAbilities)
            {
                if (!_abilityProfile.HasAbility(ability))
                {
                    return true;
                }
            }
        }
        else if (o is AreaEffect[] areaEffects)
        {
            if (!_abilityProfile.CanChooseAreaEffect())
            {
                return false;
            }

            if (_abilityProfile.HasAbility(MagicalAbility.Poisoning) && !_abilityProfile.HasAbility(AreaEffect.PoisonBreath))
            {
                return true;
            }

            foreach (var ability in areaEffects)
            {
                if (!_abilityProfile.HasAbility(ability))
                {
                    return true;
                }
            }
        }
        else if (o is SkillName[] skillNames)
        {
            foreach (var name in skillNames)
            {
                var skill = _creature.Skills[name];

                if (skill.Base > 0 && skill.Cap < 120)
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var id = info.ButtonID;

        if (id == 0) // Cancel
        {
            return;
        }

        if (id == 1) // Plan
        {
            Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
            {
                if (_from.HasGump<DashboardAnimalLoreGump>())
                {
                    DisplayTo(_from, _creature, _category);
                    _from.CloseGump<PetTrainingPlanningGump>();
                    PetTrainingPlanningGump.DisplayTo(_from, _creature);
                }
            });
            return;
        }

        if (id == 2) // Info
        {
            Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
            {
                if (_from.HasGump<DashboardAnimalLoreGump>())
                {
                    DisplayTo(_from, _creature, _category);
                    _from.CloseGump<PetTrainingInfoGump>();
                    PetTrainingInfoGump.DisplayTo(_from);
                }
            });
            return;
        }

        if (id <= 11) // Category tab buttons
        {
            Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
            {
                if (_from.HasGump<DashboardAnimalLoreGump>())
                {
                    DisplayTo(_from, _creature, id - 3);
                }
            });
            return;
        }

        TrainingPoint tp = null;

        switch (_category)
        {
            case 0: // stats
                tp = PetTrainingHelper.GetTrainingPoint((PetStat)(id - 100));
                break;
            case 1: // resists
                tp = PetTrainingHelper.GetTrainingPoint((ResistanceType)(id - 100));
                break;
            case 2: // mag skill
            case 3: // combat skill
                tp = PetTrainingHelper.GetTrainingPoint((SkillName)(id - 100));
                break;
            case 4: // mag abil
                tp = PetTrainingHelper.GetTrainingPoint(PetTrainingHelper.MagicalAbilities[id - 100]);
                break;
            case 5: // spec abil
                tp = PetTrainingHelper.GetTrainingPoint(SpecialAbility.Abilities[id - 100]);
                break;
            case 6: // spec moves
                tp = PetTrainingHelper.GetTrainingPoint(_definition.WeaponAbilities[id - 100]);
                break;
            case 7: // area effects
                tp = PetTrainingHelper.GetTrainingPoint(AreaEffect.Effects[id - 100]);
                break;
        }

        if (tp != null)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
            {
                if (_from.HasGump<DashboardAnimalLoreGump>())
                {
                    _from.CloseGump<PetTrainingConfirmationGump>();
                    PetTrainingConfirmationGump.DisplayTo(_from, _creature, tp);
                }
            });
        }
    }
}

// ==========================================================================================
// PET TRAINING CONFIRMATION GUMP  (value-adjust / train)
// ==========================================================================================
public class PetTrainingConfirmationGump : DynamicGump
{
    private readonly PlayerMobile _from;
    private readonly BaseCreature _creature;
    private readonly TrainingPoint _trainingPoint;

    private readonly TrainingProfile _profile;
    private readonly int _startValue;
    private readonly int _value;
    private readonly int _max;
    private readonly double _weight;
    private readonly int _cost;
    private readonly int _avail;

    public override bool Singleton => true;

    private PetTrainingConfirmationGump(PlayerMobile pm, BaseCreature bc, TrainingPoint tp, int rawValue)
        : base(50, 200)
    {
        _from = pm;
        _creature = bc;
        _trainingPoint = tp;

        _profile = PetTrainingHelper.GetTrainingProfile(bc, true);

        _max = tp.GetMax(bc);
        _weight = tp.Weight;

        var start = -1;
        PetTrainingHelper.GetStartValue(tp, bc, ref start);
        _startValue = start;

        var value = rawValue;

        if (_startValue > value)
        {
            value = _startValue;
        }

        if (value > _max)
        {
            value = _max;
        }

        var cost = PetTrainingHelper.GetTotalCost(tp, bc, value, _startValue);

        if (CanAdjust(tp, _max) && cost > _profile.TrainingPoints)
        {
            value = _startValue + (int)(_profile.TrainingPoints / _weight);
        }

        if (_startValue > 0)
        {
            var valueWeight = value * _weight;
            var maxWeight = _max * _weight;
            var nonAdjustedWeight = (value - _startValue) * _weight;

            var originalValueWeight = valueWeight;

            if (tp.TrainPoint is PetStat stat && stat <= PetStat.Mana)
            {
                var maxTotalCap = PetTrainingHelper.GetTrainingCapTotal(stat);
                var currentTotal = stat <= PetStat.Int
                    ? PetTrainingHelper.GetTotalStatWeight(bc)
                    : PetTrainingHelper.GetTotalAttributeWeight(bc);

                if (currentTotal >= maxTotalCap)
                {
                    valueWeight = 0;
                }
                else if (currentTotal + nonAdjustedWeight > maxTotalCap)
                {
                    valueWeight -= currentTotal + nonAdjustedWeight - maxTotalCap;
                }
            }
            else if (tp.TrainPoint is ResistanceType resist)
            {
                var maxTotalCap = PetTrainingHelper.GetTrainingCapTotal(resist);
                var currentTotal = PetTrainingHelper.GetTotalResistWeight(bc);

                if (currentTotal >= maxTotalCap)
                {
                    valueWeight = 0;
                }
                else if (currentTotal + nonAdjustedWeight > maxTotalCap)
                {
                    valueWeight -= currentTotal + nonAdjustedWeight - maxTotalCap;
                }
            }

            if (valueWeight > maxWeight)
            {
                valueWeight = maxWeight;
            }

            if (originalValueWeight != valueWeight)
            {
                value = Math.Max(_startValue, (int)(valueWeight / _weight));
            }
        }

        _value = value;
        _cost = PetTrainingHelper.GetTotalCost(tp, bc, _value, _startValue);
        _avail = _profile.TrainingPoints - _cost;
    }

    public static void DisplayTo(PlayerMobile pm, BaseCreature bc, TrainingPoint tp, int? value = null)
    {
        if (pm?.NetState == null || bc == null || bc.Deleted || tp == null)
        {
            return;
        }

        pm.SendGump(new PetTrainingConfirmationGump(pm, bc, tp, value ?? tp.Start));
    }

    // Called by PetTrainingPlanningGump's ENABLE/DISABLE toggle via
    // FindGump<PetTrainingConfirmationGump>()?.Refresh(). Re-sends with the
    // same point and current value; the constructor re-applies clamping.
    public void Refresh() => DisplayTo(_from, _creature, _trainingPoint, _value);

    private static bool CanAdjust(TrainingPoint tp, int max) => max != tp.Start;

    private bool CanAdjust() => CanAdjust(_trainingPoint, _max);

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        var tp = _trainingPoint;

        builder.AddPage();

        builder.AddBackground(0, 0, 574, 470, 0x24A4);
        builder.AddHtmlLocalized(0, 12, 574, 16, 1157486); // <CENTER>TRAINING CONFIRMATION</CENTER>

        builder.AddImageTiled(35, 40, 245, 140, 2624);
        builder.AddImageTiled(37, 42, 241, 136, 3004);

        builder.AddImageTiled(290, 40, 245, 140, 2624);
        builder.AddImageTiled(292, 42, 241, 136, 3004);

        builder.AddImageTiled(35, 190, 245, 140, 2624);
        builder.AddImageTiled(37, 192, 241, 136, 3004);

        builder.AddImageTiled(290, 190, 245, 140, 2624);
        builder.AddImageTiled(292, 192, 241, 136, 3004);

        builder.AddImageTiled(35, 340, 500, 60, 2624);
        builder.AddImageTiled(37, 342, 496, 56, 3004);

        builder.AddButton(40, 410, 0x9CC8, 0x9CC7, 1, GumpButtonType.Reply, 0);
        builder.AddHtml(40, 413, 126, 20, Center("Back"));

        builder.AddButton(415, 410, 0x9CC8, 0x9CC7, _profile.TrainingMode == TrainingMode.Regular ? 8 : 9, GumpButtonType.Reply, 0);
        builder.AddHtml(415, 413, 126, 20, Center(_profile.TrainingMode == TrainingMode.Regular ? "Train Pet" : "Add To Plan"));

        builder.AddHtmlLocalized(35, 55, 245, 20, CenterLoc, "#1114269", 0); // PROPERTY INFORMATION
        builder.AddHtmlLocalized(50, 85, 60, 16, 1114270); // Property:
        builder.AddHtmlLocalized(50, 105, 60, 16, 1114272); // Weight:

        if (tp.Name.Number > 0)
        {
            builder.AddHtmlLocalized(120, 85, 200, 16, tp.Name.Number);
        }
        else if (tp.Name.String != null)
        {
            builder.AddLabel(120, 85, 0, tp.Name.String);
        }

        builder.AddLabel(120, 105, 0, tp.Weight.ToString("0.0"));

        if (tp.Description.Number > 0)
        {
            builder.AddHtmlLocalized(305, 55, 215, 115, tp.Description.Number, true, true);
        }
        else if (tp.Description.String != null)
        {
            builder.AddHtml(305, 55, 215, 115, tp.Description.String, background: true, scrollbar: true);
        }

        builder.AddHtmlLocalized(290, 205, 245, 20, CenterLoc, "#1113650", 0); // RESULTS
        builder.AddHtmlLocalized(35, 205, 245, 20, CenterLoc, "#1157493", 0);  // REQUIREMENTS

        if (tp.Requirements != null && tp.Requirements.Length > 0)
        {
            for (var i = 0; i < tp.Requirements.Length; i++)
            {
                var req = tp.Requirements[i];

                if (req == null)
                {
                    continue;
                }

                if (req.Name.Number > 0)
                {
                    builder.AddHtmlLocalized(45, 225 + i * 20, 190, 18, req.Name.Number);
                }
                else if (req.Name.String != null)
                {
                    builder.AddHtml(45, 225 + i * 20, 190, 18, req.Name.String);
                }

                builder.AddTooltip(1157523);

                var reqCost = req.Cost;

                if (req.Requirement is SkillName reqSkill && _creature.Skills[reqSkill].Value > 0)
                {
                    reqCost = 0;
                }

                builder.AddLabel(245, 225 + i * 20, 0x26, reqCost.ToString());
                builder.AddTooltip(1157523);
            }
        }
        else if (tp.TrainPoint is SkillName)
        {
            var cliloc = _value switch
            {
                100 => 1049640,
                150 => 1049641,
                200 => 1049642,
                _   => 1049639 // default, and 50
            };

            builder.AddHtmlLocalized(45, 225, 225, 60, cliloc, $"#{tp.Name.Number}", 0);
        }

        builder.AddHtmlLocalized(305, 225, 145, 18, 1157490); // Avail. Training Points:
        builder.AddLabel(455, 225, _avail <= 0 ? 0x26 : 0, _avail.ToString());

        builder.AddHtmlLocalized(305, 245, 145, 18, 1113646); // Total Property Weight:
        builder.AddLabel(455, 245, 0, $"{(int)(_value * _weight)}/{_max * _weight}");

        if (tp.Name.Number > 0)
        {
            builder.AddHtmlLocalized(305, 265, 145, 18, tp.Name.Number);
        }
        else if (tp.Name.String != null)
        {
            builder.AddLabel(305, 265, 0, tp.Name.String);
        }

        if (tp.TrainPoint is SkillName)
        {
            builder.AddLabel(455, 265, 0, (100.0 + (double)_value / 10).ToString("0.0"));
        }
        else
        {
            builder.AddLabel(455, 265, 0, _startValue > _value ? _startValue.ToString() : _value.ToString());
        }

        builder.AddHtmlLocalized(230, 352, 150, 18, 1113586); // Property Weight:
        builder.AddHtml(267, 373, 40, 18, Center(((int)(_value * _weight)).ToString()));

        if (CanAdjust())
        {
            builder.AddButton(205, 375, 0x1464, 0x1464, 2, GumpButtonType.Reply, 0);
            builder.AddButton(213, 375, 0x1466, 0x1466, 2, GumpButtonType.Reply, 0);

            builder.AddButton(225, 375, 0x1464, 0x1464, 3, GumpButtonType.Reply, 0);
            builder.AddButton(233, 375, 0x1466, 0x1466, 3, GumpButtonType.Reply, 0);

            builder.AddButton(245, 375, 0x1464, 0x1464, 4, GumpButtonType.Reply, 0);
            builder.AddButton(253, 375, 0x1466, 0x1466, 4, GumpButtonType.Reply, 0);

            builder.AddButton(305, 375, 0x1464, 0x1464, 5, GumpButtonType.Reply, 0);
            builder.AddButton(313, 375, 0x1466, 0x1466, 5, GumpButtonType.Reply, 0);

            builder.AddButton(325, 375, 0x1464, 0x1464, 6, GumpButtonType.Reply, 0);
            builder.AddButton(333, 375, 0x1466, 0x1466, 6, GumpButtonType.Reply, 0);

            builder.AddButton(345, 375, 0x1464, 0x1464, 7, GumpButtonType.Reply, 0);
            builder.AddButton(353, 375, 0x1466, 0x1466, 7, GumpButtonType.Reply, 0);
        }
        else
        {
            builder.AddImage(205, 375, 0x1464);
            builder.AddImage(213, 375, 0x1466);

            builder.AddImage(225, 375, 0x1464);
            builder.AddImage(233, 375, 0x1466);

            builder.AddImage(245, 375, 0x1464);
            builder.AddImage(253, 375, 0x1466);

            builder.AddImage(305, 375, 0x1464);
            builder.AddImage(313, 375, 0x1466);

            builder.AddImage(325, 375, 0x1464);
            builder.AddImage(333, 375, 0x1466);

            builder.AddImage(345, 375, 0x1464);
            builder.AddImage(353, 375, 0x1466);
        }

        builder.AddLabel(207, 373, 0, "<");
        builder.AddLabel(211, 373, 0, "<");
        builder.AddLabel(215, 373, 0, "<");

        builder.AddLabel(228, 373, 0, "<");
        builder.AddLabel(232, 373, 0, "<");

        builder.AddLabel(251, 373, 0, "<");

        builder.AddLabel(312, 373, 0, ">");

        builder.AddLabel(329, 373, 0, ">");
        builder.AddLabel(333, 373, 0, ">");

        builder.AddLabel(348, 373, 0, ">");
        builder.AddLabel(352, 373, 0, ">");
        builder.AddLabel(356, 373, 0, ">");
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        switch (info.ButtonID)
        {
            case 0:
                break;

            case 1: // Back
                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                {
                    if (_from.HasGump<DashboardAnimalLoreGump>())
                    {
                        PetTrainingOptionsGump.DisplayTo(_from, _creature);
                    }
                });
                break;

            case 2: // <<<
            {
                var newValue = CanAdjust() ? _trainingPoint.Start : _value;
                DisplayTo(_from, _creature, _trainingPoint, newValue);
                break;
            }

            case 3: // <<
            {
                var newValue = _value;

                if (CanAdjust())
                {
                    newValue = _trainingPoint.TrainPoint is SkillName
                        ? Math.Max(_trainingPoint.Start, _value - 50)
                        : _value - _max / 10;
                }

                DisplayTo(_from, _creature, _trainingPoint, newValue);
                break;
            }

            case 4: // <
            {
                var newValue = _value;

                if (CanAdjust())
                {
                    if (_trainingPoint.TrainPoint is SkillName)
                    {
                        newValue = Math.Max(_trainingPoint.Start, _value - 50);
                    }
                    else
                    {
                        newValue = _weight < 1.0 ? _value - (int)(_weight * 100) : _value - 1;
                    }
                }

                DisplayTo(_from, _creature, _trainingPoint, newValue);
                break;
            }

            case 5: // >
            {
                var newValue = _value;

                if (CanAdjust())
                {
                    if (_trainingPoint.TrainPoint is SkillName)
                    {
                        newValue = Math.Min(_max, _value + 50);
                    }
                    else
                    {
                        newValue = _weight < 1.0 ? _value + (int)(_weight * 100) : _value + 1;
                    }
                }

                DisplayTo(_from, _creature, _trainingPoint, newValue);
                break;
            }

            case 6: // >>
            {
                var newValue = _value;

                if (CanAdjust())
                {
                    newValue = _trainingPoint.TrainPoint is SkillName
                        ? Math.Min(_max, _value + 50)
                        : _value + _max / 10;
                }

                DisplayTo(_from, _creature, _trainingPoint, newValue);
                break;
            }

            case 7: // >>>
            {
                var newValue = CanAdjust() ? _max : _value;
                DisplayTo(_from, _creature, _trainingPoint, newValue);
                break;
            }

            case 8: // Train Pet
            {
                var cost = PetTrainingHelper.GetTotalCost(_trainingPoint, _creature, _value, _startValue);
                Item scroll = null;

                if (!_creature.Controlled || _from != _creature.ControlMaster)
                {
                    _from.SendLocalizedMessage(1114368); // This is not your pet!
                }
                else if (!_creature.InRange(_from.Location, 12))
                {
                    _from.SendLocalizedMessage(1153204); // The pet is too far away from you!
                }
                else if (Server.Spells.SpellHelper.CheckCombat(_from) || Server.Spells.SpellHelper.CheckCombat(_creature) ||
                         _creature.Aggressed.Count > 0 || _creature.Combatant != null)
                {
                    _from.SendLocalizedMessage(1156876); // Since you have been in combat recently you may not use this feature.
                }
                else if (!_profile.HasIncreasedControlSlot && _from.Followers >= _from.FollowersMax)
                {
                    _from.SendLocalizedMessage(1157498); // You do not have the available pet slots to train this pet. Please free up pet slots and try again.
                }
                else if (cost > _profile.TrainingPoints)
                {
                    _from.SendLocalizedMessage(1157500); // Your pet lacks the required points to complete this training.
                }
                else if (_trainingPoint.TrainPoint is SkillName skillName && !CheckPowerScroll(skillName, _value, ref scroll))
                {
                    _from.SendLocalizedMessage(1157499); // You are unable to train your pet. The required powerscroll was not found in your main backpack.
                }
                else if (_startValue >= _value)
                {
                    _from.SendLocalizedMessage(1157501); // Your pet looks to have already completed that training.
                }
                else if (PetTrainingHelper.CanControl(_from, _creature, _profile))
                {
                    PetTrainingConfirmGump.DisplayTo(
                        _from, 1157502, _trainingPoint.TrainPoint is MagicalAbility ? 1157566 : 1157503,
                        () =>
                        {
                            if (PetTrainingHelper.ApplyTrainingPoint(_creature, _trainingPoint, _value))
                            {
                                _from.SendLocalizedMessage(1157497); // You successfully train the pet!

                                if (_trainingPoint.TrainPoint is SkillName)
                                {
                                    Effects.SendLocationParticles(
                                        EffectItem.Create(_creature.Location, _creature.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0
                                    );
                                    Effects.PlaySound(_creature.Location, _creature.Map, 0x243);

                                    Effects.SendMovingParticles(
                                        new Entity(Server.Serial.Zero, new Point3D(_creature.X - 6, _creature.Y - 6, _creature.Z + 15), _creature.Map),
                                        _creature, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100
                                    );
                                    Effects.SendMovingParticles(
                                        new Entity(Server.Serial.Zero, new Point3D(_creature.X - 4, _creature.Y - 6, _creature.Z + 15), _creature.Map),
                                        _creature, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100
                                    );
                                    Effects.SendMovingParticles(
                                        new Entity(Server.Serial.Zero, new Point3D(_creature.X - 6, _creature.Y - 4, _creature.Z + 15), _creature.Map),
                                        _creature, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100
                                    );

                                    Effects.SendTargetParticles(_creature, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

                                    scroll?.Delete();
                                }
                                else
                                {
                                    _creature.PlaySound(0x1EB);
                                    _creature.FixedEffect(0x375A, 1, 11);
                                }

                                _profile.OnTrain(_from, cost);

                                ResendGumps(_profile.HasBegunTraining);

                                //Server.Engines.Quests.TeachingSomethingNewQuest.CheckComplete(_from);
                            }
                        },
                        () => ResendGumps(_profile.HasBegunTraining)
                    );
                }
                else
                {
                    _from.SendLocalizedMessage(1157550); // You lack the taming skill required to train this creature.
                }

                break;
            }

            case 9: // Add To Plan
                if (_value > _startValue)
                {
                    PetTrainingHelper.GetPlanningProfile(_creature, true).AddToPlan(
                        _trainingPoint.TrainPoint, _value,
                        PetTrainingHelper.GetTotalCost(_trainingPoint, _creature, _value, _startValue)
                    );

                    _from.SendLocalizedMessage(1157592); // Your selection has been added to your training plan.
                }

                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                {
                    if (_from.HasGump<DashboardAnimalLoreGump>())
                    {
                        DisplayTo(_from, _creature, _trainingPoint, _value);

                        var planningGump = _from.FindGump<PetTrainingPlanningGump>();

                        if (planningGump != null)
                        {
                            planningGump.Refresh();
                        }
                        else
                        {
                            PetTrainingPlanningGump.DisplayTo(_from, _creature);
                        }
                    }
                });
                break;
        }
    }

    private void ResendGumps(bool sendOptions)
    {
        Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
        {
            var dashboard = _from.FindGump<DashboardAnimalLoreGump>();

            if (dashboard != null)
            {
                dashboard.Refresh();
            }
            else
            {
                DashboardAnimalLoreGump.DisplayTo(_from, _creature);
            }

            if (sendOptions)
            {
                var options = _from.FindGump<PetTrainingOptionsGump>();

                if (options != null)
                {
                    options.Refresh();
                }
                else
                {
                    PetTrainingOptionsGump.DisplayTo(_from, _creature);
                }
            }
        });
    }

    private bool CheckPowerScroll(SkillName name, int value, ref Item scroll)
    {
        if (_from.Backpack == null)
        {
            return false;
        }

        scroll = _from.Backpack.Items.OfType<PowerScroll>().FirstOrDefault(ps => ps.Skill == name && ps.Value == 100 + value / 10);

        return scroll != null;
    }
}

// ==========================================================================================
// PET TRAINING PLANNING GUMP
// ==========================================================================================
public class PetTrainingPlanningGump : DynamicGump
{
    private readonly PlayerMobile _from;
    private readonly BaseCreature _creature;

    public override bool Singleton => true;

    private PetTrainingPlanningGump(PlayerMobile pm, BaseCreature bc)
        : base(50, 200)
    {
        _from = pm;
        _creature = bc;
    }

    public static void DisplayTo(PlayerMobile pm, BaseCreature bc)
    {
        if (pm?.NetState == null || bc == null || bc.Deleted)
        {
            return;
        }

        pm.SendGump(new PetTrainingPlanningGump(pm, bc));
    }

    public void Refresh() => DisplayTo(_from, _creature);

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        var profile = PetTrainingHelper.GetTrainingProfile(_creature, true);
        var plan = profile.PlanningProfile;

        builder.AddPage();

        builder.AddBackground(0, 0, 654, 524, 0x24A4);

        builder.AddButton(275, 35, 0x9CC8, 0x9CC7, 1, GumpButtonType.Reply, 0);
        builder.AddHtml(
            275, 38, 126, 20,
            Center(profile.TrainingMode == TrainingMode.Planning ? "DISABLE" : "ENABLE")
        );

        builder.AddHtmlLocalized(0, 11, 654, 18, CenterLoc, "#1157591", 0xF424E5); // Pet Training Planning
        builder.AddHtmlLocalized(60, 60, 216, 18, 1044010);  // <CENTER>CATEGORIES</CENTER>
        builder.AddHtmlLocalized(275, 60, 216, 18, 1044011); // <CENTER>SELECTIONS</CENTER>
        builder.AddHtmlLocalized(510, 60, 150, 18, 1113586); // Property Weight:

        builder.AddHtmlLocalized(260, 80, 150, 16, 1157490, C32216(0x8B0000)); // Avail. Training Points:
        builder.AddLabel(510, 80, 0, profile.TrainingPoints.ToString());

        var y = 95;
        var total = 0;

        for (var i = 0; i < plan.Entries.Count; i++)
        {
            var entry = plan.Entries[i];

            builder.AddButton(25, y, 4017, 4019, i + 100, GumpButtonType.Reply, 0);
            builder.AddHtmlLocalized(60, y, 200, 18, PetTrainingHelper.GetCategoryLocalization(entry.TrainPoint));

            var loc = PetTrainingHelper.GetLocalization(entry.TrainPoint);

            if (loc[0] != null)
            {
                if (loc[0].Number > 0)
                {
                    builder.AddHtmlLocalized(260, y, 200, 18, loc[0].Number);
                }
                else if (loc[0].String != null)
                {
                    builder.AddHtml(260, y, 200, 18, loc[0].String);
                }
            }

            var value = entry.TrainPoint is SkillName ? entry.Value + 1000 : entry.Value;

            builder.AddLabel(460, y, entry.Value == 0 ? 0x27 : 0, value.ToString());
            builder.AddLabel(510, y, entry.Cost == 0 ? 0x27 : 0, $"-{entry.Cost}");

            total += entry.Cost;
            y += 22;
        }

        var remaining = profile.TrainingPoints - total;

        builder.AddHtmlLocalized(260, y, 200, 18, 1157599, C32216(0x8B0000)); // Remaining Training Points:
        builder.AddLabel(510, y + 5, remaining < profile.TrainingPoints ? 0x27 : 0, remaining.ToString());
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var id = info.ButtonID;

        if (id == 0)
        {
            return;
        }

        var profile = PetTrainingHelper.GetTrainingProfile(_creature, true);
        var plan = profile.PlanningProfile;

        if (id == 1) // Toggle TrainingMode
        {
            if (profile.TrainingMode == TrainingMode.Planning)
            {
                profile.TrainingMode = TrainingMode.Regular;
                _from.SendLocalizedMessage(1157598); // You have disabled pet training planning.
            }
            else
            {
                profile.TrainingMode = TrainingMode.Planning;
                _from.SendLocalizedMessage(1157597);
            }

            // Reflect the toggle in any open confirmation gump — matches ServUO.
            _from.FindGump<PetTrainingConfirmationGump>()?.Refresh();

            Refresh();
        }
        else
        {
            id -= 100;

            if (id >= 0 && id < plan.Entries.Count)
            {
                plan.Entries.RemoveAt(id);
                _from.SendLocalizedMessage(1157593); // Your selection has been removed to your training plan.
            }

            Refresh();
        }
    }
}

// ==========================================================================================
// PET TRAINING INFO GUMP
// ==========================================================================================
public class PetTrainingInfoGump : DynamicGump
{
    private readonly PlayerMobile _from;

    public override bool Singleton => true;

    private PetTrainingInfoGump(PlayerMobile pm)
        : base(50, 200)
    {
        _from = pm;
    }

    public static void DisplayTo(PlayerMobile pm)
    {
        if (pm?.NetState == null)
        {
            return;
        }

        pm.SendGump(new PetTrainingInfoGump(pm));
    }

    public void Refresh() => DisplayTo(_from);

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();
        builder.AddBackground(0, 0, 654, 500, 0x24A4);
        builder.AddHtmlLocalized(0, 45, 654, 16, CenterLoc, "#1157527", 0xF424E5); // Discovering Animal Training

        builder.AddPage(1);
        builder.AddHtmlLocalized(30, 75, 590, 380, 1157558, true, true);
        builder.AddButton(560, 45, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 4);
        builder.AddButton(585, 45, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 2);

        builder.AddPage(2);
        builder.AddHtmlLocalized(30, 75, 590, 380, 1157563, true, true);
        builder.AddButton(560, 45, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 1);
        builder.AddButton(585, 45, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 3);

        builder.AddPage(3);
        builder.AddHtmlLocalized(30, 75, 590, 380, 1157552, true, true);
        builder.AddButton(560, 45, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 2);
        builder.AddButton(585, 45, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 4);

        builder.AddPage(4);
        builder.AddHtmlLocalized(30, 75, 590, 380, 1157553, true, true);
        builder.AddButton(560, 45, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 3);
        builder.AddButton(585, 45, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 1);
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        // Page navigation is handled client-side; no reply buttons on this gump.
    }
}
