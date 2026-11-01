using System;
using Server.Items;
using Server.Network;

namespace Server.Gumps;

[Flags]
public enum ReforgingOption
{
    None = 0x00000000,
    Powerful = 0x00000001,
    Structural = 0x00000002,
    Fortified = 0x00000004,
    Fundamental = 0x00000008,
    Integral = 0x00000010,
    GrandArtifice = 0x00000020,
    InspiredArtifice = 0x00000040,
    ExaltedArtifice = 0x00000080,
    SublimeArtifice = 0x00000100,

    PowerfulAndStructural = Powerful | Structural,
    PowerfulAndFundamental = Powerful | Fundamental,
    StructuralAndFundamental = Structural | Fundamental,
    PowerfulStructuralAndFundamental = PowerfulAndStructural | Fundamental
}

public class RunicReforgingGump : DynamicGump
{
    // Row order of the option list. Each row's button id is its index + 100, so this order is part
    // of the gump protocol and must not change.
    private static readonly ReforgingOption[] _allOptions =
    [
        ReforgingOption.Powerful,
        ReforgingOption.Structural,
        ReforgingOption.Fortified,
        ReforgingOption.Fundamental,
        ReforgingOption.Integral,
        ReforgingOption.GrandArtifice,
        ReforgingOption.InspiredArtifice,
        ReforgingOption.ExaltedArtifice,
        ReforgingOption.SublimeArtifice
    ];

    private readonly Mobile _from;
    private readonly BaseRunicTool _tool;
    private readonly Item _toReforge;
    private readonly ReforgingContext _context;

    private ReforgingOption _options;
    private ReforgedPrefix _prefix;
    private ReforgedSuffix _suffix;

    public override bool Singleton => true;

    public RunicReforgingGump(Mobile from, Item toReforge, BaseRunicTool tool) : base(100, 100)
    {
        from.CloseGump<RunicReforgingGump>();
        from.CloseGump<Server.Engines.Imbuing.ImbueGump>();

        _from = from;
        _context = ReforgingContext.GetContext(from);

        if (!_context.Contexts.ContainsKey(tool))
        {
            _context.Contexts[tool] = ReforgingOption.None;
        }

        _tool = tool;
        _toReforge = toReforge;
        _options = _context.Contexts[tool];

        _prefix = _context.Prefix;
        _suffix = _context.Suffix;
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();
        builder.AddBackground(0, 0, 370, 440, 83);
        // Runic Crafting Options
        builder.AddHtmlLocalized(10, 10, 350, 18, 1114513, "#1151952", 0x4BB7, false, false);

        var y = 40;

        for (var i = 0; i < _allOptions.Length; i++)
        {
            var option = _allOptions[i];

            int buttonHue;
            int buttonId;

            if ((_options & option) != 0)
            {
                buttonId = 4006;
                buttonHue = CanReforge(option) && HasMetPrerequisite(option) ? 0x4BB2 : 0x7652;
            }
            else if (CanReforge(option) && HasMetPrerequisite(option))
            {
                buttonId = 4005;
                buttonHue = 0x6F7B;
            }
            else
            {
                buttonId = 4006;
                buttonHue = 0x7652;
            }

            if (HasMetPrerequisite(option) && CanReforge(option))
            {
                builder.AddButton(15, y, buttonId, buttonId, i + 100);
            }

            builder.AddHtmlLocalized(55, y, 250, 20, GetCliloc(option), buttonHue, false, false);

            y += 25;
        }

        var totalCharges = GetTotalCharges();

        if ((_options & ReforgingOption.InspiredArtifice) != 0)
        {
            builder.AddButton(15, 305, 4005, 4007, 1);
            builder.AddHtmlLocalized(55, 305, 250, 20, 1152087, 0x6F7B, false, false);
            builder.AddHtmlLocalized(190, 305, 250, 20, RunicReforging.GetName((int)_prefix), 0x5757, false, false);
        }

        if ((_options & ReforgingOption.SublimeArtifice) != 0)
        {
            builder.AddButton(15, 330, 4005, 4007, 2);
            builder.AddHtmlLocalized(55, 330, 250, 20, 1152088, 0x6F7B, false, false);
            builder.AddHtmlLocalized(190, 330, 250, 20, RunicReforging.GetName((int)_suffix), 0x5757, false, false);
        }

        // CHARGES NEEDED:
        builder.AddHtmlLocalized(10, 363, 140, 22, 1114514, "#1152078", 0x4BB2, false, false);
        builder.AddLabel(160, 363, 0x113, $"{totalCharges}");

        // TOOL CHARGES:
        builder.AddHtmlLocalized(10, 385, 140, 22, 1114514, "#1152077", 0x6F7B, false, false);
        builder.AddLabel(160, 385, 0x44E, $"{_tool.UsesRemaining}");

        builder.AddButton(10, 412, 4017, 4018, 0);
        // CLOSE
        builder.AddHtmlLocalized(45, 410, 200, 20, 1060675, 0x6F7B, false, false);

        builder.AddButton(330, 363, 4014, 4016, 3);
        // REFORGE ITEM
        builder.AddHtmlLocalized(190, 363, 135, 22, 1114514, "#1152080", 0x6F7B, false, false);

        builder.AddButton(330, 412, 4011, 4013, 4);
        // HELP
        builder.AddHtmlLocalized(185, 412, 140, 18, 1114514, "#1149735", 0x6F7B, false, false);
    }

