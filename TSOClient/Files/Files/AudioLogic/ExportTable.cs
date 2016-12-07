/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Files.AudioLogic
{
    /// <summary>
    /// The export table was introduced into the HIT format in TSO. The HIT export table exports subroutines to handle the 
    /// kSndobPlay events for tracks. 
    /// </summary>
    public class ExportTable
    {
        private Dictionary<uint, uint> m_SubRoutines = new Dictionary<uint, uint>();

        public ExportTable(Stream Data)
        {
            FileReader Reader = new FileReader(Data, false);

            int StartLocation = Data.BinaryContains(new byte[] { (byte)'E', (byte)'N', (byte)'T', (byte)'P' });
            Reader.Seek(StartLocation);

            while (true)
            {
                string EndTest = ASCIIEncoding.ASCII.GetString(Reader.ReadBytes(4));

                if (!EndTest.Equals("EENT", StringComparison.InvariantCultureIgnoreCase))
                {
                    Reader.Seek(Reader.Position - 4);
                    uint TrackID = Reader.ReadUInt32();
                    uint Address = Reader.ReadUInt32();
                    //TrackID, Address
                    m_SubRoutines.Add(TrackID, Address);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Returns an subroutine address for a corresponding TrackID.
        /// </summary>
        /// <param name="TrackID">ID of the track for which to find a subroutine.</param>
        /// <returns>An address for a subroutine if found, otherwise returns null.</returns>
        public uint FindSubroutineForTrack(uint TrackID)
        {
            foreach (KeyValuePair<uint, uint> KVP in m_SubRoutines)
                if (KVP.Key == TrackID)
                    return KVP.Value;

            return 0;
        }
    }
}
