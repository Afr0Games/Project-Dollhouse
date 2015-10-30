using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace Files.IFF
{
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
                m_Colors[i] = new Color(new Vector3(Reader.ReadByte(), Reader.ReadByte(), Reader.ReadByte()));

            Reader.Close();
            m_Data = null;
        }
    }
}
