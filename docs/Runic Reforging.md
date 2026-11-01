# Runic Reforging for ModernUO

A port of the ServUO Runic Reforging system to **ModernUO**.

Runic Reforging lets players use a runic crafting tool at a soulforge to re-roll an item's magic properties, optionally steering the result toward a named prefix/suffix (Might, Mystic, Arcane, Vampiric, etc.) or pushing the item into artifact-tier power. The system was originally written for ServUO; this port adapts it to ModernUO's item, imbuing, and persistence APIs.

---

## Features

- **Runic reforging gump** — a dedicated UI opened by double-clicking a runic tool near a soulforge. The classic craft gump still opens anywhere else, so tool behavior is unchanged when away from a forge.
- **Reforging options** — Powerful, Structural, Fortified, Fundamental, Integral, and the four Artifice tiers, each gated behind Imbuing skill and prerequisite options.
- **Named prefixes & suffixes** — 12 prefixes and 18 suffixes (including dungeon-themed ones like Minax, Doom, Khaldun, and Kotl) with per-item-type, per-resource property tables.
- **Budget-based property generation** — monster difficulty, luck, and runic resource all feed a budget that determines how many and how strong the properties are.
- **Overcapped properties** — "powerful" loot can exceed standard imbuing caps (e.g. higher-than-normal resistances or leeches).
- **Item power rating** — reforged items are tagged as Reforged Minor/Lesser/Greater/Major/Legendary based on their total imbuing weight.
- **Tool combining** — target another runic tool of the same type and material to merge charges (capped at 100).
- **Persistence** — per-player reforging selections survive logout and server restarts.
- **GM commands** — `[Reforge` and `[GetCreatureScore` for testing.

---

## How It Works (Player Perspective)

1. Stand near a **soulforge** with a runic tool in your backpack.
2. Double-click the tool — the **Runic Reforging gump** opens.
3. Toggle reforging options (they cost extra tool charges).
4. Optionally pick a **prefix** and/or **suffix** from the name selection gumps.
5. Press **Reforge Item** and target an item in your pack.

The result depends on the tool's resource tier, the selected options, the item type, and RNG. Higher-tier tools and more expensive options yield higher budgets, which means more properties and a chance at overcap values.

---

## How It Works (Under the Hood)

The system is budget-driven:

- Each runic resource maps to a **base budget** (Dull Copper = 140 … Valorite = 750).
- Options add to that budget; the item is considered **powerful** above a threshold.
- A **property count** is rolled from the tool's resource, then properties are drawn from the item's loot table and paid for out of the remaining budget.
- Named prefixes/suffixes pull from curated per-item-type tables with material-scaled intensity ranges.
- After the properties are applied, the item is classified into an **ItemPower** tier based on its total imbuing weight.

---

## Files

| File | Purpose |
|------|---------|
| `RunicReforging.cs` | Core engine: property application, budget logic, item power, random generation. |
| `RunicReforging.Tables.cs` | Prefix/suffix property tables, per-resource intensity tables, GM commands, name clilocs. |
| `RunicReforgingGumps.cs` | Reforging gump, name-selection gump, help gump, `ReforgingOption` flags. |
| `RunicReforgingAdapters.cs` | Facade over the custom imbuing engine (`ReforgingPropertyInfo`, `ReforgingImbuing`). |
| `RunicReforgingItemExtensions.cs` | `ICombatEquipment` and per-item-type `ItemPower` / prefix / suffix properties. |
| `RunicReforgingToolHook.cs` | Soulforge gate + tool double-click hook. |
| `ReforgingContext.cs` | Per-player, persisted reforging selection state. |
| `RandomItemGenerator.cs` | Monster-difficulty → budget helper. |

---

## Known Limitations

- **Reforging state is runtime-only.** The prefix/suffix/`ItemPower` on a reforged item is *not* serialized in this port — it survives until the next server restart. Persisting it would require a serialized field plus a version bump on each affected item class.
- **Some ServUO-only attributes are dropped** (eat-on-hit absorption, hit mana drain, battle lust, reactive paralyze, soul charge, splintering weapon, gargish item variants). See the `TODO(migrate)` comments in `RunicReforging.Tables.cs` and `RunicReforgingAdapters.cs`.
- **Creature difficulty** is approximated from fame (`100 + fame/25`), since the ServUO scoring routine wasn't available.
- **Non-imbuable list** isn't enforced — the custom imbuing engine has no such list, so `IsInNonImbueList` always returns `false`.

---

## Requirements

- ModernUO
- The custom imbuing engine (`Server.Engines.Imbuing`) that exposes `ItemPropertyInfo.Table` with `PropCategories`, `LootTable`, and `GetMaxIntensity`.
- The tool/craft systems must initialize before `RunicReforging.Initialize()` runs (it is marked `[CallPriority(1)]`).

---

## Credits

Ported from the ServUO Runic Reforging system. ModernUO adaptation and cleanup by this project.