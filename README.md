# ModernUO – Custom Scripts

**Custom file systems for ModernUO Ultima Online servers.**

---

[![YouTube Channel](https://img.shields.io/badge/YouTube-Warrior%20of%20Destiny-lightgrey?style=for-the-badge&logo=youtube&logoColor=white&labelColor=FF0000)](https://www.youtube.com/@Warrior_of_Destiny)

---

## What's Inside

A collection of drop-in custom systems for ModernUO shards. Each system is self-contained, documented, and designed to compile against a stock ModernUO build.

| System | Summary |
|--------|---------|
| **MyStats** | One-window statistics dashboard — attributes, resistances, combat and magic ratings, plus a filtered skill list. |
| **Pet&nbsp;Training** | Full port of the classic Animal Training system, including stats, resists, skill caps, abilities and a companion shrink system. |
| **Companion,&nbsp;Legendary&nbsp;&&nbsp;Named&nbsp;Pets** | Three tiers of tamable pets: cosmetic Companion dogs delivered as blessed statuettes, wilderness Legendary variants of familiar creatures, and seventeen unique Named creatures. All fully trainable through the Pet Training system. |

---

## Requirements

- **ModernUO** — built and tested against the current release.
- **.NET SDK** — whatever the current ModernUO build expects (10.0.x at time of writing).
- **Two stock files are replaced** — `Scripts/SkillHandlers/AnimalLore.cs` and `Scripts/Mobiles/AIType.cs`. Back these up before deploying. Details are in each system's documentation.

---

## Getting Started

1. Clone or download this repository.
2. Copy the files into your ModernUO `Scripts/` folder following the layout in each system's doc.
3. Back up the two stock files listed above, then overwrite them.
4. `dotnet build` — the project should compile cleanly.
5. Test on a copy of your save data before going live.

---

## Notes on the Pet Collection

The **Companion, Legendary & Named Pets** pack builds on the **Pet Training** system — pets from all three tiers use the same training gump and ability packages, so no additional setup is needed beyond having Pet Training installed. These custom pets are untested.

A few things worth knowing before deploying:

- **Legendary variants replace their stock counterparts.** TsukiWolf, ShadowWyrm, PolarBear, Phoenix, OsseinRam, Nightmare, Kirin and FireSteed must be spawned as this pack's versions for the legendary chance to fire. Stock ModernUO versions of these creatures won't produce Legendary rolls.
- **Companion statuettes are blessed** — they can't be lost on death or stolen, making them safe for an in-game store, subscriber rewards, or quest prizes. Redemption requires no taming skill; wild-spawned Companions require 100.0 Animal Taming.
- **Named pets have no default spawn location.** They're self-contained creatures with full stats and training packages — where and how they appear is entirely up to the shard admin.

---

#### License **GPL-3.0**