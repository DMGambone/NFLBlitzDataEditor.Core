# NFL Blitz 2k Game Editor

## Team Record
A single team's information is stored in a block of data that is 116 bytes long.  This information is stored in the `Team` model.

| Position | Size (bytes) | Purpose | `Team` Property Name |
|----------|--------------|---------|----------------------|
| 0 | 2 | Team Passing Rating | `PassingRating` |
| 2 | 2 | Team Rushing Rating | `RushingRating` |
| 4 | 2 | Team Linemen Rating | `LinemenRating` |
| 6 | 2 | Team Defense Rating | `DefenseRating` |
| 8 | 2 | Team Special Teams Rating | `SpecialTeamsRating` |
| 10 | 2 | Unused value | `Reserved1` |
| 10 | 4 | Base setup of the team AI | `DroneBase` |
| 16 | 32 | Team Name | `Name` |
| 48 | 32 | City Name | `CityName` |
| 80 | 4 | City Abbreviation | `CityAbbreviation` |
| 84 | 4 | Team Abbreviation | `TeamAbbreviation` |
| 88 | 4 | Memory address to the team's players | `PlayersAddress` |  
| 92 | 4 | Memory address to the team logo `ImageInfo` | `LogoAddress` |  
| 96 | 4 | Memory address to a reduced size team logo `ImageInfo` | `Logo30Address` |  
| 100 | 4 | Memory address to the team name `ImageInfo` used for selection | `NameAddress` |
| 104 | 4 | Memory address to the selected team name `ImageInfo` used for selection | `SelectedNameAddress` |
| 108 | 4 | Always 0 | `Reserved2` |
| 112 | 4 | Memory address to a collection of `ImageInfo` used on the loading screen | `LoadingScreenImagesAddress` |

The team records are located at memory address `0x80185548`.

## Player Record
A single player's information is stored in a block of data that is 92 bytes long.  This information is loaded into the `Player` model.

| Position | Size (bytes) | Purpose | `Player` Property Name |
|----------|--------------|---------|------------------------|
| 0 | 20 | 5 player stats (Spd, Pwr, QB, WR, Off, Def) | `Stats` |
| 20 | 4 | Scale, maybe for texture scaling? | `Scale` |
| 24 | 4 | Unused? It's always 0. |  |
| 28 | 4 | Player Size (0 = normal, 1 = large) | `Size` |
| 32 | 4 | Player Skin color (0 = dark, 1 = light) | `SkinColor` |
| 36 | 4 | Player Number<sup>1</sup> | `Number` |
| 40 | 4 | Player Position | `Position` |
| 44 | 4 | Location of player's image used in the game (eg: mugshot) | `MugShotAddress`  |
| 48 | 4 | Location of image used to show player's name as being selected | `SelectedNameAddress` |
| 52 | 4 | Location of image used to show player's name | `NameAddress` |
| 56 | 4 | ?? - Values of 10, 11, 12, 13 are only assigned to QB, WR, RB, or TE players. | `Ancr` |
| 60 | 16 | Last Name | `LastName` |
| 76 | 16 | First Name | `FirstName` |

<sup>1</sup>Player Numbers:  These are stored as a byte.  However, the byte value is not the actual player number.  The value is to be converted to a string in hex format in order to see the proper player number.  For example:  player #16 is stored as byte value 22, which converted to hex, is 16.

## Playbook
NFL Blitz 2000 has 60 plays in the playbook.  There are 48 offsensive plays, of which 3 are custom, and 22 defensive plays.  These are loaded into the `PlaybookEntry` model, shown below.

| Position | Size (bytes) | Purpose | `PlaybookEntry` Property Name |
|----------|--------------|---------|-------------------------------|
| 0 | 4 | Memory address of the play name | `NameAddress` |
| 4 | 4 | Memory address of the actual play data | `PlayDataAddress` |
| 8 | 4 | ?? | `Unknown1` |
| 12 | 4 | ?? | `Unknown2` |
| 16 | 4 | ?? | `Unknown3` |

### Field layout
In NFL Blitz, the field is broken down into a grid that is 40 units wide (sideline to sideline) and 44 units long (endzone to endzone).  The directions include 2 units on each size to account for the out of bounds area.

* The sideline-to-sideline direction is labeled as X
* The endzone-to-endzone direction is labeled as Z

### Play
The play data is broken down into 3 parts, loaded into the `Play` model.  

| Position | Size (bytes) | Purpose | `Play` Property Name |
|----------|--------------|---------|-------------------------------|
| 0 | 4 | Memory address of the play's initial formation | `PlayFormationAddress` |
| 4 | 4 | The type of play | `Type` |
| 8 | 28 | Memory address of each player's set of actions (ie: a play route) to execute for the play | `Routes` |

### Play Starting Position
The play's initial formation is a collection of data for each player that indicates how they line up at the beginning of the play and any pre-snap actions to take.  This information is loaded into the `PlayStartingPosition` model.

| Position | Size (bytes) | Purpose | `PlayStartingPosition` Property Name |
|----------|--------------|---------|-------------------------------|
| 0 | 4 | A players starting position offset from the line of scrimmage (ie: distance away from the line of scrimmage) | `X` |
| 4 | 4 | A players position along the line of scrimmage (sideline to sideline) | `Z` |
| 8 | 4 | A sequence of pre-snap actions the player is to take | `Sequence` |
| 12 | 4 | ?? | `Mode` |
| 16 | 4 | Pre-snap control flags | `ControlFlag` |

### Routes
Once the play starts, the players execute a series of actions (a.k.a.: the route).  Each action identifies a location and what should the player do there.

| Position | Size (bytes) | Purpose | `RouteAction` Property Name |
|----------|--------------|---------|-------------------------------|
| 0 | 4 | The direction the player should move towards the endzone | `X` |
| 4 | 4 | The direction the player should move laterally (sideline-to-sideline) | `Z` |
| 8 | 4 | ?? | `Unknown` |
| 12 | 4 | The action the player should perform, such as duke, spin, or wave | `Action` |