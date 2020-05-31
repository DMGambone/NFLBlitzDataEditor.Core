# NFL Blitz 2k data file layout

## Player Record
A single player's information is stored in a block of data that is 92 bytes long.  This information is loaded into the `Player` model.

| Position | Size (bytes) | Purpose | `Player` Property Name |
|----------|--------------|---------|---------------|
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

## Team Record
A single team's information is stored in a block of data that is 116 bytes long.  This information is stored in the `Team` model.

| Position | Size (bytes) | Purpose | `Team` Property Name |
|----------|--------------|---------|----------------------|
| 0 | 2 | Team Passing Rating | `PassingRating` |
| 2 | 2 | Team Rushing Rating | `RushingRating` |
| 4 | 2 | Team Linemen Rating | `LinemenRating` |
| 6 | 2 | Team Defense Rating | `DefenseRating` |
| 8 | 2 | Team Special Teams Rating | `SpecialTeamsRating` |
| 10 | 6 | ?? | `UnknownRegion1` |
| 16 | 32 | Team Name | `Name` |
| 48 | 32 | City Name | `CityName` |
| 80 | 4 | City Abbreviation | `CityAbbreviation` |
| 84 | 4 | Team Abbreviation | `TeamAbbreviation` |
| 88 | 4 | ?? - Points to a 1472 byte section? | `FileOffset1` |  
| 92 | 4 | ?? - Points to a 48 byte section? | `FileOffset2` |  
| 96 | 4 | ?? - Points to a 36 or 52 byte section? | `FileOffset3` |  
| 100 | 4 | ?? - Points to a 36 or 1344 (only the Ravens) byte section? | `FileOffset4` |
| 104 | 4 | ?? - Points to a 36 or 1668 (only the Bengals) byte section? | `FileOffset5` |
| 108 | 4 | ?? - Always 0 | `FileOffset6` |
| 112 | 4 | ?? - Points to a 16 or 20 byte section? | `FileOffset7` |

## Readers
Readers used for NFL Blitz 2k Arcade files are located in the `NFLBlitzDataEditor.Core.Readers.Blitz2kArcade` namespace.  These include
 - `Blitz2kArcadeDataFileReader` (inherits `DataFileReader`): An instance of `IDataFileReader` that is used to read information from the NFL Blitz 2000 Data File
 - `PlayerRecordReader`: An implementation of `IDataFileRecordReader<Player>` that reads a single player record
 - `TeamReacordReader`: An implementation of `IDataFileRecordReader<Team>` that reads a single team record
