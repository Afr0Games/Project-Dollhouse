using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Files.Vitaboy
{
    /// <summary>
    /// Appearances (known as suits in The Sims 1) collect together bindings and attribute them with a preview thumbnail.
    /// </summary>
    public class Appearance
    {
        public UniqueFileID ThumbnailID;
        public List<UniqueFileID> BindingIDs = new List<UniqueFileID>();

        public Appearance(Stream Data)
        {
            FileReader Reader = new FileReader(Data, true);

            Reader.ReadUInt32(); //Version

            uint FileID = Reader.ReadUInt32(), TypeID = Reader.ReadUInt32();

            ThumbnailID = new UniqueFileID(TypeID, FileID);

            uint BindingCount = Reader.ReadUInt32();
            for (uint i = 0; i < BindingCount; i++)
            {
                FileID = Reader.ReadUInt32();
                TypeID = Reader.ReadUInt32();
                BindingIDs.Add(new UniqueFileID(TypeID, FileID));
            }
        }
    }
}
