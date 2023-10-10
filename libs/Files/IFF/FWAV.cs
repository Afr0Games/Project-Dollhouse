/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.IO;

namespace Files.IFF
{

    /// <summary>
    /// This chunk type holds a null-terminated string that specifies the name of a track/event pair. 
    /// This chunk type is used by the SimAntics "Play sound event" instruction, which looks up the 
    /// track/event pair with this name in the EVT files of the game, and then sends the event to the track.
    /// </summary>
    public class FWAV : IFFChunk
    {
        public string TrackEventPair;

        public FWAV(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            TrackEventPair = Reader.ReadCString();

            Reader.Close();
            m_Data = null;
        }
    }
}
