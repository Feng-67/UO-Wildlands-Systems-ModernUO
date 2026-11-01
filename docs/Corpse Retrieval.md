## Corpse Retrieval System Overview

* Provides a death recovery and corpse retrieval framework for ModernUO/ServUO.


* Automates teleportation to resurrection locations.


* Allows instant corpse retrieval with smart paperdoll re-equipping.


* Includes a gold-sink retrieval fee.



## Key Features

* **Automated Death Teleport:** Dead players logging in are presented with a prompt to teleport to the Corpse Retrieval Stone.


* **On-Demand Gump Access:** Players can reopen the teleport prompt while dead using the `[Rez` command.


* **Proximity Resurrection:** Ghosts approaching or double-clicking the retrieval stone are offered resurrection.


* **Smart Equipment Restoration:** Retrieving a corpse teleports it to the player and triggers `Corpse.Open()`, re-equipping worn gear and restoring backpack slots.


* **Gold Penalty & Fee:** Charges a 15,000 Gold fee (drawn from Backpack, Bank, or Bank Box) and destroys gold carried inside the corpse.


* **Side-Effect-Free Cancellation:** Gold destruction and fee processing occur only upon payment confirmation.



## Player Workflow

* Player dies or logs in as a ghost.


* `DeathTeleportGump` prompts the player (can be reopened via `[Rez`).


* Player selects CONTINUE to teleport to the stone at 3471, 2601, 10 in Trammel.


* An automatic resurrection prompt is offered upon arrival.


* Player double-clicks the stone to open `CorpseRetrievalGump`.


* Player confirms the 15,000 Gold fee.


* Corpse gold is destroyed, corpse is teleported to the player, and items are automatically re-equipped and restored.



## Commands & Configuration

* **Command `[Rez`:** Reopens the `DeathTeleportGump` prompt for players who are currently dead.


* **Retrieval Fee:** Set to 15,000 Gold in `CorpseRetrievalStone.cs` (`_totalCost`), deducted from Backpack or Bank.


* **Teleport Location:** Set to (3471, 2601, 10) in Trammel within `DeathTeleportGump.cs` (`OnResponse`).


* **Resurrect Range:** Set to 3 tiles in `CorpseRetrievalStone.cs` (`ResurrectRange`) for triggering resurrection.



## File Architecture

* **`CorpseRetrievalStone.cs`:** Contains `ResurrectHelper`, `CorpseRetrievalStoneWest`, `CorpseRetrievalStoneNorth`, recursive gold deletion, bank payment handler, and `CorpseRetrievalStoneGump`.


* **`DeathTeleportCommand.cs`:** Registers the `[Rez` command for players.


* **`DeathTeleportGump.cs`:** Renders the teleport prompt and executes the map transfer.



## Technical Details

* **Payment Resolution Hierarchy:** Processes fees via Game Master bypass, Backpack withdrawal, `Banker.Withdraw()`, and `FindBankNoCreate()` container check sequentially.


* **Gold Destruction Routine:** `ResurrectHelper.DeleteGoldRecursively()` scans all corpse item layers (including nested bags) and deletes `Gold` instances before moving the corpse.