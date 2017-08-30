######
DOSDungeon
######

This is just a little try on a Dungeon game programmed as a WindowsForms App using Visual Studio 2017 and C#.
This is done in context of the current (Aug 2017) it-talents.de challenge.


###
Description
###
Very simple Dungeon game, where a hero can be controlled and has to collect treasures, fight rather dump monsters and ultimately has to reach the exit of a level. After a level is completed, a brief summary will be shown with the current player stats and the next level will be generated upon the wish of the player. 
All levels are randomly generated, i.e. the experience for each player will be a different one. For the random generation, first a start and exit of the level is created. Afterwards, a path is grown from those respective start and end positions, such that the two positions are connected. Therefore it is ensured that the hero, always starting at the start position, is always able to get to the end of the level (if he survives the monsters that is). Sprouting from the initial path, branching paths are then generated to make the levels more interesting. 

###
Controls
###

Arrow-keys: Turn/Move player
space: Attack monster (standing in front of you)
n: Instantly load next level 
s: toggle sound effects
