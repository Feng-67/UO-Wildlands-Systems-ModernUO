# Mannequin System for ModernUO

A custom **mannequin** NPC for **ModernUO** that lets players display full outfits in their homes and compare gear stat-by-stat before swapping it in. Great for players who like to optimize builds and see exactly what a new item will change.

This is an original UO Wildlands custom script (not a port), licensed under GPL-3.0.

---

## Features

- **Placeable in houses** — use a Mannequin Deed to place one in a house you own or co-own.
- **Full equipment display** — dress the mannequin the same way you dress yourself.
- **Owner-only control** — only the owner (or a GM) can equip, rotate, or redeed it. Everyone else just sees the paperdoll.
- **Suit stats gump** — view the combined stats of the whole outfit, split into easy categories: Attributes, Resistances, Combat, Casting, Misc, Hit Effects, and Skill Bonuses.
- **Item comparison** — compare any item in your pack against what's equipped, with green/red numbers showing the difference at a glance.
- **Preview mode** — see what the outfit's stats would look like *with* the new item swapped in.
- **Short descriptions** — add a small tooltip line to the mannequin (e.g. *"Mage suit v3"*).
- **Switch clothes** — swap your currently worn gear with the mannequin's in one click.
- **Rotate and redeed** — turn the mannequin to face any direction, or pack it back into a blessed deed.

---

## How It Works (Player Perspective)

1. Use a **Mannequin Deed** inside your house and target a valid spot.
2. Double-click the mannequin to see its paperdoll, then drag items onto it to equip.
3. Right-click the mannequin for options:
   - **View Suit Stats** — aggregated stats across the whole outfit.
   - **Compare With Selected Item** — target an item to see a side-by-side of what's equipped vs. your candidate.
   - **View Suit Stats With Selected Item** — same as above, but shows the whole outfit's stats with your item swapped in.
   - **Add Description** — set a short tooltip line.
   - **Switch Clothes** — swap your worn gear with the mannequin's.
   - **Rotate** / **Redeed** — turn it, or pack it back into a deed.

---

## Adding a New Property

New stats are picked up automatically. To add one:

1. Pick the right base class for the stat type (attribute, weapon attribute, resistance, skill bonus, slayer, or generic).
2. Override the label number, icon, colour, and cap.
3. Add it — the mannequin scans for property classes on startup, so no registration is needed.

---

## Known Limitations

- **Descriptions are capped at 44 characters** (client text-entry limit).
- **Medable Armor doesn't show in the comparison gump** — it's a yes/no flag, so it's not a useful side-by-side number.
- **The Next Page button uses the wrong arrow graphic** on the first page. Cosmetic only; paging still works.
- **Ownership follows placement** — whoever places the deed becomes the owner, so a house co-owner could take it.
- **No filtering in the comparison view** — every property with a value on either side is listed, so busy suits can make for a long gump.

---

## Requirements

- ModernUO
- The housing system (for placement and ownership rules)
- ModernUO's `DynamicGump` UI framework

---

## Credits

Original custom system by the **UO Wildlands Team**. GPL-3.0. Property modelling adapted from ServUO's mannequin system and community stats scripts (original authors unknown).