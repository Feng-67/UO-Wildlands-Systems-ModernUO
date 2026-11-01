using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
//using ShrinkSystem;

namespace Server.SkillHandlers;

public static class AnimalLore
{
    public static void Initialize()
    {
        SkillInfo.Table[(int)SkillName.AnimalLore].Callback = OnUse;
    }

    public static TimeSpan OnUse(Mobile m)
    {
        if (PetTrainingHelper.Enabled && m.HasGump<DashboardAnimalLoreGump>())
        {
            m.SendLocalizedMessage(500118); // You must wait a few moments to use another skill.
        }
        else
        {
            m.Target = new InternalTarget();
            m.SendLocalizedMessage(500328); // What animal should I look at?
        }

        return TimeSpan.FromSeconds(1.0);
    }

    private class InternalTarget : Target
    {
        public InternalTarget() : base(8, false, TargetFlags.None) { }

        private static void SendGump(Mobile from, BaseCreature c)
        {
            from.CheckTargetSkill(SkillName.AnimalLore, c, 0.0, 120.0);

            if (PetTrainingHelper.Enabled && from is PlayerMobile pm)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1), () => DashboardAnimalLoreGump.DisplayTo(pm, c));
            }
            else
            {
                AnimalLoreGump.DisplayTo(from, c);
            }
        }

        private static void Check(Mobile from, BaseCreature c, double min)
        {
            if (from.CheckTargetSkill(SkillName.AnimalLore, c, min, 120.0)) SendGump(from, c);
            else from.SendLocalizedMessage(500334); // You can't think of anything you know offhand.
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive)
            {
                from.SendLocalizedMessage(500331);
                return;
            }

            if (targeted is not (BaseCreature or IShrinkItem))
            {
                from.SendLocalizedMessage(500329);
                return;
            }

            BaseCreature c = targeted switch
            {
                BaseCreature bc => bc,
                IShrinkItem si => si.ShrunkenPet,
                _ => null
            };

            if (c == null || c.IsDeadPet)
            {
                from.SendLocalizedMessage(500331);
                return;
            }

            if (!(c.Body.IsAnimal || c.Body.IsMonster || c.Body.IsSea || c is BaseHire || c is BaseMount))
            {
                from.SendLocalizedMessage(500329);
                return;
            }

            var skill = from.Skills[SkillName.AnimalLore].Value;

            if (skill < 100.0)
            {
                if (c.Controlled) SendGump(from, c);
                else from.SendLocalizedMessage(1049674);
            }
            else if (skill < 110.0)
            {
                if (c.Controlled) SendGump(from, c);
                else if (c.Tamable) Check(from, c, 80.0);
                else from.SendLocalizedMessage(1049675);
            }
            else
            {
                if (c.Controlled) SendGump(from, c);
                else if (c.Tamable) Check(from, c, 80.0);
                else Check(from, c, 100.0);
            }
        }
    }
}

// Vanilla fallback gump - moved here from ModernUO's AnimalLore.cs
public class AnimalLoreGump : DynamicGump
{
    private const int LabelColor = 0x24E5;
    private readonly BaseCreature _creature;

    public override bool Singleton => true;

    private AnimalLoreGump(BaseCreature c) : base(250, 50) => _creature = c;

    public static void DisplayTo(Mobile from, BaseCreature c)
    {
        if (from?.NetState == null || c == null || c.Deleted) return;
        from.SendGump(new AnimalLoreGump(c));
    }

    public static string FormatSkill(BaseCreature c, SkillName name)
    {
        var skill = c.Skills[name];
        return skill.Base < 10.0 ? "<div align=right>---</div>" : $"<div align=right>{skill.Value:F1}</div>";
    }

    public static string FormatAttributes(int cur, int max) =>
        max == 0 ? "<div align=right>---</div>" : $"<div align=right>{cur}/{max}</div>";

    public static string FormatStat(int val) =>
        val == 0 ? "<div align=right>---</div>" : $"<div align=right>{val}</div>";

    public static string FormatDouble(double val) =>
        val == 0 ? "<div align=right>---</div>" : $"<div align=right>{val:F1}</div>";

    public static string FormatElement(int val, string color = null)
    {
        if (color == null)
            return val == 0 ? "<div align=right>---</div>" : $"<div align=right>{val}%</div>";

        return val == 0
            ? $"<BASEFONT COLOR={color}><div align=right>---</div>"
            : $"<BASEFONT COLOR={color}><div align=right>{val}%</div>";
    }

    public static string FormatDamage(int min, int max) =>
        min <= 0 || max <= 0 ? "<div align=right>---</div>" : $"<div align=right>{min}-{max}</div>";

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        var c = _creature;

        builder.AddPage(0);

        builder.AddImage(100, 100, 2080);
        builder.AddImage(118, 137, 2081);
        builder.AddImage(118, 207, 2081);
        builder.AddImage(118, 277, 2081);
        builder.AddImage(118, 347, 2083);
        builder.AddHtml(147, 108, 210, 18, $"<center><i>{c.Name}</i></center>");
        builder.AddButton(240, 77, 2093, 2093, 2);
        builder.AddImage(140, 138, 2091);
        builder.AddImage(140, 335, 2091);

