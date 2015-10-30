using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Files.IFF
{
    public class TPRP : IFFChunk
    {
        public TPRP(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);
        }
    }
}