    private bool HasMetPrerequisite(ReforgingOption option) =>
        option switch
        {
            ReforgingOption.None => true,
            ReforgingOption.Powerful => true,
            ReforgingOption.Structural => true,
            ReforgingOption.Fortified => (_options & ReforgingOption.Structural) != 0,
            ReforgingOption.Fundamental => true,
            ReforgingOption.Integral => (_options & ReforgingOption.Fundamental) != 0,
            ReforgingOption.GrandArtifice => true,
            ReforgingOption.InspiredArtifice => (_options & ReforgingOption.GrandArtifice) != 0,
            ReforgingOption.ExaltedArtifice => (_options & ReforgingOption.GrandArtifice) != 0,
            ReforgingOption.SublimeArtifice =>
                (_options & ReforgingOption.ExaltedArtifice) != 0 && (_options & ReforgingOption.GrandArtifice) != 0,
            _ => true
        };

    private void InvalidatePrerequisite(ReforgingOption option)
    {
        switch (option)
        {
            case ReforgingOption.None:
            case ReforgingOption.Powerful:
                break;
            case ReforgingOption.Structural:
                if ((_options & ReforgingOption.Fortified) != 0)
                {
                    _options ^= ReforgingOption.Fortified;
                }

                break;
            case ReforgingOption.Fortified:
                break;
            case ReforgingOption.Fundamental:
                if ((_options & ReforgingOption.Integral) != 0)
                {
                    _options ^= ReforgingOption.Integral;
                }

                break;
            case ReforgingOption.Integral:
                break;
            case ReforgingOption.GrandArtifice:
                if ((_options & ReforgingOption.InspiredArtifice) != 0)
                {
                    _options ^= ReforgingOption.InspiredArtifice;
                }

                if ((_options & ReforgingOption.ExaltedArtifice) != 0)
                {
                    _options ^= ReforgingOption.ExaltedArtifice;
                }

                if ((_options & ReforgingOption.SublimeArtifice) != 0)
                {
                    _options ^= ReforgingOption.SublimeArtifice;
                }

                break;
            case ReforgingOption.InspiredArtifice:
                break;
            case ReforgingOption.ExaltedArtifice:
                if ((_options & ReforgingOption.SublimeArtifice) != 0)
                {
                    _options ^= ReforgingOption.SublimeArtifice;
                }

                break;
            case ReforgingOption.SublimeArtifice:
                break;
        }
    }