        var pages = Core.AOS ? 5 : 3;
        var page = 0;

        builder.AddPage(++page);
        builder.AddImage(128, 152, 2086);
        builder.AddHtmlLocalized(147, 150, 160, 18, 1049593, 200);

        builder.AddHtmlLocalized(153, 168, 160, 18, 1049578, LabelColor);
        builder.AddHtml(280, 168, 75, 18, FormatAttributes(c.Hits, c.HitsMax));
        builder.AddHtmlLocalized(153, 186, 160, 18, 1049579, LabelColor);
        builder.AddHtml(280, 186, 75, 18, FormatAttributes(c.Stam, c.StamMax));
        builder.AddHtmlLocalized(153, 204, 160, 18, 1049580, LabelColor);
        builder.AddHtml(280, 204, 75, 18, FormatAttributes(c.Mana, c.ManaMax));
        builder.AddHtmlLocalized(153, 222, 160, 18, 1028335, LabelColor);
        builder.AddHtml(320, 222, 35, 18, FormatStat(c.Str));
        builder.AddHtmlLocalized(153, 240, 160, 18, 3000113, LabelColor);
        builder.AddHtml(320, 240, 35, 18, FormatStat(c.Dex));
        builder.AddHtmlLocalized(153, 258, 160, 18, 3000112, LabelColor);
        builder.AddHtml(320, 258, 35, 18, FormatStat(c.Int));

        if (Core.AOS)
        {
            var y = 276;
            if (Core.SE)
            {
                var bd = BaseInstrument.GetBaseDifficulty(c);
                if (c.Uncalmable) bd = 0;
                builder.AddHtmlLocalized(153, 276, 160, 18, 1070793, LabelColor);
                builder.AddHtml(320, y, 35, 18, FormatDouble(bd));
                y += 18;
            }

            builder.AddImage(128, y + 2, 2086);
            builder.AddHtmlLocalized(147, y, 160, 18, 1049594, 200);
            y += 18;
            builder.AddHtmlLocalized(153, y, 160, 18,
                !c.Controlled || c.Loyalty == 0 ? 1061643 : 1049595 + c.Loyalty / 10, LabelColor);
        }
        else
        {
            builder.AddImage(128, 278, 2086);
            builder.AddHtmlLocalized(147, 276, 160, 18, 3001016, 200);
            builder.AddHtmlLocalized(153, 294, 160, 18, 1049581, LabelColor);
            builder.AddHtml(320, 294, 35, 18, FormatStat(c.VirtualArmor));
        }

