using System;
using System.IO;
using System.Collections.Generic;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Enums;

/// <summary>
/// Data file reader used for the arcade version of NFL Blitz 2000 
/// </summary>
namespace NFLBlitzDataEditor.Core.Readers
{
    public class Blitz2KArcadeDataFileReader
    : DataFileReaderBase, IDataFileReader
    {
        public Blitz2KArcadeDataFileReader(Stream stream, DataFileSettings settings)
        : base(stream, settings)
        {

        }

        /// <summary>
        /// The data file version the reader supports
        /// </summary>
        protected override NFLBlitzVersion Version { get { return NFLBlitzVersion.Blitz2000Arcade; } }

        protected override Team ReadNextTeam(System.IO.BinaryReader reader)
        {
            using (System.IO.BinaryReader recordReader = ReadNextBlock(reader, Settings.TeamRecordSize))
            {
                Team team = new Team();

                team.PassingRating = recordReader.ReadInt16();
                team.RushingRating = recordReader.ReadInt16();
                team.LinemenRating = recordReader.ReadInt16();
                team.DefenseRating = recordReader.ReadInt16();
                team.SpecialTeamsRating = recordReader.ReadInt16();
                team.UnknownRegion1 = recordReader.ReadBytes(6);

                team.Name = ReadAsString(recordReader, 32);
                team.CityName = ReadAsString(recordReader, 32);
                team.CityAbbreviation = ReadAsString(recordReader, 4);
                team.TeamAbbreviation = ReadAsString(recordReader, 4);

                team.FileOffset1 = recordReader.ReadUInt32();
                team.FileOffset2 = recordReader.ReadUInt32();
                team.FileOffset3 = recordReader.ReadUInt32();
                team.FileOffset4 = recordReader.ReadUInt32();
                team.FileOffset5 = recordReader.ReadUInt32();
                team.FileOffset6 = recordReader.ReadUInt32();
                team.FileOffset7 = recordReader.ReadUInt32();

                return team;
            }
        }

        /// <summary>
        /// Reads the next player record from a stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>An instance of <see cref="Player" />.  If there is no player record to read, null is returned</returns>
        protected override Player ReadNextPlayer(BinaryReader reader)
        {
            using (System.IO.BinaryReader recordReader = ReadNextBlock(reader, Settings.PlayerRecordSize))
            {
                Player player = new Player();

                player.UnknownRegion1 = ReadAsInt32Array(recordReader, 7);

                player.Size = (PlayerSize)recordReader.ReadInt32();
                player.SkinColor = (PlayerSkinColor)recordReader.ReadInt32();

                //Player number is stored as a integer, which needs to be converted to a hex value to be the correct number.  For example:
                // player #16 is stored as integer value 22, which converted to hex, is 16.
                // Yes, that is odd...
                int numberAsInt = recordReader.ReadInt32();
                player.Number = int.Parse(numberAsInt.ToString("x2"));

                player.Position = (PlayerPosition)recordReader.ReadInt32();

                player.UnknownValue1 = recordReader.ReadInt32();
                player.UnknownAddr1 = recordReader.ReadUInt32();
                player.UnknownAddr2 = recordReader.ReadUInt32();
                player.UnknownValue2 = recordReader.ReadInt32();

                player.LastName = ReadAsString(recordReader, 16);
                player.FirstName = ReadAsString(recordReader, 16);

                return player;
            }
        }
    }
}