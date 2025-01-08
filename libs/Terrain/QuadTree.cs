/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;

namespace Terrain
{
    /// <summary>
    /// A quadtree.
    /// </summary>
    public class Quadtree
    {
        private MeshPool m_Pool;
        private QuadtreeNode m_Root;  //The root node of the quadtree
        private int m_MaxDepth;  //Maximum depth of the tree (limits subdivisions)
        private CityCamera m_Camera;
        private Color[] m_PixelData;

        public Color[] PixelData {  get { return m_PixelData; } }

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
        public Quadtree(Rectangle TerrainBounds, CityMesh Terrain, int MaxDepth, CityCamera Camera, Color[] PixelData)
        {
            m_Pool = new MeshPool(Terrain.Device, Terrain.FX);
            m_Root = new QuadtreeNode(this, 0, 512, -512, 0, Terrain.Device, Terrain.FX, PixelData);
            m_MaxDepth = MaxDepth;
            m_Camera = Camera;
            m_PixelData = PixelData;
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

            //If the node has children, render them recursively
            if (!Node.IsLeaf)
            {
                for (int i = 0; i < 4; i++)
                    RenderNode(Node.Children[i], CurrentDepth + 1);
            }
        }
    }
}
