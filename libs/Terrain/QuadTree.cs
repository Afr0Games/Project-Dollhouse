/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        public Color[] PixelData { get { return m_PixelData; } }

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
            RenderNode(m_Root, 0);
        }

        /// <summary>
        /// Recursively renders the nodes in the quad tree.
        /// </summary>
        /// <param name="Node">A <see cref="QuadtreeNode"/> instance to start with (root node).</param>
        /// <param name="CurrentDepth">This tree's current depth.</param>
        private void RenderNode(QuadtreeNode Node, int CurrentDepth)
        {
            if (Node == null) return;

            //Render this node
            Node.Render(m_Camera, CurrentDepth);
        }

        /// <summary>
        /// Zooms towards the actual heightmap surface at the center of the view.
        /// No changes to projection FOV or tile/mesh selection.
        /// </summary>
        public void Zoom(float steps)
        {
            m_Camera.ZoomTowardsTerrain(steps);
        }

        /// <summary>
        /// Tries to get the height of the terrain.
        /// </summary>
        /// <param name="x">X coordinate of position.</param>
        /// <param name="y">Y coordinate of position.</param>
        /// <param name="height">Height at position.</param>
        /// <returns>True if it suceeded; false otherwise.</returns>
        private bool TryGetTerrainHeight(float x, float y, out float height)
        {
            height = 0f;

            if (m_HeightmapTex == null || m_PixelData == null)
                return false;

            int ix = (int)Math.Floor(x);
            int iy = (int)Math.Floor(-y);
            int width = m_HeightmapTex.Width, rows = m_HeightmapTex.Height;

            if (ix < 0 || ix >= width || iy < 0 || iy >= rows)
                return false;

            //Match the diamond-shaped city map mask used by QuadtreeNode.
            if (x < Math.Abs(306f + y) || x > 511f - Math.Abs(205f + y))
                return false;

            height = m_PixelData[iy * width + ix].R / 8f;

            return true;
        }
    }
}
