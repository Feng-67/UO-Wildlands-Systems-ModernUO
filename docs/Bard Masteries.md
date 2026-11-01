# Bard Masteries for ModernUO

A custom port of the ServUO **Skill Masteries** system, limited to bards for now. Bards who reach 90 skill in Provocation, Peacemaking, or Discordance can unlock that path's masteries, switch between paths with a dedicated book, and cast songs that buff their party or debuff enemies.

The other mastery paths from live UO (weapon masteries, etc.) are **not** included in this port.

This is a custom UO Wildlands script, licensed under GPL-3.0.

> ⚠️ **Important:** Only `BardMasteries.cs` and `BookOfBardMasteries.cs` are new custom files. The other files listed below are **stock ModernUO files that were modified** to add the mastery hooks — they must be **overwritten** on your server, not merged by hand, or the masteries won't function.

---

## Features

- **Three bard paths** — Provocation, Peacemaking, and Discordance, each with two abilities.
- **Skill-gated** — level 1 unlocks at 90 skill, level 2 at 100.
- **Mastery book** — switch your active path (or turn it off) at any time.
- **Party buffs** — four of the six abilities follow your party and their pets automatically.
- **Upkeep cost** — songs last up to 30 minutes, but drain mana every couple of seconds. Run out and the song ends.
- **Persistent choice** — your active mastery is saved across restarts.
- **Instrument required** — every mastery needs an instrument, same as normal bard songs.

---

## The Six Abilities

| Path | Ability | Type | Effect |
|------|---------|------|--------|
| **Provocation** | Inspire | Party buff | +Hit Chance, +Spell Damage, +Weapon Damage |
| **Provocation** | Invigorate | Party buff | +Max HP, +Str/Dex/Int, small periodic heal |
| **Peacemaking** | Resilience | Party buff | +HP, +Stamina, +Mana regeneration |
| **Peacemaking** | Perseverance | Party buff | +Defense Chance, reduces party damage taken |
| **Discordance** | Tribulation | Target debuff | Lowers Hit Chance and Spell Damage; occasional burst damage |
| **Discordance** | Despair | Target debuff | Lowers Strength; damage over time |

---

## How It Works (Player Perspective)

1. Reach **90 skill** in a bard skill.
2. Get a **Book of Bard Masteries** and keep it in your backpack.
3. Right-click the book → **Switch Mastery** → pick a path.
4. Cast any mastery from that path with an instrument in hand.

Only one path can be active at a time. Switching paths cancels the current song. If you run out of mana, leave the target's range, or die, the song ends and its buffs fall off.

---

## How It Works (Under the Hood)

- The masteries register as spells **700–705**, so the book displays and casts them like any other spellbook.
- Each ability is a `BardSpell` with an upkeep timer. Every tick it re-checks mana, skill, and range, then either fires its effect or expires.
- Buff strength scales with your bard skills — a higher active-path skill means bigger numbers, and high skill in the other two bard skills adds a small bonus.
- Party members (and their pets) in range get the buff automatically. Leave the party, leave range, or the caster dies, and the buff is removed.
- Your active mastery choice is stored persistently, so it survives a restart.

---

## Files

**Custom (new):**
- `BardMasteries.cs` — the system and all six spells.
- `BookOfBardMasteries.cs` — the book item, its right-click menu, and the mastery selection gump.

**Stock ModernUO files that must be overwritten:**
- `Initializer.cs` — spell registration for IDs 700–705.
- `AOS.cs` — damage and attribute hooks used by the songs.
- `PlayerMobile.cs` — hooks the Invigorate HP bonus into max hit points.

Overwrite these three as-is. They contain the small integration points the masteries need; merging them by hand into an older codebase risks missing one and leaving the songs inert.

---

## Known Limitations

- **Bards only** — weapon and other masteries are not part of this port.
- **No mastery quests** — unlocked purely by skill level, not by a quest like live UO.
- **Party pets count as party members** — they receive buffs and can be healed by Invigorate.
- **No PvP-specific tuning** — same numbers in PvE and PvP.

---

## Requirements

- ModernUO
- `Spellbook`, `BuffInfo`, and `GenericPersistence` support (all stock).

---

## Credits

Ported from ServUO's Skill Mastery system. ModernUO adaptation and bard-only scope by the **UO Wildlands Team**. GPL-3.0.