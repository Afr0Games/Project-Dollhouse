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
            BoundingBox NodeBounds = new BoundingBox(
                new Vector3(m_MinX, m_MinY, -1000),
                new Vector3(m_MaxX, m_MaxY, 1000));

            if (!IsInFrustum(Camera, NodeBounds))
            {
                if (m_Terrain != null)
                {
                    m_Parent.MeshPool.ReturnMesh(m_Terrain);
                    m_Terrain = null;
                }

                return;
            }

            if (ShouldSubdivide(Camera, CurrentDepth))
            {
                Subdivide(CurrentDepth);

                foreach (var child in m_Children)
                    child.Render(Camera, CurrentDepth + 1);

                if (m_Terrain != null)
                {
                    m_Parent.MeshPool.ReturnMesh(m_Terrain);
                    m_Terrain = null;
                }
            }
            else
            {
                if (m_Terrain == null)
                    GenerateMesh();

                if (m_Terrain != null)
                    m_Terrain.Draw(Camera);
            }
        }

        /// <summary>
        /// Determines if the given coordinates are valid coordinates for reading height map data.
        /// </summary>
        /// <param name="X">The x coordinate to check.</param>
        /// <param name="Y">The y coordinate to check.</param>
        /// <returns>True if the coordinates are valid, false otherwise.</returns>
        private bool IsValidCoord(float X, float Y)
        {
            return (X >= Math.Abs(306 + Y) && X <= 511 - Math.Abs(205 + Y));
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

            //Subdivide if the node is close enough
            float subdivisionThreshold = 512f * (CurrentDepth + 1);
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
            float distanceToCamera = Vector3.Distance(m_Parent.Camera.Position, BoundsCenter);
            int lod = DetermineLOD(distanceToCamera);

            for (int y = m_MaxY - 1; y >= m_MinY; y -= lod)
            {
                for (int x = m_MinX; x < m_MaxX; x += lod)
                {
                    if (!IsValidCoord(x, y))
                        continue;

                    float z = m_Parent.PixelData[512 * (-y) + x].R / 8.0f;

                    AddQuadWithTexCoords(x, y, z, m_Terrain);

                    //Right quad (if needed)
                    if (x < 511 && IsValidCoord(x + 1, y))
                    {
                        float zr = m_Parent.PixelData[512 * (-y) + (x + 1)].R / 8.0f;
                        AddVerticalQuad(x, y, z, x + 1, zr);
                    }

                    //Bottom quad (if needed)
                    if (y > -511 && IsValidCoord(x, y - 1))
                    {
                        float zb = m_Parent.PixelData[512 * (-(y - 1)) + x].R / 8.0f;
                        AddHorizontalQuad(x, y, z, y - 1, zb);
                    }
                }
            }

            if (m_Terrain.VertexCount == 0)
            {
                //No vertices were added; dispose of the mesh
                m_Parent.MeshPool.ReturnMesh(m_Terrain);
                m_Terrain = null;

                return;
            }

            m_Terrain.InitBuffers();
        }

        /// <summary>
        /// Determines the Level Of Detail for a generated mesh based on the camera's distance.
        /// </summary>
        /// <param name="DistanceToCamera">How far away is the camera?</param>
        /// <returns></returns>
        private int DetermineLOD(float DistanceToCamera)
        {
            if (DistanceToCamera < 500f)
                return 1; //Highest detail
            else if (DistanceToCamera < 1500f)
                return 2;
            else if (DistanceToCamera < 3000f)
                return 4;
            else
                return 8; //Lower detail
        }

        /// <summary>
        /// Adds a quad with texture coordinates at the given coordinates.
        /// </summary>
        /// <param name="X">The x coordinate.</param>
        /// <param name="Y">The y coordinate.</param>
        /// <param name="Z">The z coordinate.</param>
        /// <param name="Mesh">A <see cref="CityMesh"/> instance.</param>
        private void AddQuadWithTexCoords(int X, int Y, float Z, CityMesh Mesh)
        {
            //Each valid coordinate will generate 4 smaller quads
            float halfSize = 0.5f; //Half the size for the smaller quads

            //Loop to generate 4 smaller quads (2x2) for each valid coord
            for (int i = 0; i < 2; i++) //For top-left and top-right smaller quads
            {
                for (int j = 0; j < 2; j++) //For bottom-left and bottom-right smaller quads
                {
                    //Calculate the offsets for the smaller quads
                    float offsetX = i * halfSize;
                    float offsetY = j * halfSize;

                    float u1 = (X + offsetX) / 512f;
                    float v1 = -(Y + offsetY) / 512f;
                    float u2 = (X + offsetX + halfSize) / 512f;
                    float v2 = -(Y + offsetY + halfSize) / 512f;

                    //Generate 4 smaller quads to cover each valid coord's space
                    Mesh.AddQuad(new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(new Vector3(X + offsetX, Y + offsetY, Z), new Vector2(u1, v1)), //Bottom-left
                        new VertexPositionTexture(new Vector3(X + offsetX + halfSize, Y + offsetY, Z), new Vector2(u2, v1)), //Bottom-right
                        new VertexPositionTexture(new Vector3(X + offsetX + halfSize, Y + offsetY - halfSize, Z), new Vector2(u2, v2)), //Top-right
                        new VertexPositionTexture(new Vector3(X + offsetX, Y + offsetY - halfSize, Z), new Vector2(u1, v2)) //Top-left
                    });
                }
            }
        }

        /// <summary>
        /// Adds a vertical quad.
        /// </summary>
        /// <param name="X1">The first x coord.</param>
        /// <param name="Y">The y coord.</param>
        /// <param name="Z1">The first z coord.</param>
        /// <param name="X2">The second x coord.</param>
        /// <param name="Z2">The second z coord.</param>
        private void AddVerticalQuad(float X1, float Y, float Z1, int X2, float Z2)
        {
            float U1 = X1 / 512f;
            float V1 = -Y / 512f;
            float U2 = X2 / 512f;
            float V2 = (-Y - 1) / 512f;

            m_Terrain.AddQuad(new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(X1, Y, Z1), new Vector2(U1, V1)),
                new VertexPositionTexture(new Vector3(X2, Y, Z2), new Vector2(U2, V1)),
                new VertexPositionTexture(new Vector3(X2, Y - 1, Z2), new Vector2(U2, V2)),
                new VertexPositionTexture(new Vector3(X1, Y - 1, Z1), new Vector2(U1, V2))
            });
        }

        /// <summary>
        /// Adds a horizontal quad.
        /// </summary>
        /// <param name="X">The x coord.</param>
        /// <param name="Y1">The first y coord.</param>
        /// <param name="Z1">The first z coord.</param>
        /// <param name="Y2">The second y coord.</param>
        /// <param name="Z2">The second z coord.</param>
        private void AddHorizontalQuad(float X, float Y1, float Z1, int Y2, float Z2)
        {
            float u1 = X / 512f;
            float v1 = -Y1 / 512f;
            float u2 = (X + 1) / 512f;
            float v2 = -Y2 / 512f;

            m_Terrain.AddQuad(new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(X, Y1, Z1), new Vector2(u1, v1)),
                new VertexPositionTexture(new Vector3(X + 1, Y1, Z1), new Vector2(u2, v1)),
                new VertexPositionTexture(new Vector3(X + 1, Y2, Z2), new Vector2(u2, v2)),
                new VertexPositionTexture(new Vector3(X, Y2, Z2), new Vector2(u1, v2))
            });
        }
    }
}
