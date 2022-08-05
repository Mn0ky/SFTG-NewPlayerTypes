# NewPlayerTypes

<p style="text-align: center;">
  <a href="https://forthebadge.com">
    <img src="https://forthebadge.com/images/badges/made-with-c-sharp.svg" alt="">
  </a>
</p>
<p style="text-align: center;">
  <a href="https://github.com/Mn0ky/SFTG-NewPlayerTypes/releases/latest">
    <img src="https://img.shields.io/github/downloads/Mn0ky/SFTG-NewPlayerTypes/total?label=Github%20downloads&logo=github" alt="">
  </a>
  <a href="https://www.gnu.org/licenses/MIT">
    <img src="https://img.shields.io/badge/MIT-blue.svg" alt="">
  </a>
</p>

https://user-images.githubusercontent.com/67206766/182985003-4b4d977e-a132-4fd0-b1e4-ac0e086344c0.mp4

## Description

A mod that adds more character types to choose from!

Previously, these characters could only be found in Stick Fight's
unimplemented AI. With the the use of a new menu, users can now select and
play as 3 different choices (The Bolt, The Player, and The Zombie).

Whenever you're in the main menu, select your desired character choice. 
To re-select, simply exit back to the main menu. **This is <ins>not</ins> 
client-side, the character type that you choose will be the one everyone
else plays with, and vice-versa**.

### Advantages and Disadvantages:

| Character Type | Advantages                                                                                                  | Disadvantages                                           |
|----------------|-------------------------------------------------------------------------------------------------------------|---------------------------------------------------------|
| The Bolt       | • Quick and nimble.                                                                                         | • Unable to pick up most weapons nor use melee weapons. |
| The Player     | • Your average reliable Joe, well-rounded in all fields.<br>• Able to use all weapons where others cannot. | • Nothing special about it.                             |
| The Zombie     | • Long and slender arms provide superior range.<br>• Pressing the Tab key (changeable in config) allows you to grab an object; release by jumping.| • Unable to pick up most weapons nor use melee weapons. |

## Installation

To install the mod, one can follow the [same instructions](https://github.com/Mn0ky/QOL-Mod/#installation) 
as found on the QOL-Mod page and apply them here. **In addition to putting
the ``NewPlayerTypes.dll`` into the ``BepInEx\Plugins`` folder, you 
must [download](https://github.com/Mn0ky/SFTG-SimpleAntiCheat/releases/latest/download/SimpleAntiCheat.dll) 
and put the ``SimpleAntiCheat.dll`` into the folder as well, <ins>or the 
mod will not run</ins>**.

## Know Issues

- Grabbing as a zombie will not appear as so to others. This can be fixed in a later update that provides a custom packet for 
grabbing, letting others know that you did indeed grab an object.
