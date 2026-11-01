# Custom Items for ModernUO

A collection of custom items created for the UO Wildlands shard — utility items, containers, dyes, instruments, and quality-of-life tools that aren't part of stock ModernUO.

---

## The Full List

| Item | What it does |
|------|--------------|
| **Advanced Interior Decorator** | House decorator with move/turn/hue/lock commands in any of eight directions. |
| **Alacrity Scroll Picker** | Book that lets the player pick *any* skill for a 15-minute accelerated skillgain boost. |
| **Power Scroll Picker** | Deed (105/110/115/120) that lets the player choose which skill the scroll applies to. |
| **Champion Monolith** | Statue that teleports players to any champion spawn altar, with cooldown status shown. |
| **Vase of Virtue** | Teleports players to any of the twelve virtue shrines across Trammel, Felucca, and Tokuno. |
| **Cities of Britannia Atlas** | Pre-filled runic atlas with every major city on Trammel, Felucca, Tokuno, Malas, and Ter Mur. |
| **Runic Atlas (Exceptional)** | Exceptional-quality runic atlas with 100 charges, ready to be filled in. |
| **Compassion Pigment** | Single-hue dye with 5 uses; 28 colour variants available. |
| **Cub Store Dyes** | 28 single-hue dye tubs (5 uses each) matching the Compassion Pigment colours. |
| **Pet Dye Tubs** | Permanent single-use dyes that permanently recolour a player's pet. |
| **Pet Dye Samples** | 30-second temporary pet dyes for previewing a colour before committing. |
| **PetDyeTub** | Reusable dye tub that recolours any pet you own. |
| **Spellbook Dye Tub** | Dyes spellbooks to any colour. |
| **Scroll of Decurse** | Removes the Cursed flag from any item in your pack or worn. |
| **Transmog Potion** | Changes an item's appearance to match another item (respecting one-/two-handed weapons). |
| **Elixir of Rebirth** | Instantly resurrects a dead bonded pet. |
| **Break-Paralysis Potion** | Cures paralysis and frozen states, consumed one dose at a time. |
| **Mark Rune Stone** | Withdraws 25 gp from your bank to create a marked rune at your current location. |
| **Instrument Case** | Holds instruments with 50 % weight reduction. |
| **Runebook Strap** | Holds runebooks and runic atlases with 100 % weight reduction. |
| **Spellbook Strap** | Holds spellbooks with 100 % weight reduction and lets you cast without removing the book. |
| **Weapon Strap** | Holds melee weapons with 50 % weight reduction; worn as a cloak. |
| **Large BOD Deed Box** | Splits a Large Bulk Order Deed into its component Small BODs. |
| **Full Spellbooks** | Complete spellbooks for every school: Magery, Necromancy, Chivalry, Bushido, Ninjitsu, Spellweaving, Mysticism. |
| **Nature's Tears / Primordial Decay / Arachnid Doom** | Exceptional instruments with dual slayer properties and 450 uses. |
| **Custom Stat Roses** | Roses that produce +5 stat petals (Str/Dex/Int) once every 4 hours, up to 10 at a time. |
| **Trash Barrel (Portable)** | Movable trash barrel that only works while locked down or secured in a house. |
| **Store Veteran Clothing** | Blessed valorite-hued cloak, robe, and dress for display or wear. |

---

## Feature Items

### Champion Monolith

A locked-down skull monolith that acts as a shard-wide champion spawn directory, letting players browse every champion altar at a glance instead of hunting for one in person. Opening it lists all altars alphabetically with their current status — **ACTIVE** (green, meaning a champion is currently up), **READY** (white, meaning the cooldown has finished and the altar can be started), or **COOLDOWN** (grey, with the remaining minutes shown) — so a player can see at a glance which altars are worth heading to without leaving the house. Clicking any row teleports the player and their pets to a hand-tuned arrival point just outside the altar (each of the 30+ altars has a curated coordinate pair rather than a generic offset, so players don't spawn inside terrain), with a fallback to a safe nearby spot and a warning message if an altar isn't in the coordinate table. The monolith only works while locked down for regular players, requires the player to stay within 3 tiles when confirming the teleport, and warns staff (who can test it anywhere without lockdown) that the monolith's power can only be harnessed when secured — all of which keeps it a legitimate house utility rather than a portable escape tool.

### Scroll Pickers (Power & Alacrity)

The Power Scroll Picker and Alacrity Scroll Picker replace the fixed-skill behaviour of the stock scrolls with a menu that lets the player choose any skill from the full list. Power Scroll Picker deeds come in 105, 110, 115, and 120 variants and produce a normal Power Scroll for the chosen skill at that level, so a player who loots a 120 deed can apply it to whichever skill they're actually working on rather than being forced into whatever the loot roll gave them. The Alacrity Scroll Picker works the same way but grants a 15-minute accelerated skillgain effect on the chosen skill (stacking with normal Scroll of Alacrity rules, so only one accelerated skill can be active at a time) and consumes itself on use. Both items check that the deed is still in the player's pack when the menu is confirmed, so trading or dropping them mid-selection safely cancels the effect.

### Compassion Pigments & Cub Store Dyes

A coordinated collection of 28 shard-exclusive colours, split between single-use dye pots (Compassion Pigments, 5 uses each) and reusable-hue store dye tubs (Cub Store Dyes, also 5 uses) — both share the same underlying palette, so players can mix and match the two formats without ending up with slightly different shades. Colours range from classic staples like Phoenix Red, Lavender, and Deep Violet to newer additions like Star Blue, Mottled Sunset Blue, and Intense Teal, each with its own hue value and cliloc name so they appear distinctly in tooltips and trade windows. The dyes accept the usual selection of leather, metal, cloth, jewellery, weapons, talismans, and books (via the Cub Store version) while refusing to touch artifacts, blessed items, and other restricted categories, and they consume a use on every successful application so the palette stays meaningful as a status symbol rather than a cosmetic free-for-all.

---

## Notes

- Most items carry a **legacy `Deserialize` hook** that reads the old ServUO format so pre-existing saves continue to load cleanly.
- The three slayer instruments (**Nature's Tears**, **Primordial Decay**, **Arachnid Doom**) use placeholder harp graphics and sounds — swap in the real values once you have them.
- **Store Veteran Clothing** is a partial file; only the human/elf cloak, robe, and dress are implemented so far.

---

## Credits

Original custom content by the **UO Wildlands Team**. GPL-3.0. Several scripts derived from ServUO Core and community sources (original authors noted in the file headers where known).