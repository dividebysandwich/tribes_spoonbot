# Spoonbot
![Spoonbot](https://github.com/user-attachments/assets/5ae1191e-f18e-4dc7-85b7-864864136dc4)

## Overview

Spoonbot is a "bot" for the game Starsiege: TRIBES. A bot is meant to be a replacement for human opponents.
So if you have a bad internet connection, you can spawn a few bots and play with/against them.

For the first time in TRIBES history, SPOONBOT Version 0.4 introduced roaming bots which walk around and attack on their own accord. Now you can have real fights without playing over the internet.
Since v0.4a bots also understand the principle of CTF (Capture the Flag), so you can actually play matches against SPOONBOT.
The final release (1.0) also brought C&H gameplay support.

## Additional Credits

Wicked69 wrote the whole tree code (that's the path navigation) thus he coded about 50% of the bot!!
Sam Wilson optimized the tree calculation routine so it takes minutes instead of hours!!! Thanks!!
EMO1313 made the BotGear code, many bug reports, and the SHIFTER mod.
Savage shared the code of his bot, especially the mortar code. Thanks Savage!

Additional Credits to Belial for the improved Treefile navigation code in v1.0
Kudos to Dewy for the awesome JetToPos code.

Many thanks go to Weirdo, Nollykin, [CYA]WoLf, XXX_Quitch, Mr. X, Balkatori, Cyberdachshund, Storm, Kerusso, Stephen East and all the
countless people on the Spoonbot Development Board, who were loyal Spoonbot-testers and contributors for almost a year now.

Special Thanks to [CoD]h0micide (http://www.codx.com) for his CTF code improvements and his gorgeous spoonbot map pack!!!




## Installation

### Client Install: None needed. 

### Server Install: 

Unzip the archive with pathnames.
When installed The Files Should be here: 
    Tribes\config\spoonbot.cs
    Tribes\spoonbot_12\*.cs
    Tribes\spoonbot_readme_xx.txt
    Tribes\spoonbot.bat


To start the server with this mod use the following command
    tribes -mod spoonbot_xx +exec spoonbot.cs
..where "spoonbot_xx" is "spoonbot_13" for version 1.3 and so on.


or for a dedicated server type

    tribes -mod spoonbot_13 +exec spoonbot.cs -dedicated

For instructions on how to run SPOONBOT together with Renegades or other mods, see the FAQ! NOTE however that I do NOT support dual-mods, and I will disregard emails with "Spoonbot & Renegades doesn't work"

NOTE:  Make sure you have Tribes version 1.11 or later installed first!


## Features
--------------------------------------

- Bots can be spawned into ALL MAPS! You no longer need to manually modify maps. Just play the standard maps with bots!
To spawn a bot, press TAB and select "Spawn bot", then select a bot class, gender and if it should be roaming or a normal bot.
The first "Guard" bot will be called "T0N1_Guard", the second "T0N2_Guard" etc... for the second team it would be "T1N1_Guard" respectively.
You can have a maximum of 3 bots of the same team/gender/category. This means you can only have 3 Female Snipers in team 1, however you can have 3 more MALE snipers in the same team.
For 6 different bot classes and 2 genders this would mean 3x6x2 which equals to a maximum of 36 bots per team.

- Bots will choose the adequate weapons depending on range and player status.

- Different Bot types exhibit different behaviour patterns

- Bots can now deploy Sensors, Turrets and Stations.
To have a bot deploy something, first make sure he's at the point where you want the turret/etc. to deploy. Then go to the commander screen, select the Bot, select "Deploy" and the Item you want the bot to deploy. If you click somewhere in the map, the bot will first deploy the item, THEN go to the point you clicked.
Example: Bot is at location A. You want a Sensor at location B. Command the bot to "attack" location B. When he's there, order the bot to deploy a Sensor, and as target location select your homebase. The bot will set up the Sensor at location B, and then head home to your home base.
All clear?

- Different armor/weapon configuration for bots

- Bots run around the map ON THEIR OWN, without needing a human commander.

- Bots play CAPTURE THE FLAG
New in v0.4a is the bot's ability to capture the enemy's flag and return it to your home base. 
If en enemy captures your flag, ALL your bots will go after him and attempt to kill him. 
If the enemy flagrunner gets killed and drops your flag, your bots will automatically try to return your flag.
In short: Virtually ALL aspects of CTF gameplay are implemented into the bot's AI.

- Bots play C&H
This is new in v1.0

- Bots buy and fly vehicles
This is new in v1.3

## Vehicles

Bots can also pilot vehicles. They use fixed flight-paths and the movement looks jerky, but it's better than nothing.
I supply pilot path data for the maps DarkAurora, NightSlide, Raindance, Rollercoaster and Scarabrae.
If you want to have vehicle support on other maps, you can do that! Just edit spoonbot.cs, set $Pilot::WaypointEditor=True;
Then, get into your map, buy a vehicle, and press F5 to create waypoints for the pilots. You should press F5 every 5 seconds or so. Keep in mind that the bots won't check for missile turrets, so fly a safe path for them. You last point should be somewhere safe to land, i.e. on top of the enemy base or something like that. Keep in mind that there should be normal treepoints in the landing spot's proximity so the bots don't get lost.

## Commandable Bots

Someone requested to have commandable bots again, so I re-enabled them. They contain the word "CMD" in their name, and do NOT move on their own unless attacked.
To send them somewhere, just issue commands via the commander console (key "c") They can also deploy items via that method. Other than that, they do NOTHING.


## Navigation issues

SPOONBOT uses a pre-stored set of paths. You have to download or create these Paths (called "Treefiles") if you want the bot to play inside Bases. 


## Treefiles

"Treefiles" contain the path-navigation the bot needs to roam inside buildings.
Treefiles have to be created for every map, and are stored inside the config/ directory. "BT_Raindance.cs" would be the treefile for Raindance. To create a treefile, just set $Spoonbot::BotTree_Design = True and run SPOONBOT on the map you want to create the treefile for.

Spoonbot v0.6 and later create the Treepoints automatically. However, it only makes sure that there is a route for the bots, it doesn't check if the treepoints it creates are placed wisely. That means you should place Most of the Treepoints BY HAND using the TAB menu. Just press TAB and then "2. Place Treepoint" - do this at every corner. Always consider that the bots will walk directly from one treepoint to another, and place them so that the bots won't get stuck. Also, keep in mind that the bots will most often take the shortest route between visible treepoints, so you may have to fiddle with treepoint placement a little.

Now run around the map and visit all important places, for example Generators and Stations. Run around large buildings and jump on every tower to create the neccessary path information for the bot. Treepoints will be added automatically as you wander around the map. You have to visit all important areas like Turrets, Generators, Stations, and of course the Flag and/or Switches.

Don't forget to switch teams when you're halfway done, to map the other team's base.

When you're done, press TAB and select "3.CALCULATE TREE ROUTES". This should just take a moment.

Be patient. For comparison: In v0.5c a 160 points treefile took up to 6 hours!! In v1.0 this is a matter of maybe 10 seconds...

**Don't forget to set $Spoonbot::BotTree_Design = False before playing the thing!**

Treefiles aren't really limited to a size anymore because of the greatly improved system. The more treepoints you have, the better the map plays.


## Spoonbot with Renegades

I modified the distribution code to make it MOD-friendly. That means, Spoonbot MIGHT run with your favourite mod, but I didn't test it.
There are files and functions which SPOONBOT might override with it's own, thus defying the other MOD's functionality. Weapon-only mods should work well.

However, there are mods which already incorporate SPOONBOT, like the SHIFTER MOD at http://www.dopplegangers.com/tribes/shifter.The SHIFTER MOD is quite similar to Renegades, so if you're a hardcode Renegades fan I recommend you to try it.

Recently, the MiniMod also incorporated Spoonbot into it's set of plugins. Highly recommended stuff, although they don't have the latest versions of the code implemented.

## Bot Spawn Menu

You can spawn bots into the game and also remove bots from the game. This is done by pressing TAB, whereafter the game menu appears.
It should look like this:

***********************
Options:

1. Change Teams/Observe
2. Spoonbot controls
3. Kick
4. Ban
5. etc..
***********************



Now if you press [2], the following menu appears:



***********************
Select Bot action:

1. Spawn bot
2. Remove bot
***********************



If you press [2], a list of the bots currently in the game will be displayed. Just choose a bot to remove and press the appropriate number, and he'll be gone.

If you press [1], you can spawn a bot into the game:



***********************
Select bot type:

1. Guard
2. Demo
3. Painter
4. Sniper
5. Medic
6. Miner
7. Mortar
***********************



Select a bot class (see description later in this file) and you'll be directed to the last menu:



***********************
Bot gender and type:

1. Male Roaming
2. Female Roaming
3. Male CMD
4. Female CMD
***********************



Press [1] or [2] for a standard, self-conscious bot ;-) i.e. the bot will act autonomously, thus NOT responding to your commands, but running around like real human players.
Press [3] or [4] for a COMMANDABLE BOT, which will be solely dependend on commands YOU give him via the command console (key "c")


After you've made your way through these menus, your handcrafted bot will spawn and report for duty.


## Automatic Bot Spawning

It is possible to pre-configure SPOONBOT for your server. You can pre-define bot teams, lock the bot-menu for your players and set the respawn delay. All this is done via the **spoonbot.cs** file which should reside in the Tribes\config folder.
This is especially cool for server operators, since now they can run a server which spawns their favourite setup of bots into every map the server runs. - No need to manually spawn bots anymore!

Here's an example **spoonbot.cs** file, just like the one included with the SPOONBOT distribution archive:



    // This file will set up a fixed set of bots which are spawned automatically
    // so you can have a dedicated server running without users having to spawn bots.
    $Spoonbot::DebugMode = False;
    $Spoonbot::AutoSpawn = True;			//== Automatic bot-spawning on
    $Spoonbot::BotTree_Design = False;		//== Enables Bot tree design mode
    $SpoonBot::BotTree_MaxAutoCalc = 10;		//== Threshhold after which auto route generation is disabled.

    $Spoonbot::UserMenu = True;			//== Users may add/remove bots via menu
    $Spoonbot::BotChat = True;			//== If the bot's chat messages annoy you, you can turn them off here.
    
    $BotTree::DebugMode = False;
    
    $Spoonbot::RespawnDelay = 10;		//== How many seconds until bots respawn after being killed?
    $Spoonbot::IQ = 240;				//== The IQ controls the bot's overall skill, like targeting precision, speed, etc.
    
    
    $Spoonbot::ThinkingInterval = 5;	//== Interval in sec between which bots will "reconsider" their situation
    					//== NOTE: RespawnDelay MUST be higher than ThinkingInterval
    					//== ANOTHER NOTE: The slower your CPU, the higher this should be.
    
    
    $Spoonbot::MovementInterval = 1.5;	//== Interval in sec between calls of the Movement code.
    					//== This should be generally lower than ThinkingInterval
    					//== NOTE: Again, the slower your CPU, the higher this should be.
    					//== If you experience "lag", set these values even higher.
    
    
    $Spoonbot::RetreatDamageLevel = 1.0;		//== Bots will retreat if damage exceeds this value. 0.0 means no damage, 1.0 means dead.
    						//== To disable retreating, set to 1.0
    
    $BotHUD::ToggleKey = "b";			//== CTRL + this key will open the BotHUD.
    						//== The BotHUD displays what your bots are doing at the moment.
    
    
    $Spoonbot::DefaultTeamEnergy = Infinite;		//== The default energy each team starts with. Set to "Infinite" for standard TRIBES rules,
    						//== or set to 1000 or similar for having to worry about cash ;-)
    
    
    
    						//== Now, the auto-spawned bots are being set up
    						//== NOTE: $Spoonbot::AutoSpawn must be "True" for this to work!!
    
    $Spoonbot::Bot1Name = "Scotty_Guard_Roam_Male";
    $Spoonbot::Bot1Team = 0;
    
    $Spoonbot::Bot2Name = "Uhura_Sniper_Roam_Female";
    $Spoonbot::Bot2Team = 0;
    
    $Spoonbot::Bot3Name = "Chapel_Painter_Roam_Female";
    $Spoonbot::Bot3Team = 0;
    
    $Spoonbot::Bot4Name = "Spock_Demo_Roam_Male";
    $Spoonbot::Bot4Team = 0;
    
    $Spoonbot::Bot5Name = "Checkov_Mortar_Roam_Male";
    $Spoonbot::Bot5Team = 0;
    
    $Spoonbot::Bot6Name = "Sulu_Miner_Roam_Male";
    $Spoonbot::Bot6Team = 0;
    
    
    $Spoonbot::Bot7Name = "Shag_Guard_Roam_Male";
    $Spoonbot::Bot7Team = 1;
    
    $Spoonbot::Bot8Name = "Colleen_Sniper_Roam_Female";
    $Spoonbot::Bot8Team = 1;
    
    $Spoonbot::Bot9Name = "Exile_Painter_Roam_Male";
    $Spoonbot::Bot9Team = 1;
    
    $Spoonbot::Bot10Name = "Hunter_Demo_Roam_Male";
    $Spoonbot::Bot10Team = 1;
    
    $Spoonbot::Bot11Name = "Balto_Mortar_Roam_Male";
    $Spoonbot::Bot11Team = 1;
    
    $Spoonbot::Bot12Name = "Jenna_Miner_Roam_Female";
    $Spoonbot::Bot12Team = 1;
    

**THE REST OF SPOONBOT.CS IS FOR THE BOT'S GEAR-UP, AND ONLY MEANT FOR MOD-CODERS!**

That way, you can make sure the game doesn't get dull and boring if there are only 1 or 2 human players!
And if players mess around with the bots and start to spawn huge armies, just set $Spoonbot::UserMenu = False and they cannot spawn or remove bots anymore. 
I have to admit that the Bot Retreat code is quite useless in heavy battle, however it should save someone's butt from time to time.


## Other changes to Tribes

All weapon, item and vehicle definitions were left untouched, which means you play standard Tribes unless you use a weapon mod together with Spoonbot.


## Version history
### v1.2
- Added a few new treepoint files (try out Sulfurious!!)
- Fixed Weapon changing code for most bots. No more harmless painter-bots...

### v1.1
- Fixed Bots sometimes standing around forever
- Tweaked JetToPos a bit.
- Fixed MortarBot not following painter properly

### v1.0
- Updated Treefile code to version 2 -> Treefiles now 1/10th of the original size
- Added C&H capability, they also capture the switches in CTF maps
- Bots now use repairkits (This should greatly improve their effectiveness)
- Fixed Demo behaviour (sometimes attacked underground generators)
- Fixed Sniper behaviour (now they search for good sniping locations)
- Fixed Medic behaviour (sometimes didn't repair stuff)
- Fixed Mortar behaviour (didn't wait till in range before mortaring)
- Fixed Guard behaviour (did't work at all)
- Fixed Cheat-Teleport function warping bots out of the game field sometimes (Now that was creepy)

### v0.6a
- Found and fixed what seems to be the final treepoint navigation bug
- Fixed Medic and Mortar not using Treepoints
- Fixed JetToPos funtion to work more reliable
- Fixed Medic not repairing Solar Panels

### v0.6
- Fixed approx. 200 different bugs of all sizes and severities.
- Got Treecode working 100%, provided that the Treefiles are ok.
- Got CTF working 100% (They cap the flag on Broadside with no problems!!!!)

### v0.5i
- Fixed Energy bug
- Fixed "Bot Runaway" bug
- Improved a couple of AI issues regarding repairing/attacking objects
- Some changes I forgot to remember

### v0.5h
- Switched ALL movement to utilize the treecode. Treefiles are now MANDATORY for operation!
- Reduced CPU load
- Smalled treefiles while still being compatible to the old format
- Improved Miner bot
- Improved Demo bot
- Improved Mortar bot
- Reduced Range-Of-Sight for the Sniper, for realisms sake.
- Tested for TribesDemo-compatibility

### v0.5g
- No official release

### v0.5f
- New bot type: MORTAR BOT
- CTF now works again, thanks to [COD]h0micide
- Miner bot will go after the enemy flag
- Guard bot now especially guards the own flag
- Mortar bot goes into the field for mortar-jobs ;-)
- Changed bot evade code. They now skyrocket a bit too much - please tell me if this annoys you.

### v0.5e
- Fixed Demo-bot attacking teammembers
- Dedicated Server stability issues solved!! (It's now safe to run a dedicated server, yay!!)
- Standard weapon usage modified
- Fixed HUD-incompatibilities.
- Fixed some stupid typos
- Got rid of MOST console error messages -> You can now run a dedicated server without having a 100Meg logfile after 10 hours

### v0.5d
- Optimized Treefile caltulation (takes minutes instead of hours!!)
- Demolition bot behaviour added
- Bots are more agressive, Medic and Painter now have defensive weapons too.
- Re-introduced COMMANDABLE BOT.
- Fixed some more bugs.
- Playing Tribes is fun again ;-)

### v0.5c
- Painter Bot now fully functional!!!
- Bots now attack more "coordinated"
- Fixed various typos and small bugs
- All bots are now ROAMING bots

### v0.5b
- Restored Medic repair behaviour
- Bots now jump on your HPC on own accord
- The also jump off on their own ;-)
- Turrets fire again
- Even more compatibility with other mods!

### v0.5a
- Second Generation of Treecode. Treefiles created automatically
- Deathmatch play works great.
- Treefile calculation is AWKWARDLY slow. A 150 point treefile takes > 5 hours on a K6-3 400 !!! Calculation grows geometrically with each additional Treepoint
- The BotThink code is a mess:
- CTF doesn't work again
- Medic doesn't work again
- Individual bots seem to "stop" sometimes. Thus they just stand at the home base and do nothing.
- The "Treefile" Path navigation seems to work great, but often fails at high jumps
- Changed the distribution format from .VOL to .CS
- Only included my own CS files, so people can run their favourite Weapon MOD with SPOONBOT
- Compared with v0.4f, this version adds a lot new features, but gameplay is less great.

### v0.4g
- First Generation of Treecode. Treefiles created manually

### v0.4f
- Last version of Spoonbot without Tree Code! I consider this to be the best possible bot without pre-stored paths

### v0.4b
- Mortaring bots finally works!! Every teammember can assign targets for all friendly Guard bots vie targeting laser.
- Bots now chat while fighting (you can switch this off in SPOONBOT.CS)
- The annoying "jumpy bot jetting" can now be switched off in SPOONBOT.CS

### v0.4a
- Bots retreat if severely damaged
- Bots play CTF on their own (capturing the flag, returning the flag, attacking flagrunner)
- You may now disable the A-Bomb in SPOONBOT.CS if you want, thus having the ORIGINAL rules of gameplay in Tribes.
- Fixed the "Bot decides to attack target <NUL>" error
- Fixed an AI logical loop which caused servers to slow down and hang after approx. 30 minutes of play.
- Fixed several waypoint issues.

### v0.4
- Bots now roam around like human players if their name contains "Roam"
- Bots can now be AutoSpawned via config file spoonbot.cs
- Bots now jump and jetpack while fighting, thus really having a chance against humans!
- Bots now have a rudimentary move-around code to avoid getting stuck in obstacles, though they still get stuck sometimes.
- Bots now chase enemy players carrying the flag.
- Bots now return the flag to the base if the enemy flagrunner is dead
- Bots riding vehicles works again
- Medic drops medikit if touched by human teammate
- Modified bot spawn menu layout
- Ability to remove individual bots
- Turrets now have a deploy limit
- Bot code is now based on TRIBES v1.41 base code


## Known Bugs

- The bot's jumping often isn't precise enough to climb obstacles. Therefore I made them "cheat" a little.
- Bots MAY get stuck inside buildings. Most of the time, this is due to erroneous treefiles.
- There's no sensor/station deploy limit for bots. This can lead to serious slowdowns if mis-used.

## Bot Classes:

Bots are organized in classes. To have a bot spawn in "Medic" configuration, simply have his name contain the word "Medic". This applies to all classes, so a bot named "BillClinton_Demo" will spawn as Demolition-bot.

### Mortar
Armor: Heavy
Weapons: Mortar, Disc Launcher, Plasma Gun
Backpacks: Ammo Pack

This is a heavy bot, designed to take out turrets and other installations from long-distance. Just "paint" a target with the targeting laser and he'll mortar away at it!! That way, you can have bots destroy turrets etc. from safe distance. He also works together with a painter bot. However, if several painter laser are being fired, he'll mortar at the FIRST.

### Guard
Armor: Heavy
Weapons: Mortar, Disc Launcher, Plasma Gun
Backpacks: Ammo Pack

This is the MortarBot's younger brother. He is doing a fine job at defending the team's flag. That's why you rarely see him fighting in the field. Although if your own flag is taken, he will go after the enemy flagcarrier.

### Demo
Armor: Medium
Weapons: Disc Launcher, Plasma Gun, Grenade Launcher
Backpacks: Jammer Pack

The "workhorse" of all bots. This guy does the dirty work, and is equipped with the means to clear the path of nasty Turrets and stuff. This is a cool, determined guy who usually takes out the generators first. Jetting players die like flies in his highly-accurate chaingun fire. He will also go after the flag if no enemy equipment survived his temper

###  Painter
Armor: Light
Weapons: Blaster, Disc Launcher, Chaingun, Targeting Laser
Backpacks: Ammo Pack

This guy tries to paint enemy turrets for your mortar guys as top-priority. Once the turrets are destroyed, he'll paint the enemies vehicle station and sensors. He always tries to stay at a safe distance, and only moves forward if he can't see the target. And if some enemy tries to mess with him, he'll get a disk right up the keister.

### Sniper
Armor: Light
Weapons: Disc Launcher, Plasma Gun, Laser Rifle
Backpacks: Energy Pack

He'll search for good sniping spots and make toast of your enemies. He is also a switch-addict personality, which means he's quick and fast enough to hop from switch to switch on C&H maps. However, close combat is NOT one of his strenghts.

### Medic
Armor: Light
Weapons: Blaster, Disc Launcher, Chaingun
Backpacks: Repair Pack

The Medic automatically repairs destroyed structures like Turrets and Stations. If needed, he rushes to the front line to heal comrades, and throw a disc or two at the enemy. He also likes to cap the flag every now and then

### Miner
Armor: Light
Weapons: Blaster, Disc Launcher, Plasma Gun, Mines
Backpacks: Jammer Pack

His name might be misleading, as the Miner will go after the enemy flag as top priority and escort any teammembers carrying the enemies flag. This guy is the real dedicated "flag-capper"
