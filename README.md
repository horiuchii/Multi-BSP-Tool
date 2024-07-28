# Multi BSP Tool
A tool for packing specific folders into BSP files without hassle.

## Usage
Create a `commands.txt` file next to the executable or define one with the `-commands` parameter.

The commands file takes a list of commands and executes them in order.

### Commands
 - `bspzip` - Defines a `bspzip.exe` to use for packing or repacking BSP files.
 - `game` - Defines a game folder containing the `maps` and `custom` folders to search for assets.
 - `map` - Sets an input map to operate on relative to `game/maps/`.
 - `new_map` - Copy the input map and operate on this map instead of overwriting the input map relative to `game/maps`.
 - `folder` - Pack this folder into the current input map.
 - `custom_folder` - Pack this folder relative to `game/custom/` into the current input map.
 - `repack` - Repack the current input map with the option to compress.

 ### Example
 ```
bspzip=F:\SteamLibrary\steamapps\common\Team Fortress 2\bin\x64\bspzip.exe
game=F:\SteamLibrary\steamapps\common\Team Fortress 2\tf\

# Ignores commented lines!
//map=2fort
//new_map=2fort_new
//custom_folder=eclipse-assets

// Copy cp_badlands and copy our assets and resave to a new BSP.
// Make sure to uncompress compressed maps before operating on them.
map=cp_badlands
new_map=cp_badlands_w_eclipse_assets
repack=false
custom_folder=eclipse-assets
repack=true

//Overwrites the original cp_granary.
map=cp_granary
repack=false
custom_folder=eclipse-assets
repack=true
 ```

 ### Whitelist
 The tool will only pack files with the following extensions:
  - `mp3` `wav`
  - `vmt` `vtf`
  - `nut` `nuc`
  - `pcf` `res`
  - `ain` `nav` `ani` `rad` `vbsp`
  - `mdl` `vvd` `vtx` `phy`
  - `txt` `jpg`