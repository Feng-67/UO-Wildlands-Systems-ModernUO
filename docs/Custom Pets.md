# UO Wildlands — Companion, Legendary & Named Pets

A pet collection for ModernUO. This pack adds three distinct tiers of pets to your shard: store purchasable **Companions**, wilderness **Legendary** variants of familiar creatures, and unique **Named** creatures not ordinarily spawned in the world.

All three tiers are fully trainable through the pet training system (included in Training Definitions) — players can train them through the Animal Lore gump just like any other tamable creature.

---

## Overview

| Tier | What They Are | Taming Required |
|------|---------------|-----------------|
| **Companions** | Six dog breeds, delivered as blessed statuettes | None (statuette-granted) |
| **Legendary Pets** | Eight rare wilderness variants of familiar creatures | Yes (~120 Animal Taming) |
| **Named Pets** | Seventeen unique prestige creatures | Yes (108–120 Animal Taming) |

---

## Companions

Six faithful dog breeds, delivered as **blessed statuettes**. Double-click the statuette from your backpack, confirm, and the pet appears at your side, fully under your control. If a shard admin prefers to spawn these pets in the world rather than distribute statuettes, a minimum of 100.0 Animal Taming will be required for players to tame them.

**The six breeds:**

- **Alaskan Malamute**
- **Great Dane**
- **Newfoundland**
- **Rottweiler**
- **Russian Terrier**
- **Saint Bernard**

**What makes them special:**

- All six share the same combat profile — the choice is purely cosmetic and personal
- Each comes as a blessed statuette, so it cannot be lost on death or stolen
- Redeemable at any time — no taming skill required
- Full pet training system supported — no different from raising any other companion

Because the statuettes are blessed, they're ideal for an **in-game store**, rewards, or quest prizes.

---

## Legendary Pets

Eight familiar creatures can spawn as **Legendary** variants in the wild. These aren't separate creatures — they're rare rolls that happen when the base creature spawns.

When a Legendary spawns, it gains:

- **A unique colored hue** — a striking shade that immediately marks it as special
- **A gold "Legendary" tag** in its name that other players can see
- **Boosted stats** — higher health, stronger attacks, better resistances
- **Reduced control slots** — legendary pets are easier to keep, not harder
- **A signature ability** the base version doesn't have
- **A higher taming requirement** — usually 120 Animal Taming

**⚠️ Important — Legendary pets replace their stock counterparts.**

The Legendary variant lives inside the base creature's class. For the legendary chance to occur, you must **spawn these versions in your world instead of the stock ModernUO creatures**. If your shard is running stock TsukiWolves, ShadowWyrms, and Nightmares, players will never see a legendary variant of them.

**The eight legendary-capable creatures:**

| Creature | Spawn Chance | Legendary Upgrade |
|----------|--------------|-------------------|
| **TsukiWolf** | 10% | Unique hue list, `LifeLeech` ability, 2 control slots |
| **ShadowWyrm** | 10% | Dark hue, 80% poison damage, `LifeLeech`, 2 control slots |
| **Polar Bear** | 10% (Winter Spur, Tokuno only) | Combat AI, significantly boosted stats, `ColossalRage`, 2 control slots |
| **Phoenix** | 15% (three variants) | One of three schools: Magery Mastery, Mysticism, or Spellweaving |
| **OsseinRam** | 10% | Necromage school, extended Necromancy skill, 1 control slot |
| **Nightmare** | 10% | `Heal` ability, `Healing` skill, distinctive hue |
| **Kirin** | 10% (fire or cold variant) | Melee AI, elemental damage focus, matching abilities |
| **FireSteed** | 10% | `Inferno` ability, boosted stats, 1 control slot |

**How they play out:**

A group hunting in Tokuno might spot a TsukiWolf. Nine times out of ten it's an ordinary one. But every so often, the wild wolf has a *legendary* hue — a vivid color that catches the eye from a screen away, with a gold tag floating above its head. If the group has a tamer with 120 skill and a spare control slot, that's the moment to stop and tame.

Legendary pets reward players who know where to look. Spawners generate a limited number of creatures in a fixed region, so a player who learns where a creature spawns can farm the stock versions until a Legendary rolls. This pack ships at 10% to make Legendaries easy to encounter during testing. 5% is the recommended live rate — on par with standard artifact drops from bosses. Admins can go lower still if they want a rarer, chase-goal feel.

To adjust the rate, look for Utility.RandomDouble() < 0.10 in each Legendary creature's OnBeforeSpawn method, and change 0.10 to 0.05 (or your preferred rate).

---

## Named Pets

Seventeen high-end, prestige pets. Each is a unique creature with its own theme, custom stats, and full training package.

**The seventeen Named pets:**

| Pet | Theme |
|-----|-------|
| **Black Unicorn** | Corrupted fey |
| **Bone Cat** | Undead feline |
| **Capybara** | Giant rodent |
| **Dread Hound** | Infernal canine |
| **Ancient Hound** | Mythic hound |
| **Dread Steed** | Demonic mount |
| **Dread Wolf** | Samurai dire wolf |
| **Frost Mite Mount** | Icy insectoid |
| **Juvenile Manticore** | Draconic beast |
| **Juvenile Umbrascale** | Shadow drake |
| **Kuraku Dread** | Dinosaur lord |
| **Moldering Ursine** | Undead bear |
| **Moon Tail** | Celestial creature |
| **Rainbow Unicorn** | Radiant fey |
| **Shadow Spider** | Ambush predator |
| **Skeletal Horse** | Undead mount |
| **Uber Turkey** | Unusually large pet |

Each Named pet has:

- **A unique taming requirement** — usually 108, sometimes 120 Animal Taming
- **3 control slots** — standard starting point that can be tweaked after testing
- **A full training package** with a curated ability list, magical schools, weapon abilities, and area effects

**What makes them distinct from Legendary pets:**

Legendary pets are *rare versions of everyday creatures* — the surprise of finding a wolf with a golden tag. Named pets are *specific, one-of-a-kind creatures* — each one is a named entity with its own identity, abilities, and purpose in your world.

How they appear in your shard is entirely up to you. Spawn them directly, place them at the end of custom content, use them as reward markers, or gate them behind whatever encounter design fits your shard. The creatures themselves are self-contained and ready to use.

---

## Quick Reference

| You want... | The player gets... |
|-------------|-------------------|
| A guaranteed, cosmetic companion | A **Companion statuette** (blessed, redeemable) |
| A rare wilderness discovery | **Legendary-capable creatures** spawned in the world |
| A prestige or endgame pet | A **Named creature** |

---

## Notes for Shard Admins

- **Legendary pets must replace their stock versions if they currently exist.** This is the one hard requirement. Spawn `TsukiWolf`, `ShadowWyrm`, `PolarBear`, `Phoenix`, `OsseinRam`, `Nightmare`, `Kirin`, and `FireSteed` instead of the standard creature types for the legendary chance to fire.

---

## Summary

- **Companions** are six cosmetic dog breeds delivered as blessed statuettes — ideal for an in-game store, subscriber rewards, or quest prizes.
- **Legendary Pets** are eight rare wilderness spawns that replace their stock counterparts — exploration rewards with unique hues, extra abilities, and a gold name tag.
- **Named Pets** are seventeen unique prestige creatures — high tame requirements, full training packages, and their own identities.

Each tier supports the same pet training system, so players can invest in any pet they obtain. The question is just which one they want to earn, tame, and raise.