using System;
using System.Collections.Generic;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Contains information about a single play from the playbook
    /// </summary>
    public class Play
    {
        /// <summary>
        /// The memory address of the initial play formation
        /// </summary>
        public uint PlayFormationAddress { get; set; }

        /// <summary>
        /// Indicator of the type of play
        /// </summary>
        public PlayType Type { get; set; }

        /// <summary>
        /// The memory address of each player's route once the play starts
        /// </summary>
        public uint[] RouteAddresses { get; set; }

        #region Resolved Values
        /// <summary>
        /// The play's initial formation
        /// </summary>
        public PlayFormation[] Formation { get; set; } = new PlayFormation[6];

        /// <summary>
        /// The routes executed by individual players
        /// </summary>
        public PlayRoute[] Routes { get; set; } = new PlayRoute[6];
        #endregion
    }
}