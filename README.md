# SaveStates
Quick save/load for Phoenotopia: Awakening using Unity Mod Manager
<img src="https://raw.githubusercontent.com/Gyoshi/PhoA-SaveStates/master/Resources/logo.png" alt="Not actual gameplay footage" align="right"/>

Save and load at the click of a button:
- Press the right analog stick (while centered) to make a **quicksave**
- Hold the grab button, then press the camera button to **quickload**
- Or the Home/End buttons on keyboard

You can swap between 16 quicksave slots by using RShift + left/right arrow keys.

Your quicksave data is stored as a file in the mod folder. Each slot has a separate file.

*Better controls, more detailed state preservation, and more is still under development.*


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
