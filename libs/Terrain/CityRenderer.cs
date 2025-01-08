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
using System.Diagnostics;

namespace Terrain
{
    /// <summary>
    /// Renderer for the city mesh.
    /// </summary>
    public class CityRenderer : IDisposable
    {
        GraphicsDevice m_Device;

        private CityCamera m_Camera;
        private Vector3 m_CameraTarget;

        private CityMesh m_CityMesh;
        private TextureAtlas m_Atlas;

        private Color[] m_HeightMapData;
        private float m_DynamicAspectRatio;

        public CityRenderer(GraphicsDevice Device)
        {
            m_Device = Device;
            m_DynamicAspectRatio = m_Device.Viewport.Width / (float)m_Device.Viewport.Height;
            m_Camera = new CityCamera(m_DynamicAspectRatio);
        }

        /// <summary>
        /// Generates the city mesh that this renderer will render.
        /// TODO: Async?
        /// </summary>
        public void GenerateMesh()
        {
            //TODO: Create the mesh instance?

            for (int y = 0; y > -512; y--)
            {
                for (int x = 0; x < 512; x++)
                {
                    if (!ValidCoord(x, y)) continue;

                    float z = m_HeightMapData[512 * (-y) + x].R / 8.0f;

                    //Main quad
                    AddQuadWithTexCoords(x, y, z);

                    //Right quad (if needed)
                    if (x < 511 && ValidCoord(x + 1, y))
                    {
                        float zr = m_HeightMapData[512 * (-y) + (x + 1)].R / 8.0f;
                        AddVerticalQuad(x, y, z, x + 1, zr);
                    }

                    //Bottom quad (if needed)
                    if (y > -511 && ValidCoord(x, y - 1))
                    {
                        float zb = m_HeightMapData[512 * (-(y - 1)) + x].R / 8.0f;
                        AddHorizontalQuad(x, y, z, y - 1, zb);
                    }
                }
            }

            m_CityMesh.InitBuffers();
        }

        /// <summary>
        /// Is the coordinate valid?
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private bool ValidCoord(float x, float y)
        {
            return (x >= Math.Abs(306 + y) && x <= 511 - Math.Abs(205 + y));
        }

        /// <summary>
        /// Adds a quad with texture coordinates to this renderer's mesh at the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        private void AddQuadWithTexCoords(int x, int y, float z)
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

                    float u1 = (x + offsetX) / 512f;
                    float v1 = -(y + offsetY) / 512f;
                    float u2 = (x + offsetX + halfSize) / 512f;
                    float v2 = -(y + offsetY + halfSize) / 512f;

                    //Generate 4 smaller quads to cover each valid coord's space
                    m_CityMesh.AddQuad(new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(new Vector3(x + offsetX, y + offsetY, z), new Vector2(u1, v1)), //Bottom-left
                        new VertexPositionTexture(new Vector3(x + offsetX + halfSize, y + offsetY, z), new Vector2(u2, v1)), //Bottom-right
                        new VertexPositionTexture(new Vector3(x + offsetX + halfSize, y + offsetY - halfSize, z), new Vector2(u2, v2)), //Top-right
                        new VertexPositionTexture(new Vector3(x + offsetX, y + offsetY - halfSize, z), new Vector2(u1, v2)) //Top-left
                    });
                }
            }
        }

        /// <summary>
        /// Adds a vertical quad with texture coordinates to this renderer's mesh at the specified coordinates.
        /// </summary>
        /// <param name="x1">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z1">Z coordinate.</param>
        /// <param name="x2">Texture coordinate 1.</param>
        /// <param name="z2">Texture coordinate 2.</param>
        private void AddVerticalQuad(float x1, float y, float z1, float x2, float z2)
        {
            float u1 = x1 / 512f;
            float v1 = -y / 512f;
            float u2 = x2 / 512f;
            float v2 = (-y - 1) / 512f;

            m_CityMesh.AddQuad(new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(x1, y, z1), new Vector2(u1, v1)),
                new VertexPositionTexture(new Vector3(x2, y, z2), new Vector2(u2, v1)),
                new VertexPositionTexture(new Vector3(x2, y - 1, z2), new Vector2(u2, v2)),
                new VertexPositionTexture(new Vector3(x1, y - 1, z1), new Vector2(u1, v2))
            });
        }

        /// <summary>
        /// Adds a horizontal quad with texture coordinates to this renderer's mesh at the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y1">Y coordinate.</param>
        /// <param name="z1">Z coordinate.</param>
        /// <param name="y2">Texture coordinate 1.</param>
        /// <param name="z2">Texture coordinate 2.</param>
        private void AddHorizontalQuad(float x, float y1, float z1, int y2, float z2)
        {
            float u1 = x / 512f;
            float v1 = -y1 / 512f;
            float u2 = (x + 1) / 512f;
            float v2 = -y2 / 512f;

            m_CityMesh.AddQuad(new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(x, y1, z1), new Vector2(u1, v1)),
                new VertexPositionTexture(new Vector3(x + 1, y1, z1), new Vector2(u2, v1)),
                new VertexPositionTexture(new Vector3(x + 1, y2, z2), new Vector2(u2, v2)),
                new VertexPositionTexture(new Vector3(x, y2, z2), new Vector2(u1, v2))
            });
        }

        protected void Draw()
        {
            m_Device.Clear(Color.CornflowerBlue);

            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(m_Camera.Yaw, m_Camera.Pitch, m_Camera.Roll);
            Vector3 transformedForward = Vector3.Transform(Vector3.Forward, cameraRotation);
            Vector3 cameraTarget = m_Camera.Position + transformedForward;

            //Compute matrices
            Matrix worldMatrix = Matrix.Identity;

            Matrix viewMatrix = Matrix.CreateLookAt(m_Camera.Position, cameraTarget, Vector3.Up);
            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), m_DynamicAspectRatio, 0.1f, 4096f);

            m_Camera.ViewMatrix = viewMatrix;

            //Draw the city mesh with the matrices
            m_CityMesh.Draw(m_Camera);
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~CityRenderer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this CityRenderer instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this CityRenderer instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_CityMesh != null)
                    m_CityMesh.Dispose();

                //Prevent the finalizer from calling ~CityRenderer, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                Debug.WriteLine("CityRenderer not explicitly disposed!");
        }
    }
}
