# ModernUO – Custom Scripts

**Custom file systems for ModernUO Ultima Online servers.**

---

[![YouTube Channel](https://img.shields.io/badge/YouTube-Warrior%20of%20Destiny-lightgrey?style=for-the-badge&logo=youtube&logoColor=white&labelColor=FF0000)](https://www.youtube.com/@Warrior_of_Destiny)

---

## What's Inside

A collection of drop-in custom systems for ModernUO shards. Each system is self-contained, documented, and designed to compile against a stock ModernUO build.

| System | Summary |
|--------|---------|
| [**MyStats**](docs/MyStats.md) | One-window statistics dashboard — attributes, resistances, combat and magic ratings, plus a filtered skill list. |
| [**Pet&nbsp;Training**](docs/PetTraining.md) | Full port of the classic Animal Training system, including stats, resists, skill caps, abilities and a companion shrink system. |

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



#### License **GPL-3.0**
