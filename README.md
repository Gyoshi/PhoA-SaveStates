# SaveStates
Quick save/load for Phoenotopia: Awakening using Unity Mod Manager
<img src="https://raw.githubusercontent.com/Gyoshi/PhoA-SaveStates/master/Resources/logo.png" alt="Not actual gameplay footage" align="right"/>

While holding the camera button:
- Press the right analog stick (while centered) to make a **quicksave**
- Press RB (grab button) to **quickload**
- Press LT/RT to swap between save **slots**
- On keyboard, the following keys also work correspondingly: Home/End, PgUp/PgDn
- Press the Alt Tool button to use the regular camera feature.

You can swap between 16 quicksave slots. The save data of each slot is stored as a file in the mod folder.

*This tool is still under development.*

## State preservation
Everything related to Gail and her progress through the story is preserved across quicksaves and -loads. Most of the state of the npcs and objects in a room is not. More precisely:

This utility saves and loads
- Location
- Inventory
- Health/energy state
- Game state that is transferred across normal saves (progression flags, chrysalis locations, etc.)
- State of doors, boxes

Does not currently save
- Gail's exact state (velocity, rolling/swimming/climbing, etc.)
- State of NPCs
- Location of Boxes
- Projectiles, Bombs
- Pause/Inv menu
- Cutscene/dialogue progress

## Installation
Requires [Unity Mod Manager](https://www.nexusmods.com/site/mods/21/). You just need the `PhoA-SaveStates.zip` file [(which you can download here)](https://github.com/Gyoshi/PhoA-SaveStates/releases/latest), and then follow the mod installation instructions for Unity Mod Manager.

## Known Issues
- Currently does not work from the title screen or main menu. First load into a save file before quickloading to avoid bugging the game out.
- Quickloading will fail if you do it during a level transition, but you can just quickload again.

---
Feel free to @Gyoshi on the Discord [Phoenotopia Fan Server](https://discord.gg/Swd6zcTCQZ) for whatever

For more info about what I plan to work on, you can look at the [PhoA SaveStates Trello board](https://trello.com/b/LoMwIPi0/phoa-savestates). I don't really expect anyone to be that interested, but I put up a pretty background, so I felt like sharing

source code: https://github.com/Gyoshi/PhoA-SaveStates
