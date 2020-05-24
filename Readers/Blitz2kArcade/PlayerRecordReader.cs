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
    public class PlayerRecordReader
        : IDataFileRecordReader<Player>
    {   
        /// <summary>
        /// Reads the next player record from a stream
        /// </summary>
        /// <param name="reader">An instance of <see cref="BinaryReader"/> to read the record from</param>
        /// <returns>An instance of <see cref="Player" />.</returns>
        public Player Read(BinaryReader reader)
        {
                Player player = new Player();

                player.Stats = reader.ReadAsInt32Array(5);

                //The next int is unused.  Skip it
                reader.ReadInt32();

                //Not sure what Scale is used for, but is consistent for all players.  Might be the texture scale.
                player.Scale = reader.ReadSingle();

                player.Size = (PlayerSize)reader.ReadInt32();
                player.SkinColor = (PlayerSkinColor)reader.ReadInt32();

                //Player number is stored as a integer, which needs to be converted to a hex value to be the correct number.  For example:
                // player #16 is stored as integer value 22, which converted to hex, is 16.
                // Yes, that is odd...
                int numberAsInt = reader.ReadInt32();
                player.Number = int.Parse(numberAsInt.ToString("x2"));

                player.Position = (PlayerPosition)reader.ReadInt32();

                player.MugShotAddress = reader.ReadUInt32();
                player.SelectedNameAddress = reader.ReadUInt32();
                player.NameAddress = reader.ReadUInt32();
                player.Ancr = reader.ReadUInt32();

                player.LastName = reader.ReadAsString(16);
                player.FirstName = reader.ReadAsString(16);

                return player;
        }
    }
}