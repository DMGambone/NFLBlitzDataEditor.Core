using System.IO;
using System.Collections.Generic;
using NFLBlitzDataEditor.Core.Models;

namespace NFLBlitzDataEditor.Core.Readers
{
    public interface IGameReader
    {
        /// <summary>
        /// Returns a stream that is pointing to data that would be stored in memory
        /// </summary>
        /// <param name="address">The address of data as it would be in memory</param>
        /// <param name="size">The size of the block of memory</param>
        Stream OpenMemoryRead(uint address, int size = -1);

        /// <summary>
        /// Reads the complete set of teams defined in the data file
        /// </summary>
        /// <returns>A collection of teams defined in the data file</returns>
        IEnumerable<Team> ReadAllTeams();

        /// <summary>
        /// Returns an instance of <see cref="ImageInfo" /> containing information about an image referenced by the memory address
        /// </summary>
        /// <param name="address">The memory address to read the data from</param>
        /// <returns>An instance of <see cref="ImageInfo" />.</returns>
        ImageInfo GetImageInfo(uint address);

        /// <summary>
        /// Returns the string located at a specific memory address
        /// </summary>
        /// <param name="address">The memory address to read the string from</param>
        /// <returns>The string value located at the address</returns>
        string ReadString(uint address);

        /// <summary>
        /// Returns the string located at a specific memory address
        /// </summary>
        /// <param name="address">The memory address to read the string from</param>
        /// <param name="size">The size of the string</param>
        /// <returns>The string value located at the address</returns>
        string ReadString(uint address, int size);


        /// <summary>
        /// Reads a complete play located a memory address specified
        /// </summary>
        /// <param name="address">The memory address where the play route is located</param>
        /// <returns>An instance of <see cref="Play" /> containing the complete play</returns>
        Play ReadPlay(uint address);

        /// <summary>
        /// Gets the complete playbook
        /// </summary>
        /// <returns>An instance of <see cref="Playbook" /> defined in the games</returns>
        Playbook GetPlaybook();
    }
}