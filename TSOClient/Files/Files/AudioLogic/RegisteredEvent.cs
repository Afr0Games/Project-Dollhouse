using System;
using System.Collections.Generic;
using System.Text;

namespace Files.AudioLogic
{
    /// <summary>
    /// Represents an event registered by the VM.
    /// </summary>
    public class RegisteredEvent
    {
        /// <summary>
        /// The name of the event.
        /// </summary>
        public string Name;

        /// <summary>
        /// The ID of the track associated with this event.
        /// </summary>
        public uint TrackID;

        /// <summary>
        /// The resource group that this event belongs to. 
        /// </summary>
        public HitResourcegroup Rsc;
    }
}
