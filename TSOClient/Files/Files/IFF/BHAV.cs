using System;
using System.Linq;
using System.Text;
using System.IO;

namespace Files.IFF
{
    /// <summary>
    /// This chunk type holds Behavior code in SimAntics.
    /// </summary>
    public class BHAV : IFFChunk
    {
        public BHAV(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);
        }
    }
}
