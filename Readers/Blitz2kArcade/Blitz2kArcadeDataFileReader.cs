using System;
using System.Linq;
using System.IO;
using NFLBlitzDataEditor.Core.Models;

namespace NFLBlitzDataEditor.Core.Readers.Blitz2kArcade
{
    public class Blitz2kArcadeDataFileReader
        : DataFileReader
    {
        public static DataFileSettings Settings = new DataFileSettings()
        {
            Version = NFLBlitzVersion.Blitz2000Arcade,
            PlayerListOffset = 90483472,
            PlayerRecordSize = 92,
            PlayersPerTeam = 16,
            TeamListOffset = 90529104,
            TeamRecordSize = 116,
            TeamCount = 31
        };

        public Blitz2kArcadeDataFileReader(Stream stream)
            : base(stream, Blitz2kArcadeDataFileReader.Settings, new ImageRecordReader(), new PlayerRecordReader(), new TeamRecordReader(), new ImageTableReader())
        {

        }

    }
}