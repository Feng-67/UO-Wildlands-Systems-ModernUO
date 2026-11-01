# Free Skills for ModernUO

A core mechanic that lets **twenty skills** grow without counting toward a character's skill cap. Players can grandmaster these skills alongside a full 720-point build, and the server presents them to the client as "0 base" so they never eat into the total shown on the skills menu.

This is part of ModernUO's stock skill system (in `Skills.cs`), not a custom script — but it's a rule worth documenting because it quietly affects skill gain, soulstones, NPC training, and the client display.

---

## The Skills

The following skills are free:

| Crafting & Gathering | Utility | Support |
|----------------------|---------|---------|
| Alchemy | Begging | Arms Lore |
| Cartography | Camping | Detect Hidden |
| Cooking | Fishing | Forensics |
| Inscribe | Herding | Item ID |
| Lumberjacking | Hiding | Taste ID |
| Mining | Lockpicking | Tracking |
| Imbuing | Remove Trap | |

That's twenty in total. Everything else — combat skills, magery, taming, blacksmithing, tailoring, the crafting skills tied to BODs, and so on — counts normally.

---

## What "Free" Actually Means

A free skill is treated as if it doesn't exist when the game tallies up a character's total skill points. The practical effects:

- **No cap cost** — a grandmaster fisher with 100 fishing still shows 0 skill points spent, so a player can GM every free skill *and* still use the full 720-point cap on real skills.
- **Always gains** — free skills can still go up when the player's total is at cap. There's no "you must drop something else first" message.
- **No atrophy** — gaining a free skill never forces another skill downward, even at full cap.
- **Client displays 0 base** — the skills packet sends a base of 0 for these skills, so the skills menu and skill tracker don't count them. The player still sees their real value in the skill list.
- **Soulstones absorb for free** — storing or retrieving a free skill from a soulstone never requires the player to free up cap space.
- **NPC training ignores cap** — when a NPC teaches a free skill (via the standard teaching system), it doesn't check whether the player has room under the cap.

---

## How It Works

The list lives in one place — `SkillInfo.FreeSkills`, a `HashSet<SkillName>` in `Skills.cs`. Every skill-related code path that needs to know "does this count?" checks that set. That means:

- **Skill gain** skips the atrophy roll for free skills and lets them gain unconditionally.
- **Skill serialization** leaves them out of the saved `Total`.
- **Skill packets** send `0` for the base value so the client ignores them.
- **Soulstone transfer** treats the required-cap amount as zero.
- **NPC teaching** skips the "you don't have enough free points" check.

Because every consumer reads from the same set, adding or removing a skill from it takes effect everywhere at once — no per-system changes needed.

---

## A Note on Classic UO

The list matches the classic "free skills" from live Ultima Online, with **Imbuing** added in the Stygian Abyss era. ModernUO ships with Imbuing in the set because the imbuing skill works the same way there — it's a crafting/gathering skill that players are expected to max out without sacrificing a combat or taming build.

If your shard customizes the skill list or the cap behaviour, this is the one place to edit.

---

## Known Limitations

- **The set is static** — it's a `static readonly HashSet` populated in `SkillInfo`. Changing it requires a recompile; there's no config file or runtime toggle.
- **Not per-character** — every character on the shard uses the same free-skill list. There's no way to make a skill free for one player and not another.
- **Cap is still enforced on the individual skill** — "free" means it doesn't count toward the *total* cap; each skill still has its own 100 (or power-scrolled 120) cap.
- **Free skills can still be set to "Down"** — a player who wants to sacrifice, say, Mining to feed another skill can still lock it downward manually. The free-skill rule only affects the automatic atrophy path.

---

## Where It Lives

- **`Skills.cs`** — `SkillInfo.FreeSkills`, plus the skill gain, serialization, and teaching logic that references it.
- **`OutgoingPlayerPackets.cs`** — the skill packets that zero out free-skill bases for the client.
- **`SkillCheck.cs`** — the gain routine that skips atrophy and cap checks for free skills.
- **`SoulStone.cs`** — the transfer logic that ignores cap requirements for free skills.

---

## Credits

Part of ModernUO's stock skill system. Documented for the UO Wildlands shard.