    private bool CanReforge(ReforgingOption option)
    {
        var skill = _from.Skills[SkillName.Imbuing].Value;

        return option switch
        {
            ReforgingOption.Structural or ReforgingOption.Fortified => skill >= 85.0,
            ReforgingOption.Fundamental or ReforgingOption.Integral => skill >= 97.0,
            ReforgingOption.GrandArtifice or ReforgingOption.InspiredArtifice => skill >= 100.1,
            ReforgingOption.ExaltedArtifice or ReforgingOption.SublimeArtifice => skill >= 110.1,
            // None, Powerful and any unlisted combination.
            _ => skill >= 65.0
        };
    }

    private static int GetCliloc(ReforgingOption option) =>
        option switch
        {
            ReforgingOption.Powerful => 1151954,
            ReforgingOption.Structural => 1151955,
            ReforgingOption.Fortified => 1151956,
            ReforgingOption.Fundamental => 1151957,
            ReforgingOption.Integral => 1151958,
            ReforgingOption.GrandArtifice => 1151961,
            ReforgingOption.InspiredArtifice => 1151962,
            ReforgingOption.ExaltedArtifice => 1151963,
            ReforgingOption.SublimeArtifice => 1151964,
            _ => 0
        };

    private int GetTotalCharges()
    {
        var count = 1;

        foreach (var option in _allOptions)
        {
            if ((_options & option) != 0)
            {
                count++;
            }
        }

        return Math.Min(10, count);
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;

        if (!BaseTool.CheckAccessible(_tool, from))
        {
            return;
        }

        switch (info.ButtonID)
        {
            case 0:
                break;
            case 1: // prefix
                from.SendGump(new ItemNameGump(_toReforge, _tool, _options, _prefix, _suffix, true));
                break;
            case 2: // suffix
                from.SendGump(new ItemNameGump(_toReforge, _tool, _options, _prefix, _suffix, false));
                break;
            case 3: // Reforge Item
                {
                    if (!RunicReforging.CanReforge(from, _toReforge, _tool.CraftSystem))
                    {
                        return;
                    }

                    var totalCharges = GetTotalCharges();

                    if (_tool.UsesRemaining >= totalCharges)
                    {
                        var resInfo = CraftResources.GetInfo(_tool.Resource);

                        if (resInfo == null)
                        {
                            return;
                        }

                        int min = 10;
                        int max = 80;

                        var budget = GetBudget();

                        var prefix = ReforgedPrefix.None;
                        var suffix = ReforgedSuffix.None;

                        if ((_options & ReforgingOption.GrandArtifice) != 0)
                        {
                            // choosing name 1
                            if ((_options & ReforgingOption.InspiredArtifice) != 0)
                            {
                                prefix = _prefix;

                                if (prefix == ReforgedPrefix.None)
                                {
                                    // Re-forging failed. You did not choose a name! Please try again.
                                    from.SendLocalizedMessage(1152287);
                                    return;
                                }
                            }
                            // Not choosing name 1 or 2
                            else if ((_options & ReforgingOption.SublimeArtifice) == 0)
                            {
                                // random prefix AND suffix
                                if ((_options & ReforgingOption.ExaltedArtifice) != 0)
                                {
                                    prefix = RunicReforging.ChooseRandomPrefix(_toReforge, budget);
                                    suffix = RunicReforging.ChooseRandomSuffix(_toReforge, budget, _prefix);
                                }
                                // random prefix OR suffix
                                else if (0.5 > Utility.RandomDouble())
                                {
                                    prefix = RunicReforging.ChooseRandomPrefix(_toReforge, budget);
                                }
                                else
                                {
                                    suffix = RunicReforging.ChooseRandomSuffix(_toReforge, budget, _prefix);
                                }
                            }
                        }

                        if ((_options & ReforgingOption.ExaltedArtifice) != 0)
                        {
                            if ((_options & ReforgingOption.SublimeArtifice) != 0)
                            {
                                suffix = _suffix;

                                if (suffix == ReforgedSuffix.None)
                                {
                                    // Re-forging failed. You did not choose a name! Please try again.
                                    from.SendLocalizedMessage(1152287);
                                    return;
                                }
                            }
                            else
                            {
                                suffix = RunicReforging.ChooseRandomSuffix(_toReforge, budget, _prefix);
                                budget = Math.Min(800, budget + 50);
                            }
                        }

                        // 50% chance to switch prefix/suffix around
                        if ((prefix != ReforgedPrefix.None || suffix != ReforgedSuffix.None) && 0.5 > Utility.RandomDouble())
                        {
                            var pre = (int)prefix;
                            var suf = (int)suffix;

                            prefix = (ReforgedPrefix)suf;
                            suffix = (ReforgedSuffix)pre;
                        }

                        RunicReforging.ApplyReforgedProperties(
                            _toReforge,
                            prefix,
                            suffix,
                            budget,
                            min,
                            max,
                            RunicReforging.GetPropertyCount(_tool),
                            0,
                            _tool,
                            _options
                        );

                        OnAfterReforged(_toReforge);
                        // You re-forge the item!
                        from.SendLocalizedMessage(1152286);

                        from.PlaySound(0x665);

                        _tool.UsesRemaining -= totalCharges;

                        if (_tool.UsesRemaining <= 0)
                        {
                            _tool.Delete();
                            // You have worn out your tool!
                            from.SendLocalizedMessage(1044038);
                        }
                    }

                    break;
                }
            case 4:
                from.SendGump(new ReforgingHelpGump());
                break;
            default: // Option
                {
                    var option = _allOptions[info.ButtonID - 100];

                    if (HasMetPrerequisite(option))
                    {
                        if ((_options & option) == 0)
                        {
                            _options |= option;

                            if (_prefix != ReforgedPrefix.None &&
                                !RunicReforging.HasSelection((int)_prefix, _toReforge, _tool, _options, -1, -1))
                            {
                                _prefix = ReforgedPrefix.None;
                                _context.Prefix = ReforgedPrefix.None;
                            }

                            if (_suffix != ReforgedSuffix.None &&
                                !RunicReforging.HasSelection((int)_suffix, _toReforge, _tool, _options, -1, -1))
                            {
                                _suffix = ReforgedSuffix.None;
                                _context.Suffix = ReforgedSuffix.None;
                            }
                        }
                        else
                        {
                            _options ^= option;
                            InvalidatePrerequisite(option);
                        }

                        _context.Contexts[_tool] = _options;
                    }

                    from.SendGump(new RunicReforgingGump(from, _toReforge, _tool));
                    break;
                }
        }
    }

