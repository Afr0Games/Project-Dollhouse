/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework.Graphics;

namespace Terrain
{
    /// <summary>
    /// Represents a city mesh.
    /// </summary>
    public class CityMesh : IDisposable
    {
        private GraphicsDevice m_GraphicsDevice;
        private Effect m_Effect;

        private bool m_IsInitialized = false;

        /// <summary>
        /// Returns true if this mesh has been initialized, false otherwise.
        /// </summary>
        public bool IsInitialized
        {
            get { return m_IsInitialized; }
        }

        /// <summary>
        /// Gets this CityMesh's GraphicsDevice instance.
        /// </summary>
        public GraphicsDevice Device { get { return m_GraphicsDevice; } }

        /// <summary>
        /// Gets this CityMesh's Effect instance.
        /// </summary>
        public Effect FX { get { return m_Effect; } }

        /// <summary>
        /// Represents a part of a city mesh.
        /// </summary>
        private class MeshPart
        {
            public List<VertexPositionTexture> Vertices = new List<VertexPositionTexture>();
            public List<ushort> Indices = new List<ushort>();
            public int IndexOffset = 0;
            public VertexBuffer VertexBuffer;
            public IndexBuffer IndexBuffer;

            //Properties for subset rendering
            public int StartIndex = 0;
            public int PrimitiveCount = 0;
        }

        private List<MeshPart> m_Parts;

        /// <summary>
        /// Gets the number of vertices in this mesh.
        /// </summary>
        public int VertexCount
        {
            get
            {
                int Count = 0;

                foreach (var part in m_Parts)
                    Count += part.Vertices.Count;

                return Count;
            }
        }

        /// <summary>
        /// Constructs a new CityMesh instance.
        /// </summary>
        /// <param name="graphicsDevice">An GraphicsDevice instance.</param>
        /// <param name="effect">An Effect instance.</param>
        public CityMesh(GraphicsDevice graphicsDevice, Effect effect)
        {
            m_GraphicsDevice = graphicsDevice;
            m_Effect = effect;
            m_Parts = new List<MeshPart> { new MeshPart() };
        }

        /// <summary>
        /// Draws this CityMesh instance.
        /// </summary>
        /// <param name="Camera">A CityCamera instance.</param>
        public void Draw(CityCamera Camera)
        {
            if (!IsInitialized)
                return;

            m_Effect.CurrentTechnique = m_Effect.Techniques["BasicTechnique"];

            m_Effect.Parameters["World"].SetValue(Camera.WorldMatrix);
            m_Effect.Parameters["View"].SetValue(Camera.ViewMatrix);
            m_Effect.Parameters["Projection"].SetValue(Camera.ProjectionMatrix);

            foreach (var pass in m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (var part in m_Parts)
                {
                    if (part.VertexBuffer == null || part.IndexBuffer == null)
                        continue;

                    if (part.PrimitiveCount == 0)
                        continue;

                    m_GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                    m_GraphicsDevice.Indices = part.IndexBuffer;

                    m_GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        0,
                        0,
                        part.Vertices.Count,
                        part.StartIndex,
                        part.PrimitiveCount);
                }
            }
        }

        /// <summary>
        /// Requests a MeshPart instance.
        /// </summary>
        /// <returns>A MeshPart instance.</returns>
        private MeshPart RequestPart()
        {
            var Part = m_Parts.Last();

            if (Part.Vertices.Count >= 65536)
            {
                Part = new MeshPart();
                m_Parts.Add(Part);
            }

            return Part;
        }

        /// <summary>
        /// Adds a quad to this mesh.
        /// </summary>
        /// <param name="Vertices">An array of VertexPositionTexture instances to create the quad from.</param>
        public void AddQuad(VertexPositionTexture[] Vertices)
        {
            var part = RequestPart();

            part.Vertices.AddRange(Vertices);

            part.Indices.Add((ushort)part.IndexOffset);
            part.Indices.Add((ushort)(part.IndexOffset + 1));
            part.Indices.Add((ushort)(part.IndexOffset + 2));
            part.Indices.Add((ushort)part.IndexOffset);
            part.Indices.Add((ushort)(part.IndexOffset + 2));
            part.Indices.Add((ushort)(part.IndexOffset + 3));

            part.IndexOffset += 4;
            part.PrimitiveCount += 2;
        }

        /// <summary>
        /// Initializes this CityMesh instance's vertex and index buffers.
        /// </summary>
        public void InitBuffers()
        {
            if (IsInitialized)
                return;

            foreach (var part in m_Parts)
            {
                //Skip parts with no vertices or indices
                if (part.Vertices.Count == 0 || part.Indices.Count == 0)
                    continue;

                if (part.VertexBuffer == null)
                {
                    part.VertexBuffer = new VertexBuffer(m_GraphicsDevice, typeof(VertexPositionTexture), part.Vertices.Count, BufferUsage.WriteOnly);
                    part.VertexBuffer.SetData(part.Vertices.ToArray());
                }

                if (part.IndexBuffer == null)
                {
                    part.IndexBuffer = new IndexBuffer(m_GraphicsDevice, IndexElementSize.SixteenBits, part.Indices.Count, BufferUsage.WriteOnly);
                    part.IndexBuffer.SetData(part.Indices.ToArray());
                }
            }

            m_IsInitialized = true;
        }

        /// <summary>
        /// Clears vertex and index data from this mesh's parts.
        /// </summary>
        public void Clear()
        {
            foreach (var part in m_Parts)
            {
                if (part.Vertices != null)
                    part.Vertices.Clear();
                if (part.Indices != null)
                    part.Indices.Clear();

                part.VertexBuffer?.Dispose();
                part.VertexBuffer = null;

                part.IndexBuffer?.Dispose();
                part.IndexBuffer = null;

                part.IndexOffset = 0;
                part.PrimitiveCount = 0;
            }

            m_IsInitialized = false;
        }

        /// <summary>
        /// Prepares a CityMesh instance for reuse.
        /// </summary>
        public void Reset()
        {
            m_Parts = new List<MeshPart> { new MeshPart() };
            m_IsInitialized = false;
        }

        ~CityMesh()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                //Effect is managed elsewhere; do not dispose it here.

                if (m_Parts != null)
                {
                    foreach (var part in m_Parts)
                    {
                        if (part.VertexBuffer != null)
                        {
                            part.VertexBuffer.Dispose();
                            part.VertexBuffer = null;
                        }
                        if (part.IndexBuffer != null)
                        {
                            part.IndexBuffer.Dispose();
                            part.IndexBuffer = null;
                        }

                        if (part.Vertices != null)
                        {
                            part.Vertices.Clear();
                            part.Vertices = null;
                        }
                        if (part.Indices != null)
                        {
                            part.Indices.Clear();
                            part.Indices = null;
                        }
                    }

                    m_Parts.Clear();
                    m_Parts = null;
                }

                m_IsInitialized = false;

                m_GraphicsDevice = null;
                m_Effect = null;

                GC.SuppressFinalize(this);
            }
        }
    }
}
