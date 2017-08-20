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
using System.Globalization;

namespace Files.AudioLogic
{
    public enum HITEvents
    {
        kSoundobPlay = 1,
        kSoundobStop = 2,
        kSoundobKill = 3,
        kSoundobUpdate = 4,
        kSoundobSetVolume = 5,
        kSoundobSetPitch = 6,
        kSoundobSetPan = 7,
        kSoundobSetPosition = 8,
        kSoundobSetFxType = 9,
        kSoundobSetFxLevel = 10,
        kSoundobPause = 11,
        kSoundobUnpause = 12,
        kSoundobLoad = 13,
        kSoundobUnload = 14,
        kSoundobCache = 15,
        kSoundobUncache = 16,
        kSoundobCancelNote = 19,
        kKillAll = 20,
        kPause = 21,
        kUnpause = 22,
        kKillInstance = 23,
        kTurnOnTV = 30,
        kTurnOffTV = 31,
        kUpdateSourceVolPan = 32,
        kSetMusicMode = 36,
        kPlayPiano = 43,
        debugeventson = 44,
        debugeventsoff = 45,
        debugsampleson = 46,
        debugsamplesoff = 47,
        debugtrackson = 48,
        debugtracksoff = 49
    }

    public class TrackEvent
    {
        public string Name;
        public HITEvents EventType;
        public uint TrackID;
        public uint Unknown;
        public uint Unknown2;
        public uint Unknown3;
        public uint Unknown4;
    }

    public class EVT : IDisposable
    {
        private FileReader m_Reader;
        public List<TrackEvent> Events = new List<TrackEvent>();

        public EVT(Stream Data)
        {
            m_Reader = new FileReader(Data, false);
            ASCIIEncoding Enc = new ASCIIEncoding();

            string[] TrackEvents = Enc.GetString(m_Reader.ReadToEnd()).Split("\r\n".ToCharArray(), 
                StringSplitOptions.RemoveEmptyEntries);

            foreach(string TrckEvent in TrackEvents)
            {
                string[] Elements = TrckEvent.Split(',');
                TrackEvent Event = new TrackEvent();
                Event.Name = Elements[0];
                Event.EventType = (HITEvents)Enum.ToObject(typeof(HITEvents), ParseHexString(Elements[1]));
                if(!Event.Name.Contains("bkground")) //Sigh, Maxis...
                    Event.TrackID = (Elements[2].Equals("", StringComparison.InvariantCultureIgnoreCase)) ? 0 : uint.Parse(Elements[2].Replace("0x", ""), NumberStyles.HexNumber);
                else
                    Event.TrackID = (Elements[2].Equals("", StringComparison.InvariantCultureIgnoreCase)) ? 0 : uint.Parse(Elements[2]);
                Event.Unknown = ParseHexString(Elements[3]);
                Event.Unknown2 = ParseHexString(Elements[4]);
                Event.Unknown3 = ParseHexString(Elements[5]);
                Event.Unknown4 = ParseHexString(Elements[6]);
                Events.Add(Event);
            }

            m_Reader.Close();
        }

        /// <summary>
        /// Converts the given hex string to its unsigned integer equivalent.
        /// </summary>
        /// <param name="HexStr">The hex string.</param>
        /// <returns>A uint representation of the hex string.</returns>
        private uint ParseHexString(string HexStr)
        {
            bool IsHex = false;
            HexStr = HexStr.ToLowerInvariant();

            if (HexStr == "") return 0;
            if (HexStr.StartsWith("0x"))
            {
                HexStr = HexStr.Substring(2);
                IsHex = true;
            }
            //Sigh, Maxis...
            else if (HexStr.Contains("a") || HexStr.Contains("b") || HexStr.Contains("b") ||
                HexStr.Contains("c") || HexStr.Contains("d") || HexStr.Contains("e") || HexStr.Contains("f"))
            {
                IsHex = true;
            }

            if (IsHex)
            {
                return Convert.ToUInt32(HexStr, 16);
            }
            else
            {
                return Convert.ToUInt32(HexStr);
            }
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
