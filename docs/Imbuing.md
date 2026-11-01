# Imbuing System for ModernUO

A custom implementation of the **Imbuing** skill for **ModernUO** — the Stygian Abyss crafting skill that lets players add magical properties to equipment by consuming gems and rare ingredients harvested from creatures around the world. Includes soulforges, ingredient drops, the full imbuing and unraveling menus, and ~150 imbuable properties.

This is an original UO Wildlands custom script, licensed under GPL-3.0.

---

## Features

- **Full Imbuing skill** — add or upgrade item properties, or unravel unwanted magic items into ingredients, at a soulforge.
- **Soulforges** — three addon types: large (4×4), small (1 tile), and royal (4×4 alternate design). All three are recognized by the skill.
- **~150 imbuable properties** — stats, regens, combat stats, casting stats, hit effects, resistances, slayers, and skill bonuses on jewelry.
- **Three-tier ingredient system** — every property costs a primary resource, a gem, and (at high intensity) a special ingredient.
- **Intensity control** — pick how strong the property is with +/- buttons in the confirmation window. Higher intensity costs more materials and has a lower success chance.
- **Unraveling** — break down unwanted magic items at a soulforge to produce Magical Residue, Enchanted Essence, or Relic Fragments based on the item's total property weight and your skill.
- **Thematic creature drops** — daemon claws from daemons, spider carapace from spiders, essence of passion from fire creatures, and more.
- **Handy shortcuts** — Reimbue Last, Imbue Last Item, and Imbue Last Property save clicks on repeat work.
- **GM tooling** — `[ImbuingKit` fills your backpack with 999 of every ingredient for testing.

---

## How It Works (Player Perspective)

1. Stand near a **soulforge** with an imbuable item in your backpack.
2. Use the **Imbuing skill** — the main menu opens.
3. Pick **Imbue Item** and target your item (weapons, armor, shields, hats, rings, bracelets).
4. Choose a category (Casting, Combat, Resists, Hit Effects, Slayers, etc.) and pick a property.
5. Adjust the **intensity** with the +/- buttons. The window shows:
   - materials needed
   - current and projected item weight
   - current and projected property count
   - times imbued (max 20)
   - success chance
6. Press **Imbue Item**. On success you get the property; on failure you only lose the primary resource.

You can also **Unravel Item** or **Unravel Container** to destroy unwanted magic items and produce ingredients.

---

## Ingredient Cheat Sheet

| Tier | Item | Source | Used For |
|------|------|--------|----------|
| Primary | Magical Residue | Unravel light items | Low-weight properties |
| Primary | Enchanted Essence | Unravel medium items | Medium-weight properties |
| Primary | Relic Fragment | Unravel heavy items (95+ skill) | Heavy properties (HCI, DI, slayers) |
| Gem | Standard gem per property | Mining / shops | Every property |
| Special | Essence of X | High-fame creature drops | Defensive / hit-spell properties |
| Special | Raptor Teeth, Void Orb, etc. | Monster drops | Offensive properties |
| Special | Fey Wings, Daemon Claw, etc. | Slayer-tied drops | Slayer properties |

---

## Files

| File | Purpose |
|------|---------|
| `Imbuing.cs` | Core skill logic: validation, success chance, material costs, applying properties, unraveling. |
| `ImbuingContext.cs` | Remembers your last item/property for the shortcut options. |
| `ImbuingGump.cs` | Main skill menu. |
| `ImbueSelectGump.cs` | Category and property picker. |
| `ImbueGump.cs` | Confirmation window with the intensity slider. |
| `UnravelGump.cs` | Unravel confirmations for single items and containers. |
| `ItemPropertyInfo.cs`, `ItemPropertyTable.cs`, `ItemPropertyTable2.cs` | The full property tables (stats, hit effects, resistances, slayers, skill bonuses). |
| `ResourceDrops.cs` | Creature ingredient and essence drops. |
| `ImbuingCommands.cs` | The `[ImbuingKit` GM command. |
| `SoulForge.cs` | Soulforge addons and deeds. |
| `MagicalResidue.cs`, `EnchantedEssence.cs`, `RelicFragment.cs` | The three unravel tiers. |
| `ImbuingEssences.cs` | The eleven essence types. |
| `ImbuingIngredients.cs` | ~30 special ingredients. |

---

## Known Limitations

- **Times Imbued isn't tracked** — the 20-imbue cap is shown in the gump but not saved on items, so it resets on restart.
- **Weight caps are partly display-only** — some calculations are placeholders, so the projected totals aren't always enforced.
- **Artifact detection is basic** — only low-quality weapons and armor are excluded right now.
- **No region bonuses yet** — Queen's Palace and Royal City don't give the extra success chance they do on real UO.
- **Imbuing session doesn't persist** — "Reimbue Last" won't work across a restart.
- **Hat properties are limited** — some hat-only interactions aren't wired up.

---

## Requirements

- ModernUO
- A creature-drop hook on `BaseCreature.OnDeath` (to call `ImbuingResourceDrops.AddDrops`)
- The `BaseAddon` system (for placing soulforges in the world)

---

## Credits

Original custom system by the **UO Wildlands Team**. GPL-3.0.