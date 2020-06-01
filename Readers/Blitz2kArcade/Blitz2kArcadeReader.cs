using System;
using System.Linq;
using System.IO;
using NFLBlitzDataEditor.Core.Models;

namespace NFLBlitzDataEditor.Core.Readers.Blitz2kArcade
{
    public class Blitz2kArcadeReader
        : GameReader
    {
        public static GameFileSettings Settings = new GameFileSettings()
        {
            Version = GameVersion.NFLBlitz2000Arcade,
            PlayerRecordSize = 92,
            PlayersPerTeam = 16,
            TeamListOffset = 0x80185548,
            TeamRecordSize = 116,
            TeamCount = 31
        };

        public Blitz2kArcadeReader(Stream stream)
            : base(stream, Blitz2kArcadeReader.Settings, new PlayerRecordReader(), new TeamRecordReader())
        {

        }

    }
}