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

namespace Files.IFF
{
    /// <summary>
    /// This chunk type holds a regular Windows BMP file.
    /// </summary>
    public class BMP_ : IFFChunk, IDisposable
    {
        public MemoryStream BitmapStream;

        public BMP_(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);
            BitmapStream = new MemoryStream(Reader.ReadToEnd());

            Reader.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool CleanUpManagedResources)
        {
            if (CleanUpManagedResources)
            {
                if (BitmapStream != null)
                    BitmapStream.Dispose();
            }
        }
    }
}
