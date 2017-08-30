/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the SimsLib.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Text;
using System.IO;
using System.Globalization;

namespace Files.AudioLogic
{
    public enum HITTrackArguments
    {
        kArgsNNormal = 0,
        kArgsVolPan = 1,
        kArgsIDVolPan = 2,
        kArgsXYZ = 3
    }

    public enum HITControlGroup
    {
        kGroupSfx = 1,
        kGroupMusic = 2,
        kGroupVox = 3
    }

    /// <summary>
    /// TRK is a CSV format that defines a HIT track, analogous to the HOT Track section in the The Sims 1. 
    /// It may optionally be stored as a Pascal string with a 4-byte little-endian length; if so, it is 
    /// preceded by the 4-byte magic number "2DKT".
    /// </summary>
    public class TRK : IDisposable
    {
        private FileReader m_Reader;
        private int m_Version;
        public string TrackName;
        public uint SoundID; //InstanceID of sound associated with this track.
        public HITTrackArguments Argument;
        public HITControlGroup ControlGroup;
        public int DuckingPriority = 0;
        public bool Looped = false;
        public int Volume = 0;

        public TRK(Stream Data)
        {
            m_Reader = new FileReader(Data, false);
            string DataStr = "";
            string[] Elements;

            ASCIIEncoding Enc = new ASCIIEncoding();
            string MagicNumber = Enc.GetString(m_Reader.ReadBytes(4));

            if (!MagicNumber.Equals("2DKT", StringComparison.InvariantCultureIgnoreCase) && !MagicNumber.Equals("TKDT", StringComparison.InvariantCultureIgnoreCase))
                throw new TRKException("Invalid TrackData header - TRK.cs");

            if (MagicNumber.Equals("2DKT", StringComparison.InvariantCultureIgnoreCase))
            {
                DataStr = Enc.GetString(m_Reader.ReadBytes((int)m_Reader.ReadUInt32()));
                Elements = DataStr.Split(',');
            }
            else
                Elements = Enc.GetString(m_Reader.ReadToEnd()).Split(',');

            m_Version = int.Parse(Elements[1], NumberStyles.Integer);
            TrackName = Elements[2];

            if (!Elements[3].Equals("", StringComparison.InvariantCultureIgnoreCase))
                SoundID = uint.Parse(Elements[3].Replace("0x", ""), NumberStyles.HexNumber);
            else
                SoundID = 0;

            if (Elements[5].Equals("\r\n", StringComparison.InvariantCultureIgnoreCase))
                return;

            if (!Elements[5].Equals("", StringComparison.InvariantCultureIgnoreCase))
                Argument = (HITTrackArguments)Enum.Parse(typeof(HITTrackArguments), Elements[5]);

            if (!Elements[7].Equals("", StringComparison.InvariantCultureIgnoreCase))
                ControlGroup = (HITControlGroup)Enum.Parse(typeof(HITControlGroup), Elements[7]);

            if (!Elements[(m_Version != 2) ? 11 : 12].Equals("", StringComparison.InvariantCultureIgnoreCase))
                DuckingPriority = int.Parse(Elements[(m_Version != 2) ? 11 : 12], NumberStyles.Integer);

            if (!Elements[(m_Version != 2) ? 12 : 13].Equals("", StringComparison.InvariantCultureIgnoreCase))
                Looped = (int.Parse(Elements[(m_Version != 2) ? 12 : 13], NumberStyles.Integer) != 0) ? true : false;

            if (!Elements[(m_Version != 2) ? 13 : 14].Equals("", StringComparison.InvariantCultureIgnoreCase))
                Volume = int.Parse(Elements[(m_Version != 2) ? 13 : 14], NumberStyles.Integer);

            m_Reader.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool CleanUpNativeAndManagedResources)
        {
            if (CleanUpNativeAndManagedResources)
            {
                if (m_Reader != null)
                    m_Reader.Close();
            }
        }
    }
}
