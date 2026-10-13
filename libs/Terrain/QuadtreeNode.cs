/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terrain
{
    /// <summary>
    /// A node in a <see cref="Quadtree"/>.
    /// </summary>
    public class QuadtreeNode
    {
        private Quadtree m_Parent;
        private int m_MinX, m_MaxX, m_MinY, m_MaxY;
        private QuadtreeNode[] m_Children;
        private bool m_IsLeaf;
        private bool m_IsSubDivided = false;
        private CityMesh m_Terrain;
        private int m_GeneratedLOD = 0;

        private GraphicsDevice m_GraphicsDevice;
        private Effect m_Effect;
        private Color[] m_PixelData;

        /// <summary>
        /// The center of this QuadTreeNode instance.
        /// </summary>
        public Vector3 BoundsCenter
        {
            get
            {
                float CenterX = (m_MinX + m_MaxX) / 2.0f;
                float CenterY = (m_MinY + m_MaxY) / 2.0f;
                return new Vector3(CenterX, CenterY, 0);
            }
        }

        /// <summary>
        /// This node's children.
        /// </summary>
        public QuadtreeNode[] Children { get { return m_Children; } }

        /// <summary>
        /// Is this node a leaf?
        /// </summary>
        public bool IsLeaf { get { return m_IsLeaf; } }

        public QuadtreeNode(Quadtree Parent, int MinX, int MaxX, int MinY, int MaxY, 
            GraphicsDevice graphicsDevice, Effect effect, Color[] PixelData)
        {
            m_Parent = Parent;
            m_MinX = MinX;
            m_MaxX = MaxX;
            m_MinY = MinY;
            m_MaxY = MaxY;
            m_GraphicsDevice = graphicsDevice;
            m_Effect = effect;
            m_PixelData = PixelData;

            m_IsLeaf = true;
            m_IsSubDivided = false;
        }

        /// <summary>
        /// Subdivides this quadtree node.
        /// </summary>
        /// <param name="CurrentDepth">The current depth of the <see cref="Quadtree"/>.</param>
        private void Subdivide(int CurrentDepth)
        {
            if (m_IsSubDivided)
                return;

            int midX = (m_MinX + m_MaxX) / 2;
            int midY = (m_MinY + m_MaxY) / 2;

            m_Children = new QuadtreeNode[4];
            m_Children[0] = new QuadtreeNode(
                m_Parent, m_MinX, midX, midY, m_MaxY, m_GraphicsDevice, m_Effect, m_PixelData);
            m_Children[1] = new QuadtreeNode(
                m_Parent, midX, m_MaxX, midY, m_MaxY, m_GraphicsDevice, m_Effect, m_PixelData);
            m_Children[2] = new QuadtreeNode(
                m_Parent, m_MinX, midX, m_MinY, midY, m_GraphicsDevice, m_Effect, m_PixelData);
            m_Children[3] = new QuadtreeNode(
                m_Parent, midX, m_MaxX, m_MinY, midY, m_GraphicsDevice, m_Effect, m_PixelData);

            m_IsLeaf = false;
            m_IsSubDivided = true;

            //Dispose of this node's mesh since it's subdivided
            if (m_Terrain != null)
            {
                m_Parent.MeshPool.ReturnMesh(m_Terrain);
                m_Terrain = null;
                    m_GeneratedLOD = 0;
            }
        }

        /// <summary>
        /// Renders this QuadTreeNode instance.
        /// </summary>
        /// <param name="Camera">A <see cref="CityCamera"/> instance.</param>
        /// <param name="CurrentDepth">The current depth of the <see cref="Quadtree"/>.</param>
        public void Render(CityCamera Camera, int CurrentDepth)
        {
            //Calculate the bounding box for frustum culling
            BoundingBox NodeBounds = new BoundingBox(new Vector3(m_MinX, m_MinY, 0f), new Vector3(m_MaxX, m_MaxY, 32f));

            if (!IsInFrustum(Camera, NodeBounds))
            {
                if (m_Terrain != null)
                {
                    m_Parent.MeshPool.ReturnMesh(m_Terrain);
                    m_Terrain = null;
                    m_GeneratedLOD = 0;
                }

                return;
            }

            if (ShouldSubdivide(Camera, CurrentDepth))
            {
                Subdivide(CurrentDepth);

                foreach (QuadtreeNode Child in m_Children)
                    Child.Render(Camera, CurrentDepth + 1);

                if (m_Terrain != null)
                {
                    m_Parent.MeshPool.ReturnMesh(m_Terrain);
                    m_Terrain = null;
                    m_GeneratedLOD = 0;
                }
            }
            else
            {
                int DesiredLOD = m_Parent.MeshLOD;
                if (m_Terrain != null && DesiredLOD != m_GeneratedLOD)
                {
                    m_Parent.MeshPool.ReturnMesh(m_Terrain);
                    m_Terrain = null;
                    m_GeneratedLOD = 0;
                }
                if (m_Terrain == null)
                    GenerateMesh();

                if (m_Terrain != null)
                    m_Terrain.Draw(Camera);
            }
        }

        /// <summary>
        /// Checks if a bounding box is inside the camera's view(ing frustum).
        /// </summary>
        /// <param name="Camera">A <see cref="CityCamera"/> instance.</param>
        /// <param name="Bounds">A bounding box to check for.</param>
        /// <returns></returns>
        private bool IsInFrustum(CityCamera Camera, BoundingBox Bounds)
        {
            Matrix ViewProjMatrix = Camera.ViewMatrix * Camera.ProjectionMatrix;
            BoundingFrustum Frustum = new BoundingFrustum(ViewProjMatrix);

            return Frustum.Intersects(Bounds);
        }

        /// <summary>
        /// Should this node be subdivided?
        /// </summary>
        /// <param name="Camera">A <see cref="CityCamera"/> instance.</param>
        /// <param name="CurrentDepth">The current depth of the <see cref="Quadtree"/>.</param>
        /// <returns></returns>
        private bool ShouldSubdivide(CityCamera Camera, int CurrentDepth)
        {
            if (CurrentDepth >= m_Parent.MaxDepth)
                return false;

            float distance = Vector3.Distance(Camera.Position, BoundsCenter);

            //Threshold shrinks with depth - close nodes subdivide further, distant ones don't.
            float subdivisionThreshold = 2048f / (float)Math.Pow(2, CurrentDepth);
            return distance < subdivisionThreshold;
        }

        /// <summary>
        /// Generates a mesh.
        /// </summary>
        private void GenerateMesh()
        {
            if (m_Terrain != null)
                return;

            m_Terrain = m_Parent.MeshPool.GetMesh();
            int lod = m_Parent.MeshLOD;
            m_GeneratedLOD = lod;

            //Generate a continuous heightfield instead of disconnected flat tile
            //tops plus vertical walls. Roads and terrain use the same XY positions,
            //so they naturally follow the slopes rather than stretching down walls.
            //
            //All visible quadtree nodes use the SAME LOD for this frame. Their
            //shared border vertices therefore coincide without T-junction cracks.
            for (int y = m_MaxY; y > m_MinY; y -= lod)
            {
                for (int x = m_MinX; x < m_MaxX; x += lod)
                    AddTerrainCell(x, y, lod);
            }

            if (m_Terrain.VertexCount == 0)
            {
                //No vertices were added; dispose of the mesh
                m_Parent.MeshPool.ReturnMesh(m_Terrain);
                m_Terrain = null;
                m_GeneratedLOD = 0;

                return;
            }

            m_Terrain.InitBuffers();
        }

        /// <summary>
        /// City boundary: 306 <= x-y <= 716, -306 <= x+y <= 306.
        /// Clip each terrain cell against this convex diamond. Skipping a whole
        /// cell based on its top-left vertex leaves visible triangular holes.
        /// Clipping also prevents out-of-map quads from overhanging the coast.
        /// </summary>
        /// <param name="X">The x position of the cell to add.</param>
        /// <param name="Y">The y position of the cell to add.</param>
        /// <param name="LOD">THe level of detail of the cell.</param>
        private void AddTerrainCell(int X, int Y, int LOD)
        {
            //The diamond is the intersection of four half-planes. Since
            //distance to each plane is linear, checking its min/max over the
            //rectangular cell is sufficient to reject or accept it cheaply.
            const float Epsilon = 1e-5f;
            if (X - Y + 2 * LOD < 306f - Epsilon ||
                716f - X + Y < -Epsilon ||
                X + Y + LOD < -306f - Epsilon ||
                306f - X - Y + LOD < -Epsilon)
                return;

            bool allInside =
                X - Y >= 306f - Epsilon &&
                716f - X + Y - 2 * LOD >= -Epsilon &&
                X + Y - LOD >= -306f - Epsilon &&
                306f - X - Y - LOD >= -Epsilon;

            var v0 = TerrainVertex(X,       Y);
            var v1 = TerrainVertex(X + LOD, Y);
            var v2 = TerrainVertex(X + LOD, Y - LOD);
            var v3 = TerrainVertex(X,       Y - LOD);

            if (allInside)
            {
                m_Terrain.AddQuad(new[] { v0, v1, v2, v3 });
                return;
            }

            var Polygon = new List<VertexPositionTexture>(4)
            {
                v0, v1, v2, v3
            };

            for (int Plane = 0; Plane < 4; ++Plane)
            {
                Polygon = ClipToCity(Polygon, Plane);

                if (Polygon.Count < 3) 
                    return;
            }

            //A clipped quad can still have four vertices. It may also have
            //more than four; triangles are the common case near a corner.
            for (int i = 1; i + 1 < Polygon.Count; ++i)
            {
                var a = Polygon[0];
                var b = Polygon[i];
                var c = Polygon[i + 1];
                float TwiceArea = (b.Position.X - a.Position.X) * (c.Position.Y - a.Position.Y)
                                - (b.Position.Y - a.Position.Y) * (c.Position.X - a.Position.X);
                
                if (Math.Abs(TwiceArea) > 1e-5f)
                    m_Terrain.AddTriangle(a, b, c);
            }
        }

        private VertexPositionTexture TerrainVertex(int X, int Y)
        {
            int Width = m_Parent.HeightmapWidth;
            int Height = m_Parent.HeightmapHeight;
            int sx = Math.Max(0, Math.Min(Width - 1, X));
            int sy = Math.Max(0, Math.Min(Height - 1, -Y));
            float z = m_PixelData[sy * Width + sx].R / 8.0f;

            return new VertexPositionTexture(new Vector3(X, Y, z), new Vector2(X / (float)Width, -Y / (float)Height));
        }

        private static float CityPlaneDistance(Vector3 P, int Plane)
        {
            switch (Plane)
            {
                case 0: 
                    return P.X - P.Y - 306f;
                case 1: 
                    return 716f - P.X + P.Y;
                case 2: 
                    return P.X + P.Y + 306f;
                default: 
                    return 306f - P.X - P.Y;
            }
        }

        private static List<VertexPositionTexture> ClipToCity(List<VertexPositionTexture> Polygon, int Plane)
        {
            List<VertexPositionTexture> Output = new List<VertexPositionTexture>(Polygon.Count + 1);

            if (Polygon.Count == 0)
                return Output;

            VertexPositionTexture Previous = Polygon[Polygon.Count - 1];
            float PrevDistance = CityPlaneDistance(Previous.Position, Plane);
            bool PrevInside = PrevDistance >= -1e-5f;

            foreach (VertexPositionTexture current in Polygon)
            {
                float Distance = CityPlaneDistance(current.Position, Plane);
                bool Inside = Distance >= -1e-5f;

                if (PrevInside != Inside)
                {
                    float t = PrevDistance / (PrevDistance - Distance);
                    Output.Add(new VertexPositionTexture(
                        Vector3.Lerp(Previous.Position, current.Position, t),
                        Vector2.Lerp(Previous.TextureCoordinate, current.TextureCoordinate, t)));
                }
                if (Inside)
                    Output.Add(current);

                Previous = current;
                PrevDistance = Distance;
                PrevInside = Inside;
            }

            return Output;
        }
    }
}
