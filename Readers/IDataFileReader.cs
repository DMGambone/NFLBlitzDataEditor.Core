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
        /// <returns>An instance of <see cref="ImageData" />.  If there is no valid image data, null is returned.</returns>
        ImageData ReadImageData(long offset);

        /// <summary>
        /// Returns an instance of <see cref="ImageTable" /> located at a location in the file
        /// </summary>
        /// <param name="position">The starting position to read the data</param>
        /// <returns>An instance of <see cref="ImageTable" />.</returns>
        ImageTable ReadImageTable(long position, uint numberOfEntries);

        /// <summary>
        /// Returns an instance of <see cref="Image" /> for the image table and image specified
        /// </summary>
        /// <param name="imageTableLocation">The starting position to read the data for the image table</param>
        /// <param name="numberOfEntries">The number of entries in the image table</param>
        /// <param name="imageLocation">The starting position to read the data for the raw image data</param>
        /// <returns>An instance of <see cref="Image" />.</returns>
        Image ReadImage(long imageTableLocation, uint numberOfEntries, long imageLocation);
    }
}