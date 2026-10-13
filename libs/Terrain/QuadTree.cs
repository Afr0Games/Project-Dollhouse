/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terrain
{
    /// <summary>
    /// A quadtree.
    /// </summary>
    public class Quadtree
    {
        private MeshPool m_Pool;
        private QuadtreeNode m_Root;  //The root node of the quadtree
        private int m_MaxDepth;       //Maximum depth of the tree (limits subdivisions)
        private CityCamera m_Camera;
        private Color[] m_PixelData;
        private Texture2D m_HeightmapTex;

        public Color[] PixelData {  get { return m_PixelData; } }

        public int HeightmapWidth { get { return m_HeightmapTex.Width; } }
        public int HeightmapHeight { get { return m_HeightmapTex.Height; } }

        //Uniform per-frame LOD removes cracks between independently subdivided
        //quadtree nodes. Individual nodes still control visibility and mesh lifetime.
        public int MeshLOD { get; private set; } = 1;


        /// <summary>
        /// Gets this QuadTree's MeshPool instance.
        /// </summary>
        public MeshPool MeshPool { get { return m_Pool; } }

        /// <summary>
        /// The maximum depth of this quadtree instance.
        /// </summary>
        public int MaxDepth { get { return m_MaxDepth; } }

        /// <summary>
        /// This quadtree's camera.
        /// </summary>
        public CityCamera Camera { get { return m_Camera; } }

        /// <summary>
        /// Constructs a new QuadTree instance.
        /// </summary>
        /// <param name="TerrainBounds">The bounds of the terrain represented by this quad tree.</param>
        /// <param name="Terrain">The terrain itself.</param>
        /// <param name="MaxDepth">The maximum depth of this quad tree.</param>
        /// <param name="Camera">A <see cref="CityCamera"/> instance.</param>
        /// <param name="PixelData">The pixeldata from the heightmap.</param>
        /// <param name="HeightmapTex">The heightmap.</param>
        public Quadtree(Rectangle TerrainBounds, CityMesh Terrain, int MaxDepth, CityCamera Camera, Color[] PixelData, Texture2D HeightmapTex)
        {
            m_Pool = new MeshPool(Terrain.Device, Terrain.FX);
            m_Root = new QuadtreeNode(this, 0, 512, -512, 0, Terrain.Device, Terrain.FX, PixelData);
            m_MaxDepth = MaxDepth;
            m_Camera = Camera;
            m_PixelData = PixelData;
            m_HeightmapTex = HeightmapTex;
            //Supply the camera with the terrain-height lookup without coupling it to Quadtree.
            m_Camera.SampleTerrainHeight = TryGetTerrainHeight;
        }

        /// <summary>
        /// Renders this quad tree.
        /// </summary>
        public void Render()
        {
            Vector3 focus;
            float distance = m_Camera.TryGetFocus(out focus) ? Vector3.Distance(m_Camera.Position, focus) : Math.Abs(m_Camera.Position.Z);

            MeshLOD = distance < 160f ? 1 : distance < 350f ? 2 : distance < 700f ? 4 : 8;
            RenderNode(m_Root, 0);
        }

        /// <summary>
        /// Recursively renders the nodes in the quad tree.
        /// </summary>
        /// <param name="Node">A <see cref="QuadtreeNode"/> instance to start with (root node).</param>
        /// <param name="CurrentDepth">This tree's current depth.</param>
        private void RenderNode(QuadtreeNode Node, int CurrentDepth)
        {
            if (Node == null) 
                return;

            //Render this node
            Node.Render(m_Camera, CurrentDepth);
        }

        /// <summary>
        /// Zooms towards the actual heightmap surface at the center of the view.
        /// No changes to projection FOV or tile/mesh selection.
        /// </summary>
        public void Zoom(float Steps)
        {
            m_Camera.ZoomTowardsTerrain(Steps);
        }

        /// <summary>
        /// Keep the point at the center of the view inside the city diamond.
        /// Deliberately clamp the VIEW FOCUS, not the camera XYZ: a distant,
        /// tilted perspective camera normally sits outside the footprint.
        /// The camera and mesh elevation remain unchanged in Z.
        /// </summary>
        public void ConstrainCameraFocus()
        {
            // A terrain ray can switch from sea-level fallback to elevated land
            // after a move. Re-evaluate a few times to keep the focus in bounds.
            for (int attempt = 0; attempt < 3; ++attempt)
            {
                Vector3 Focus;

                if (!m_Camera.TryGetFocus(out Focus)) 
                    return;

                Vector2 Current = new Vector2(Focus.X, Focus.Y);
                Vector2 Clamped = CityViewBounds.ClampFocus(Current);
                Vector2 Correction = Clamped - Current;

                if (Correction.LengthSquared() < 0.0001f) 
                    return;

                m_Camera.Position += new Vector3(Correction.X, Correction.Y, 0f);
            }
        }

        /// <summary>
        /// Tries to get the height of the terrain.
        /// </summary>
        /// <param name="x">X coordinate of position.</param>
        /// <param name="y">Y coordinate of position.</param>
        /// <param name="height">Height at position.</param>
        /// <returns>True if it suceeded; false otherwise.</returns>
        private bool TryGetTerrainHeight(float X, float Y, out float Height)
        {
            Height = 0f;

            if (m_HeightmapTex == null || m_PixelData == null)
                return false;

            int ix = (int)Math.Floor(X);
            int iy = (int)Math.Floor(-Y);
            int Width = m_HeightmapTex.Width, Rows = m_HeightmapTex.Height;

            if (ix < 0 || ix >= Width || iy < 0 || iy >= Rows)
                return false;

            //Match the diamond-shaped city map mask used by QuadtreeNode.
            if (X < Math.Abs(306f + Y) || X > 511f - Math.Abs(205f + Y))
                return false;

            Height = m_PixelData[iy * Width + ix].R / 8f;

            return true;
        }
    }
}
