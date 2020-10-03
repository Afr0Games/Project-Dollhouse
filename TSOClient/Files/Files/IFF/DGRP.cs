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
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using log4net;

namespace Files.IFF
{
    /// <summary>
    /// This chunk type collects SPR# and SPR2 resources into a "drawing group" which can be used to display one tile of 
    /// an object from all directions and zoom levels. Objects which span across multiple tiles have a separate DGRP 
    /// chunk for each tile. A DGRP chunk always consists of 12 images (one for every direction/zoom level combination), 
    /// which in turn contain info about one or more sprites.
    /// </summary>
    public class DGRP : IFFChunk, IDisposable
    {
        private ushort m_Version;
        public uint ImageCount;
        public List<DGRPImg> Images = new List<DGRPImg>();
        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public iSprite GetSprite(ushort ID)
        {
            return m_Parent.GetSprite(ID);
        }

        public DGRP(GraphicsDevice Device, IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            m_Version = Reader.ReadUShort();

            switch(m_Version)
            {
                case 20000:
                    ImageCount = Reader.ReadUShort();
                    break;
                case 20001:
                    ImageCount = Reader.ReadUShort();
                    break;
                case 20003:
                    ImageCount = Reader.ReadUInt32();
                    break;
                case 20004:
                    ImageCount = Reader.ReadUInt32();
                    break;
            }

            for(int i = 0; i < ImageCount; i++)
            {
                Images.Add(new DGRPImg(Device, m_Parent, Reader, m_Version));
            }

            Reader.Close();
            m_Data = null;
        }

        ~DGRP()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this FAR3Archive instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this DGRP instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                foreach(DGRPImg Img in Images)
                {
                    if (Img != null)
                        Img.Dispose();
                }

