# MyStats — Player Statistics Dashboard

A simple in-game command that gives players a single, readable window showing everything about their character — no more digging through the paperdoll and skill menu.

## The Command

```
[mystats
```

Available to **all players**. Opens the dashboard instantly.

## What It Shows

The window is split into clear sections so players can see their whole build at a glance:

- **Attributes** — Strength, Dexterity and Intelligence, alongside current/max Hit Points, Stamina and Mana.
- **Resistances** — Physical, Fire, Cold, Poison and Energy, each colour-coded so weaknesses stand out.
- **Combat Ratings** — HCI, DCI, DI and SSI (with their caps shown next to them), plus swing speed, bandage speed, regeneration rates, stat bonuses, followers and weight.
- **Magic Ratings** — SDI, LRC, LMC, FC and FCR.
- **Miscellaneous** — Luck, Fame, Karma, Tithing points and total account gold.
- **Skills** — only the skills the character has actually invested in. Human characters won't see all their free 20.0 "Jack of All Trades" skills cluttering the list unless gear has pushed them higher.

## Key Features

- **One click, one window** — everything a player needs to plan a template or check a weakness.
- **Live values** — swing speed is worked out from the weapon actually equipped, and lower-ammo comes from the quiver being worn.
- **Stays current** — the refresh button re-reads everything, so players can leave it open and tap refresh after changing gear.
- **Clean skill list** — filters out skills a character doesn't use, so the list stays short and useful.

## Stubbed Fields

Two fields — **Damage Eater** and **CUB Points** — depend on custom systems that aren't part of a standard ModernUO install. They'll display as `0` unless those systems are added to your shard. Everything else works out of the box.
