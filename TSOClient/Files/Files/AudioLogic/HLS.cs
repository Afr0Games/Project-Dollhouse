using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace Files.AudioLogic
{
    public class HLS
    {
        private FileReader m_Reader;
        public List<uint> SoundsAndHitlists = new List<uint>();

        public HLS(Stream Data)
        {
            m_Reader = new FileReader(Data, false);

            uint Unknown = m_Reader.ReadUInt32();

            if(Unknown == 1) //First format
            {
                if ((m_Reader.StreamLength - m_Reader.Position) > 4) //... because sometimes it will just end here D:
                {
                    uint Count = m_Reader.ReadUInt32();

                    for (int i = 0; i < Count; i++)
                        SoundsAndHitlists.Add(m_Reader.ReadUInt32());
                }
            }
            else
            {
                ASCIIEncoding Enc = new ASCIIEncoding();
                string[] StrData = Enc.GetString(m_Reader.ReadBytes((int)Unknown)).Split(',');

                foreach(string Entry in StrData)
                {
                    if (Entry.Length != 0)
                    {
                        try
                        {
                            if (!Entry.Contains("-"))
                                SoundsAndHitlists.Add(uint.Parse(Entry.Replace("0x", ""), NumberStyles.HexNumber));
                            else
                            {
                                string[] Range = Entry.Split('-');

                                for (uint i = uint.Parse(Range[0].Replace("0x", "")); i < uint.Parse(Range[1].Replace("0x", ""), NumberStyles.HexNumber); i++)
                                    SoundsAndHitlists.Add(i);

                                SoundsAndHitlists.Add(uint.Parse(Range[1].Replace("0x", ""), NumberStyles.HexNumber));
                            }
                        }
                        catch(Exception)
                        {
                            throw new HLSException("Unknown HLS format!");
                        }
                    }
                }
            }

            m_Reader.Close();
        }
    }
}
