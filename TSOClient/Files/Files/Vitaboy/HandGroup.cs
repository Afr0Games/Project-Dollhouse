/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.IO;
using System.Reflection;
using log4net;

namespace Files.Vitaboy
{
    public class HandGroup : IDisposable
    {
        private FileReader m_Reader;

        public HandSet Light, Medium, Dark;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Constructs a new instance of the HandGroup class.
        /// </summary>
        /// <param name="Data">A stream of data retrieved from a FAR3 archive.</param>
        public HandGroup(Stream Data)
        {
            m_Reader = new FileReader(Data, true);

            m_Reader.ReadUInt32(); //Version.

            Light = new HandSet(m_Reader);
            Medium = new HandSet(m_Reader);
            Dark = new HandSet(m_Reader);
        }

        ~HandGroup()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this HandGroup instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this HandGroup instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Reader != null)
                    m_Reader.Dispose();

                // Prevent the finalizer from calling ~HandGroup, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("HandGroup not explicitly disposed!");
        }
    }
}
