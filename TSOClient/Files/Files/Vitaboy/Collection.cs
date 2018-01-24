using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;

namespace Files.Vitaboy
{
    public class Collection : IDisposable
    {
        private FileReader m_Reader;
        public List<UniqueFileID> PurchasableOutfitIDs = new List<UniqueFileID>();

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        ~Collection()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this Collection instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this Collection instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Reader != null)
                    m_Reader.Dispose();

                // Prevent the finalizer from calling ~Collection, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("Collection not explicitly disposed!");
        }
    }
}
