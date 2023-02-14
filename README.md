# SaveStates
Quick save/load for Phoenotopia: Awakening using Unity Mod Manager

Save and load at the click of a button:
- Press the right analog stick (while centered) to make a **quicksave**
    - Or the Home button on keyboard
- Hold the grab button, then press the camera button to **quickload**

Your quicksave data is stored as a file in the mod folder.

This utility saves and loads
- Location
- Inventory
- Health/energy state
- Game state that is transferred across normal saves (progression flags)

Does not currently save
- Gail's exact state (velocity, rolling/swimming/climbing, etc.)
- State of NPCs, Boxes, Doors
- Projectiles, Bombs
- Pause/Inv menu
- Cutscene/dialogue progress

Multiple save slots, better controls, more detailed state preservation, and more is still under development.

## Known Issues
- Currently does not work from the title screen or main menu. First load into a save file before quickloading to avoid bugging the game out.
- Quickloading will fail if you do it during a level transition, but you can just quickload again.

---
Feel free to @Gyoshi on the Discord [Phoenotopia Fan Server](https://discord.gg/Swd6zcTCQZ) for whatever
