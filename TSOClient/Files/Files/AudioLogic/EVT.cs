using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Files.AudioLogic
{
    public class TrackEvent
    {
        public string Name;
        public string Event;
        public uint TrackID;
    }

    public class EVT
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
                Event.Event = Elements[1];
                Event.TrackID = (Elements[2].Equals("", StringComparison.InvariantCultureIgnoreCase)) ? 0 : uint.Parse(Elements[2].Replace("0x", ""), NumberStyles.HexNumber);
                Events.Add(Event);
            }

            m_Reader.Close();
        }
    }
}
