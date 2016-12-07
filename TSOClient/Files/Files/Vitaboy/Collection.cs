using System;
using System.Collections.Generic;
using System.IO;

namespace Files.Vitaboy
{
    public class Collection : IDisposable
    {
        private FileReader m_Reader;
        public List<UniqueFileID> PurchasableOutfitIDs = new List<UniqueFileID>();

        public Collection(Stream Data)
        {
            m_Reader = new FileReader(Data, true);

            uint Count = m_Reader.ReadUInt32();
            uint FileID = 0, TypeID = 0;

            for(int i = 0; i < Count; i++)
            {
                m_Reader.ReadUInt32(); //Index

                FileID = m_Reader.ReadUInt32();
                TypeID = m_Reader.ReadUInt32();

                PurchasableOutfitIDs.Add(new UniqueFileID(TypeID, FileID));
            }
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
                if (m_Reader != null)
                    m_Reader.Dispose();
            }
        }
    }
}
