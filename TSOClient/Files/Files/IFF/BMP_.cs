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

namespace Files.IFF
{
    /// <summary>
    /// This chunk type holds a regular Windows BMP file.
    /// </summary>
    public class BMP_ : IFFChunk, IDisposable
    {
        public MemoryStream BitmapStream;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BMP_(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);
            BitmapStream = new MemoryStream(Reader.ReadToEnd());

            Reader.Close();
        }

        ~BMP_()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this BMP_ instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this BMP_ instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (BitmapStream != null)
                    BitmapStream.Dispose();

                // Prevent the finalizer from calling ~BMP_, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("BMP_ not explicitly disposed!");
        }
    }
}
