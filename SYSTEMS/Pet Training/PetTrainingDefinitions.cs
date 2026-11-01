// UO Wildlands Custom Script — ModernUO
// Trainable pet ability registry: creature training definitions and ability packages.
// Licensed under GPL-3.0.

using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    /// <summary>
    /// The ServUO creature training definitions. Creature types are resolved by name so the table
    /// loads on a core that does not have every shard-specific creature; missing types are skipped
    /// and reported through <see cref="MissingTypes"/> instead of failing to compile.
    /// </summary>
    public static class PetTrainingDefinitions
    {
        #region Special ability packages

        public static readonly SpecialAbility[] SpecialAbilityNone = Array.Empty<SpecialAbility>();
        public static readonly SpecialAbility[] SpecialAbilityUnicorn = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.ManaDrain, SpecialAbility.RagingBreath, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.StealLife };
        public static readonly SpecialAbility[] SpecialAbilityMagical1 = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.RagingBreath };
        public static readonly SpecialAbility[] SpecialAbilityMagical2 = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.StealLife };
        public static readonly SpecialAbility[] SpecialAbilityNecroMagical = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.LifeLeech };
        public static readonly SpecialAbility[] SpecialAbilityBites = new[] { SpecialAbility.VenomousBite, SpecialAbility.ViciousBite };
        public static readonly SpecialAbility[] SpecialAbilityAnimalStandard = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds };
        public static readonly SpecialAbility[] SpecialAbilityBitingAnimal = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite };
        public static readonly SpecialAbility[] SpecialAbilityClawed = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw };
        public static readonly SpecialAbility[] SpecialAbilityTailed = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.TailSwipe };
        public static readonly SpecialAbility[] SpecialAbilityClawedAndTailed = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe };
        public static readonly SpecialAbility[] SpecialAbilityInsectoid = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.RuneCorruption };
        public static readonly SpecialAbility[] SpecialAbilityMagicalInsectoid = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.RuneCorruption, SpecialAbility.AngryFire, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.RagingBreath };
        public static readonly SpecialAbility[] SpecialAbilityStickySkin = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.StickySkin };
        public static readonly SpecialAbility[] SpecialAbilityBitingStickySkin = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.StickySkin, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite };
        public static readonly SpecialAbility[] SpecialAbilityTailedAndStickySkin = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.TailSwipe, SpecialAbility.StickySkin, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite };
        public static readonly SpecialAbility[] SpecialAbilityBitingTailed = new[] { SpecialAbility.VenomousBite, SpecialAbility.ViciousBite, SpecialAbility.TailSwipe };
        public static readonly SpecialAbility[] SpecialAbilityBitingClawedAndTailed = new[] { SpecialAbility.VenomousBite, SpecialAbility.ViciousBite, SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe };
        public static readonly SpecialAbility[] SpecialAbilityClawedAndNecromantic = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.LifeLeech };
        public static readonly SpecialAbility[] SpecialAbilityClawedTailedAndMagical1 = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe, SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.RagingBreath };
        public static readonly SpecialAbility[] SpecialAbilityClawedTailedAndMagical2 = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe, SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.StealLife };
        public static readonly SpecialAbility[] SpecialAbilityBaneDragon = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe, SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite };
        public static readonly SpecialAbility[] SpecialAbilityDreadSpider = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.LifeLeech, SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite };
        public static readonly SpecialAbility[] SpecialAbilityFireBeetle = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.RuneCorruption, SpecialAbility.AngryFire, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.RagingBreath };
        public static readonly SpecialAbility[] SpecialAbilityImp = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.LifeLeech, SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe, SpecialAbility.ViciousBite };
        public static readonly SpecialAbility[] SpecialAbilityTsukiWolf = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.LifeLeech, SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe };
        public static readonly SpecialAbility[] SpecialAbilityTriton = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.ManaDrain, SpecialAbility.RagingBreath, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.StealLife, SpecialAbility.StickySkin, SpecialAbility.TailSwipe, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite };
        public static readonly SpecialAbility[] SpecialAbilitySabreTri = new[] { SpecialAbility.SearingWounds, SpecialAbility.TailSwipe };
        public static readonly SpecialAbility[] SpecialAbilityGrizzledMare = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LifeLeech, SpecialAbility.LightningForce, SpecialAbility.ManaDrain, SpecialAbility.RagingBreath, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.StealLife, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite };
        public static readonly SpecialAbility[] SpecialAbilitySkeletalCat = new[] { SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe, SpecialAbility.ViciousBite, SpecialAbility.VenomousBite, SpecialAbility.LifeLeech };
        public static readonly SpecialAbility[] SpecialAbilityCoconutCrab = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.ManaDrain, SpecialAbility.RagingBreath, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite };
        public static readonly SpecialAbility[] SpecialAbilityPhoenix = new[] { SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.GraspingClaw, SpecialAbility.Inferno, SpecialAbility.LightningForce, SpecialAbility.ManaDrain, SpecialAbility.RagingBreath, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite };

        #endregion

        #region Area effect packages

        public static readonly AreaEffect[] AreaEffectNone = Array.Empty<AreaEffect>();
        public static readonly AreaEffect[] AreaEffectExplosiveGoo = new[] { AreaEffect.ExplosiveGoo };
        public static readonly AreaEffect[] AreaEffectEarthen = new[] { AreaEffect.EssenceOfEarth, AreaEffect.ExplosiveGoo };
        public static readonly AreaEffect[] AreaEffectDisease = new[] { AreaEffect.AuraOfNausea, AreaEffect.EssenceOfDisease };
        public static readonly AreaEffect[] AreaEffectArea1 = new[] { AreaEffect.EssenceOfEarth, AreaEffect.ExplosiveGoo, AreaEffect.AuraOfEnergy };
        public static readonly AreaEffect[] AreaEffectArea2 = new[] { AreaEffect.EssenceOfEarth, AreaEffect.ExplosiveGoo, AreaEffect.AuraOfEnergy, AreaEffect.AuraOfNausea, AreaEffect.EssenceOfDisease, AreaEffect.PoisonBreath };
        public static readonly AreaEffect[] AreaEffectArea3 = new[] { AreaEffect.AuraOfNausea, AreaEffect.EssenceOfDisease, AreaEffect.PoisonBreath };
        public static readonly AreaEffect[] AreaEffectArea4 = new[] { AreaEffect.AuraOfEnergy, AreaEffect.ExplosiveGoo, AreaEffect.AuraOfNausea, AreaEffect.PoisonBreath, AreaEffect.EssenceOfDisease };
        public static readonly AreaEffect[] AreaEffectArea5 = new[] { AreaEffect.ExplosiveGoo, AreaEffect.AuraOfNausea, AreaEffect.PoisonBreath, AreaEffect.EssenceOfDisease };

        #endregion

        #region Special move packages

        public static readonly WeaponAbility[] WepAbilityNone = Array.Empty<WeaponAbility>();
        public static readonly WeaponAbility[] WepAbility1 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility2 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility3 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility4 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility5 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility6 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Disarm, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility7 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility8 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility9 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.Block, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility10 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility11 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };
        public static readonly WeaponAbility[] WepAbility12 = new[] { WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike };

        #endregion

        #region Definitions

        public static TrainingDefinition[] Build()
        {
            var definitions = new TrainingDefinition[]
            {
                Def("VvVMount", Class.Clawed, MagicalAbility.None, SpecialAbilityClawed, WepAbilityNone, AreaEffectNone, 1, 3),
                Def("VvVMount", Class.None, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 3),
                Def("AlaskanMalamute", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("GreatDane", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("Newfoundland", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("Rottweiler", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("RussianTerrier", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("SaintBernard", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("DreadHound", Class.ClawedNecromanticAndTokuno, MagicalAbility.Wolf, SpecialAbilityClawedAndNecromantic, WepAbility1, AreaEffectNone, 3, 5),
                Def("BlackUnicorn", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 3, 5),
                Def("Capybara", Class.ClawedNecromanticAndTokuno, MagicalAbility.Wolf, SpecialAbilityClawedAndNecromantic, WepAbility1, AreaEffectNone, 3, 5),
                Def("MoonTail", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 3, 5),
                Def("JuvenileManticore", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("JuvenileUmbrascale", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("KurakuDread", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("UberTurkey", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("MolderingUrsine", Class.ClawedNecromanticAndTokuno, MagicalAbility.Wolf, SpecialAbilityClawedAndNecromantic, WepAbility1, AreaEffectNone, 3, 5),
                Def("DreadWolf", Class.ClawedNecromanticAndTokuno, MagicalAbility.Wolf, SpecialAbilityClawedAndNecromantic, WepAbility1, AreaEffectNone, 3, 5),
                Def("RainbowUnicorn", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 3, 5),
                Def("ShadowSpider", Class.MagicalNecromanticAndTokuno, MagicalAbility.DreadSpider, SpecialAbilityDreadSpider, WepAbility2, AreaEffectArea2, 3, 5),
                Def("BoneCat", Class.ClawedTailedAndNecromantic, MagicalAbility.Hellcat, SpecialAbilitySkeletalCat, WepAbility4, AreaEffectArea3, 3, 5),
                Def("SkeletalHorse", Class.ClawedTailedAndNecromantic, MagicalAbility.GrizzledMare, SpecialAbilityGrizzledMare, WepAbility2, AreaEffectArea4, 3, 5),
                Def("DreadSteed", Class.MagicalAndNecromantic, MagicalAbility.DreadWarhorse, new[] { SpecialAbility.DragonBreath }, WepAbility2, AreaEffectArea2, 3, 5),
                Def("FrostMiteMount", Class.Insectoid, MagicalAbility.Poisoning, SpecialAbilityMagicalInsectoid, WepAbility1, AreaEffectNone, 3, 5),
                Def("ClydesdaleHorse", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 1, 5),
                Def("ElementalAirHorse", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 1, 5),
                Def("ElementalEarthHorse", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 1, 5),
                Def("ElementalFireHorse", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 1, 5),
                Def("ElementalWaterHorse", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 1, 5),
                Def("FlameOfKindlehart", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 2, 5),
                Def("SpritelightForager", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 2, 5),
                Def("SwampDragon", Class.Untrainable, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 1),
                Def("WarClydesdale", Class.Untrainable, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 1),
                Def("WarOstard", Class.Untrainable, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 1),
                Def("WarUnicorn", Class.Untrainable, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 1),
                Def("CuSidhe", Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbilityClawedTailedAndMagical2, WepAbility5, AreaEffectArea1, 3, 5),
                Def("Triton", Class.None, MagicalAbility.Triton, SpecialAbilityTriton, WepAbility9, AreaEffectArea2, 2, 5),
                Def("Alligator", Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawedAndTailed, WepAbility1, AreaEffectNone, 1, 4),
                Def("BakeKitsune", Class.ClawedTailedAndTokuno, MagicalAbility.Tokuno1, SpecialAbilityClawedTailedAndMagical2, WepAbility2, AreaEffectArea1, 3, 5),
                Def("BaneDragon", Class.MagicalClawedAndTailed, MagicalAbility.Dragon1, SpecialAbilityBaneDragon, WepAbility2, AreaEffectArea2, 3, 5),
                Def("BattleChickenLizard", Class.Untrainable, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 1),
                Def("Bird", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 2),
                Def("BlackBear", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("BloodFox", Class.None, MagicalAbility.Poisoning, SpecialAbilityNone, WepAbility3, AreaEffectNone, 2, 3),
                Def("Boar", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("BrownBear", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("Bull", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1 ,4),
                Def("BullFrog", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("Cat", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility4, AreaEffectNone, 1, 3),
                Def("Chicken", Class.Clawed, MagicalAbility.None, SpecialAbilityClawed, WepAbilityNone, AreaEffectNone, 1, 2),
                Def("ChickenLizard", Class.Untrainable, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 1),
                Def("LeatherWolf", Class.None, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 1),
                Def("ColdDrake", Class.None, MagicalAbility.ColdDrake, SpecialAbilityNone, WepAbility2, new[] { AreaEffect.AuraOfEnergy, AreaEffect.ExplosiveGoo, AreaEffect.EssenceOfEarth }, 3, 5),
                Def("CorrosiveSlime", Class.StickySkin, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 3),
                Def("Cougar", Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawedAndTailed, WepAbility1, AreaEffectNone, 1, 3),
                Def("Cow", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("CrimsonDrake", Class.None, MagicalAbility.Dragon2, SpecialAbilityNone, WepAbility2, AreaEffectArea1, 2, 5), // CrimsonDrake[Poison] = AreaEffectArea2
                Def("DreadWarhorse", Class.MagicalAndNecromantic, MagicalAbility.DreadWarhorse, new[] { SpecialAbility.DragonBreath }, WepAbility2, AreaEffectArea2, 3, 5),
                Def("DeathwatchBeetle", Class.Insectoid, MagicalAbility.Poisoning, SpecialAbilityMagicalInsectoid, WepAbility5, AreaEffectNone, 1, 4),
                Def("DesertOstard", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("Dimetrosaur", Class.None, MagicalAbility.Poisoning, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 3, 5),
                Def("DireWolf", Class.ClawedNecromanticAndTokuno, MagicalAbility.Wolf, SpecialAbilityClawedAndNecromantic, WepAbility1, AreaEffectNone, 1, 4),
                Def("Dog", Class.Tailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityTailed, WepAbility1, AreaEffectNone, 1, 3),
                Def("Dragon", Class.MagicalClawedAndTailed, MagicalAbility.Dragon2, SpecialAbilityNone, WepAbility2, AreaEffectArea1, 4, 5),
                Def("DragonTurtleHatchling", Class.MagicalAndTailed, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 4, 5),
                Def("DragonWolf", Class.None, MagicalAbility.DragonWolf, SpecialAbilityNone, WepAbility1, AreaEffectNone, 4, 5),
                Def("Drake", Class.MagicalAndTailed, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 2, 5),
                Def("DreadSpider", Class.MagicalNecromanticAndTokuno, MagicalAbility.DreadSpider, SpecialAbilityDreadSpider, WepAbility2, AreaEffectArea2, 3, 5),
                Def("Eagle", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("Ferret", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 2),
                Def("FireBeetle", Class.MagicalAndInsectoid, MagicalAbility.StandardClawedOrTailed, SpecialAbilityMagicalInsectoid, WepAbility1, AreaEffectArea5, 1 ,5),
                Def("FireSteed", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityNone, WepAbility2, AreaEffectArea1, 2, 5),
                Def("ForestOstard", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("FrenziedOstard", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("FrostDragon", Class.None, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 5, 5),
                Def("FrostDrake", Class.None, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 3, 5),
                Def("FrostMite", Class.Insectoid, MagicalAbility.Poisoning, SpecialAbilityMagicalInsectoid, WepAbility1, AreaEffectNone, 2, 5),
                Def("FrostSpider", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility2, AreaEffectNone, 1, 4),
                Def("Gallusaurus", Class.None, MagicalAbility.Poisoning, SpecialAbilityNone, WepAbility1, AreaEffectNone, 3, 5),
                Def("Gaman", Class.Tailed, MagicalAbility.Poisoning, SpecialAbilityTailed, WepAbility1, AreaEffectNone, 1, 4),
                Def("Beetle", Class.Insectoid, MagicalAbility.StandardClawedOrTailed, SpecialAbilityMagicalInsectoid, WepAbility1, AreaEffectNone, 1, 5),
                Def("GiantIceWorm", Class.Tailed, MagicalAbility.Variety1, SpecialAbilityBitingTailed, WepAbility2, AreaEffectArea3, 1, 2),
                Def("GiantRat", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("GiantSpider", Class.None, MagicalAbility.Variety1, SpecialAbilityBitingAnimal, WepAbility1, AreaEffectArea3, 1, 3),
                Def("GiantToad", Class.StickySkin, MagicalAbility.StandardClawedOrTailed, SpecialAbilityStickySkin, WepAbility1, AreaEffectNone, 1, 3),
                Def("Goat", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("Gorilla", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("GreatHart", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("GreaterDragon", Class.MagicalClawedAndTailed, MagicalAbility.GreaterDragon, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 4, 5),
                Def("GreaterMongbat", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("GreyWolf", Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawedAndTailed, WepAbility1, AreaEffectNone, 1, 3),
                Def("GrizzlyBear", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("HellHound", Class.ClawedTailedNecromanticAndTokuno, MagicalAbility.Wolf, SpecialAbilityNone, WepAbility1, AreaEffectExplosiveGoo, 2, 4),
                Def("HellCat", Class.ClawedTailedAndNecromantic, MagicalAbility.Hellcat, SpecialAbilityNone, WepAbility1, AreaEffectExplosiveGoo, 2, 3),
                Def("HighPlainsBoura", Class.Tailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityNone, WepAbility1, AreaEffectNone, 2, 5),
                Def("Hind", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("Hiryu", Class.ClawedTailedMagicalAndTokuno, MagicalAbility.Hiryu, SpecialAbilityNone, WepAbility7, AreaEffectArea1, 3, 5),
                Def("Horse", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("Imp", Class.MagicalClawedTailedAndNecromantic, MagicalAbility.Variety2, SpecialAbilityImp, WepAbility2, AreaEffectArea2, 2, 4),
                Def("IronBeetle", Class.Insectoid, MagicalAbility.StandardClawedOrTailed, SpecialAbilityMagicalInsectoid, WepAbility1, AreaEffectNone, 2, 5),
                Def("JackRabbit", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 2),
                Def("Kirin", Class.MagicalClawedAndTailed, MagicalAbility.Dragon2, SpecialAbilityClawedTailedAndMagical2, WepAbility2, AreaEffectArea1, 2, 5),
                Def("Lasher", Class.Magical, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 2, 4),
                Def("LavaLizard", Class.ClawedTailedMagicalAndTokuno, MagicalAbility.LavaLizard, SpecialAbilityNone, WepAbility6, AreaEffectArea1, 1, 4),
                Def("LesserHiryu", Class.ClawedTailedMagicalAndTokuno, MagicalAbility.Tokuno1, SpecialAbilityNone, WepAbility7, AreaEffectArea1, 1, 5),
                Def("Lion", Class.ClawedAndTailed, MagicalAbility.Poisoning, SpecialAbilityBitingClawedAndTailed, WepAbility1, AreaEffectArea3, 2, 5),
                Def("Llama", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility9, AreaEffectNone, 1, 3),
                Def("LowlandBoura", Class.Tailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityTailed, WepAbility1, AreaEffectNone, 2, 3),
                Def("Mongbat", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 2),
                Def("MountainGoat", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("Najasaurus", Class.StickySkinAndTailed, MagicalAbility.Variety1, SpecialAbilityTailedAndStickySkin, WepAbility1, AreaEffectArea3, 2, 5),
                Def("Nightmare", Class.MagicalAndNecromantic, MagicalAbility.Variety2, SpecialAbilityNone, WepAbility2, AreaEffectArea1, 3, 5),
                Def("OsseinRam", Class.None, MagicalAbility.Variety3, new[] { SpecialAbility.LifeLeech }, WepAbility12, AreaEffectArea2, 2, 5),
                Def("PackHorse", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("PackLlama", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("Palomino", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("Panther", Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawedAndTailed, WepAbility1, AreaEffectNone, 1, 3),
                Def("Paralithode", Class.None, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 1),
                Def("ParoxysmusSwampDragon", Class.Untrainable, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 1),
                Def("Phoenix", Class.MagicalAndClawed, MagicalAbility.Dragon1, SpecialAbilityPhoenix, WepAbility1, AreaEffectArea2, 3, 5),
                Def("Pig", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("PlatinumDrake", Class.None, MagicalAbility.Dragon2, SpecialAbilityNone, WepAbility2, AreaEffectArea1,2, 5),
                Def("PolarBear", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("PredatorHellCat", Class.ClawedTailedAndNecromantic, MagicalAbility.Hellcat, SpecialAbilityNone, WepAbility1, AreaEffectExplosiveGoo, 2, 3),
                Def("Rabbit", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 2),
                Def("Raptor", Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawedAndTailed, WepAbility1, AreaEffectNone, 2, 5),
                Def("Rat", Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawedAndTailed, WepAbility1, AreaEffectNone, 1, 2),
                Def("Reptalon", Class.MagicalAndTailed, MagicalAbility.Cusidhe, SpecialAbilityNone, WepAbility10, AreaEffectArea2, 2, 5),
                Def("RidableLlama", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("Ridgeback", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("RuddyBoura", Class.Tailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityTailed, WepAbility1, AreaEffectNone, 2, 3),
                Def("RuneBeetle", Class.Insectoid, MagicalAbility.RuneBeetle, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 3, 5),
                Def("SabertoothedTiger", Class.ClawedAndTailed, MagicalAbility.SabreToothedTiger, SpecialAbilitySabreTri, WepAbility1, AreaEffectNone, 2, 5),
                Def("Saurosaurus", Class.Tailed, MagicalAbility.Poisoning, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 3, 5),
                Def("SavageRidgeback", Class.Clawed, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 3),
                Def("Scorpion", Class.Tailed, MagicalAbility.Variety1, SpecialAbilityBitingTailed, WepAbility1, AreaEffectArea3, 1, 3),
                Def("SerpentineDragon", Class.MagicalClawedAndTailed, MagicalAbility.Dragon2, SpecialAbilityNone, WepAbility2, AreaEffectArea2, 3, 5),
                Def("Sewerrat", Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawedAndTailed, WepAbility3, AreaEffectNone, 1, 2),
                Def("ShadowWyrm", Class.TailedAndNecromantic, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 5, 5),
                Def("Sheep", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 2),
                Def("SilverSteed", Class.None, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 2),
                Def("SkitteringHopper", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1 , 2),
                Def("Skree", Class.MagicalClawedAndTailed, MagicalAbility.Dragon2, SpecialAbilityClawedTailedAndMagical1, WepAbility2, AreaEffectArea1, 3, 5),
                Def("Slime", Class.StickySkin, MagicalAbility.Variety1, SpecialAbilityBitingStickySkin, WepAbility1, AreaEffectArea3, 1, 2),
                Def("Slith", Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityNone, WepAbility1, AreaEffectNone, 1, 4),
                Def("Snake", Class.Tailed, MagicalAbility.Variety1, SpecialAbilityBitingTailed, WepAbility1, AreaEffectArea3, 1, 2),
                Def("SnowLeopard", Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawedAndTailed, WepAbility1, AreaEffectNone, 1, 3),
                Def("Squirrel", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 2),
                Def("StoneSlith", Class.ClawedAndTailed, MagicalAbility.Poisoning, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 5),
                Def("StygianDrake", Class.MagicalClawedAndTailed, MagicalAbility.StygianDrake, SpecialAbilityClawedTailedAndMagical1, WepAbility2, AreaEffectArea1, 4, 5),
                Def("TimberWolf", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 3),
                Def("Triceratops", Class.Tailed, MagicalAbility.Triceratops, SpecialAbilitySabreTri, WepAbilityNone, AreaEffectNone, 3, 5),
                Def("TropicalBird", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 2),
                Def("TsukiWolf", Class.MagicalClawedTailedNecromanticAndTokuno, MagicalAbility.TsukiWolf, SpecialAbilityTsukiWolf, WepAbility2, AreaEffectArea1, 3, 5),
                Def("Turkey", Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawed, WepAbility1, AreaEffectNone, 1, 2),
                Def("Unicorn", Class.Magical, MagicalAbility.Dragon2, SpecialAbilityUnicorn, WepAbility11, AreaEffectArea1, 2, 5),
                Def("Vollem", Class.MagicalAndTailed, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 3, 5),
                Def("Walrus", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 3),
                Def("WhiteWolf", Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbilityClawedAndTailed, WepAbility1, AreaEffectNone, 1, 3),
                Def("WhiteWyrm", Class.MagicalClawedAndTailed, MagicalAbility.Dragon1, SpecialAbilityClawedTailedAndMagical2, WepAbility2, AreaEffectEarthen, 4, 5),
                Def("WildTiger", Class.ClawedAndTailed, MagicalAbility.Poisoning, SpecialAbilityNone, WepAbility3, AreaEffectNone, 2, 5),
                Def("WildWhiteTiger", Class.ClawedAndTailed, MagicalAbility.Poisoning, SpecialAbilityNone, WepAbility3, AreaEffectNone, 2, 5),
                Def("WildBlackTiger", Class.ClawedAndTailed, MagicalAbility.Poisoning, SpecialAbilityNone, WepAbility3, AreaEffectNone, 2, 5),
                Def("Windrunner", Class.TailedAndNecromantic, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 2, 4),
                Def("WolfSpider", Class.None, MagicalAbility.Vartiety, SpecialAbilityBitingAnimal, WepAbility1, AreaEffectDisease, 1, 3),
                Def("Eowmu", Class.Clawed, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 3),
                Def("TigerCub", Class.ClawedAndTailed, MagicalAbility.None, SpecialAbilityNone, WepAbilityNone, AreaEffectNone, 1, 3),
                Def("SkeletalCat", Class.ClawedTailedAndNecromantic, MagicalAbility.Hellcat, SpecialAbilitySkeletalCat, WepAbility4, AreaEffectArea3, 2, 3),
                Def("GrizzledMare", Class.ClawedTailedAndNecromantic, MagicalAbility.GrizzledMare, SpecialAbilityGrizzledMare, WepAbility2, AreaEffectArea4, 1, 3),
                Def("HungryCoconutCrab", Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbilityAnimalStandard, WepAbility1, AreaEffectNone, 1, 2),
                Def("CoconutCrab", Class.None, MagicalAbility.CoconutCrab, SpecialAbilityCoconutCrab, WepAbility2, AreaEffectArea2, 1, 3),
            };

            return definitions;
        }

        private static TrainingDefinition Def(
            string creatureType,
            Class classification,
            MagicalAbility magical,
            SpecialAbility[] specials,
            WeaponAbility[] weapons,
            AreaEffect[] areas,
            int controlMin,
            int controlMax)
        {
            var type = AssemblyHandler.FindTypeByName(creatureType, ignoreCase: true);

            if (type == null || !type.IsSubclassOf(typeof(BaseCreature)))
            {
                _missing.Add(creatureType);
                return null;
            }

            return new TrainingDefinition(type, classification, magical, specials, weapons, areas, controlMin, controlMax);
        }

        /// <summary>
        /// Creature types named by the definition table that this core does not have. The table
        /// omits them; this is the list to act on when a creature is added to the shard.
        /// </summary>
        public static string[] MissingTypes
        {
            get
            {
                _missing.Sort(StringComparer.OrdinalIgnoreCase);
                return _missing.ToArray();
            }
        }

        private static readonly List<string> _missing = new();

        #endregion
    }
}
