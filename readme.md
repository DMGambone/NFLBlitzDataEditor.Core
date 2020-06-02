# NFL Blitz Data Editor Core Library 
The NFL Blitz Data Editor Core Library  is .NET Standard assembly that allows a consumer of the libary to read and update an Midway NFL Blitz Arcade data content.  This project is very much a work in progress.

# Project Goals
The initial goal of the NFL Blitz Data Editor Core Library is as follows:
- [x] Reading team information (name, city, team stats, etc)
- [x] Reading player information (name, position, number, stats, etc)
- [x] Reading of graphics in the data
- [ ] Reading of audio in the data
- [ ] Writing team information (name, city, team stats, etc)
- [ ] Writing player information (name, position, number, stats, etc)
- [ ] Writing of graphics in the data
- [ ] Writing of audio in the data
- [ ] Ability to add additional teams to the game (Currently only contains 31 teams, Houston Texans are missing).

# Data File Specifications
While there are significant consistencies between all arcade versions, each version of NFL Blitz does differ a bit.  As a result, only NFL Blitz 2000 is currently being worked on with hopes to expand this to other version of NFL Blitz.

Information about the data file and structures used in NFL Blitz
- [Midway File System](docs/file-system.md)
- [NFL Blitz 2k data file format](docs/blitz2k-arcade.md)
- [Image formats](docs/image-format.md)

# Concepts/philosophies
## MidwayGamesFS vs NFLBlitzDataEditor.Core
When Brandon join the effort, he brought the knowledge that the hard drives ran a basic file system.  Original implementations of the `NFLBlitzDataEditor.Core` library attempted to write to a single large block representing the whole of the data.  Once this was understood and the logic coded, it was moved into the `MidwayGamesFS` namespace as a generic approach to reading/writing a common file system that Midway Games used in many of its games.  `MidwayGamesFS` has no concept of the games themselves.

`NFLBlitzDataEditor.Core` focuses on being able to read and update the files that have been extracted from a Midway Games file system.  It provides the classes used to process the extracted files, such as the images and `game.exe` file.

# Acknowledgements
There is a group of people who have done most of the discovery work already.  Once that information is known, building an library and editor is easy.  Much of what I found out initially was based on code by [Jake](https://github.com/thompjake) and information that was shared on http://NFLBlitzFans.com.  [Brandon](https://github.com/bre80) came in shortly afterwards and provided a much needed insight into the low-level workings of NFL Blitz and the file system code is based on his previous work. 

All the contributions of others made the work put into this so far much easier.