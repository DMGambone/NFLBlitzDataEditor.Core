# NFL Blitz Data Editor Core Library 
The NFL Blitz Data Editor Core Library  is .NET Standard assembly that allows a consumer of the libary to read and update an Midway NFL Blitz Arcade data file.  This project is very much a work in progress.

# Project Goals
The initial goal of the NFL Blitz Data Editor Core Library is as follows:
- [ ] Enable the retrieval/updating of player information (name, position, number, stats, etc)
- [ ] Enable the retrieval/updating of team information (name, city, team stats, etc)
- [ ] Enable the retrieval/updating of graphics in the data file
- [ ] Enable the retrieval/updating of audio in the data file
- [ ] Enable adding additional teams to the game(Currently only contains 31 teams, Houston Texans are missing).  This may not be possible without updates to the actual ROMs in the game

# Data File Specifications
While there are significant consistencies between all arcade versions, each version of NFL Blitz does differ a bit.  As a result, only NFL Blitz 2000 is currently being worked on with hopes to expand this to other version of NFL Blitz.

Information about the data file and structures used in NFL Blitz
- [NFL Blitz 2k data file format](docs/blitz2k-arcade.md)
- [Image formats](docs/image-format.md)

# Acknowledgements
There is a group of people who have done most of the discovery work already.  Once that information is known, building an editor is easy.  Much of what I found out initially was based on code by https://github.com/thompjake and information that was shared on http://NFLBlitzFans.com.  All the contributions of others made the work put into this so far much easier.