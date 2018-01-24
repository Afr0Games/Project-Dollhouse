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
using System.IO;
using System.Reflection;
using log4net;

namespace Files.AudioLogic
{
    /// <summary>
    /// HLS refers to two binary formats that both define a list of IDs, known as a hitlist.
    /// One format is a Pascal string with a 4-byte, little-endian length, representing a 
    /// comma-seperated list of decimal values, or decimal ranges (e.g. "1025-1035"), succeeded 
    /// by a single LF newline.
    /// </summary>
    public class HLS : IDisposable
    {
        private FileReader m_Reader;
        public List<uint> SoundsAndHitlists = new List<uint>();

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HLS(Stream Data)
        {
            if (Data != null)
            {
                m_Reader = new FileReader(Data, false);
                m_Reader.Seek(0);
            }
            else
                return;

            uint Unknown = m_Reader.ReadUInt32();

            try
            {
                if (Unknown == 1) //First format
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
                    string Str = m_Reader.ReadString((int)Unknown).Replace("\n", "");
                    string[] SplitByComma = Str.Split(',');

                    for(int i = 0; i < SplitByComma.Length; i++)
                    {
                        string[] SplitByDash = SplitByComma[i].Split('-');

                        if(SplitByDash.Length > 1)
                        {
                            uint Min = Convert.ToUInt32(SplitByDash[0]);
                            uint Max = Convert.ToUInt32(SplitByDash[1]);

                            for(uint j = Min; j <= Max; j++)
                                SoundsAndHitlists.Add(j);
                        }
                        else
                            SoundsAndHitlists.Add(Convert.ToUInt32(SplitByComma[i]));
                    }
                }
            }
            catch
            {
                m_Reader.Seek(4);
                for (int i = 0; i < Unknown; i++)
                    SoundsAndHitlists.Add(m_Reader.ReadUInt32());

                m_Reader.Close();
            }

            m_Reader.Close();
        }

        ~HLS()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this HLS instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this HLS instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Reader != null)
                    m_Reader.Close();

                // Prevent the finalizer from calling ~HLS, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("HLS not explicitly disposed!");
        }
    }
}