    private int GetBudget()
    {
        var budget = _tool.Resource switch
        {
            CraftResource.DullCopper or CraftResource.ShadowIron or CraftResource.SpinedLeather or CraftResource.OakWood => 140,
            CraftResource.Copper or CraftResource.AshWood => 350,
            CraftResource.Bronze or CraftResource.YewWood or CraftResource.HornedLeather => 500,
            CraftResource.Gold or CraftResource.Agapite or CraftResource.Heartwood or CraftResource.Bloodwood => 600,
            CraftResource.Verite or CraftResource.Frostwood or CraftResource.BarbedLeather => 700,
            CraftResource.Valorite => 750,
            // Any resource not listed above budgets the same as the lowest tier.
            _ => 140
        };

        if ((_options & ReforgingOption.Powerful) != 0)
        {
            budget += 60;
        }

        if ((_options & ReforgingOption.Structural) != 0)
        {
            budget += 60;
        }

        if ((_options & ReforgingOption.Fundamental) != 0)
        {
            budget += 100;
        }

        return budget;
    }

    public void OnAfterReforged(Item item)
    {
        var attr = RunicReforging.GetAosAttributes(item);

        var durability = 0;

        if (item is BaseWeapon weapon)
        {
            attr = weapon.Attributes;
        }
        else if (item is BaseArmor armor)
        {
            attr = armor.Attributes;
        }

        if (attr != null && (_options & ReforgingOption.Structural) != 0)
        {
            if ((_options & ReforgingOption.Fortified) != 0)
            {
                durability = 150;
            }

            if (item is BaseArmor or BaseClothing)
            {
                item.Hue = 2500;
            }
        }

        if ((_options & ReforgingOption.Fundamental) != 0)
        {
            durability = (_options & ReforgingOption.Integral) != 0 ? 255 : 200;

            if (item.Hue == 0 && (item is BaseArmor or BaseClothing))
            {
                item.Hue = 2500;
            }
        }

        if (durability > 0 && item is IDurability dur)
        {
            dur.MaxHitPoints = durability;
            dur.HitPoints = durability;
        }

        RunicReforging.ApplyItemPower(item, true);
    }

