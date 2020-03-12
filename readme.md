# NFL Blitz 2k Data Editor
The NFL Blitz 2000 Data Editor is a utility that allows the user to update their Midway NFL Blitz 2000 Arcade version.  This project is very much a work in progress.

The initial goal of the NFL Blitz 2000 Data Editor is as follows:
- [ ] Updating of NFL Blitz 2000 Arcade data file (i.e.: content that is saved to the hard drive in NFL Blitz)
- [ ] Enable the viewing/updating of player information (name, position, number, stats, etc)
- [ ] Enable the viewing/updating of team information (name, city, team stats)
- [ ] Enable the viewing/updating of graphics in NFL Blitz 2000
- [ ] Enable adding additional teams to NFL Blitz 2000 (Currently only contains 31 teams, Houston Texans are missing)

## NFL Blitz 2k data file layout
Below are the known regions of an NFL Blitz 2k file.  As more information is gathered, this will be updated

| Start | End | Purpose |
|-------|-----|---------|
| 0 | 90483741 | ?? |
| 90483742 | 90529103 | Players data block |
| 90529104 | 90532700 | Team data block |
| 90532701 | 2149076871 | ?? |
| 2149032712 | 2149078344 | ?? - Team data block `FileOffset1` points to a location in this range |
| 2149540676 | 2149542116 + ?? | ?? - Team data block `FileOffset2` points to a location in this range |
| 2149542152 | 2149542652 + ?? | ?? - Team data block `FileOffset7` points to a location in this range |
| 2149863100 | 2149865812 + ?? | ?? - Team data block `FileOffset5` points to a location in this range |
| 2149863640 | 2149866028 + ?? | ?? - Team data block `FileOffset4` points to a location in this range |
| 2149877072 | 2149878216 + ?? | ?? - Team data block `FileOffset3` points to a location in this range |
| 2149878216 + ?? | 6448619520 | ?? |

### Player Record
A single player's information is stored in a block of data that is 92 bytes long.  This information is loaded into the `Player` model.

| Position | Size (bytes) | Purpose | `Player` Property Name |
|----------|--------------|---------|---------------|
| 0 | 28 | ?? - 7 file pointers? | `UnknownRegion1` |
| 28 | 4 | Player Size (0 = normal, 1 = large) | `Size` |
| 32 | 4 | Player Skin color (0 = dark, 1 = light) | `SkinColor` |
| 36 | 4 | Player Number<sup>1</sup> | `Number` |
| 40 | 4 | Player Position | `Position` |
| 44 | 4 | ?? - Reserved?  It's always 0. | `UnknownValue1` |
| 48 | 4 | ?? - Only set for QBs, a file pointer to where player updated stats are saved? | `UnknownAddr1` |
| 52 | 4 | ?? - Set for non-linement, a file pointer to where player updated stats are saved? | `UnknownAddr2` |
| 56 | 4 | ?? - Values of 10, 11, 12, 13 are only assigned to QB, WR, RB, or TE players | `UnknownValue2` |
| 60 | 16 | Last Name | `LastName` |
| 76 | 16 | First Name | `FirstName` |

<sup>1</sup>Player Numbers:  These are stored as a byte.  However, the byte value is not the actual player number.  The value is to be converted to a string in hex format in order to see the proper player number.  For example:  player #16 is stored as byte value 22, which converted to hex, is 16.

### Team Record
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

# Acknowledgements
There is a group of people who have done most of the discover work already.  Once that information is known, building an editor is easy.  Much of what I found out initially was based on code by https://github.com/thompjake and information that was shared on http://NFLBlitzFans.com.  All the contributions of others made the work put into this so far much easier.