using System;
using System.Linq;
using System.IO;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Enums;
using NFLBlitzDataEditor.Core.Extensions;

namespace NFLBlitzDataEditor.Core.Readers.Blitz2kArcade
{
    /// <summary>
    /// Used to read an image from a stream of data
    /// </summary>
    public class TeamRecordReader
        : IDataFileRecordReader<Team>
    {
        /// <summary>
        /// Reads the next team record from a stream
        /// </summary>
        /// <param name="reader">An instance of <see cref="BinaryReader"/> to read the record from</param>
        /// <returns>An instance of <see cref="Team" />.</returns>
        public Team Read(System.IO.BinaryReader reader)
        {
            Team team = new Team();

            team.PassingRating = reader.ReadInt16();
            team.RushingRating = reader.ReadInt16();
            team.LinemenRating = reader.ReadInt16();
            team.DefenseRating = reader.ReadInt16();
            team.SpecialTeamsRating = reader.ReadInt16();
            team.Reserved1 = reader.ReadInt32();
            
            team.DroneBase = reader.ReadInt16();

            team.Name = reader.ReadAsString(32);
            team.CityName = reader.ReadAsString(32);
            team.CityAbbreviation = reader.ReadAsString(4);
            team.TeamAbbreviation = reader.ReadAsString(4);

            team.PlayersAddress = reader.ReadUInt32();
            team.TeamLogoAddress = reader.ReadUInt32();
            team.TeamLogo30Address = reader.ReadUInt32();
            team.TeamSelectedNameAddress = reader.ReadUInt32();
            team.TeamNameAddress = reader.ReadUInt32();
            team.Reserved2 = reader.ReadUInt32();
            team.UnknownAddress = reader.ReadUInt32();

            return team;
        }
    }
}