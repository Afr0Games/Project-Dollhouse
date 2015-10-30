using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Files.IFF
{
    /// <summary>
    /// This chunk type holds a regular Windows BMP file.
    /// </summary>
    public class BMP_ : IFFChunk
    {
        public MemoryStream BitmapStream;

        public BMP_(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);
            BitmapStream = new MemoryStream(Reader.ReadToEnd());

            Reader.Close();
        }
    }
}