        builder.AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
        builder.AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, pages);

        if (Core.AOS)
        {
            builder.AddPage(++page);
            builder.AddImage(128, 152, 2086);
            builder.AddHtmlLocalized(147, 150, 160, 18, 1061645, 200);
            builder.AddHtmlLocalized(153, 168, 160, 18, 1061646, LabelColor);
            builder.AddHtml(320, 168, 35, 18, FormatElement(c.PhysicalResistance));
            builder.AddHtmlLocalized(153, 186, 160, 18, 1061647, LabelColor);
            builder.AddHtml(320, 186, 35, 18, FormatElement(c.FireResistance, "#FF0000"));
            builder.AddHtmlLocalized(153, 204, 160, 18, 1061648, LabelColor);
            builder.AddHtml(320, 204, 35, 18, FormatElement(c.ColdResistance, "#000080"));
            builder.AddHtmlLocalized(153, 222, 160, 18, 1061649, LabelColor);
            builder.AddHtml(320, 222, 35, 18, FormatElement(c.PoisonResistance, "#008000"));
            builder.AddHtmlLocalized(153, 240, 160, 18, 1061650, LabelColor);
            builder.AddHtml(320, 240, 35, 18, FormatElement(c.EnergyResistance, "#BF80FF"));
            builder.AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
            builder.AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);

            builder.AddPage(++page);
            builder.AddImage(128, 152, 2086);
            builder.AddHtmlLocalized(147, 150, 160, 18, 1017319, 200);
            builder.AddHtmlLocalized(153, 168, 160, 18, 1061646, LabelColor);
            builder.AddHtml(320, 168, 35, 18, FormatElement(c.PhysicalDamage));
            builder.AddHtmlLocalized(153, 186, 160, 18, 1061647, LabelColor);
            builder.AddHtml(320, 186, 35, 18, FormatElement(c.FireDamage, "#FF0000"));
            builder.AddHtmlLocalized(153, 204, 160, 18, 1061648, LabelColor);
            builder.AddHtml(320, 204, 35, 18, FormatElement(c.ColdDamage, "#000080"));
            builder.AddHtmlLocalized(153, 222, 160, 18, 1061649, LabelColor);
            builder.AddHtml(320, 222, 35, 18, FormatElement(c.PoisonDamage, "#008000"));
            builder.AddHtmlLocalized(153, 240, 160, 18, 1061650, LabelColor);
            builder.AddHtml(320, 240, 35, 18, FormatElement(c.EnergyDamage, "#BF80FF"));

            if (Core.ML)
            {
                builder.AddHtmlLocalized(153, 258, 160, 18, 1076750, LabelColor);
                builder.AddHtml(300, 258, 55, 18, FormatDamage(c.DamageMin, c.DamageMax));
            }

            builder.AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
            builder.AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);
        }

        builder.AddPage(++page);
        builder.AddImage(128, 152, 2086);
        builder.AddHtmlLocalized(147, 150, 160, 18, 3001030, 200);
        builder.AddHtmlLocalized(153, 168, 160, 18, 1044103, LabelColor);
        builder.AddHtml(320, 168, 35, 18, FormatSkill(c, SkillName.Wrestling));
        builder.AddHtmlLocalized(153, 186, 160, 18, 1044087, LabelColor);
        builder.AddHtml(320, 186, 35, 18, FormatSkill(c, SkillName.Tactics));
        builder.AddHtmlLocalized(153, 204, 160, 18, 1044086, LabelColor);
        builder.AddHtml(320, 204, 35, 18, FormatSkill(c, SkillName.MagicResist));
        builder.AddHtmlLocalized(153, 222, 160, 18, 1044061, LabelColor);
        builder.AddHtml(320, 222, 35, 18, FormatSkill(c, SkillName.Anatomy));

        if (c is CuSidhe)
        {
            builder.AddHtmlLocalized(153, 240, 160, 18, 1044077, LabelColor);
            builder.AddHtml(320, 240, 35, 18, FormatSkill(c, SkillName.Healing));
        }
        else
        {
            builder.AddHtmlLocalized(153, 240, 160, 18, 1044090, LabelColor);
            builder.AddHtml(320, 240, 35, 18, FormatSkill(c, SkillName.Poisoning));
        }

        builder.AddImage(128, 260, 2086);
        builder.AddHtmlLocalized(147, 258, 160, 18, 3001032, 200);
        builder.AddHtmlLocalized(153, 276, 160, 18, 1044085, LabelColor);
        builder.AddHtml(320, 276, 35, 18, FormatSkill(c, SkillName.Magery));
        builder.AddHtmlLocalized(153, 294, 160, 18, 1044076, LabelColor);
        builder.AddHtml(320, 294, 35, 18, FormatSkill(c, SkillName.EvalInt));
        builder.AddHtmlLocalized(153, 312, 160, 18, 1044106, LabelColor);
        builder.AddHtml(320, 312, 35, 18, FormatSkill(c, SkillName.Meditation));
        builder.AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
        builder.AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);

        builder.AddPage(++page);
        builder.AddImage(128, 152, 2086);
        builder.AddHtmlLocalized(147, 150, 160, 18, 1049563, 200);

        var foodPref = 3000340;
        if ((c.FavoriteFood & FoodType.Meat) != 0) foodPref = 1049564;
        else if ((c.FavoriteFood & FoodType.FruitsAndVeggies) != 0) foodPref = 1049565;
        else if ((c.FavoriteFood & FoodType.GrainsAndHay) != 0) foodPref = 1049566;
        else if ((c.FavoriteFood & FoodType.Fish) != 0) foodPref = 1049568;
        else if ((c.FavoriteFood & FoodType.Metal) != 0) foodPref = 1049567;
        else if ((c.FavoriteFood & FoodType.Eggs) != 0) foodPref = 1044477;

        builder.AddHtmlLocalized(153, 168, 160, 18, foodPref, LabelColor);
        builder.AddImage(128, 188, 2086);
        builder.AddHtmlLocalized(147, 186, 160, 18, 1049569, 200);

        var packInstinct = 3000340;
        if ((c.PackInstinct & PackInstinct.Canine) != 0) packInstinct = 1049570;
        else if ((c.PackInstinct & PackInstinct.Ostard) != 0) packInstinct = 1049571;
        else if ((c.PackInstinct & PackInstinct.Feline) != 0) packInstinct = 1049572;
        else if ((c.PackInstinct & PackInstinct.Arachnid) != 0) packInstinct = 1049573;
        else if ((c.PackInstinct & PackInstinct.Daemon) != 0) packInstinct = 1049574;
        else if ((c.PackInstinct & PackInstinct.Bear) != 0) packInstinct = 1049575;
        else if ((c.PackInstinct & PackInstinct.Equine) != 0) packInstinct = 1049576;
        else if ((c.PackInstinct & PackInstinct.Bull) != 0) packInstinct = 1049577;

        builder.AddHtmlLocalized(153, 204, 160, 18, packInstinct, LabelColor);

        if (!Core.AOS)
        {
            builder.AddImage(128, 224, 2086);
            builder.AddHtmlLocalized(147, 222, 160, 18, 1049594, 200);
            builder.AddHtmlLocalized(153, 240, 160, 18,
                !c.Controlled || c.Loyalty == 0 ? 1061643 : 1049595 + c.Loyalty / 10, LabelColor);
        }

        builder.AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, 1);
        builder.AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);
    }
}
