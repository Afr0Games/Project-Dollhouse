/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Files.AudioLogic;

namespace Sound
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
        /// The type of the event.
        /// </summary>
        public HITEvents EventType;

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
