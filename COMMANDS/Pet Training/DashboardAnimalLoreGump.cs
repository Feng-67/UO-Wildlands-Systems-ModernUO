// UO Wildlands Custom Script
// Adapted for ModernUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)

using System;
using System.Collections.Generic;
using System.Text;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    // ==========================================================================================
    // ANIMAL LORE GUMP (Custom Dashboard Layout)
    // ==========================================================================================
    public class DashboardAnimalLoreGump : DynamicGump
    {

        // Color Constants
        private const int C_YELLOW = 2213; // Label Titles
        private const int C_WHITE = 2036;  // Values
        private const int C_RED = 1258;    // Fire / Special Moves
        private const int C_BLUE = 1264;   // Cold
        private const int C_GREEN = 1270;  // Poison
        private const int C_PURPLE = 1276; // Energy
        private const int C_GREY = 2401;   // Physical / Empty Slots

        private readonly PlayerMobile _from;
        private readonly BaseCreature _creature;

        public override bool Singleton => true;

        private DashboardAnimalLoreGump(PlayerMobile pm, BaseCreature bc) : base(50, 50)
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

            pm.SendGump(new DashboardAnimalLoreGump(pm, bc));
        }

        // Re-sends the gump with fresh data. DynamicGump rebuilds its whole layout on every
        // send, so this is the equivalent of the original BaseGump wrapper's Refresh() —
        // kept as an instance method (rather than inlining DisplayTo everywhere) because
        // PetTrainingGate.cs calls gump.Refresh() on a FindGump<DashboardAnimalLoreGump>() result.
        public void Refresh() => DisplayTo(_from, _creature);

        protected override void BuildLayout(ref DynamicGumpBuilder builder)
        {
            var c = _creature;

            const int width = 640;
            const int height = 850;

            builder.AddPage();

            // Main Dark Background
            builder.AddBackground(0, 0, width, height, 9200);
            builder.AddImageTiled(10, 10, width - 20, height - 20, 2624);
            builder.AddAlphaRegion(10, 10, width - 20, height - 20);

            // ------------------------------------------------------------------------
            // HEADER SECTION
            // ------------------------------------------------------------------------
            // Icon Frame (Top Left)
            //   builder.AddBackground(20, 20, 110, 110, 9270);

            // Draw the actual pet icon
            //   int petItemID = ShrinkTable.Lookup(c);
            //   builder.AddItem(35, 35, petItemID);

            var name = c.Name;

            // Check if the creature is flagged as Legendary via the interface
            //if (c is ILegendaryPet { IsLegendary: true })
            //{
            //    name += " <BASEFONT COLOR=#FFD700>[Legendary]</BASEFONT>";
            //}

            if (c.IsBonded)
            {
                name += " [Bonded]";
            }

            builder.AddHtml(140, 25, 360, 25, ColorAndCenter("#FFCC00", name));

            // Top Right Buttons
            builder.AddButton(width - 75, 21, 4005, 4007, 5, GumpButtonType.Reply, 0); // Refresh
            builder.AddButton(width - 40, 20, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0); // Close

            // ------------------------------------------------------------------------
            // ATTRIBUTES PANEL
            // ------------------------------------------------------------------------
            const int attrX = 30;   // Matches c1X
            const int attrY = 55;
            const int attrCol2 = 340; // Matches c2X

            builder.AddLabel(attrX, attrY, C_YELLOW, "Attributes");

            // Helper strings to calculate alignment
            var sStr = c.Str.ToString();
            var sDex = c.Dex.ToString();
            var sInt = c.Int.ToString();

            var sHits = $"{c.Hits} / {c.HitsMax}";
            var sStam = $"{c.Stam} / {c.StamMax}";
            var sMana = $"{c.Mana} / {c.ManaMax}";

            // Left Column: Attributes
            builder.AddLabel(attrX, attrY + 25, C_WHITE, "Strength");
            builder.AddLabel(attrX + 240 - GetLabelWidth(sStr), attrY + 25, C_WHITE, sStr);

            builder.AddLabel(attrX, attrY + 50, C_WHITE, "Dexterity");
            builder.AddLabel(attrX + 240 - GetLabelWidth(sDex), attrY + 50, C_WHITE, sDex);

            builder.AddLabel(attrX, attrY + 75, C_WHITE, "Intelligence");
            builder.AddLabel(attrX + 240 - GetLabelWidth(sInt), attrY + 75, C_WHITE, sInt);

            // Right Column: Stats
            builder.AddLabel(attrCol2, attrY + 25, C_WHITE, "Hit Points");
            builder.AddLabel(attrCol2 + 240 - GetLabelWidth(sHits), attrY + 25, C_WHITE, sHits);

            builder.AddLabel(attrCol2, attrY + 50, C_WHITE, "Stamina");
            builder.AddLabel(attrCol2 + 240 - GetLabelWidth(sStam), attrY + 50, C_WHITE, sStam);

            builder.AddLabel(attrCol2, attrY + 75, C_WHITE, "Mana");
            builder.AddLabel(attrCol2 + 240 - GetLabelWidth(sMana), attrY + 75, C_WHITE, sMana);

            // ------------------------------------------------------------------------
            // RESISTANCES & DAMAGE TYPES
            // ------------------------------------------------------------------------
            const int resY = 175;
            DrawDivider(ref builder, 20, resY - 7, width - 40);
            builder.AddLabel(30, resY + 5, C_YELLOW, "Resistances");

            DrawResistBox(ref builder, 120, resY + 5, "Physical", c.PhysicalResistance, C_GREY);
            DrawResistBox(ref builder, 220, resY + 5, "Fire", c.FireResistance, C_RED);
            DrawResistBox(ref builder, 320, resY + 5, "Cold", c.ColdResistance, C_BLUE);
            DrawResistBox(ref builder, 420, resY + 5, "Poison", c.PoisonResistance, C_GREEN);
            DrawResistBox(ref builder, 520, resY + 5, "Energy", c.EnergyResistance, C_PURPLE);

            const int damY = 210;
            DrawDivider(ref builder, 20, damY - 5, width - 40);
            builder.AddLabel(30, damY + 5, C_YELLOW, "Damage Type");

            DrawDamageBox(ref builder, 120, damY + 5, "Physical", c.PhysicalDamage, C_GREY);
            DrawDamageBox(ref builder, 220, damY + 5, "Fire", c.FireDamage, C_RED);
            DrawDamageBox(ref builder, 320, damY + 5, "Cold", c.ColdDamage, C_BLUE);
            DrawDamageBox(ref builder, 420, damY + 5, "Poison", c.PoisonDamage, C_GREEN);
            DrawDamageBox(ref builder, 520, damY + 5, "Energy", c.EnergyDamage, C_PURPLE);

            // ------------------------------------------------------------------------
            // SKILLS COLUMNS
            // ------------------------------------------------------------------------
            const int colY = 255;
            DrawDivider(ref builder, 20, colY - 10, width - 40);

            const int c1X = 30;
            builder.AddHtml(c1X, colY, 250, 18, "<CENTER><BASEFONT COLOR=#FFD700>Combat Ratings</BASEFONT></CENTER>");
            var curY = colY + 30;

            DrawSkill(ref builder, c, c1X, curY, "Wrestling", SkillName.Wrestling); curY += 20;
            DrawSkill(ref builder, c, c1X, curY, "Tactics", SkillName.Tactics); curY += 20;
            DrawSkill(ref builder, c, c1X, curY, "Resisting Spells", SkillName.MagicResist); curY += 20;
            DrawSkill(ref builder, c, c1X, curY, "Anatomy", SkillName.Anatomy); curY += 20;
            DrawSkill(ref builder, c, c1X, curY, "Healing", SkillName.Healing); curY += 20;
            DrawSkill(ref builder, c, c1X, curY, "Poisoning", SkillName.Poisoning); curY += 20;
            DrawSkill(ref builder, c, c1X, curY, "Detecting Hidden", SkillName.DetectHidden); curY += 20;
            DrawSkill(ref builder, c, c1X, curY, "Hiding", SkillName.Hiding); curY += 20;
            DrawSkill(ref builder, c, c1X, curY, "Parrying", SkillName.Parry); curY += 20;

            const int c2X = 340;
            builder.AddHtml(c2X, colY, 250, 18, "<CENTER><BASEFONT COLOR=#FFD700>Lore & Knowledge</BASEFONT></CENTER>");
            curY = colY + 30;

            DrawSkill(ref builder, c, c2X, curY, "Magery", SkillName.Magery); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Eval Intelligence", SkillName.EvalInt); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Meditation", SkillName.Meditation); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Necromancy", SkillName.Necromancy); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Spirit Speak", SkillName.SpiritSpeak); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Mysticism", SkillName.Mysticism); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Focus", SkillName.Focus); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Spellweaving", SkillName.Spellweaving); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Discordance", SkillName.Discordance); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Bushido", SkillName.Bushido); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Ninjitsu", SkillName.Ninjitsu); curY += 20;
            DrawSkill(ref builder, c, c2X, curY, "Chivalry", SkillName.Chivalry); curY += 20;

            // ------------------------------------------------------------------------
            // REGENERATION & SPECIAL ABILITIES
            // ------------------------------------------------------------------------
            var regenY = colY + 235;
            //     DrawDivider(ref builder, 20, regenY - 5, (width / 2) - 40);

            builder.AddHtml(c1X, regenY, 250, 18, "<CENTER><BASEFONT COLOR=#FFD700>Regeneration</BASEFONT></CENTER>");
            var profile = PetTrainingHelper.GetAbilityProfile(c, true);

            builder.AddLabel(c1X, regenY + 25, C_WHITE, "Hit Point Regeneration");
            builder.AddLabel(c1X + 220, regenY + 25, C_WHITE, profile.RegenHits.ToString());

            builder.AddLabel(c1X, regenY + 45, C_WHITE, "Stamina Regeneration");
            builder.AddLabel(c1X + 220, regenY + 45, C_WHITE, profile.RegenStam.ToString());

            builder.AddLabel(c1X, regenY + 65, C_WHITE, "Mana Regeneration");
            builder.AddLabel(c1X + 220, regenY + 65, C_WHITE, profile.RegenMana.ToString());

            // --- SPECIAL ABILITIES HEADING (RIGHT COLUMN - ALIGNED) ---
            // Changed + 36 to + 65 to match Mana Regeneration exactly
            builder.AddHtml(c2X, regenY + 65, 250, 18, "<CENTER><BASEFONT COLOR=#FFD700>Special Abilities</BASEFONT></CENTER>");

            // --- ABILITIES LIST STARTING POINT ---
            var moveY = regenY + 70;

            // --- CENTERED SPECIAL ABILITIES (Combined Innate + Trained) ---
            var displayedAbils = new List<string>();

            // 0. Magical Ability (school)
            if (profile.MagicalAbility != MagicalAbility.None)
            {
                foreach (var abil in PetTrainingHelper.MagicalAbilities)
                {
                    if ((profile.MagicalAbility & abil) != 0)
                    {
                        var loc = PetTrainingHelper.GetLocalization(abil);
                        var displayName = loc is { Length: > 0 } && loc[0] != null && !string.IsNullOrEmpty(loc[0].String)
                            ? loc[0].String
                            : FormatAbilityName(abil.ToString());

                        moveY += 18;
                        builder.AddHtml(c2X, moveY, 250, 18, $"<CENTER><BASEFONT COLOR=#FFFFFF>{displayName}</BASEFONT></CENTER>");
                    }
                }
            }

            if (profile.SpecialAbilities != null)
            {
                foreach (var abil in profile.SpecialAbilities)
                {
                    var typeKey = abil.GetType().Name;

                    if (displayedAbils.Contains(typeKey))
                    {
                        continue;
                    }

                    if (typeKey is "ColossalRage" or "Heal")
                    {
                        continue;
                    }

                    var loc = PetTrainingHelper.GetLocalization(abil);
                    var displayName = loc is { Length: > 0 } && loc[0] != null && !string.IsNullOrEmpty(loc[0].String)
                        ? loc[0].String
                        : FormatAbilityName(typeKey);

                    // This now starts exactly 1 line (18px) below the "Special Abilities" header
                    moveY += 18;
                    builder.AddHtml(c2X, moveY, 250, 18, $"<CENTER><BASEFONT COLOR=#FFFFFF>{displayName}</BASEFONT></CENTER>");
                    displayedAbils.Add(typeKey);
                }
            }

            // 2. Weapon Abilities stored in the profile (set via SetWeaponAbility)
            if (profile.WeaponAbilities != null)
            {
                foreach (var abil in profile.WeaponAbilities)
                {
                    var typeKey = abil.GetType().Name;

                    if (displayedAbils.Contains(typeKey))
                    {
                        continue;
                    }

                    // GetLocalization has no WeaponAbility overload in most builds,
                    // so always fall back to the formatted type name.
                    moveY += 18;
                    builder.AddHtml(c2X, moveY, 250, 18, $"<CENTER><BASEFONT COLOR=#FFFFFF>{FormatAbilityName(typeKey)}</BASEFONT></CENTER>");
                    displayedAbils.Add(typeKey);
                }
            }

            // 3. Fallback: catch weapon ability returned only via GetWeaponAbility() override
            //    (i.e. a creature that overrides the method without calling SetWeaponAbility)
            var overrideWa = c.GetWeaponAbility();

            if (overrideWa != null)
            {
                var typeKey = overrideWa.GetType().Name;

                if (!displayedAbils.Contains(typeKey))
                {
                    moveY += 18;
                    builder.AddHtml(c2X, moveY, 250, 18, $"<CENTER><BASEFONT COLOR=#FFFFFF>{FormatAbilityName(typeKey)}</BASEFONT></CENTER>");
                    displayedAbils.Add(typeKey);
                }
            }

            // 4. Area Effects
            if (profile.AreaEffects != null)
            {
                foreach (var abil in profile.AreaEffects)
                {
                    var typeKey = abil.GetType().Name;

                    if (displayedAbils.Contains(typeKey))
                    {
                        continue;
                    }

                    var loc = PetTrainingHelper.GetLocalization(abil);
                    var displayName = loc is { Length: > 0 } && loc[0] != null && !string.IsNullOrEmpty(loc[0].String)
                        ? loc[0].String
                        : FormatAbilityName(typeKey);

                    moveY += 18;
                    builder.AddHtml(c2X, moveY, 250, 18, $"<CENTER><BASEFONT COLOR=#FFFFFF>{displayName}</BASEFONT></CENTER>");
                    displayedAbils.Add(typeKey);
                }
            }

            // ------------------------------------------------------------------------
            // MISCELLANEOUS
            // ------------------------------------------------------------------------
            var miscY = regenY + 130;
            builder.AddHtml(c1X, miscY, 250, 18, "<CENTER><BASEFONT COLOR=#FFD700>Miscellaneous</BASEFONT></CENTER>");

            builder.AddLabel(c1X, miscY + 25, C_WHITE, "Loyalty Rating");
            builder.AddLabel(c1X + 140, miscY + 25, C_WHITE, GetHappinessString(c));

            builder.AddLabel(c1X, miscY + 45, C_WHITE, "Preferred Foods");
            builder.AddLabel(c1X + 140, miscY + 45, C_WHITE, GetFoodString(c.FavoriteFood));

            builder.AddLabel(c1X, miscY + 65, C_WHITE, "Pack Instincts");
            builder.AddLabel(c1X + 140, miscY + 65, C_WHITE, c.PackInstinct.ToString());

            builder.AddLabel(c1X, miscY + 85, C_WHITE, "Base Damage");
            builder.AddLabel(c1X + 140, miscY + 85, C_WHITE, $"{c.DamageMin} - {c.DamageMax}");

            builder.AddLabel(c1X, miscY + 105, C_WHITE, "Pet Slots");

            var slotText = c is BaseHire
                ? $"{c.ControlSlots} (Fully Trained)"
                : $"{c.ControlSlots} => {c.ControlSlotsMax} (Req. {c.MinTameSkill:F1})";

            builder.AddLabel(c1X + 140, miscY + 105, C_WHITE, slotText);

            // Slayer vulnerabilities — appended to Miscellaneous
            GetSlayerInfo(c, out var superSlayers, out var lesserSlayers);

            var allSlayers = new List<string>();

            foreach (var s in superSlayers)
            {
                allSlayers.Add($"{s} (Super)");
            }

            foreach (var s in lesserSlayers)
            {
                allSlayers.Add(s);
            }

            builder.AddLabel(c1X, miscY + 125, C_WHITE, "Slayer Vulnerabilities");

            if (allSlayers.Count == 0)
            {
                builder.AddLabel(c1X + 140, miscY + 125, C_WHITE, "None");
            }
            else
            {
                // First slayer on the same row as the label; subsequent ones below it,
                // all still aligned to the second-column X position.
                builder.AddLabel(c1X + 140, miscY + 125, C_WHITE, allSlayers[0]);

                for (var i = 1; i < allSlayers.Count; i++)
                {
                    builder.AddLabel(c1X + 140, miscY + 125 + i * 20, C_WHITE, allSlayers[i]);
                }
            }

            // ------------------------------------------------------------------------
            // FOOTER & TRAINING
            // ------------------------------------------------------------------------
            var footerY = height - 50;
            DrawDivider(ref builder, 20, footerY - 10, width - 40);

            if (c.Controlled && c.ControlMaster == _from)
            {
                var trainProfile = PetTrainingHelper.GetTrainingProfile(c, true);
                var def = PetTrainingHelper.GetTrainingDefinition(c);

                // Use robust logic from original gump to check if training is possible/begun
                if (trainProfile.HasBegunTraining && def != null && def.Class != Class.Untrainable)
                {
                    builder.AddLabel(40, footerY + 10, C_YELLOW, "Training Progress:");
                    builder.AddImage(170, footerY + 15, 0x805);

                    var progress = trainProfile.TrainingProgressPercentile * 100;

                    if (progress >= 1)
                    {
                        builder.AddBackground(170, footerY + 15, (int)(109.0 * (progress / 100)), 11, 0x806);
                    }

                    builder.AddLabel(290, footerY + 10, C_WHITE, $"{progress:F1}%");

                    // Show options if 100% progress
                    if (trainProfile.CanApplyOptions)
                    {
                        builder.AddButton(400, footerY + 10, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0); // Options
                        builder.AddLabel(430, footerY + 10, C_WHITE, "Options");
                    }

                    // Cancel button
                    builder.AddButton(600, footerY + 12, 0xFB1, 0xFB3, 3, GumpButtonType.Reply, 0);
                }
                else if (!(c is BaseHire) && c.ControlSlots < c.ControlSlotsMax)
                {
                    builder.AddButton(40, footerY + 10, 4005, 4007, 4, GumpButtonType.Reply, 0);
                    builder.AddLabel(75, footerY + 10, C_YELLOW, "Begin Animal Training");
                }
                else
                {
                    builder.AddLabel(40, footerY + 10, 0x35, "This pet is fully trained.");
                }
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (_creature == null || _creature.Deleted)
            {
                return;
            }

            switch (info.ButtonID)
            {
                case 2: // Options
                    {
                        if (_from.HasGump<PetTrainingConfirmationGump>() ||
                            _from.HasGump<PetTrainingOptionsGump>() ||
                            _from.HasGump<PetTrainingPlanningGump>() ||
                            _from.HasGump<PetTrainingConfirmGump>())
                        {
                            Refresh();
                            break;
                        }

                        var trainProfile = PetTrainingHelper.GetTrainingProfile(_creature, true);

                        if (trainProfile.HasBegunTraining)
                        {
                            if (!trainProfile.HasRecievedControlSlotWarning)
                            {
                                trainProfile.HasRecievedControlSlotWarning = true;

                                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                                {
                                    // TODO: PetTrainingConfirmGump not yet ported — assumes a static
                                    // DisplayTo(Mobile from, int headerCliloc, int bodyCliloc, Action onConfirm).
                                    PetTrainingConfirmGump.DisplayTo(_from, 1157571, 1157572, () =>
                                    {
                                        Refresh();
                                        _from.CloseGump<PetTrainingOptionsGump>();
                                        // TODO: PetTrainingOptionsGump not yet ported — assumes a static
                                        // DisplayTo(Mobile from, BaseCreature creature).
                                        PetTrainingOptionsGump.DisplayTo(_from, _creature);
                                    });
                                });
                            }
                            else
                            {
                                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                                {
                                    Refresh();
                                    _from.CloseGump<PetTrainingOptionsGump>();
                                    PetTrainingOptionsGump.DisplayTo(_from, _creature);
                                });
                            }
                        }

                        break;
                    }

                case 3: // Cancel Training
                    PetTrainingConfirmGump.DisplayTo(_from, 1153093, 1158019, () =>
                    {
                        var tp = PetTrainingHelper.GetTrainingProfile(_creature, true);
                        tp?.EndTraining();
                    });
                    break;

                case 4: // Begin Training
                    PetTrainingHelper.GetTrainingProfile(_creature, true).BeginTraining();
                    Refresh();
                    // Important: Trigger the quest check that original Gumps.cs has
                    //Server.Engines.Quests.UsingAnimalLoreQuest.CheckComplete(_from);
                    break;

                case 5: // Refresh
                    Refresh();
                    break;
            }
        }

        // ==========================================================================================
        // DYNAMIC SHRINK TABLE LOOKUP
        // ==========================================================================================
        public class ShrinkTable
        {
            public static int Lookup(BaseCreature bc)
            {
                if (bc == null)
                {
                    return 0x20D1;
                }

                var body = bc.Body.BodyID;

                if (body == 178 || body == 179)
                {
                    return 0x2121; // Nightmare Statue
                }

                if (body == 210 || body == 211)
                {
                    return 0x211F; // Bake Kitsune
                }

                if (body == 0x317)
                {
                    return 0x2615; // Beetle
                }

                // Fallback to a generic pet icon if a specific statue isn't mapped
                return 0x20D1;
            }
        }

        private static string ColorAndCenter(string color, string text) =>
            $"<center><basefont color={color}>{text}</center>";

        private static void DrawResistBox(ref DynamicGumpBuilder builder, int x, int y, string name, int val, int color)
        {
            builder.AddLabel(x, y, C_WHITE, name);
            builder.AddLabel(x + 55, y, color, $"{val}%");
        }

        private static void DrawDamageBox(ref DynamicGumpBuilder builder, int x, int y, string name, int val, int color)
        {
            builder.AddLabel(x, y, C_WHITE, name);
            builder.AddLabel(x + 55, y, val > 0 ? color : C_GREY, val > 0 ? $"{val}%" : "---");
        }

        private static void DrawSkill(ref DynamicGumpBuilder builder, BaseCreature creature, int x, int y, string name, SkillName sk)
        {
            var skill = creature.Skills[sk];
            builder.AddLabel(x, y, C_WHITE, name);

            var val = skill.Base > 0 ? $"{skill.Value:F1} / {skill.Cap:F1}" : "---";

            // This forces the value to be right-aligned based on its exact pixel width
            builder.AddLabel(x + 240 - GetLabelWidth(val), y, C_WHITE, val);
        }

        private static int GetLabelWidth(string text)
        {
            var width = 0;

            foreach (var c in text)
            {
                width += c switch
                {
                    '1' => 4, // 3px + 1px spacing
                    '.' => 3, // 2px + 1px spacing
                    ' ' => 4, // 3px + 1px spacing
                    '-' => 6, // 5px + 1px spacing
                    '/' => 7, // 6px + 1px spacing
                    _ => 8  // Most numbers and letters are 7px + 1px spacing
                };
            }

            return width;
        }

        private static void DrawDivider(ref DynamicGumpBuilder builder, int x, int y, int width)
        {
            builder.AddImageTiled(x, y, width, 2, 96);
        }

        // Converts a PascalCase type name (e.g. "ArmorIgnore", "LifeLeech") into a
        // human-readable label ("Armor Ignore", "Life Leech") for abilities whose
        // localization data is missing or empty.
        private static string FormatAbilityName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return typeName;
            }

            var sb = new StringBuilder();

            for (var i = 0; i < typeName.Length; i++)
            {
                if (i > 0 && char.IsUpper(typeName[i]))
                {
                    sb.Append(' ');
                }

                sb.Append(typeName[i]);
            }

            return sb.ToString();
        }

        private static string GetHappinessString(BaseCreature bc) =>
            bc.Loyalty switch
            {
                >= 100 => "Wonderfully Happy",
                >= 80 => "Extremely Happy",
                >= 60 => "Very Happy",
                >= 40 => "Rather Happy",
                >= 20 => "Happy",
                _ => "Confused"
            };

        private static string GetFoodString(FoodType food)
        {
            if (food == FoodType.None)
            {
                return "None";
            }

            var foods = new List<string>();

            if ((food & FoodType.Meat) != 0)
            {
                foods.Add("Meat");
            }

            if ((food & FoodType.FruitsAndVeggies) != 0)
            {
                foods.Add("Fruits/Veg");
            }

            if ((food & FoodType.GrainsAndHay) != 0)
            {
                foods.Add("Grains");
            }

            if ((food & FoodType.Fish) != 0)
            {
                foods.Add("Fish");
            }

            if ((food & FoodType.Eggs) != 0)
            {
                foods.Add("Eggs");
            }

            if ((food & FoodType.Gold) != 0)
            {
                foods.Add("Gold");
            }

            return foods.Count == 0 ? "Unknown" : string.Join(", ", foods);
        }

        // Collects the super slayer(s) and lesser slayer(s) that affect the given creature.
        // Reads from SlayerGroup.Groups — see SlayerGroup.cs for the definitions.
        private static void GetSlayerInfo(BaseCreature bc, out List<string> superSlayers, out List<string> lesserSlayers)
        {
            superSlayers = new List<string>();
            lesserSlayers = new List<string>();

            if (bc == null)
            {
                return;
            }

            foreach (var group in SlayerGroup.Groups)
            {
                if (group.Super != null && group.Super.Slays(bc))
                {
                    var formatted = FormatAbilityName(group.Super.Name.ToString());

                    if (!superSlayers.Contains(formatted))
                    {
                        superSlayers.Add(formatted);
                    }
                }

                if (group.Entries != null)
                {
                    foreach (var entry in group.Entries)
                    {
                        if (!entry.Slays(bc))
                        {
                            continue;
                        }

                        var formatted = FormatAbilityName(entry.Name.ToString());

                        if (!lesserSlayers.Contains(formatted))
                        {
                            lesserSlayers.Add(formatted);
                        }
                    }
                }
            }
        }

        // ==========================================================================================
        // GUMP FORMATTING HELPERS (shared by PetTrainingGumps and DashboardAnimalLoreGump)
        // ==========================================================================================
        internal static class GumpFormattingHelpers
        {
            public const int CenterLoc = 1154645;
            public const int AlignRightLoc = 1114514;

            public static int C16232(int c16)
            {
                c16 &= 0x7FFF;

                var r = (c16 >> 10 & 0x1F) << 3;
                var g = (c16 >> 05 & 0x1F) << 3;
                var b = (c16 >> 00 & 0x1F) << 3;

                return (r << 16) | (g << 8) | (b << 0);
            }

            public static int C16216(int c16) => c16 & 0x7FFF;

            public static int C32216(int c32)
            {
                c32 &= 0xFFFFFF;

                var r = (c32 >> 16 & 0xFF) >> 3;
                var g = (c32 >> 08 & 0xFF) >> 3;
                var b = (c32 >> 00 & 0xFF) >> 3;

                return (r << 10) | (g << 5) | (b << 0);
            }

            public static string Color(string color, string str) => $"<basefont color={color}>{str}";
            public static string Color(int color, string str) => $"<basefont color=#{color:X6}>{str}";
            public static string ColorAndCenter(string color, string str) => $"<center><basefont color={color}>{str}</center>";
            public static string ColorAndCenter(int color, string str) => $"<basefont color=#{color:X6}><center>{str}</center>";
            public static string ColorAndSize(string color, int size, string str) => $"<basefont color={color} size={size}>{str}</basefont>";
            public static string ColorAndCenterAndSize(string color, int size, string str) =>
                $"<basefont color={color} size={size}><center>{str}</center>";
            public static string Center(string str) => $"<CENTER>{str}</CENTER>";
            public static string ColorAndAlignRight(int color, string str) =>
                $"<DIV ALIGN=RIGHT><basefont color=#{color:X6}>{str}</DIV>";
            public static string ColorAndAlignRight(string color, string str) =>
                $"<DIV ALIGN=RIGHT><basefont color={color}>{str}</DIV>";
            public static string AlignRight(string str) => $"<DIV ALIGN=RIGHT>{str}</DIV>";
        }
    }
}
