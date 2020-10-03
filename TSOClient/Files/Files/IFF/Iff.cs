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
using System.Diagnostics;
using System.Linq;

namespace Files.IFF
{
    /// <summary>
    /// Interchange File Format (IFF) is a chunk-based file format for binary resource data intended to promote a 
    /// common model for store and use by an executable.
    /// </summary>
    public class Iff : IDisposable
    {
        GraphicsDevice m_Device;

        /// <summary>
        /// Graphicsdevice used to construct SPR# and SPR2 chunks.
        /// </summary>
        public GraphicsDevice Device { get { return m_Device; } }

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


#if DEBUG

        /// <summary>
        /// The chunks in this IFF. Only used by Iffinator.
        /// </summary>
        public List<IFFChunk> Chunks = new List<IFFChunk>();

        /// <summary>
        /// This *.iff file's list of SPR chunks.
        /// Used by Iffinator.
        /// </summary>
        public List<SPR> SPRs
        {
            get
            {
                List<SPR> Sprites = new List<SPR>();
                foreach (KeyValuePair<ushort, IFFChunk> KVP in m_SPRChunks)
                    Sprites.Add((SPR)KVP.Value);

                return Sprites;
            }
        }

        /// <summary>
        /// This *.iff file's list of SPR2 chunks.
        /// Used by Iffinator.
        /// </summary>
        public List<SPR2> SPR2s
        {
            get
            {
                List<SPR2> Sprites = new List<SPR2>();
                foreach (KeyValuePair<ushort, IFFChunk> KVP in m_SPR2Chunks)
                    Sprites.Add((SPR2)KVP.Value);

                return Sprites;
            }
        }

        /// <summary>
        /// This *.iff file's list of DGRP chunks.
        /// Used by Iffinator.
        /// </summary>
        public List<DGRP> DrawGroups
        {
            get
            {
                List<DGRP> DrawGroups = new List<DGRP>();
                foreach (KeyValuePair<ushort, IFFChunk> KVP in m_DGRPChunks)
                    DrawGroups.Add((DGRP)KVP.Value);

                return DrawGroups;
            }
        }

        /// <summary>
        /// This *.iff file's list of BHAV chunks.
        /// Used by Iffinator.
        /// </summary>
        public List<BHAV> BHAVs
        {
            get
            {
                List<BHAV> Behaviours = new List<BHAV>();
                foreach (KeyValuePair<ushort, IFFChunk> KVP in m_BHAVChunks)
                    Behaviours.Add((BHAV)KVP.Value);

                return Behaviours;
            }
        }

        /// <summary>
        /// This *.iff file's list of STR# chunks.
        /// Used by Iffinator.
        /// </summary>
        public List<STR> StringTables
        {
            get
            {
                List<STR> STables = new List<STR>();
                foreach (KeyValuePair<ushort, IFFChunk> KVP in m_STRChunks)
                    STables.Add((STR)KVP.Value);

                return STables;
            }
        }
#endif

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
        /// return the first OBJD found in the IFF. If the IFF
        /// doesn't have any OBJDs, this will return null.
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
                {
                    if (m_OBJDs.Count > 0)
                        return m_OBJDs[0];
                    else return null;
                }
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
        /// Gets a specific DGRP from this IFF.
        /// </summary>
        /// <param name="ID">ID of the DGRP to get.</param>
        /// <returns>A DGRP chunk.</returns>
        public DGRP GetDGRP(ushort ID)
        {
            return (DGRP)m_DGRPChunks[ID];
        }

        /// <summary>
        /// Gets a specific SPR or SPR2 from this IFF.
        /// </summary>
        /// <param name="ID">ID of the SPR or SPR2 to get.</param>
        /// <returns>A SPR or SPR2 chunk.</returns>
        public iSprite GetSprite(ushort ID)
        {
            if (m_SPRChunks.ContainsKey(ID))
                return (iSprite)m_SPRChunks[ID];
            else
                return (iSprite)m_SPR2Chunks[ID];
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

        public Iff(GraphicsDevice Device)
        {
            m_Device = Device;
        }

        public Iff()
        {
        }

        /// <summary>
        /// Attempts to read an IFF file from a given stream.
        /// </summary>
        /// <param name="Data">The stream from which to open the IFF.</param>
        /// <param name="ThrowException">Should this method throw an exception if it couldn't open the file?</param>
        /// <returns>True if successful, false otherwise (if ThrowException is set to false).</returns>
        public bool Init(Stream Data, bool ThrowException)
        {
            m_Reader = new FileReader(Data, true);

            string MagicNumber = m_Reader.ReadString(60);

            if (!MagicNumber.Equals("IFF FILE 2.5:TYPE FOLLOWED BY SIZE\0 JAMIE DOORNBOS & MAXIS 1\0", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ThrowException)
                    throw new IFFException("MagicNumber was wrong - IFF.cs!");
                else
                    return false;
            }

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
                        DGRP DGRPChunk = new DGRP(m_Device, Chunk);
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

                if (IsAssemblyDebugBuild(Assembly.GetExecutingAssembly()))
                    Chunks.Add(Chunk);
            }

            return true;
        }

        private bool IsAssemblyDebugBuild(Assembly assembly)
        {
            return assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled);
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

                foreach(KeyValuePair<ushort, IFFChunk> KVP in m_SPR2Chunks)
                {
                    SPR2 Sprite = (SPR2)KVP.Value;

                    if (Sprite != null)
                        Sprite.Dispose();
                }

                foreach (KeyValuePair<ushort, IFFChunk> KVP in m_SPRChunks)
                {
                    SPR Sprite = (SPR)KVP.Value;

                    if (Sprite != null)
                        Sprite.Dispose();
                }

                foreach (KeyValuePair<ushort, IFFChunk> KVP in m_BMP_Chunks)
                {
                    BMP_ Bitmap = (BMP_)KVP.Value;

                    if (Bitmap != null)
                        Bitmap.Dispose();
                }

                foreach (KeyValuePair<ushort, IFFChunk> KVP in m_FBMPChunks)
                {
                    FBMP Bitmap = (FBMP)KVP.Value;

                    if (Bitmap != null)
                        Bitmap.Dispose();
                }

                // Prevent the finalizer from calling ~Iff, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("Iff not explicitly disposed!");
        }
    }
}
