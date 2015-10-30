using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Files.IFF
{
    /// <summary>
    /// This chunk type holds a number of constants that behavior code can refer to. 
    /// Labels may be provided for them in a TRCN chunk with the same ID.
    /// </summary>
    public class BCON: IFFChunk
    {
        public List<short> Constants = new List<short>();

        public BCON(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            byte NumConstants = Reader.ReadByte();
            Reader.ReadByte(); //Unknown.

            for (int i = 0; i < NumConstants; i++)
                Constants.Add(Reader.ReadInt16());
        }
    }
}
