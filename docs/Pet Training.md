\# Pet Training System



A full port of the classic Animal Training system to ModernUO. Players can take their pets through a structured training path — raising stats, resistances, skill caps, and teaching them magical abilities, special attacks and area effects — all through a custom in-game window.



\## The Basics for Players



\*\*Train a pet\*\* by using the \*\*Animal Lore skill\*\* on it. Instead of the old vanilla lore window, a full pet dashboard opens.



From the dashboard a player can:

\- See the pet's complete stats, resistances, damage types, skills and special abilities in one place.

\- \*\*Begin Animal Training\*\* — starts the pet on the training path.

\- \*\*View Options\*\* — opens the training menu where points are spent.



Training is paid for with \*\*training points\*\* the pet earns by fighting alongside its owner. Higher-slot pets earn points more slowly — a deliberate trade-off for their extra power.



\## The Training Menu



Pets are trained category by category, and each pet type has its own set of options (a Dragon can learn things a Horse cannot):



| Category | What it covers |

|----------|----------------|

| \*\*Stats\*\* | Strength, Dexterity, Intelligence, Hit Points, Stamina, Mana, regeneration and base damage |

| \*\*Resists\*\* | Physical, Fire, Cold, Poison and Energy |

| \*\*Magic Skill Caps\*\* | Magery, Necromancy, Chivalry, Bushido, Ninjitsu, Spellweaving, Mysticism and more |

| \*\*Combat Skill Caps\*\* | Wrestling, Tactics, Anatomy, Parry and Healing |

| \*\*Magical Abilities\*\* | Schools such as Poisoning, Magery, Necromancy and the weapon masteries |

| \*\*Special Abilities\*\* | Dragon Breath, Inferno, Rune Corruption, Tail Swipe, Venomous Bite and many others |

| \*\*Special Moves\*\* | Armour Ignore, Bleed Attack, Mortal Strike, Paralyzing Blow and more |

| \*\*Area Effects\*\* | Auras of fire, cold, poison, disease and energy |



Some abilities are \*\*rule-breakers\*\* — big, flashy moves that a pet can only learn if it has committed to a particular mastery path. The menu enforces these rules automatically, so players can't build something impossible.



\## Planning Mode



Players who want to map out a full build before spending a single point can switch the training menu into \*\*Planning Mode\*\*. Selections get added to a plan showing the total cost, and players can add or remove entries freely until they're happy — then train for real.



\## Staff Tools



These are for GMs and admins:



| Command / Item | What it does |

|----------------|--------------|

| `\[PetTrainTest <creature>` | Spawns a fully tamed test pet of the named type, ready for training. Use it to check how a new creature trains before releasing it to players. |

| `\[add PetTrainingGate` | A placeable gate. Any pet that walks through while its owner is nearby instantly fills its training bar — handy for events or for skipping the grind on a test shard. |

| `\[add PetBondRemoveGate` | A placeable gate that instantly bonds any pet that passes through. |

| `\[add PowerScrollGiver` | A placeable object. Double-click it to receive a bag containing every +20 power scroll relevant to pet training. |



\## Shrink System



The bundle includes a companion pet-shrinking system, useful for players who want to carry pets around without them following.



| Item | How it works |

|------|--------------|

| \*\*Bottle of Shrinking\*\* | Stackable. Double-click, target a pet, and it becomes a shrunken-pet item in your backpack. |

| \*\*Pet Leash\*\* | Same idea, but with a limited number of charges instead of being stackable. |



To restore a pet, double-click the shrunken-pet item in your backpack — it reappears next to you.



\*\*Staff command:\*\* `\[SSToggle` (Administrator) switches the entire shrink system on or off shard-wide.



Pets can't be shrunk in combat, while polymorphed, while summoned, or while carrying items — and mounts can't be unshrunk inside dungeons. All of these checks are handled automatically.



\## Key Features at a Glance



\- \*\*Per-creature training paths\*\* — every pet type has its own curated list of what it can and can't learn.

\- \*\*Points-based progression\*\* — pets earn training points through combat, encouraging players to actually use their pets.

\- \*\*Control slot growth\*\* — a pet can grow into more control slots as it trains, with a warning shown first so nobody is caught out.

\- \*\*Full save compatibility\*\* — trained pets persist correctly across restarts.

\- \*\*Integrated with Animal Lore\*\* — the standard skill now opens the custom dashboard, so there's nothing new for players to learn.



\## A Note for Shard Owners



The system is a faithful port, but a handful of features from the original depend on custom systems that a standard ModernUO install doesn't include (custom damage absorbers, special shard-specific creatures, and a few unique AI behaviours). Those pieces are either stubbed out harmlessly or omitted — nothing breaks, and nothing requires configuration. If you later add those systems to your shard, they can be hooked back in without touching the pet training files themselves.

