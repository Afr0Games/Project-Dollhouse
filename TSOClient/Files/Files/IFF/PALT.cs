/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.IO;
using Microsoft.Xna.Framework;

namespace Files.IFF
{
    /// <summary>
    /// This chunk type holds a color palette.
    /// </summary>
    public class PALT : IFFChunk
    {
        private Color[] m_Colors;

        /// <summary>
        /// Gets the specified color in this PALT chunk.
        /// </summary>
        /// <param name="Key">Index of color to retrieve.</param>
        /// <returns>A Color instance.</returns>
        public Color this[int Key]
        {
            get
            {
                return m_Colors[Key];
            }
        }

        public PALT(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            Reader.ReadUInt32(); //Version

            m_Colors = new Color[Reader.ReadUInt32()];
            Reader.ReadBytes(8); //Reserved

            for (int i = 0; i < m_Colors.Length; i++)
                m_Colors[i] = new Color(Reader.ReadByte(), Reader.ReadByte(), Reader.ReadByte());

            Reader.Close();
            m_Data = null;
        }
    }
}