                // Prevent the finalizer from calling ~DGRP, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("DGRP not explicitly disposed!");
        }
    }

    public class SpriteInfo : IDisposable
    {
        private DGRPImg m_Img;
        public short Type = 0;
        public int SPRChunkID;
        public int SPRFrameNum;
        public int Flags = 0;
        public Vector2 SpriteOffset = new Vector2();
        public Vector3 ObjectOffset = new Vector3();
        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SpriteInfo(FileReader Reader, DGRPImg Img, uint Version)
        {
            if (Version == 20000 || Version == 20001)
            {
                Type = Reader.ReadInt16();
                SPRChunkID = Reader.ReadInt16();
                SPRFrameNum = Reader.ReadInt16();
                Flags = Reader.ReadInt16();
                SpriteOffset.X = Reader.ReadInt16();
                SpriteOffset.Y = Reader.ReadInt16();
                ObjectOffset.Z = Reader.ReadFloat();
            }
            else
            {
                SPRChunkID = Reader.ReadInt32();
                SPRFrameNum = Reader.ReadInt32();
                SpriteOffset.X = Reader.ReadInt32();
                SpriteOffset.Y = Reader.ReadInt32();
                ObjectOffset.Z = Reader.ReadFloat();
                Flags = Reader.ReadInt32();

                if (Version == 20004)
                {
                    ObjectOffset.Z = Reader.ReadFloat();
                    ObjectOffset.Z = Reader.ReadFloat();
                }
            }
        }

        ~SpriteInfo()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this FAR3Archive instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this SpriteInfo instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Img != null)
                    m_Img.Dispose();

                // Prevent the finalizer from calling ~SpriteInfo, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("SpriteInfo not explicitly disposed!");
        }
    }

    public class DrawGroupSprite : IDisposable
    {
        private SpriteInfo m_SprInfo;
        private readonly Texture2D m_Texture;
        private readonly iSpriteFrame m_Sprite;
        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ushort Type { get { return (ushort)m_SprInfo.Type; } }
        public uint Flags { get { return (uint)m_SprInfo.Flags; } }
        public Vector2 SpriteOffset { get { return m_SprInfo.SpriteOffset; } }
        public Vector3 ObjectOffset { get { return m_SprInfo.ObjectOffset; } }
        public Texture2D Texture { get { return m_Texture; } }
        public iSpriteFrame Sprite { get { return m_Sprite; } }

        public DrawGroupSprite(GraphicsDevice Device, SpriteInfo SprInfo, iSpriteFrame frame)
        {
            m_SprInfo = SprInfo;
            m_Sprite = frame;

            if ((m_SprInfo.Flags & 0x1) == 0x1)
            {
                m_Texture = SaveAsFlippedTexture2D(frame.GetTexture(), true, false);
            }
        }

        private Texture2D SaveAsFlippedTexture2D(Texture2D Input, bool Vertical, bool Horizontal)
        {
            Texture2D flipped = new Texture2D(Input.GraphicsDevice, Input.Width, Input.Height);
            Color[] Data = new Color[Input.Width * Input.Height];
            Color[] FlippedData = new Color[Data.Length];

            Input.GetData(Data);

            for (int x = 0; x < Input.Width; x++)
            {
                for (int y = 0; y < Input.Height; y++)
                {
                    int index = 0;
                    if (Horizontal && Vertical)
                        index = Input.Width - 1 - x + (Input.Height - 1 - y) * Input.Width;
                    else if (Horizontal && !Vertical)
                        index = Input.Width - 1 - x + y * Input.Width;
                    else if (!Horizontal && Vertical)
                        index = x + (Input.Height - 1 - y) * Input.Width;
                    else if (!Horizontal && !Vertical)
                        index = x + y * Input.Width;

                    FlippedData[x + y * Input.Width] = Data[index];
                }
            }

            flipped.SetData(FlippedData);

            return flipped;
        }

        ~DrawGroupSprite()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this FAR3Archive instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this DrawGroupSprite instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Texture != null)
                    m_Texture.Dispose();

                // Prevent the finalizer from calling ~DrawGroupImage, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("DrawGroupSprite not explicitly disposed!");
        }
    }

    /// <summary>
    /// Flags specifying what direction the object is facing in the image.
    /// </summary>
    public enum DirectionFlags
    {
        RightRear = 0x01,
        RightFront = 0x04,
        LeftFront = 0x10,
        LeftRear = 0x40
    }

    /// <summary>
    /// A drawgroup image, which can reference multiple sprites.
    /// </summary>
    public class DGRPImg : IDisposable
    {
        private GraphicsDevice m_Graphics;
        private SpriteBatch m_SBatch;

        public uint SpriteCount;
        public DirectionFlags Direction;
        public uint ZoomLevel;
        private List<DrawGroupSprite> m_Sprites = new List<DrawGroupSprite>();
        public Texture2D CompiledTexture;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DGRPImg(GraphicsDevice Device, Iff Parent, FileReader Reader, uint Version)
        {
            m_Graphics = Device;
            m_SBatch = new SpriteBatch(m_Graphics);

            if (Version == 20000 || Version == 20001)
            {
                SpriteCount = Reader.ReadUShort();
                Direction = (DirectionFlags)Reader.ReadByte();
                ZoomLevel = Reader.ReadByte();

                for (int i = 0; i < SpriteCount; i++)
                {
                    SpriteInfo Info = new SpriteInfo(Reader, this, Version);
                    m_Sprites.Add(new DrawGroupSprite(Device, Info, 
                        Parent.GetSprite((ushort)Info.SPRChunkID).GetFrame(Info.SPRFrameNum)));
                }
            }
            else
            {
                Direction = (DirectionFlags)Reader.ReadUInt32();
                ZoomLevel = Reader.ReadUInt32();
                SpriteCount = Reader.ReadUInt32();

                for (int i = 0; i < SpriteCount; i++)
                {
                    SpriteInfo Info = new SpriteInfo(Reader, this, Version);
                    m_Sprites.Add(new DrawGroupSprite(Device, Info, 
                        Parent.GetSprite((ushort)Info.SPRChunkID).GetFrame(Info.SPRFrameNum)));
                }
            }
        }

        /// <summary>
        /// Compiles the list of sprites into a tile bitmap
        /// </summary>
        public void CompileSprites()
        {
            // TODO: Render transparency and z-buffer channels
            // TODO: Mirrored sprites are not aligned correctly

            m_SBatch.Begin(SpriteSortMode.BackToFront);

            CompiledTexture = new Texture2D(m_Graphics, 136, 384); //TODO: Where is the size from?
            RenderTarget2D RTarget = new RenderTarget2D(m_Graphics, 
                CompiledTexture.Width, CompiledTexture.Height);

            m_Graphics.SetRenderTarget(RTarget);

            foreach (DrawGroupSprite Sprite in m_Sprites)
            {
                float xOffset = CompiledTexture.Width / 2 + Sprite.SpriteOffset.X;
                float yOffset = CompiledTexture.Height / 2 + Sprite.SpriteOffset.Y;

                m_SBatch.Draw(Sprite.Texture, new Rectangle((int)xOffset, (int)yOffset, CompiledTexture.Width, 
                    CompiledTexture.Height), Color.White);
            }

            m_SBatch.End();

            CompiledTexture = RTarget;
            m_Graphics.SetRenderTarget(null);
        }

        ~DGRPImg()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this DGRPImg instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this FAR3Archive instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (CompiledTexture != null)
                    CompiledTexture.Dispose();

                foreach(DrawGroupSprite Sprite in m_Sprites)
                {
                    if (Sprite != null)
                        Sprite.Dispose();
                }

                // Prevent the finalizer from calling ~DGRPImg, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("DGRPImg not explicitly disposed!");
        }
    }
}
