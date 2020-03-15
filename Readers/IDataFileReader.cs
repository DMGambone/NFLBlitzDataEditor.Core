using System.Collections.Generic;
using NFLBlitzDataEditor.Core.Models;

namespace NFLBlitzDataEditor.Core.Readers
{
    public interface IDataFileReader
    {
        /// <summary>
        /// Reads the complete set of teams defined in the data file
        /// </summary>
        /// <returns>A collection of teams defined in the data file</returns>
        IEnumerable<Team> ReadAllTeams();

        /// <summary>
        /// Reads in the complete list of players in the data file
        /// </summary>
        /// <returns>A collection of players defined in the data file</returns>
        IEnumerable<Player> ReadAllPlayers();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DataFile Read();

        /// <summary>
        /// Returns the image data located at a location in the file
        /// </summary>
        /// <param name="offset">The starting position to read the data</param>
        /// <returns>An instance of <see cref="Image" />.  If there is no valid image data, null is returned.</returns>
        public Image ReadImage(long offset);
    }
}