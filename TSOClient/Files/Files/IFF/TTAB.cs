using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Files.IFF
{
    public class TTAB : IFFChunk
    {
        public TTAB(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);
        }
    }
}
