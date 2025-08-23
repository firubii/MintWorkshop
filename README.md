# MintWorkshop
A GUI editor for HAL's Mint bytecode and its updated form found in modern Kirby games, the BoxBoy series, and the mobile version of Part-Time UFO

**For information regarding Mint and Basil versions and instructions, see [this spreadsheet](https://docs.google.com/spreadsheets/d/1A_08ytw1oIBhqBzpkxDIU86RwmYAjG4DopogqCQllMo).**

## Features
* Reads and rebuilds Mint archives that properly function ingame
* A nice color-coded disassembly view
* Adding, editing, and removing classes, variables, functions, and constants
* Loading hashes from an external file (`hashes_<version>.txt`) and automatically generating hashes
* LZ77 Extended (de)compression for Mint archives found in HAL's 3DS games

## Supported Mint Versions and Games
* 0.2
  * Kirby's Return to Dream Land
  * Kirby's Dream Collection
* 1.0.5
  * Kirby: Triple Deluxe
* 1.0.8
  * Kirby Fighters Deluxe
  * Dedede's Drum Dash Deluxe 
* 1.0.13
  * BoxBoy!
* 1.0.17
  * Kirby and the Rainbow Curse
* 1.1.3
  * BoxBoxBoy!
  * Kirby: Planet Robobot
* 1.1.8
  * Kirby's Blowout Blast
* 1.1.12
  * Bye-Bye BoxBoy!
  * Team Kirby Clash Deluxe
* 2.1.4
  * Kirby: Battle Royale
  * Part-Time UFO (iOS/Android)
* 2.1.5.1
  * Kirby: Star Allies
  * Super Kirby Clash
* 2.4
  * Kirby Fighters 2

## Supported Basil Versions and Games
* 7.0.2 (Semi-complete)
  * Kirby and the Forgotten Land
* 7.0.6 (Very incomplete)
  * Kirby's Dream Buffet
* 7.0.8 (Very incomplete)
  * Kirby's Return to Dream Land Deluxe

# Usage
After starting the program, use `File -> Open` or `Ctrl+O` to select a Mint archive to open. (For Kirby's Return to Dream Land and Kirby's Dream Collection, use `File -> Open RtDL` or `Ctrl+Shift+O` instead.)

Wait for the program to read the archive data and populate the tree view on the left. After it has finished, you can expand the nodes to explore the archive.

Right-click on namespaces, scripts, classes, variables, functions, and constants to edit them or add to their contents.
Click on functions to display them in an editor tab on the right, where you are free to edit the instructions in whatever way you like.

Press `Editor->Save Tab` or `Ctrl+S` to assemble and save the currently selected editor tab.

Press `Editor->Close Tab` or `Ctrl+W` to close the currently selected editor tab. Use `Editor->Close All Tabs` or `Ctrl+Shift+W` to close all of them.

Right-click an archive and select `Build` to build the archive in place, where the original opened file is.

Right-click an archive and select `Build As...` to build the archive somewhere else. The place where you build this to is saved and will be used by the `Build` operation.

# Credits

This project uses **BrawlLib** from **BrawlCrate** for LZ77 Extended compression: https://github.com/soopercool101/BrawlCrate
