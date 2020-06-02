using System;
using System.Collections.Generic;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// A collection of <see cref="PlayRouteAction" /> representing the actions a player takes
    /// </summary>
    public class PlayRoute
    {
        public IEnumerable<PlayRouteAction> Actions { get; set; }
    }
}