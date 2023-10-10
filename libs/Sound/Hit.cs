/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Text;
using System.IO;
using System.Reflection;
using Files;
using log4net;

namespace Sound
{
    public class Hit : IDisposable
    {
        private FileReader m_Reader;

        public ExportTable ExTable;
        public byte[] InstructionData;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Hit(Stream Data)
        {
            m_Reader = new FileReader(Data, false);

            ASCIIEncoding Enc = new ASCIIEncoding();
            string MagicNumber = Enc.GetString(m_Reader.ReadBytes(4));

            if (!MagicNumber.Equals("HIT!", StringComparison.InvariantCultureIgnoreCase))
                throw new HitException("MagicNumber was wrong - Hit.cs!");

            m_Reader.ReadUInt32(); //MajorVersion
            m_Reader.ReadUInt32(); //MinorVersion

            string Trax = Enc.GetString(m_Reader.ReadBytes(4));
            if (!Trax.Equals("TRAX", StringComparison.InvariantCultureIgnoreCase))
                throw new HitException("Invalid TRAX header - Hit.cs!");

            ExTable = new ExportTable(Data);
            m_Reader.Seek(0);
            InstructionData = m_Reader.ReadBytes((int)m_Reader.StreamLength);

            m_Reader.Close();
        }

        ~Hit()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this Hit instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this Hit instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Reader != null)
                    m_Reader.Close();

                // Prevent the finalizer from calling ~Hit, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("Hit not explicitly disposed!");
        }
    }
}
