/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using log4net;

namespace Files.IFF
{
    /// <summary>
    /// Interchange File Format (IFF) is a chunk-based file format for binary resource data intended to promote a 
    /// common model for store and use by an executable.
    /// </summary>
    public class Iff : IDisposable
    {
        /// <summary>
        /// Graphicsdevice used to construct SPR# and SPR2 chunks.
        /// Will be 0 if IFF is not of type SPF.
        /// </summary>
        private GraphicsDevice m_Device;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private FileReader m_Reader;
        private Dictionary<ushort, IFFChunk> m_BHAVChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_OBJfChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_FBMPChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_BMP_Chunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_GLOBChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_BCONChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_TTABChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_TTAsChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_TPRPChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_DGRPChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_FWAVChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_FCNSChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_SPR2Chunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_CTSSChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_PALTChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_SPRChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_STRChunks = new Dictionary<ushort, IFFChunk>();
        private Dictionary<ushort, IFFChunk> m_CSTChunks = new Dictionary<ushort, IFFChunk>();
        private List<OBJD> m_OBJDs = new List<OBJD>();

        /// <summary>
        /// Is this a multi-tile object?
        /// </summary>
        public bool MultiTile
        {
            get
            {
                foreach (OBJD ObjectDefinition in m_OBJDs)
                {
                    if (ObjectDefinition.IsMaster)
                    {
                        if (ObjectDefinition.IsMultiTile)
                            return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the master OBJD for a multi tile object.
        /// If this object isn't multi tile, this will simply
        /// return the first OBJD found in the IFF.
        /// </summary>
        public OBJD Master
        {
            get
            {
                if (MultiTile)
                {
                    foreach (OBJD ObjectDefinition in m_OBJDs)
                    {
                        if (ObjectDefinition.IsMaster)
                            return ObjectDefinition;
                    }

                    //Should never end up here, because a multi tile object will always have a OBJD with a SubIndex of -1.
                    return m_OBJDs[0];
                }
                else
                    return m_OBJDs[0];
            }
        }

        /// <summary>
        /// Get a specific palette from this IFF.
        /// </summary>
        /// <param name="ID">ID of the palette to get.</param>
        /// <returns>A PALT chunk.</returns>
        public PALT GetPalette(ushort ID)
        {
            return (PALT)m_PALTChunks[ID];
        }

        /// <summary>
        /// Get a specific STR# from this IFF.
        /// </summary>
        /// <param name="ID">ID  of the STR# to get.</param>
        /// <returns>A STR# chunk.</returns>
        public STR GetSTR(ushort ID)
        {
            return (STR)m_STRChunks[ID];
        }

        /// <summary>
        /// Get a specific BHAV from this IFF.
        /// </summary>
        /// <param name="ID">ID  of the BHAV to get.</param>
        /// <returns>A BHAV chunk.</returns>
        public BHAV GetBHAV(ushort ID)
        {
            return (BHAV)m_BHAVChunks[ID];
        }

        /// <summary>
        /// Get a specific GLOB from this IFF.
        /// </summary>
        /// <param name="ID">The ID of the GLOB to get.</param>
        /// <returns>A GLOB chunk.</returns>
        public GLOB GetGLOB(ushort ID)
        {
            return (GLOB)m_BHAVChunks[ID];
        }

        public Iff(Stream Data, GraphicsDevice Device)
        {
            m_Device = Device;
            Init(Data);
        }

        public Iff(Stream Data)
        {
            Init(Data);
        }

        private void Init(Stream Data)
        {
            m_Reader = new FileReader(Data, true);

            string MagicNumber = m_Reader.ReadString(60);

            if (!MagicNumber.Equals("IFF FILE 2.5:TYPE FOLLOWED BY SIZE\0 JAMIE DOORNBOS & MAXIS 1\0", StringComparison.InvariantCultureIgnoreCase))
                throw new IFFException("MagicNumber was wrong - IFF.cs!");

            m_Reader.ReadUInt32(); //RSMP offset

            //Size of a chunk header is 76 bytes.
            while ((m_Reader.StreamLength - m_Reader.Position) > 76)
            {
                IFFChunk Chunk;

                if (m_Device != null)
                    Chunk = new IFFChunk(m_Reader, m_Device, this);
                else
                    Chunk = new IFFChunk(m_Reader, this);

                switch (Chunk.Type)
                {
                    case IFFChunkTypes.FBMP:
                        FBMP FBMPChunk = new FBMP(Chunk);
                        m_FBMPChunks.Add(Chunk.ID, FBMPChunk);
                        break;
                    case IFFChunkTypes.FWAV:
                        FWAV FWAVChunk = new FWAV(Chunk);
                        m_FWAVChunks.Add(Chunk.ID, FWAVChunk);
                        break;
                    case IFFChunkTypes.BMP_:
                        BMP_ BMPChunk = new BMP_(Chunk);
                        m_BMP_Chunks.Add(Chunk.ID, BMPChunk);
                        break;
                    case IFFChunkTypes.DGRP:
                        DGRP DGRPChunk = new DGRP(Chunk);
                        m_DGRPChunks.Add(Chunk.ID, DGRPChunk);
                        break;
                    case IFFChunkTypes.BCON:
                        BCON BCONChunk = new BCON(Chunk);
                        m_BCONChunks.Add(Chunk.ID, BCONChunk);
                        break;
                    case IFFChunkTypes.GLOB:
                        GLOB GlobChunk = new GLOB(Chunk);
                        m_GLOBChunks.Add(Chunk.ID, GlobChunk);
                        break;
                    case IFFChunkTypes.OBJD:
                        OBJD OBJDChunk = new OBJD(Chunk);
                        m_OBJDs.Add(OBJDChunk);
                        break;
                    case IFFChunkTypes.TTAs:
                        TTAs TTAsChunk = new TTAs(Chunk);
                        m_TTAsChunks.Add(Chunk.ID, TTAsChunk);
                        break;
                    case IFFChunkTypes.TTAB:
                        TTAB TTABChunk = new TTAB(Chunk);
                        TTABChunk.Type = Chunk.Type;
                        TTABChunk.ID = Chunk.ID;
                        m_TTABChunks.Add(Chunk.ID, TTABChunk);
                        break;
                    case IFFChunkTypes.TPRP:
                        TPRP TPRPChunk = new TPRP(Chunk);
                        m_TPRPChunks.Add(Chunk.ID, TPRPChunk);
                        break;
                    case IFFChunkTypes.STR:
                        STR STRChunk = new STR(Chunk);
                        m_STRChunks.Add(Chunk.ID, STRChunk);
                        break;
                    case IFFChunkTypes.BHAV:
                        BHAV BHAVChunk = new BHAV(Chunk);
                        m_BHAVChunks.Add(Chunk.ID, BHAVChunk);
                        break;
                    case IFFChunkTypes.OBJf:
                        OBJf OBJfChunk = new OBJf(Chunk);
                        m_OBJfChunks.Add(Chunk.ID, OBJfChunk);
                        break;
                    case IFFChunkTypes.FCNS:
                        FCNS FCNSChunk = new FCNS(Chunk);
                        m_FCNSChunks.Add(Chunk.ID, FCNSChunk);
                        break;
                    case IFFChunkTypes.SPR:
                        SPR SPRChunk = new SPR(Chunk);
                        m_SPRChunks.Add(Chunk.ID, SPRChunk);
                        break;
                    case IFFChunkTypes.SPR2:
                        SPR2 SPR2Chunk = new SPR2(Chunk);
                        m_SPR2Chunks.Add(Chunk.ID, SPR2Chunk);
                        break;
                    case IFFChunkTypes.PALT:
                        PALT PALTChunk = new PALT(Chunk);
                        m_PALTChunks.Add(Chunk.ID, PALTChunk);
                        break;
                    case IFFChunkTypes.CTSS:
                        CTSS CTSSChunk = new CTSS(Chunk);
                        m_CTSSChunks.Add(Chunk.ID, CTSSChunk);
                        break;
                    case IFFChunkTypes.CST:
                        CST CSTChunk = new CST(Chunk);
                        m_CSTChunks.Add(Chunk.ID, CSTChunk);
                        break;
                }
            }
        }

        ~Iff()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this Iff instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this Iff instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Reader != null)
                    m_Reader.Dispose();

                // Prevent the finalizer from calling ~Iff, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("Iff not explicitly disposed!");
        }
    }
}
