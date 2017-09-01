######
## DOSDungeon
######

This is try on a Dungeon game programmed as a WindowsForms App using Visual Studio 2017 and C#.
This is done in context of the current (Aug 2017) it-talents.de challenge.

###
## Description
###
Quite simple Dungeon game, where a hero can be controlled and has to collect treasures, fight rather dump monsters and ultimately has to reach the exit of a level. 
After a level is completed, a brief summary will be shown with the current player stats and the next level will be generated upon the wish of the player. 
All levels are randomly generated, i.e. the experience for each player will be a different one. For the random generation, first a start and exit of the level is created. 
Afterwards, a path is grown from those respective start and end positions, such that the two positions are connected. Therefore it is ensured that the hero, 
always starting at the start position, is always able to get to the end of the level (if he survives the monsters that is). 
Sprouting from the initial path, branching paths are then generated to make the levels more interesting. 

Elements of the game:

|element|description|
|--|--|
|Treasures | Upon collection they either yield a heart (health point) or a random amount of coints between 1-50|
|Monster | Randomly move around the level, will attack player if they stand in front of him|
|Player | Can collect treasures and defeat monsters. For defeating monsters, player must stand in front of monsters, face them and press the attack button (SPACE)|


###
## Controls
###
Control is still a little bit clumsily implemented but nevertheless working (especially movement of the player).

|key|action|
|--|--|
| Arrow-keys | Turn/Move player |
| [space] | Attack monster (standing in front of you) |
| [n] | Instantly load next level |
| [s] | Toggle sound effects |


###
## Art and SFX
###
Credits for Art and sound effects go to www.opengameart.org and were not created on my own! Slight modifications took place in some cases, though.

Specifially we use some
* sounds - https://opengameart.org/content/rpg-sound-pack
* graphics - https://opengameart.org/content/free-desert-platformer-tileset and https://opengameart.org/content/halloween-gift-for-oga 


