# Virtual Storage System for ModernUO

A custom stockpile system for **ModernUO** where players can deposit raw materials, reagents, scrolls, and other crafting supplies into specialized storage chests. Each chest only accepts a curated whitelist of item types, tracks quantities virtually (no item sprawl), and offers a paged withdraw UI.

This is an original UO Wildlands custom script, licensed under GPL-3.0.

---

## Features

- **Eight specialized chest types** — Food, Logs/Boards, Miner (ore/ingots/granite/scales/sand), Mondain's Legacy (gems/ML reagents), Reagents, Scrolls, Stygian Abyss (imbuing ingredients), and Tailoring supplies.
- **Type whitelist per chest** — each chest only accepts items that match its allowed type list (subclasses included).
- **Virtual storage** — items are deleted on deposit and only their type + count are tracked. No item counts on the ground, no serial bloat.
- **Deposit a single item or a whole container** — targeting a bag or box recursively pulls every item out of it (nested containers included).
- **Paged withdraw UI** — a clean gump with item name, count, prev/next paging, and one-click withdraw up to 100 units at a time.
- **House-secured only** — chests must be locked down or secured to function, matching UO housing storage conventions. Also works in the custom Townhouses system.
- **Smart sorting** — boards before logs, ingots before granite, gems grouped together, and scrolls sorted by school then by circle (magery circle 1 → 8, then necromancy, mysticism, spellweaving).
- **Persistence** — chest contents survive server restarts by serializing type names + amounts.

---

## How It Works (Player Perspective)

1. Place a storage chest in your house and **lock it down or secure it**.
2. Double-click the chest to open the **Virtual Storage Gump**.
3. Click **ADD** and target either:
   - a single item in your pack, or
   - a container in your pack / on the ground (all matching contents are pulled out).
4. To withdraw, click the **arrow button** next to any entry — up to 100 of that item is placed in your backpack.

The chest refuses anything that doesn't match its whitelist with: *"That item does not belong in this stockpile."*

---

## How It Works (Under the Hood)

- Each chest is a subclass of **`VirtualStorageChest`**, which holds a `Dictionary<Type, int>` of item type → quantity, plus two abstract properties:
  - `AllowedTypes` — the whitelist (evaluated against the concrete type, with subclass matching).
  - `ChestTitle` — the gump header.
- On deposit, `TryAdd` verifies the type, deletes the item, increments the count, and marks the chest dirty.
- On withdraw, the gump's `Withdraw` instantiates a fresh item of the stored type, sets `.Amount`, runs `CheckHold` against the player's backpack, adds it, and decrements the count.
- The gump sorts entries once when opened, so button indices stay stable across the session.
- Serialization writes each entry as `FullName` + `Amount`; on load, types are resolved through `AssemblyHandler.FindTypeByFullName`. Unknown types are silently skipped.

---

## Chest Types

| Chest | Accepts |
|-------|---------|
| **Food Storage Chest** | Meats, fish, breads, cheese, eggs, honey, produce, chocolatiering ingredients, standard fish. |
| **Log & Board Storage Chest** | All log and board tiers, arrows/bolts/shafts, feathers, blank scrolls. |
| **Miner's Storage Chest** | All ore and ingot tiers, granite, scales, sand. |
| **Mondain's Legacy Storage Chest** | Standard + ML gems, ML harvesting specials, Peerless reagents. |
| **Reagent Storage Chest** | Standard and necromancy reagents, fertile dirt, daemon bone, bottles. |
| **Scroll Storage Chest** | All `SpellScroll` subclasses (magery, necromancy, mysticism, spellweaving). |
| **Stygian Abyss Storage Chest** | Imbuing ingredients, essences, monster/SA expansion drops. |
| **Tailoring Storage Chest** | Leather, hides, thread/yarn, cloth/bolts, bandages, cotton, flax, wool. |

Several items are commented out in each whitelist — those are either not present in this codebase or deliberately excluded. Uncomment when the corresponding types become available.

---

## Files

| File | Purpose |
|------|---------|
| `VirtualStorageChest.cs` | Abstract base: whitelist check, deposit, withdraw, serialization. |
| `VirtualStorageGump.cs` | The paged UI, add target, withdraw logic, sort priorities. |
| `FoodStorageChest.cs` | Food whitelist. |
| `LogStorageChest.cs` | Log/board whitelist. |
| `MinerStorageChest.cs` | Ore/ingot/granite/scale/sand whitelist. |
| `MondainStorageChest.cs` | Gem and ML reagent whitelist. |
| `ReagentStorageChest.cs` | Reagent whitelist. |
| `ScrollStorageChest.cs` | `SpellScroll` whitelist (single entry — subclasses cover all schools). |
| `StygianAbyssStorageChest.cs` | Imbuing/SA whitelist. |
| `TailorStorageChest.cs` | Leather/hide/cloth whitelist. |

---

## Known Limitations

- **No per-player ownership** — anyone with house access can add or withdraw. Ownership is governed entirely by the house security the chest is placed under.
- **No per-type withdrawal amount dialog** — withdrawing is capped at 100 per click. Players who need thousands will click multiple times.
- **No bulk withdraw-all button** — everything is one row at a time.
- **Items are destroyed on deposit and re-created on withdraw** — any item-specific state (dye color, crafted-by tag, runic properties, durability) is lost. That is by design for a stockpile chest, but it means reforged or imbued items should never be stored here.
- **Type identity is by `FullName`** — renaming or moving a class between namespaces will orphan its stored entries (they will load as zero and be silently dropped).
- **Whitelist matching is exact-type or subclass** — a class that *contains* another matching class won't be caught unless it inherits from it.

---

## Requirements

- ModernUO with `ModernUO.Serialization` (`[SerializationGenerator]` for the chest classes; the base class serializes by hand because of the `Type`-keyed dictionary).
- `DynamicGump` support (used for the storage UI).
- Housing lockdown / secure system (chests refuse to open without it).

---

## Credits

Original custom system by the **UO Wildlands Team**. GPL-3.0.