using System;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Contains Information about a play route
    /// </summary>
    public class PlayRouteAction
    {
        /// <summary>
        /// The endzone-to-endzone direction the player should move
        /// </summary>
        public byte X { get; set; }

        /// <summary>
        /// The sideline-to-sideline direction the player should move
        /// </summary>
        public byte Z { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte Unknown { get; set; }

        /// <summary>
        /// The action the player should perform
        /// </summary>
        public RouteAction Action { get; set; }
    }
}