    private class ReforgingHelpGump : DynamicGump
    {
        public ReforgingHelpGump() : base(100, 100)
        {
        }

        protected override void BuildLayout(ref DynamicGumpBuilder builder)
        {
            builder.AddPage();
            builder.AddBackground(0, 0, 370, 440, 83);

            // Runic Crafting Help
            builder.AddHtmlLocalized(10, 10, 350, 18, 1114513, "#1151966", 0x4BB7, false, false);
            builder.AddHtmlLocalized(10, 40, 353, 365, 1151965, 0xFFE0, false, true);

            builder.AddHtmlLocalized(45, 412, 100, 20, 1060675, 0x6F7B, false, false);
            builder.AddButton(10, 412, 4017, 4019, 0);
        }
    }
}

public class ItemNameGump : DynamicGump
{
    private const int White = 0x6F7B;
    private const int Green = 0x4BB2;
    private const int Yellow = 0x6B55;

    private readonly BaseRunicTool _tool;
    private readonly Item _toReforge;
    private readonly ReforgingOption _options;
    private readonly bool _isPrefix;

    private ReforgedPrefix _prefix;
    private ReforgedSuffix _suffix;

    public override bool Singleton => true;

    public ItemNameGump(
        Item toReforge, BaseRunicTool tool, ReforgingOption options, ReforgedPrefix prefix, ReforgedSuffix suffix, bool isPrefix
    ) : base(100, 100)
    {
        _tool = tool;
        _toReforge = toReforge;
        _options = options;
        _prefix = prefix;
        _suffix = suffix;
        _isPrefix = isPrefix;
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();
        builder.AddBackground(0, 0, 370, 440, 83);

        // Runic Crafting - Item Name Selection
        builder.AddHtmlLocalized(10, 10, 350, 18, 1114513, "#1152089", 0x4BB7, false, false);

        var y = 50;

        foreach (var value in Enum.GetValues<ReforgedPrefix>())
        {
            var index = (int)value;

            if (index == 0)
            {
                continue;
            }

            int buttonId;
            int buttonHue;

            if (_isPrefix ? _prefix == value : _suffix == (ReforgedSuffix)index)
            {
                buttonId = 4006;
                buttonHue = Green;
            }
            else
            {
                buttonId = 4005;
                buttonHue = White;
            }

            if (RunicReforging.HasSelection(index, _toReforge, _tool, _options, (int)_prefix, (int)_suffix))
            {
                builder.AddButton(15, y, buttonId, buttonId, 100 + index);
            }
            else
            {
                buttonHue = Yellow;
            }

            builder.AddHtmlLocalized(55, y, 250, 20, RunicReforging.GetName(index), buttonHue, false, false);

            y += 25;
        }

        builder.AddHtmlLocalized(45, 412, 100, 20, 1060675, White, false, false);
        builder.AddButton(10, 412, 4017, 4019, 0);
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;

        if (info.ButtonID == 0)
        {
            return;
        }

        var index = info.ButtonID - 100;

        if (index >= 0 && index <= 12 &&
            RunicReforging.HasSelection(index, _toReforge, _tool, _options, (int)_prefix, (int)_suffix))
        {
            var context = ReforgingContext.GetContext(from);

            if (_isPrefix)
            {
                context.Prefix = (ReforgedPrefix)index;
                _prefix = (ReforgedPrefix)index;
            }
            else
            {
                context.Suffix = (ReforgedSuffix)index;
                _suffix = (ReforgedSuffix)index;
            }
        }

        from.SendGump(new RunicReforgingGump(from, _toReforge, _tool));
    }
}
